using System;

namespace ApiCalculator.Dtos
{
    /// <summary>
    /// DTO used for reading records of Historica Calculation
    /// </summary>
    public class HistoricalCalculationReadDto
    {
        public int Id { get; set; }

        public DateTime OperationDate { get; set; }

        public decimal FirstElement { get; set; }

        public char Operation { get; set; }

        public decimal SecondElement { get; set; }

        public decimal Result { get; set; }
    }
}