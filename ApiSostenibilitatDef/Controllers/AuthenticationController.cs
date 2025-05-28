using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models.DTOs;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        /// <summary>
        /// Retrieves all the users registered in the system. It returns a list of users with summarized information in DTO format.
        /// </summary>
        /// <returns>Returns a list of UserDTO objects if users are found in the database. If no users are found, returns a 404 error.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            var users = await _context.Users.OfType<User>().Include(n => n.Results).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound("There are no users in the database yet!");
            }

            var userDTO = users.Select(n => new UserDTO
            {
                Id = n.Id,
                Name = n.Name,
                Surname = n.Surname,
                Email = n.Email != null ? n.Email : "Error",
                Password = n.PasswordHash != null ? n.PasswordHash : "Error",
                UserName = n.UserName != null ? n.UserName : n.Name,
                Weight = n.Weight,
                Exercise = n.Exercise.ToString(),
                HoursSleep = n.HoursSleep,
                Age = n.Age,
                Results = n.Results.Select(r => r.FiResult).ToList(),
                Diet = n.Diet != null ? n.Diet.Id.ToString() : "No diet"
            }).ToList();

            return Ok(userDTO);
        }

        /// <summary>
        /// Registers a new user in the system. It assigns the data received in the RegisterDTO model to the new user.
        /// </summary>
        /// <param name="model">The RegisterDTO object that contains the new user's data.</param>
        /// <returns>Returns a success message if the registration is successful, or a 400 error with details if there are issues during registration.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDTO model)
        {
            var newUser = new User { Name = model.Name, Surname = model.Surname, UserName = model.UserName, Email = model.Email, Weight = model.Weight, Age = model.Age, HoursSleep = model.HoursSleep };
            switch (model.Exercise)
            {
                case "Molt": newUser.Exercise = ExerciciEnum.Molt; break;
                case "Mig": newUser.Exercise = ExerciciEnum.Mig; break;
                case "Poc": newUser.Exercise = ExerciciEnum.Poc; break;
                default: newUser.Exercise = ExerciciEnum.Res; break;
            }
            if(_userManager.Users.FirstOrDefault(u=>u.Email==newUser.Email) != null)
            {
                return BadRequest("Email already taken.");
            }
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                return Ok("User registered successfully.");
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Registers a new user and assigns the role of "Admin" to the user.
        /// </summary>
        /// <param name="model">The RegisterDTO object that contains the new user's data.</param>
        /// <returns>Returns a success message if the registration is successful and the "Admin" role is assigned, or a 400 error with details if there are issues.</returns>
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
                _logger.LogInformation($"Roles assigned to {user.UserName}: {string.Join(", ", resultRol)}");
            }
            if (result.Succeeded && resultRol.Succeeded)
            {
                return Ok("Admin registered successfully.");
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Registers a new user and assigns the role of "Doctor" to the user.
        /// </summary>
        /// <param name="model">The RegisterDTO object that contains the new user's data.</param>
        /// <returns>Returns a success message if the registration is successful and the "Doctor" role is assigned, or a 400 error with details if there are issues.</returns>
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
                _logger.LogInformation($"Roles assigned to {user.UserName}: {string.Join(", ", resultRol)}");
            }
            if (result.Succeeded && resultRol.Succeeded)
            {
                return Ok("Doctor registered successfully.");
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Allows a user to authenticate using their email and password, and returns a JWT if authentication is successful.
        /// </summary>
        /// <param name="model">The LoginDTO object that contains the user's email and password.</param>
        /// <returns>Returns a JWT token if the login is successful, or a 401 error if the email or password is incorrect.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Incorrect email or password.");
            }

            var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, user.UserName != null ? user.UserName : user.Name),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim("role", role));
                }
            }

            return Ok(CreateToken(claims.ToArray()));
        }

        /// <summary>
        /// Creates a JWT token for the authenticated user based on the provided claims.
        /// </summary>
        /// <param name="claims">An array of claims that contain user-related information such as username and roles.</param>
        /// <returns>Returns a JWT token as a string.</returns>
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

        /// <summary>
        /// Retrieves the claims associated with the authenticated user.
        /// </summary>
        /// <returns>Returns a list of the user's claims, including their name, identifier, and roles.</returns>
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}

