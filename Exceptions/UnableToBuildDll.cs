using System;

namespace Banana.Core.Exceptions
{
    public class UnableToBuildDll : Exception
    {
        public UnableToBuildDll()
        {
        }

        public UnableToBuildDll(string message) : base(message)
        {
        }

        public UnableToBuildDll(string message, Exception inner) : base(message, inner)
        {
        }
    }
}