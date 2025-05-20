using Asteroids.Models;
using Asteroids.Services.Contracts;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Services.ApiModule
{
    public class ExportToExcelService : IExportToExcelService
    {
        /// <summary>
        ///     Generates an Excel file (.xlsx) containing a list of asteroids and details about them.
        /// </summary>
        /// <param name="asteroids">
        ///     The list of objects to export.
        /// </param>
        /// <returns>
        ///     A byte array representing the Excel file content.
        /// </returns>

        public byte[] ExportToExcel(List<Asteroid> asteroids)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Asteroids");

            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Close Approach Date";
            worksheet.Cell(1, 3).Value = "Diameter (km)";
            worksheet.Cell(1, 4).Value = "Velocity (km/h)";
            worksheet.Cell(1, 5).Value = "Miss Distance (km)";
            worksheet.Cell(1, 6).Value = "Potentially Hazardous";

            for (int i = 0; i < asteroids.Count; i++)
            {
                var a = asteroids[i];
                int row = i + 2;
                worksheet.Cell(row, 1).Value = a.Name;
                worksheet.Cell(row, 2).Value = a.CloseApproachDate;
                worksheet.Cell(row, 3).Value = a.EstimatedDiameterKm;
                worksheet.Cell(row, 4).Value = a.RelativeVelocityKph;
                worksheet.Cell(row, 5).Value = a.MissDistanceKm;
                worksheet.Cell(row, 6).Value = a.IsPotentiallyHazardous ? "Yes" : "No";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}