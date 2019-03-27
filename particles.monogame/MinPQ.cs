using System.Collections.Generic;

// copied from cs ai

namespace particles.monogame
{
    /// <summary>
    /// Min priority queue
    /// </summary>
    public class MinPQ<T>
    {
        private readonly BinaryMinHeap<T> _minHeap;

        public MinPQ(IComparer<T> comparer)
        {
            _minHeap = new BinaryMinHeap<T>(comparer);
        }

        public bool IsEmpty => _minHeap.Size == 0;

        public void Push(T item)
        {
            _minHeap.Add(item);
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