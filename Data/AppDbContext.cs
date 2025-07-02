using Microsoft.EntityFrameworkCore;
using FaceAuthApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace FaceAuthApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Optional: Configure table names or constraints here
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
