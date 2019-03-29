using System.Collections.Generic;

// copied from cs ai

namespace particles
{
    /// <summary>
    /// Min priority queue
    /// </summary>
    public class MinPQ<T>
    {
        private readonly BinaryMinHeap<T> _minHeap;

        public MinPQ(IComparer<T> comparer, int keepMinNItems = -1)
        {
            _minHeap = new BinaryMinHeap<T>(comparer, keepMinNItems);
        }

        public bool IsEmpty => _minHeap.Size == 0;

        public T Push(T item)
        {
            return _minHeap.Add(item);
        }

        public T Pop()
        {
            return _minHeap.RemoveMin();
        }

        public T Peek()
        {
            return _minHeap.PeekMin();
        }
    }
}