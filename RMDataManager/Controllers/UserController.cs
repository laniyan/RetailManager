using RMDataManager.Library.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using RMDataManager.Library.Internal.Models;

namespace RMDataManager.Controllers
{
    [Authorize]//you have to be auth to use these controls 
    public class UserController : ApiController
    {
     
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();/*this returns the current users Id in the method we dont want to ask the user for the id we want
                                                                          to get it  so we know who it is and what they call look for if we allow them to tell us then 
                                                                          they can look up wot they want*/
            UserData data = new UserData();

            return data.GetUserById(userId).FirstOrDefault();
        }

    }
}
