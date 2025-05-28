using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ApiSostenibilitatDef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RecipeController(ApplicationDbContext context) { _context = context; }
        /// <summary>
        /// Retrieves all the recipes from the database, including their associated ingredients and diets.
        /// It returns a list of RecipeDTO objects with recipe details, including ingredient names and diet IDs.
        /// </summary>
        /// <returns>Returns a list of RecipeDTO objects if recipes are found. If no recipes are found, returns a 404 error.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            var recipes = await _context.Recipes.Include(n => n.Ingredients).Include(n => n.Diets).ToListAsync();
            if (recipes.Count == 0)
            {
                return NotFound("There are no recipes in the database yet!");
            }

            // Map recipes to RecipeDTO to avoid circular reference errors
            var recipeDTO = recipes.Select(n => new RecipeDTO
            {
                Id = n.Id,
                Name = n.Name,
                Description = n.Description,
                Ingredients = n.Ingredients.Select(v => v.Name).ToList(),
                Diets = n.Diets.Select(r => r.Id).ToList()
            }).ToList();

            return Ok(recipeDTO);
        }

        /// <summary>
        /// Retrieves a specific recipe by its ID, including the associated ingredients and diets.
        /// It returns the recipe as a RecipeDTO object if found.
        /// </summary>
        /// <param name="id">The ID of the recipe to retrieve.</param>
        /// <returns>Returns a RecipeDTO object if the recipe is found, or a 404 error if the recipe is not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _context.Recipes.Include(n => n.Diets).Include(n => n.Ingredients).FirstOrDefaultAsync(n => n.Id == id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            var recipeDTO = new RecipeDTO
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients.Select(v => v.Name).ToList(),
                Diets = recipe.Diets.Select(r => r.Id).ToList()
            };

            return Ok(recipeDTO);
        }

        /// <summary>
        /// Retrieves a list of recipes by a diet Id, including the associated ingredients.
        /// It returns the recipe as a RecipeDTO object if found.
        /// </summary>
        /// <param name="id">The ID of the diet to retrieve.</param>
        /// <returns>Returns a list of RecipeDTO object if there are recipes found, or a 404 error if there are no recipes in that diet.</returns>
        [HttpGet("diet/{id}")]
        public async Task<ActionResult<List<Recipe>>> GetByDietId(int id)
        {
            var recipes = await _context.Recipes.Include(n => n.Diets).Include(n => n.Ingredients).ToListAsync();
            if (recipes == null || recipes.Count == 0)
            {
                return NotFound("There are no recipes in this diet.");
            }
            var recipesDTO = recipes.Select(r => new RecipeDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Ingredients = r.Ingredients.Select(v => v.Name).ToList(),
                Diets = r.Diets.Select(r => r.Id).ToList()
            }
            ).ToList().Where(r=> r.Diets.Contains(id));

            return Ok(recipesDTO);
        }


        /// <summary>
        /// Adds a new recipe to the database based on the provided RecipeDTO object.
        /// The recipe includes associated ingredients and diets mapped from the DTO.
        /// </summary>
        /// <param name="recipeDTO">The RecipeDTO object containing the new recipe's data.</param>
        /// <returns>Returns a 201 status with the created recipe if successful, or a 400 error if the provided data is invalid.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPost]
        public async Task<ActionResult<Recipe>> Add(RecipeDTO recipeDTO)
        {
            var recipe = new Recipe { Name = recipeDTO.Name, Description = recipeDTO.Description };

            // Add ingredients to the recipe
            foreach (var i in recipeDTO.Ingredients)
            {
                var result = await _context.Ingredients.FirstOrDefaultAsync(n => n.Name == i);
                if (result != null)
                {
                    recipe.Ingredients.Add(result);
                }
            }

            // Add diets to the recipe
            foreach (var i in recipeDTO.Diets)
            {
                var result = await _context.Diets.FindAsync(i);
                if (result != null)
                {
                    recipe.Diets.Add(result);
                }
            }

            try
            {
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), recipe);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }


        /// <summary>
        /// Deletes a specific recipe from the database by its ID.
        /// It returns the deleted recipe if successful, or a 400 error if the deletion fails.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete.</param>
        /// <returns>Returns the deleted recipe if successful, or a 400 error if the recipe cannot be deleted.</returns>

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<Recipe>> Delete(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            try
            {
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
                return Ok(recipe);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the recipe.");
            }
        }


        /// <summary>
        /// Updates an existing recipe by its ID with the data from the provided RecipeDTO object.
        /// It also updates the associated ingredients and diets of the recipe.
        /// </summary>
        /// <param name="recipeDTO">The RecipeDTO object containing the updated recipe's data.</param>
        /// <param name="id">The ID of the recipe to update.</param>
        /// <returns>Returns the updated recipe if successful, or a 400 error if the update fails.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPut("{id}")]
        public async Task<ActionResult<Recipe>> Update(RecipeDTO recipeDTO, int id)
        {
            var recipe = await _context.Recipes.Include(i => i.Ingredients).Include(i => i.Diets).FirstOrDefaultAsync(n => n.Id == id);

            if (recipe == null)
            {
                return NotFound("The recipe does not exist.");
            }

            recipe.Name = recipeDTO.Name;
            recipe.Description = recipeDTO.Description;

            // Update ingredients
            recipe.Ingredients.Clear();
            foreach (var ingName in recipeDTO.Ingredients)
            {
                var result = await _context.Ingredients.FirstOrDefaultAsync(n => n.Name == ingName);
                if (result != null)
                {
                    recipe.Ingredients.Add(result);
                }
            }

            // Update diets
            recipe.Diets.Clear();
            foreach (var dietId in recipeDTO.Diets)
            {
                var result = await _context.Diets.FindAsync(dietId);
                if (result != null)
                {
                    recipe.Diets.Add(result);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(recipe);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }
    }

}

