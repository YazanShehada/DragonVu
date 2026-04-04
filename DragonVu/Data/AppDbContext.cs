using DragonVu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<Question> questions { get; set; }

        public DbSet<Result> results { get; set; }

        public DbSet<Subject> subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            //this shoud be restricted but i make it cascade because i am lazy

            builder.Entity<IdentityRole>().HasData(

                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "a1f7c1b2-1111-4444-8888-aaaaaaaaaaaa"
                },

                new IdentityRole
                {
                    Id = "2",
                    Name = "Editor",
                    NormalizedName = "EDITOR",
                    ConcurrencyStamp = "b2f7c1b2-2222-4444-8888-bbbbbbbbbbbb"
                },

                new IdentityRole
                {
                    Id = "3",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "c2f7c1b2-3333-4444-8888-cccccccccccc"
                }

            );
        }
    }
}






