using Geo.Calc;

using System;
using System.Collections.Generic;

namespace Geo
{
   [Serializable]
   public class Arc2d : ICurve2d
   {
      private Point3d startPoint;
      private Point3d endPoint;

      public Point3d StartPoint { get => startPoint; set { startPoint = value; CalcArc(); } }
      public Point3d EndPoint { get => endPoint; set { endPoint = value; CalcArc(); } }
      public Point3d Center { get; private set; }
      public double Length { get; private set; }
      public double Radius { get; private set; }
      public double Angle { get; private set; }
      public double Angle0 { get; private set; }
      public double Bulge { get; private set; }
      public int Sign { get; private set; }
      public int Id { get; set; }
      public object Parent { get; set; }

      public CurveType Type => CurveType.arc;

      /// <summary>
      /// Создание плоской дуги по двум точкам и радиусу.
      /// </summary>
      /// <param name="pt1">Начальная точка</param>
      /// <param name="pt2">Конечная точка</param>
      /// <param name="r">Радиус</param>
      /// <param name="sign"></param>
      public Arc2d(Point3d pt1, Point3d pt2, double r, int sign = -1)
      {
         startPoint = pt1;
         endPoint = pt2;
         Radius = r;
         Sign = Math.Sign(sign);
         CalcArc();
      }

      public Arc2d(Vertex2d pt1, Vertex2d pt2, double r, int sign = -1)
      {
         startPoint = pt1.ToPoint3d();
         endPoint = pt2.ToPoint3d();
         Radius = r;
         Sign = Math.Sign(sign);
         CalcArc();
      }

      public Arc2d(Point3d pt1, Point3d pt2, double bulge)
      {
         startPoint = pt1;
         endPoint = pt2;
         Sign = Math.Sign(bulge);
         Bulge = bulge;
         Vector3d p = EndPoint - StartPoint;
         double l = Math.Sqrt(p[0] * p[0] + p[1] * p[1]);
         Angle = 4 * Math.Atan(Math.Abs(bulge));
         Radius = 0.5 * l / Math.Sin(0.5 * Angle);
         Matrix C = new Matrix(3, 3);
         C[0, 0] = p[0] / l;
         C[0, 1] = p[1] / l;
         C[1, 0] = p[1] / l;
         C[1, 1] = -p[0] / l;
         C[2, 2] = 1;
         Vector3d cl = new Vector3d(new double[] { 0.5 * l, Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) * -Sign, 0 });
         Vector3d c = C.Inverse() * cl + StartPoint.ToVector3d();
         Center = new Point3d(c.ToArray());
         Angle0 = 0.5 * Math.PI - 0.5 * Angle;
         Length = Radius * Angle;
      }

      public Arc2d(Vertex2d pt1, Vertex2d pt2, double bulge)
      {
         startPoint = pt1.ToPoint3d();
         endPoint = pt2.ToPoint3d();
         Sign = Math.Sign(bulge);
         Bulge = bulge;
         Vector3d p = EndPoint - StartPoint;
         double l = Math.Sqrt(p[0] * p[0] + p[1] * p[1]);
         Angle = 4 * Math.Atan(Math.Abs(bulge));
         Radius = 0.5 * l / Math.Sin(0.5 * Angle);
         Matrix C = new Matrix(3, 3);
         C[0, 0] = p[0] / l;
         C[0, 1] = p[1] / l;
         C[1, 0] = p[1] / l;
         C[1, 1] = -p[0] / l;
         C[2, 2] = 1;
         Vector3d cl = new Vector3d(new double[] { 0.5 * l, Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) * -Sign, 0 });
         Vector3d c = C.Inverse() * cl + StartPoint.ToVector3d();
         Center = new Point3d(c.ToArray());
         Angle0 = 0.5 * Math.PI - 0.5 * Angle;
         Length = Radius * Angle;
      }

      private void CalcArc()
      {
         Vector3d p = EndPoint - StartPoint;
         double l = Math.Sqrt(p[0] * p[0] + p[1] * p[1]);
         Matrix C = new Matrix(3, 3);
         C[0, 0] = p[0] / l;
         C[0, 1] = p[1] / l;
         C[1, 0] = p[1] / l;
         C[1, 1] = -p[0] / l;
         C[2, 2] = 1;
         Vector3d cl = new Vector3d(new double[] { 0.5 * l, Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) * -Sign, 0 });
         Vector3d c = C.Inverse() * cl + StartPoint.ToVector3d();
         Center = new Point3d(c.ToArray());
         Angle = 2 * Math.Acos(Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) / Radius);
         Angle0 = 0.5 * Math.PI - 0.5 * Angle;
         Length = Radius * Angle;
         Bulge = Math.Tan(0.25 * Angle) * Math.Sign(Sign);
      }

      /// <summary>
      /// Получение точки на дуге по заданнамо параметру (на основе параметрического уравнения дуги)
      /// </summary>
      /// <param name="param">Параметр в долях единицы или абсолютное значение в диапазоне от 0 до величины угла дуги</param>
      /// <param name="paramType">Тип параметра (относительный или абсолютный)</param>
      /// <returns>Возвращает вычисленную точку</returns>
      public Point3d GetPoint(double param, ParamType paramType = ParamType.rel)
      {
         if ((param < 0 || param > 1) && paramType == ParamType.rel) return null;
         if ((param < 0 || param > Angle) && paramType == ParamType.abs) return null;
         Vector3d p = EndPoint - StartPoint;
         double l = Math.Sqrt(p[0] * p[0] + p[1] * p[1]);
         p[0] = p[0] / l;
         p[1] = p[1] / l;
         Vector3d n = new Vector3d(new double[] { p[1], -p[0], 0 });

         if (paramType == ParamType.rel) param = Angle * param;
         Point3d res = Center + Sign * Radius * Math.Cos(Angle0 + param) * p + Sign * Radius * Math.Sin(Angle0 + param) * n;
         return res;
      }

      /// <summary>
      /// Деление дуги на равные участки по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки дуги в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки дуги в результат деления.</param>
      /// <remarks>
      /// В качестве шага деления с абсолютным значением следует задавать значение части длины дуги.
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
            step = Angle * param;
            range = new Range(0, Angle);
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
      /// Деление дуги на равные участки по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки дуги в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки дуги в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true)
      {
         if (nDiv <= 0) nDiv = 1;
         Range range = new Range(0, Angle);
         Vector vector = range.GetVectorByNumber(nDiv, start, end);
         List<Point3d> pts = new List<Point3d>(vector.N);
         for (int i = 0; i < vector.N; i++) pts.Add(GetPoint(vector[i], ParamType.abs));

         return new Pline2d(pts.ToArray());
      }
   }

   public enum ParamType { rel, abs }
}