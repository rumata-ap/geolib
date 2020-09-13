using Geo.Calc;

using System;

namespace Geo
{
   public class Circle2d
   {
      public Point3d Center { get; private set; }
      public double Length { get; private set; }
      public double Radius { get; private set; }
      public int Id { get; set; }
      public object Parent { get; set; }

      public CurveType Type => CurveType.circle;

      public Circle2d(ICoordinates cc, double r)
      {
         Center = new Point3d(cc);
         Radius = r;
         Length = 2 * Radius * Math.PI;
      }

      public Circle2d(ICoordinates p1, ICoordinates p2, ICoordinates p3)
      {
         double ma = (p2.Y - p1.Y) / (p2.X - p1.X);
         double mb = (p3.Y - p2.Y) / (p3.X - p2.X);

         double x = (ma * mb * (p1.Y - p3.Y) + mb * (p2.X + p1.X) - ma * (p3.X + p2.X)) / (2 * (mb - ma));
         double y = 1 / ma * (x - 0.5 * (p2.X + p1.X)) + 0.5 * (p2.Y + p1.Y);
         Center = new Point3d(x, y);
         Radius = (p1.ToVector3d() - Center.ToVector3d()).Norma;
         Length = 2 * Radius * Math.PI;
      }

      /// <summary>
      /// Вычисление пересечения линии и сферы.
      /// </summary>
      /// <param name="p1">Начальная точка линии.</param>
      /// <param name="p2">Конечная точка линии.</param>
      /// <param name="sc">Центр сферы.</param>
      /// <param name="r">Радиус сферы.</param>
      /// <param name="mu1">Первый результирующий параметр.</param>
      /// <param name="mu2">Второй результирующий параметр.</param>
      /// <remarks>
      ///  Отрезок определяется от p1 до p2.
      ///  Сфера определяется радиусом r с центром в sc.
      ///  Есть потенциально две точки пересечения, заданные
      ///  p = p1 + mu1 (p2 - p1)
      ///  p = p1 + mu2 (p2 - p1)
      /// </remarks>
      /// <returns>FALSE, если луч не пересекает сферу.</returns>
      private bool RaySphere(ICoordinates p1, ICoordinates p2, ICoordinates sc, double r, out double mu1, out double mu2)
      {
         double a, b, c, bb4ac;
         Vector3d dp = p2.ToVector3d() - p1.ToVector3d();

         a = dp.Norma;
         b = 2 * (dp.Vx * (p1.X - sc.X) + dp.Vy * (p1.Y - sc.Y) + dp.Vz * (p1.Z - sc.Z));
         c = sc.ToVector3d().Norma;
         c += p1.ToVector3d().Norma;
         c -= 2 * (sc.X * p1.X + sc.Y * p1.Y + sc.Z * p1.Z);
         c -= r * r;
         bb4ac = b * b - 4 * a * c;
         if (Math.Abs(a) < 1e-12 || bb4ac < 0)
         {
            mu1 = double.NaN;
            mu2 = double.NaN;
            return false;
         }
         else if (bb4ac > 0 && bb4ac <= 1e-12)
         {
            mu1 = -b / (2 * a);
            mu2 = double.NaN;
            return false;
         }

         mu1 = (-b + Math.Sqrt(bb4ac)) / (2 * a);
         mu2 = (-b - Math.Sqrt(bb4ac)) / (2 * a);

         return true;
      }

      public Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true)
      {
         throw new NotImplementedException();
      }

      public Pline2d TesselationByStep(double step, ParamType stepType = ParamType.rel, bool start = true, bool end = true)
      {
         throw new NotImplementedException();
      }
   }
}