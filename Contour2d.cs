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

      public Polygon2d(IEnumerable<Point2d> points)
      {
         List<Point2d> point2Ds = new List<Point2d>(points);
         List<Vertex2d> vertices = new List<Vertex2d>(point2Ds.Count);
         foreach (Point2d item in point2Ds)
         {
            vertices.Add(new Vertex2d(item.ToPoint3d()));
         }
         Vertices = vertices;
         Close();

         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }
      
      public Polygon2d(IEnumerable<Point3d> points)
      {
         List<Point3d> point3Ds = new List<Point3d>(points);
         List<Vertex2d> vertices = new List<Vertex2d>(point3Ds.Count);
         foreach (Point3d item in point3Ds)
         {
            vertices.Add(new Vertex2d(item));
         }
         Vertices = vertices;
         Close();

         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }
      
      public Polygon2d(IEnumerable<Vertex2d> vertices)
      {
         Vertices = new List<Vertex2d>(vertices);
         Close();

         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public Polygon2d(Pline2d pline)
      {
         Vertices = pline.Vertices;
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
         int count = GetSegsCount();
         if (count < 1) return;
         List<double> segs = new List<double>(count);
         for (int i = 0; i < count; i++)
         {
            segs.Add(GetSegment(i).Length);
         }
         perimeter = segs.Sum();
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
      public override Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs, bool start = true, bool end = true)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, start, end));
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
      public override Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            res.AddVertices(GetSegment(i).TesselationByNumber(nDiv, start, end));
         }
         res.Close();
         return res;
      }

      public Polygon2d Partition()
      {
         return null;
      }
   }
}
