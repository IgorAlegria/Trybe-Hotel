using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 9. Refatore o endpoint POST /booking
        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            var hotel = _context.Hotels.FirstOrDefault(h => h.HotelId == room!.HotelId);
            _context.Cities.FirstOrDefault(c => c.CityId == hotel!.CityId);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (room == null || booking.GuestQuant > room.Capacity)
            {
                return null!;
            }
            var newBooking = new Booking
            {
                CheckIn = (DateTime)booking.CheckIn!,
                CheckOut = (DateTime)booking.CheckOut!,
                GuestQuant = booking.GuestQuant,
                UserId = user!.UserId!,
                RoomId = room.RoomId
            };
            _context.Bookings.Add(newBooking);
            _context.SaveChanges();
            return new BookingResponse
            {
                bookingId = newBooking.BookingId,
                checkIn = newBooking.CheckIn,
                checkOut = newBooking.CheckOut,
                guestQuant = newBooking.GuestQuant,
                room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.Hotel!.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City!.Name,
                        State = room.Hotel.City.State
                    }
                }
            };
            // throw new NotImplementedException();
        }

        // 10. Refatore o endpoint GET /booking
        public BookingResponse GetBooking(int bookingId, string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var bookingResponse = _context.Bookings
                .Where(b => b.UserId == user!.UserId && b.BookingId == bookingId)
                .Select(b => new BookingResponse
                {
                    bookingId = b.BookingId,
                    checkIn = b.CheckIn,
                    checkOut = b.CheckOut,
                    guestQuant = b.GuestQuant,
                    room = new RoomDto
                    {
                        RoomId = b.Room!.RoomId,
                        Name = b.Room.Name,
                        Capacity = b.Room.Capacity,
                        Image = b.Room.Image,
                        Hotel = new HotelDto
                        {
                            HotelId = b.Room.Hotel!.HotelId,
                            Name = b.Room.Hotel.Name,
                            Address = b.Room.Hotel.Address,
                            CityId = b.Room.Hotel.CityId,
                            CityName = b.Room.Hotel.City!.Name,
                            State = b.Room.Hotel.City.State
                        }
                    }
                }).FirstOrDefault();

            return bookingResponse!;
            // throw new NotImplementedException();
        }

        public Room GetRoomById(int RoomId)
        {
            return _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId)!;
            // throw new NotImplementedException();
        }

    }

}