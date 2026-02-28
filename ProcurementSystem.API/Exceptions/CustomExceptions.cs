namespace ProcurementSystem.API.Exceptions
{
    public class CustomExceptions
    {
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class DuplicateException : Exception
        {
            public DuplicateException(string message) : base(message) { }
        }

        public class DatabaseException : Exception
        {
            public DatabaseException(string message, Exception innerException):base(message,innerException) { }
        }
    }
}
