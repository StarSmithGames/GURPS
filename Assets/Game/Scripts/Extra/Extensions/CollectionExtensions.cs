using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public static class CollectionExtensions
{
    public static T RandomItem<T>(this IList<T> list, int from = 0, int to = -1)
	{
        if (list.Count == 0) return default;
        return list[UnityEngine.Random.Range(from, to == -1 ? list.Count : to)];
	}

    /// <summary>
    /// https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </summary>
    public static void Shuffle <T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


	/// <summary>
	/// Resize for listB by listA
	/// </summary>
	public static void Resize<FIRST, SECOND>(IList<FIRST> listA, IList<SECOND> listB, Func<SECOND> onCreate, Func<SECOND> onRemove)
	{
		Resize(listA.Count, listB, onCreate, onRemove);
	}

	/// <summary>
	/// Resize list by size
	/// </summary>
	public static void Resize<T>(int size, IList<T> list, Func<T> onCreate, Func<T> onRemove)
	{
		int diff = size - list.Count;

		if (diff != 0)
		{
			if (diff > 0)//add
			{
				for (int i = 0; i < diff; i++)
				{
					Add();
				}
			}
			else//rm
			{
				for (int i = 0; i < -diff; i++)
				{
					Remove();
				}
			}
		}

		void Add()
		{
			if (onCreate != null)
			{
				T item = onCreate.Invoke();
				list.Add(item);
			}
		}

		void Remove()
		{
			if (onRemove != null)
			{
				T item = onRemove.Invoke();
				list.Remove(item);
			}
		}
	}



	/// <summary>
	/// https://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
	/// </summary>
	//public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
	//{
	//    T[] elements = source.ToArray();
	//    for (int i = elements.Length - 1; i >= 0; i--)
	//    {
	//        // Swap element "i" with a random earlier element it (or itself)
	//        // ... except we don't really need to swap it fully, as we can
	//        // return it immediately, and afterwards it's irrelevant.
	//        int swapIndex = rng.Next(i + 1);
	//        yield return elements[swapIndex];
	//        elements[swapIndex] = elements[i];
	//    }
	//}
}