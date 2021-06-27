using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMDataManager.API.Data;
using RMDataManager.API.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]//you have to be auth to use these controls
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);/*this returns the current users Id in the method we dont want to ask the user for the id we want
                                                                          to get it  so we know who it is and what they call look for if we allow them to tell us then 
                                                                          they can look up wot they want*/
            UserData data = new UserData(_config);

            return data.GetUserById(userId).FirstOrDefault();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            //this all comes from Entity Framework goes into the user table and gets all the users 

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id//ths links the role Id to the id in the role table
                select new {ur.UserId, ur.RoleId, r.Name};/*we select a new annyomus obj we have the user id which is linked to
                                                            a particular role we have dat same role id and the role name now with this info we can set the role
                                                            to be only the one that matches our Id like we do just underneath u.Role = userRoles*/

                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                    };

                    u.Roles = userRoles.Where(x => x.UserId == u.Id)
                        .ToDictionary(key => key.RoleId, val => val.Name);//this replaces the 4each
                    //foreach (var r in user.Roles)
                    //{
                    //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).FirstOrDefault().Name);
                    //}

                    output.Add(u);
                }

                return output;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            //convert the roles into a dict and set the key to the role id and the value to the role name
                var roles = _context.Roles.ToDictionary(r => r.Id, r => r.Name);

                return roles;
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pairing)//coz its a post u only send in one obj i.e a body
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);

            
        }
    }
}