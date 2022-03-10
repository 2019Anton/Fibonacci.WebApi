using System;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Services.Interfaces;

namespace Fibonacci.Logic.Infrastructure
{
    public class AppMemoryGetter : IAppMemoryGetter
    {
        public long GetUsedMemory()
        {
            return GC.GetTotalMemory(true);
        }
    }
}
