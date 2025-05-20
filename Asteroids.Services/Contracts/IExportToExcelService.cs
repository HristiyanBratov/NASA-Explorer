using Asteroids.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Services.Contracts
{
    public interface IExportToExcelService
    {
        byte[] ExportToExcel(List<Asteroid> asteroids);
    }
}