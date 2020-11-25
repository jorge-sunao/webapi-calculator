using ApiCalculator.Authentication;
using ApiCalculator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiCalculator.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<OperationsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates new instance of <see cref="AuthenticateController"/>.
        /// </summary>
        /// <param name="logger">ILogger<OperationsController></param>
        /// <param name="userManager">UserManager<ApplicationUser></param>
        /// <param name="roleManager">RoleManager<IdentityRole></param>
        /// <param name="configuration">IConfiguration</param>
        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<OperationsController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model">Instance of <see cref="LoginModel"/>.</param>
        /// <response code="200">Login successfully and access token generated.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Parameter cannot be null.", nameof(model));
                }

                _logger.LogInformation($"Login attempt with: {model.Username}.");

                var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
                {
                    var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserId", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model">Instance of <see cref="RegisterModel"/>.</param>
        /// <response code="200">New user created.</response>
        /// <response code="409">Duplicated username.</response>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Parameter cannot be null.", nameof(model));
                }

                _logger.LogInformation($"Register new user attempt with: {model.Username}.");

                var userExists = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = "User already exists!" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                if (!await _roleManager.RoleExistsAsync(UserRoles.User).ConfigureAwait(false))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User)).ConfigureAwait(false);

                if (await _roleManager.RoleExistsAsync(UserRoles.User).ConfigureAwait(false))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User).ConfigureAwait(false);
                }

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Register a new admin user
        /// </summary>
        /// <param name="model">Instance of <see cref="RegisterModel"/>.</param>
        /// <response code="200">New admin user created.</response>
        /// <response code="409">Duplicated username.</response>
        [HttpPost]
        [Route("register-admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Parameter cannot be null.", nameof(model));
                }

                _logger.LogInformation($"Register new admin user attempt with: {model.Username}.");

                var userExists = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = "User already exists!" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin).ConfigureAwait(false))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin)).ConfigureAwait(false);

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin).ConfigureAwait(false))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin).ConfigureAwait(false);
                }

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
