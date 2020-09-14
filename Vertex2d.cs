using Geo.Calc;

using System;

namespace Geo
{
   [Serializable]
   public class Vertex2d : IXYZ
   {
      public int Id { get; set; }
      public int Nref { get; set; }
      public double Bulge { get; set; }
      public Vertex2d Prev { get => prev; set { prev = value; GetAngle(); } }
      public Vertex2d Next { get => next; set { next = value; GetAngle(); } }
      public VertexPosition Pos { get; set; }
      public double Angle { get; private set; }
      public double AngleDeg { get; private set; }
      double x;
      private double z;
      private double y;
      private Vertex2d prev;
      private Vertex2d next;

      public double Y { get => y; set { y = value; GetAngle(); } }
      public double Z { get => 0; set { z = 0; } }
      public double X { get => x; set { x = value; GetAngle(); } }

      public Vertex2d()
      {

      }

      public Vertex2d(IXYZ source)
      {
         X = source.X;
         Y = source.Y;
         Z = source.Z;
         Pos = VertexPosition.Middle;
      }

      public Vertex2d(double x, double y)
      {
         X = x;
         Y = y;
         Z = 0;
         Pos = VertexPosition.Middle;
      }

      public Vertex2d(IXYZ pt, Vertex2d prev = null, Vertex2d next = null, VertexPosition pos = VertexPosition.Middle)
      {
         X = pt.X;
         Y = pt.Y;
         Z = pt.Z;
         Prev = prev;
         Next = next;
         Pos = pos;
         //GetAngle();
      }

      public static Vertex2d Copy (Vertex2d pt)
      {
         return new Vertex2d { X = pt.X, Y = pt.Y, Pos = pt.Pos, Nref = pt.Nref, Bulge = pt.Bulge };
      }

      void GetAngle()
      {
         if (Prev == null || Next == null) return;
         Vector3d pp = Prev - this;
         Vector3d pn = Next - this;
         Vector3d nd = pp ^ pn;
         double cosTo = Vector3d.CosAngleBetVectors(pp, pn);
         Angle = Math.Acos(cosTo);
         AngleDeg = RadToDeg(Math.Acos(cosTo));

         if (nd.Unit[2] < 0)
         {
            Angle = 2 * Math.PI - Angle;
            AngleDeg = 360 - AngleDeg;
         }

         if (double.IsNaN(AngleDeg)) AngleDeg = 180;
      }

      public static double GetAngleDeg(IXYZ vp, IXYZ v, IXYZ vn)
      {
         if (vp == null || vn == null) return 0;
         Vector3d pp = new Vector3d(vp.X - v.X, vp.Y - v.Y, vp.Z - v.Z);
         Vector3d pn = new Vector3d(vn.X - v.X, vn.Y - v.Y, vn.Z - v.Z);
         Vector3d nd = pp ^ pn;
         double cosTo = Vector3d.CosAngleBetVectors(pp, pn);
         double angleDeg = RadToDeg(Math.Acos(cosTo));

         if (nd.Unit[2] < 0)
         {
            angleDeg = 360 - angleDeg;
         }

         return angleDeg;
      }

      public Vector2d GetBisector()
      {
         Vector2d dir1 = new Vector2d(new Vector2d(prev.X - X, prev.Y - Y).Unit);
         Vector2d dir2 = new Vector2d(new Vector2d(next.X - X, next.Y - Y).Unit);

         if (AngleDeg > 180) return new Vector2d((dir1 + dir2).Unit) * -1.0;
         else return new Vector2d((dir1 + dir2).Unit);
      }

      public double[] ToArray()
      {
         return new double[] { x, y, z };
      }

      public Vector3d ToVector3d()
      {
         return new Vector3d(x, y, z);
      }

      public Point3d ToPoint3d()
      {
         return new Point3d(x, y, z);
      }

      public double DistanceTo(IXYZ target)
      {
         double dx = X - target.X;
         double dy = Y - target.Y;
         return Math.Sqrt(dx * dx + dy * dy);
      }

      public bool IsNaN()
      {
         return double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);
      }

      public static Vector3d operator -(Vertex2d pt1, Vertex2d pt2)
      {
         Vector3d res = new Vector3d();
         res[0] = pt1.X - pt2.X;
         res[1] = pt1.Y - pt2.Y;
         res[2] = pt1.Z - pt2.Z;
         return res;
      }
      
      public static Vertex2d operator -(Vertex2d pt1, IXYZ pt2)
      {
         Vertex2d res = new Vertex2d();
         res.X = pt1.X - pt2.X;
         res.Y = pt1.Y - pt2.Y;
         res.Z = pt1.Z - pt2.Z;
         return res;
      }

      public static Point3d operator +(Vertex2d pt1, Vector3d v2)
      {
         Point3d res = new Point3d();
         res[0] = pt1.X + v2[0];
         res[1] = pt1.Y + v2[1];
         res[2] = pt1.Z + v2[2];
         return res;
      }

      public static Point3d operator -(Vertex2d pt1, Vector3d v2)
      {
         Point3d res = new Point3d();
         res[0] = pt1.X - v2[0];
         res[1] = pt1.Y - v2[1];
         res[2] = pt1.Z - v2[2];
         return res;
      }

      static double RadToDeg(double radians)
      {
         return radians * 180 / Math.PI;
      }

      public bool IsMatch(IXYZ pt)
      {
         return Calcs.IsEqual(this, pt);
      }

      public double DistanceSquaredTo(IXYZ target)
      {
         double dx = X - target.X;
         double dy = Y - target.Y;
         double dz = Z - target.Z;
         return dx * dx + dy * dy + dz * dz;
      }
   }

   public enum VertexPosition { Middle, First, Last}
}
