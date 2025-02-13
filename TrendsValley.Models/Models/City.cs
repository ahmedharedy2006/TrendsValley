using System.ComponentModel.DataAnnotations;

namespace BooksMine.Models
{
    public class City
    {
        public int Id { get; set; }

        [StringLength(150)]
        [Required]
        public string name { get; set; }

    }
}
