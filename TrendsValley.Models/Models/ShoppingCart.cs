﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.ViewModels;

namespace TrendsValley.Models.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
       
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [NotMapped]
        public ProductDetailsViewModel ProductDetailsViewModel { get; set; }

        [Required]
        [Range(1,1000 , ErrorMessage = "Please Enter A Value Between 1 And 1000")]
        public int Count { get; set; }

        [ForeignKey("customer")]

        public string UserId { get; set; }

        public Customer customer { get; set; }
    }
}
