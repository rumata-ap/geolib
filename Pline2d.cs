using System.Collections.Generic;

namespace Geo
{
   public class Pline2d
   {
      protected List<Vertex2d> pts;
      protected List<Line2d> segs;
      protected BoundingBox2d bb;

      public List<Vertex2d> Vertexs { get => pts; internal set { pts = value; CalcBB(); CalcSegs(); } }
      public BoundingBox2d BoundingBox { get => bb; }
      public List<Line2d> Segments { get => segs; }

      public Pline2d()
      {
         pts = new List<Vertex2d>();
         segs = new List<Line2d>();
      }

      public Pline2d(List<Point2d> points)
      {
         segs = new List<Line2d>();
         pts = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point2d item in points)
            {
               pts.Add(new Vertex2d(item.ToPoint3d()));
            }
         }
         CalcBB();
         CalcSegs();
      }

      public Pline2d(List<Point3d> points)
      {
         segs = new List<Line2d>();
         pts = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point3d item in points)
            {
               pts.Add(new Vertex2d(item));
            }
         }
         CalcBB();
         CalcSegs();
      }

      protected virtual void AddPoint(Point2d pt)
      {
         pts.Add(new Vertex2d(pt.ToPoint3d()));
         CalcBB();
         CalcSegs();
      }

      public virtual void AddPoint(Point3d pt)
      {
         pts.Add(new Vertex2d(pt));
         CalcBB();
         CalcSegs();
      }

      public virtual void AddVertex(Vertex2d pt)
      {
         pts.Add(pt);
         CalcBB();
         CalcSegs();
      }

      protected void CalcBB()
      {
         double minX = 1000000000;
         double minY = 1000000000;
         double maxX = -1000000000;
         double maxY = -1000000000;

         if (pts.Count > 0)
         {
            foreach (ICoordinates item in pts)
            {
               if (item.X < minX) { minX = item.X; }
               if (item.Y < minY) { minY = item.Y; }
               if (item.X > maxX) { maxX = item.X; }
               if (item.Y > maxY) { maxY = item.Y; }
            }

            bb = new BoundingBox2d(new Point2d(minX, minY), new Point2d(maxX, maxY));
         }
      }

      protected void CalcSegs()
      {
         if (pts.Count > 1)
         {
            segs = new List<Line2d>();

            for (int i = 1; i < pts.Count; i++)
            {
               segs.Add(new Line2d(pts[i - 1], pts[i]));
            }

            if (pts.Count > 2) { segs.Add(new Line2d(pts[pts.Count - 1], pts[0])); }
         }
         CalcVertexs();
      }

      void CalcVertexs()
      {
         List<Vertex2d> res = new List<Vertex2d>(pts);
         for (int i = 1; i < pts.Count-1; i++)
         {
            pts[i].Prev = pts[i-1];
            pts[i].Next = pts[i+1];
         }
         pts[0].Pos = VertexPosition.First;
         pts[0].Next = pts[1];
         pts[pts.Count - 1].Pos = VertexPosition.Last;
         pts[pts.Count - 1].Prev = pts[pts.Count - 2];
      }

   }
}
