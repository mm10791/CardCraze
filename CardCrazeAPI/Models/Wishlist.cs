﻿//Author of class: Max/Adrian

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardCrazeAPI.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CardID { get; set; }

        [ForeignKey("CardID")]
        public Card Card { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}