using Geo.Calc;

using System;
using System.Collections.Generic;

namespace Geo
{
   [Serializable]
   public class Line3d
   {
      Point3d startPoint;
      Point3d endPoint;

      public Point3d StartPoint { get => startPoint; set { startPoint = value; CalcLine(); } }
      public Point3d EndPoint { get => endPoint; set { endPoint = value; CalcLine(); } }
      public Vector3d Directive { get; private set; }
      public double Length { get => Directive.Norma; }
      public double A1 { get; private set; }
      public double A2 { get; private set; }
      public double B1 { get; private set; }
      public double B2 { get; private set; }
      public double C1 { get; private set; }
      public double C2 { get; private set; }
      public double D1 { get; private set; }
      public double D2 { get; private set; }

      public Line3d()
      {

      }

      public Line3d(Point2d startPt, Point2d endPt)
      {
         startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
         CalcLine();
      }
      
      public Line3d(Vertex2d startPt, Vertex2d endPt)
      {
         startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
         CalcLine();
      }

      public Line3d(Point3d startPt, Point3d endPt)
      {
         startPoint = startPt; endPoint = endPt;
         CalcLine();
      }

      void CalcLine()
      {
         Directive = endPoint - startPoint;
         Vector3d n1 = new Vector3d(Directive.Vy, -Directive.Vx); // 1-й вектор нормали
         if (Directive.Vx == 0 && Directive.Vy == 0)
         {
            n1 = new Vector3d(0, 0, 1);
         }
         Vector3d n2 = Directive ^ n1; // 2-й вектор нормали
         A1 = n2.Vx;
         B1 = n2.Vy;
         C1 = n2.Vz;
         D1 = -A1 * StartPoint.X - B1 * StartPoint.Y - C1 * StartPoint.Z;
         A2 = n1.Vx;
         B2 = n1.Vy;
         C2 = n1.Vz;
         D2 = -A2 * StartPoint.X - B2 * StartPoint.Y - C2 * StartPoint.Z;
      }

      /// <summary>
      /// Проверяет, находится ли точка внутри отрезка, определенного начальной и конечной точками прямой.
      /// </summary>
      /// <param name="p">Проверяемая точка.</param>
      /// <param name="bounds">Учет совпадения с начальной и конечной точками отрезка.</param>
      /// <param name="threshold">Точность определения.</param>
      /// <returns>
      /// TRUE, если точка находится внутри отрезка.</returns>

      public bool PointInSegment(ICoordinates p, bool bounds = false, double threshold = 1e-12)
      {
         double tx, ty, tz, t;
         tx = Calcs.IsZero((endPoint - startPoint).Vx, threshold) ?
            double.NaN : (p.ToVector3d() - startPoint.ToVector3d()).Vx / (endPoint - startPoint).Vx;
         ty = Calcs.IsZero((endPoint - startPoint).Vy, threshold) ?
            double.NaN : (p.ToVector3d() - startPoint.ToVector3d()).Vy / (endPoint - startPoint).Vy;
         tz = Calcs.IsZero((endPoint - startPoint).Vz, threshold) ?
            double.NaN : (p.ToVector3d() - startPoint.ToVector3d()).Vz / (endPoint - startPoint).Vz;

         if (double.IsNaN(tx) && Calcs.IsEqual(tz, ty, threshold)) t = ty;
         else if (double.IsNaN(ty) && Calcs.IsEqual(tx, tz, threshold)) t = tx;
         else if (double.IsNaN(tz) && Calcs.IsEqual(tx, ty, threshold)) t = tx;
         else if (double.IsNaN(tx) && double.IsNaN(ty) && double.IsNaN(tz)) return false;
         else if (double.IsNaN(tx) && double.IsNaN(ty)) t = tx;
         else if (double.IsNaN(tx) && double.IsNaN(tz)) t = ty;
         else if (double.IsNaN(ty) && double.IsNaN(tz)) t = tx;
         else if (Calcs.IsEqual(tx, ty, threshold) && Calcs.IsEqual(tx, tz, threshold)) t = tx;
         else return false;

         if (t > 0 && t < 1) return true;
         else if (!bounds && (Calcs.IsZero(t, threshold) || Calcs.IsOne(t, threshold))) return false;
         else if (bounds && (Calcs.IsZero(t, threshold) || Calcs.IsOne(t, threshold))) return true;
         else return false;
      }

