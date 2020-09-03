using Geo.Calc;

using System;

using static System.Math;

namespace Geo
{
   [Serializable]
   public class Plane
   {
      double a;
      double b;
      double c;
      double d;
      double gammaRad;
      double gammaDeg;

      public double A { get => a; set { a = value; CalcPlane(); } }
      public double B { get => b; set { b = value; CalcPlane(); } }
      public double C { get => c; set { c = value; CalcPlane(); } }
      public double D { get => d; set { d = value; CalcPlane(); } }
      public double AlfaRad { get; private set; }
      public double BetaRad { get; private set; }
      public double GammaRad { get => gammaRad; set { gammaRad = value; RadToDeg(); } }
      public double AlfaDeg { get; private set; }
      public double BetaDeg { get; private set; }
      public double GammaDeg { get => gammaDeg; set { gammaDeg = value; DegToRad(); } }
      public Matrix MrAlfa { get; private set; }
      public Matrix MrBeta { get; private set; }
      public Matrix MrGamma { get; private set; }
      public Vector3d X1 { get; private set; }
      public Vector3d Y1 { get; private set; }
      public Vector3d Z1 { get; private set; }
      public Vector3d Normal { get; private set; }
      public Matrix Mr { get; private set; }
      public Point3d Basis { get; set; }

      public Plane()
      {
         Basis = new Point3d() { X = 0, Y = 0, Z = 0 };

         a = 0;
         b = 0;
         c = 1;
         d = 0;

         CalcPlane();
      }

      public Plane(ICoordinates pt1, ICoordinates pt2, ICoordinates pt3)
      {
         Basis = new Point3d(pt1);

         Vector3d v1 = pt2.ToVector3d() - pt1.ToVector3d();
         Vector3d v2 = pt3.ToVector3d() - pt1.ToVector3d();

         a = v1.Vy * v2.Vz - v1.Vz * v2.Vy;
         b = v1.Vz * v2.Vx - v1.Vx * v2.Vz;
         c = v1.Vx * v2.Vy - v1.Vy * v2.Vx;
         d = -a * pt1.X - b * pt1.Y - c * pt1.Z;

         Normal = new Vector3d(a, b, c);

         //CalcPlane();
         //RadToDeg();

      }

      public Plane(ICoordinates pt1, ICoordinates pt2, ICoordinates pt3, ICoordinates bpt)
      {
         Basis = new Point3d(bpt);

         Vector3d v1 = pt2.ToVector3d() - pt1.ToVector3d();
         Vector3d v2 = pt3.ToVector3d() - pt1.ToVector3d();

         a = v1.Vy * v2.Vz - v1.Vz * v2.Vy;
         b = v1.Vz * v2.Vx - v1.Vx * v2.Vz;
         c = v1.Vx * v2.Vy - v1.Vy * v2.Vx;
         d = -a * pt1.X - b * pt1.Y - c * pt1.Z;

         Normal = new Vector3d(a, b, c);

         //CalcPlane();
         //RadToDeg();

      }


      public double Interpolation(double x, double y)
      {
         if (C == 0) return double.PositiveInfinity;
         else return (-A * x - B * y - D) / C;
      }

      /// <summary>
      ///Вычисление точки пересечения прямой с плоскостью
      /// </summary>
      /// <param name="line">Линия в пространстве.</param>
      /// <param name="ip">Результирующая точка пересечения.</param>
      /// <returns>TRUE, если пересечение существует.</returns>
      public bool Intersection(Line3d line, out Point3d ip)
      {
         double denom = Normal / (line.EndPoint - line.StartPoint);
         double nom = Normal / (Basis - line.StartPoint);
         if (Calcs.IsZero(denom))
         {
            ip = null;
            return false;
         }
         else
         {
            double u = nom / denom;
            ip = line.StartPoint + (line.EndPoint - line.StartPoint) * u;
            return true;
         }
      }

      protected void CreatePlane(ICoordinates pt1, ICoordinates pt2, ICoordinates pt3)
      {
         Basis = new Point3d(pt1);

         Vector3d v1 = pt2.ToVector3d() - pt1.ToVector3d();
         Vector3d v2 = pt3.ToVector3d() - pt1.ToVector3d();

         a = v1.Vy * v2.Vz - v1.Vz * v2.Vy;
         b = v1.Vz * v2.Vx - v1.Vx * v2.Vz;
         c = v1.Vx * v2.Vy - v1.Vy * v2.Vx;
         d = -a * pt1.X - b * pt1.Y - c * pt1.Z;

         CalcPlane();
         RadToDeg();
      }

