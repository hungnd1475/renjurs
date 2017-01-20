using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations
{
    public class Heap<T> : IEnumerable<T>
    {
        private T[] usingArray; // current array being used
        private T[] firstArray;
        private T[] secondArray;

        private readonly Comparer<T> comparer;

        /// <summary>
        /// Gets the number of elements actually contained in the heap.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the total number of elements that this heap can contained without resizing.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the value indicates if this heap is a max heap.
        /// </summary>
        public readonly bool IsMaxHeap;

        private int lastIndex { get { return Count - 1; } }
        private readonly int comparedValue = 1; //1 - MaxHeap, -1 - MinHeap

        /// <summary>
        /// Gets the element at a specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return usingArray[index]; }
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Heap{T}"/> that is empty and has the default initial capacity.
        /// </summary>
        /// <param name="isMaxHeap"></param>
        public Heap(bool isMaxHeap, Comparer<T> comparer)
            : this(7, isMaxHeap, comparer)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Heap{T}"/> that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="initCapacity"></param>
        /// <param name="isMaxHeap"></param>
        public Heap(int initCapacity, bool isMaxHeap, Comparer<T> comparer)
        {
            this.comparer = comparer;

            firstArray = new T[initCapacity];
            secondArray = null;
            usingArray = firstArray;
            
            this.Capacity = initCapacity;
            this.Count = 0;

            comparedValue = isMaxHeap ? 1 : -1;
            IsMaxHeap = isMaxHeap;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Heap{T}"/> that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isMaxHeap"></param>
        public Heap(IEnumerable<T> list, bool isMaxHeap, Comparer<T> comparer)
        {
            this.comparer = comparer;
            this.Count = list.Count();

            this.Capacity = 2;
            while (Capacity - 1 < Count)
                Capacity = Capacity << 1;

            Capacity -= 1;
            
            firstArray = new T[Capacity];
            secondArray = null;
            usingArray = firstArray;

            int i = 0;
            foreach (var item in list)
                firstArray[i++] = item;

            for (i = (lastIndex - 1) / 2; i >= 0; i--)
                DownHeap(i);

            comparedValue = isMaxHeap ? 1 : -1;
            IsMaxHeap = isMaxHeap;
        }

        public void DownHeap(int index)
        {
            while (true)
            {
                int left = (index + 1) * 2 - 1;
                int right = (index + 1) * 2;

                if (left >= Count) break;

                int expect = left; // expect to swap with left node.

                if (right < Count && comparer.Compare(usingArray[right], usingArray[left]) == comparedValue) // but right node is better.
                {
                    expect = right;
                }

                if (comparer.Compare(usingArray[index], usingArray[expect]) == -comparedValue) // if current node needs to get down
                {
                    swap(index, expect); // get it down
                    index = expect; // continue with the swapped node.
                }
                else break;
            }
        }

        private void swap(int index1, int index2)
        {
            var temp = usingArray[index1];
            usingArray[index1] = usingArray[index2];
            usingArray[index2] = temp;
        }

        public void UpHeap(int index)
        {
            while (index >= 0)
            {
                int parent = (index - 1) / 2;
                if (comparer.Compare(usingArray[index], usingArray[parent]) == comparedValue)
                {
                    swap(parent, index);
                    index = parent;
                }
                else break;
            }
        }

        /// <summary>
        /// Adds new element to the end of this heap and moves it to the right position.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            if (Count == Capacity)
            {
                increaseSize();
            }
            int index = Count++;
            usingArray[index] = value;
            UpHeap(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Remove()
        {
            var result = usingArray[0];
            swap(0, --Count);
            DownHeap(0);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return usingArray[0];
        }

        private void increaseSize()
        {
            Capacity = (Capacity + 1) * 2 - 1;
            if (firstArray == null)
            {
                firstArray = new T[Capacity];
                Array.Copy(secondArray, firstArray, Count);
                usingArray = firstArray;
                secondArray = null;
            }
            else if (secondArray == null)
            {
                secondArray = new T[Capacity];
                Array.Copy(firstArray, secondArray, Count);                
                usingArray = secondArray;
                firstArray = null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return usingArray[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
