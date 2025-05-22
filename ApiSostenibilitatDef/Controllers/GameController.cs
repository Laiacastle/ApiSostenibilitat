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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAll()
        {
            var games = await _context.Games.Include(n => n.Results).ToListAsync();
            if (games.Count == 0)
            {
                return NotFound("Encara no hi han jocs a la base de dades!");
            }

            // M apeamos para pasarlo al dto para evitar el error de infinidad
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetById(int id)
        {
            var game = await _context.Games.Include(n => n.Results).FirstOrDefaultAsync(n => n.Id == id);
            if (game == null)
            {
                return NotFound("No s'ha trobat el joc");
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Game>> Add(GameDTO gameDTO)
        {
            var game = new Game { Type = gameDTO.Type, MinRes = gameDTO .MinRes, MaxRes = gameDTO .MaxRes};

            //Afegim els results 
            
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
                return BadRequest("Dades erroneas");
            }
        }
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
                return BadRequest("No s'ha pogut esborrar el joc");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Game>> Update(GameDTO gameDTO, int id)
        {
            var game = await _context.Games.Include(i => i.Results).FirstOrDefaultAsync(n => n.Id == id);

            if (game == null)
            {
                return NotFound("L'ingredient no existeix!");
            }


            game.Type = gameDTO.Type;
            game.MinRes = gameDTO.MinRes;
            game.MaxRes = gameDTO.MaxRes;

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
                return BadRequest("No s'ha pogut fer l'update");
            }
        }
        
        
    }
}
