using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Models;
using Fibonacci.Logic.Services.Interfaces;

namespace Fibonacci.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly ILogger<FibonacciController> _logger;
        private readonly IFibonacciService _fibonacciService;
        private readonly TaskFactory _taskFactory;
        
        public FibonacciController(ITwoThreadTaskScheduler taskScheduler,
            IFibonacciService fibonacciService,
            ILogger<FibonacciController> logger)
        {
            _taskFactory = new TaskFactory((TaskScheduler)taskScheduler);
            _fibonacciService = fibonacciService;
            _logger = logger;
        }

        /// <summary>
        /// Calculates Fibonacci numbers subsequence
        /// </summary>
        /// <param name="firstIndex">First index of subsequence</param>
        /// <param name="lastIndex">Last index of subsequence</param>
        /// <param name="cacheEnabled">To use cache for calculated numbers</param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="memoryLimitBytes">Maximum size of used memory in bytes</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FibonacciResult> Get(int firstIndex, int lastIndex, bool cacheEnabled,
            int timeoutMilliseconds, int memoryLimitBytes)
        {
            int threadId = 0;

            var task = await _taskFactory.StartNew(() =>
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
                return _fibonacciService.GetSubsequenceAsync(firstIndex, lastIndex, cacheEnabled,
                    timeoutMilliseconds, memoryLimitBytes);
            });

            var result = await task;
            result.ThreadId = threadId;
            return result;
        }
    }
}