using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.BLL.Services;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using BookStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient.DataClassification;

namespace ServerApp.BLL.Services;
public interface IBookService : IBaseService<Book>
{
    Task<IEnumerable<BookVm>> GetAllBookAsync();
    Task<IEnumerable<BookVm>> FilterBooksAsync(FilterRequest filterRequest);
    Task<BookVm> AddBookAsync(InputBookVm categoryVm);
    Task<BookVm> UpdateBookAsync(int id, InputBookVm categoryVm);
    Task<int> UpdateBookAsync(Book book);
    Task<IEnumerable<BookVm>> GetBooksByCategoryAsync(int categoryId);
    Task<IEnumerable<BookVm>> GetBooksByNameAsync(string name);
    Task<BookVm> DeleteBookAsync(int id);
    Task<BookDetailVm> GetBookDetailsAsync(int bookId);

    Task<BookVm?> GetByBookIdAsync(int id);
    Task<IEnumerable<BookInformationVm>> GetBookInformationsByBookIdAsync(int bookId);
    Task<bool> AddBookToCartAsync(int booktId, CartVm cartVm);
    Task<IEnumerable<BookVm>> GetNewestBooksAsync();
    Task<PagedResult<BookVm>> GetAllBookAsync(int? pageNumber, int? pageSize);
}
public class BookService : BaseService<Book>, IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInformationTypeService _informationTypeService;

    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<Category> _categoryRepository;


    public BookService(IUnitOfWork unitOfWork, IInformationTypeService informationTypeService) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _informationTypeService = informationTypeService;
        _categoryRepository = unitOfWork.GenericRepository<Category>();
    }

    private async Task<List<BookInformation>> ProcessInformationTypesAsync(
IEnumerable<InputBookInformationVm> bookInformations)
    {
        var bookInformationsToAdd = new List<BookInformation>();

        foreach (var spec in bookInformations)
        {
            var informationType = await _informationTypeService.GetByInformationTypeIdAsync(spec.InformationTypeId);

            var informationTypeVm = new InputInformationTypeVm
            {
                Code = spec.InformationType.Code
            };

            if (informationType != null)
            {
                informationType = await _informationTypeService.UpdateInformationTypeAsync(spec.InformationTypeId, informationTypeVm);
            }
            else
            {
                informationType = await _informationTypeService.AddInformationTypeAsync(informationTypeVm);
            }

            if (informationType == null)
            {
                throw new ArgumentException($"Không thể xử lý thông tin: {spec.InformationType.Code}");
            }

            bookInformationsToAdd.Add(new BookInformation
            {
                InformationTypeId = informationType.InformationTypeId,
                Publish = spec.Publish
            });
        }

        return bookInformationsToAdd;
    }
    private async Task<List<BookInformation>> ProcessAndSyncInformationsAsync(int bookId, IEnumerable<InputBookInformationVm> bookInformations)
    {
        var bookInformationsToAddOrUpdate = new List<BookInformation>();

        var existingBookInformations = await _unitOfWork.GenericRepository<BookInformation>()
            .GetAllAsync(ps => ps.BookId == bookId);

        foreach (var spec in bookInformations)
        {
            var informationType = await _informationTypeService.GetByInformationTypeIdAsync(spec.InformationTypeId);

            var informationTypeVm = new InputInformationTypeVm
            {
                Code = spec.InformationType.Code
            };

            if (informationType != null)
            {
                informationType = await _informationTypeService.UpdateInformationTypeAsync(spec.InformationTypeId, informationTypeVm);
            }
            else
            {
                informationType = await _informationTypeService.AddInformationTypeAsync(informationTypeVm);
            }

            if (informationType == null)
            {
                throw new ArgumentException($"Không thể xử lý thông tin: {spec.InformationType.Code}");
            }

            var existingBookSpec = existingBookInformations
                .FirstOrDefault(ps => ps.InformationTypeId == informationType.InformationTypeId);

            if (existingBookSpec != null)
            {
                existingBookSpec.Publish = spec.Publish;
                bookInformationsToAddOrUpdate.Add(existingBookSpec);
            }
            else
            {
                var newBookInformation = new BookInformation
                {
                    BookId = bookId,
                    InformationTypeId = informationType.InformationTypeId,
                    Publish = spec.Publish
                };
                bookInformationsToAddOrUpdate.Add(newBookInformation);
            }
        }

        var bookInformationsToDelete = existingBookInformations
            .Where(ps => !bookInformations.Any(spec => spec.InformationTypeId == ps.InformationTypeId))
            .ToList();

        foreach (var bookInformation in bookInformationsToDelete)
        {
            _unitOfWork.GenericRepository<BookInformation>().Delete(bookInformation);
        }
        return bookInformationsToAddOrUpdate;
    }

    public async Task<IEnumerable<BookVm>> GetBooksByCategoryAsync(int categoryId)
    {
        var books = await _unitOfWork.GenericRepository<Book>()
            .GetAllAsync(b => b.CategoryId == categoryId);

        var bookVms = books.Select(b => new BookVm
        {
            BookId = b.BookId,
            Name = b.Name,
            Description = b.Description,
            Price = b.Price,
            OldPrice = b.OldPrice,
            StockQuantity = b.StockQuantity,
            CategoryId = b.CategoryId,
            ImageUrl = b.ImageUrl,
            Author = b.Author,
            Discount = b.Discount,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            Category = b.Category != null ? new CategoryVm
            {
                CategoryId = b.Category.CategoryId,
                Name = b.Category.Name
            } : null,
            BookInformations = b.BookInformations?.Select(bi => new BookInformationVm
            {
                BookId = bi.BookId,
                Publish = bi.Publish
            }).ToList()
        }).ToList();

        return bookVms;
    }


    public async Task<BookVm> AddBookAsync(InputBookVm bookVm)
    {
        ValidateModelPropertiesWithAttribute(bookVm);

        var findBook = await _unitOfWork.GenericRepository<Book>().GetAsync(p =>
            p.Name == bookVm.Name
        );

        if (findBook != null)
        {
            throw new ExceptionBusinessLogic("Book name đã được sử dụng.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var bookInformationsToAdd = await ProcessInformationTypesAsync(bookVm.BookInformations);

            var book = new Book
            {
                Name = bookVm.Name,
                Description = bookVm.Description,
                Price = bookVm.Price,
                OldPrice = bookVm.OldPrice,
                StockQuantity = bookVm.StockQuantity,
                CategoryId = bookVm.CategoryId,
                ImageUrl = bookVm.ImageUrl,
                Author = bookVm.Author,
                Discount = bookVm.Discount
            };

            await _unitOfWork.GenericRepository<Book>().AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            foreach (var bookInformation in bookInformationsToAdd)
            {
                bookInformation.BookId = book.BookId;
                await _unitOfWork.GenericRepository<BookInformation>().AddAsync(bookInformation);
            }

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            return new BookVm
            {
                BookId = book.BookId,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                OldPrice = book.OldPrice,
                StockQuantity = book.StockQuantity,
                CategoryId = book.CategoryId,
                ImageUrl = book.ImageUrl,
                Author = book.Author,
                Discount = book.Discount,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");

            await _unitOfWork.RollbackTransactionAsync();

            throw new ArgumentException($"{ex.Message}", ex);
        }
    }


    public async Task<BookVm> DeleteBookAsync(int id)
    {
        var book = await _unitOfWork.GenericRepository<Book>().GetByIdAsync(id);

        if (book == null)
        {
            throw new ExceptionNotFound("Book không tìm thấy");
        }

        _unitOfWork.GenericRepository<Book>().Delete(book);
        var isdelete = await _unitOfWork.SaveChangesAsync();

        if (isdelete > 0)
        {
            return new BookVm
            {
                BookId = book.BookId,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                OldPrice = book.OldPrice,
                StockQuantity = book.StockQuantity,
                CategoryId = book.CategoryId,
                ImageUrl = book.ImageUrl,
                Author = book.Author,
                Discount = book.Discount,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
            };
        }
        throw new ArgumentException("Không thể xóa book");
    }

    public async Task<IEnumerable<BookVm>> GetAllBookAsync()
    {
        var resuilt = await GetAllAsync(
                includesProperties: "Category,BookInformations,BookInformations.InformationType"
            );
        var bookViewModels = resuilt.Select(book => new BookVm
        {
            BookId = book.BookId,
            Name = book.Name,
            Description = book.Description,
            Price = book.Price,
            OldPrice = book.OldPrice,
            StockQuantity = book.StockQuantity,
            CategoryId = book.CategoryId,
            ImageUrl = book.ImageUrl,
            Author = book.Author,
            Discount = book.Discount,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            Category = new CategoryVm
            {
                CategoryId = book.Category.CategoryId,
                Name = book.Category.Name,
                ImageUrl = book.Category.ImageUrl,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.Category.UpdatedAt
            }, 
            BookInformations = book.BookInformations.Select(ps => new BookInformationVm
            {
                BookId = ps.BookId,
                InformationTypeId = ps.InformationTypeId,
                Publish = ps.Publish,
                CreatedAt = ps.CreatedAt,
                UpdatedAt = ps.UpdatedAt,
                InformationType = new InformationTypeVm
                {
                    InformationTypeId = ps.InformationType.InformationTypeId,
                    Code = ps.InformationType.Code,
                    CreatedAt = ps.InformationType.CreatedAt,
                    UpdatedAt = ps.InformationType.UpdatedAt,
                }
            }).ToList()
        });

        return bookViewModels;
    }

    public async Task<IEnumerable<BookVm>> GetBooksByNameAsync(string name)
    {
        var result = await GetAllAsync(
            filter: b => b.Name.ToLower().Contains(name.ToLower()),
            includesProperties: "Category,BookInformations,BookInformations.InformationType"
        );

        var bookViewModels = result.Select(book => new BookVm
        {
            BookId = book.BookId,
            Name = book.Name,
            Description = book.Description,
            Price = book.Price,
            OldPrice = book.OldPrice,
            StockQuantity = book.StockQuantity,
            CategoryId = book.CategoryId,
            ImageUrl = book.ImageUrl,
            Author = book.Author,
            Discount = book.Discount,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            Category = new CategoryVm
            {
                CategoryId = book.Category.CategoryId,
                Name = book.Category.Name,
                ImageUrl = book.Category.ImageUrl,
                CreatedAt = book.Category.CreatedAt,
                UpdatedAt = book.Category.UpdatedAt
            },
            BookInformations = book.BookInformations.Select(ps => new BookInformationVm
            {
                BookId = ps.BookId,
                InformationTypeId = ps.InformationTypeId,
                Publish = ps.Publish,
                CreatedAt = ps.CreatedAt,
                UpdatedAt = ps.UpdatedAt,
                InformationType = new InformationTypeVm
                {
                    InformationTypeId = ps.InformationType.InformationTypeId,
                    Code = ps.InformationType.Code,
                    CreatedAt = ps.InformationType.CreatedAt,
                    UpdatedAt = ps.InformationType.UpdatedAt,
                }
            }).ToList()
        });

        return bookViewModels;
    }



    public async Task<PagedResult<BookVm>> GetAllBookAsync(int? pageNumber, int? pageSize)
    {
        int currentPage = pageNumber ?? 1;
        int currentPageSize = pageSize ?? 10;

        var query = await GetAllBookAsync();

        var totalCount = query.Count();
        var paginatedBooks = query
            .OrderBy(u => u.BookId)
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .ToList();

        return new PagedResult<BookVm>
        {
            CurrentPage = currentPage,
            PageSize = currentPageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
            Items = paginatedBooks
        };
    }

    public async Task<IEnumerable<BookInformationVm>> GetBookInformationsByBookIdAsync(int bookId)
    {
        var book = await GetByBookIdAsync(bookId);

        if (book == null)
        {
            throw new ExceptionNotFound("Book không tìm thấy");
        }

        var bookInformationViewModels = book.BookInformations.Select(ps => new BookInformationVm
        {
            BookId = ps.BookId,
            InformationTypeId = ps.InformationTypeId,
            Publish = ps.Publish,
            InformationType = new InformationTypeVm
            {
                InformationTypeId = ps.InformationType.InformationTypeId,
                Code = ps.InformationType.Code
            }
        }).ToList();

        return bookInformationViewModels;
    }

    public async Task<BookVm?> GetByBookIdAsync(int id)
    {
        var book = await GetOneAsync(
            p => p.BookId == id,
            includesProperties: "Category,BookInformations.InformationType"
        );

        if (book == null)
        {
            throw new ExceptionNotFound("Book không tìm thấy");
        }

        var bookVm = new BookVm
        {
            BookId = book.BookId,
            Name = book.Name,
            Description = book.Description,
            Price = book.Price,
            OldPrice = book.OldPrice,
            StockQuantity = book.StockQuantity,
            CategoryId = book.CategoryId,
            ImageUrl = book.ImageUrl,
            Author = book.Author,
            Discount = book.Discount, 
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            Category = book.Category != null ? new CategoryVm
            {
                CategoryId = book.Category.CategoryId,
                Name = book.Category.Name,
                ImageUrl = book.Category.ImageUrl,
                CreatedAt = book.Category.CreatedAt,
                UpdatedAt = book.Category.UpdatedAt
            } : null, 
            BookInformations = book.BookInformations?.Select(ps => new BookInformationVm
            {
                BookId = ps.BookId,
                InformationTypeId = ps.InformationTypeId,
                Publish = ps.Publish,
                InformationType = ps.InformationType != null ? new InformationTypeVm
                {
                    InformationTypeId = ps.InformationType.InformationTypeId,
                    Code = ps.InformationType.Code
                } : null 
            }).ToList()
        };

        return bookVm;
    }


    public async Task<BookVm> UpdateBookAsync(int id, InputBookVm bookVm)
    {
        ValidateModelPropertiesWithAttribute(bookVm);

        var existingBook = await _unitOfWork.GenericRepository<Book>().GetByIdAsync(id);
        if (existingBook == null)
        {
            throw new ExceptionBusinessLogic("Book không tìm thấy.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var bookInformationsToAddOrUpdate = await ProcessAndSyncInformationsAsync(id, bookVm.BookInformations);

            existingBook.Name = bookVm.Name;
            existingBook.Description = bookVm.Description;
            existingBook.Price = bookVm.Price;
            existingBook.OldPrice = bookVm.OldPrice;
            existingBook.StockQuantity = bookVm.StockQuantity;
            existingBook.CategoryId = bookVm.CategoryId;
            existingBook.ImageUrl = bookVm.ImageUrl;
            existingBook.Author = bookVm.Author;
            existingBook.Discount = bookVm.Discount;
            existingBook.UpdatedAt = DateTime.Now;
           
            _unitOfWork.GenericRepository<Book>().Update(existingBook);
            await _unitOfWork.SaveChangesAsync();

            foreach (var bookInformation in bookInformationsToAddOrUpdate)
            {
                var existingSpec = await _unitOfWork.GenericRepository<BookInformation>().GetAsync(ps =>
                    ps.BookId == id && ps.InformationTypeId == bookInformation.InformationTypeId);

                if (existingSpec != null)
                {
                    existingSpec.Publish = bookInformation.Publish;
                    _unitOfWork.GenericRepository<BookInformation>().Update(existingSpec);
                }
                else
                {
                    bookInformation.BookId = id; 
                    await _unitOfWork.GenericRepository<BookInformation>().AddAsync(bookInformation);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            return new BookVm
            {
                BookId = existingBook.BookId,
                Name = existingBook.Name,
                Description = existingBook.Description,
                Price = existingBook.Price,
                OldPrice = existingBook.OldPrice,
                StockQuantity = existingBook.StockQuantity,
                CategoryId = existingBook.CategoryId,
                ImageUrl = existingBook.ImageUrl,
                Author = existingBook.Author,
                Discount = existingBook.Discount,
                CreatedAt = existingBook.CreatedAt,
                UpdatedAt = existingBook.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");

            await _unitOfWork.RollbackTransactionAsync();

            throw new ArgumentException($"{ex.Message}", ex);
        }


    }
    public async Task<int> UpdateBookAsync(Book book)
    {
        book.Name = book.Name;
        book.Description = book.Description;
        book.Price = book.Price;
        book.OldPrice = book.OldPrice;
        book.StockQuantity = book.StockQuantity;
        book.CategoryId = book.CategoryId;
        book.ImageUrl = book.ImageUrl;
        book.Author = book.Author;
        book.Discount = book.Discount;


        return await _unitOfWork.GenericRepository<Book>().ModifyAsync(book);
    }
    public async Task<BookDetailVm> GetBookDetailsAsync(int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            return null;  
        }

        var category = await _categoryRepository.GetByIdAsync((int)book.CategoryId);

        var bookDetailVm = new BookDetailVm
        {
            BookId = book.BookId,
            BookName = book.Name,
            Description = book.Description,
            Price = book.Price,
            ImageUrl = book.ImageUrl,

            CreatedDate = book.CreatedDate,
            UpdatedDate = book.UpdatedDate
        };

        return bookDetailVm;
    }


    public async Task<IEnumerable<BookVm>> FilterBooksAsync(FilterRequest filterRequest)
    {
        try
        {
            var query = await GetAllAsync(includesProperties: "Category,BookInformations,BookInformations.InformationType");

            if (filterRequest.Categories != null && filterRequest.Categories.Any())
            {
                query = query.Where(p => p.Category != null && filterRequest.Categories.Contains(p.Category.Name));
            }

            if (filterRequest.Prices != null && filterRequest.Prices.Any())
            {
                foreach (var price in filterRequest.Prices)
                {
                    switch (price)
                    {
                        case "under2m":
                            query = query.Where(p => p.Price < 200000);
                            break;
                        case "2to5m":
                            query = query.Where(p => p.Price >= 200000 && p.Price <= 500000);
                            break;
                        case "5to10m":
                            query = query.Where(p => p.Price >= 500000 && p.Price <= 1000000);
                            break;
                        case "10to15m":
                            query = query.Where(p => p.Price >= 1000000 && p.Price <= 1500000);
                            break;
                        case "above15m":
                            query = query.Where(p => p.Price >= 1500000);
                            break;
                    }
                }
            }

            switch (filterRequest.Sort)
            {
                case "banchay":
                    query = query.OrderByDescending(p => p.OrderItems
                        .Where(oi => oi.Order.OrderStatus == "done" && oi.BookId == p.BookId)
                        .Sum(oi => oi.Quantity));
                    break;
                case "giathap":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "giacao":
                    query = query.OrderByDescending(p => p.Price);
                    break;
            }

            return query.Select(p => new BookVm
            {
                BookId = p.BookId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                OldPrice = p.OldPrice,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                Author = p.Author,
                Discount = p.Discount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
            return Enumerable.Empty<BookVm>();
        }
    }
    public async Task<IEnumerable<BookVm>> GetNewestBooksAsync()
    {
        try
        {
            var query = await GetAllAsync(includesProperties: "Category, BookInformations, BookInformations.InformationType");

            var newestBooks = query
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .Select(p => new BookVm
                {
                    BookId = p.BookId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    Author = p.Author,
                    Discount = p.Discount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                })
                .ToList();

            return newestBooks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
            return Enumerable.Empty<BookVm>();
        }
    }
    public Task<bool> AddBookToCartAsync(int bookId, CartVm cartVm)
    {
        throw new NotImplementedException();
    }
}

