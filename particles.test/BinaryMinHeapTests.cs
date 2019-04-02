using System.Collections.Generic;
using NUnit.Framework;

namespace particles.test
{
    public class BinaryMinHeapTests
    {
        private BinaryMinHeap<int> _intHeap;

        [Test]
        public void GivenKeepMin2Items_ShouldHold3Not4()
        {
            _intHeap = BinaryMinHeap<int>.CreateWithSizeLimit(new IntComparer(), 2);

            Assert.AreEqual(0, _intHeap.Add(1));
            Assert.AreEqual(0, _intHeap.Add(2));
            Assert.AreEqual(0, _intHeap.Add(3));

            // level 2 is now full, should discard on next add
            Assert.AreEqual(4, _intHeap.Add(4));
        }

        [Test]
        public void GivenKeepMin2Items_AddInReverseOrder_ShouldHold3Not4()
        {
            _intHeap = BinaryMinHeap<int>.CreateWithSizeLimit(new IntComparer(), 2);

            Assert.AreEqual(0, _intHeap.Add(4));
            Assert.AreEqual(0, _intHeap.Add(3));
            Assert.AreEqual(0, _intHeap.Add(2));

            // level 2 is now full, should discard on next add
            Assert.AreEqual(4, _intHeap.Add(1));
        }

        private class IntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x < y ? -1 : x > y ? 1 : 0;
            }
        }
    }
}
