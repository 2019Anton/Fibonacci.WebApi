using System.Threading.Tasks;
using Fibonacci.Logic.Models;

namespace Fibonacci.Logic.Services.Interfaces
{
    public interface IFibonacciService
    {
        Task<FibonacciResult> GetSubsequenceAsync(int firstIndex, int lastIndex, bool cacheEnabled, int timeoutMilliseconds, int maxMemorySizeBytes);
    }
}
