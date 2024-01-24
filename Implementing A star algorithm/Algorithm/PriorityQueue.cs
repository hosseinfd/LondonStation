using System;
using System.Collections.Generic;
using System.Linq;

namespace Implementing_A_star_algorithm.Algorithm;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<(T item, double priority)> items = new List<(T item, double priority)>();

    public void Enqueue(T item, double priority)
    {
        items.Add((item, priority));
        items.Sort((x, y) => x.priority.CompareTo(y.priority));
    }

    public T Dequeue()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }
        var item = items[0].item;
        items.RemoveAt(0);
        return item;
    }

    public bool IsEmpty => !items.Any();

    public bool Contains(T item)
    {
        return items.Any(x => x.item.Equals(item));
    }

    public int Count => items.Count;

}