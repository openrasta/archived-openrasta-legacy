using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRasta.Collections.Specialized
{
    public class DependencyTree<T>
    {
        bool _isNormalized;

        public DependencyTree(T rootNode)
        {
            Nodes = new List<DependencyNode<T>>();
            RootNode = new DependencyNode<T>(rootNode);
            Nodes.Add(RootNode);
        }

        public ICollection<DependencyNode<T>> Nodes { get; private set; }

        public DependencyNode<T> RootNode { get; private set; }

        public DependencyNode<T> CreateNode(T value)
        {
            _isNormalized = false;
            var newNode = new DependencyNode<T>(value);
            Nodes.Add(newNode);
            return newNode;
        }

        public IEnumerable<DependencyNode<T>> GetCallGraph()
        {
            var list = new List<DependencyNode<T>>();
            NormalizeVertices();
            RootNode.QueueNodes(list);
            return list;
        }

        public void NormalizeVertices()
        {
            if (!_isNormalized)
            {
                foreach (var node in Nodes)
                {
                    foreach (var parent in node.ParentNodes)
                        if (!parent.ChildNodes.Contains(node))
                            parent.ChildNodes.Add(node);
                    foreach (var child in node.ChildNodes)
                        if (!child.ParentNodes.Contains(node))
                            child.ParentNodes.Add(node);
                }
                VerifyNoCyclicDependency();
                _isNormalized = true;
            }
        }

        void VerifyNoCyclicDependency()
        {
            if (Nodes.Any(x => x.HasRecursiveNodes()))
                throw new InvalidOperationException();
        }
    }
}