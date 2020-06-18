using System;
using System.IO;
using System.Threading.Tasks;
using Geo.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThematicMapCreator.Api.Services;

namespace ThematicMapCreator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private readonly FilesService service;

        public FilesController(FilesService service)
        {
            this.service = service;
        }

        [HttpPost("csv")]
        public async Task<ActionResult<IGeoJsonObject>> GetGeoJsonFromCsv(IFormFile file)
        {
            if (".csv" != Path.GetExtension(file.FileName).ToLower())
            {
                return BadRequest("Расширение файла должно быть \"CSV\".");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                using var streamReader = new StreamReader(stream);

                var csv = await streamReader.ReadToEndAsync();
                var geoJson = service.ConvertCsvToGeoJson(csv);

                return Json(geoJson);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("gpx")]
        public async Task<ActionResult<IGeoJsonObject>> GetGeoJsonFromGpx(IFormFile file)
        {
            if (".gpx" != Path.GetExtension(file.FileName).ToLower())
            {
                return BadRequest("Расширение файла должно быть \"GPX\".");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var geoJson = service.ConvertGpxToGeoJson(stream);
                return Json(geoJson);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("kml")]
        public async Task<ActionResult<IGeoJsonObject>> GetGeoJsonFromKml(IFormFile file)
        {
            if (".kml" != Path.GetExtension(file.FileName).ToLower())
            {
                return BadRequest("Расширение файла должно быть \"KML\".");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var geoJson = service.ConvertKmlToGeoJson(stream);
                return Json(geoJson);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("xlsx")]
        public async Task<ActionResult<IGeoJsonObject>> GetGeoJsonFromXlsx(IFormFile file)
        {
            if (".xlsx" != Path.GetExtension(file.FileName).ToLower())
            {
                return BadRequest("Расширение файла должно быть \"XLSX\".");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var geoJson = service.ConvertXlsxToGeoJson(stream);
                return Json(geoJson);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
