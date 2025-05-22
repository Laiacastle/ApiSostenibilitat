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
    public class ResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ResultController(ApplicationDbContext context) { _context = context; }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetAll()
        {
            var results = await _context.Results.ToListAsync();
            if (results.Count == 0)
            {
                return NotFound("Encara no hi han resultats a la base de dades!");
            }

            // M apeamos para pasarlo al dto para evitar el error de infinidad
            var resultDTO = results.Select(n => new ResultDTO
            {
                Id =n.Id,
                Date = n.Date,
                UserId = n.UserId,
                GameId = n.GameId,
                DietId = n.DietId,
                FiResult = n.FiResult
            }).ToList();

            return Ok(resultDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> GetById(int id)
        {
            var result = await _context.Results.FirstOrDefaultAsync(n => n.Id == id);
            if (result == null)
            {
                return NotFound("No s'ha trobat el resultat");
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Result>> Add(ResultDTO resultDTO)
        {
            var result = new Result { UserId = resultDTO.UserId, GameId = resultDTO.GameId, DietId = resultDTO.DietId, FiResult = resultDTO.FiResult, Date = DateTime.Now };
            result.User = await _context.Users.FirstOrDefaultAsync(n => n.Id == resultDTO.UserId);
            result.Game = await _context.Games.FirstOrDefaultAsync(n => n.Id == resultDTO.GameId);
            result.Diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == resultDTO.DietId);
            try
            {
                _context.Results.Add(result);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), result);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Dades erroneas");
            }
        }
        [Authorize(Roles = "Admin")]
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
                return BadRequest("No s'ha pogut esborrar el resultat");
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> Update(ResultDTO resultDTO, int id)
        {
            var result = await _context.Results.FirstOrDefaultAsync(n => n.Id == id);

            if (result == null)
            {
                return NotFound("El resultat no existeix!");
            }

            result.UserId = resultDTO.UserId;
            result.GameId = resultDTO.GameId;
            result.DietId = resultDTO.DietId;
            
            result.FiResult = resultDTO.FiResult;
            result.User = await _context.Users.FirstOrDefaultAsync(n => n.Id == resultDTO.UserId);
            result.Game = await _context.Games.FirstOrDefaultAsync(n => n.Id == resultDTO.GameId);
            result.Diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == resultDTO.DietId);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(result);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No s'ha pogut fer l'update");
            }
        }
    }
}
