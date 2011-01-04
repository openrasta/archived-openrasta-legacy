using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    /// <summary>
    /// Please do not use, it's not ready for prime time yet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionIndexerSurrogate<T> : ISurrogate
    {
        ICollection<T> _surrogatedValue;

        int _lastSeenIndex = -1;

        object ISurrogate.Value
        {
            get { return _surrogatedValue; }
            set { _surrogatedValue = (ICollection<T>)value; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");

                if (index == _lastSeenIndex)
                {
                    return _surrogatedValue.Last();
                }

                _lastSeenIndex = index;
                _surrogatedValue.Add(default(T));
                return default(T);
            }

            set
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");
                if (index == _lastSeenIndex)
                {
                    var tempValues = _surrogatedValue.ToList();
                    for (int i = 0; i < tempValues.Count - 1; i++)
                        _surrogatedValue.Add(tempValues[i]);
                }

                _lastSeenIndex = index;
                _surrogatedValue.Add(value);
            }
        }
    }
}