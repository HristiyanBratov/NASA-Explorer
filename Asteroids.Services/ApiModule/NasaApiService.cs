using Asteroids.Models;
using Asteroids.Models.DTOs;
using Asteroids.Services.Contracts;
using Asteroids.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Services.ApiModule
{
    public class NasaApiService : INasaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NasaApiService(HttpClient httpClient, IOptions<NasaApiSettings> settings)
        {
            _httpClient = httpClient;
            _apiKey = settings.Value.ApiKey;
        }

        /// <summary>
        ///     Fetches asteroids data from the NASA Near Earth Object (NEO) REST API for a specified date range.
        /// </summary>
        /// <param name="startDate">
        ///     The start date of the range to fetch asteroid data.
        /// </param>
        /// <param name="endDate">
        ///     The end date of the range to fetch asteroid data.
        /// </param>
        /// <returns>
        ///     A list containing asteroids detected between the given dates, or an empty list if none found.
        /// </returns>

        public async Task<List<Asteroid>> GetAsteroidsAsync(DateTime startDate, DateTime endDate)
        {
            var formattedStartDate = startDate.ToString("yyyy-MM-dd");
            var formattedEndDate = endDate.ToString("yyyy-MM-dd");

            var url = 
                $"https://api.nasa.gov/neo/rest/v1/feed?start_date={formattedStartDate}&end_date={formattedEndDate}&api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<AsteroidApiResponse>();

            var asteroidList = new List<Asteroid>();

            if(apiResponse?.NearEarthObjects != null)
            {
                foreach(var dateEntry in apiResponse.NearEarthObjects)
                {
                    foreach(var neo in dateEntry.Value)
                    {
                        var diameter = neo.EstimatedDiameter?.Kilometers;
                        var approach = neo.CloseApproachData?.FirstOrDefault();

                        if (diameter == null && approach == null)
                            continue;

                        double.TryParse(approach.RelativeVelocity?.KilometersPerHour, out double velocityKph);
                        double.TryParse(approach.MissDistance?.Kilometers, out double missKm);

                        asteroidList.Add(new Asteroid
                        {
                            Name = neo.Name,
                            CloseApproachDate = approach.CloseApproachDate,
                            EstimatedDiameterKm = (diameter.EstimatedDiameterMin + diameter.EstimatedDiameterMax) / 2,
                            RelativeVelocityKph = velocityKph,
                            MissDistanceKm = missKm,
                            IsPotentiallyHazardous = neo.IsPotentiallyHazardousAsteroid
                        });
                    }
                }
            }

            return asteroidList;
        }

        /// <summary>
        ///     Retrieves NASA's Astronomy Picture of the Day (APOD) for a specified date.
        /// </summary>
        /// <param name="date">
        ///     The date for which to retrieve the APOD. If null, retrieves today's APOD.
        /// </param>
        /// <returns>
        ///    An object containing the APOD details for the specified date.
        /// </returns>

        public async Task<Apod> GetApodAsync(DateTime? date = null)
        {
            string dateParam = date.HasValue ? $"&date={date.Value:yyyy-MM-dd}" : "";
            string url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}{dateParam}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            Apod apod = await response.Content.ReadFromJsonAsync<Apod>();

            return apod;
        }
    }
}