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
    public class VitaminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public VitaminController(ApplicationDbContext context) { _context = context; }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vitamin>>> GetAll()
        {
            var vitamins = await _context.Vitamins.ToListAsync();
            if (vitamins.Count == 0)
            {
                return NotFound("Encara no hi han resultats a la base de dades!");
            }

            return Ok(vitamins);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vitamin>> GetById(int id)
        {
            var vitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Id == id);
            if (vitamin == null)
            {
                return NotFound("No s'ha trobat la vitamina");
            }
            
            return Ok(vitamin);
        }
        //[Authorize(Roles = "Admin")]
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
                return BadRequest("Dades erroneas");
            }
        }
        //[Authorize(Roles = "Admin")]
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
                return BadRequest("No s'ha pogut esborrar la vitamina");
            }
        }
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Vitamin>> Update(Vitamin vitamin, int id)
        {
            var newVitamin = await _context.Vitamins.FirstOrDefaultAsync(n => n.Id == id);

            if (newVitamin == null)
            {
                return NotFound("La vitamina no existeix!");
            }

            newVitamin.Name = vitamin.Name;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(newVitamin);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No s'ha pogut fer l'update");
            }
        }
    }
}

