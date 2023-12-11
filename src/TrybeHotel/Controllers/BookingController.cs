using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]

    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;
        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var email = token!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            BookingResponse newBooking = _repository.Add(bookingInsert, email!);
            return Created("", newBooking);
            // throw new NotImplementedException();
        }


        [HttpGet("{Bookingid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult GetBooking(int Bookingid)
        {
           try
            {
                var token = HttpContext.User.Identity as ClaimsIdentity;
                var email = token!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                BookingResponse booking = _repository.GetBooking(Bookingid, email!);
                return Ok(booking);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
            // throw new NotImplementedException();
        }
    }
}