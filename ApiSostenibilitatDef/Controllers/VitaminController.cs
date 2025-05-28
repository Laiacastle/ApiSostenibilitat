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
    public class VitaminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public VitaminController(ApplicationDbContext context) { _context = context; }
        /// <summary>
        /// Retrieves all the vitamins from the database.
        /// It returns a list of Vitamin objects, or a 404 error if no vitamins are found.
        /// </summary>
        /// <returns>Returns a list of Vitamin objects if vitamins are found, or a 404 error if not found.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vitamin>>> GetAll()
        {
            var vitamins = await _context.Vitamins.ToListAsync();
            if (vitamins.Count == 0)
            {
                return NotFound("There are no vitamins in the database yet!");
            }

            return Ok(vitamins);
        }

        /// <summary>
        /// Retrieves a specific vitamin by its ID.
        /// It returns a Vitamin object, or a 404 error if the vitamin is not found.
        /// </summary>
        /// <param name="id">The ID of the vitamin to retrieve.</param>
        /// <returns>Returns the Vitamin object if found, or a 404 error if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Vitamin>> GetById(int id)
        {
            var vitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Id == id);
            if (vitamin == null)
            {
                return NotFound("Vitamin not found.");
            }

            return Ok(vitamin);
        }


        /// <summary>
        /// Adds a new vitamin to the database.
        /// The new vitamin data is provided in the request body as a Vitamin object.
        /// </summary>
        /// <param name="vitamin">The Vitamin object containing the new vitamin data.</param>
        /// <returns>Returns a 201 status with the created vitamin if successful, or a 400 error if the data is invalid.</returns>

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPost]
        public async Task<ActionResult<Vitamin>> Add(Vitamin vitamin)
        {
            try
            {
                _context.Vitamins.Add(vitamin);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), vitamin);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }


        /// <summary>
        /// Deletes a specific vitamin from the database by its ID.
        /// If the vitamin is found, it will be removed and returned.
        /// </summary>
        /// <param name="id">The ID of the vitamin to delete.</param>
        /// <returns>Returns the deleted vitamin if successful, or a 400 error if the deletion fails.</returns>

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<Vitamin>> Delete(int id)
        {
            var vitamin = await _context.Vitamins.FindAsync(id);
            try
            {
                _context.Vitamins.Remove(vitamin);
                _context.SaveChanges();
                return Ok(vitamin);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the vitamin.");
            }
        }


        /// <summary>
        /// Updates an existing vitamin in the database based on the provided Vitamin object and its ID.
        /// The updated vitamin data is provided in the request body.
        /// </summary>
        /// <param name="vitamin">The Vitamin object containing the updated data.</param>
        /// <param name="id">The ID of the vitamin to update.</param>
        /// <returns>Returns the updated vitamin if successful, or a 400 error if the update fails.</returns>

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Vitamin>> Update(Vitamin vitamin, int id)
        {
            var newVitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Id == id);

            if (newVitamin == null)
            {
                return NotFound("Vitamin does not exist!");
            }

            newVitamin.Name = vitamin.Name;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(newVitamin);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }
    }
}

