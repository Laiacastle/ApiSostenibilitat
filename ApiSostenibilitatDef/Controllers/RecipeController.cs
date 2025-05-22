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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            var recipes = await _context.Recipes.Include(n => n.Ingredients).Include(n => n.Diets).ToListAsync();
            if (recipes.Count == 0)
            {
                return NotFound("Encara no hi han receptes a la base de dades!");
            }

            // M apeamos para pasarlo al dto para evitar el error de infinidad
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _context.Recipes.Include(n => n.Diets).Include(n => n.Ingredients).FirstOrDefaultAsync(n => n.Id == id);
            if (recipe == null)
            {
                return NotFound("No s'ha trobat la recepta");
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
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<ActionResult<Recipe>> Add(RecipeDTO recipeDTO)
        {
            var recipe = new Recipe { Name = recipeDTO.Name, Description = recipeDTO.Description};

            //Afegim els results 

            foreach (var i in recipeDTO.Ingredients)
            {
               
                var result = await _context.Ingredients.FirstOrDefaultAsync(n=>n.Name == i);
                if (result != null)
                {
                    recipe.Ingredients.Add(result);
                }
            }
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
                return BadRequest("Dades erroneas");
            }
        }
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
                return BadRequest("No s'ha pogut esborrar la recepta");
            }
        }
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Recipe>> Update(RecipeDTO recipeDTO, int id)
        {
            var recipe = await _context.Recipes.Include(i => i.Ingredients).Include(i => i.Diets).FirstOrDefaultAsync(n => n.Id == id);

            if (recipe == null)
            {
                return NotFound("La recepta no existeix!");
            }


            recipe.Name = recipeDTO.Name;
            recipe.Description = recipeDTO.Description;


            recipe.Ingredients.Clear();
            foreach (var ingName in recipeDTO.Ingredients)
            {
                var result = await _context.Ingredients.FirstOrDefaultAsync(n=>n.Name == ingName);
                if (result != null)
                {
                    recipe.Ingredients.Add(result);
                }
            }

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
                return BadRequest("No s'ha pogut fer l'update");
            }
        }
    }
}
