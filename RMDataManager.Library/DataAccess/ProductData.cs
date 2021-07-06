using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ProductData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public List<ProductModel> GetAllProducts()
        {
            

            var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "RMData");// coz our stored proc has no param we open an empty dynamic obj

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            

            var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "RMData").FirstOrDefault();

            return output;
        }

    }
}
