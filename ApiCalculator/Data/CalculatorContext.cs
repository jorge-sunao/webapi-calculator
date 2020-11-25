using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCalculator.Models;
using ApiCalculator.Authentication;

namespace ApiCalculator.Data
{
    public class CalculatorContext : DbContext
    {
        public CalculatorContext(DbContextOptions<CalculatorContext> opt):base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<HistoricalCalculation> HistoricalCalculations { get; set; }
    }
}
