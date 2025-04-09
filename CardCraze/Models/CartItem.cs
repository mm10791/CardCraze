using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardCraze.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        public int UserID { get; set; }
        public int CardID { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("CardID")]
        public Card card {get; set;}
    }
}
