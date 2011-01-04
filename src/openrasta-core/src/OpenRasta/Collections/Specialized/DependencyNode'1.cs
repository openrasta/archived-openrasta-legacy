using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRasta.Collections.Specialized
{
    public class DependencyNode<T>
    {
        public DependencyNode(T value)
        {
            Value = value;
            ParentNodes = new List<DependencyNode<T>>();
            ChildNodes = new List<DependencyNode<T>>();
        }

        public ICollection<DependencyNode<T>> ChildNodes { get; private set; }

        public int OutgoingWeight
        {
            get
            {
                if (HasRecursiveNodes())
                    return -1;
                return ChildNodes.Aggregate(ChildNodes.Count, (count, node) => count + node.OutgoingWeight);
            }
        }

        public ICollection<DependencyNode<T>> ParentNodes { get; private set; }
        public T Value { get; set; }
        protected bool Visited { get; set; }

        public bool HasRecursiveNodes()
        {
            return HasRecursiveNodes(new Stack<DependencyNode<T>>());
        }

        public void QueueNodes(ICollection<DependencyNode<T>> nodes)
        {
            Visited = true;
            foreach (var parentNode in ParentNodes.OrderBy(x => x.OutgoingWeight))
            {
                if (parentNode.Visited)
                    continue;
                parentNode.QueueNodes(nodes);
            }
            nodes.Add(this);
            foreach (var childNode in ChildNodes.OrderBy(x => x.OutgoingWeight))
            {
                if (childNode.Visited) continue;

                childNode.QueueNodes(nodes);
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        bool HasRecursiveNodes(Stack<DependencyNode<T>> recursionDefender)
        {
            if (recursionDefender.Contains(this))
                throw new RecursionException();
            recursionDefender.Push(this);
            try
            {
                foreach (var child in ChildNodes)
                {
                    if (child.HasRecursiveNodes(recursionDefender))
                        return true;
                }
            }
            finally
            {
                recursionDefender.Pop();
            }
            return false;
        }
    }
}