using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrasturcture.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve all users
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Retrieve a user by ID
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Retrieve a user by email
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        // Add a new user
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await SaveChangesAsync(); // Ensure changes are saved to the database
        }

        // Update an existing user
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await SaveChangesAsync(); // Ensure changes are saved to the database
        }

        // Delete a user
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await SaveChangesAsync(); // Ensure changes are saved to the database
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

