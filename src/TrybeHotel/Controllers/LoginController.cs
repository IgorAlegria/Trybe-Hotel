using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {

        private readonly IUserRepository _repository;
        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                UserDto newUserLogin = _repository.Login(login);
                TokenGenerator newToken = new();

                return Ok(new { token = newToken.Generate(newUserLogin) });
            }
            catch (Exception e)
            {
                return Unauthorized(new { message = e.Message });
            }
            //    throw new NotImplementedException();   
        }
    }
}