namespace Lside_Mixture.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class BoundedQueue<T> : IEnumerable
    {
        private readonly LinkedList<T> internalList = new LinkedList<T>();
        private readonly int maxQueueSize;

        public BoundedQueue(int queueSize)
        {
            if (queueSize < 0)
            {
                throw new ArgumentException("queueSize");
            }

            this.maxQueueSize = queueSize;
        }

        public BoundedQueue(BoundedQueue<T> responses)
        {
            this.maxQueueSize = responses.maxQueueSize;
            foreach (T item in this.internalList)
            {
                this.internalList.AddLast(item);
            }
        }

        // adds to End, oldest will have at position [0]
        public void Enqueue(T elem)
        {
            if (this.internalList.Count == this.maxQueueSize)
            {
                // make room
                this.Dequeue();
            }

            this.internalList.AddLast(elem);
        }

        public T Dequeue()
        {
            if (this.internalList.Count == 0)
            {
                throw new BoundedQueueException("Empty");
            }

            T elem = this.internalList.First.Value;
            this.internalList.RemoveFirst();
            return elem;
        }

        public T ElementAt(int v)
        {
            return this.internalList.ElementAt(v);
        }

        public int Count()
        {
            return this.internalList.Count;
        }

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        // updates on the returned list will compromise the BoundedQueue
        public LinkedList<T> GetInternalLinkList()
        {
            return this.internalList;
        }
    }
}
