using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ISaleData _saleData;

        public SaleController(IConfiguration config, ISaleData saleData)
        {
            _config = config;
            _saleData = saleData;
        }


        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //old way how its done in .NET Framework - RequestContext.Principal.Identity.GetUserId();

            _saleData.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin, Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSales()
        {
            var output = _saleData.GetSaleReport();

            return output;
        }
    }
}