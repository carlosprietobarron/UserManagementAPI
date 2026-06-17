namespace UserManagementAPI.Exceptions
{
    /// <summary>
    /// Exception thrown when a user is not found
    /// </summary>
    public class UserNotFoundException : Exception
    {
        public int UserId { get; set; }

        public UserNotFoundException(int userId)
            : base($"User with ID {userId} was not found.")
        {
            UserId = userId;
        }

        public UserNotFoundException(int userId, string message)
            : base(message)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Exception thrown when user data is invalid
    /// </summary>
    public class InvalidUserDataException : Exception
    {
        public string? FieldName { get; set; }

        public InvalidUserDataException(string message)
            : base(message)
        {
        }

        public InvalidUserDataException(string fieldName, string message)
            : base(message)
        {
            FieldName = fieldName;
        }
    }

    /// <summary>
    /// Exception thrown when email already exists
    /// </summary>
    public class DuplicateEmailException : Exception
    {
        public string Email { get; set; }

        public DuplicateEmailException(string email)
            : base($"A user with email '{email}' already exists.")
        {
            Email = email;
        }
    }

    /// <summary>
    /// Exception thrown for validation errors
    /// </summary>
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationException(string message, Dictionary<string, string[]>? errors = null)
            : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}
