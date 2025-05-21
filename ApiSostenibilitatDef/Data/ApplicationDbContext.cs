
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
        public DbSet<Vitamin> Vitamins { get; set; }
        

        
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
            modelBuilder.Entity<Result>()
            .HasOne(e => e.Game)
            .WithMany(e => e.Results)
            .HasForeignKey(e => e.GameId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
            .HasOne(e => e.Diet)
            .WithMany(e => e.Results)
            .HasForeignKey(e => e.DietId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
            .HasOne(e => e.User)
            .WithMany(e => e.Results)
            .HasForeignKey(e => e.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

            //DataSEEd
            modelBuilder.Entity<Vitamin>().HasData(
                new Vitamin { Id = 1, Name = "A" },
                new Vitamin { Id = 2, Name = "C" },
                new Vitamin { Id = 3, Name = "K" },
                new Vitamin { Id = 4, Name = "B6"},
                new Vitamin { Id = 5, Name = "E" },
                new Vitamin { Id = 6, Name = "D" }
            );

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient
                {
                    Id = 1,
                    Name = "Pastanaga",
                    Fiber = 2.8,
                    Calories = 41,
                    EatForms = ["Crua", "Cuita", "Ratllada", "Batuda"]
                },
                new Ingredient
                {
                    Id = 2,
                    Name = "Espinaca",
                    Fiber = 2.2,
                    Calories = 23,
                    EatForms = ["Cuita"]
                },
                new Ingredient
                {
                    Id = 3,
                    Name = "Plátan",
                    Fiber = 2.6,
                    Calories = 89,
                    EatForms = ["Cru", "Fregit", "Batut"]
                },
                new Ingredient
                {
                    Id = 4,
                    Name = "Aguacate",
                    Fiber = 6.7,
                    Calories = 160,
                    EatForms = ["Cru", "Batut"]
                },
                new Ingredient
                {
                    Id = 5,
                    Name = "Brócoli",
                    Fiber = 2.6,
                    Calories = 34,
                    EatForms = ["Cuita", "Batut"]
                },
                new Ingredient
                {
                    Id = 6,
                    Name = "Tomaquet",
                    Fiber = 1.2,
                    Calories = 18,
                    EatForms = ["Cru", "Cuinat", "Batut"]
                },
                new Ingredient
                {
                    Id = 7,
                    Name = "Almendra",
                    Fiber = 12.5,
                    Calories = 579,
                    EatForms = ["Crua", "Ratllada"]
                }
            );

            
            modelBuilder.Entity<Vitamin>()
                .HasMany(v => v.Ingredients)
                .WithMany(i => i.Vitamins);

            modelBuilder.Entity<Ingredient>()
            .HasMany(i => i.Vitamins)
            .WithMany(i=>i.Ingredients)
            .UsingEntity(j => j.HasData(
                new { IngredientsId = 1, VitaminsId = 1 }, // Zanahoria - A
                new { IngredientsId = 1, VitaminsId = 2 }, // Zanahoria - C
                new { IngredientsId = 2, VitaminsId = 2 }, // Espinaca - C
                new { IngredientsId = 2, VitaminsId = 3 }, // Espinaca - K
                new { IngredientsId = 3, VitaminsId = 4 }, // Plátano - B6
                new { IngredientsId = 4, VitaminsId = 5 }, // Aguacate - E
                new { IngredientsId = 5, VitaminsId = 2 }, // Brócoli - C
                new { IngredientsId = 5, VitaminsId = 3 }, // Brocoli - K
                new { IngredientsId = 6, VitaminsId = 1 }, // Tomate - A
                new { IngredientsId = 6, VitaminsId = 2 }, // Tomate - C
                new { IngredientsId = 7, VitaminsId = 5 }, // Almendra - E
                new { IngredientsId = 7, VitaminsId = 6 }  // Almendra - D
            ));

        }
    }
}

