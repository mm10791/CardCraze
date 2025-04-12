//Authored by Adrian

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardCrazeAPI.Models
{
    public class CartItem
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CardID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [ForeignKey("CardID")]
        public Card Card { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}
