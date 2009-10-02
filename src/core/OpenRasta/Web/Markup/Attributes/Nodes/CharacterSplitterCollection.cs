using System;
using System.Collections.ObjectModel;
using OpenRasta.Collections;

namespace OpenRasta.Web.Markup.Attributes
{
    public class CharacterSplitterCollection : Collection<string>
    {
        string _separator;

        public CharacterSplitterCollection(string separator)
        {
            _separator = separator;
        }

        protected override void InsertItem(int index, string item)
        {
            foreach(var i in  item.Split(new[] { _separator }, StringSplitOptions.RemoveEmptyEntries))
                base.InsertItem(index, i);
        }

        protected override void SetItem(int index, string item)
        {
            foreach(var i in item.Split(new[] { _separator }, StringSplitOptions.RemoveEmptyEntries))
                base.SetItem(index, i);
        }
    }
}