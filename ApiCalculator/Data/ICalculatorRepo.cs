using ApiCalculator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCalculator.Data
{
    /// <summary>
    /// Represents the set of methods for database access.
    /// </summary>
    public interface ICalculatorRepo
    {
        /// <summary>
        /// Tries to retrieve all HistoricalCalculation objects.
        /// </summary>
        /// <returns>A collection of HistoricalCalculation objects.</returns>
        IEnumerable<HistoricalCalculation> GetAllEquations();
        /// <summary>
        /// Tries to retrieve all HistoricalCalculation objects from a specific user.
        /// </summary>
        /// <returns>A collection of HistoricalCalculation objects.</returns>
        IEnumerable<HistoricalCalculation> GetAllUserEquations(string userId);

        /// <summary>
        /// Tries to create new Historical Calculation.
        /// </summary>
        /// <param name="historicalCalculation">Instance of <see cref="HistoricalCalculation"/></param>
        /// <returns>Added record with unique identifier.</returns>
        Task<HistoricalCalculation> Insert(HistoricalCalculation historicalCalculation);
        /// <summary>
        /// Tries to delete all HistoricalCalculation objects from a specific user.
        /// </summary>
        /// <param name="userId">User Unique identifier.</param>
        /// <returns>An awaitable task.</returns>
        Task DeleteByUser(string userId);
    }
}
