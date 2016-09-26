using System;

namespace Banana.Core.Exceptions
{
    internal class ErrorOnGenerateLockdowndService : Exception
    {
        public ErrorOnGenerateLockdowndService()
        {
        }

        public ErrorOnGenerateLockdowndService(string message) : base(message)
        {
        }

        public ErrorOnGenerateLockdowndService(string message, Exception inner) : base(message, inner)
        {
        }
    }
}