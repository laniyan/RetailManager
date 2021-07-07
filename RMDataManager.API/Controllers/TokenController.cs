using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using RMDataManager.API.Data;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RMDataManager.API.Controllers
{

    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)
        {
            if (await IsValidUsernameAndPassword(username, password))
            {
                return new ObjectResult(await GenerateToken(username));//this will generate a token as we imp & it will return that obj back to the caller as an action result
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<bool> IsValidUsernameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            //return all the roles for the user
            var roles = from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id //we only want to get that users role
                select new {ur.UserId, ur.RoleId, r.Name};//this gives us a list off all the roles for a given userId

            //create a claim becoz JWT uses a claim system 
            var claims = new List<Claim>()
            {
                /*certain claim types are default/standard 1 of them is the users name this all comes from a standard schema that comes from identity which comes
                 from entity framework what happens is its going to take all these claims and create a token out of it which is then signed which wont be encrypted
                 so we can see wot the username is user id, wot roles your apart of as well as wen does this token expire this is visiable to the end user if they
                 knw how to find it but we dont pass our secret which we use to sign the JWT to the client what the secret does it allows us to create a signature
                 for ur tokens that will only be valid if nothing in the token has changed that is the security part if the user or anyone was to change something 
                 that will make the signature invalid then the changed claim wont work when you pass it back end users have the ability to look and edit the JWT
                 so that means they are safe but not encrypted safe because of the signature (Tim Corey vid 35 JWT 32:50)*/

                //init the obj and store our values in the claims var Claim is a key value pair
                new Claim(ClaimTypes.Name,
                    username), //we are saying one of the claims we our going to pass back is our username
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf,
                    new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), /*Nbf(not be for) is the token is 
                not valid before a certain time&date we pass the time&date as 2 arg, in our case we want the JWT valid st8 away so we do this by saying we are takin
                our dateTime.Now and convert to unix time seconds that returns the num of seconds that have passed since 1970 so nbf that date which means rite now */
                new Claim(JwtRegisteredClaimNames.Exp,
                    new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds()
                        .ToString()) //exp means expires we have set it to be vaild only for a day
            };

            //Add more claims have to add the users roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            //Create JWT
            var token = new JwtSecurityToken(//create a new JWT
                new JwtHeader(//instal a header
                    new SigningCredentials(//and create signing credentials and we use the algo for signing not encrypting it (SecurityAlgorithms.HmacSha256)
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Secrets:SecurityKey"))),//this is the key we use to
                                                                                                                                 //sign it it use to be "MySecretKeyIsSecretSoDoNotTell" now weve put it in appsetting
                        /*takes the string and codes it into utf and gets the bytes of that and
                          thats the key it uses to sign this token this key is very important its a secret key
                          if the key was to get out then somebody else could using H Mac sha-256 encode a new 
                          claim so we make it a long random string and eventrually store it in azure key vaults
                          for testing purposes we would leave it here or use appsettings.json (Tim Corey vid 34 43:00)*/
                        SecurityAlgorithms.HmacSha256)//H mac sha 256 security algo 
                        ),
                new JwtPayload(claims));//this is are whole claim the payload like what we send in the body of a request

            var output = new //this is a new dynamic obj
            {
                Access_Token =
                    new JwtSecurityTokenHandler().WriteToken(token), //writeToken() creates the string from our token  
                UserName = username //we pass username as well bcoz thats what our original contract passes back from our auth system
            };

            return output;
        }
    }
}