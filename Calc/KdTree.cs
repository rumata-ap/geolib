using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Calc
{
   public class KdTree
   {
      private KdNode root;
      private int size;

      private static readonly RectHV Container = new RectHV(0, 0, 1, 1);

      public bool IsEmpty => Size == 0;
      public int Size => size;

      public KdTree()
      {
         root = null;
         size = 0;
      }

      public bool Contains(IXYZ p)
      {
         return Contains(root, p.X, p.Y);
      }

      private bool Contains(KdNode node, double x, double y)
      {
         if (node == null)
         {
            return false;
         }
         if (node.X.Equals(x) && node.Y.Equals(y))
         {
            return true;
         }
         if (node.Vertical && x < node.X || !node.Vertical && y < node.Y)
         {
            return Contains(node.Left, x, y);
         }
         return Contains(node.Right, x, y);
      }

      public void Insert(IXYZ point)
      {
         root = Insert(root, point, vertical: true);
      }

      public void Insert(IEnumerable<IXYZ> points)
      {
         List<IXYZ> pts = points.ToList();
         pts.ForEach(Insert);
      }

      private KdNode Insert(KdNode node, IXYZ p, bool vertical)
      {
         if (node == null)
         {
            size++;
            return new KdNode(p.X, p.Y, vertical);
         }
         if (node.X.Equals(p.X) && node.Y.Equals(p.Y))
         {
            return node;
         }
         if (node.Vertical && p.X < node.X || !node.Vertical && p.Y < node.Y)
         {
            node.Left = Insert(node.Left, p, !node.Vertical);
         }
         else
         {
            node.Right = Insert(node.Right, p, !node.Vertical);
         }
         return node;
      }

      public IEnumerable<IXYZ> Range(RectHV rect)
      {
         var queue = new Queue<IXYZ>();
         Range(root, Container, rect, queue);
         return queue;
      }

      private void Range(KdNode node, RectHV nrect, RectHV rect, Queue<IXYZ> queue)
      {
         if (node == null)
         {
            return;
         }
         if (rect.Intersects(nrect))
         {
            var p = new Point2d(node.X, node.Y);
            if (rect.Contains(p))
            {
               queue.Enqueue(p);
            }
            Range(node.Left, LeftRect(nrect, node), rect, queue);
            Range(node.Right, RightRect(nrect, node), rect, queue);
         }
      }

      private static RectHV LeftRect(RectHV rect, KdNode node)
      {
         if (node.Vertical)
         {
            return new RectHV(rect.Xmin, rect.Ymin, node.X, rect.Ymax);
         }
         return new RectHV(rect.Xmin, rect.Ymin, rect.Xmax, node.Y);
      }

      private static RectHV RightRect(RectHV rect, KdNode node)
      {
         if (node.Vertical)
         {
            return new RectHV(node.X, rect.Ymin, rect.Xmax, rect.Ymax);
         }
         return new RectHV(rect.Xmin, node.Y, rect.Xmax, rect.Ymax);
      }

      public IXYZ Nearest(IXYZ p)
      {
         return Nearest(root, Container, p.X, p.Y, null);
      }

      private IXYZ Nearest(KdNode node, RectHV rect, double x, double y, IXYZ candidate)
      {
         if (node == null)
         {
            return candidate;
         }
         double dqn = 0;
         double drq = 0;
         var query = new Point2d(x, y);
         var nearest = candidate;

         if (nearest != null)
         {
            dqn = query.DistanceSquaredTo(nearest);
            drq = rect.DistanceSquaredTo(query);
         }

         if (nearest == null || dqn < drq)
         {
            var point = new Point2d(node.X, node.Y);
            if (nearest == null || dqn > query.DistanceSquaredTo(point))
            {
               nearest = point;
            }
         }

         var left = LeftRect(rect, node);
         var right = RightRect(rect, node);

         if (node.Vertical)
         {
            if (x < node.X)
            {
               nearest = Nearest(node.Left, left, x, y, nearest);
               nearest = Nearest(node.Right, right, x, y, nearest);
            }
            else
            {
               nearest = Nearest(node.Right, right, x, y, nearest);
               nearest = Nearest(node.Left, left, x, y, nearest);
            }
         }
         else
         {
            if (y < node.Y)
            {
               nearest = Nearest(node.Left, left, x, y, nearest);
               nearest = Nearest(node.Right, right, x, y, nearest);
            }
            else
            {
               nearest = Nearest(node.Right, right, x, y, nearest);
               nearest = Nearest(node.Left, left, x, y, nearest);
            }
         }

         return nearest;
      }

      private class KdNode
      {

         public double X { get; private set; }

         public double Y { get; private set; }

         public bool Vertical { get; private set; }

         public KdNode Left { get; set; }

         public KdNode Right { get; set; }

         public KdNode(double x, double y, bool vertical)
         {
            X = x;
            Y = y;
            Vertical = vertical;
         }

      }

   }

   public class RectHV
   {

      public double Xmin { get; private set; }
      public double Ymin { get; private set; }
      public double Xmax { get; private set; }
      public double Ymax { get; private set; }

      public RectHV(double xmin, double ymin, double xmax, double ymax)
      {
         Xmin = xmin;
         Ymin = ymin;
         Xmax = xmax;
         Ymax = ymax;
      }

      public bool Intersects(RectHV that)
      {
         return Xmax >= that.Xmin && Ymax >= that.Ymin
                && that.Xmax >= Xmin && that.Ymax >= Ymin;
      }

      public bool Contains(IXYZ p)
      {
         return (p.X >= Xmin && p.X <= Xmax)
                && (p.Y >= Ymin && p.Y <= Ymax);
      }

      public double DistanceSquaredTo(IXYZ p)
      {
         double dx = 0, dy = 0;
         if (p.X < Xmin)
         {
            dx = p.X - Xmin;
         }
         else if (p.X > Xmax)
         {
            dx = p.X - Xmax;
         }
         if (p.Y < Ymin)
         {
            dy = p.Y - Ymin;
         }
         else if (p.Y > Ymax)
         {
            dy = p.Y - Ymax;
         }
         return dx * dx + dy * dy;
      }

      public double DistanceTo(IXYZ p)
      {
         return Math.Sqrt(DistanceSquaredTo(p));
      }

      public override string ToString()
      {
         return string.Format("[{0}, {1}] x [{2}, {3}]", Xmin, Xmax, Ymin, Ymax);
      }
   }
}

