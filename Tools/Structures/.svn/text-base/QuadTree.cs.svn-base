using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using ScanMatchers.Math;

namespace ScanMatchers.Tools.Structures
{
    public class QuadTree<T> where T: Vector2
    {
        // Fields
        private QuadRootNode _RootNode;
        protected static readonly int MaxNumPoints = 5;

        // Methods
        public QuadTree()
        {
            this._RootNode = new QuadRootNode();
        }

        public T FindNearestNeighbour(Vector2 target, float maxDistance)
        {
            lock (this)
            {
                PointF point = new PointF((float)target.X, (float)target.Y);
                float radius = maxDistance * 2f;
                RectangleF window = new RectangleF(point.X - maxDistance, point.Y - maxDistance, radius, radius);
                T source = default(T);
                T nearest = source;
                double dist = maxDistance;
                if (this._RootNode.FindNearestNeighbour(point, window, ref nearest, ref dist))
                {
                    if (dist < maxDistance)
                        return nearest;
                }
                return default(T);
            }
        }

        public void Insert(T point)
        {
            lock (this)
            {
                this._RootNode.Insert(point);
            }
        }

        public void Remove(T point)
        {
            lock (this)
            {
            }
        }

        protected abstract class QuadNode
        {
            // Fields
            protected internal RectangleF Bounds;
            protected internal PointF Center;
            protected static readonly int NE;
            protected static readonly int NW;
            protected internal QuadTree<T>.QuadInternalNode Parent;
            protected static readonly int SE;
            protected static readonly int SW;

            // Methods
            static QuadNode()
            {
                QuadTree<T>.QuadNode.NW = 0;
                QuadTree<T>.QuadNode.SW = 1;
                QuadTree<T>.QuadNode.NE = 2;
                QuadTree<T>.QuadNode.SE = 3;
            }

            public QuadNode(QuadTree<T>.QuadInternalNode parent, RectangleF bounds)
            {
                this.Parent = parent;
                this.Bounds = bounds;
                this.Center = new PointF(this.Bounds.Left + (this.Bounds.Width / 2f), this.Bounds.Top + (this.Bounds.Height / 2f));
            }

            protected internal abstract void FindNearestNeighbour(PointF target, ref RectangleF window, ref T nearest, ref double sqdist);
            protected internal abstract QuadTree<T>.QuadNode FindSmallestContainingNode(RectangleF rect);
            protected internal abstract QuadTree<T>.QuadNode Insert(T point);
            protected internal QuadTree<T>.QuadNode Insert(T[] points)
            {
                QuadTree<T>.QuadNode node = (QuadTree<T>.QuadNode) this;
                foreach (T point in points)
                {
                    node = node.Insert(point);
                }
                return node;
            }
        }

        protected class QuadLeafNode : QuadTree<T>.QuadNode
        {
            // Fields
            private List<T> _Points;

            // Methods
            public QuadLeafNode(QuadTree<T>.QuadInternalNode parent, RectangleF bounds) : base(parent, bounds)
            {
                this._Points = new List<T>();
            }

            protected internal override void FindNearestNeighbour(PointF target, ref RectangleF window, ref T nearest, ref double sqdist)
            {
                bool foundOne = false;
                foreach (T point in this._Points)
                {
                    double dist = System.Math.Pow(point.X - target.X, 2.0) + System.Math.Pow(point.Y - target.Y, 2.0);
                    if (dist < sqdist)
                    {
                        sqdist = dist;
                        nearest = point;
                        foundOne = true;
                    }
                }
                if (foundOne)
                {
                    double dist = System.Math.Pow(sqdist, 0.5);
                    window = new RectangleF(target.X - ((float) dist), target.Y - ((float) dist), (float) (2.0 * dist), (float) (2.0 * dist));
                }
            }

            protected internal override QuadTree<T>.QuadNode FindSmallestContainingNode(RectangleF rect)
            {
                return this;
            }

