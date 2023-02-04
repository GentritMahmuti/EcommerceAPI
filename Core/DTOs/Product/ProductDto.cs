﻿using Nest;
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Product
{
    public class ProductDto
    {

        public int Id { get; set; }

        public string Name { get; set; }


        [Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public double ListPrice { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public int Stock { get; set; }

        public int TotalSold { get; set; }


    }
}
