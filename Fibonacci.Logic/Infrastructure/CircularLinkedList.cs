using System;
using System.Collections;
using System.Collections.Generic;
using Fibonacci.Logic.Extensions;

namespace Fibonacci.Logic.Infrastructure
{
    public class CircularLinkedList<T> : IEnumerable<T> where T: class
    {
        private LinkedListNode<T> _currentNode;
        private readonly LinkedList<T> _linkedList;

        public CircularLinkedList()
        {
            _linkedList = new LinkedList<T>();
        }

        public T Next()
        {
            lock (_linkedList)
            {
                if (_linkedList.Count == 0)
                    throw new InvalidOperationException("List is empty.");

                _currentNode = _currentNode == null ? _linkedList.First : _currentNode.NextOrFirst();

                return _currentNode?.Value;
            }
        }

        public void AddLast(T tasks)
        {
            lock (_linkedList)
            {
                _linkedList.AddLast(tasks);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_linkedList)
            {
                return _linkedList.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_linkedList)
            {
                return _linkedList.GetEnumerator();
            }
        }
    }
}