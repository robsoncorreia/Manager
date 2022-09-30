using System;
using System.Runtime.Serialization;

namespace FC.Manager.ViewModel.Login
{
    [Serializable]
    internal class PasswordNullException : Exception
    {
        public PasswordNullException() : base(message: Domain.Properties.Resources.Password_Null_Exception)
        {
        }

        public PasswordNullException(string message) : base(message)
        {
        }

        public PasswordNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}