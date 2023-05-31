using System.Collections.Generic;

namespace Fibonacci.Logic.Models
{
    public class FibonacciResult
    {
        public string ErrorMessage { get; set; }
        public bool TimeoutExceeded { get; set; }
        public bool MemoryLimitExceeded { get; set; }
        public long MaxMemoryUsed { get; set; }
        public long ExecutionTime { get; set; }
        public int ThreadId { get; set; }
        public long Amount => Numbers?.Count ?? 0;
        public List<FibonacciNumber> Numbers { get; set; } = new();
    }
}
