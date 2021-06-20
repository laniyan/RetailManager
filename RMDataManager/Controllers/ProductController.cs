﻿using System.Collections.Generic;
using System.Web.Http;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Controllers
{
    [Authorize(Roles = "Cashier")]
    public class ProductController : ApiController
    {
        public List<ProductModel> GetAllProducts()
        {
            ProductData data = new ProductData();

            return data.GetAllProducts();
        }
    }
}
