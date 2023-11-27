// Namespaces
using System;

/// <summary>
/// An interface that ensures the object is available to use in a heap.
/// </summary>
/// <typeparam name="T">The type of item to store in the heap.</typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    #region Properties
    /// <summary>
    /// The property to store the item's Heap Index.
    /// </summary>
    public int HeapIndex
    {
        get;
        set;
    }
    #endregion
}

/// <summary>
/// Improved structure to manage larger amounts of data.
/// </summary>
/// <typeparam name="T">The item type (only available if it utilizes the IHeapItem interface).</typeparam>
public class Heap<T> where T : IHeapItem<T>
{
    #region Fields
    private readonly T[] items;     // the items stored in the heap
    private int currentItemCount;   // the number of items in the heap
    #endregion

    #region Properties
    /// <summary>
	/// Gets the number of items in the heap.
	/// </summary>
	public int Count
    {
        get
        {
            return currentItemCount;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Constructs & initializes the Heap class w/ a max heap size.
    /// </summary>
    /// <param name="maxHeapSize">The maximum number of items in the heap.</param>
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Adds an item to the heap.
    /// </summary>
    /// <param name="item">The item to add to the heap.</param>
    public void Add(T item)
    {
        // Update the item to reflect position in the heap
        item.HeapIndex = currentItemCount;

        // Add the item to the heap in the last position
        items[currentItemCount] = item;

        // Sort the item up the heap
        SortUp(item);

        // Increment the # of items in the heap
        currentItemCount++;
    }

    /// <summary>
    /// Removes the first item in the heap.
    /// </summary>
    /// <returns></returns>
    public T RemoveFirst()
    {
        // Store the first item
        T firstItem = items[0];

        // Reduce the # of items
        currentItemCount--;

        // Update the last item in the heap to be the first
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        // Sort the item down the heap 
        SortDown(items[0]);

        // Return the removed element
        return firstItem;
    }

    /// <summary>
    /// Updates the item's position via sorting upward.
    /// </summary>
    /// <param name="item">The item to sort.</param>
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    /// <summary>
    /// Determines if the heap contains an item.
    /// </summary>
    /// <param name="item">The item to check if it's in the heap.</param>
    /// <returns>True if the heap has the item, false otherwise.</returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /// <summary>
    /// Sorts the item toward the end of the heap.
    /// </summary>
    /// <param name="item">The item to sort.</param>
    void SortDown(T item)
    {
        // Loop until there are no more swaps to make
        while (true)
        {
            // Get the child indexes
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;

            // The index to swap the item with
            int swapIndex;

            // If the left child index is in range
            if (childIndexLeft < currentItemCount)
            {
                // Store the left child index
                swapIndex = childIndexLeft;

                // If the right child index is in range
                if (childIndexRight < currentItemCount)
                {
                    // If the left child precedes the right child
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        // Store the right child index
                        swapIndex = childIndexRight;
                    }
                }

                // If the item precedes the right child
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    // Swap the items positions
                    Swap(item, items[swapIndex]);
                }
                // If the item doesn't precede the right child, leave the item where it is & exit loop
                else
                {
                    return;
                }

            }
            // If the first child index isn't in range, leave the item where it is & exit the loop
            else
            {
                return;
            }

        }
    }

    /// <summary>
    /// Sorts the item toward the top of the heap.
    /// </summary>
    /// <param name="item">The item to sort.</param>
    private void SortUp(T item)
    {
        // Get the parent index
        int parentIndex = (item.HeapIndex - 1) / 2;

        // Loop until there are no more swaps to make
        while (true)
        {
            // Store the parent item
            T parentItem = items[parentIndex];

            // If the item follows the parent
            if (item.CompareTo(parentItem) > 0)
            {
                // Swap the items positions
                Swap(item, parentItem);

                // Update the parent index w/ the swapped position
                parentIndex = (item.HeapIndex - 1) / 2;
            }
            // If the item precedes the parent, leave the item where it is & exit the loop
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Swap two item positions with one another in the heap.
    /// </summary>
    /// <param name="itemA">The first item to swap.</param>
    /// <param name="itemB">The second item to swap.</param>
    void Swap(T itemA, T itemB)
    {
        // Swap the positions in the heap
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        // Swap the index values for the heap indices using a tuple
        (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
    }
    #endregion
}