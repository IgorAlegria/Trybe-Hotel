namespace TrybeHotel.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // 1. Adicione o atributo State na model City
    public class City
    {
        public int CityId { get; set; }

        public string? Name { get; set; }

        public string? State {get; set;}

        public List<Hotel>? Hotels { get; set; }
    }
}