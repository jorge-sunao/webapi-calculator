using System;

namespace ApiCalculator.Dtos
{
    /// <summary>
    /// DTO used for creating new records of Historica Calculation
    /// </summary>
    public class HistoricalCalculationCreateDto
    {

        public DateTime OperationDate { get; set; }

        public decimal FirstElement { get; set; }

        public char Operation { get; set; }

        public decimal SecondElement { get; set; }

        public decimal Result { get; set; }
        public string UserId { get; set; }
    }
}