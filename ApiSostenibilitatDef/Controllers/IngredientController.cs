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
    public class IngredientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IngredientController(ApplicationDbContext context) { _context = context; }
        /// <summary>
        /// Retrieves all the ingredients from the database, including their associated vitamins and recipes.
        /// It returns a list of IngredientDTO objects with ingredient details, including the vitamins and recipe IDs.
        /// </summary>
        /// <returns>Returns a list of IngredientDTO objects if ingredients are found. If no ingredients are found, returns a 404 error.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            var ingredients = await _context.Ingredients.Include(n => n.Vitamins).Include(n => n.Recipes).ToListAsync();
            if (ingredients.Count == 0)
            {
                return NotFound("There are no ingredients in the database yet!");
            }

            // Map ingredients to IngredientDTO to avoid circular reference errors
            var ingDTO = ingredients.Select(n => new IngredientDTO
            {
                Id = n.Id,
                Name = n.Name,
                EatForms = n.EatForms,
                Fiber = n.Fiber,
                Calories = n.Calories,
                Vitamins = n.Vitamins.Select(v => v.Name).ToList(),
                Recipes = n.Recipes.Select(r => r.Id).ToList()
            }).ToList();

            return Ok(ingDTO);
        }

        /// <summary>
        /// Retrieves a specific ingredient by its ID, including the associated recipes and vitamins.
        /// It returns the ingredient as an IngredientDTO object if found.
        /// </summary>
        /// <param name="id">The ID of the ingredient to retrieve.</param>
        /// <returns>Returns an IngredientDTO object if the ingredient is found, or a 404 error if the ingredient is not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _context.Ingredients.Include(n => n.Recipes).Include(n => n.Vitamins).FirstOrDefaultAsync(n => n.Id == id);
            if (ingredient == null)
            {
                return NotFound("Ingredient not found.");
            }

            var ingDTO = new IngredientDTO
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                EatForms = ingredient.EatForms,
                Fiber = ingredient.Fiber,
                Calories = ingredient.Calories,
                Vitamins = ingredient.Vitamins.Select(v => v.Name).ToList(),
                Recipes = ingredient.Recipes.Select(r => r.Id).ToList()
            };

            return Ok(ingDTO);
        }


        /// <summary>
        /// Adds a new ingredient to the database based on the provided IngredientDTO object.
        /// The ingredient includes associated vitamins and recipes mapped from the DTO.
        /// </summary>
        /// <param name="ingDTO">The IngredientDTO object containing the new ingredient's data.</param>
        /// <returns>Returns a 201 status with the created ingredient if successful, or a 400 error if the provided data is invalid.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Add(IngredientDTO ingDTO)
        {
            var ingredient = new Ingredient { Name = ingDTO.Name, EatForms = ingDTO.EatForms, Fiber = ingDTO.Fiber, Calories = ingDTO.Calories };

            // Add vitamins to the ingredient
            foreach (var i in ingDTO.Vitamins)
            {
                var result = await _context.Vitamins.FirstOrDefaultAsync(n => n.Name == i);
                if (result != null)
                {
                    ingredient.Vitamins.Add(result);
                }
            }

            // Add recipes to the ingredient
            foreach (var i in ingDTO.Recipes)
            {
                var result = await _context.Recipes.FindAsync(i);
                if (result != null)
                {
                    ingredient.Recipes.Add(result);
                }
            }

            try
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                return Ok(ingredient);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }


        /// <summary>
        /// Deletes a specific ingredient from the database by its ID.
        /// It returns the deleted ingredient if successful, or a 400 error if the deletion fails.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        /// <returns>Returns the deleted ingredient if successful, or a 400 error if the ingredient cannot be deleted.</returns>

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<Ingredient>> Delete(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            try
            {
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
                return Ok(ingredient);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the ingredient.");
            }
        }


        /// <summary>
        /// Updates an existing ingredient by its ID with the data from the provided IngredientDTO object.
        /// It also updates the associated vitamins and recipes of the ingredient.
        /// </summary>
        /// <param name="ingDTO">The IngredientDTO object containing the updated ingredient's data.</param>
        /// <param name="id">The ID of the ingredient to update.</param>
        /// <returns>Returns the updated ingredient if successful, or a 400 error if the update fails.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPut("{id}")]
        public async Task<ActionResult<Ingredient>> Update(IngredientDTO ingDTO, int id)
        {
            var ingredient = await _context.Ingredients.Include(i => i.Vitamins).Include(i => i.Recipes).FirstOrDefaultAsync(n => n.Id == id);

            if (ingredient == null)
            {
                return NotFound("Ingredient does not exist.");
            }

            ingredient.Name = ingDTO.Name;
            ingredient.EatForms = ingDTO.EatForms;
            ingredient.Fiber = ingDTO.Fiber;
            ingredient.Calories = ingDTO.Calories;

            // Update vitamins
            ingredient.Vitamins.Clear();
            foreach (var vitaminName in ingDTO.Vitamins)
            {
                var vitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Name == vitaminName);
                if (vitamin != null)
                {
                    ingredient.Vitamins.Add(vitamin);
                }
            }

            // Update recipes
            ingredient.Recipes.Clear();
            foreach (var recipeId in ingDTO.Recipes)
            {
                var recipe = await _context.Recipes.FindAsync(recipeId);
                if (recipe != null)
                {
                    ingredient.Recipes.Add(recipe);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(ingredient);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }

    }
}
