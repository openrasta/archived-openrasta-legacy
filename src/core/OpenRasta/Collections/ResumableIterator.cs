using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenRasta.Collections
{
    public class ResumableIterator<T, TKey> : IEnumerator<T>, IEnumerable<T>
    {
        readonly Func<TKey, TKey, bool> _equalityProvider;
        readonly Func<T, TKey> _keyProvider;
        readonly IEnumerator<T> _sourceEnumerator;
        bool _isAhead;
        bool _isSuspended;
        TKey _suspendAfter;

        public ResumableIterator(IEnumerator<T> source, Func<T, TKey> keyProvider, Func<TKey, TKey, bool> equalityProvider)
        {
            _sourceEnumerator = source;
            _keyProvider = keyProvider;
            _equalityProvider = equalityProvider;
        }

        public T Current
        {
            get { return _sourceEnumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return _sourceEnumerator.Current; }
        }

        public bool ResumeFrom(TKey key)
        {
            if (ReferenceEquals(key, null)) throw new ArgumentNullException("key");
            if (CurrentKeyIs(key))
                return true;
            while (_sourceEnumerator.MoveNext())
            {
                if (CurrentKeyIs(key))
                {
                    _isAhead = true;
                    return true;
                }
            }
            return false;
        }

        public void SuspendAfter(TKey key)
        {
            if (ReferenceEquals(key, null)) throw new ArgumentNullException("key");
            _suspendAfter = key;
        }

        void IDisposable.Dispose()
        {
            _sourceEnumerator.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        bool IEnumerator.MoveNext()
        {
            if (_isSuspended)
            {
                _isSuspended = false;
                return false;
            }
            if (_isAhead)
            {
                _isAhead = false;
                return true;
            }
            if (_sourceEnumerator.MoveNext())
            {
                var newKey = _keyProvider(_sourceEnumerator.Current);

                if (!ReferenceEquals(_suspendAfter, null) && _equalityProvider(newKey, _suspendAfter))
                    _isSuspended = true;
                return true;
            }
            return false;
        }

        void IEnumerator.Reset()
        {
            // ignore the reset as we may continue iterating later on.
        }

        bool CurrentKeyIs(TKey key)
        {
            if (ReferenceEquals(Current, null)) return false;
            return _equalityProvider(_keyProvider(Current), key);
        }
    }
}