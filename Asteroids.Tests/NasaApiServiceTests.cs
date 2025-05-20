using Asteroids.Models.DTOs;
using Asteroids.Models;
using Asteroids.Services.ApiModule;
using Asteroids.Services.Contracts;
using Asteroids.Utility;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Asteroids.Tests
{
    public class NasaApiServiceTests
    {
        private HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage)
        {
            var mock = new Mock<HttpMessageHandler>();

            mock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(responseMessage);

            return new HttpClient(mock.Object);
        }

        private INasaApiService CreateService(HttpClient client)
        {
            var mockOptions = new Mock<IOptions<NasaApiSettings>>();
            mockOptions.Setup(o => o.Value).Returns(new NasaApiSettings { ApiKey = "DEMO_KEY" });

            return new NasaApiService(client, mockOptions.Object);
        }

        [Fact]
        public async Task GetApodAsync_ReturnsApod()
        {
            // Arrange
            var apodTest = new Apod
            {
                Title = "Test Title",
                Url = "https://example.com/image.jpg",
                Explanation = "Test Explanation",
                Date = "2025-05-20",
                Media_type = "image",
                Service_version = "v1"
            };

            var json = JsonSerializer.Serialize(apodTest);

            var client = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            var service = CreateService(client);

            // Act
            var result = await service.GetApodAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(apodTest.Title, result.Title);
            Assert.Equal(apodTest.Url, result.Url);
            Assert.Equal(apodTest.Date, result.Date);
        }

        [Fact]
        public async Task GetAsteroidsAsync_ReturnsAsteroidsList()
        {
            // Arrange
            var apiResponseTest = new AsteroidApiResponse
            {
                NearEarthObjects = new Dictionary<string, List<NeoObject>>
            {
                {
                    "2025-05-20", new List<NeoObject>
                    {
                        new NeoObject
                        {
                            Name = "Test Asteroid",
                            EstimatedDiameter = new EstimatedDiameter
                            {
                                Kilometers = new DiameterRange
                                {
                                    EstimatedDiameterMin = 0.5,
                                    EstimatedDiameterMax = 1.5
                                }
                            },
                            CloseApproachData = new List<CloseApproachData>
                            {
                                new CloseApproachData
                                {
                                    CloseApproachDate = "2025-05-20",
                                    RelativeVelocity = new RelativeVelocity { KilometersPerHour = "50000" },
                                    MissDistance = new MissDistance { Kilometers = "750000" }
                                }
                            },
                            IsPotentiallyHazardousAsteroid = true
                        }
                    }
                }
            }
            };

            var json = JsonSerializer.Serialize(apiResponseTest);

            var client = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            var service = CreateService(client);

            // Act
            var result = await service.GetAsteroidsAsync(new DateTime(2025, 5, 20), new DateTime(2025, 5, 20));

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var asteroid = result[0];

            Assert.Equal("Test Asteroid", asteroid.Name);
            Assert.True(asteroid.IsPotentiallyHazardous);
            Assert.InRange(asteroid.EstimatedDiameterKm, 0.5, 1.5);
            Assert.Equal(50000, asteroid.RelativeVelocityKph);
            Assert.Equal(750000, asteroid.MissDistanceKm);
            Assert.Equal("2025-05-20", asteroid.CloseApproachDate);
        }

    }
}