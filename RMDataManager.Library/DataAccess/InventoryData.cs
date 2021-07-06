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
    public class InventoryData : IInventoryData //created an interface so its easy to test
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public InventoryData( ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public List<InventoryModel> GetInventory()
        {

            var output = _sqlDataAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll",new { }, "RMData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            _sqlDataAccess.SaveData("dbo.spInventory_Insert", item, "RMData");//we dont have to specify the genric type (<InventoryModel>) coz we passed in item which is InventoryModel
        }
    }



}
