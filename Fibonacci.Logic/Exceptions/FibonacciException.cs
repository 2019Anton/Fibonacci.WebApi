using System;

namespace Fibonacci.Logic.Exceptions
{
    public class FibonacciException : Exception
    {
        public FibonacciException(string message)
            :base(message)
        {
        }

        public FibonacciException(string message, Exception exception)
            :base(message, exception)
        {
        }
    }
}
