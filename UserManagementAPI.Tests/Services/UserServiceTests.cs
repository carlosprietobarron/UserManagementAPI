using System.Threading.Tasks;
using UserManagementAPI.Services;
using UserManagementAPI.Models;
using UserManagementAPI.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace UserManagementAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(NullLogger<UserService>.Instance);
        }

        [Fact]
        public async Task CreateUserAsync_DuplicateEmail_ThrowsDuplicateEmailException()
        {
            // There is sample data seeded with john.doe@example.com
            var req = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Clone",
                Email = "john.doe@example.com"
            };

            await Assert.ThrowsAsync<DuplicateEmailException>(() => _service.CreateUserAsync(req));
        }

        [Fact]
        public async Task DeleteUserAsync_SoftDeletes_UserNoLongerFound()
        {
            // assume initial seeded user has ID 1
            var deleted = await _service.DeleteUserAsync(1);
            Assert.True(deleted);

            var user = await _service.GetUserByIdAsync(1);
            Assert.Null(user);
        }
    }
}
