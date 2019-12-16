using System;

namespace DotNetInsights.Shared.Library.Exceptions
{
    public sealed class ValidateException : Exception
    {
        public ValidateException(string memberName, string errorMessage)
            : base(string.Format("A validation error has occurred {0}:{1}", memberName, errorMessage))
        {
            MemberName = memberName;
            ErrorMessage = errorMessage;
        }

        public string MemberName { get; }
        public string ErrorMessage { get; }

        private ValidateException()
        {
        }

        private ValidateException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}