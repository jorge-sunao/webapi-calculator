using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace ApiCalculator
{
    /// <summary>
    /// Where all begins.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>A task.</returns>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs\\log.txt")
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application start-up failed");
            }

            finally
            {
                Log.CloseAndFlush();
            }


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}