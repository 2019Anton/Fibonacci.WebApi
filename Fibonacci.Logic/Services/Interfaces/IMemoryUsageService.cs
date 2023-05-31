namespace Fibonacci.Logic.Services.Interfaces
{
    public interface IMemoryUsageService
    {
        void SetLimitBytes(int memoryLimitBytes);
        bool IsLimitExceeded();
        long GetMaxMemoryUsed();
        void CalculateMaxMemoryUsage();
    }
}
