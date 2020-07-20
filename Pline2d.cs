using System.Collections.Generic;

namespace Geo
{
   public class Pline2d
    {
        protected List<Point3d> pts;
        protected List<Line2d> segs;
        protected BoundingBox2d bb;

        public List<Point3d> Points { get => pts; internal set { pts = value; CalcBB(); CalcSegs(); } }
        public BoundingBox2d BoundingBox { get => bb; }
        public List<Line2d> Segments { get => segs; }

        public Pline2d()
        {
            pts = new List<Point3d>();
            segs = new List<Line2d>();
        }

        public Pline2d(List<Point2d> points)
        {
            segs = new List<Line2d>();
            pts = new List<Point3d>();
            if (points.Count > 0)
            {
                foreach (Point2d item in points)
                {
                    pts.Add(item.ToPoint3d());
                }
            }
            CalcBB();
            CalcSegs();
        }

        public Pline2d(List<Point3d> points)
        {
            segs = new List<Line2d>();
            pts = points;
            CalcBB();
            CalcSegs();
        }

        protected virtual void AddPoint(Point2d pt)
        {
            pts.Add(pt.ToPoint3d());
            CalcBB();
            CalcSegs();
        }

        protected void CalcBB()
        {
            double minX = 1000000000;
            double minY = 1000000000;
            double maxX = -1000000000;
            double maxY = -1000000000;

            if (pts.Count > 0)
            {
                foreach (Point3d item in pts)
                {
                    if (item.X < minX) { minX = item.X; }
                    if (item.Y < minY) { minY = item.Y; }
                    if (item.X > maxX) { maxX = item.X; }
                    if (item.Y > maxY) { maxY = item.Y; }
                }

                bb = new BoundingBox2d(new Point2d(minX, minY), new Point2d(maxX, maxY));
            }
        }

        protected void CalcSegs()
        {
            if (pts.Count > 1)
            {
                segs = new List<Line2d>();

                for (int i = 1; i < pts.Count; i++)
                {
                    segs.Add(new Line2d(pts[i - 1], pts[i]));
                }

                if (pts.Count > 2) { segs.Add(new Line2d(pts[pts.Count - 1], pts[0])); }
            }
        }

    }
}
