﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.Internal.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Descripton { get; set; }
        public string RetailPrice { get; set; }
        public string QuantityInStock { get; set; }
    }
}
