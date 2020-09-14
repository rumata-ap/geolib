using Geo.Calc;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geo
{
   [Serializable]
   public class Line2d:ICurve2d
   {
      protected Point3d startPoint;
      protected Point3d endPoint;
      protected Vector2d directive;
      protected Vector2d normal;

      public Point3d StartPoint { get => startPoint; set { startPoint = value; if (EndPoint != null) { directive = endPoint.ToPoint2d() - startPoint.ToPoint2d(); CalcLine(); }; } }
      public Point3d EndPoint { get => endPoint; set { endPoint = value; directive = endPoint.ToPoint2d() - startPoint.ToPoint2d(); CalcLine(); } }
      public Point3d CenterPoint { get; private set; }
      public Vector2d Directive { get => directive; private set => directive = value; }
      public Vector2d Normal { get => normal; private set => normal = value; }
      public double A { get; private set; }
      public double B { get; private set; }
      public double C { get; private set; }
      public double k { get; private set; }
      public double b { get; private set; }
      public double Length { get => Directive.Norma; }
      public double cosAlfa { get; private set; }
      public double cosBeta { get; private set; }
      public double p { get; private set; }
      public int Id { get; set; }
      public object Parent { get; set; }

      public CurveType Type => CurveType.line;

      public Line2d()
      {

      }

      public Line2d(Point2d startPt, Point2d endPt)
      {
         startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }

      public Line2d(Vertex2d startPt, Vertex2d endPt)
      {
         startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }

      public Line2d(Point3d startPt, Point3d endPt)
      {
         startPoint = startPt;
         endPoint = endPt;
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }

      protected void CalcLine()
      {
         A = Directive.Vy;
         B = -Directive.Vx;
         normal = new Vector2d(A, B);
         C = Directive.Vx * StartPoint.Y - Directive.Vy * StartPoint.X;
         if (Directive.Vx != 0) { k = Directive.Vy / Directive.Vx; }
         else { k = Double.PositiveInfinity; }
         if (Directive.Vx != 0) { b = -Directive.Vy / Directive.Vx * StartPoint.X + StartPoint.Y; }
         else { b = Double.PositiveInfinity; }
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         double dx = 0.5 * (EndPoint.X - StartPoint.X);
         double dy = 0.5 * (EndPoint.Y - StartPoint.Y);
         CenterPoint = new Point3d(StartPoint.X + dx, StartPoint.Y + dy, 0);
      }

      public Vector2d GetUnitNormal()
      {
         return new Vector2d(cosAlfa, cosBeta);
      }

      public double LengthTo(Point2d point)
      {
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         return normC * (A * point.X + B * point.Y + C);
      }
      public double LengthTo(Point3d point)
      {
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         return normC * (A * point.X + B * point.Y + C);
      }
      public double Interpolation(double x)
      {
         if (B == 0) return double.NaN;
         else return (-A * x - C) / B;
      }

      /// <summary>
      /// Вычисляет минимальное расстояние до точки.
      /// </summary>
      /// <param name="p">Точка.</param>
      /// <returns>Минимальное расстояние между точкой и линией.</returns>
      public double DistanceToPoint(IXYZ p)
      {
         double t = Directive.ToVector3d() % (p.ToVector3d() - StartPoint.ToVector3d());
         Vector3d pPrime = StartPoint.ToVector3d() + Directive.ToVector3d() * t;
         Vector3d vec = p.ToVector3d() - pPrime;
         double distanceSquared = vec.Norma;
         return Math.Sqrt(distanceSquared);
      }

      /// <summary>
      /// Проверяет, находится ли точка внутри отрезка, определенного начальной и конечной точками прямой.
      /// </summary>
      /// <param name="p">Проверяемая точка.</param>
      /// <param name="bounds">Учет совпадения с начальной и конечной точками отрезка.</param>
      /// <param name="threshold">Точность определения.</param>
      /// <returns>
      /// TRUE, если точка находится внутри отрезка.</returns>

      public bool PointInSegment(IXYZ p, bool bounds = false, double threshold = 1e-12)
      {
         double tx, ty, t;
         tx = Calcs.IsZero((endPoint - startPoint).Vx, threshold) ? 
            double.NaN : (p.ToVector3d() - startPoint.ToVector3d()).Vx / (endPoint - startPoint).Vx;
         ty = Calcs.IsZero((endPoint - startPoint).Vy, threshold) ?
            double.NaN : (p.ToVector3d() - startPoint.ToVector3d()).Vy / (endPoint - startPoint).Vy;

         if (double.IsNaN(tx)) t = ty;
         else if (double.IsNaN(ty)) t = tx;
         else if (double.IsNaN(tx) && double.IsNaN(ty)) return false;
         else if (Calcs.IsEqual(tx, ty, threshold)) t = tx;
         else return false;

         if (t > 0 && t < 1) return true;
         else if (!bounds && (Calcs.IsZero(t, threshold) || Calcs.IsOne(t, threshold))) return false;
         else if (bounds && (Calcs.IsZero(t, threshold) || Calcs.IsOne(t, threshold))) return true;
         else return false;
      }

      /// <summary>
      /// Вычисление точки прересечения c заданной линией.
      /// </summary>
      /// <param name="line">Заданная линия.</param>
      /// <param name="ip">Точка пересечения обеих линий (если они пересекаются).</param>
      /// <param name="bounds">Учет совпадения вычисленной точки пересечения с начальной и конечной точками отрезков прямых.
      /// 0 - если проверка попадания точки пересечения внутрь отрезков обеих прямых не требуется, 
      /// 1 - если требуется проверка попадания точки пересечения внутрь или на границы отрезков обеих прямых, 
      /// 2 - если требуется проверка попадания точки пересечения только внутрь отрезков обеих прямых.
      /// </param>
      /// <param name="prec">Предопределенная точность вычислений.</param>
      /// <returns>TRUE, если точка пересечения найдена.</returns>
      /// <remarks></remarks>
      public bool Intersection(Line2d line, out Point2d ip, int bounds = 0, double prec = 1e-12)
      {
         ip = null;

         // Знаменатель для ua и ub одинаков, поэтому сохраняем этот вычисление
         double d =
            (line.endPoint.Y - line.startPoint.Y) * (endPoint.X - startPoint.X)
            -
            (line.endPoint.X - line.startPoint.X) * (endPoint.Y - startPoint.Y);

         //n_a и n_b рассчитываются как отдельные значения для удобочитаемости
         double n_a =
            (line.endPoint.X - line.startPoint.X) * (startPoint.Y - line.startPoint.Y)
            -
            (line.endPoint.Y - line.startPoint.Y) * (startPoint.X - line.startPoint.X);

         double n_b =
            (endPoint.X - startPoint.X) * (startPoint.Y - line.startPoint.Y)
            -
            (endPoint.Y - startPoint.Y) * (startPoint.X - line.startPoint.X);

         // Убедитесь, что нет деления на ноль - это также означает, что линии параллельны.
         // Если бы n_a и n_b были равны нулю, строки были бы 
         // друг над другом (совпадение). Эта проверка не выполняется, потому что 
         // она не требуется для данной реализации (это учитывается параллельной проверкой).
         if (Calcs.IsZero(d, prec)) return false;

         // Вычисление промежуточной дробной точки, которую потенциально пересекают линии.
         double ua = n_a / d;
         double ub = n_b / d;
         ip = (startPoint + (ua * (endPoint - startPoint))).ToPoint2d();

         if (ua > 0 && ua < 1 && ub > 0 && ub < 1) return true;

         // Точка деления будет между 0 и 1 включительно, если линии 
         // пересекаются. Если дробное вычисление больше 1 или меньше 
         // 0, линии должны быть длиннее для пересечения.
         switch (bounds)
         {
            case 0:
               return true;
            case 1:
               if ((ua > 0 || Calcs.IsZero(ua, prec)) && (ua <= 1 || Calcs.IsOne(ua, prec)) && 
                   (ub > 0 || Calcs.IsZero(ub, prec)) && (ub < 1 || Calcs.IsOne(ub, prec)))
               {
                  return true;
               }
               break;
         }
         
         return false;
      }

      /// <summary>
      /// Вычисление точки прересечения c линией.
      /// </summary>
      /// <param name="line">Плоская иния.</param>
      /// <param name="res">Ссылка на объект результата.</param>
      [Obsolete("Используйте Intersection(line, ip, bounds, prec) вместо этого метода.")]
      public void Intersection(Line2d line, out IntersectResult res)
      {
         res = new IntersectResult();

         double A1 = this.A;
         double B1 = this.B;
         double C1 = this.C;
         double A2 = line.A;
         double B2 = line.B;
         double C2 = line.C;

         if (A1 == 0 || B2 - (A2 * B1) / A1 == 0) return;

         double y = ((A2 * C1) / A1 - C2) / (B2 - (A2 * B1) / A1);
         double x = (-C1 - B1 * y) / A1;

         res.pts = new List<Point2d>(1);
         res.pts.Add(new Point2d(x, y));
         res.res = true;
      }

      [Obsolete("Используйте Intersection(line, ip, bounds, prec) вместо этого метода.")]
      public void IntersectionSegments(Line2d line, out IntersectResult res)
      {
         res = new IntersectResult();
         IntersectResult resl;
         Intersection(line, out resl);
         if (resl.res == false) return;
         Point2d respt = resl.pts[0];
         Range Xrange = new Range(StartPoint.X, EndPoint.X);
         Range Yrange = new Range(StartPoint.Y, EndPoint.Y);
         Range Xrangel = new Range(line.StartPoint.X, line.EndPoint.X);
         Range Yrangel = new Range(line.StartPoint.Y, line.EndPoint.Y);
         if (Xrange.InNoBound(respt.X) && Yrange.InNoBound(respt.Y) && Xrangel.InNoBound(respt.X) && Yrangel.InNoBound(respt.Y))
         {
            res.res = true;
            res.pts = new List<Point2d>(1);
            res.pts.Add(respt);
         }
      }

      public Vector3d ToUnitVector3d()
      {
         return new Vector3d(new double[] { Directive.Vx / Length, Directive.Vy / Length, 0 });
      }

      /// <summary>
      /// Получение точки на линии по заданнамо параметру.
      /// </summary>
      /// <param name="param">Параметр в долях единицы или абсолютное значение в диапазоне от -∞ до +∞</param>
      /// <param name="paramType">Тип параметра (относительный или абсолютный)</param>
      /// <returns>Возвращает вычисленную точку</returns>
      public Point3d GetPoint(double parameter, ParamType paramType = ParamType.rel)
      {
         if (paramType == ParamType.rel)
         {
            return directive.ToPoint3d() * parameter + startPoint.ToVector3d();
         }
         else
         {
            parameter /= Length;
            return directive.ToPoint3d() * parameter + startPoint.ToVector3d();
         }       
      }

      /// <summary>
      /// Деление отрезка на равные участки по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <remarks>
      /// В качестве шага деления с абсолютным значением следует задавать значение части длины отрезка.
      /// </remarks>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs, bool start = true, bool end = true)
      {
         Range range;
         Vector vector;
         if (stepType == ParamType.abs)
         {
            if (step > Length) step = Length;
            double param = step / Length;
            step = Length * param;
            range = new Range(0, Length);
            vector = range.GetVectorByStep(step, true, start, end);
         }
         else
         {
            if (step > 1) step = 1;
            range = new Range(0, 1);
            vector = range.GetVectorByStepParam(step, start, end);
         }

         if (vector == null) return new Pline2d();

         List<Point3d> pts = new List<Point3d>(vector.N);
         for (int i = 0; i < vector.N; i++) pts.Add(GetPoint(vector[i], stepType));

         return new Pline2d(pts.ToArray());
      }

      /// <summary>
      /// Деление отрезка на равные участки по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true)
      {
         if (nDiv <= 0) nDiv = 1;
         Range range = new Range(0, Length);
         Vector vector = range.GetVectorByNumber(nDiv, start, end);
         List<Point3d> pts = new List<Point3d>(vector.N);
         for (int i = 0; i < vector.N; i++) pts.Add(GetPoint(vector[i], ParamType.abs));

         return new Pline2d(pts.ToArray());
      }

   }

   [Serializable]
   public struct IntersectResult
   {
      public List<Point2d> pts;
      public bool res;
   }
}
