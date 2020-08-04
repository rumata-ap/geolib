using Geo.Calc;

using System;

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

      public Point3d GetPointByParameter(double parameter)
      {
         return Directive.ToPoint3d() * parameter + startPoint.ToVector3d();
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
   }
}