            protected internal override QuadTree<T>.QuadNode Insert(T point)
            {
                if (!this.IsFull())
                {
                    this._Points.Add(point);
                    return this;
                }
                if (this.Bounds.Width > 1f)
                {
                    QuadTree<T>.QuadNode node = new QuadTree<T>.QuadInternalNode(base.Parent, base.Bounds);
                    return node.Insert(this._Points.ToArray()).Insert(point);
                }
                return this;
            }

            protected bool IsFull()
            {
                return (this._Points.Count >= QuadTree<T>.MaxNumPoints);
            }
        }

        protected class QuadInternalNode : QuadTree<T>.QuadNode
        {
            // Fields
            protected QuadTree<T>.QuadNode[] _Children;

            // Methods
            public QuadInternalNode(QuadTree<T>.QuadInternalNode parent, RectangleF bounds) : base(parent, bounds)
            {
                this._Children = new QuadTree<T>.QuadNode[4];
                
                float width = this.Bounds.Width / 2f;
                float height = this.Bounds.Height / 2f;
                RectangleF bound = new RectangleF(this.Bounds.Left, this.Bounds.Top, width, height);

                this._Children[QuadTree<T>.QuadNode.NW] = new QuadTree<T>.QuadLeafNode((QuadTree<T>.QuadInternalNode) this, bound);
                bound = new RectangleF(this.Bounds.Left, this.Center.Y, width, height);
                this._Children[QuadTree<T>.QuadNode.SW] = new QuadTree<T>.QuadLeafNode((QuadTree<T>.QuadInternalNode) this, bound);
                bound = new RectangleF(this.Center.X, this.Bounds.Top, width, height);
                this._Children[QuadTree<T>.QuadNode.NE] = new QuadTree<T>.QuadLeafNode((QuadTree<T>.QuadInternalNode) this, bound);
                bound = new RectangleF(this.Center.X, this.Center.Y, width, height);
                this._Children[QuadTree<T>.QuadNode.SE] = new QuadTree<T>.QuadLeafNode((QuadTree<T>.QuadInternalNode) this, bound);
            }

            internal QuadInternalNode(QuadTree<T>.QuadInternalNode parent, RectangleF bounds, QuadTree<T>.QuadNode[] children) : base(parent, bounds)
            {
                this._Children = new QuadTree<T>.QuadNode[4];
                int index = 0;
                foreach (QuadTree<T>.QuadNode child in children)
                {
                    this._Children[index] = child;
                    child.Parent = (QuadTree<T>.QuadInternalNode) this;
                    index++;
                }
            }

            protected internal override void FindNearestNeighbour(PointF target, ref RectangleF window, ref T nearest, ref double sqdist)
            {
                int index = this.GetIndexOfNearestChildNode((double) target.X, (double) target.Y);
                QuadTree<T>.QuadNode child = this._Children[index];
                child.FindNearestNeighbour(target, ref window, ref nearest, ref sqdist);
                if (!child.Bounds.Contains(window))
                {
                    int Len = this._Children.Length - 1;
                    for (int i = 0; i <= Len; i++)
                    {
                        if (i != index)
                        {
                            child = this._Children[i];
                            if (child.Bounds.IntersectsWith(window))
                            {
                                child.FindNearestNeighbour(target, ref window, ref nearest, ref sqdist);
                            }
                        }
                    }
                }
            }

            protected internal override QuadTree<T>.QuadNode FindSmallestContainingNode(RectangleF rect)
            {
                int index = this.GetIndexOfNearestChildNode((double) rect.X, (double) rect.Y);
                QuadTree<T>.QuadNode child = this._Children[index];
                if (child.Bounds.Contains(rect))
                {
                    return child.FindSmallestContainingNode(rect);
                }
                return this;
            }

