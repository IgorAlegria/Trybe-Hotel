using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        //  5. Refatore o endpoint GET /hotel
        public IEnumerable<HotelDto> GetHotels()
        {
            try
            {
                var hotelQuery = from hotel in _context.Hotels
                                 join city in _context.Cities
                                 on hotel.CityId equals city.CityId
                                 select new HotelDto
                                 {
                                     HotelId = hotel.HotelId,
                                     Name = hotel.Name,
                                     Address = hotel.Address,
                                     CityId = city.CityId,
                                     CityName = city.Name,
                                     State = city.State
                                 };


                return hotelQuery.ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Erro ao executar");
            }
            // throw new NotImplementedException();
        }

        // 6. Refatore o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            try
            {
                var city = _context.Cities.First(c => c.CityId == hotel.CityId);
                _context.Hotels.Add(hotel);
                _context.SaveChanges();
                return new HotelDto
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityId = city.CityId,
                    CityName = city.Name,
                    State = city.State
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Erro ao executar");
            }
            // throw new NotImplementedException();
        }
    }
}