using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Geo.Calc;

namespace Geo
{
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

      public Arc2d(Point3d pt1, Point3d pt2, double r, int sign)
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
         Vector3d cl = new Vector3d(new double[] { 0.5 * l, Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) * Sign, 0 });
         Vector3d c = C.Inverse() * cl;
         Center = new Point3d(c.ToArray());
         Angle = 2 * Math.Acos(Math.Sqrt(Radius * Radius - (0.5 * l) * (0.5 * l)) / Radius);
         Angle0 = 0.5 * Math.PI - 0.5 * Angle;
         Lenght = Radius * Angle;
         Bulge = Math.Atan(0.25 * Angle);
      }

      public Point3d GetPoint(double param)
      {
         if (param < 0 || param > 1) return null;
         Vector3d p = EndPoint - StartPoint;
         double l = Math.Sqrt(p[0] * p[0] + p[1] * p[1]);
         Matrix C = new Matrix(3, 3);
         C[0, 0] = p[0] / l;
         C[0, 1] = p[1] / l;
      }
   }
}
