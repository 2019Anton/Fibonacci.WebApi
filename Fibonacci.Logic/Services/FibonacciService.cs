using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.Logic.Infrastructure;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Models;
using Fibonacci.Logic.Services.Interfaces;

namespace Fibonacci.Logic.Services
{
    public class FibonacciService : IFibonacciService
    {
        private readonly TaskFactory _taskFactory;
        private readonly ICacheService _cacheService;
        private readonly IMemoryUsageService _memoryUsageService;
        private CancellationTokenSource _cancelTokenSource;

        public FibonacciService(ICpuCoresThreadTaskScheduler taskScheduler, ICacheService cacheService, IMemoryUsageService memoryUsageService)
        {
            _taskFactory = new TaskFactory((TaskScheduler) taskScheduler);
            _cacheService = cacheService;
            _memoryUsageService = memoryUsageService;
        }

        public async Task<FibonacciResult> GetSubsequenceAsync(int firstIndex, int lastIndex, bool cacheEnabled, int timeoutMilliseconds, int memoryLimitBytes)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (firstIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(firstIndex),"Cannot be less then zero");
            }

            if (lastIndex < firstIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(lastIndex), $"Cannot be less then {firstIndex}");
            }

            if (timeoutMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds), "Cannot be less then zero");
            }

            _cancelTokenSource = timeoutMilliseconds == 0 ? new CancellationTokenSource() 
                : new CancellationTokenSource(timeoutMilliseconds);

            _memoryUsageService.SetLimitBytes(memoryLimitBytes);
            _cacheService.Enabled = cacheEnabled;

            var numbers = new ConcurrentBag<FibonacciNumber>();
            var result = new FibonacciResult();
            try
            {
                for (var i = firstIndex; i <= lastIndex; i++)
                {
                    var index = i;

                    await _taskFactory.StartNew(() =>
                    {
                        var number = GetNumber(index);
                        if (number == null) // Cancellation 
                        {
                            return;
                        }

                        number.ThreadId = Thread.CurrentThread.ManagedThreadId;

                        lock (numbers)
                        {
                            numbers.Add(number);
                        }
                    }, _cancelTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                if (numbers.Count == 0)
                {
                    result.ErrorMessage = "Failed to calculate any result";
                }
                else
                {
                    result.Numbers = numbers.OrderBy(x => x.Index).ToList();
                }
            }
            finally
            {
                stopwatch.Stop();
                result.TimeoutExceeded = timeoutMilliseconds != 0 && stopwatch.ElapsedMilliseconds > timeoutMilliseconds;
                result.MemoryLimitExceeded = _memoryUsageService.IsLimitExceeded();
                result.MaxMemoryUsed = _memoryUsageService.GetMaxMemoryUsed();
                result.ExecutionTime = stopwatch.ElapsedMilliseconds;
                result.Numbers = numbers.OrderBy(x => x.Index).ToList();
            }
            return result;
        }

        private FibonacciNumber GetNumber(int index)
        {
            if (_cancelTokenSource.IsCancellationRequested)
            {
                return null;
            }

            _memoryUsageService.CalculateMaxMemoryUsage();
            if (_memoryUsageService.IsLimitExceeded())
            {
                _cancelTokenSource.Cancel(false);
                return null;
            }

            if (index is 0 or 1)
            {
                return new FibonacciNumber(index, 1);
            }

            if (_cacheService.TryGet(index, out BigInteger cachedValue))
            {
                return new FibonacciNumber(index, cachedValue);
            }

            var prevNumber = GetNumber(index - 1);
            var prevPrevNumber = GetNumber(index - 2);

            // Cancellation
            if (prevNumber == null || prevPrevNumber == null)
            {
                return null;
            }

            var value = prevPrevNumber.Value + prevNumber.Value;

            _cacheService.Set(index, value);
            return new FibonacciNumber(index, value);
        }
    }
}
