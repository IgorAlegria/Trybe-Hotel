using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            try
            {
                var cityQuery = from city in _context.Cities
                                select new CityDto
                                {
                                    CityId = city.CityId,
                                    Name = city.Name,
                                    State = city.State
                                };


                return cityQuery.ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Erro ao executar");
            }
            // throw new NotImplementedException();
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                return new CityDto
                {
                    CityId = _context.Cities.Count(),
                    Name = city.Name,
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

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
            try
            {
                _context.Cities.Update(city);
                _context.SaveChanges();

                return new CityDto
                {
                    CityId = city.CityId,
                    Name = city.Name,
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