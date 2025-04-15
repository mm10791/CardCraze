//author Tristan
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CardCrazeAPI.Models
{
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }

        public int UserID { get; set; }
        public int CardID { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("CardID")]
        public Card Card { get; set; }
    }
}
