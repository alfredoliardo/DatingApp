using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(Register model){

            if(await UserExists(model.UserName) && !string.IsNullOrEmpty(model.UserName)) return BadRequest("UserName is already taken.");

            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName = model.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(Login model){
            AppUser user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == model.UserName.ToLower());
            if(user == null) return Unauthorized("Invalid user or password");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            for(int i = 0; i < computerHash.Length; i++){
                if(computerHash[i] != user.PasswordHash[i])return Unauthorized("Invalid user or password");
            }

            return user;
        }

        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(user => user.UserName == username);
        }
    }
}