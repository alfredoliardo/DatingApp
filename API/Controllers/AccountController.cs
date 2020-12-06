using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context,
         ITokenService tokenService,
         IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(Register model)
        {

            if (await UserExists(model.UserName) && !string.IsNullOrEmpty(model.UserName)) return BadRequest("UserName is already taken.");

            var user = _mapper.Map<AppUser>(model);
            using var hmac = new HMACSHA512();

                user.UserName = model.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                user.PasswordSalt = hmac.Key;


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new User
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login model)
        {
            AppUser user = await _context.Users.Include(u => u.Photos).SingleOrDefaultAsync(user => user.UserName == model.UserName.ToLower());
            if (user == null) return Unauthorized("Invalid user or password");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            for (int i = 0; i < computerHash.Length; i++)
            {
                if (computerHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid user or password");
            }

            return new User
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.Where(p => p.IsMain).SingleOrDefault()?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(user => user.UserName == username);
        }
    }
}