      protected void CalcPlane()
      {
         Normal = new Vector3d
         {
            Vx = a,
            Vy = b,
            Vz = c
         };

         Intersect();
         CreateMatrix();
      }

      protected void Intersect()
      {
         if ((a == 0 & b == 0) == false)
         {
            Vector3d v3 = new Vector3d { Vx = 0, Vy = 0, Vz = 1 };
            Z1 = new Vector3d(Normal.Unit);
            X1 = new Vector3d((v3 ^ Z1).Unit);
            Y1 = new Vector3d((Z1 ^ X1).Unit);

            AlfaRad = -Acos(X1.Vx);
            BetaRad = -Acos(Z1.Vz);

            //alfaRad = Math.Acos(unitP.Vx);
            //betaRad = Math.Acos(unitNormal.Vz);
         }
      }

      protected void CreateMatrix()
      {
         Vector3d o = new Vector3d();
         o[0] = -X1.Vx * Basis.X - X1.Vy * Basis.Y - X1.Vz * Basis.Z;
         o[1] = -Y1.Vx * Basis.X - Y1.Vy * Basis.Y - Y1.Vz * Basis.Z;
         o[2] = -Z1.Vx * Basis.X - Z1.Vy * Basis.Y - Z1.Vz * Basis.Z;

         Mr = new Matrix(4, 4);

         Mr[0, 0] = X1.Vx; Mr[0, 1] = X1.Vy; Mr[0, 2] = X1.Vz; Mr[0, 3] = o.Vx;
         Mr[1, 0] = Y1.Vx; Mr[1, 1] = Y1.Vy; Mr[1, 2] = Y1.Vz; Mr[1, 3] = o.Vy;
         Mr[2, 0] = Z1.Vx; Mr[2, 1] = Z1.Vy; Mr[2, 2] = Z1.Vz; Mr[2, 3] = o.Vz;
         Mr[3, 0] = 0; Mr[3, 1] = 0; Mr[3, 2] = 0; Mr[3, 3] = 1;

         MrAlfa = new Matrix(3, 3);

         MrAlfa[0, 0] = Cos(AlfaRad); MrAlfa[0, 1] = -Sin(AlfaRad); MrAlfa[0, 2] = 0;
         MrAlfa[1, 0] = Sin(AlfaRad); MrAlfa[1, 1] = Cos(AlfaRad); MrAlfa[1, 2] = 0;
         MrAlfa[2, 0] = 0; MrAlfa[2, 1] = 0; MrAlfa[2, 2] = 0;

         MrBeta = new Matrix(3, 3);

         MrBeta[0, 0] = 1; MrBeta[0, 1] = 0; MrBeta[0, 2] = 0;
         MrBeta[1, 0] = 0; MrBeta[1, 1] = Cos(BetaRad); MrBeta[1, 2] = -Sin(BetaRad);
         MrBeta[2, 0] = 0; MrBeta[2, 1] = Sin(BetaRad); MrBeta[2, 2] = Cos(BetaRad);

         MrGamma = new Matrix(3, 3);

         MrGamma[0, 0] = Cos(gammaRad); MrGamma[0, 1] = -Sin(gammaRad); MrGamma[0, 2] = 0;
         MrGamma[1, 0] = Sin(gammaRad); MrGamma[1, 1] = Cos(gammaRad); MrGamma[1, 2] = 0;
         MrGamma[2, 0] = 0; MrGamma[2, 1] = 0; MrGamma[2, 2] = 1;
      }

      //public Point3d PointInLCS(Point3d pt)
      //{
      //    double[] d1 = new double[] { 0, 0, 0, 1 }; ;
      //    Vector v1 = new Vector(d1);
      //    v1[0] = pt.X; v1[1] = pt.Y; v1[2] = pt.Z;

      //    return new Point3d().FromArray((Mr * v1).ToArray());
      //}

      protected double[] MultiplyMtxVec(double[,] _Matrix, double[] _Vector)
      {
         int n = _Vector.Length;
         double[] _Result = new double[n];

         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < n; j++)
            {
               _Result[i] += _Matrix[i, j] * _Vector[j];
            }
         }

         return _Result;
      }

      protected void RadToDeg()
      {
         AlfaDeg = AlfaRad * 180 / PI;
         BetaDeg = BetaRad * 180 / PI;
         gammaDeg = gammaRad * 180 / PI;
      }

      protected void DegToRad()
      {
         gammaRad = gammaDeg * 180 / PI;
      }
   }
}
