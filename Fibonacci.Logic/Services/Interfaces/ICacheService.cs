using System.Numerics;

namespace Fibonacci.Logic.Services.Interfaces
{
    public interface ICacheService
    {
        bool Enabled { get; set; }

        void Set(int index, BigInteger value);

        bool TryGet(int index, out BigInteger value);
    }
}
