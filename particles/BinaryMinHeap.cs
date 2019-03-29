using System;
using System.Collections.Generic;

// copied from cs ai

namespace particles
{
    /// <summary>
    /// Standard binary 'min' heap. Smallest items (based on comparer) float to the top.
    /// </summary>
    public class BinaryMinHeap<T>
    {
        public int Size => _buf.Count;

        private readonly List<T> _buf;
        private readonly IComparer<T> _comparer;
        private readonly int _maxSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="keepMinNItems">
        /// Optional: Limit memory usage while ensuring the min N items can fit in the heap
        /// </param>
        public BinaryMinHeap(IComparer<T> comparer, int keepMinNItems = -1)
        {
            _comparer = comparer;

            // This sets the max depth of the heap. Anything below this depth
            // can be discarded without worrying about discarding one of the min N items
            _maxSize = keepMinNItems == -1 ? -1 : NextPowerOf2(keepMinNItems) - 1;
            
            _buf = _maxSize > 0 ? new List<T>(_maxSize) : new List<T>();
        }

        /// <summary>
        /// Add item to the heap. If an item is discarded due to memory limits, it returned.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>An item discarded from the heap, or default</returns>
        public T Add(T item)
        {
            _buf.Add(item);
            Swim(Size - 1);
            if (_maxSize > 0 && Size > _maxSize)
                return RemoveAtIdx(Size - 1);
            return default(T);
        }

        public void Remove(T item)
        {
            if (Size == 0) throw new InvalidOperationException("cannot remove from empty");

            var idx = IndexOf(item, 0);

            if (idx == -1) throw new InvalidOperationException("cannot remove item not in heap");

            RemoveAtIdx(idx);
        }

        public bool Contains(T item)
        {
            return IndexOf(item, 0) >= 0;
        }

        public T RemoveMin()
        {
            if (Size == 0) throw new InvalidOperationException("cannot remove from empty");
            return RemoveAtIdx(0);
        }

        public T PeekMin()
        {
            if (Size == 0) throw new InvalidOperationException("cannot peek when empty");
            return _buf[0];
        }

        private T RemoveAtIdx(int idx)
        {
            if (idx >= Size) throw new ArgumentOutOfRangeException();

            // swap item at idx and last item
            var temp = _buf[idx];
            var lastIdx = Size - 1;
            _buf[idx] = _buf[lastIdx];
            _buf.RemoveAt(lastIdx);
            // sink last item placed at idx
            Sink(idx);
            return temp;
        }

        private int IndexOf(T item, int subRoot)
        {
            if (subRoot >= Size) {
                // gone past leaf
                return -1;
            }
            if (_comparer.Compare(item, _buf[subRoot]) == -1)
            {
                // item is less than current node - will not be in this subtree
                return -1;
            }
            if (item.Equals(_buf[subRoot]))
            {
                return subRoot;
            }

            var idx = IndexOf(item, subRoot * 2 + 1);
            return idx >= 0
                ? idx
                : IndexOf(item, subRoot * 2 + 2);
        }

        private void Swim(int idx)
        {
            if (idx == 0) return;

            var parentIdx = (idx - 1) / 2;

            while (_comparer.Compare(_buf[idx], _buf[parentIdx]) == -1)
            {
                // swap if child < parent
                Swap(idx, parentIdx);
                if (parentIdx == 0) return;
                idx = parentIdx;
                parentIdx = (idx - 1) / 2;
            }
        }

        private void Sink(int idx)
        {
            while (true)
            {
                var leftIdx = 2 * idx + 1;
                var rightIdx = 2 * idx + 2;
                // stop if no children
                if (leftIdx >= Size) return;
                // get minimum child
                var minIdx = leftIdx;
                if (rightIdx < Size && _comparer.Compare(_buf[rightIdx], _buf[leftIdx]) == -1)
                {
                    minIdx = rightIdx;
                }
                // swap if parent > min child
                if (_comparer.Compare(_buf[idx], _buf[minIdx]) == 1)
                {
                    Swap(idx, minIdx);
                }
                else
                {
                    return;
                }
                idx = minIdx;
            }
        }

        private void Swap(int idxA, int idxB)
        {
            var temp = _buf[idxA];
            _buf[idxA] = _buf[idxB];
            _buf[idxB] = temp;
        }

        private static int NextPowerOf2(int num)
        {
            var nextPower = 1;
            while (nextPower <= num) nextPower *= 2;
            return nextPower;
        }
    }
}