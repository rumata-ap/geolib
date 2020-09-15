using System;

using Geo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoTest
{
   [TestClass]
   public class UnitTest1
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="npol"></param>
      /// <param name="xp"></param>
      /// <param name="yp"></param>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      /// <remarks>
      /// Следующий код написан Рэндольфом Франклином, 
      /// он возвращает 1 для внутренних точек и 0 для внешних точек.
      /// </remarks>
      private int PointInPolygon(int npol, float[] xp, float[] yp, float x, float y)
      {
         int i, j;
         int c = 0;
         for (i = 0, j = npol - 1; i < npol; j = i++)
         {
            if ((((yp[i] <= y) && (y < yp[j])) || ((yp[j] <= y) && (y < yp[i]))) && 
               (x < (xp[j] - xp[i]) * (y - yp[i]) / (yp[j] - yp[i]) + xp[i]))
            {
               c = 1;
            }
         }
         return c;
      }

      public static bool Contains(IXYZ[] points, IXYZ p)
      {
         bool result = false;

         for (int i = 0; i < points.Length - 1; i++)
         {
            if ((((points[i + 1].Y <= p.Y) && (p.Y < points[i].Y)) || ((points[i].Y <= p.Y) && 
               (p.Y < points[i + 1].Y))) && 
               (p.X < (points[i].X - points[i + 1].X) * (p.Y - points[i + 1].Y) / (points[i].Y - points[i + 1].Y) + points[i + 1].X))
            {
               result = !result;
            }
         }
         return result;
      }

      /*
         Determine whether or not the line segment p1,p2
         Intersects the 3 vertex facet bounded by pa,pb,pc
         Return true/false and the intersection point p

         The equation of the line is p = p1 + mu (p2 - p1)
         The equation of the plane is a x + b y + c z + d = 0
                                      n.X x + n.Y y + n.Z z + d = 0
      */
      private int LineFacet(IXYZ p1, IXYZ p2, IXYZ pa, IXYZ pb, IXYZ pc, IXYZ p)
      {
         double d;
         double a1;
         double a2;
         double a3;
         double total;
         double denom;
         double mu;
         IXYZ n = new Point3d();
         IXYZ pa1 = new Point3d();
         IXYZ pa2 = new Point3d();
         IXYZ pa3 = new Point3d();

         /* Расчет параметров плоскости */
         n.X = (pb.Y - pa.Y) * (pc.Z - pa.Z) - (pb.Z - pa.Z) * (pc.Y - pa.Y);
         n.Y = (pb.Z - pa.Z) * (pc.X - pa.X) - (pb.X - pa.X) * (pc.Z - pa.Z);
         n.Z = (pb.X - pa.X) * (pc.Y - pa.Y) - (pb.Y - pa.Y) * (pc.X - pa.X);
         Normalise(n);
         d = -n.X * pa.X - n.Y * pa.Y - n.Z * pa.Z;

         /* Вычислить положение на линии, пересекающей плоскость */
         denom = n.X * (p2.X - p1.X) + n.Y * (p2.Y - p1.Y) + n.Z * (p2.Z - p1.Z);
         if (ABS(denom) < EPS) // Прямая и плоскость не пересекаются
         {
            return (0);
         }
         mu = -(d + n.X * p1.X + n.Y * p1.Y + n.Z * p1.Z) / denom;
         p.X = p1.X + mu * (p2.X - p1.X);
         p.Y = p1.Y + mu * (p2.Y - p1.Y);
         p.Z = p1.Z + mu * (p2.Z - p1.Z);
         if (mu < 0 || mu > 1) // Пересечение находится не на отрезке, задающем прямую
         {
            return (0);
         }

         /* Определение, ограничена ли точка пересечения параметрами pa, pb, pc */
         pa1.X = pa.X - p.X;
         pa1.Y = pa.Y - p.Y;
         pa1.Z = pa.Z - p.Z;
         Normalise(pa1);
         pa2.X = pb.X - p.X;
         pa2.Y = pb.Y - p.Y;
         pa2.Z = pb.Z - p.Z;
         Normalise(pa2);
         pa3.X = pc.X - p.X;
         pa3.Y = pc.Y - p.Y;
         pa3.Z = pc.Z - p.Z;
         Normalise(pa3);
         a1 = pa1.X * pa2.X + pa1.Y * pa2.Y + pa1.Z * pa2.Z;
         a2 = pa2.X * pa3.X + pa2.Y * pa3.Y + pa2.Z * pa3.Z;
         a3 = pa3.X * pa1.X + pa3.Y * pa1.Y + pa3.Z * pa1.Z;
         total = (Math.Acos(a1) + Math.Acos(a2) + Math.Acos(a3)) * RTOD; //Комментарии. Функция RTOD( ) преобразует значение числового выражения, заданное в радианах, в эквивалентное значение в градусах.
         if (Math.Abs(total - 360) > EPS)
         {
            return (0);
         }

         return (1);
      }


      /// <summary>
      /// Закрепите фасет с 3 вершинами на месте
      /// </summary>
      /// <param name="p">Трехточечная грань определяется вершинами p[0], p[1], p[2], "p [3]".</param>
      /// <param name="n">Нормаль к плоскости</param>
      /// <param name="p0">Точка на плоскости</param>
      /// <returns>Возвращает количество вершин в обрезанном многоугольнике</returns>
      /// <remarks>
      /// http://paulbourke.net/geometry/polygonmesh/
      ///Может быть задана четвертая точка для определения четырехточечной грани.
      ///Сторона грани, содержащая нормаль плоскости, обрезается.
      /// </remarks>
      [TestMethod]
      private int ClipFacet(IXYZ[] p, IXYZ n, IXYZ p0)
      {
         double A;
         double B;
         double C;
         double D;
         double l;
         double[] side = new double[3];
         IXYZ q = new Point3d();

         /*
            Определение уравнения плоскости в виде
            Ax + By + Cz + D = 0
         */
         l = Math.Sqrt(n.X * n.X + n.Y * n.Y + n.Z * n.Z);
         A = n.X / l;
         B = n.Y / l;
         C = n.Z / l;
         D = -(n.X * p0.X + n.Y * p0.Y + n.Z * p0.Z);

         /*
            Оценка уравнения плоскости для каждой вершины. 
            Если side < 0, то она находится на той стороне, 
            которую необходимо сохранить, 
            в противном случае она должна быть обрезана.
         */
         side[0] = A * p[0].X + B * p[0].Y + C * p[0].Z + D;
         side[1] = A * p[1].X + B * p[1].Y + C * p[1].Z + D;
         side[2] = A * p[2].X + B * p[2].Y + C * p[2].Z + D;

         // Все ли вершины на стононе обрезки
         if (side[0] >= 0 && side[1] >= 0 && side[2] >= 0)
         {
            return (0);
         }

         // Все ли вершины на противоположной стороне
         if (side[0] <= 0 && side[1] <= 0 && side[2] <= 0)
         {
            return (3);
         }

         // p0 единственная точка на стороне обрезки
         if (side[0] > 0 && side[1] < 0 && side[2] < 0)
         {
            q.X = p[0].X - side[0] * (p[2].X - p[0].X) / (side[2] - side[0]);
            q.Y = p[0].Y - side[0] * (p[2].Y - p[0].Y) / (side[2] - side[0]);
            q.Z = p[0].Z - side[0] * (p[2].Z - p[0].Z) / (side[2] - side[0]);
            p[3] = q;
            q.X = p[0].X - side[0] * (p[1].X - p[0].X) / (side[1] - side[0]);
            q.Y = p[0].Y - side[0] * (p[1].Y - p[0].Y) / (side[1] - side[0]);
            q.Z = p[0].Z - side[0] * (p[1].Z - p[0].Z) / (side[1] - side[0]);
            p[0] = q;
            return (4);
         }

         // p1 единственная точка на стороне обрезки
         if (side[1] > 0 && side[0] < 0 && side[2] < 0)
         {
            p[3] = p[2];
            q.X = p[1].X - side[1] * (p[2].X - p[1].X) / (side[2] - side[1]);
            q.Y = p[1].Y - side[1] * (p[2].Y - p[1].Y) / (side[2] - side[1]);
            q.Z = p[1].Z - side[1] * (p[2].Z - p[1].Z) / (side[2] - side[1]);
            p[2] = q;
            q.X = p[1].X - side[1] * (p[0].X - p[1].X) / (side[0] - side[1]);
            q.Y = p[1].Y - side[1] * (p[0].Y - p[1].Y) / (side[0] - side[1]);
            q.Z = p[1].Z - side[1] * (p[0].Z - p[1].Z) / (side[0] - side[1]);
            p[1] = q;
            return (4);
         }

         // p2 единственная точка на стороне обрезки
         if (side[2] > 0 && side[0] < 0 && side[1] < 0)
         {
            q.X = p[2].X - side[2] * (p[0].X - p[2].X) / (side[0] - side[2]);
            q.Y = p[2].Y - side[2] * (p[0].Y - p[2].Y) / (side[0] - side[2]);
            q.Z = p[2].Z - side[2] * (p[0].Z - p[2].Z) / (side[0] - side[2]);
            p[3] = q;
            q.X = p[2].X - side[2] * (p[1].X - p[2].X) / (side[1] - side[2]);
            q.Y = p[2].Y - side[2] * (p[1].Y - p[2].Y) / (side[1] - side[2]);
            q.Z = p[2].Z - side[2] * (p[1].Z - p[2].Z) / (side[1] - side[2]);
            p[2] = q;
            return (4);
         }

         // p0 единственная точка на обратной стороне обрезки
         if (side[0] < 0 && side[1] > 0 && side[2] > 0)
         {
            q.X = p[0].X - side[0] * (p[1].X - p[0].X) / (side[1] - side[0]);
            q.Y = p[0].Y - side[0] * (p[1].Y - p[0].Y) / (side[1] - side[0]);
            q.Z = p[0].Z - side[0] * (p[1].Z - p[0].Z) / (side[1] - side[0]);
            p[1] = q;
            q.X = p[0].X - side[0] * (p[2].X - p[0].X) / (side[2] - side[0]);
            q.Y = p[0].Y - side[0] * (p[2].Y - p[0].Y) / (side[2] - side[0]);
            q.Z = p[0].Z - side[0] * (p[2].Z - p[0].Z) / (side[2] - side[0]);
            p[2] = q;
            return (3);
         }

         // p1 единственная точка на обратной стороне обрезки
         if (side[1] < 0 && side[0] > 0 && side[2] > 0)
         {
            q.X = p[1].X - side[1] * (p[0].X - p[1].X) / (side[0] - side[1]);
            q.Y = p[1].Y - side[1] * (p[0].Y - p[1].Y) / (side[0] - side[1]);
            q.Z = p[1].Z - side[1] * (p[0].Z - p[1].Z) / (side[0] - side[1]);
            p[0] = q;
            q.X = p[1].X - side[1] * (p[2].X - p[1].X) / (side[2] - side[1]);
            q.Y = p[1].Y - side[1] * (p[2].Y - p[1].Y) / (side[2] - side[1]);
            q.Z = p[1].Z - side[1] * (p[2].Z - p[1].Z) / (side[2] - side[1]);
            p[2] = q;
            return (3);
         }

         // p2 единственная точка на обратной стороне обрезки
         if (side[2] < 0 && side[0] > 0 && side[1] > 0)
         {
            q.X = p[2].X - side[2] * (p[1].X - p[2].X) / (side[1] - side[2]);
            q.Y = p[2].Y - side[2] * (p[1].Y - p[2].Y) / (side[1] - side[2]);
            q.Z = p[2].Z - side[2] * (p[1].Z - p[2].Z) / (side[1] - side[2]);
            p[1] = q;
            q.X = p[2].X - side[2] * (p[0].X - p[2].X) / (side[0] - side[2]);
            q.Y = p[2].Y - side[2] * (p[0].Y - p[2].Y) / (side[0] - side[2]);
            q.Z = p[2].Z - side[2] * (p[0].Z - p[2].Z) / (side[0] - side[2]);
            p[0] = q;
            return (3);
         }

         // Shouldn't get here
         return (-1);
      }

   }
}
