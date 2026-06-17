using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(CreateUserRequest request);
        Task<User?> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        // In-memory storage (replace with database in production)
        private static List<User> _users = new();
        private static int _nextId = 1;
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
            
            // Initialize with sample data
            if (_users.Count == 0)
            {
                _logger.LogInformation("Initializing UserService with sample data");
                
                _users.Add(new User
                {
                    Id = _nextId++,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "123-456-7890",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                });

                _users.Add(new User
                {
                    Id = _nextId++,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "098-765-4321",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                });

                _logger.LogInformation("Sample data initialized. Total users: {UserCount}", _users.Count);
            }
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogDebug("Retrieving all active users");
            var users = _users.Where(u => u.IsActive).ToList();
            _logger.LogInformation("Successfully retrieved {UserCount} active users", users.Count);
            return Task.FromResult<IEnumerable<User>>(users);
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            _logger.LogDebug("Retrieving user by ID: {UserId}", id);
            var user = _users.FirstOrDefault(u => u.Id == id && u.IsActive);
            
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                return Task.FromResult<User?>(null);
            }

            _logger.LogInformation("Successfully retrieved user with ID: {UserId}. Email: {Email}", id, user.Email);
            return Task.FromResult<User?>(user);
        }

        public Task<User> CreateUserAsync(CreateUserRequest request)
        {
            _logger.LogDebug("Creating new user. Email: {Email}", request.Email);
            // Prevent duplicate emails
            if (_users.Any(u => string.Equals(u.Email, request.Email, StringComparison.OrdinalIgnoreCase) && u.IsActive))
            {
                _logger.LogWarning("Create failed: Duplicate email detected: {Email}", request.Email);
                throw new Exceptions.DuplicateEmailException(request.Email);
            }
            
            var user = new User
            {
                Id = _nextId++,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _users.Add(user);
            _logger.LogInformation("User created successfully. UserId: {UserId}, Email: {Email}, Name: {FullName}", 
                user.Id, user.Email, $"{user.FirstName} {user.LastName}");
            
            return Task.FromResult(user);
        }

        public Task<User?> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            _logger.LogDebug("Updating user. UserId: {UserId}", id);
            
            var user = _users.FirstOrDefault(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                _logger.LogWarning("Update failed: User not found with ID: {UserId}", id);
                return Task.FromResult<User?>(null);
            }

            // If email changed, ensure it's not a duplicate for another active user
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                && _users.Any(u => u.Id != id && string.Equals(u.Email, request.Email, StringComparison.OrdinalIgnoreCase) && u.IsActive))
            {
                _logger.LogWarning("Update failed: Duplicate email detected when updating user {UserId} to {Email}", id, request.Email);
                throw new Exceptions.DuplicateEmailException(request.Email);
            }

            var originalEmail = user.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.IsActive = request.IsActive;
            user.UpdatedDate = DateTime.UtcNow;

            _logger.LogInformation("User updated successfully. UserId: {UserId}, Name: {FullName}, Email changed from {OldEmail} to {NewEmail}, IsActive: {IsActive}", 
                id, $"{user.FirstName} {user.LastName}", originalEmail, user.Email, user.IsActive);
            
            return Task.FromResult<User?>(user);
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogDebug("Deleting user. UserId: {UserId}", id);
            
            var user = _users.FirstOrDefault(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                _logger.LogWarning("Delete failed: User not found with ID: {UserId}", id);
                return Task.FromResult(false);
            }

            var userEmail = user.Email;
            user.IsActive = false; // Soft delete
            
            _logger.LogInformation("User soft-deleted successfully. UserId: {UserId}, Email: {Email}, Name: {FullName}", 
                id, userEmail, $"{user.FirstName} {user.LastName}");
            
            return Task.FromResult(true);
        }
    }
}
