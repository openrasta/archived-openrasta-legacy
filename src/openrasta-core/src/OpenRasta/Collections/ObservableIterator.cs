using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenRasta.Collections
{
    /// <summary>
    /// Provides an iterator that can notify on elements being selected or discarded.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableIterator<T> : IEnumerable<T>
    {
        readonly IEqualityComparer<T> _equalityComparer = EqualityComparer<T>.Default;
        readonly Func<IEnumerable<T>, IEnumerable<T>> _filter;
        readonly Action<T> _onDiscarded;
        readonly Action<T> _onSelected;
        readonly IEnumerable<T> _target;
        T _currentInnerItem;
        T _currentOuterItem;

        public ObservableIterator(IEnumerable<T> target, Func<IEnumerable<T>, IEnumerable<T>> filter, Action<T> onSelected, Action<T> onDiscarded)
        {
            _target = target;
            _filter = filter;
            _onSelected = onSelected;
            _onDiscarded = onDiscarded;
        }

        protected bool EnumeratedElementsMatch
        {
            get { return _equalityComparer.Equals(_currentInnerItem, _currentOuterItem); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var filter = _filter(WrapperEnumerator(_target));
            foreach (var outerItem in filter)
            {
                _currentOuterItem = outerItem;
                if (_onSelected != null)
                    _onSelected(outerItem);
                yield return outerItem;
            }
        }

        void NotifyItemDiscardedIfNecessary()
        {
            if (!EnumeratedElementsMatch)
                if (_onDiscarded != null)
                    _onDiscarded(_currentInnerItem);
        }

        IEnumerable<T> WrapperEnumerator(IEnumerable<T> enumerable)
        {
            bool isFirst = true;
            foreach (var item in enumerable)
            {
                if (!isFirst)
                {
                    NotifyItemDiscardedIfNecessary();
                }
                else
                {
                    isFirst = false;
                }
                _currentInnerItem = item;
                yield return item;
            }
            NotifyItemDiscardedIfNecessary();
        }
    }
}