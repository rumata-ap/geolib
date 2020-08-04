using Geo.Calc;

using System;

namespace Geo
{
   [Serializable]
   public class Point2d
   {
      double[] arr = new double[2];

      public double this[int i] { get => arr[i]; set => arr[i] = value; }
      public double X { get => arr[0]; set => arr[0] = value; }
      public double Y { get => arr[1]; set => arr[1] = value; }

      public Point2d()
      {
         arr = new double[2];
      }

      public Point2d(double x, double y)
      {
         arr = new double[2];
         arr[0] = x; arr[1] = y;
      }

      public Point2d(Point2d source)
      {
         arr = new double[2];
         arr = (double[])source.arr.Clone();
      }

      public Point2d(Point3d source)
      {
         arr = new double[2];
         arr[0] = source.X; arr[1] = source.Y;
      }

      public Point2d(double[] source)
      {
         arr = new double[2];
         if (source.Length >= 2)
         {
            arr[0] = source[0]; arr[1] = source[1];
         }
      }

      public double[] ToArray()
      {
         return new double[] { X, Y };
      }

      public Vector2d ToVector2d()
      {
         return new Vector2d { Vx = X, Vy = Y };
      }

      public Vector3d ToVector3d()
      {
         return new Vector3d { Vx = X, Vy = Y, Vz = 0 };
      }

      public Point3d ToPoint3d()
      {
         return new Point3d { X = X, Y = Y, Z = 0 };
      }

      public Point2d FromArray(double[] arr)
      {
         X = arr[0]; Y = arr[1];
         return this;
      }

      public double LengthTo(Point2d node)
      {
         Line2d line = new Line2d(this, node);
         return line.Length;
      }

      public static Vector2d operator -(Point2d pt1, Point2d pt2)
      {
         Vector2d res = new Vector2d();
         res[0] = pt1.X - pt2.X;
         res[1] = pt1.Y - pt2.Y;
         return res;
      }

      public static Vector2d operator +(Point2d pt1, Point2d pt2)
      {
         Vector2d res = new Vector2d();
         res[0] = pt1.X + pt2.X;
         res[1] = pt1.Y + pt2.Y;
         return res;
      }

      public static Vector2d operator *(Point2d pt1, Point2d pt2)
      {
         Vector2d res = new Vector2d();
         res[0] = pt1.X * pt2.X;
         res[1] = pt1.Y * pt2.Y;
         return res;
      }

      public static bool operator ==(Point2d p1, Point2d p2)
      {
         bool check = false;
         if (p1.X == p2.X && p1.Y == p2.Y)
         {
            check = true;
         }
         return (check);
      }

      public static bool operator !=(Point2d p1, Point2d p2)
      {
         bool check = true;
         if (p1.X == p2.X && p1.Y == p2.Y)
         {
            check = false;
         }
         return (check);
      }

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }
}
