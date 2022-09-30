using System;
using System.Runtime.Serialization;

namespace FC.Manager.ViewModel.Login
{
    [Serializable]
    internal class EmailNullException : Exception
    {
        public EmailNullException() : base(Domain.Properties.Resources.Email_Null_Exception)
        {
        }

        public EmailNullException(string message) : base(message)
        {
        }

        public EmailNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmailNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}