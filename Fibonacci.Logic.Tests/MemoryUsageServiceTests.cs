using System;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Services;
using Moq;
using NUnit.Framework;

namespace Fibonacci.Logic.Tests
{
    public class MemoryUsageServiceTests
    {
        [Test]
        public void SetLimitBytes_LimitLessThenZero_Exception()
        {
            // Arrange
            int limitBytes = -1;
            var appMemoryGetter = new Mock<IAppMemoryGetter>();
            appMemoryGetter.Setup(x => x.GetUsedMemory()).Returns(1000);

            var memoryUsageService = new MemoryUsageService(appMemoryGetter.Object);

            // Acts
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => memoryUsageService.SetLimitBytes(limitBytes));
        }

        [TestCase(2, 2, false)]
        [TestCase(2, 1, false)]
        [TestCase(1, 2, true)]
        public void IsLimitExceed_CorrectResult(int limit, int maxUsed, bool targetResult)
        {
            // Arrange
            var appMemoryGetter = new Mock<IAppMemoryGetter>();
            appMemoryGetter.Setup(x => x.GetUsedMemory()).Returns(maxUsed);
            var memoryUsageService = new MemoryUsageService(appMemoryGetter.Object);

            // Acts
            memoryUsageService.SetLimitBytes(limit);
            memoryUsageService.CalculateMaxMemoryUsage();
            var result = memoryUsageService.IsLimitExceeded();

            // Assert
            Assert.AreEqual(targetResult, result);
        }

        [Test]
        public void GetMaxMemoryUsed_MaxValueFromAppMemoryGetter()
        {
            // Arrange
            var usedMemory1 = 5;
            var usedMemory2 = 9;
            var usedMemory3 = 7;

            var appMemoryGetter = new Mock<IAppMemoryGetter>();
            var memoryUsageService = new MemoryUsageService(appMemoryGetter.Object);

            // Acts
            appMemoryGetter.Setup(x => x.GetUsedMemory()).Returns(usedMemory1);
            memoryUsageService.CalculateMaxMemoryUsage();
            appMemoryGetter.Setup(x => x.GetUsedMemory()).Returns(usedMemory2);
            memoryUsageService.CalculateMaxMemoryUsage();
            appMemoryGetter.Setup(x => x.GetUsedMemory()).Returns(usedMemory3);
            memoryUsageService.CalculateMaxMemoryUsage();

            var result = memoryUsageService.GetMaxMemoryUsed();

            // Assert
            Assert.AreEqual(usedMemory2, result);
        }
    }
}
