using System.Text.RegularExpressions;
using UserManagementAPI.Exceptions;
using UserManagementAPI.Models;

namespace UserManagementAPI.Validators
{
    /// <summary>
    /// Validator for user data
    /// </summary>
    public interface IUserValidator
    {
        void ValidateCreateUserRequest(CreateUserRequest request);
        void ValidateUpdateUserRequest(UpdateUserRequest request);
        void ValidateUserExists(User? user, int userId);
    }

    public class UserValidator : IUserValidator
    {
        private readonly ILogger<UserValidator> _logger;
        private const int MinNameLength = 2;
        private const int MaxNameLength = 100;
        private const int MaxEmailLength = 255;
        private const int MaxPhoneLength = 20;
        private const string EmailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        private const string PhoneRegex = @"^[\d\s\-\+\(\)]{7,20}$";

        public UserValidator(ILogger<UserValidator> logger)
        {
            _logger = logger;
        }

        public void ValidateCreateUserRequest(CreateUserRequest request)
        {
            _logger.LogDebug("Validating CreateUserRequest");

            if (request == null)
            {
                _logger.LogWarning("CreateUserRequest is null");
                throw new ArgumentNullException(nameof(request), "User data cannot be null");
            }

            var errors = new Dictionary<string, string[]>();

            // Validate FirstName
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                errors["FirstName"] = new[] { "FirstName is required" };
            }
            else if (request.FirstName.Length < MinNameLength || request.FirstName.Length > MaxNameLength)
            {
                errors["FirstName"] = new[] { $"FirstName must be between {MinNameLength} and {MaxNameLength} characters" };
            }

            // Validate LastName
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                errors["LastName"] = new[] { "LastName is required" };
            }
            else if (request.LastName.Length < MinNameLength || request.LastName.Length > MaxNameLength)
            {
                errors["LastName"] = new[] { $"LastName must be between {MinNameLength} and {MaxNameLength} characters" };
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors["Email"] = new[] { "Email is required" };
            }
            else if (request.Email.Length > MaxEmailLength)
            {
                errors["Email"] = new[] { $"Email cannot exceed {MaxEmailLength} characters" };
            }
            else if (!IsValidEmail(request.Email))
            {
                errors["Email"] = new[] { "Email format is invalid" };
            }

            // Validate PhoneNumber
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (request.PhoneNumber.Length > MaxPhoneLength)
                {
                    errors["PhoneNumber"] = new[] { $"PhoneNumber cannot exceed {MaxPhoneLength} characters" };
                }
                else if (!IsValidPhoneNumber(request.PhoneNumber))
                {
                    errors["PhoneNumber"] = new[] { "PhoneNumber format is invalid" };
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning("CreateUserRequest validation failed with {ErrorCount} errors", errors.Count);
                throw new ValidationException("Request validation failed", errors);
            }

            _logger.LogDebug("CreateUserRequest validation passed");
        }

        public void ValidateUpdateUserRequest(UpdateUserRequest request)
        {
            _logger.LogDebug("Validating UpdateUserRequest");

            if (request == null)
            {
                _logger.LogWarning("UpdateUserRequest is null");
                throw new ArgumentNullException(nameof(request), "User data cannot be null");
            }

            var errors = new Dictionary<string, string[]>();

            // Validate FirstName
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                errors["FirstName"] = new[] { "FirstName is required" };
            }
            else if (request.FirstName.Length < MinNameLength || request.FirstName.Length > MaxNameLength)
            {
                errors["FirstName"] = new[] { $"FirstName must be between {MinNameLength} and {MaxNameLength} characters" };
            }

            // Validate LastName
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                errors["LastName"] = new[] { "LastName is required" };
            }
            else if (request.LastName.Length < MinNameLength || request.LastName.Length > MaxNameLength)
            {
                errors["LastName"] = new[] { $"LastName must be between {MinNameLength} and {MaxNameLength} characters" };
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors["Email"] = new[] { "Email is required" };
            }
            else if (request.Email.Length > MaxEmailLength)
            {
                errors["Email"] = new[] { $"Email cannot exceed {MaxEmailLength} characters" };
            }
            else if (!IsValidEmail(request.Email))
            {
                errors["Email"] = new[] { "Email format is invalid" };
            }

            // Validate PhoneNumber
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (request.PhoneNumber.Length > MaxPhoneLength)
                {
                    errors["PhoneNumber"] = new[] { $"PhoneNumber cannot exceed {MaxPhoneLength} characters" };
                }
                else if (!IsValidPhoneNumber(request.PhoneNumber))
                {
                    errors["PhoneNumber"] = new[] { "PhoneNumber format is invalid" };
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning("UpdateUserRequest validation failed with {ErrorCount} errors", errors.Count);
                throw new ValidationException("Request validation failed", errors);
            }

            _logger.LogDebug("UpdateUserRequest validation passed");
        }

        public void ValidateUserExists(User? user, int userId)
        {
            if (user == null)
            {
                _logger.LogWarning("User not found during existence check. UserId: {UserId}", userId);
                throw new UserNotFoundException(userId);
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhoneNumber(string phone)
        {
            try
            {
                return Regex.IsMatch(phone, PhoneRegex);
            }
            catch
            {
                return false;
            }
        }
    }
}
