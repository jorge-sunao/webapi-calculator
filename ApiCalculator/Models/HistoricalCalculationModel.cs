using ApiCalculator.Authentication;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiCalculator.Models
{
    /// <summary>
    /// Historica Calculation
    /// </summary>
    public class HistoricalCalculation
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Date equation was executed
        /// </summary>
        [Required(ErrorMessage = "Operation Date is required")]
        public DateTime OperationDate { get; set; }
        /// <summary>
        /// Equation's first element
        /// </summary>
        [Required(ErrorMessage = "First Element is required")]
        public decimal FirstElement { get; set; }
        /// <summary>
        /// Equation's operator (+, -, * or /)
        /// </summary>
        [Required(ErrorMessage = "Operator is required")]
        [RegularExpression("[+-*/]$", ErrorMessageResourceType = typeof(char), ErrorMessageResourceName = "Invalid Math Operator. Use only +, -, * or /")]
        public char Operation { get; set; }
        /// <summary>
        /// Equation's second element
        /// </summary>
        [Required(ErrorMessage = "Second Element is required")]
        public decimal SecondElement { get; set; }
        /// <summary>
        /// Equation's result
        /// </summary>
        public decimal Result { get; set; }

        /// <summary>
        /// User ID of the owner of the operation
        /// </summary>
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}