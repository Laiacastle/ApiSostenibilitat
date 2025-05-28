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
    public class ResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ResultController(ApplicationDbContext context) { _context = context; }
        /// <summary>
        /// Retrieves all the results from the database. 
        /// It returns a list of ResultDTO objects, each representing a result with user, game, diet, and final result data.
        /// </summary>
        /// <returns>Returns a list of ResultDTO objects if results are found, or a 404 error if no results exist in the database.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetAll()
        {
            var results = await _context.Results.ToListAsync();
            if (results.Count == 0)
            {
                return NotFound("There are no results in the database yet!");
            }

            // Map results to ResultDTO to avoid circular reference issues
            var resultDTO = results.Select(n => new ResultDTO
            {
                Id = n.Id,
                Date = n.Date,
                UserId = n.UserId,
                GameId = n.GameId,
                DietId = n.DietId,
                FiResult = n.FiResult
            }).ToList();

            return Ok(resultDTO);
        }

        /// <summary>
        /// Retrieves a specific result by its ID. 
        /// It returns a ResultDTO object that contains the result details, including user, game, diet, and final result data.
        /// </summary>
        /// <param name="id">The ID of the result to retrieve.</param>
        /// <returns>Returns a ResultDTO object if the result is found, or a 404 error if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> GetById(int id)
        {
            var result = await _context.Results.FirstOrDefaultAsync(n => n.Id == id);
            if (result == null)
            {
                return NotFound("Result not found.");
            }

            var resultDTO = new ResultDTO
            {
                Id = result.Id,
                Date = result.Date,
                UserId = result.UserId,
                GameId = result.GameId,
                DietId = result.DietId,
                FiResult = result.FiResult
            };

            return Ok(resultDTO);
        }

        /// <summary>
        /// Adds a new result to the database based on the provided ResultDTO object. 
        /// It includes the user, game, and diet details by fetching them from their respective tables using the IDs from the DTO.
        /// </summary>
        /// <param name="resultDTO">The ResultDTO object containing the new result's data.</param>
        /// <returns>Returns a 201 status with the created result if successful, or a 400 error if the provided data is invalid.</returns>
        [HttpPost]
        public async Task<ActionResult<Result>> Add(ResultDTO resultDTO)
        {
            var result = new Result
            {
                UserId = resultDTO.UserId,
                GameId = resultDTO.GameId,
                DietId = resultDTO.DietId,
                FiResult = resultDTO.FiResult,
                Date = DateTime.Now
            };

            try
            {
                result.User = await _context.Users.FirstOrDefaultAsync(n => n.Id == resultDTO.UserId);
                result.Game = await _context.Games.FirstOrDefaultAsync(n => n.Id == resultDTO.GameId);
                result.Diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == resultDTO.DietId);

                _context.Results.Add(result);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), result);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }

        /// <summary>
        /// Deletes a specific result from the database by its ID.
        /// It returns the deleted result if successful, or a 400 error if the deletion fails.
        /// </summary>
        /// <param name="id">The ID of the result to delete.</param>
        /// <returns>Returns the deleted result if successful, or a 400 error if the result cannot be deleted.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> Delete(int id)
        {
            var result = await _context.Results.FindAsync(id);
            try
            {
                _context.Results.Remove(result);
                _context.SaveChanges();
                return Ok(result);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the result.");
            }
        }

        /// <summary>
        /// Updates an existing result in the database based on the provided ResultDTO object and result ID. 
        /// It also updates the associated user, game, and diet details.
        /// </summary>
        /// <param name="resultDTO">The ResultDTO object containing the updated result's data.</param>
        /// <param name="id">The ID of the result to update.</param>
        /// <returns>Returns the updated result if successful, or a 400 error if the update fails.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> Update(ResultDTO resultDTO, int id)
        {
            var result = await _context.Results.FirstOrDefaultAsync(n => n.Id == id);

            if (result == null)
            {
                return NotFound("The result does not exist.");
            }

            result.UserId = resultDTO.UserId;
            result.GameId = resultDTO.GameId;
            result.DietId = resultDTO.DietId;
            result.FiResult = resultDTO.FiResult;

            try
            {
                result.User = await _context.Users.FirstOrDefaultAsync(n => n.Id == resultDTO.UserId);
                result.Game = await _context.Games.FirstOrDefaultAsync(n => n.Id == resultDTO.GameId);
                result.Diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == resultDTO.DietId);

                await _context.SaveChangesAsync();
                return Ok(result);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }
    }
}
