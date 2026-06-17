using System;
using UserManagementAPI.Models;
using UserManagementAPI.Validators;
using UserManagementAPI.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace UserManagementAPI.Tests.Validators
{
    public class UserValidatorTests
    {
        private readonly UserValidator _validator;

        public UserValidatorTests()
        {
            _validator = new UserValidator(NullLogger<UserValidator>.Instance);
        }

        [Fact]
        public void ValidateCreateUserRequest_Valid_DoesNotThrow()
        {
            var req = new CreateUserRequest
            {
                FirstName = "Alice",
                LastName = "Wonderland",
                Email = "alice@example.com",
                PhoneNumber = "+1 555-555-5555"
            };

            _validator.ValidateCreateUserRequest(req);
        }

        [Fact]
        public void ValidateCreateUserRequest_InvalidEmail_ThrowsValidationException()
        {
            var req = new CreateUserRequest
            {
                FirstName = "Bob",
                LastName = "Builder",
                Email = "invalid-email",
            };

            var ex = Assert.Throws<ValidationException>(() => _validator.ValidateCreateUserRequest(req));
            Assert.Contains("Email", ex.Errors.Keys);
        }
    }
}
