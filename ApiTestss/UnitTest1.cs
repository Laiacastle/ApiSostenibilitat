using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ApiSostenibilitat.Models;
using ApiSostenibilitatDef.Controllers;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Data;
using Microsoft.AspNetCore.Identity;
using ApiSostenibilitat.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Configuration;

namespace ApiTestss.Tests
{
    public class UnitTest1
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuthenticationController _controller;
        public UnitTest1()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;

            _context = new ApplicationDbContext(options);
            // Configurar UserManager utilizando un store en la m3moria
            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);

            //Configurar logger y configuración
            _logger = new LoggerFactory().CreateLogger<AuthenticationController>();
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "jwtSettings:Key", "TestSecretKey" },
                { "jwtSettings:Issuer", "TestIssuer" },
                { "jwtSettings:Audience", "TestAudience" },
                { "jwtSettings:ExpirationMinutes", "30" }
            }).Build();

            // Crear el controlador con las dependencies
            _controller = new AuthenticationController(_userManager, _logger, _configuration, _context);
        }

        //-------------------------------------------Vitamin Controller-----------------------------------

        [Fact]
        public async Task GetAll_ShouldReturnAllVitamins()
        {
            // Arrange
            _context.Vitamins.Add(new Vitamin { Name = "Vitamin A" });
            _context.Vitamins.Add(new Vitamin { Name = "Vitamin B" });
            await _context.SaveChangesAsync();
            var controller = new VitaminController(_context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var vitamins = Assert.IsAssignableFrom<List<Vitamin>>(okResult.Value);
            Assert.Equal(2, vitamins.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnVitaminById()
        {
            // Arrange
            var vitamin = new Vitamin { Name = "Vitamin C" };
            _context.Vitamins.Add(vitamin);
            await _context.SaveChangesAsync();
            var controller = new VitaminController(_context);

            // Act
            var result = await controller.GetById(vitamin.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVitamin = Assert.IsType<Vitamin>(okResult.Value);
            Assert.Equal(vitamin.Id, returnedVitamin.Id);
        }

        [Fact]
        public async Task Add_ShouldCreateNewVitamin()
        {
            // Arrange
            var vitaminDTO = new Vitamin { Name = "Vitamin D" };
            var controller = new VitaminController(_context);

            // Act
            var result = await controller.Add(vitaminDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdVitamin = Assert.IsType<Vitamin>(createdAtActionResult.Value);
            Assert.Equal(vitaminDTO.Name, createdVitamin.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyExistingVitamin()
        {
            // Arrange
            var vitamin = new Vitamin { Name = "Vitamin E" };
            _context.Vitamins.Add(vitamin);
            await _context.SaveChangesAsync();

            var updatedVitamin = new Vitamin { Name = "Vitamin F" };
            var controller = new VitaminController(_context);

            // Act
            var result = await controller.Update(updatedVitamin, vitamin.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVitamin = Assert.IsType<Vitamin>(okResult.Value);
            Assert.Equal(updatedVitamin.Name, returnedVitamin.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveVitamin()
        {
            // Arrange
            var vitamin = new Vitamin { Name = "Vitamin G" };
            _context.Vitamins.Add(vitamin);
            await _context.SaveChangesAsync();
            var controller = new VitaminController(_context);

            // Act
            var result = await controller.Delete(vitamin.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedVitamin = Assert.IsType<Vitamin>(okResult.Value);
            Assert.Equal(vitamin.Name, deletedVitamin.Name);
        }

        //-------------------------------------------Recipe Controller-----------------------------------

        [Fact]
        public async Task GetAllRecipes_ShouldReturnAllRecipes()
        {
            // Arrange
            var recipe = new Recipe { Name = "Pasta", Description = "Delicious pasta" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            var controller = new RecipeController(_context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recipes = Assert.IsAssignableFrom<List<RecipeDTO>>(okResult.Value);
            Assert.Equal(1, recipes.Count);
        }

        [Fact]
        public async Task AddRecipe_ShouldCreateNewRecipe()
        {
            // Arrange
            var recipeDTO = new RecipeDTO { Name = "Salad", Description = "Healthy salad" };
            var controller = new RecipeController(_context);

            // Act
            var result = await controller.Add(recipeDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdRecipe = Assert.IsType<Recipe>(createdAtActionResult.Value);
            Assert.Equal(recipeDTO.Name, createdRecipe.Name);
        }

        [Fact]
        public async Task UpdateRecipe_ShouldModifyExistingRecipe()
        {
            // Arrange
            var recipe = new Recipe { Name = "Soup", Description = "Healthy soup" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            var updatedRecipe = new RecipeDTO { Name = "Vegetable Soup", Description = "Vegetable soup with herbs" };
            var controller = new RecipeController(_context);

            // Act
            var result = await controller.Update(updatedRecipe, recipe.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecipe = Assert.IsType<Recipe>(okResult.Value);
            Assert.Equal(updatedRecipe.Name, returnedRecipe.Name);
        }

        [Fact]
        public async Task DeleteRecipe_ShouldRemoveRecipe()
        {
            // Arrange
            var recipe = new Recipe { Name = "Pizza", Description = "Cheese pizza" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            var controller = new RecipeController(_context);

            // Act
            var result = await controller.Delete(recipe.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedRecipe = Assert.IsType<Recipe>(okResult.Value);
            Assert.Equal(recipe.Name, deletedRecipe.Name);
        }

        //-------------------------------------------Ingredient Controller-----------------------------------

        [Fact]
        public async Task GetAllIngredients_ShouldReturnAllIngredients()
        {
            // Arrange
            _context.Ingredients.Add(new Ingredient { Name = "Tomato", Fiber = 2, Calories = 30 });
            _context.Ingredients.Add(new Ingredient { Name = "Cucumber", Fiber = 1, Calories = 15 });
            await _context.SaveChangesAsync();
            var controller = new IngredientController(_context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var ingredients = Assert.IsAssignableFrom<List<IngredientDTO>>(okResult.Value);
            Assert.Equal(2, ingredients.Count);
        }

        [Fact]
        public async Task GetIngredientById_ShouldReturnCorrectIngredient()
        {
            // Arrange
            var ingredient = new Ingredient { Name = "Carrot", Fiber = 3, Calories = 40 };
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            var controller = new IngredientController(_context);

            // Act
            var result = await controller.GetById(ingredient.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedIngredient = Assert.IsType<IngredientDTO>(okResult.Value);
            Assert.Equal(ingredient.Id, returnedIngredient.Id);
        }

        [Fact]
        public async Task AddIngredient_ShouldCreateNewIngredient()
        {
            // Arrange
            var ingredientDTO = new IngredientDTO { Name = "Spinach", Fiber = 1, Calories = 25 };
            var controller = new IngredientController(_context);

            // Act
            var result = await controller.Add(ingredientDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdIngredient = Assert.IsType<Ingredient>(okResult.Value);
            Assert.Equal(ingredientDTO.Name, createdIngredient.Name);
        }

        [Fact]
        public async Task UpdateIngredient_ShouldModifyExistingIngredient()
        {
            // Arrange
            var ingredient = new Ingredient { Name = "Lettuce", Fiber = 2, Calories = 20 };
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            var updatedIngredientDTO = new IngredientDTO { Name = "Iceberg Lettuce", Fiber = 3, Calories = 15 };
            var controller = new IngredientController(_context);

            // Act
            var result = await controller.Update(updatedIngredientDTO, ingredient.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var updatedIngredient = Assert.IsType<Ingredient>(okResult.Value);
            Assert.Equal(updatedIngredientDTO.Name, updatedIngredient.Name);
        }

        [Fact]
        public async Task DeleteIngredient_ShouldRemoveIngredient()
        {
            // Arrange
            var ingredient = new Ingredient { Name = "Onion", Fiber = 2, Calories = 40 };
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            var controller = new IngredientController(_context);

            // Act
            var result = await controller.Delete(ingredient.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedIngredient = Assert.IsType<Ingredient>(okResult.Value);
            Assert.Equal(ingredient.Name, deletedIngredient.Name);
        }
    
//-------------------------------Result Controller-------------------------

        [Fact]
        public async Task GetAllResults_ShouldReturnAllResults()
        {
            // Arrange
            var result = new Result { Date = DateTime.Now, UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 100 };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            var controller = new ResultController(_context);

            // Act
            var resultList = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultList.Result);
            var results = Assert.IsAssignableFrom<List<ResultDTO>>(okResult.Value);
            Assert.Equal(1, results.Count);
        }

        [Fact]
        public async Task GetResultById_ShouldReturnCorrectResult()
        {
            // Arrange
            var result = new Result { Date = DateTime.Now, UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 100 };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            var controller = new ResultController(_context);

            // Act
            var resultFromDb = await controller.GetById(result.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultFromDb.Result);
            var returnedResult = Assert.IsType<ResultDTO>(okResult.Value);
            Assert.Equal(result.Id, returnedResult.Id);
        }

        [Fact]
        public async Task AddResult_ShouldCreateNewResult()
        {
            // Arrange
            var resultDTO = new ResultDTO { UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 100 };
            var controller = new ResultController(_context);

            // Act
            var result = await controller.Add(resultDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdResult = Assert.IsType<Result>(okResult.Value);
            Assert.Equal(resultDTO.FiResult, createdResult.FiResult);
        }

        [Fact]
        public async Task UpdateResult_ShouldModifyExistingResult()
        {
            // Arrange
            var result = new Result { Date = DateTime.Now, UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 100 };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            var updatedResultDTO = new ResultDTO { UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 90 };
            var controller = new ResultController(_context);

            // Act
            var resultUpdate = await controller.Update(updatedResultDTO, result.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultUpdate.Result);
            var updatedResult = Assert.IsType<Result>(okResult.Value);
            Assert.Equal(updatedResultDTO.FiResult, updatedResult.FiResult);
        }

        [Fact]
        public async Task DeleteResult_ShouldRemoveResult()
        {
            // Arrange
            var result = new Result { Date = DateTime.Now, UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37", GameId = 1, DietId = 1, FiResult = 100 };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            var controller = new ResultController(_context);

            // Act
            var resultDelete = await controller.Delete(result.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultDelete.Result);
            var deletedResult = Assert.IsType<Result>(okResult.Value);
            Assert.Equal(result.FiResult, deletedResult.FiResult);
        }
        //-----------------------------------------------Diet Controller------------------------


        [Fact]
        public async Task GetAllDiets_ShouldReturnAllDiets()
        {
            // Arrange
            var user = new User { Id = "b1d0da7c-da61-4651-b50d-4f976b51ca37", Name = "Test User" };
            _context.Users.Add(user);

            var diet = new Diet { Name = "Keto", Characteristics = "Low carb, high fat", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            _context.Diets.Add(diet);

            var diet2 = new Diet { Name = "Vegan", Characteristics = "Plant-based", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            _context.Diets.Add(diet2);

            await _context.SaveChangesAsync();
            var controller = new DietController(_context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var diets = Assert.IsAssignableFrom<List<Diet>>(okResult.Value);
            Assert.Equal(2, diets.Count);
        }

        [Fact]
        public async Task GetDietById_ShouldReturnCorrectDiet()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
           

            var diet = new Diet { Name = "Keto", Characteristics = "Low carb, high fat", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            var controller = new DietController(_context);

            // Act
            var result = await controller.GetById(diet.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDiet = Assert.IsType<Diet>(okResult.Value);
            Assert.Equal(diet.Name, returnedDiet.Name);
        }

        [Fact]
        public async Task AddDiet_ShouldCreateNewDiet()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
            

            var dietDTO = new DietDTO { Name = "Paleo", Characteristics = "Whole foods diet", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            var controller = new DietController(_context);

            // Act
            var result = await controller.Add(dietDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdDiet = Assert.IsType<Diet>(okResult.Value);
            Assert.Equal(dietDTO.Name, createdDiet.Name);
            Assert.Equal(dietDTO.UserId, createdDiet.UserId);
        }

        [Fact]
        public async Task UpdateDiet_ShouldModifyExistingDiet()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
            

            var diet = new Diet { Name = "Vegan", Characteristics = "Plant-based", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            var updatedDiet = new DietDTO { Name = "Whole Foods Vegan", Characteristics = "Vegan with whole foods", UserId = "user1" };
            var controller = new DietController(_context);

            // Act
            var result = await controller.Update(updatedDiet, diet.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var updatedDietResult = Assert.IsType<Diet>(okResult.Value);
            Assert.Equal(updatedDiet.Name, updatedDietResult.Name);
            Assert.Equal(updatedDiet.Characteristics, updatedDietResult.Characteristics);
        }

        [Fact]
        public async Task DeleteDiet_ShouldRemoveDiet()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
            

            var diet = new Diet { Name = "Mediterranean", Characteristics = "Healthy heart diet", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37    " };
            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            var controller = new DietController(_context);

            // Act
            var result = await controller.Delete(diet.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedDiet = Assert.IsType<Diet>(okResult.Value);
            Assert.Equal(diet.Name, deletedDiet.Name);
        }

        [Fact]
        public async Task DietShouldIncludeUser_WhenAddingNewDiet()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
            var dietDTO = new DietDTO { Name = "Keto", Characteristics = "Low carb, high fat", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            var controller = new DietController(_context);

            // Act
            var result = await controller.Add(dietDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdDiet = Assert.IsType<Diet>(okResult.Value);
            Assert.Equal("b1d0da7c-da61-4651-b50d-4f976b51ca37", createdDiet.UserId);
            Assert.Equal(user.Name, createdDiet.User.Name); 
        }

        [Fact]
        public async Task DietWithRecipesAndResults_ShouldReturnCorrectCount()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == "b1d0da7c-da61-4651-b50d-4f976b51ca37");
            

            var diet = new Diet { Name = "Keto", Characteristics = "Low carb, high fat", UserId = "b1d0da7c-da61-4651-b50d-4f976b51ca37" };
            var recipe = new Recipe { Name = "Keto Pancakes" };
            var result = new Result { FiResult = 85, Date = DateTime.Now };

            _context.Diets.Add(diet);
            _context.Recipes.Add(recipe);
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            diet.Recipes.Add(recipe);
            diet.Results.Add(result);
            await _context.SaveChangesAsync();

            var controller = new DietController(_context);

            // Act
            var resultFromDb = await controller.GetById(diet.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultFromDb.Result);
            var returnedDiet = Assert.IsType<Diet>(okResult.Value);

            Assert.Single(returnedDiet.Recipes); 
            Assert.Single(returnedDiet.Results); 
        }

        //---------------------------------Game Controller---------------------

        [Fact]
        public async Task GetAll_Games_ReturnsNotFound_WhenNoGamesExist()
        {
            // Act
            var controller = new GameController(_context);
            var result = await controller.GetAll();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("There are no games in the database yet!", notFoundResult.Value);
        }

        
        [Fact]
        public async Task GetAll_Games_ReturnsOk_WhenGamesExist()
        {
            // Arrange
            var game = new Game
            {
                MinRes = 10,
                MaxRes = 100,
                Type = "Board Game"
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Act
            var controller = new GameController(_context);
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var games = Assert.IsAssignableFrom<List<GameDTO>>(okResult.Value);
            Assert.Single(games); 
            Assert.Equal(game.MinRes, games[0].MinRes);
            Assert.Equal(game.MaxRes, games[0].MaxRes);
            Assert.Equal(game.Type, games[0].Type);
        }

        
        [Fact]
        public async Task GetById_Game_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Act
            var controller = new GameController(_context);
            var result = await controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Game not found.", notFoundResult.Value);
        }

        
        [Fact]
        public async Task GetById_Game_ReturnsOk_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                MinRes = 10,
                MaxRes = 100,
                Type = "Board Game"
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Act

            var controller = new GameController(_context);
            var result = await controller.GetById(game.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var gameDTO = Assert.IsType<GameDTO>(okResult.Value);
            Assert.Equal(game.MinRes, gameDTO.MinRes);
            Assert.Equal(game.MaxRes, gameDTO.MaxRes);
            Assert.Equal(game.Type, gameDTO.Type);
        }

        [Fact]
        public async Task Add_Game_ReturnsCreatedAtAction_WhenGameIsValid()
        {
            // Arrange
            var gameDTO = new GameDTO
            {
                MinRes = 20,
                MaxRes = 150,
                Type = "Video Game"
            };

            // Act
            var controller = new GameController(_context);
            var result = await controller.Add(gameDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdGame = Assert.IsType<Game>(createdResult.Value);
            Assert.Equal(gameDTO.MinRes, createdGame.MinRes);
            Assert.Equal(gameDTO.MaxRes, createdGame.MaxRes);
            Assert.Equal(gameDTO.Type, createdGame.Type);
        }

        [Fact]
        public async Task Delete_Game_ReturnsOk_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                MinRes = 10,
                MaxRes = 100,
                Type = "Board Game"
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Act
            var controller = new GameController(_context);
            var result = await controller.Delete(game.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedGame = Assert.IsType<Game>(okResult.Value);
            Assert.Equal(game.Id, deletedGame.Id);
        }

        [Fact]
        public async Task Delete_Game_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Act
            var controller = new GameController(_context);
            var result = await controller.Delete(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Could not delete the game.", notFoundResult.Value);
        }

        [Fact]
        public async Task Update_Game_ReturnsOk_WhenGameIsValid()
        {
            // Arrange
            var game = new Game
            {
                MinRes = 10,
                MaxRes = 100,
                Type = "Board Game"
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var gameDTO = new GameDTO
            {
                MinRes = 50,
                MaxRes = 200,
                Type = "Card Game"
            };

            // Act
            var controller = new GameController(_context);
            var result = await controller.Update(gameDTO, game.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var updatedGame = Assert.IsType<Game>(okResult.Value);
            Assert.Equal(gameDTO.MinRes, updatedGame.MinRes);
            Assert.Equal(gameDTO.MaxRes, updatedGame.MaxRes);
            Assert.Equal(gameDTO.Type, updatedGame.Type);
        }

        [Fact]
        public async Task Update_Game_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameDTO = new GameDTO
            {
                MinRes = 50,
                MaxRes = 200,
                Type = "Card Game"
            };

            // Act
            var controller = new GameController(_context);
            var result = await controller.Update(gameDTO, 999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Game does not exist.", notFoundResult.Value);
        }

        //------------------------------------Authentication Controller-----------------------------
        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsRegistered()
        {
            // Arrange
            var model = new RegisterDTO
            {
                Name = "John",
                Surname = "Doe",
                UserName = "john.doe@example.com",
                Email = "john.doe@example.com",
                Password = "Password123",
                Exercise = "Mig",
                Weight = 70,
                Age = 30,
                HoursSleep = 8
            };

            // Act
            var result = await _controller.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var model = new RegisterDTO
            {
                Name = "John",
                Surname = "Doe",
                UserName = "john.doe@example.com",
                Email = "john.doe@example.com",
                Password = "Password123",
                Exercise = "Mig",
                Weight = 70,
                Age = 30,
                HoursSleep = 8
            };

            // Intentar crear un usuario con el mismo email que ya existE
            await _userManager.CreateAsync(new User { UserName = model.UserName }, "Password123");

            // Act
            var result = await _controller.Register(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already taken.", badRequestResult.Value.ToString());
        }



        //Admins tests

        [Fact]
        public async Task AdminRegister_ReturnsOk_WhenAdminIsRegistered()
        {
            // Arrange
            var model = new RegisterDTO
            {
                Name = "Jane",
                Surname = "Smith",
                UserName = "jane.smith@example.com",
                Email = "jane.smith@example.com",
                Password = "Password123",
                Exercise = "Molt",
                Weight = 65,
                Age = 28,
                HoursSleep = 7
            };

            // Act
            var result = await _controller.AdminRegister(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Admin registered successfully.", okResult.Value);
        }

        //Login Tests

        [Fact]
        public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var model = new LoginDTO
            {
                Email = "john.doe@example.com",
                Password = "Password123"
            };

            // Crear un usuario para que inicie sesión
            var newUser = new User { UserName = model.Email };
            await _userManager.CreateAsync(newUser, model.Password);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = Assert.IsType<string>(okResult.Value);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var model = new LoginDTO
            {
                Email = "john.doe@example.com",
                Password = "WrongPassword"
            };

            // Crear un usuario con una contraseña diferente
            var newUser = new User { UserName = model.Email };
            await _userManager.CreateAsync(newUser, "Password123");

            // Act
            var result = await _controller.Login(model);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Incorrect email or password.", unauthorizedResult.Value);
        }
    }
}
