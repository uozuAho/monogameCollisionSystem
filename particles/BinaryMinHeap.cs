using System.Collections.Generic;

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

        public BinaryMinHeap(IComparer<T> comparer) : this(comparer, -1, -1) {}

        public static BinaryMinHeap<T> CreateAndPreSize(IComparer<T> comparer, int preSize)
        {
            return new BinaryMinHeap<T>(comparer, preSize);
        }

        /// <summary>
        /// Guaranteed to keep the highest priority N items, where N = maxSize. Memory usage
        /// beyond this is limited.
        /// </summary>
        public static BinaryMinHeap<T> CreateWithSizeLimit(IComparer<T> comparer, int maxSize)
        {
            return new BinaryMinHeap<T>(comparer, maxSize, maxSize);
        }

        private BinaryMinHeap(IComparer<T> comparer, int preSize = -1, int keepMinNItems = -1)
        {
            _comparer = comparer;

            // This sets the max depth of the heap. Anything below this depth
            // can be discarded without worrying about discarding one of the min N items
            _maxSize = keepMinNItems == -1 ? -1 : NextPowerOf2(keepMinNItems) - 1;
            
            _buf = preSize > 0 ? new List<T>(preSize) : new List<T>();
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

        public T RemoveMin()
        {
            return RemoveAtIdx(0);
        }

        public T PeekMin()
        {
            return _buf[0];
        }

        private T RemoveAtIdx(int idx)
        {
            // swap item at idx and last item
            var temp = _buf[idx];
            var lastIdx = Size - 1;
            _buf[idx] = _buf[lastIdx];
            _buf.RemoveAt(lastIdx);
            // sink last item placed at idx
            Sink(idx);
            return temp;
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