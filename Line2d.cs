using Geo.Calc;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geo
{
   [Serializable]
   public class Line2d
   {
      protected Point3d startPoint;
      protected Point3d endPoint;
      protected Vector2d directive;
      protected Vector2d normal;

      public Point3d StartPoint { get => startPoint; set { startPoint = value; if (EndPoint != null) { directive = endPoint.ToPoint2d() - startPoint.ToPoint2d(); CalcLine(); }; } }
      public Point3d EndPoint { get => endPoint; set { endPoint = value; directive = endPoint.ToPoint2d() - startPoint.ToPoint2d(); CalcLine(); } }
      public Point3d CenterPoint { get; private set; }
      public Vector2d Directive { get => directive; private set => directive = value; }
      public Vector2d Normal { get => normal; private set => normal = value; }
      public double A { get; private set; }
      public double B { get; private set; }
      public double C { get; private set; }
      public double k { get; private set; }
      public double b { get; private set; }
      public double Length { get => Directive.Norma; }
      public double cosAlfa { get; private set; }
      public double cosBeta { get; private set; }
      public double p { get; private set; }

      public Line2d()
      {

      }

      public Line2d(Point2d startPt, Point2d endPt)
      {
         startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }
      
      public Line2d(Vertex2d startPt, Vertex2d endPt)
      {
         startPoint = startPt.Point.ToPoint3d(); endPoint = endPt.Point.ToPoint3d();
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }

      public Line2d(Point3d startPt, Point3d endPt)
      {
         startPoint = startPt;
         endPoint = endPt;
         directive = endPoint.ToPoint2d() - startPoint.ToPoint2d();
         CalcLine();
      }

      protected void CalcLine()
      {
         A = Directive.Vy;
         B = -Directive.Vx;
         normal = new Vector2d(A, B);
         C = Directive.Vx * StartPoint.Y - Directive.Vy * StartPoint.X;
         if (Directive.Vx != 0) { k = Directive.Vy / Directive.Vx; }
         else { k = Double.PositiveInfinity; }
         if (Directive.Vx != 0) { b = -Directive.Vy / Directive.Vx * StartPoint.X + StartPoint.Y; }
         else { b = Double.PositiveInfinity; }
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         double dx = 0.5 * (EndPoint.X - StartPoint.X);
         double dy = 0.5 * (EndPoint.Y - StartPoint.Y);
         CenterPoint = new Point3d(StartPoint.X + dx, StartPoint.Y + dy, 0);
      }

      public Vector2d GetUnitNormal()
      {
         return new Vector2d(cosAlfa, cosBeta);
      }

      public double LengthTo(Point2d point)
      {
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         return normC * (A * point.X + B * point.Y + C);
      }
      public double LengthTo(Point3d point)
      {
         double normC = 1 / Math.Sqrt(A * A + B * B);
         if (C < 0) normC *= -1;
         cosAlfa = A * normC;
         cosBeta = B * normC;
         p = C * normC;
         return normC * (A * point.X + B * point.Y + C);
      }
      public double Interpolation(double x)
      {
         if (B == 0) return double.PositiveInfinity;
         else return (-A * x - C) / B;
      }

      public void Intersection(Line2d line, out IntersectResult res)
      {
         res = new IntersectResult();

         double A1 = this.A;
         double B1 = this.B;
         double C1 = this.C;
         double A2 = line.A;
         double B2 = line.B;
         double C2 = line.C;

         if (A1 == 0 || B2 - (A2 * B1) / A1 == 0) return;

         double y = ((A2 * C1) / A1 - C2) / (B2 - (A2 * B1) / A1);
         double x = (-C1 - B1 * y) / A1;

         res.pts = new List<Point2d>(1);
         res.pts[0] = new Point2d(x, y);
         res.res = true;
      }

      public void IntersectionSegments(Line2d line, out IntersectResult res)
      {
         res = new IntersectResult();
         IntersectResult resl;
         Intersection(line, out resl);
         if (resl.res == false) return;
         Point2d respt = resl.pts[0];
         Range Xrange = new Range(StartPoint.X, EndPoint.X);
         Range Yrange = new Range(StartPoint.Y, EndPoint.Y);
         Range Xrangel = new Range(line.StartPoint.X, line.EndPoint.X);
         Range Yrangel = new Range(line.StartPoint.Y, line.EndPoint.Y);
         if (Xrange.Affiliation(respt.X) && Yrange.Affiliation(respt.Y) && Xrangel.Affiliation(respt.X) && Yrangel.Affiliation(respt.Y))
         {
            res.res = true;
            res.pts = new List<Point2d>(1);
            res.pts[0] = respt;
         }
      }
   }

   [Serializable]
   public struct IntersectResult
   {
      public List<Point2d> pts;
      public bool res;
   }
}
