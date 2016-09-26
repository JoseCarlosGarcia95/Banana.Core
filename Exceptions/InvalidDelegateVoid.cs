using System;

namespace Banana.Core.Exceptions
{
    internal class InvalidDelegateVoid : Exception
    {
        public InvalidDelegateVoid()
        {
        }

        public InvalidDelegateVoid(string message) : base(message)
        {
        }

        public InvalidDelegateVoid(string message, Exception inner) : base(message, inner)
        {
        }
    }
}