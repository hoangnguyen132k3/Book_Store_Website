using BookStore.DAL.Models;
using BookStore.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BookStore.DAL.Seed
{
    public static class SeedData
    {
        public static async Task SeedAsync(BookDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                    new Category
                    {
                        Name = "Kinh dị",
                        ImageUrl = "https://png.pngtree.com/png-clipart/20231024/original/pngtree-halloween-design-element-scary-skull-for-greeting-card-png-image_13402697.png"
                    },

                    new Category
                    {
                        Name = "Nấu Ăn",
                        ImageUrl = "https://png.pngtree.com/png-clipart/20220903/ourmid/pngtree-chef-hat-and-cooking-logo-png-image_6136205.png"
                    },

                    new Category
                    {
                        Name = "Lịch Sử",
                        ImageUrl = "https://dongphuchaianh.com/wp-content/uploads/2024/04/logo-ao-lop-chuyen-su-history.jpg"
                    },

                    new Category
                    {
                        Name = "Thiếu Nhi",
                        ImageUrl = "https://www.logoground.com/uploads/2017121722017-12-304184255kid%20logo.jpg"
                    },

                    new Category
                    {
                        Name = "Tôn Giáo",
                        ImageUrl = "https://png.pngtree.com/png-vector/20220520/ourmid/pngtree-abstract-religious-logo-of-christian-cross-linear-church-symbol-vector-png-image_46298254.jpg"
                    },

                    new Category
                    {
                        Name = "Trinh Thám",
                        ImageUrl = "https://inkythuatso.com/uploads/thumbnails/800/2021/12/logo-trinh-sat-doan-inkythuatso-22-13-59-46.jpg"
                    }
                    );
                    
                    await context.SaveChangesAsync(); 
                }

                if (!context.Books.Any())
                {
                    context.Books.AddRange(
                        new Book
                        {
                            Name = "Địa Ngục Tầng Thứ 19",
                            Description = "Hay",
                            Price = 200000,
                            OldPrice = 250000,
                            StockQuantity = 30,
                            CategoryId = 1,
                            ImageUrl = "https://cdn.popsww.com/blog/sites/2/2021/04/cam-tu-ky-bao.jpg",
                            Author = "Haong",
                            Discount = 10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Book
                        {
                            Name = "Lịch sử Việt Nam",
                            Description = "Hay",
                            Price = 200000,
                            OldPrice = 250000,
                            StockQuantity = 30,
                            CategoryId = 2,
                            ImageUrl = "https://salt.tikicdn.com/cache/750x750/ts/product/3a/4d/1e/1988a25f90815354c78693c3eecdac95.jpg.webp",
                            Author = "Haong",
                            Discount = 10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Book
                        {
                            Name = "Vì cậu là bạn nhỏ của tớ",
                            Description = "Hay",
                            Price = 200000,
                            OldPrice = 250000,
                            StockQuantity = 30,
                            CategoryId = 1,
                            ImageUrl = "https://pos.nvncdn.com/fd5775-40602/ps/20240516_AHZBxcf3v3.jpeg",
                            Author = "Haong",
                            Discount = 10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Book
                        {
                            Name = "Sách kinh thánh",
                            Description = "Hay",
                            Price = 200000,
                            OldPrice = 250000,
                            StockQuantity = 30,
                            CategoryId = 1,
                            ImageUrl = "https://sachkinhthanh.com/wp-content/uploads/2022/10/z5245730918369_3c7194911883b2e064bf20e26905b957.jpg",
                            Author = "Haong",
                            Discount = 10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    );
                    Random random = new Random();
                    for (int i = 1; i <= 20; i++)
                    {
                        context.Books.Add(new Book
                        {
                            Name = $"Book {i}",
                            Description = $"Description for Book {i}",
                            Price = 10000000,
                            OldPrice = 10500000,
                            StockQuantity = 50,
                            CategoryId = random.Next(1, 6),
                            ImageUrl = "https://cdn.popsww.com/blog/sites/2/2021/04/cam-tu-ky-bao.jpg",
                            Author = "Hanh",
                            Discount = 12,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.InformationTypes.Any())
                {
                    context.InformationTypes.AddRange(
                        new InformationType { Code = "39483ru394834" }
                    );
                    await context.SaveChangesAsync();
                }

                if (!context.BookInformations.Any())
                {
                    context.BookInformations.AddRange(
                        new BookInformation { BookId = 1, InformationTypeId = 1, Publish = "Kim Dong" }
                    );
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static void EnableIdentityInsert(BookDbContext context, string tableName, bool enable)
        {
            var rawSql = enable
                ? $"SET IDENTITY_INSERT {tableName} ON;"
                : $"SET IDENTITY_INSERT {tableName} OFF;";
            context.Database.ExecuteSqlRaw(rawSql);
        }
    }
}
