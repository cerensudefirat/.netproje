using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace api.Data
{
    public class ApplicationDB : IdentityDbContext<AppUser>
    {
        public ApplicationDB(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x=>x.HasKey(p=>new{p.AppUserId,p.StockId}));

            builder.Entity<Portfolio>().HasOne(u => u.AppUser).WithMany(u => u.Portfolios).HasForeignKey(p => p.AppUserId);

            builder.Entity<Portfolio>().HasOne(u => u.Stock).WithMany(u => u.Portfolios).HasForeignKey(p => p.StockId);

            // SABİT GUID'LER: Değiştirmeden sabit bırakın
            const string AdminRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            const string UserRoleId = "b27c0f1e-5b0f-4c9f-8c0a-2c9b1a7a9e21";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "6cba2b40-5d55-4c5d-88e6-2b7e2d6b2df1"
                },
                new IdentityRole
                {
                    Id = UserRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "d1e9b2a7-0a44-4f53-bc2c-9a1a0e7b4f22"
                }
            );
        }
    }
}