      /// <summary>
      /// Вычисляет отрезок линии пересечения с заданной линией (не отрезком).
      /// </summary>
      /// <param name="line3D">Заданная линия.</param>
      /// <param name="ip1">Первая результирующая точка пересечения.</param>
      /// <param name="ip2">Вторая результирующая точка пересечения.</param>
      /// <param name="threshold">Точность определения.</param>
      /// <returns>Возвращает FALSE, если решение не найдено.</returns>
      public bool Intersection(Line3d line3D, out Point3d ip1, out Point3d ip2, double threshold = 1e-12)
      {
         // Алгоритм портирован из C-алгоритма Пола Бурка на http://paulbourke.net/geometry/pointlineplane/lineline.c
         ip1 = null;
         ip2 = null;

         Vector3d p1 = startPoint.ToVector3d();
         Vector3d p2 = endPoint.ToVector3d();
         Vector3d p3 = line3D.startPoint.ToVector3d();
         Vector3d p4 = line3D.endPoint.ToVector3d();
         Vector3d p13 = p1 - p3;
         Vector3d p43 = p4 - p3;

         if (p43.Norma < threshold) return false;

         Vector3d p21 = p2 - p1;
         if (p21.Norma < threshold) return false;

         double d1343 = p13 % p43;
         double d4321 = p43 % p21;
         double d1321 = p13 % p21;
         double d4343 = p43 % p43;
         double d2121 = p21 % p21;

         double denom = d2121 * d4343 - d4321 * d4321;
         if (Math.Abs(denom) < threshold) return false;

         double numer = d1343 * d4321 - d1321 * d4343;

         double mua = numer / denom;
         double mub = (d1343 + d4321 * mua) / d4343;

         ip1 = (p1 + mua * p21).ToPoint3d();
         ip2 = (p3 + mub * p43).ToPoint3d();

         return true;
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
            return Directive.ToPoint3d() * parameter + startPoint.ToVector3d();
         }
         else
         {
            parameter /= Length;
            return Directive.ToPoint3d() * parameter + startPoint.ToVector3d();
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

      public bool IsMath(ICoordinates pt, int t = 6)
      {
         if (Math.Round((pt.X - startPoint.X) / Directive.Vx, 6) == 
            Math.Round((pt.Y - startPoint.Y) / Directive.Vy, 6) &&
            Math.Round((pt.X - startPoint.X) / Directive.Vx, 6) == 
            Math.Round((pt.Z - startPoint.Z) / Directive.Vz, 6)) return true;
         else return false;
      }
      
      public bool IsMath(Line3d line, int t = 6)
      {
         if (Math.Round((line.StartPoint.X - startPoint.X) / Directive.Vx, 6) == 
            Math.Round((line.StartPoint.Y - startPoint.Y) / Directive.Vy, 6) &&
            Math.Round((line.StartPoint.X - startPoint.X) / Directive.Vx, 6) == 
            Math.Round((line.StartPoint.Z - startPoint.Z) / Directive.Vz, 6) &&
            Math.Round((line.EndPoint.X - startPoint.X) / Directive.Vx, 6) ==
            Math.Round((line.EndPoint.Y - startPoint.Y) / Directive.Vy, 6) &&
            Math.Round((line.EndPoint.X - startPoint.X) / Directive.Vx, 6) ==
            Math.Round((line.EndPoint.Z - startPoint.Z) / Directive.Vz, 6)) return true;
         else return false;
      }
      
      public bool IsContain(ICoordinates pt, int t = 6)
      {
         if (pt.X >= startPoint.X && pt.X <= endPoint.X &&
            pt.Y >= startPoint.Y && pt.Y <= endPoint.Y &&
            pt.Z >= startPoint.Z && pt.Z <= endPoint.Z && IsMath(pt, t)) return true;
         else return false;
      }
      
      public bool IsContain(Line3d line, int t = 6)
      {
         if (line.StartPoint.X >= startPoint.X && line.StartPoint.X <= endPoint.X &&
            line.StartPoint.Y >= startPoint.Y && line.StartPoint.Y <= endPoint.Y &&
            line.StartPoint.Z >= startPoint.Z && line.StartPoint.Z <= endPoint.Z &&
            line.EndPoint.X >= startPoint.X && line.EndPoint.X <= endPoint.X &&
            line.EndPoint.Y >= startPoint.Y && line.EndPoint.Y <= endPoint.Y &&
            line.EndPoint.Z >= startPoint.Z && line.EndPoint.Z <= endPoint.Z &&
            IsMath(line, t)) return true;
         else return false;
      }

   }
}

