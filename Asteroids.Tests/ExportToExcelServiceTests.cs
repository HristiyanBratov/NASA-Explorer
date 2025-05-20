using Asteroids.Models;
using Asteroids.Services.ApiModule;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Tests
{
    public class ExportToExcelServiceTests
    {
        [Fact]
        public void ExportToExcel_ReturnsValidExcelFile()
        {
            // Arrange
            var service = new ExportToExcelService();
            var asteroids = new List<Asteroid>
            {
                new Asteroid
                {
                    Name = "Test Asteroid",
                    CloseApproachDate = "2025-05-20",
                    EstimatedDiameterKm = 1.23,
                    RelativeVelocityKph = 45678.9,
                    MissDistanceKm = 789000,
                    IsPotentiallyHazardous = true
                }
            };

            // Act
            var result = service.ExportToExcel(asteroids);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            using var stream = new MemoryStream(result);
            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet("Asteroids");

            Assert.Equal("Name", worksheet.Cell(1, 1).GetValue<string>());
            Assert.Equal("Test Asteroid", worksheet.Cell(2, 1).GetValue<string>());
            Assert.Equal("Yes", worksheet.Cell(2, 6).GetValue<string>());
        }
    }
}