using Asteroids.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Services.Contracts
{
    public interface INasaApiService
    {
        public Task<List<Asteroid>> GetAsteroidsAsync(DateTime startDate, DateTime endDate);

        public Task<Apod> GetApodAsync(DateTime? date = null);
    }
}