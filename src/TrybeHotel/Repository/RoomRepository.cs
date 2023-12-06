using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 7. Refatore o endpoint GET /room
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            try
            {
                var city = _context.Cities.First(c => c.CityId == HotelId);
                var hotel = _context.Hotels.First(h => h.CityId == HotelId);

                var roomQuery = from hotelQuery in _context.Hotels
                                join room in _context.Rooms
                                on hotelQuery.HotelId equals room.RoomId
                                select new RoomDto
                                {
                                    RoomId = room.RoomId,
                                    Name = room.Name,
                                    Capacity = room.Capacity,
                                    Image = room.Image,
                                    Hotel = new HotelDto
                                    {
                                        HotelId = hotel.HotelId,
                                        Name = hotel.Name,
                                        Address = hotel.Address,
                                        CityId = city.CityId,
                                        CityName = city.Name,
                                        State = city.State
                                    }
                                };


                return roomQuery;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Erro ao executar");
            }
            // throw new NotImplementedException(); 
        }

        // 8. Refatore o endpoint POST /room
        public RoomDto AddRoom(Room room)
        {
            try
            {
                var hotel = _context.Hotels.First(c => c.HotelId == room.HotelId);
                var city = _context.Cities.First(c => c.CityId == hotel.CityId);
                _context.Rooms.Add(room);
                _context.SaveChanges();
                return new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = hotel.HotelId,
                        Name = hotel.Name,
                        Address = hotel.Address,
                        CityId = city.CityId,
                        CityName = city.Name,
                        State = city.State
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Erro ao executar");
            }
            // throw new NotImplementedException();
        }

        public void DeleteRoom(int RoomId)
        {
            try
            {

                var roomRemove = _context.Rooms.Find(RoomId);

                if (roomRemove != null)
                {
                    _context.Rooms.Remove(roomRemove);
                    _context.SaveChanges();
                }
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