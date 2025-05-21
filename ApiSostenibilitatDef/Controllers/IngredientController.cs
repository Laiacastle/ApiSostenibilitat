using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSostenibilitatDef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IngredientController(ApplicationDbContext context) { _context = context; }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            var ingredients = await _context.Ingredients.Include(n => n.Vitamins).Include(n=>n.Recipes).ToListAsync();
            if (ingredients.Count == 0)
            {
                return NotFound("Encara no hi han ingredients a la base de dades!");
            }

            // M apeamos para pasarlo al dto para evitar el error de infinidad
            var ingDTO = ingredients.Select(n => new IngredientDTO
            {
                Id = n.Id,
                Name = n.Name,
                EatForms = n.EatForms,
                Fiber = n.Fiber,
                Calories = n.Calories,
                Vitamins = n.Vitamins.Select(v=>v.Name).ToList(),
                Recipes = n.Recipes.Select(r => r.Id).ToList()
            }).ToList();

            return Ok(ingDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _context.Ingredients.Include(n => n.Recipes).Include(n=>n.Vitamins).FirstOrDefaultAsync(n => n.Id == id);
            if (ingredient == null)
            {
                return NotFound("No s'ha trobat l'ingredient");
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
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Ingredient>> Add(IngredientDTO ingDTO)
        {
            var ingredient = new Ingredient { Name = ingDTO.Name, EatForms = ingDTO.EatForms, Fiber = ingDTO.Fiber, Calories = ingDTO.Calories };

            //Afegim els results 

            foreach (var i in ingDTO.Vitamins)
            {
                var result = await _context.Vitamins.FirstOrDefaultAsync(n=>n.Name==i);
                if (result != null)
                {
                    ingredient.Vitamins.Add(await _context.Vitamins.FindAsync(result));
                }
            }
            foreach (var i in ingDTO.Recipes)
            {
                var result = await _context.Recipes.FindAsync(i);
                if (result != null)
                {
                    ingredient.Recipes.Add(await _context.Recipes.FindAsync(result));
                }
            }

            try
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), ingredient);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Dades erroneas");
            }
        }
        //[Authorize(Roles = "Admin")]
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
                return BadRequest("No s'ha pogut esborrar l'ingredient");
            }
        }
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Ingredient>> Update(IngredientDTO ingDTO, int id)
        {
            var ingredient = await _context.Ingredients.Include(i => i.Vitamins).Include(i => i.Recipes).FirstOrDefaultAsync(n => n.Id == id);

            if (ingredient == null)
            {
                return NotFound("L'ingredient no existeix!");
            }

            
            ingredient.Name = ingDTO.Name;
            ingredient.EatForms = ingDTO.EatForms;
            ingredient.Fiber = ingDTO.Fiber;
            ingredient.Calories = ingDTO.Calories;

            ingredient.Vitamins.Clear();
            foreach (var vitaminName in ingDTO.Vitamins)
            {
                var vitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Name == vitaminName);
                if (vitamin != null)
                {
                    ingredient.Vitamins.Add(vitamin);
                }
            }

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
                return BadRequest("No s'ha pogut fer l'update");
            }
        }



    }
}
