﻿using System.Collections.Generic;

namespace Geo
{
   public class Pline2d
   {
      protected List<Vertex2d> vertxs;
      protected List<Line2d> segs;
      protected BoundingBox2d bb;

      public List<Vertex2d> Vertexs { get => vertxs; internal set { vertxs = value; CalcBB(); CalcSegs(); } }
      public BoundingBox2d BoundingBox { get => bb; }
      public List<Line2d> Segments { get => segs; }

      public Pline2d()
      {
         vertxs = new List<Vertex2d>();
         segs = new List<Line2d>();
      }

      public Pline2d(List<Point2d> points)
      {
         segs = new List<Line2d>();
         vertxs = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point2d item in points)
            {
               vertxs.Add(new Vertex2d(item.ToPoint3d()));
            }
         }
         CalcBB();
         CalcSegs();
      }

      public Pline2d(List<Point3d> points)
      {
         segs = new List<Line2d>();
         vertxs = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point3d item in points)
            {
               vertxs.Add(new Vertex2d(item));
            }
         }
         CalcBB();
         CalcSegs();
      }

      protected virtual void AddPoint(Point2d pt)
      {
         vertxs.Add(new Vertex2d(pt.ToPoint3d()));
         CalcBB();
         CalcSegs();
      }

      public virtual void AddPoint(Point3d pt)
      {
         vertxs.Add(new Vertex2d(pt));
         CalcBB();
         CalcSegs();
      }

      public virtual void AddVertex(Vertex2d pt)
      {
         vertxs.Add(pt);
         CalcBB();
         CalcSegs();
      }

      protected void CalcBB()
      {
         double minX = 1000000000;
         double minY = 1000000000;
         double maxX = -1000000000;
         double maxY = -1000000000;

         if (vertxs.Count > 0)
         {
            foreach (ICoordinates item in vertxs)
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
         if (vertxs.Count > 1)
         {
            segs = new List<Line2d>();

            for (int i = 1; i < vertxs.Count; i++)
            {
               segs.Add(new Line2d(vertxs[i - 1], vertxs[i]));
            }

            if (vertxs.Count > 2) { segs.Add(new Line2d(vertxs[vertxs.Count - 1], vertxs[0])); }
         }
         CalcVertexs();
      }

      void CalcVertexs()
      {
         List<Vertex2d> res = new List<Vertex2d>(vertxs);
         for (int i = 1; i < vertxs.Count-1; i++)
         {
            vertxs[i].Id = i + 1;
            vertxs[i].Prev = vertxs[i-1];
            vertxs[i].Next = vertxs[i+1];
         }
         vertxs[0].Id = 1;
         vertxs[0].Pos = VertexPosition.First;
         vertxs[0].Next = vertxs[1];
         vertxs[0].Prev = null;
         vertxs[vertxs.Count - 1].Id = vertxs.Count;
         vertxs[vertxs.Count - 1].Pos = VertexPosition.Last;
         vertxs[vertxs.Count - 1].Prev = vertxs[vertxs.Count - 2];
         vertxs[vertxs.Count - 1].Next = null;
      }

      /// <summary>
      ///Смена направления полилинии.
      /// </summary>
      public void Inverse()
      {
         vertxs.Reverse();
         CalcSegs();
      }

   }
}
