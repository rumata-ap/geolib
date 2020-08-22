using Geo.Calc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo
{
   public class Triangle
   {
      private ICoordinates vertex1;
      private ICoordinates vertex2;
      private ICoordinates vertex3;

      public double Area { get; private set; }
      public double Xc { get; private set; }
      public double Yc { get; private set; }
      //public double MaxAngleDeg { get; private set; }
      public ICoordinates Vertex1 { get => vertex1; set { vertex1 = value; CalcTriangle(); } }
      public ICoordinates Vertex2 { get => vertex2; set { vertex2 = value; CalcTriangle(); } }
      public ICoordinates Vertex3 { get => vertex3; set { vertex3 = value; CalcTriangle(); } }

      public Triangle(ICoordinates node1, ICoordinates node2, ICoordinates node3)
      {
         vertex1 = node1;
         vertex2 = node2;
         vertex3 = node3;
         CalcTriangle();
      }

      public void CalcTriangle()
      {
         //double ang1, ang2, ang3;
         //ang1 = vertex1.AngleTo(vertex3, vertex2);
         //ang2 = vertex2.AngleTo(vertex1, vertex3);
         //ang3 = vertex3.AngleTo(vertex2, vertex1);
         //MaxAngleDeg = Math.Max(Math.Max(ang1, ang2), ang3);

         Xc = (vertex1.X + vertex2.X + vertex3.X) / 3;
         Yc = (vertex1.Y + vertex2.Y + vertex3.Y) / 3;
         Area = Math.Abs(0.5 * ((vertex2.X - vertex1.X) * (vertex3.Y - vertex1.Y) -
            (vertex3.X - vertex1.X) * (vertex2.Y - vertex1.Y)));
      }

      public bool IsPointIn(ICoordinates pt)
      {
         if (pt.IsMatch(vertex1) || pt.IsMatch(vertex2) || pt.IsMatch(vertex3)) return true;

         double bx = vertex2.X - vertex1.X;
         double by = vertex2.Y - vertex1.Y;
         double cx = vertex3.X - vertex1.X;
         double cy = vertex3.Y - vertex1.Y;
         double px = pt.X - vertex1.X;
         double py = pt.Y - vertex1.Y;

         double m = (px * by - bx * py) / (cx * by - bx * cy);
         if (m >= 0 && m <= 1)
         {
            double l = (px - m * cx) / bx;
            if (l > 0 && m + l <= 1) return true;
            else return false;
         }
         else return false;
      }

      public bool Intersect(ICurve2d l, out ICoordinates[] pti)
      {
         bool res = false;
         pti = new ICoordinates[] { null, null, null };
         Line2d line = (Line2d)l;
         Line2d edg1 = new Line2d(new Point2d(vertex1.ToArray()), new Point2d(vertex2.ToArray()));
         Line2d edg2 = new Line2d(new Point2d(vertex2.ToArray()), new Point2d(vertex3.ToArray()));
         Line2d edg3 = new Line2d(new Point2d(vertex3.ToArray()), new Point2d(vertex1.ToArray()));

         line.Intersection(edg1, out IntersectResult res1);
         line.Intersection(edg2, out IntersectResult res2);
         line.Intersection(edg3, out IntersectResult res3);

         Range Xrange1 = new Range(vertex1.X, vertex2.X);
         Range Yrange1 = new Range(vertex1.Y, vertex2.Y);
         Range Xrange2 = new Range(vertex2.X, vertex3.X);
         Range Yrange2 = new Range(vertex2.Y, vertex3.Y);
         Range Xrange3 = new Range(vertex3.X, vertex1.X);
         Range Yrange3 = new Range(vertex3.Y, vertex1.Y);
         Range Xrangel = new Range(line.StartPoint.X, line.EndPoint.X);
         Range Yrangel = new Range(line.StartPoint.Y, line.EndPoint.Y);

         if (!res1.res && !res2.res && !res3.res ) return res;
         
         if (res1.res)
         {
            Point2d respt1 = res1.pts[0];
            if (Xrange1.InNoBound(respt1.X) && Yrange1.InNoBound(respt1.Y) && Xrangel.InNoBound(respt1.X) && Yrangel.InNoBound(respt1.Y))
            {
               pti[0] = respt1;
            }
         }
         if (res2.res)
         {
            Point2d respt2 = res2.pts[0];
            if (Xrange2.InNoBound(respt2.X) && Yrange2.InNoBound(respt2.Y) && Xrangel.InNoBound(respt2.X) && Yrangel.InNoBound(respt2.Y))
            {
               pti[1] = respt2;
            }
         }
         if(res3.res)
         {
            Point2d respt3 = res3.pts[0];
            if (Xrange3.InNoBound(respt3.X) && Yrange3.InNoBound(respt3.Y) && Xrangel.InNoBound(respt3.X) && Yrangel.InNoBound(respt3.Y))
            {
               pti[2] = respt3;
            }                       
         }

         if (pti[0] != null || pti[1] != null || pti[2] != null) res = true;

         return res;
      }
   }
}