            protected int GetIndexOfNearestChildNode(double x, double y)
            {
                if (x < this.Center.X)
                {
                    if (y < this.Center.Y)
                    {
                        return QuadTree<T>.QuadNode.NW;
                    }
                    return QuadTree<T>.QuadNode.SW;
                }
                if (y < this.Center.Y)
                {
                    return QuadTree<T>.QuadNode.NE;
                }
                return QuadTree<T>.QuadNode.SE;
            }

            protected internal override QuadTree<T>.QuadNode Insert(T point)
            {
                int index = this.GetIndexOfNearestChildNode(point.X, point.Y);
                this._Children[index] = this._Children[index].Insert(point);
                return this;
            }
        }

        protected class QuadRootNode : QuadTree<T>.QuadInternalNode
        {
            // Methods
            public QuadRootNode() 
                : base(null, new RectangleF(-2000f, -2000f, 4000f, 4000f))
            {
            }

            protected internal bool FindNearestNeighbour(PointF target, RectangleF window, ref T nearest, ref double distance)
            {
                QuadTree<T>.QuadNode start = this.FindSmallestContainingNode(window);
                double sqdist = double.MaxValue;
                start.FindNearestNeighbour(target, ref window, ref nearest, ref sqdist);
                distance = System.Math.Pow(sqdist, 0.5);
                return ((T) nearest) != null;
            }

            protected internal override QuadTree<T>.QuadNode Insert(T point)
            {
                int newIndex;
                if (this.Bounds.Contains((float) point.X, (float) point.Y))
                {
                    return base.Insert(point);
                }
                QuadTree<T>.QuadInternalNode newNode = new QuadTree<T>.QuadInternalNode(this, base.Bounds, base._Children);
                PointF newOffset = new PointF(this.Bounds.X, this.Bounds.Y);
                SizeF newSize = new SizeF(this.Bounds.Width * 2f, this.Bounds.Height * 2f);
                if (point.X < this.Bounds.Left)
                {
                    if (point.Y < this.Bounds.Top)
                    {
                        newIndex = QuadTree<T>.QuadNode.SE;
                        newOffset.X -= this.Bounds.Width;
                        newOffset.Y -= this.Bounds.Height;
                    }
                    else
                    {
                        newIndex = QuadTree<T>.QuadNode.NE;
                        newOffset.X -= this.Bounds.Width;
                    }
                }
                else if (point.Y < this.Bounds.Top)
                {
                    newIndex = QuadTree<T>.QuadNode.SW;
                    newOffset.Y -= this.Bounds.Height;
                }
                else
                {
                    newIndex = QuadTree<T>.QuadNode.NW;
                }
                RectangleF newBounds = new RectangleF(newOffset, newSize);
                base.Bounds = newBounds;
                this.Center = new PointF(this.Bounds.Left + (this.Bounds.Width / 2f), this.Bounds.Top + (this.Bounds.Height / 2f));
                float width = this.Bounds.Width / 2f;
                float height = this.Bounds.Height / 2f;
                RectangleF bound = new RectangleF(this.Bounds.Left, this.Bounds.Top, width, height);
                base._Children[QuadTree<T>.QuadNode.NW] = new QuadTree<T>.QuadLeafNode(this, bound);
                bound = new RectangleF(this.Bounds.Left, this.Center.Y, width, height);
                base._Children[QuadTree<T>.QuadNode.SW] = new QuadTree<T>.QuadLeafNode(this, bound);
                bound = new RectangleF(this.Center.X, this.Bounds.Top, width, height);
                base._Children[QuadTree<T>.QuadNode.NE] = new QuadTree<T>.QuadLeafNode(this, bound);
                bound = new RectangleF(this.Center.X, this.Center.Y, width, height);
                base._Children[QuadTree<T>.QuadNode.SE] = new QuadTree<T>.QuadLeafNode(this, bound);
                base._Children[newIndex] = newNode;
                return this.Insert(point);
            }
        }

    }
}
