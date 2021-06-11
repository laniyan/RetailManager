﻿using System.Collections.Generic;
using System.Web.Http;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Internal.Models;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class ProductController : ApiController
    {
        public List<ProductModel> GetAllProducts()
        {
            ProductData data = new ProductData();

            return data.GetAllProducts();
        }
    }
}