﻿using Geo.Triangulation;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Geo
{
   public class Polygon2d : Pline2d
   {
      double area;
      double perimeter;
      Point2d centroid;
      double ix;
      double iy;

      public double Area { get => Math.Abs(area); }
      public double Perimeter { get => perimeter; }
      public Point2d Centroid { get => centroid; }
      public double Ix { get => Math.Abs(ix); }
      public double Iy { get => Math.Abs(iy); }

      public Polygon2d(IEnumerable<ICoordinates> vertices):base (vertices)
      {
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }
      
      public Polygon2d(List<Vertex2d> vertices):base (vertices)
      {
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }
      
      public Polygon2d(Pline2d pline)
      {
         Vertices = pline.Copy().Vertices;
         Close();

         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public void RecalcToCentroid()
      {
         CalcArea();
         CalcCentroid();
         List<Vertex2d> temp = new List<Vertex2d>();
         foreach (Vertex2d item in Vertices)
         {
            temp.Add(new Vertex2d(item.X - centroid.X, item.Y - centroid.Y));
         }
         Vertices = temp;
         //CalcCentroid();

         CalcI();
         CalcBB();
      }

      protected void CalcI()
      {
         double tempX = 0;
         double tempY = 0;
         Open();
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            tempX = tempX + (Math.Pow(arrTemp.X, 2) + arrTemp.X * arrTemp1.X + Math.Pow(arrTemp1.X, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            tempY = tempY + (Math.Pow(arrTemp.Y, 2) + arrTemp.Y * arrTemp1.Y + Math.Pow(arrTemp1.Y, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
         }
         ix = tempX / 12;
         iy = tempY / 12;
         Close();
      }

      protected void CalcCentroid()
      {
         Open();
         ICoordinates temp = new Point3d();
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            temp.X = temp.X + 1 / (6 * area) * (arrTemp.X + arrTemp1.X) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            temp.Y = temp.Y + 1 / (6 * area) * (arrTemp.Y + arrTemp1.Y) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
         }
         centroid = new Point2d(temp.X, temp.Y);
         Close();
      }

      protected void CalcArea()
      {
         Open();
         double temp = 0;
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            temp = temp + 0.5 * (arrTemp.X * arrTemp1.Y - arrTemp1.X * arrTemp.Y);
         }
         area = temp;
         Close();
      }

      protected void CalcPerimeter()
      {
         Open();
         int count = GetSegsCount();
         if (count < 1) return;
         List<double> segs = new List<double>(count);
         for (int i = 0; i < count; i++)
         {
            segs.Add(GetSegment(i).Length);
         }
         perimeter = segs.Sum();
         Close();
      }

      /// <summary>
      /// Деление контура на сегменты путем деления исходных сегментов по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <remarks>
      /// В качестве шага деления с абсолютным значением следует задавать значение части длины отрезка.
      /// </remarks>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public override Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, true, true));
         }
         res.Close();
         return res;
      }

      /// <summary>
      /// Деление контура на сегменты путем деления исходных сегментов по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public override Pline2d TesselationByNumber(int nDiv)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            res.AddVertices(GetSegment(i).TesselationByNumber(nDiv, true, true));
         }
         res.Close();
         return res;
      }

      public Polygon2d Partition(Vertex2d vrt1, Vertex2d vrt2)
      {
         List<Vertex2d> original = new List<Vertex2d>(vrtxs);
         List<Vertex2d> newl = new List<Vertex2d>
         {
            Vertex2d.Copy(vrt1)
         };

         Vertex2d item = vrt1;
         while (!item.Next.Equals(vrt2))
         {
            item = item.Next;
            newl.Add(item);
            original.Remove(item);
         }
         original.Remove(vrt2);

         vrtxs = original;
         CalcVertices();
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();

         return new Polygon2d(newl);
      }

      public void Partition(Vertex2d vrt1, Vertex2d vrt2, out Polygon2d res)
      {
         res = Partition(vrt1, vrt2);
      }

      public void Insert(Vertex2d main, Vertex2d insert, Polygon2d insertPoly)
      {
         List<Vertex2d> origin = Break(main).Vertices;
         List<Vertex2d> newl = insertPoly.Break(insert).Vertices;
         //newl[0].Prev = origin[i1 - 1].Prev;
         //newl[i2 - 1].Next = origin[0].Next;
         origin.RemoveAt(origin.Count - 1);
         origin.RemoveAt(0);
         newl.Reverse();
         origin.AddRange(newl);
         ReplaceVertices(origin);
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public Pline2d Break(Vertex2d point)
      {
         List<Vertex2d> newl = new List<Vertex2d>(vrtxs.Count + 1)
         {
            Vertex2d.Copy(point)
         };
         Vertex2d item = point;
         while (!item.Next.Equals(point))
         {
            item = item.Next;
            newl.Add(item);
         }
         newl.Add(point);

         return new Pline2d(newl);
      }

      public Mesh Triangulation(double step, ParamType stepType = ParamType.abs)
      {
         return new Mesh();
      }

      public Mesh TriangulationSimple()
      {
         Mesh res = new Mesh();
         res.Out = new List<Node>(vrtxs.Count);
         Polygon2d poly = new Polygon2d(vrtxs);
         for (int i = 0; i < vrtxs.Count; i++)
         {
            res.Out.Add(new Node(vrtxs[i], NodeType.border) { Id = i + 1 });
            poly.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(poly);

         int cntr = 1;

         while (work.Count > 0)
         {
            poly = work.Pop();

            while (poly.Vertices.Count>3)
            {
               var sel = from i in poly.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();
               Triangle tria = new Triangle(v.Next, v, v.Prev);

               #region Проверка на самопересечение при генерации треугольника
               List<Vertex2d> sect = new List<Vertex2d>(poly.Vertices);
               sect.Remove(v.Next);
               sect.Remove(v);
               sect.Remove(v.Prev);
               sel = from i in sect orderby i.LengthTo(v) select i;
               if (tria.IsPointIn(sel.First()))
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, sel.First().Nref) { Id = cntr });
                  cntr++;
                  res.Simplexs.Add(new Tri(v.Nref, sel.First().Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  work.Push(poly.Partition(sel.First(), v));
               }
               #endregion
               else
               {
                  res.Simplexs.Add(new Tri(v.Next.Nref, v.Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  poly.RemoveVertex(v);
               }
            }

            if (poly.Vertices.Count == 3)
            {
               res.Simplexs.Add(new Tri(poly.Vertices[0].Nref, poly.Vertices[1].Nref, poly.Vertices[2].Nref) { Id = cntr });
               cntr++;
            }
         }

         return res;
      }
   }
}
