using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(u => u.Photos)
            .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
            .Include(u => u.Photos)
            .SingleAsync(u => u.Id == id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
            .Include(u => u.Photos)
            .SingleAsync(u => u.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                return true;
            }
            return false;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<Member> GetMemberAsync(string username)
        {
            return await _context.Users.Where(u => u.UserName == username)
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PagedList<Member>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
            .AsNoTracking()
            .ProjectTo<Member>(_mapper.ConfigurationProvider);

            return await PagedList<Member>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }
    }
}