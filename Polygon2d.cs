using Geo.Calc;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Geo
{
   public class Polygon2d : Pline2d
   {
      double area;
      double ix;
      double iy;

      public double Area { get => Math.Abs(area); }
      public double Perimeter { get; private set; }
      public Point2d Centroid { get; private set; }
      public double Ix { get => Math.Abs(ix); }
      public double Iy { get => Math.Abs(iy); }
      public int Id { get; set; }
      public string Label { get; set; }

      public Polygon2d(IEnumerable<ICoordinates> vertices) : base(vertices)
      {
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public Polygon2d(List<Vertex2d> vertices) : base(vertices)
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

      protected void CalcI()
      {
         double tempX = 0;
         double tempY = 0;
         Open();
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            tempX = tempX + (Math.Pow(arrTemp.X, 2) + arrTemp.X * arrTemp1.X + Math.Pow(arrTemp1.X, 2)) * 
               (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            tempY = tempY + (Math.Pow(arrTemp.Y, 2) + arrTemp.Y * arrTemp1.Y + Math.Pow(arrTemp1.Y, 2)) * 
               (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
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
            temp.X = temp.X + 1 / (6 * area) * (arrTemp.X + arrTemp1.X) * 
               (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            temp.Y = temp.Y + 1 / (6 * area) * (arrTemp.Y + arrTemp1.Y) * 
               (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
         }
         Centroid = new Point2d(temp.X, temp.Y);
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
         Perimeter = segs.Sum();
         Close();
      }

      /// <summary>
      /// Поверка направления обхода полигона.
      /// </summary>
      /// <param name="polygon">Полигон <see cref="Polygon2d" /></param>
      /// <returns> true - если направление обхода по часовой стрелке.</returns>
      public bool IsClockwise()
      {
         List<Vertex2d> sort = (from i in vrtxs orderby i.X select i).ToList();
         Vertex2d v = sort[0];
         Vector3d v1 = v.Prev - v;
         Vector3d v2 = v.Next - v;
         Vector3d v3 = v1 ^ v2;
         if (v3.Vz > 0) return true;
         else return false;
      }

      /// <summary>
      /// Пересчет координат вершин полигона от начала координат в центроиде (центре тяжести).
      /// </summary>
      public void RecalcToCentroid()
      {
         CalcArea();
         CalcCentroid();
         List<Vertex2d> temp = new List<Vertex2d>();
         foreach (Vertex2d item in Vertices)
         {
            temp.Add(new Vertex2d(item.X - Centroid.X, item.Y - Centroid.Y));
         }
         Vertices = temp;
         //CalcCentroid();

         CalcI();
         CalcBB();
      }

      /// <summary>
      /// Деление контура на сегменты путем деления исходных сегментов по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <returns>Возврашает плоский полигон с вершинами в точках деления и линейными сегментами.</returns>
      public override Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, true, true));
         }
         return new Polygon2d(res);
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
         return new Polygon2d(res);
      }

      /// <summary>
      /// Разделение полигона в указанных вершинах с учетом их совмещения.
      /// </summary>
      /// <param name="move">Совмещаемая вершина.</param>
      /// <param name="anchor">Неподвижная вершина.</param>
      /// <returns>Возвращает отделенный полигон.</returns>
      public Polygon2d Partition(Vertex2d move, Vertex2d anchor)
      {
         List<Vertex2d> original = new List<Vertex2d>(vrtxs);
         List<Vertex2d> newl = new List<Vertex2d>
         {
            Vertex2d.Copy(move)
         };

         Vertex2d item = move;
         while (!item.Next.Equals(anchor))
         {
            item = item.Next;
            newl.Add(item);
            original.Remove(item);
         }
         original.Remove(anchor);

         vrtxs = original;
         CalcVertices();
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();

         return new Polygon2d(newl);
      }

      /// <summary>
      /// Разделение полигона в указанных вершинах с учетом их совмещения.
      /// </summary>
      /// <param name="move">Совмещаемая вершина.</param>
      /// <param name="anchor">Неподвижная вершина.</param>
      /// <param name="res">Отделенный полигон.</param>
      public void Partition(Vertex2d move, Vertex2d anchor, out Polygon2d res)
      {
         res = Partition(move, anchor);
      }

      /// <summary>
      /// Объединение с отдельным полигоном в указанных вершинах.
      /// </summary>
      /// <param name="m">Вершина на исходном полигоне.</param>
      /// <param name="u">Вершина на включаемом полигоне.</param>
      /// <param name="cp">Включаемый полигон.</param>
      public void Combin(Vertex2d m, Vertex2d u, Polygon2d cp)
      {
         List<Vertex2d> origin = Break(m).Vertices;
         List<Vertex2d> newl = cp.Break(u).Vertices;
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

      /// <summary>
      /// Разрыв полигона в указанной вершине.
      /// </summary>
      /// <param name="point">Вершина, в которой будет выполнен разрыв.</param>
      /// <returns>Возвращает плоскую разомкнутую полилинию.</returns>
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

      /// <summary>
      /// Определение максимального угла полигона.
      /// </summary>
      /// <returns>Возвращает величину максимального угла в градусах.</returns>
      public double MaxAngleDeg()
      {
         var sel = from i in vrtxs orderby i.AngleDeg select i;
         return sel.Last().AngleDeg;
      }
   }
}
