using BookStore.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.DAL.Models.AuthenticationModels;

namespace BookStore.DAL.Data
{
    public class BookDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<InformationType> InformationTypes { get; set; }
        public DbSet<BookInformation> BookInformations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Book>()
                .HasOne(p => p.Category)
                .WithMany(b => b.Books)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BookInformation>()
                .HasKey(ps => new { ps.BookId, ps.InformationTypeId });
            modelBuilder.Entity<BookInformation>()
                .HasOne(ps => ps.Book)
                .WithMany(p => p.BookInformations)
                .HasForeignKey(ps => ps.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BookInformation>()
                .HasOne(ps => ps.InformationType)
                .WithMany(st => st.BookInformations)
                .HasForeignKey(ps => ps.InformationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Cart>()
            .HasKey(c => new { c.UserId, c.BookId });

            modelBuilder.Entity<Review>()
                .HasKey(c => new { c.UserId, c.BookId });

            modelBuilder.Entity<OrderItem>()
                .HasKey(c => new { c.OrderId, c.BookId });

            // Cảnh báo kiểu dữ liệu decimal bị cắt ngắn
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var decimalProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasColumnType("decimal(18,2)");
                }
            }
        }
    }
}
