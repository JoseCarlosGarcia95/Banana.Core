using System;

namespace Banana.Core.Exceptions
{
    internal class ErrorOnConnect : Exception
    {
        public ErrorOnConnect()
        {
        }

        public ErrorOnConnect(string message) : base(message)
        {
        }

        public ErrorOnConnect(string message, Exception inner) : base(message, inner)
        {
        }
    }
}