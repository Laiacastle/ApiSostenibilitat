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
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GameController(ApplicationDbContext context) { _context = context; }
        /// <summary>
        /// Retrieves all the games from the database, including their associated results.
        /// It returns a list of GameDTO objects with game details, including the results IDs.
        /// </summary>
        /// <returns>Returns a list of GameDTO objects if games are found. If no games are found, returns a 404 error.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAll()
        {
            var games = await _context.Games.Include(n => n.Results).ToListAsync();
            if (games.Count == 0)
            {
                return NotFound("There are no games in the database yet!");
            }

            // Map games to GameDTO to avoid circular reference errors
            var gameDTO = games.Select(n => new GameDTO
            {
                Id = n.Id,
                MinRes = n.MinRes,
                MaxRes = n.MaxRes,
                Type = n.Type,
                Results = n.Results.Select(r => r.Id).ToList()
            }).ToList();

            return Ok(gameDTO);
        }

        /// <summary>
        /// Retrieves a specific game by its ID, including the associated results.
        /// It returns the game as a GameDTO object if found.
        /// </summary>
        /// <param name="id">The ID of the game to retrieve.</param>
        /// <returns>Returns a GameDTO object if the game is found, or a 404 error if the game is not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetById(int id)
        {
            var game = await _context.Games.Include(n => n.Results).FirstOrDefaultAsync(n => n.Id == id);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            var gameDTO = new GameDTO
            {
                Id = game.Id,
                Type = game.Type,
                MinRes = game.MinRes,
                MaxRes = game.MaxRes,
                Results = game.Results.Select(r => r.Id).ToList()
            };

            return Ok(gameDTO);
        }

        /// <summary>
        /// Adds a new game to the database based on the provided GameDTO object.
        /// The game includes associated results mapped from the DTO.
        /// </summary>
        /// <param name="gameDTO">The GameDTO object containing the new game's data.</param>
        /// <returns>Returns a 201 status with the created game if successful, or a 400 error if the provided data is invalid.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Game>> Add(GameDTO gameDTO)
        {
            var game = new Game { Type = gameDTO.Type, MinRes = gameDTO.MinRes, MaxRes = gameDTO.MaxRes };

            // Add results to the game
            foreach (var i in gameDTO.Results)
            {
                var result = await _context.Results.FindAsync(i);
                if (result != null)
                {
                    game.Results.Add(result);
                }
            }

            try
            {
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), game);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Invalid data.");
            }
        }


        /// <summary>
        /// Deletes a specific game from the database by its ID.
        /// It returns the deleted game if successful, or a 400 error if the deletion fails.
        /// </summary>
        /// <param name="id">The ID of the game to delete.</param>
        /// <returns>Returns the deleted game if successful, or a 400 error if the game cannot be deleted.</returns>

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<Game>> Delete(int id)
        {
            var game = await _context.Games.FindAsync(id);
            try
            {
                _context.Games.Remove(game);
                _context.SaveChanges();
                return Ok(game);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete the game.");
            }
        }        /// <summary>
        /// Updates an existing game by its ID with the data from the provided GameDTO object.
        /// It also updates the associated results of the game.
        /// </summary>
        /// <param name="gameDTO">The GameDTO object containing the updated game data.</param>
        /// <param name="id">The ID of the game to update.</param>
        /// <returns>Returns the updated game if successful, or a 400 error if the update fails.</returns>

        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<ActionResult<Game>> Update(GameDTO gameDTO, int id)
        {
            var game = await _context.Games.Include(i => i.Results).FirstOrDefaultAsync(n => n.Id == id);

            if (game == null)
            {
                return NotFound("Game does not exist.");
            }

            game.Type = gameDTO.Type;
            game.MinRes = gameDTO.MinRes;
            game.MaxRes = gameDTO.MaxRes;

            // Update the results
            game.Results.Clear();
            foreach (var rId in gameDTO.Results)
            {
                var result = await _context.Results.FindAsync(rId);
                if (result != null)
                {
                    game.Results.Add(result);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(game);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Update failed.");
            }
        }
    }
}
