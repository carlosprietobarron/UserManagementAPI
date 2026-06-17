using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using UserManagementAPI.Validators;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserValidator _validator;

        public UsersController(IUserService userService, ILogger<UsersController> logger, IUserValidator validator)
        {
            _userService = userService;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <returns>List of all active users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("GET request received: Retrieving all users");
            var users = await _userService.GetAllUsersAsync();
            _logger.LogInformation("GET /api/users completed successfully. Returned {UserCount} users", users.Count());
            return Ok(users);
        }

        /// <summary>
        /// Retrieve a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            _logger.LogInformation("GET request received: Retrieving user with ID {UserId}", id);
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("GET /api/users/{UserId} - User not found", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            _logger.LogInformation("GET /api/users/{UserId} completed successfully. User: {Email}", id, user.Email);
            return Ok(user);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="request">User creation request</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
        {
            _logger.LogInformation("POST request received: Creating new user with email {Email}", request.Email);

            // Validate request using validator (throws ValidationException on failure)
            _validator.ValidateCreateUserRequest(request);

            var user = await _userService.CreateUserAsync(request);
            _logger.LogInformation("POST /api/users completed successfully. Created user with ID {UserId}, Email: {Email}", user.Id, user.Email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Update an existing user's details
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="request">User update request</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> UpdateUser(int id, UpdateUserRequest request)
        {
            _logger.LogInformation("PUT request received: Updating user with ID {UserId}, Email: {Email}", id, request.Email);

            // Validate request using validator (throws ValidationException on failure)
            _validator.ValidateUpdateUserRequest(request);

            var user = await _userService.UpdateUserAsync(id, request);
            if (user == null)
            {
                _logger.LogWarning("PUT /api/users/{UserId} - User not found", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            _logger.LogInformation("PUT /api/users/{UserId} completed successfully. Updated user: {Email}", id, user.Email);
            return Ok(user);
        }

        /// <summary>
        /// Remove a user by ID (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("DELETE request received: Deleting user with ID {UserId}", id);
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("DELETE /api/users/{UserId} - User not found", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            _logger.LogInformation("DELETE /api/users/{UserId} completed successfully", id);
            return NoContent();
        }
    }
}
