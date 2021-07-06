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
using Microsoft.Extensions.Logging;
using RMDataManager.API.Data;
using RMDataManager.API.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.API.Controllers
{
    [Route("api/[controller]")]//this means all methods routes in this controller will start with api/User (controller is user)
    [ApiController]
    [Authorize]//you have to be auth to use these controls
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;


        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IUserData userData, ILogger<UserController> logger )
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);/*this returns the current users Id in the method we dont want to ask the user for the id we want
                                                                          to get it  so we know who it is and what they call look for if we allow them to tell us then 
                                                                          they can look up wot they want*/
            return _userData.GetUserById(userId).FirstOrDefault();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
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
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            //convert the roles into a dict and set the key to the role id and the value to the role name
                var roles = _context.Roles.ToDictionary(r => r.Id, r => r.Name);

                return roles;
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pairing)//coz its a post u only send in one obj i.e a body
        {

            //add logger we want to log additional info for when a role is changed

            //find user info base on who is logged in 
            string loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUserId = _userData.GetUserById(loggedInUser).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            //we put the log here so incase it crashes we want it to tell us someone tried to switch users role
            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}"
                , loggedInUserId, user.Id, pairing.RoleName);/*we dont use string temp($) we use the formatt that ive used bcoz this way for structured loggers this will pull 
                                        * these items out and store them separately so we can do queries upon them its much more ideal if we do string temp it will
                                        * return one big long string that we cant do any queries on but with structured logging we can pull out who the user was and say give
                                        * me all the actions that Lanre did for adding and removing roles and it will list all the times i add or removed a role from someone
                                        * and who they was with this way now we have the ability to pull out the var separtely in a structured logger like seriaLog (Tim Corey vid 38 56:00)*/
            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            string loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUserId = _userData.GetUserById(loggedInUser).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            //we put the log here so incase it crashes we want it to tell us someone tried to switch users role more info in AddARole(UserRolePairModel pairing)
            _logger.LogInformation("Admin {Admin} remove user {User} to role {Role}"
                , loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);

            
        }
    }
}