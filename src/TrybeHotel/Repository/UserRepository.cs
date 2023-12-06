using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            try
            {
                var userLogin = _context.Users.First((user) => user.Email == login.Email);
                if (userLogin.Password != login.Password) throw new Exception();

                return new UserDto()
                {
                    UserId = userLogin.UserId,
                    Name = userLogin.Name,
                    Email = userLogin.Email,
                    UserType = userLogin.UserType
                };
            }
            catch (Exception)
            {
                throw new Exception("Incorrect e-mail or password");
            }
            // throw new NotImplementedException();
        }
        public UserDto Add(UserDtoInsert user)
        {
            foreach (var item in _context.Users)
            {
                if (item.Email == user.Email) throw new Exception("User email already exists");
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return GetUserByEmail(user.Email!);
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var email = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (email == null)
            {
                return null!;
            }

            return new UserDto
            {
                UserId = email.UserId!,
                Name = email.Name,
                Email = email.Email,
                UserType = email.UserType
            };
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var users = _context.Users.Select(user => new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                UserType = user.UserType
            });

            return users;
            // throw new NotImplementedException();
        }

    }
}