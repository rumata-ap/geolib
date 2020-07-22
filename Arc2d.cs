using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Geo.Calc;

namespace Geo
{
   [Serializable]
   public class Arc2d
   {
      public Point3d StartPoint { get; private set; }
      public Point3d EndPoint { get; private set; }
      public Point3d Center { get; private set; }
      public double Lenght { get; private set; }
      public double Radius { get; private set; }
      public double Angle { get; private set; }
      public double Angle0 { get; private set; }
      public double Bulge { get; private set; }
      public int Sign { get; private set; }

      public Arc2d(Point3d pt1, Point3d pt2, double r, int sign = -1)
      {
         StartPoint = pt1;
         EndPoint = pt2;
         Radius = r;
         Sign = Math.Sign(sign);
         CalcArc();
      }

      void CalcArc()
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
         Lenght = Radius * Angle;
         Bulge = Math.Atan(0.25 * Angle);
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
      /// 
      /// </summary>
      /// <param name="step"></param>
      /// <param name="stepType"></param>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <returns></returns>
      public Pline2d Tesselation(double step, ParamType stepType = ParamType.rel, bool start=true, bool end = true)
      {
         Range range;
         Vector vector;
         if (stepType == ParamType.abs)
         {
            double param = Lenght / step;
            step = Angle * param;
            range = new Range(0, Angle);
            vector = range.GetVectorByStep(step, true, start, end);
         }
         else
         {
            range = new Range(0, 1);
            vector = range.GetVectorByStepParam(step, start, end);
         }

         List<Point3d> pts = new List<Point3d>(vector.N);
         for (int i = 0; i < vector.N; i++) pts.Add(GetPoint(vector[i], stepType));

         return new Pline2d(pts);
      }
   }

   public enum ParamType { rel, abs}
}
