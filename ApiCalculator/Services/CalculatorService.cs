using ApiCalculator.Models;
using System;
using System.Text.RegularExpressions;

namespace ApiCalculator.Services
{
    public class CalculatorService : ICalculatorService
    {
        public bool ValidateEquation(HistoricalCalculation historicalCalculation)
        {
            try
            {
                if (historicalCalculation == null)
                {
                    throw new ArgumentException("There is no equation to validate.", nameof(historicalCalculation));
                }

                var operators = new Regex(@"[+*/-]");
                var checkOperator = operators.Matches(historicalCalculation.Operation.ToString());

                if (checkOperator.Count == 1)
                {
                    if (historicalCalculation.Operation == '/' && historicalCalculation.SecondElement == 0)
                    {
                        throw new InvalidOperationException("The divisor of a division cannot be 0.");
                    }

                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Operator '{historicalCalculation.Operation}' is invalid for this calculator.");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal CalculateEquation(HistoricalCalculation historicalCalculation)
        {
            try
            {
                decimal equationResult;
                if (ValidateEquation(historicalCalculation))
                {
                    switch (historicalCalculation.Operation)
                    {
                        case '+':
                            equationResult = historicalCalculation.FirstElement + historicalCalculation.SecondElement;
                            break;
                        case '-':
                            equationResult = historicalCalculation.FirstElement - historicalCalculation.SecondElement;
                            break;
                        case '*':
                            equationResult = historicalCalculation.FirstElement * historicalCalculation.SecondElement;
                            break;
                        case '/':
                            equationResult = historicalCalculation.FirstElement / historicalCalculation.SecondElement;
                            break;
                        default:
                            throw new InvalidOperationException($"Operator '{historicalCalculation.Operation}' is invalid for Calculator API.");
                    }

                    return equationResult;
                }
                else
                {
                    throw new InvalidOperationException("Equation is invalid.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
