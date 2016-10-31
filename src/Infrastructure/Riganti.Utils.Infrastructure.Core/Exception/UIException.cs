using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// An exception with a message which can be presented to the application user.
    /// </summary>
    public class UIException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UIException"/> class.
        /// </summary>
        public UIException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UIException(string message) : base(message)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public UIException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
        
    }
}