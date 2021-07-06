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
    public class SaleData : ISaleData
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IProductData _productData;

        public SaleData(ISqlDataAccess sqlDataAccess, IProductData productData)
        {
            _sqlDataAccess = sqlDataAccess;
            _productData = productData;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO: make this soild  bcoz with added business logic in here where we should only have data access action here its important to have working code asap

            //we use all these 8 steps bcoz we dont trust the frontend the frontend could of gave us all this info so now we are verifying i.e checking at the front and the back
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();//get all the sales details listed in the saleDetailDBModel obj
            var taxRate = ConfigHelper.GetTaxRate()/100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    //1 start filling in the models we will save to the db
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //2 Get the info about this product
                var productInfo = _productData.GetProductById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"$The product id of {detail.ProductId} could not be found in the database.");
                }


                //3 fill in the available info
                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }
            

            //4 Create the sale detail model
            SaleDBModel sale = new SaleDBModel
            {
                
                SubTotal = details.Sum(s => s.PurchasePrice),
                Tax = details.Sum(t => t.Tax),

                CashierId = cashierId

            };

            sale.Total = sale.SubTotal + sale.Tax;

            //5 Save the sale model

            //open the db connection we want this a low as possible in the method coz we want the db open for as lil time as possible
            {
                try
                {
                    _sqlDataAccess.StartTransaction("RMData");

                    _sqlDataAccess.SaveDataInTransaction("dbo.spSale_Insert", sale, "RMData");

                    //6 Get the id from the sale mode
                    sale.Id = _sqlDataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "RMData").FirstOrDefault();

                    //7 Finish filling in the sale detail models
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;

                        //8 Save the sale detail model
                        _sqlDataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item, "RMData");
                    }

                    _sqlDataAccess.CommitTransaction();
                }
                catch
                {
                    _sqlDataAccess.RollbackTransaction();
                    throw;//this just throws the original exception
                }
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sqlDataAccess.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RMData");

            return output;
        }
    }
}
