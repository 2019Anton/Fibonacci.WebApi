using System;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Services.Interfaces;

namespace Fibonacci.Logic.Services
{
    public class MemoryUsageService : IMemoryUsageService
    {
        private readonly IAppMemoryGetter _appMemoryGetter;
        private long _maxMemoryUsageBytes;
        private long _memoryLimitBytes;

        public MemoryUsageService(IAppMemoryGetter appMemoryGetter)
        {
            _appMemoryGetter = appMemoryGetter;
        }

        public long GetMaxMemoryUsed()
        {
            return _maxMemoryUsageBytes;
        }

        public void CalculateMaxMemoryUsage()
        {
            var totalMemory = _appMemoryGetter.GetUsedMemory();
            if (totalMemory >= _maxMemoryUsageBytes)
            {
                _maxMemoryUsageBytes = totalMemory;
            }
        }

        public bool IsLimitExceeded()
        {
            if (_memoryLimitBytes == 0)
            {
                return false;
            }

            return _maxMemoryUsageBytes > _memoryLimitBytes;
        }

        public void SetLimitBytes(int memoryLimitBytes)
        {
            if (memoryLimitBytes < 0) throw new ArgumentOutOfRangeException(nameof(memoryLimitBytes), "cannot be less then zero");
            _memoryLimitBytes = memoryLimitBytes;
        }
    }
}
