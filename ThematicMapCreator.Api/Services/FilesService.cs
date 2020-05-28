using System;
using System.Globalization;
using System.Linq;
using BAMCIS.GeoJSON;
using Csv;

namespace ThematicMapCreator.Api.Services
{
    public class FilesService
    {
        public FeatureCollection ConvertCsvToGeoJson(string csv)
        {
            // TODO: Получать нужные названия колонок.
            var latColumnNames = new[] { "lat", "latitude", "latitude_wgs84", "широта" };
            var lngColumnNames = new[] { "lng", "lon", "long", "longitude", "longitude_wgs84", "долгота" };

            var lines = CsvReader.ReadFromText(csv);
            if (!lines.Any())
            {
                throw new Exception("Файл пуст.");
            }

            var headers = lines.First().Headers;
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

                    var position = new Position(longitude, latitude);
                    var geometry = new Point(position);
                    var properties = line.Headers.ToDictionary(header => header.Trim(), header => line[header].Trim() as dynamic);
                    return new Feature(geometry, properties);
                })
                .Where(feature => feature != null);

            return new FeatureCollection(features);
        }
    }
}
