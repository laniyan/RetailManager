using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        public List<InventoryModel> Get()
        {
            var data = new InventoryData();

            return data.GetInventory();
        }


        public void PostInventoryRecord(InventoryModel item)
        {
            var data = new InventoryData();

            data.SaveInventoryRecord(item);
        }
    }
}
