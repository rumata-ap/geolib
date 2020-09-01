﻿using System;
using static System.Math;

namespace Geo.Calc
{
   [Serializable]
   public class Vector3d
   {
      double[] arr;
      int n = 3;

      public double this[int i] { get => arr[i]; set => arr[i] = value; }
      public double Vx { get => arr[0]; set => arr[0] = value; }
      public double Vy { get => arr[1]; set => arr[1] = value; }
      public double Vz { get => arr[2]; set => arr[2] = value; }
      public double Norma { get => Sqrt(Pow(Vx, 2) + Pow(Vy, 2) + Pow(Vz, 2)); }
      public double[] Unit { get => new double[] { Vx / Norma, Vy / Norma, Vz / Norma }; }
      public int N { get => n; }

      public Vector3d()
      {
         arr = new double[3];
      }

      public Vector3d(Vector3d source)
      {
         arr = new double[3];
         arr = (double[])source.arr.Clone();
      }

      public Vector3d(Point3d source)
      {
         arr = new double[3];
         arr[0] = source.X; arr[1] = source.Y; arr[2] = source.Z;
      }

      public Vector3d(Point3d pt1, Point3d pt2)
      {
         arr = new double[3];
         Vx = pt2.X - pt1.X;
         Vy = pt2.Y - pt1.Y;
         Vz = pt2.Z - pt1.Z;
      }

      public Vector3d(Vector source)
      {
         arr = new double[3];
         if (source.N >= 3)
         {
            arr[0] = source[0]; arr[1] = source[1]; arr[2] = source[2];
         }
      }

      public Vector3d(Vector2d source)
      {
         arr = new double[3];
         arr[0] = source.Vx;
         arr[1] = source.Vy;
         arr[2] = 0;
      }

      public Vector3d(double[] source)
      {
         arr = new double[3];
         if (source.Length >= 3)
         {
            arr[0] = source[0]; arr[1] = source[1]; arr[2] = source[2];
         }
      }

      public Vector3d(double vx,double vy, double vz = 0)
      {
         arr = new double[3];
         arr[0] = vx;
         arr[1] = vy;
         arr[2] = vz;
      }

      public static double CosAngleBetVectors(Vector3d v1, Vector3d v2)
      {
         return (v1.Vx * v2.Vx + v1.Vy * v2.Vy + v1.Vz * v2.Vz) / (Sqrt(v1.Vx * v1.Vx + v1.Vy * v1.Vy + v1.Vz * v1.Vz) * Sqrt(v2.Vx * v2.Vx + v2.Vy * v2.Vy + v2.Vz * v2.Vz));
      }

      public double[] ToArray()
      {
         return arr;
      }

      public Vector2d ToVector2d()
      {
         return new Vector2d() { Vx = Vx, Vy = Vy };
      }

      public Vector ToVector()
      {
         return new Vector(arr);
      }

      public Point3d ToPoint3d()
      {
         return new Point3d(Vx, Vy, Vz);
      }
      
      public Point2d ToPoint2d()
      {
         return new Point2d(Vx, Vy);
      }

      /// <summary>
      /// Нулевой вектор.
      /// </summary>
      public static Vector3d Zero
      {
         get { return new Vector3d(0, 0, 0); }
      }

      /// <summary>
      /// Единичный X вектор.
      /// </summary>
      public static Vector3d UnitX
      {
         get { return new Vector3d(1, 0, 0); }
      }

      /// <summary>
      /// Единичный Y вектор.
      /// </summary>
      public static Vector3d UnitY
      {
         get { return new Vector3d(0, 1, 0); }
      }

      /// <summary>
      /// Единичный Z вектор.
      /// </summary>
      public static Vector3d UnitZ
      {
         get { return new Vector3d(0, 0, 1); }
      }

      public bool IsMatch(Vector3d vector)
      {
         if (Vx == vector.Vx && Vy == vector.Vy && Vz == vector.Vz) return true;
         else return false;
      }
      
      public bool IsMatchAbs(Vector3d vector)
      {
         if (Vx == -vector.Vx && Vy == -vector.Vy && Vz == -vector.Vz) return true;
         else return false;
      }

      /// <summary>
      /// Возвращает скалярное произведение двух векторов.
      /// </summary>
      /// <param name="u">Двумерный вектор.</param>
      /// <param name="v">Двумерный вектор.</param>
      /// <returns>Точечный продукт.</returns>
      public static double operator /(Vector3d u, Vector3d v)
      {
         return u.Vx * v.Vx + u.Vy * v.Vy + u.Vz * v.Vz;
      }

      /// <summary>
      /// Векторное произведение
      /// </summary>
      /// <param name="v1">1-й вектор</param>
      /// <param name="v2">2-й ветор</param>
      /// <returns>Возвращает результат векторного произведения в виде трехмерного вектора</returns>
      public static Vector3d operator ^(Vector3d v1, Vector3d v2)
      {
         return new Vector3d
         {
            Vx = v1.Vy * v2.Vz - v1.Vz * v2.Vy,
            Vy = v1.Vz * v2.Vx - v1.Vx * v2.Vz,
            Vz = v1.Vx * v2.Vy - v1.Vy * v2.Vx
         };
      }

      public static Vector3d operator *(Vector3d v1, Vector3d v2)
      {
         return new Vector3d
         {
            Vx = v1.Vx * v2.Vx,
            Vy = v1.Vy * v2.Vy,
            Vz = v1.Vz * v2.Vz
         };
      }

      public static Vector3d operator *(Vector3d v1, double prime)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] * prime;
         }

         return new Vector3d(res);
      }

      public static Vector3d operator *(double prime, Vector3d v1)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] * prime;
         }

         return new Vector3d(res);
      }

      public static Vector3d operator +(Vector3d v1, Vector3d v2)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] + v2[i];
         }

         return new Vector3d(res);
      }

      public static Vector3d operator -(Vector3d v1, Vector3d v2)
      {
         double[] res = new double[3];
         for (int i = 0; i < 3; i++)
         {
            res[i] = v1[i] - v2[i];
         }

         return new Vector3d(res);
      }
   }
}
