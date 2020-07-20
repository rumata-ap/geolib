using Geo.Calc;

using System;

namespace Geo
{
   [Serializable]
    public class Line3d
    {
        Point3d startPoint;
        Point3d endPoint;

        public Point3d StartPoint { get => startPoint; set { startPoint = value; Directive = endPoint - startPoint; } }
        public Point3d EndPoint { get => endPoint; set { endPoint = value; Directive = endPoint - startPoint; } }
        public Vector3d Directive { get; private set; }
        public double Length { get => Directive.Norma; }

        public Line3d()
        {

        }

        public Line3d(Point2d startPt, Point2d endPt)
        {
            startPoint = startPt.ToPoint3d(); endPoint = endPt.ToPoint3d();
            Directive = endPoint - startPoint; ;
        }

        public Line3d(Point3d startPt, Point3d endPt)
        {
            startPoint = startPt; endPoint = endPt;
            Directive = endPoint - startPoint; ;
        }
    }
}
