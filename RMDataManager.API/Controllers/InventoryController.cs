using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }
        [Authorize(Roles = "Admin, Manager")]
        public List<InventoryModel> Get()
        {
            var data = new InventoryData(_config);

            return data.GetInventory();
        }

        [Authorize(Roles = "Admin")]
        public void PostInventoryRecord(InventoryModel item)
        {
            var data = new InventoryData(_config);

            data.SaveInventoryRecord(item);
        }
    }
}