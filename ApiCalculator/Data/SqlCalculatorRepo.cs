using ApiCalculator.Authentication;
using ApiCalculator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCalculator.Data
{
    public class SqlCalculatorRepo : ICalculatorRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SqlCalculatorRepo> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public SqlCalculatorRepo(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<SqlCalculatorRepo> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IEnumerable<HistoricalCalculation> GetAllEquations()
        {
            try
            {
                _logger.LogInformation("Executing GetAllEquations.");
                return _context.HistoricalCalculations.OrderByDescending(c => c.OperationDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                throw;
            }
        }

        public IEnumerable<HistoricalCalculation> GetAllUserEquations(string userId)
        {
            try
            {
                _logger.LogInformation("Executing GetAllEquations.");
                return _context.HistoricalCalculations.Where(p => p.UserId == userId).OrderByDescending(c => c.OperationDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                throw;
            }
        }

        public async Task<HistoricalCalculation> Insert(HistoricalCalculation historicalCalculation)
        {
            try
            {
                _logger.LogInformation($"Executing Insert {historicalCalculation}.");

                await _context.HistoricalCalculations.AddAsync(historicalCalculation).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return historicalCalculation;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                throw;
            }
        }

        public async Task DeleteByUser(string userId)
        {
            try
            {
                _logger.LogInformation($"Executing Delete for UserId: {userId}.");
                if (!string.IsNullOrEmpty(userId))
                {
                    var historicalCalculation = _context.HistoricalCalculations.Where(p => p.UserId == userId).ToList();

                    if (historicalCalculation is null) return;
                    _context.HistoricalCalculations.RemoveRange(historicalCalculation);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                throw;
            }
        }
    }
}
