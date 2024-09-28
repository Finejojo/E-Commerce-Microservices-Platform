using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = new PasswordHasher<User>();
            _tokenService = tokenService;
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            // Verify the entered password against the hashed password
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, enteredPassword);
            return result == PasswordVerificationResult.Success;
        }

        // Fetch all users
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                id = u.Id,
                Username = u.Username,
                Email = u.Email
            }).ToList();
        }

        // Fetch a single user by ID
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        // Create a new user
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email
            };

            await _userRepository.CreateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            userDto.id = user.Id; // Map the generated ID back to the DTO
            return userDto;
        }

        // Update an existing user
        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.id);
            if (user == null)
                return null;

            // Update the user entity
            user.Username = userDto.Username;
            user.Email = userDto.Email;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return userDto;
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        // Register a new user
        public async Task RegisterAsync(RegisterUserDto model)
        {
            // Validate model
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Check if the user already exists
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Create a new user entity and hash the password
            var newUser = new User
            {
                Email = model.Email,
                Password = _passwordHasher.HashPassword(null, model.Password), // Hash the password
            };

            // Save the new user to the database
            await _userRepository.CreateUserAsync(newUser);

        }

        // Login user and generate JWT token
        public async Task<string> LoginAsync(LoginUserDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Retrieve user by email
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                // User not found, return null (authentication failed)
                return null;
            }

            // Validate password
            bool isPasswordValid = VerifyPassword(model.Password, user.Password);
            if (!isPasswordValid)
            {
                // Password is incorrect, return null (authentication failed)
                return null;
            }

            // Generate JWT token
            var roles = new List<string> { user.Role }; // Assuming the user has a single role
            var token = await _tokenService.CreateAccessTokenAsync(user.Email, roles, DateTime.UtcNow.AddHours(1));
            return token;
        }
    }
}



