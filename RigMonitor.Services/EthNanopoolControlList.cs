using System;
using System.Collections;
using System.Collections.Generic;
using RigMonitor.Models.DataModels;

namespace RigMonitor.Services
{
    public class EthNanopoolControlList : IEnumerable<EthNanopoolControl>
    {
        EthNanopoolControl head; // головной/первый элемент
        EthNanopoolControl tail; // последний/хвостовой элемент
        int count;  // количество элементов в списке

        public EthNanopoolControl GetHead()
        {
            return head;
        }

        public EthNanopoolControl GetTail()
        {
            return tail;
        }

        public int Length()
        {
            return count;
        }

        public void Add(EthNanopoolControl data)
        {
            EthNanopoolControl node = new EthNanopoolControl(data);

            if (head == null)
                head = node;
            else
            {
                tail.Next = node;
                node.Prev= tail;
            }
            tail = node;
            count++;
        }
        public void AddFirst(EthNanopoolControl data)
        {
            EthNanopoolControl node = new EthNanopoolControl(data);
            EthNanopoolControl temp = head;
            node.Next = temp;
            head = node;
            if (count == 0)
                tail = head;
            else
                temp.Prev= node;
            count++;
        }
        // удаление
        public bool Remove(EthNanopoolControl data)
        {
            var current = head;

            // поиск удаляемого узла
            while (current != null)
            {
                if (current.TimeMoment.Equals(data.TimeMoment))
                {
                    break;
                }
                current = current.Next;
            }
            if (current != null)
            {
                // если узел не последний
                if (current.Next != null)
                {
                    current.Next.Prev= current.Prev;
                }
                else
                {
                    // если последний, переустанавливаем tail
                    tail = current.Prev;
                }

                // если узел не первый
                if (current.Prev != null)
                {
                    current.Prev.Next = current.Next;
                }
                else
                {
                    // если первый, переустанавливаем head
                    head = current.Next;
                }
                count--;
                return true;
            }
            return false;
        }

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public bool Contains(EthNanopoolControl data)
        {
            var current = head;
            while (current != null)
            {
                if (current.Equals(data))
                    return true;
                current = current.Next;
            }
            return false;
        }

        public EthNanopoolControl SearchLatest(DateTime data)
        {
            var current = head;
            while (current != null)
            {
                if (current.TimeMoment < data)
                    return current;
                current = current.Next;
            }
            return head;
        }

        public EthNanopoolControl SearchEarliest(DateTime data)
        {
            var current = head;
            while (current != null)
            {
                if (current.TimeMoment > data)
                    return current;
                current = current.Next;
            }
            return tail;
        }


        public bool ContainsByCreationTime(DateTime timeMoment)
        {
            var current = head;
            while (current != null)
            {
                if (current.TimeMoment.Equals(timeMoment))
                    return true;
                current = current.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }

        IEnumerator<EthNanopoolControl> IEnumerable<EthNanopoolControl>.GetEnumerator()
        {
            EthNanopoolControl current = head;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        public IEnumerable<EthNanopoolControl> BackEnumerator()
        {
            EthNanopoolControl current = tail;
            while (current != null)
            {
                yield return current;
                current = current.Prev;
            }
        }
    }
}
