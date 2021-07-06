using System.Collections.Generic;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        List<ProductModel> GetAllProducts();
        ProductModel GetProductById(int productId);
    }
}