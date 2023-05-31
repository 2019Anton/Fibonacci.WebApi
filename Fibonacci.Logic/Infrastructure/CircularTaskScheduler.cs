using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.Logic.Infrastructure.Interfaces;

namespace Fibonacci.Logic.Infrastructure
{
    public sealed class CircularTaskScheduler : TaskScheduler, IDisposable, ITwoThreadTaskScheduler, ICpuCoresThreadTaskScheduler
    {
        private const byte MaxThreadNumberLimit = 32;

        private readonly CircularLinkedList<BlockingCollection<Task>> _taskCollectionList = new();

        public CircularTaskScheduler(byte threadNumber)
        {
            if (threadNumber < 1 || threadNumber > MaxThreadNumberLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(threadNumber));
            }
            
            for (int i = 0; i < threadNumber; i++)
            {
                var taskCollection = new BlockingCollection<Task>();
                var thread = new Thread(() =>
                {
                    foreach (var task in taskCollection.GetConsumingEnumerable())
                    {
                        TryExecuteTask(task);
                    }
                })
                {
                    IsBackground = true
                };

                thread.Start();

                _taskCollectionList.AddLast(taskCollection);
            }
        }

        protected override void QueueTask(Task task)
        {
            var taskCollection = _taskCollectionList.Next();
            taskCollection.Add(task);
        }
        
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _taskCollectionList.SelectMany(x => x.ToArray());
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var taskCollection in _taskCollectionList)
            {
                taskCollection.CompleteAdding(); 
            }
        }

        #endregion
    }
}