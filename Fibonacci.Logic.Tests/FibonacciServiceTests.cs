using System;
using System.Numerics;
using System.Threading.Tasks;
using Fibonacci.Logic.Infrastructure;
using Fibonacci.Logic.Services;
using Fibonacci.Logic.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace Fibonacci.Logic.Tests
{
    public class FibonacciServiceTests
    {
        [TestCase(-1, 1)]
        [TestCase(0, -1)]
        [TestCase(2, 1)]
        public void GetSubsequenceAsync_FailIndexParams_Exception(int firstIndex, int lastIndex)
        {
            // Arrange
            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), new Mock<ICacheService>().Object,
                new Mock<IMemoryUsageService>().Object);

            // Act
            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => 
                await fibonacciService.GetSubsequenceAsync(firstIndex, lastIndex, false, 0, 0));
        }

        [TestCase(0, 1)]
        public void GetSubsequenceAsync_CorrectIndexParams_NoException(int firstIndex, int lastIndex)
        {
            // Arrange
            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), new Mock<ICacheService>().Object,
                new Mock<IMemoryUsageService>().Object);

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () => 
                await fibonacciService.GetSubsequenceAsync(firstIndex, lastIndex, false, 0, 0));
        }

        [Test]
        public async Task GetSubsequenceAsync_SetLimitBytesCalledOnce()
        {
            // Arrange
            var memoryLimitBytes = 0;
            var memoryUsageServiceMock = new Mock<IMemoryUsageService>();

            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), new Mock<ICacheService>().Object,
                memoryUsageServiceMock.Object);

            // Act
            await fibonacciService.GetSubsequenceAsync(0, 0, false, 0, memoryLimitBytes);

            // Assert
            memoryUsageServiceMock.Verify(x => x.SetLimitBytes(memoryLimitBytes), Times.Once());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetSubsequenceAsync_CacheEnabled_CacheEnabledCalledOnce(bool cacheEnabled)
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheService>();
            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), cacheServiceMock.Object,
                new Mock<IMemoryUsageService>().Object);

            // Act
            await fibonacciService.GetSubsequenceAsync(0, 0, cacheEnabled, 0, 0);

            // Assert
            cacheServiceMock.VerifySet(x => x.Enabled = cacheEnabled, Times.Once());
        }
        
        [Test]
        public async Task GetSubsequenceAsync_CacheEnabled_CacheServiceIsUsed()
        {
            // Arrange
            var cacheEnabled = true;
            var cacheServiceMock = new Mock<ICacheService>();
            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), cacheServiceMock.Object,
                new Mock<IMemoryUsageService>().Object);

            // Act
            await fibonacciService.GetSubsequenceAsync(2, 2, cacheEnabled, 0, 0);

            // Assert
            cacheServiceMock.Verify(x => x.Set(2, 2), Times.Once());
            BigInteger bigInt;
            cacheServiceMock.Verify(x => x.TryGet(2, out bigInt), Times.Once());
        }

        [TestCase(0, 0, false, 0, 0, new[] { 1 })]
        [TestCase(1, 1, false, 0, 0, new[] { 1 })]
        [TestCase(2, 5, false, 0, 0, new[] { 2, 3, 5, 8 })]
        public async Task GetSubsequenceAsync_CorrectResults(int firstIndex, int lastIndex, bool cacheEnabled, int timeout, int memoryLimit, int[] targetResult)
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheService>();
            var fibonacciService = new FibonacciService(new CircularTaskScheduler(4), cacheServiceMock.Object,
                new Mock<IMemoryUsageService>().Object);

            // Act
            var result = await fibonacciService.GetSubsequenceAsync(firstIndex, lastIndex, cacheEnabled, timeout, memoryLimit);

            // Assert
            Assert.True(AreResultsEqual(targetResult, result));
        }

        private bool AreResultsEqual(int[] targetResult, Models.FibonacciResult result)
        {
            if (targetResult == null) throw new NullReferenceException();
            if (result == null || result.Numbers == null) throw new NullReferenceException();

            if (targetResult.Length != result.Numbers.Count)
            {
                return false;
            }

            int i = 0;
            foreach (var number in result.Numbers)
            {
                if (number.Value != targetResult[i])
                {
                    return false;
                }

                i++;
            }

            return true;
        }
    }
}