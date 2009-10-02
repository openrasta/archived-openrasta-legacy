using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Collections;

namespace OpenRasta
{
    public static class FuncExtensions
    {
        public static Func<T,T> Chain<T>(this IEnumerable<Func<T,T>> functions)
        {
            Func<T, T> root = null;
            foreach (var func in functions)
            {
                if (root == null)
                    root = func;
                else
                {
                    var current = root;
                    var next = func;
                    root = x => next(current(x));
                }
            }
            return root;
        }
    }
}