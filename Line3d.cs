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
      public Pline2d TesselationByStep(double step, ParamType stepType = ParamType.rel, bool start = true, bool end = true)
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

         return new Pline2d(pts);
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

         return new Pline2d(pts);
      }

   }
}

