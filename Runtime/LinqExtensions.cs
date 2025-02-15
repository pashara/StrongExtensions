﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace StrongExtensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        // Return the first item when the list is of length one and otherwise returns default
        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            IEnumerable<TSource> array = source as TSource[] ?? source.ToArray();

            return array.Count() > 1
                ? default
                : array.FirstOrDefault();
        }

        // These are more efficient than Count() in cases where the size of the collection is not known
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int amount) =>
            enumerable.Take(amount).Count() == amount;

        public static bool HasMoreThan<T>(this IEnumerable<T> enumerable, int amount) =>
            enumerable.HasAtLeast(amount + 1);

        public static bool HasLessThan<T>(this IEnumerable<T> enumerable, int amount) =>
            enumerable.HasAtMost(amount - 1);

        public static bool HasAtMost<T>(this IEnumerable<T> enumerable, int amount) =>
            enumerable.Take(amount + 1).Count() <= amount;

        public static bool IsEmpty<T>([NoEnumeration]this IEnumerable<T> enumerable) =>
            !enumerable.Any();

        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> list) =>
            list.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T item) => 
            list.Except(item.Yield());

        // LINQ already has a method called "Contains" that does the same thing as this
        // BUT it fails to work with Mono 3.5 in some cases.
        // For example the following prints False, True in Mono 3.5 instead of True, True like it should:
        //
        // IEnumerable<string> args = new string[]
        // {
        //     "",
        //     null,
        // };

        // Log.Info(args.ContainsItem(null));
        // Log.Info(args.Where(x => x == null).Any());
        public static bool ContainsItem<T>(this IEnumerable<T> list, T value) =>
            // Use object.Equals to support null values
            list.Any(x => Equals(x, value));
    }
}