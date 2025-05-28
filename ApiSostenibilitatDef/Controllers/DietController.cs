using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSostenibilitatDef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DietController(ApplicationDbContext context) { _context = context; }
        
        /// <summary>
        /// Retrieves all the diets from the database along with their associated results and recipes.
        /// It returns a list of DietDTO objects containing relevant diet details.
        /// </summary>
        /// <returns>Returns a list of DietDTO objects if diets are found in the database. If no diets are found, returns a 404 error.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diet>>> GetAll()
        {
            var diets = await _context.Diets.Include(n => n.Results).Include(d => d.Recipes).ToListAsync();
            if (diets.Count == 0)
            {
                return NotFound("There are no diets in the database yet!");
            }

            // Map diets to DietDTO to avoid circular reference errors
            var dietDTO = diets.Select(n => new DietDTO
            {
                Id = n.Id,
                Name = n.Name,
                Characteristics = n.Characteristics != null ? n.Characteristics : "No info",
                UserId = n.UserId,
                Recipes = n.Recipes.Select(u => u.Id).ToList(),
                Results = n.Results.Select(r => r.Date).ToList()
            }).ToList();

            return Ok(dietDTO);
        }

        /// <summary>
        /// Retrieves a specific diet by its ID, including its associated results and recipes.
        /// It returns the diet as a DietDTO object if found.
        /// </summary>
        /// <param name="id">The ID of the diet to retrieve.</param>
        /// <returns>Returns a DietDTO object if the diet is found, or a 404 error if the diet is not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Diet>> GetById(int id)
        {
            var diet = await _context.Diets.Include(n => n.Results).Include(r => r.Recipes).FirstOrDefaultAsync(n => n.Id == id);
            if (diet == null)
            {
                return NotFound("Diet not found.");
            }

            var dietDTO = new DietDTO
            {
                Id = diet.Id,
                Name = diet.Name,
                Characteristics = diet.Characteristics != null ? diet.Characteristics : "No info",
                UserId = diet.UserId,
                Recipes = diet.Recipes.Select(u => u.Id).ToList(),
                Results = diet.Results.Select(r => r.Date).ToList()
            };

            return Ok(dietDTO);
        }


        /// <summary>
        /// Adds a new diet to the database based on the provided DietDTO object.
        /// The diet includes associated recipes and results that are mapped from the DTO.
        /// </summary>
        /// <param name="dietDTO">The DietDTO object containing the new diet's data.</param>
        /// <returns>Returns a 201 status with the created diet if successful, or a 400 error if there are issues with the provided data.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPost]
        public async Task<ActionResult<Diet>> Add(DietDTO dietDTO)
        {
            var diet = new Diet { Name = dietDTO.Name, Characteristics = dietDTO.Characteristics ?? "No info", UserId = dietDTO.UserId };

            // Add recipes and results
            foreach (var i in dietDTO.Recipes)
            {
                var recipe = await _context.Recipes.FindAsync(i);
                if (recipe != null)
                {
                    diet.Recipes.Add(await _context.Recipes.FindAsync(recipe));
                }
            }

            foreach (var i in dietDTO.Results)
            {
                var result = await _context.Results.FindAsync(i);
                if (result != null)
                {
                    diet.Results.Add(await _context.Results.FindAsync(result));
                }
            }

            try
            {
                _context.Diets.Add(diet);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), diet);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }


        /// <summary>
        /// Deletes a specific diet from the database by its ID.
        /// It returns the deleted diet if successful, or a 400 error if the deletion fails.
        /// </summary>
        /// <param name="id">The ID of the diet to delete.</param>
        /// <returns>Returns the deleted diet if successful, or a 400 error if the diet cannot be deleted.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<Diet>> Delete(int id)
        {
            var diet = await _context.Diets.FindAsync(id);
            try
            {
                _context.Diets.Remove(diet);
                _context.SaveChanges();
                return Ok(diet);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the diet.");
            }
        }


        /// <summary>
        /// Updates an existing diet by its ID with the data from the provided DietDTO object.
        /// It also updates the associated recipes and results of the diet.
        /// </summary>
        /// <param name="dietDTO">The DietDTO object containing the updated diet data.</param>
        /// <param name="id">The ID of the diet to update.</param>
        /// <returns>Returns the updated diet if successful, or a 400 error if the update fails.</returns>
    
        [Authorize(Roles = "Admin,Doctor")]

        [HttpPut("{id}")]
        public async Task<ActionResult<Diet>> Update(DietDTO dietDTO, int id)
        {
            var diet = await _context.Diets.Include(i => i.Recipes).Include(i => i.Results).FirstOrDefaultAsync(n => n.Id == id);

            if (diet == null)
            {
                return NotFound("Diet does not exist.");
            }

            diet.Name = dietDTO.Name;
            diet.Characteristics = dietDTO.Characteristics;
            diet.UserId = dietDTO.UserId;

            // Update recipes and results
            diet.Recipes.Clear();
            foreach (var rId in dietDTO.Recipes)
            {
                var result = await _context.Recipes.FindAsync(rId);
                if (result != null)
                {
                    diet.Recipes.Add(result);
                }
            }

            diet.Results.Clear();
            foreach (var rId in dietDTO.Results)
            {
                var result = await _context.Results.FindAsync(rId);
                if (result != null)
                {
                    diet.Results.Add(result);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(diet);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }


        /// <summary>
        /// Assigns or removes a diet for a specific user. If the diet is already assigned, it will be removed; otherwise, it will be assigned.
        /// </summary>
        /// <param name="id">The ID of the diet to assign or remove.</param>
        /// <param name="idUser">The ID of the user to whom the diet will be assigned or removed.</param>
        /// <returns>Returns a success message indicating whether the diet was assigned or removed, or a 400 error if the operation fails.</returns>

        [Authorize]

        [HttpPut("/Asign/{id}/{idUser}")]
        public async Task<ActionResult> AsignDiet(int id, string idUser)
        {
            var diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == id);
            var user = await _context.Users.OfType<User>().FirstOrDefaultAsync(n => n.Id == idUser);

            try
            {
                var isAssigned = diet.UserId == idUser;

                if (isAssigned)
                {
                    user.Diet = null;
                    diet.UserId = null;
                    diet.User = null;
                    _context.Update(user);
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                    return Ok($"Diet {diet.Name} for {user.Name} removed.");
                }
                else
                {
                    user.Diet = diet;
                    diet.User = user;
                    diet.UserId = user.Id;
                    _context.Update(user);
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                    return Ok($"Diet {diet.Name} for {user.Name} added.");
                }
            }
            catch (DbUpdateException)
            {

                return BadRequest("Could not assign or remove the diet.");
            }
        }
    }
}
