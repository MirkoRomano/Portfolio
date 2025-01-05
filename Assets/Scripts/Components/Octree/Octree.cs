using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Portfolio.Shared.Octree
{
    public interface ISpacialData
    {
        Vector3 GetLocation();
        Bounds GetBounds();
        float GetRadius();
    }

    public sealed class Node
    {
        public const int NODE_MAX_CHILDREN = 8;

        public static readonly Vector3[] PositionsNormalized = new Vector3[]
        {
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, 1, -1),
        };

        Bounds bounds;
        int depth = -1;

        private Node[] children = null;
        private readonly HashSet<ISpacialData> spatialData = new HashSet<ISpacialData>();

        public Node(Bounds bounds, int depth = 0)
        {
            this.bounds = bounds;
            this.depth = depth;
        }


        private void SplitInternal(Octree owner, ISpacialData spacialData)
        {
            Vector3 size = bounds.extents;
            Vector3 offset = size / 2f;

            children = new Node[NODE_MAX_CHILDREN];
            for (int i = 0; i < NODE_MAX_CHILDREN; i++) 
            {
                children[i] = new Node(new Bounds(bounds.center + Vector3.Scale(offset, PositionsNormalized[i]), size), 
                                       depth + 1);
            }

            foreach (var data in spatialData)
            {
                AddInternal(owner, spacialData);
            }

            spatialData.Clear();
        }

        private void AddInternal(Octree owner, ISpacialData spacialData)
        {
            foreach (var child in children) 
            {
                if (child.Overlaps(spacialData.GetBounds()))
                {
                    child.Add(owner, spacialData);
                    return;
                }
            }
        }

        private void SearchInternal(Bounds bounds, HashSet<ISpacialData> spatialData)
        {
            if(children == null)
            {
                if(this.spatialData.Count == 0)
                {
                    return;
                }

                spatialData.UnionWith(this.spatialData);
                return;
            }

            foreach(var child in children)
            {
                if (child.Overlaps(bounds))
                {
                    child.SearchInternal(bounds, spatialData);
                }
            }
        }

        public void Add(Octree owner, ISpacialData spacialData)
        {
            if (this.spatialData.Count + 1 > owner.MaxDataPerNode && CanSplit(owner))
            {
                SplitInternal(owner, spacialData);
                Add(owner, spacialData);
            }
            else
            {
                this.spatialData.Add(spacialData);
            }
        }

        public HashSet<ISpacialData> Search(Vector3 location, float radius)
        {
            if(depth != 0)
            {
                throw new System.InvalidOperationException("Cannot search elements in a empty tree");
            }

            return null;

        }

        public bool Overlaps(Bounds other)
        {
            return bounds.Intersects(other);
        }

        public bool CanSplit(Octree owner)
        {
            return bounds.size.x >= owner.MinimumNodeSize &&
                   bounds.size.y >= owner.MinimumNodeSize &&
                   bounds.size.z >= owner.MinimumNodeSize;
        }
    }

    public sealed class Octree
    {
        public float MinimumNodeSize;
        public float MaxDataPerNode;

        private Node root;

        public void Prepare(Bounds bounds)
        {
            root = new Node(bounds);
        }

        public void Add(ISpacialData data)
        {
            root.Add(this, data);
        }

        public void AddRange(List<ISpacialData> datas)
        {
            if (datas == null)
            {
                throw new System.ArgumentNullException("Cannot add a null collection");
            }

            if (datas.Count == 0)
            {
                throw new System.ArgumentException("Cannot add an empty collection");
            }

            for (int i = 0; i < datas.Count; i++)
            {
                root.Add(this, datas[i]);
            }
        }

        public HashSet<ISpacialData> Search(Vector3 location, float radius)
        {
            return root.Search(location, radius);
        }

    }
}