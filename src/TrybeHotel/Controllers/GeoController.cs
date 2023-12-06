using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;


namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("geo")]
    public class GeoController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly IGeoService _geoService;

        private readonly IHttpClientFactory _httpClient;


        public GeoController(IHotelRepository repository, IGeoService geoService, IHttpClientFactory httpClient)
        {
            _repository = repository;
            _geoService = geoService;
            _httpClient = httpClient;
        }


        // 11. Desenvolva o endpoint GET /geo/status
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            var response = _geoService.GetGeoStatus();

            return Ok(response);
            // throw new NotImplementedException();
        }

        // 12. Desenvolva o endpoint GET /geo/address
        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> GetHotelsByLocation([FromBody] GeoDto address)
        {
            throw new NotImplementedException();
        }
    }


}