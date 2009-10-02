using System;
using System.Collections.Generic;

namespace OpenRasta.Collections
{
    public static class ObservableIteratorExtensions
    {
        /// <summary>
        /// Filters an enumerable and notify when elements are selected or discarded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="filter"></param>
        /// <param name="onSelected"></param>
        /// <param name="onDiscarded"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsObservable<T>(this IEnumerable<T> target, 
                                                     Func<IEnumerable<T>, IEnumerable<T>> filter, 
                                                     Action<T> onSelected, 
                                                     Action<T> onDiscarded)
        {
            return new ObservableIterator<T>(target, filter, onSelected, onDiscarded);
        }
    }
}