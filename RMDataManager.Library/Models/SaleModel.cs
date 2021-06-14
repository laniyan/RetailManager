using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMDataManager.Library.Models
{
    public class SaleModel
    {
        public List<SaleDetailModel> SaleDetails { get; set; } //we dont init this list here like we do in the desktop lib coz we want to knw if ur posting in a null saleModel
    }
}