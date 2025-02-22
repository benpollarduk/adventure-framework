﻿using System.Linq;

namespace NetAF.Extensions
{
    /// <summary>
    /// Provides extension functions for arrays.
    /// </summary>
    internal static class ArrayExtensions
    {
        /// <summary>
        /// Add an element to an array.
        /// </summary>
        /// <typeparam name="T">The type of element.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="element">The element to add.</param>
        /// <returns>The original array with the element appended.</returns>
        internal static T[] Add<T>(this T[] value, T element)
        {
            var list = value?.ToList() ?? [];
            list.Add(element);
            return [.. list];
        }

        /// <summary>
        /// Remove an element from an array.
        /// </summary>
        /// <typeparam name="T">The type of element.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="element">The element to remove.</param>
        /// <returns>The original array with the element removed.</returns>
        internal static T[] Remove<T>(this T[] value, T element)
        {
            var list = value?.ToList() ?? [];
            list.Remove(element);
            return [.. list];
        }
    }
}
