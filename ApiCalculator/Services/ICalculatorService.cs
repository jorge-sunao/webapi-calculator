using ApiCalculator.Models;

namespace ApiCalculator.Services
{
    /// <summary>
    /// Represents the set of methods for Calculation.
    /// </summary>
    public interface ICalculatorService
    {
        /// <summary>
        /// Calculate equation.
        /// </summary>
        /// <param name="historicalCalculation">Instance of <see cref="HistoricalCalculation"/></param>
        /// <returns>Equation result.</returns>
        decimal CalculateEquation(HistoricalCalculation historicalCalculation);
        /// <summary>
        /// Validate equation elements.
        /// </summary>
        /// <param name="historicalCalculation">Instance of <see cref="HistoricalCalculation"/></param>
        /// <returns>Equation validity (true = valid, false = invalid).</returns>
        bool ValidateEquation(HistoricalCalculation historicalCalculation);
    }
}
