#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.ObjectModel;

namespace OpenRasta.Collections
{
    public class NotifyCollection<T> : Collection<T>
    {
        public EventHandler<CollectionChangedEventArgs<T>> ItemAdded = (src, e) => { };
        public EventHandler<CollectionChangedEventArgs<T>> ItemRemoved = (src, e) => { };

        protected override void ClearItems()
        {
            this.ForEach(item => ItemRemoved(this, new CollectionChangedEventArgs<T>(item)));
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            OnItemInsert(index, item);
            ItemAdded(this, new CollectionChangedEventArgs<T>(item));
        }

        protected virtual void OnItemAdd(int index, T item)
        {
            base.SetItem(index, item);
        }

        protected void OnItemInsert(int index, T item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            OnItemRemoved(index);
            ItemRemoved(this, new CollectionChangedEventArgs<T>(this[index]));
        }

        protected override void SetItem(int index, T item)
        {
            if (!ReferenceEquals(this[index],null))
                ItemRemoved(this, new CollectionChangedEventArgs<T>(this[index]));
            OnItemAdd(index, item);
            ItemAdded(this, new CollectionChangedEventArgs<T>(item));
        }

        void OnItemRemoved(int index)
        {
            base.RemoveItem(index);
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion