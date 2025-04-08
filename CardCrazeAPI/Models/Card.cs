//Author of class: Max

namespace CardCrazeAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Card
    {
        [Key]
        public int CardID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Rarity { get; set; } //we can do rarity like "common", "rare", "legendary", etc...

        [Required]
        public decimal Price { get; set; }

        public string ImageURL { get; set; } //card image
    }

}
