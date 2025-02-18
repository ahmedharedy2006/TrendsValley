using System.ComponentModel.DataAnnotations;


namespace BooksMine.Models
{
    public class City
    {
        [Key]
        public int City_Id { get; set; }

        [StringLength(150)]
        [Required]
        public string City_Name { get; set; }

    }
}
