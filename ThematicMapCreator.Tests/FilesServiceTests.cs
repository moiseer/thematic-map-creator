using System;
using System.Linq;
using BAMCIS.GeoJSON;
using ThematicMapCreator.Api.Services;
using Xunit;

namespace ThematicMapCreator.Tests
{
    public class FilesServiceTests
    {
        private readonly FilesService service;

        public FilesServiceTests()
        {
            service = new FilesService();
        }

        [Fact]
        public void ConvertCsvToGeoJson_Success()
        {
            var nameHeader = "name";
            var points = new[]
            {
                new TestPoint { Latitude = 10.5, Longitude = 50.2, Name = "Point 1" },
                new TestPoint { Latitude = 20.2, Longitude = 34.5, Name = "Point 2" }
            };

            var csv = points.Aggregate($"lat; lng; {nameHeader}", (result, point) => $"{result}\r\n{point.Latitude}; {point.Longitude}; {point.Name}");

            var featureCollection = service.ConvertCsvToGeoJson(csv);

            Assert.NotNull(featureCollection?.Features);
            Assert.Equal(2, featureCollection.Features.Count());
            var features = featureCollection.Features.ToArray();

            Assert.IsType<Point>(features[0].Geometry);
            var point0 = features[0].Geometry as Point;
            Assert.Equal(points[0].Latitude, point0.GetLatitude());
            Assert.Equal(points[0].Longitude, point0.GetLongitude());
            Assert.Equal(points[0].Name, features[0].Properties[nameHeader]);

            Assert.IsType<Point>(features[0].Geometry);
            var point1 = features[1].Geometry as Point;
            Assert.Equal(points[1].Latitude, point1.GetLatitude());
            Assert.Equal(points[1].Longitude, point1.GetLongitude());
            Assert.Equal(points[1].Name, features[1].Properties[nameHeader]);
        }

        private class TestPoint
        {
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public string Name { get; set; }
        }
    }
}
