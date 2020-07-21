using Geo.Calc;
using System;
using System.Collections.Generic;

namespace Geo
{
   [Serializable]
   public class Point3d : ICoordinates
   {
      double[] arr;
      //int n = 3;

      public double this[int i] { get => arr[i]; set => arr[i] = value; }
      public double X { get => arr[0]; set => arr[0] = value; }
      public double Y { get => arr[1]; set => arr[1] = value; }
      public double Z { get => arr[2]; set => arr[2] = value; }
      public Dictionary<string, object> Attr { get; set; }

      public int N => 3;


      //int N { get => n; }

      public Point3d()
      {
         arr = new double[3];
      }

      public Point3d(double x, double y, double z)
      {
         arr = new double[3];
         arr[0] = x; arr[1] = y; arr[2] = z;
      }

      public Point3d(double x, double y)
      {
         arr = new double[3];
         arr[0] = x; arr[1] = y; arr[2] = 0;
      }

      public Point3d(Point3d source)
      {
         arr = new double[3];
         arr = (double[])source.arr.Clone();
      }

      public Point3d(double[] source)
      {
         arr = new double[3];
         if (source.Length >= 3) { arr[0] = source[0]; arr[1] = source[1]; arr[2] = source[2]; }
         else if (source.Length == 2) { arr[0] = source[0]; arr[1] = source[1]; }
         else { arr[0] = source[0]; }
      }

      public Point3d(Vector3d source)
      {
         arr = source.ToArray();
      }

      public double[] ToArray()
      {
         return new double[] { X, Y, Z };
      }

      public Vector3d ToVector3d()
      {
         return new Vector3d { Vx = X, Vy = Y, Vz = Z };
      }

      public Point2d ToPoint2d()
      {
         return new Point2d { X = X, Y = Y };
      }

      public Point3d FromArray(double[] arr)
      {
         X = arr[0]; Y = arr[1]; Z = arr[2];
         return this;
      }

      public double LengthTo(Point3d node)
      {
         Line3d line = new Line3d(this, node);
         return line.Length;
      }

      public double AngleTo(Point3d startPoint, Point3d endPoint)
      {
         Point3d tempNode = new Point3d();

         Line3d tempLine1 = new Line3d(this, startPoint);
         Line3d tempLine2 = new Line3d(this, endPoint);
         double cosTo = Vector3d.CosAngleBetVectors(tempLine1.Directive, tempLine2.Directive);
         return RadToDeg(System.Math.Acos(cosTo));
      }

      public bool NormalDirection(Point3d startNode, Point3d endNode)
      {
         Line3d tempLine1 = new Line3d(this, startNode);
         Line3d tempLine2 = new Line3d(this, endNode);
         double norm = (tempLine1.Directive ^ tempLine2.Directive).Vz;
         bool res = false;
         if (norm > 0) res = true;
         return res;
      }

      private double RadToDeg(double radians)
      {
         return radians * 180 / System.Math.PI;
      }

      public static Point3d operator *(Point3d pt, double prime)
      {
         return new Point3d
         {
            X = pt.X * prime,
            Y = pt.Y * prime,
            Z = pt.Z * prime
         };
      }

      public static Vector3d operator -(Point3d pt1, Point3d pt2)
      {
         Vector3d res = new Vector3d();
         res[0] = pt1.X - pt2.X;
         res[1] = pt1.Y - pt2.Y;
         res[2] = pt1.Z - pt2.Z;
         return res;
      }

      public static Point3d operator +(Point3d pt1, Vector3d v2)
      {
         Point3d res = new Point3d();
         res[0] = pt1.X + v2[0];
         res[1] = pt1.Y + v2[1];
         res[2] = pt1.Z + v2[2];
         return res;
      }

      public static Point3d operator -(Point3d pt1, Vector3d v2)
      {
         Point3d res = new Point3d();
         res[0] = pt1.X - v2[0];
         res[1] = pt1.Y - v2[1];
         res[2] = pt1.Z - v2[2];
         return res;
      }

      //public static bool operator ==(Point3d p1, Point3d p2)
      //{
      //   bool check = false;
      //   if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
      //   {
      //      check = true;
      //   }
      //   return (check);
      //}

      //public static bool operator !=(Point3d p1, Point3d p2)
      //{
      //   bool check = true;
      //   if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
      //   {
      //      check = false;
      //   }
      //   return (check);
      //}

      public bool IsEqual(ICoordinates pt)
      {
         bool check = false;
         if (X == pt.X && Y == pt.Y && Z == pt.Z)
         {
            check = true;
         }
         return (check);
      }

      public bool NotEqual(ICoordinates pt)
      {
         bool check = false;
         if (X != pt.X || Y != pt.Y || Z != pt.Z)
         {
            check = true;
         }
         return (check);
      }
   }
}
