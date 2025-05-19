
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiSostenibilitat.Data
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Game> Games { get; set; }
        public DbSet<Diet> Diets { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Result> Results { get; set; }
        

        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ONE-TO-ONE
            modelBuilder.Entity<User>()
                .HasOne(u => u.Diet)       
                .WithOne(p => p.User)         
                .HasForeignKey<Diet>(p => p.UserId);

            //MANY-TO-MANY
            modelBuilder.Entity<Ingredient>()
               .HasMany(n => n.Recipes)
               .WithMany(n => n.Ingredients);

            modelBuilder.Entity<Recipe>()
                .HasMany(n => n.Ingredients)
                .WithMany(n => n.Recipes);

            modelBuilder.Entity<Diet>()
               .HasMany(n => n.Recipes)
               .WithMany(n => n.Diets);

            modelBuilder.Entity<Recipe>()
                .HasMany(n => n.Diets)
                .WithMany(n => n.Recipes);

            //ONE-TO-MANY
            modelBuilder.Entity<Game>()
            .HasMany(e => e.Results)
            .WithOne(e => e.Game)
            .HasForeignKey(e => e.GameId)
            .IsRequired();

            modelBuilder.Entity<Diet>()
            .HasMany(e => e.Results)
            .WithOne(e => e.Diet)
            .HasForeignKey(e => e.DietId)
            .IsRequired();

            modelBuilder.Entity<User>()
            .HasMany(e => e.Results)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
        }
    }
}

