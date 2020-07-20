using System;
using System.Collections.Generic;

namespace Geo
{
   public class Contour2d : Pline2d
    {
        bool closed;
        double area;
        double perimeter;
        Point2d centroid;
        double ix;
        double iy;

        public bool Closed { get => closed; set => closed = value; }
        public double Area { get => Math.Abs(area);}
        public double Perimeter { get => perimeter; }
        public Point2d Centroid { get => centroid;}
        public double Ix { get => Math.Abs(ix);}
        public double Iy { get => Math.Abs(iy);}

        public Contour2d(bool closed = true)
        {
            this.closed = closed;
            Points = new List<Point3d>();
        }

        public Contour2d(List<Point2d> points, bool closed = true)
        {
            this.closed = closed;
            Points = new List<Point3d>();
            if (points.Count > 0)
            {
                foreach (Point2d item in points)
                {
                    Points.Add(item.ToPoint3d());
                }
            }
            CalcSegs();
            CalcPerimeter();
            CalcArea();
            CalcCentroid();
            CalcI();
            CalcBB();
        }

        override protected void AddPoint(Point2d pt)
        {
            base.AddPoint(pt);
            CalcSegs();
            CalcPerimeter();
            CalcArea();
            CalcCentroid();
            CalcI();
            CalcBB();
        }

        virtual protected void AddNewPoints(List<Point2d> pts)
        {
            Points = new List<Point3d>();
            if (pts.Count > 0)
            {
                foreach (Point2d item in pts)
                {
                    Points.Add(item.ToPoint3d());
                }
            }
            CalcSegs();
            CalcPerimeter();
            CalcArea();
            CalcCentroid();
            CalcI();
            CalcBB();
        }

        public void RecalcToCentroid()
        {
            CalcArea();
            CalcCentroid();
            List<Point3d> temp = new List<Point3d>();
            foreach (Point3d item in Points)
            {
                temp.Add(new Point3d(item.X - centroid.X, item.Y - centroid.Y,item.Z));
            }
            Points = temp;
            //CalcCentroid();
            CalcSegs();
            CalcI();
            CalcBB();
        }

        protected void CalcI()
        {
            if (closed && Segments.Count > 2)
            {
                double tempX = 0;
                double tempY = 0;
                for (int i = 0; i < Segments.Count; i++)
                {
                    Point3d arrTemp = Segments[i].StartPoint; Point3d arrTemp1 = Segments[i].EndPoint;
                    tempX = tempX + (Math.Pow(arrTemp.X, 2) + arrTemp.X * arrTemp1.X + Math.Pow(arrTemp1.X, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
                    tempY = tempY + (Math.Pow(arrTemp.Y, 2) + arrTemp.Y * arrTemp1.Y + Math.Pow(arrTemp1.Y, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
                }
                ix = tempX/12;
                iy = tempY/12;
            }
        }

        protected void CalcCentroid()
        {
            if (closed && Segments.Count > 2)
            {
                Point2d temp = new Point2d();
                for (int i = 0; i < Segments.Count; i++)
                {
                    Point3d arrTemp = Segments[i].StartPoint; Point3d arrTemp1 = Segments[i].EndPoint;
                    temp.X = temp.X + 1 / (6 * area) * (arrTemp.X + arrTemp1.X) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
                    temp.Y = temp.Y + 1 / (6 * area) * (arrTemp.Y + arrTemp1.Y) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
                }
                centroid = temp;
            }
        }

        protected void CalcArea()
        {
            double temp = 0;
            if (closed && Segments.Count > 2)
            {
                for (int i = 0; i < Segments.Count; i++)
                {
                    Point3d arrTemp = Segments[i].StartPoint; Point3d arrTemp1 = Segments[i].EndPoint;
                    temp = temp + 0.5 * (arrTemp.X * arrTemp1.Y - arrTemp1.X * arrTemp.Y);
                }
                area = temp;
            }
        }

        protected void CalcPerimeter()
        {
            if (closed && Segments.Count > 2)
            {
                perimeter = 0;
                foreach (Line2d item in Segments)
                {
                    perimeter = perimeter + item.Directive.Norma;
                }
            }
            else if(closed==false)
            {
                perimeter = 0;
                for (int i = 0; i < Segments.Count-1; i++)
                {
                    perimeter = perimeter + Segments[i].Directive.Norma;
                }
            }
            else if (Segments.Count < 3)
            {
                perimeter = 0;
                for (int i = 0; i < Segments.Count; i++)
                {
                    perimeter = perimeter + Segments[i].Directive.Norma;
                }
            }
        }
    }
}
