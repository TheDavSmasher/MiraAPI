using System;
using System.Collections.Generic;
using System.Linq;

namespace MiraAPI.Utilities;

/// <summary>
/// Extensions for IEnumerable.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Shuffle the elements of the source enumerable using a new instance of Random.
    /// </summary>
    /// <param name="source">The source enumerable.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <returns>>The shuffled enumerable.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new Random());
    }

    /// <summary>
    /// Shuffle the elements of the source enumerable using the provided Random instance.
    /// </summary>
    /// <param name="source">The source enumerable.</param>
    /// <param name="rng">The Random instance to use for shuffling.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <returns>>The shuffled enumerable.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(rng);

        return source.ShuffleIterator(rng);
    }

    private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng)
    {
        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}
