namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using TrybeHotel.Services;
using TrybeHotel.Dto;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;

public class LoginJson
{
    public string? token { get; set; }
}


public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City { CityId = 1, Name = "Manaus", State = "AM" });
                    appContext.Cities.Add(new City { CityId = 2, Name = "Palmas", State = "TO" });
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1 });
                    appContext.Hotels.Add(new Hotel { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2 });
                    appContext.Hotels.Add(new Hotel { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1 });
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1 });
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4 });
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Testando a rota GET/ City")]
    [InlineData("/city")]
    public async Task TestGetCity(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
        response!.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
    }

    [Theory(DisplayName = "Testando a rota GET/ Hotel")]
    [InlineData("/hotel")]
    public async Task TestGetHotel(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
        response!.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
    }

    [Theory(DisplayName = "Testando a rota GET/ Room")]
    [InlineData("/room/1")]
    public async Task TestGetRoom(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
        response!.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
    }

    [Theory(DisplayName = "Testando a rota POST/ City")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        var city = new
        {
            Name = "São Paulo"
        };

        var jsonCovert = JsonConvert.SerializeObject(city);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Hotel falha ao um usuario não admin tenta criar um Hotel")]
    [InlineData("/hotel")]
    public async Task TestPostHotelWithoutAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var hotel = new
        {
            Name = "Trybe Hotel teste",
            Address = "Addres 5",
            CityId = 1
        };

        var jsonCovert = JsonConvert.SerializeObject(hotel);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Hotel")]
    [InlineData("/hotel")]
    public async Task TestPostHotelWithAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var hotel = new
        {
            Name = "Trybe Hotel teste",
            Address = "Addres 5",
            CityId = 1
        };

        var jsonCovert = JsonConvert.SerializeObject(hotel);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Room falha ao um usuario não admin tenta criar uma Room")]
    [InlineData("/room")]
    public async Task TestPostRoomWithoutAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var hotel = new
        {
            Name = "Trybe Hotel teste",
            Address = "Addres 5",
            CityId = 1
        };

        var jsonCovert = JsonConvert.SerializeObject(hotel);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Room")]
    [InlineData("/room")]
    public async Task TestPostRoomWithAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var room = new
        {
            Name = "Room 10",
            Capacity = 4,
            Image = "Image 10",
            HotelId = 3
        };

        var jsonCovert = JsonConvert.SerializeObject(room);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota DELETE/ Room")]
    [InlineData("/room/2")]
    public async Task TestDeleteRoomWithAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota DELETE/ Room falha ao um usuario não admin tenta deletar uma Room")]
    [InlineData("/room/2")]
    public async Task TestDeleteRoomWithoutAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ User")]
    [InlineData("/user")]
    public async Task TestPostUser(string url)
    {
        var user = new UserDtoInsert
        {
            UserId = 4,
            Name = "Teste",
            Email = "Teste@trybehotel.com",
            Password = "Senha4"
        };

        var jsonCovert = JsonConvert.SerializeObject(user);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ User falha ao passar um email já cadastrado")]
    [InlineData("/user")]
    public async Task TestPostUserEmailAlreadyExist(string url)
    {
        var user = new UserDtoInsert
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            Password = "Senha2"
        };

        var jsonCovert = JsonConvert.SerializeObject(user);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Room falha ao um usuario tenta entra com uma conta inexistente")]
    [InlineData("/login")]
    public async Task TestPostLoginWithoutExistingAccount(string url)
    {
        var user = new LoginDto
        {
            Email = "Teste@trybehotel.com",
            Password = "Senha4"
        };

        var jsonCovert = JsonConvert.SerializeObject(user);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Login")]
    [InlineData("/login")]
    public async Task TestPostLogin(string url)
    {
        var user = new LoginDto
        {
            Email = "beatriz@trybehotel.com",
            Password = "Senha2"
        };


        var jsonCovert = JsonConvert.SerializeObject(user);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);

        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota POST/ Booking")]
    [InlineData("/booking")]
    public async Task TestPostBooking(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var booking = new BookingDtoInsert
        {
            CheckIn = new DateTime(2023, 07, 02),
            CheckOut = new DateTime(2023, 07, 03),
            GuestQuant = 1,
            RoomId = 1
        };

        var jsonCovert = JsonConvert.SerializeObject(booking);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PostAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota GET/ Booking")]
    [InlineData("/booking/1")]
    public async Task TestGetBooking(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
        response!.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
    }

    [Theory(DisplayName = "Testando a rota GET/ Booking falha ao usuario que não é cliente")]
    [InlineData("/booking/1")]
    public async Task TestPostBookingWithoutClientEmail(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "Teste@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota GET/ User falha ao um usuario não admin, busca os usuarios")]
    [InlineData("/user")]
    public async Task TestPostUserithoutAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota GET/ User")]
    [InlineData("/user")]
    public async Task TestPostUserWithAdmin(string url)
    {
        var user = new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        };

        var token = new TokenGenerator().Generate(user);
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota PUT/ City")]
    [InlineData("/city")]
    public async Task TestPutCity(string url)
    {
        var city = new
        {
            Name = "São Paulo"
        };

        var jsonCovert = JsonConvert.SerializeObject(city);
        var convert = new StringContent(jsonCovert, Encoding.UTF8, "application/json");

        var response = await _clientTest.PutAsync(url, convert);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Theory(DisplayName = "Testando a rota GET/ Geo Status")]
    [InlineData("/geo/status")]
    public async Task TestGetGeoStatus(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    // [Theory(DisplayName = "Testando a rota GET/ Geo Address")]
    // [InlineData("/geo/address")]
    // public async Task TestGetGeoAddress(string url)
    // {

    //     var response = await _clientTest.GetAsync(url);
    //     Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    // }



}