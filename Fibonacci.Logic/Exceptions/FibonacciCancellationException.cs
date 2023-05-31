using System;

namespace Fibonacci.Logic.Exceptions
{
    public class FibonacciCancellationException : FibonacciException
    {
        public FibonacciCancellationException(string message)
            :base(message)
        {
        }

        public FibonacciCancellationException(string message, Exception exception)
            :base(message, exception)
        {
        }
    }
}
