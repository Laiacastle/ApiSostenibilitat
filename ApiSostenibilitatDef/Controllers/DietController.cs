﻿using ApiSostenibilitat.Data;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diet>>> GetAll()
        {
            var diets = await _context.Diets.Include(n => n.Results).Include(d => d.Recipes).ToListAsync();
            if (diets.Count == 0)
            {
                return NotFound("Encara no hi han dietes a la base de dades!");
            }

            // Mapeamos para pasarlo al dto para evitar el error de infinidad
            var dietDTO = diets.Select(n => new DietDTO
            {
                Id = n.Id,
                Name = n.Name,
                Characteristics = n.Characteristics != null ? n.Characteristics : "no info",
                UserId = n.UserId,
                Recipes = n.Recipes.Select(u => u.Id).ToList(),
                Results = n.Results.Select(r => r.Date).ToList()
            }).ToList();

            return Ok(dietDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Diet>> GetById(int id)
        {
            var diet = await _context.Diets.Include(n => n.Results).Include(r => r.Recipes).FirstOrDefaultAsync(n => n.Id == id);
            if (diet == null)
            {
                return NotFound("No s'ha trobat la dieta");
            }
            var dietDTO = new DietDTO
            {
                Id = diet.Id,
                Name = diet.Name,
                Characteristics = diet.Characteristics != null ? diet.Characteristics : "no info",
                UserId = diet.UserId,
                Recipes = diet.Recipes.Select(u => u.Id).ToList(),
                Results = diet.Results.Select(r => r.Date).ToList()
            };
            return Ok(dietDTO);
        }
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<ActionResult<Diet>> Add(DietDTO dietDTO)
        {
            var diet = new Diet { Name = dietDTO.Name, Characteristics = dietDTO.Characteristics !=null ? dietDTO.Characteristics: "no info", UserId = dietDTO.UserId};

            //Afegim els results i recipes
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
                return BadRequest("Dades erroneas");
            }
        }
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
                return BadRequest("No s'ha pogut esborrar la dieta");
            }
        }
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Diet>> Update(DietDTO dietDTO, int id)
        {
            var diet = await _context.Diets.Include(i => i.Recipes).Include(i => i.Results).FirstOrDefaultAsync(n => n.Id == id);

            if (diet == null)
            {
                return NotFound("L'ingredient no existeix!");
            }


            diet.Name = dietDTO.Name;
            diet.Characteristics = dietDTO.Characteristics;
            diet.UserId = dietDTO.UserId;


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
                return BadRequest("No s'ha pogut fer l'update");
            }
        }
        [Authorize]
        [HttpPut("/Asign/{id}/{idUser}")]
        public async Task<ActionResult> AsignDiet(int id, string idUser)
        {
            var diet = await _context.Diets.FirstOrDefaultAsync(n => n.Id == id);
            var user = await _context.Users.OfType<User>().FirstOrDefaultAsync(n => n.Id == idUser);
            try
            {
                var Asigned = diet.UserId == idUser;

                if (Asigned)
                {
                    user.Diet = null;
                    diet.UserId = null;
                    diet.User = null;
                    _context.Update(user);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok($"Dieta {diet.Name} per {user.Name} eliminada");
                }
                else
                {
                    user.Diet = diet;
                    diet.User = user;
                    diet.UserId = user.Id;
                    _context.Update(user);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok($"Dieta {diet.Name} per {user.Name} afegida");
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest("No s'ha pogut asignar la dieta");
            }
        }
    }
}
