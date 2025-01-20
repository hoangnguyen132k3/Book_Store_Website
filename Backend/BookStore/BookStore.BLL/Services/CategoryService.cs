using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BookStore.BLL.Services
{
    public interface ICategoryService : IBaseService<Category>
    {
        Task<CategoryVm> AddCategoryAsync(InputCategoryVm categoryVm);
        Task<CategoryVm> UpdateCategoryAsync(int id, InputCategoryVm categoryVm);

        Task<CategoryVm> DeleteCategoryAsync(int id);

        Task<CategoryVm?> GetByCategoryIdAsync(int id);

        Task<IEnumerable<CategoryVm>> GetAllCategoryAsync();
        Task<int> UpdateCategoryAsync(Category category);
        Task<PagedResult<CategoryVm>> GetAllCategoryAsync(int? pageNumber, int? pageSize);
    }
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryVm>> GetAllCategoryAsync()
        {
            var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();

            var categoryViewModels = categories.Select(category => new CategoryVm
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });

            return categoryViewModels;
        }

        public async Task<PagedResult<CategoryVm>> GetAllCategoryAsync(int? pageNumber, int? pageSize)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 10;

            var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();

            var totalCount = categories.Count();

            var paginatedCategories = categories
                .OrderBy(c => c.CategoryId)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            var categoryVms = paginatedCategories.Select(category => new CategoryVm
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });

            return new PagedResult<CategoryVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = categoryVms.ToList()
            };
        }



        public async Task<CategoryVm?> GetByCategoryIdAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                throw new ExceptionNotFound("Category không tìm thấy");
            }
            var categoryVm = new CategoryVm
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            return categoryVm;

        }
        public async Task<CategoryVm> AddCategoryAsync(InputCategoryVm categoryVm)
        {
            ValidateModelPropertiesWithAttribute(categoryVm);

            var findCategory = await _unitOfWork.GenericRepository<Category>().GetAsync(b =>
                b.Name == categoryVm.Name
            );
            if (findCategory == null)
            {
                var category = new Category
                {
                    Name = categoryVm.Name,
                    ImageUrl = categoryVm.ImageUrl
                };

                if (await AddAsync(category) > 0)
                {
                    return new CategoryVm
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        ImageUrl = category.ImageUrl
                    };
                }
                throw new ArgumentException("Không thể cập nhật category");
            }
            throw new ExceptionBusinessLogic("Category name đã được sử dụng.");

        }

        public async Task<CategoryVm> UpdateCategoryAsync(int id, InputCategoryVm categoryVm)
        {
            ValidateModelPropertiesWithAttribute(categoryVm);
            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                throw new ExceptionNotFound("Category không tìm thấy");
            }

            var findCategory = await _unitOfWork.GenericRepository<Category>().GetAsync(b =>
                b.CategoryId != id &&
                b.Name == categoryVm.Name
             );
            if (findCategory != null)
            {
                throw new ExceptionBusinessLogic("Category name đã được sử dụng.");
            }
            category.Name = categoryVm.Name;
            category.ImageUrl = categoryVm.ImageUrl;
            category.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.GenericRepository<Category>().ModifyAsync(category);

            if (result <= 0)
            {
                throw new ArgumentException("Không thể cập nhật Category");
            }

            return new CategoryVm
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt
            };

        }
        public async Task<int> UpdateCategoryAsync(Category category)
        {
            category.Name = category.Name;
            category.ImageUrl = category.ImageUrl;
            category.UpdatedAt = DateTime.Now;

            return await _unitOfWork.GenericRepository<Category>().ModifyAsync(category);
        }
        public async Task<CategoryVm> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);

            if (category == null)
            {
                throw new ExceptionNotFound("Category không tìm thấy");
            }

            _unitOfWork.GenericRepository<Category>().Delete(category);
            var isdelete = await _unitOfWork.SaveChangesAsync();

            if (isdelete > 0)
            {
                return new CategoryVm
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ImageUrl = category.ImageUrl
                };
            }

            throw new ArgumentException("Không thể xóa category");
        }


    }
}
