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
    /// Shuffle the elements of the <paramref name="source"/> <see cref="IEnumerable{T}"/> using a new instance of <see cref="Random"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <returns>>The shuffled <see cref="IEnumerable{T}"/>.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new Random());
    }

    /// <summary>
    /// Shuffle the elements of the <paramref name="source"/> <see cref="IEnumerable{T}"/> using the provided <see cref="Random"/> instance.
    /// </summary>
    /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
    /// <param name="rng">The <see cref="Random"/> instance to use for shuffling.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <returns>>The shuffled <see cref="IEnumerable{T}"/>.</returns>
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
