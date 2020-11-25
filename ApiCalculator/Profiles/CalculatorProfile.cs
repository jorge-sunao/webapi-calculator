using ApiCalculator.Dtos;
using ApiCalculator.Models;
using AutoMapper;

namespace ApiCalculator.Profiles
{
    /// <summary>
    /// Automapper Profile
    /// </summary>
    public class CalculatorProfile : Profile
    {
        public CalculatorProfile()
        {
            //Source -> Target
            CreateMap<HistoricalCalculation, HistoricalCalculationReadDto>();
            //Target -> Source
            CreateMap<HistoricalCalculationReadDto, HistoricalCalculation>();
            CreateMap<HistoricalCalculationCreateDto, HistoricalCalculation>();
        }
    }
}
