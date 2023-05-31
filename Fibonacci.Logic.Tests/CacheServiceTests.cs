using System.Numerics;
using System.Threading;
using Fibonacci.Logic.Services;
using NUnit.Framework;

namespace Fibonacci.Logic.Tests
{
    public class CacheServiceTests
    {
        [Test]
        public void Set_EnabledFalse_Fail()
        {
            // Arrange
            int index = 1;
            BigInteger value = 10;
            var cacheService = new CacheService(0) {Enabled = false};

            // Act
            cacheService.Set(index, value);

            // Assert
            Assert.False(cacheService.TryGet(index, out _));
        }

        [Test]
        public void Set_EnabledTrue_Success()
        {
            // Arrange
            int index = 1;
            BigInteger testValue = 10;
            var cacheService = new CacheService(0) {Enabled = true};

            // Act
            cacheService.Set(index, testValue);

            // Assert
            Assert.True(cacheService.TryGet(index, out BigInteger value));
            Assert.AreEqual(testValue, value);
        }

        [Test]
        public void TryGet_NotSet_False()
        {
            // Arrange
            int index = 1;
            BigInteger testFailValue = -1;
            var cacheService = new CacheService(0) {Enabled = true};

            // Act
            var result = cacheService.TryGet(index, out BigInteger value);

            // Assert
            Assert.False(result);
            Assert.AreEqual(testFailValue, value);
        }

        [Test]
        public void Invalidate_WaitMoreThanLifetime_Cleaned()
        {
            // Arrange
            int index = 1;
            BigInteger testValue = 10;
            BigInteger testFailValue = -1;
            int lifetime = 50;

            var cacheService = new CacheService(lifetime) {Enabled = true};
            cacheService.Set(index, testValue);

            // Act
            Thread.Sleep(lifetime*2);

            // Assert
            var result = cacheService.TryGet(index, out BigInteger value);
            Assert.False(result);
            Assert.AreEqual(testFailValue, value);
        }

        [Test]
        public void Invalidate_WaitLessThanLifetime_NotCleaned()
        {
            // Arrange
            int index = 1;
            BigInteger testValue = 10;
            int lifetime = 100;

            var cacheService = new CacheService(lifetime) {Enabled = true};
            cacheService.Set(index, testValue);

            // Act
            Thread.Sleep(lifetime/2);

            // Assert
            var result = cacheService.TryGet(index, out BigInteger value);
            Assert.True(result);
            Assert.AreEqual(testValue, value);
        }
    }
}