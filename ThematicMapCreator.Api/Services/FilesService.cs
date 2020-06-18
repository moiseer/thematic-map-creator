using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Csv;
using Geo;
using Geo.Gps.Serialization;
using Geo.IO.GeoJson;
using OfficeOpenXml;
using SharpKml.Dom;
using SharpKml.Engine;
using Feature = Geo.IO.GeoJson.Feature;
using Point = Geo.Geometries.Point;

namespace ThematicMapCreator.Api.Services
{
    public class FilesService
    {
        private readonly string[] latColumnNames = { "lat", "latitude", "latitude_wgs84", "широта" };
        private readonly string[] lngColumnNames = { "lng", "lon", "long", "longitude", "longitude_wgs84", "долгота" };

        public FeatureCollection ConvertCsvToGeoJson(string csv)
        {
            var lines = CsvReader.ReadFromText(csv);
            if (!lines.Any())
            {
                throw new Exception("Файл пуст.");
            }

            (string latColumnName, string lngColumnName) = GetLatLngColumnNames(lines.First().Headers);

            // TODO: Сообщать об исключении строк.
            var features = lines
                .Select(line =>
                {
                    var hasLatitude = double.TryParse(line[latColumnName], NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var latitude);
                    var hasLongitude = double.TryParse(line[lngColumnName], NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var longitude);
                    if (!hasLatitude || !hasLongitude)
                    {
                        return null;
                    }

                    var position = new Coordinate(latitude, longitude);
                    var geometry = new Point(position);
                    var properties = line.Headers.ToDictionary(header => header.Trim(), header => line[header].Trim() as dynamic);
                    return new Feature(geometry, properties);
                })
                .Where(feature => feature != null);

            return new FeatureCollection(features);
        }

        public FeatureCollection ConvertGpxToGeoJson(Stream gpxStream)
        {
            using var streamWrapper = new StreamWrapper(gpxStream);
            var serializer = new Gpx11Serializer();
            if (!serializer.CanDeSerialize(streamWrapper))
            {
                throw new Exception("Невозможно десериализовать.");
            }

            var gpsData = serializer.DeSerialize(streamWrapper);
            var features = gpsData.Routes.Select(route => new Feature(route.ToLineString(), route.Metadata.ToDictionary(meta => meta.Key, meta => meta.Value as dynamic)))
                .Concat(gpsData.Tracks.Select(track => new Feature(track.ToLineString(), track.Metadata.ToDictionary(meta => meta.Key, meta => meta.Value as dynamic))));

            return new FeatureCollection(features);
        }

        public FeatureCollection ConvertXlsxToGeoJson(Stream xlsxStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(xlsxStream);
            var worksheet = package.Workbook.Worksheets[1];

            var dimension = worksheet.Dimension;
            if (dimension == null)
            {
                throw new Exception("Файл пуст.");
            }

            int rowCount = dimension.Rows;
            int colCount = dimension.Columns;

            var headers = new string[colCount];
            for (int colIndex = 1; colIndex <= colCount; colIndex++)
            {
                headers[colIndex - 1] = worksheet.Cells[1, colCount].Value.ToString();
            }

            (string latColumnName, string lngColumnName) = GetLatLngColumnNames(headers);
            var latColumnIndex = Array.IndexOf(headers, latColumnName) + 1;
            var lngColumnIndex = Array.IndexOf(headers, lngColumnName) + 1;

            var features = new List<Feature>(rowCount - 1);
            for (int rowIndex = 2; rowIndex <= rowCount; rowIndex++)
            {
                var hasLatitude = double.TryParse(worksheet.Cells[rowIndex, latColumnIndex].Value.ToString(), NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var latitude);
                var hasLongitude = double.TryParse(worksheet.Cells[rowIndex, lngColumnIndex].Value.ToString(), NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var longitude);
                if (!hasLatitude || !hasLongitude)
                {
                    continue;
                }

                var position = new Coordinate(latitude, longitude);
                var geometry = new Point(position);
                var properties = new Dictionary<string, dynamic>(colCount);
                for (int colIndex = 1; colIndex <= colCount; colIndex++)
                {
                    properties.Add(headers[colIndex - 1], worksheet.Cells[rowIndex, colCount].Value);
                }

                features.Add(new Feature(geometry, properties));
            }

            return new FeatureCollection(features);
        }

        public FeatureCollection ConvertKmlToGeoJson(Stream kmlStream)
        {
            var kmlFile = KmlFile.Load(kmlStream);
            var kml = kmlFile.Root as Kml;
            if (kml == null)
            {
                throw new Exception("Файл пуст.");
            }

            var flatten = kml.Flatten();
            var points = flatten.OfType<SharpKml.Dom.Point>().Select(point => new Feature(new Point(point.Coordinate.Latitude, point.Coordinate.Longitude)));

            return new FeatureCollection(points);
        }

        private (string LatColumnName, string LngColumnName) GetLatLngColumnNames(string[] headers)
        {
            var latColumnName = headers.FirstOrDefault(header => latColumnNames.Contains(header.Trim().ToLower()));
            if (latColumnName == default)
            {
                throw new Exception($"Не найдена колонка с широтой: {latColumnNames.Aggregate((result, name) => $"{result}, {name}")}");
            }

            var lngColumnName = headers.FirstOrDefault(header => lngColumnNames.Contains(header.Trim().ToLower()));
            if (lngColumnName == default)
            {
                throw new Exception($"Не найдена колонка с долготой: {lngColumnNames.Aggregate((result, name) => $"{result}, {name}")}");
            }

            return (latColumnName, lngColumnName);
        }
    }
}
