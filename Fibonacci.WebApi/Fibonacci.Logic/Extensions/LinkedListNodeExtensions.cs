using System.Collections.Generic;

namespace Fibonacci.Logic.Extensions
{
    public static class LinkedListNodeExtensions
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List?.First;
        }
    }
}