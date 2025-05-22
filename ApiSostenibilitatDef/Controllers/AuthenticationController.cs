using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiSostenibilitat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<User> userManager, ILogger<AuthenticationController> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _context.Users.OfType<User>().Include(n => n.Results).Include(n=>n.Diet).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound("Encara no hi han usuaris a la base de dades!");
            }
            var u = users.Select(n => n.Diet);
            var user = users.Select(n => new UserDTO { Diet = n.Diet != null ? n.Diet.Id.ToString() : "Sense dieta" });
            //mapeamos para q no haya el error de infinidad
            var userDTO = users.Select(n => new UserDTO
            {
                Id = n.Id,
                Name = n.Name,
                Surname = n.Surname,
                Email = n.Email != null ? n.Email : "Error",
                Password = n.PasswordHash != null ? n.PasswordHash : "Error",
                UserName = n.UserName != null  ? n.UserName: n.Name,
                Weight = n.Weight,
                Exercise = n.Exercise.ToString(),
                HoursSleep = n.HoursSleep,
                Age = n.Age,
                Results = n.Results.Select(r => r.FiResult).ToList(),
                Diet = n.Diet != null ? n.Diet.Id.ToString() : "Sense dieta"
            }).ToList();

            return Ok(userDTO);
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDTO model)
        {
            var newUser = new User { Name = model.Name, Surname=model.Surname, UserName = model.UserName, Email = model.Email, Weight = model.Weight, Age = model.Age, HoursSleep = model.HoursSleep };
            switch (model.Exercise)
            {
                case "Molt": newUser.Exercise = ExerciciEnum.Molt; break;
                case "Mig": newUser.Exercise = ExerciciEnum.Mig; break;
                case "Poc": newUser.Exercise = ExerciciEnum.Poc; break;
                default: newUser.Exercise = ExerciciEnum.Res;break;
            }
            var resultat = await _userManager.CreateAsync(newUser, model.Password);
            if (resultat.Succeeded)
            {
                return Ok("Usuari registrat");
            }
            return BadRequest(resultat.Errors);

        }

        [HttpPost("admin/registre")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterDTO model)
        {
            var user = new User { Name = model.Name, Surname = model.Surname, UserName = model.UserName, Email = model.Email, Weight = model.Weight, Age = model.Age, HoursSleep = model.HoursSleep };
            switch (model.Exercise)
            {
                case "Molt": user.Exercise = ExerciciEnum.Molt; break;
                case "Mig": user.Exercise = ExerciciEnum.Mig; break;
                case "Poc": user.Exercise = ExerciciEnum.Poc; break;
                default: user.Exercise = ExerciciEnum.Res; break;
            }
            var result = await _userManager.CreateAsync(user, model.Password);
            var resultRol = new IdentityResult();
            if (result.Succeeded)
            {
                resultRol = await _userManager.AddToRoleAsync(user, "Admin");
                _logger.LogInformation($"Rols assignats a {user.UserName}: {string.Join(", ", resultRol)}");
            }
            if (result.Succeeded && resultRol.Succeeded)
            {
                return Ok("Administrador registrat");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("doctor/registre")]
        public async Task<IActionResult> DoctorRegister([FromBody] RegisterDTO model)
        {
            var user = new User { Name = model.Name, Surname = model.Surname, UserName = model.UserName, Email = model.Email, Weight = model.Weight, Age = model.Age, HoursSleep = model.HoursSleep };
            switch (model.Exercise)
            {
                case "Molt": user.Exercise = ExerciciEnum.Molt; break;
                case "Mig": user.Exercise = ExerciciEnum.Mig; break;
                case "Poc": user.Exercise = ExerciciEnum.Poc; break;
                default: user.Exercise = ExerciciEnum.Res; break;
            }
            var result = await _userManager.CreateAsync(user, model.Password);
            var resultRol = new IdentityResult();
            if (result.Succeeded)
            {
                resultRol = await _userManager.AddToRoleAsync(user, "Doctor");
                _logger.LogInformation($"Rols assignats a {user.UserName}: {string.Join(", ", resultRol)}");
            }
            if (result.Succeeded && resultRol.Succeeded)
            {
                return Ok("Doctor registrat");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Mail o contrasenya erronis");
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName != null ? user.UserName : user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Count > 0)
            {
                foreach (var rol in roles)
                {
                    claims.Add(new Claim("role", rol));
                }
            }
            return Ok(CreateToken(claims.ToArray()));
        }

        private string CreateToken(Claim[] claims)
        {
            var jwtConfig = _configuration.GetSection("jwtSettings");
            var secretKey = jwtConfig["Key"];
            var issuer = jwtConfig["Issuer"];
            var audience = jwtConfig["Audience"];
            var expirationMinutes = int.Parse(jwtConfig["ExpirationMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}
