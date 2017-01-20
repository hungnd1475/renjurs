using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations
{
    public class SortedLinkedList<T> : IEnumerable<T>, ICollection<T>
    {
        private readonly LinkedList<T> internalList;
        private readonly int compareValue = 1;
        private readonly IComparer<T> comparer;

        public bool IsAscendingOrder
        {
            get { return compareValue == 1; }
        }

        public SortedLinkedList()
            : this(Comparer<T>.Default, true)
        {
        }

        public SortedLinkedList(bool isAscendingOrder)
            : this(Comparer<T>.Default, isAscendingOrder)
        {
        }

        public SortedLinkedList(IComparer<T> comparer)
            : this(comparer, true)
        {
        }

        public SortedLinkedList(IComparer<T> comparer, bool isAscendingOrder)
        {
            internalList = new LinkedList<T>();
            compareValue = isAscendingOrder ? 1 : -1;
            this.comparer = comparer;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        // || comparer.Compare(node.Value, value) == 0)
        public void Add(T value)
        {
            LinkedListNode<T> node = internalList.First, preNode = null;
            while (node != null && (comparer.Compare(node.Value, value) == compareValue))
            {
                preNode = node;
                node = node.Next;
            }
            if (node == null)
            {
                internalList.AddLast(value);
            }
            else if (preNode == null)
            {
                internalList.AddFirst(value);
            }
            else
            {
                internalList.AddAfter(preNode, value);
            }
        }

        public void Clear()
        {
            internalList.Clear();
        }

        public bool Contains(T item)
        {
            return internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T value)
        {
            return internalList.Remove(value);
        }

        public void Remove(LinkedListNode<T> node)
        {
            internalList.Remove(node);
        }

        public LinkedListNode<T> First
        {
            get { return internalList.First; }
        }

        public LinkedListNode<T> Last
        {
            get { return internalList.Last; }
        }
    }
}
