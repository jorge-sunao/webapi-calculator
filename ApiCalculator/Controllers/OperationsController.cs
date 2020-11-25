using ApiCalculator.Authentication;
using ApiCalculator.Data;
using ApiCalculator.Dtos;
using ApiCalculator.Models;
using ApiCalculator.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCalculator.Controllers
{
    /// <summary>
    /// Operations Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly ILogger<OperationsController> _logger;
        private readonly IMapper _mapper;
        private readonly ICalculatorService _calculatorService;
        private readonly ICalculatorRepo _calculatorRepo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Creates new instance of <see cref="OperationsController"/>.
        /// </summary>
        /// <param name="context">ApplicationDbContext</param>
        /// <param name="userManager">UserManager<ApplicationUser></param>
        /// <param name="logger">ILogger<OperationsController></param>
        /// <param name="mapper">IMapper</param>
        /// <param name="calculatorService">ICalculatorService</param>
        /// <param name="calculatorRepo">ICalculatorRepo</param>
        public OperationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<OperationsController> logger, IMapper mapper, ICalculatorService calculatorService, ICalculatorRepo calculatorRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _calculatorService = calculatorService;
            _calculatorRepo = calculatorRepo;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Calculate equation and add to historical data
        /// </summary>
        /// <param name="firstElement">First element of the operation (decimal)</param>
        /// <param name="operation">type of operation (+: addition, -: Subtraction, *: Multiplication, /: Division)</param>
        /// <param name="secondElement">Second element of the operation (decimal)</param>
        /// <returns>Equation result</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Calculate(decimal firstElement, char operation, decimal secondElement)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation($"Calculate: {firstElement} {operation} {secondElement}.");

                    HistoricalCalculation historicalCalculation = new HistoricalCalculation();
                    historicalCalculation.FirstElement = firstElement;
                    historicalCalculation.Operation = operation;
                    historicalCalculation.SecondElement = secondElement;

                    decimal equationResult = _calculatorService.CalculateEquation(historicalCalculation);

                    historicalCalculation.Result = equationResult;
                    historicalCalculation.OperationDate = DateTime.Now;

                    string userId = this.User.Claims.First(i => i.Type == "UserId").Value;
                    historicalCalculation.UserId = userId;

                    if (historicalCalculation.UserId == null)
                    {
                        return Unauthorized();
                    }

                    _logger.LogInformation($"Equation Result: {equationResult}.");

                    _logger.LogInformation($"Create {historicalCalculation} calculation API call.");

                    HistoricalCalculation createdHistoricalCalculation = await _calculatorRepo.Insert(historicalCalculation).ConfigureAwait(false);
                    return Ok(equationResult);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get calculation history of current user
        /// </summary>
        /// <returns>Calculation history of current user</returns>
        [Authorize]
        [HttpGet("user-history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<HistoricalCalculationReadDto>> UserHistory()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("History calculations API call.");
                    string userId = this.User.Claims.First(i => i.Type == "UserId").Value;
                    var equations = _calculatorRepo.GetAllUserEquations(userId);

                    if (!equations.Any())
                    {
                        _logger.LogWarning("History is empty.");
                        return NotFound();
                    }

                    return Ok(_mapper.Map<IEnumerable<HistoricalCalculationReadDto>>(equations));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get calculation history of all users (requires admin access)
        /// </summary>
        /// <returns>Calculation history of all users</returns>
        [Authorize]
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("admin-history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<HistoricalCalculationReadDto>> AdminHistory()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("History calculations API call.");
                    var equations = _calculatorRepo.GetAllEquations();

                    if (!equations.Any())
                    {
                        _logger.LogWarning("History is empty.");
                        return NotFound();
                    }

                    return Ok(_mapper.Map<IEnumerable<HistoricalCalculationReadDto>>(equations));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Clean current user history
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ClearUserHistory()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userId = this.User.Claims.First(i => i.Type == "UserId").Value;
                    _logger.LogInformation($"Clearing calculation history from User ID {userId}.");

                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogError("Invalid user");
                        return Unauthorized();
                    }

                    await _calculatorRepo.DeleteByUser(userId);

                    return NoContent();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
