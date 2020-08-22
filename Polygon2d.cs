using Geo.Calc;
using Geo.Triangulation;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Geo
{
   public class Polygon2d : Pline2d
   {
      double area;
      double perimeter;
      Point2d centroid;
      double ix;
      double iy;

      public double Area { get => Math.Abs(area); }
      public double Perimeter { get => perimeter; }
      public Point2d Centroid { get => centroid; }
      public double Ix { get => Math.Abs(ix); }
      public double Iy { get => Math.Abs(iy); }

      public Polygon2d(IEnumerable<ICoordinates> vertices) : base(vertices)
      {
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public Polygon2d(List<Vertex2d> vertices) : base(vertices)
      {
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public Polygon2d(Pline2d pline)
      {
         Vertices = pline.Copy().Vertices;
         Close();

         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      public void RecalcToCentroid()
      {
         CalcArea();
         CalcCentroid();
         List<Vertex2d> temp = new List<Vertex2d>();
         foreach (Vertex2d item in Vertices)
         {
            temp.Add(new Vertex2d(item.X - centroid.X, item.Y - centroid.Y));
         }
         Vertices = temp;
         //CalcCentroid();

         CalcI();
         CalcBB();
      }

      protected void CalcI()
      {
         double tempX = 0;
         double tempY = 0;
         Open();
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            tempX = tempX + (Math.Pow(arrTemp.X, 2) + arrTemp.X * arrTemp1.X + Math.Pow(arrTemp1.X, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            tempY = tempY + (Math.Pow(arrTemp.Y, 2) + arrTemp.Y * arrTemp1.Y + Math.Pow(arrTemp1.Y, 2)) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
         }
         ix = tempX / 12;
         iy = tempY / 12;
         Close();
      }

      protected void CalcCentroid()
      {
         Open();
         ICoordinates temp = new Point3d();
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            temp.X = temp.X + 1 / (6 * area) * (arrTemp.X + arrTemp1.X) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
            temp.Y = temp.Y + 1 / (6 * area) * (arrTemp.Y + arrTemp1.Y) * (arrTemp.X * arrTemp1.Y - arrTemp.Y * arrTemp1.X);
         }
         centroid = new Point2d(temp.X, temp.Y);
         Close();
      }

      protected void CalcArea()
      {
         Open();
         double temp = 0;
         for (int i = 0; i < vrtxs.Count - 1; i++)
         {
            ICoordinates arrTemp = vrtxs[i]; ICoordinates arrTemp1 = vrtxs[i + 1];
            temp = temp + 0.5 * (arrTemp.X * arrTemp1.Y - arrTemp1.X * arrTemp.Y);
         }
         area = temp;
         Close();
      }

      protected void CalcPerimeter()
      {
         Open();
         int count = GetSegsCount();
         if (count < 1) return;
         List<double> segs = new List<double>(count);
         for (int i = 0; i < count; i++)
         {
            segs.Add(GetSegment(i).Length);
         }
         perimeter = segs.Sum();
         Close();
      }

      /// <summary>
      /// Деление контура на сегменты путем деления исходных сегментов по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <remarks>
      /// В качестве шага деления с абсолютным значением следует задавать значение части длины отрезка.
      /// </remarks>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public override Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, true, true));
         }
         return new Polygon2d(res);
      }

      /// <summary>
      /// Деление контура на сегменты путем деления исходных сегментов по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public override Pline2d TesselationByNumber(int nDiv)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            res.AddVertices(GetSegment(i).TesselationByNumber(nDiv, true, true));
         }
         return new Polygon2d(res);
      }

      public Polygon2d Partition(Vertex2d vrt1, Vertex2d vrt2)
      {
         List<Vertex2d> original = new List<Vertex2d>(vrtxs);
         List<Vertex2d> newl = new List<Vertex2d>
         {
            Vertex2d.Copy(vrt1)
         };

         Vertex2d item = vrt1;
         while (!item.Next.Equals(vrt2))
         {
            item = item.Next;
            newl.Add(item);
            original.Remove(item);
         }
         original.Remove(vrt2);

         vrtxs = original;
         CalcVertices();
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();

         return new Polygon2d(newl);
      }

      public void Partition(Vertex2d vrt1, Vertex2d vrt2, out Polygon2d res)
      {
         res = Partition(vrt1, vrt2);
      }

      public void Insert(Vertex2d main, Vertex2d insert, Polygon2d insertPoly)
      {
         List<Vertex2d> origin = Break(main).Vertices;
         List<Vertex2d> newl = insertPoly.Break(insert).Vertices;
         //newl[0].Prev = origin[i1 - 1].Prev;
         //newl[i2 - 1].Next = origin[0].Next;
         origin.RemoveAt(origin.Count - 1);
         origin.RemoveAt(0);
         newl.Reverse();
         origin.AddRange(newl);
         ReplaceVertices(origin);
         Close();
         CalcPerimeter();
         CalcArea();
         CalcCentroid();
         CalcI();
      }

      /// <summary>
      /// Разрыв полигона в указанной вершине.
      /// </summary>
      /// <param name="point">Вершина, в которой будет выполнен разрыв.</param>
      /// <returns>Возвращает плоскую разомкнутую полилинию.</returns>
      public Pline2d Break(Vertex2d point)
      {
         List<Vertex2d> newl = new List<Vertex2d>(vrtxs.Count + 1)
         {
            Vertex2d.Copy(point)
         };
         Vertex2d item = point;
         while (!item.Next.Equals(point))
         {
            item = item.Next;
            newl.Add(item);
         }
         newl.Add(point);

         return new Pline2d(newl);
      }

      /// <summary>
      /// Определение максимального угла полигона.
      /// </summary>
      /// <returns>Возвращает величину максимального угла в градусах.</returns>
      public double MaxAngleDeg()
      {
         var sel = from i in vrtxs orderby i.AngleDeg select i;
         return sel.Last().AngleDeg;
      }

      /// <summary>
      ///Получение вершины с минимальным углом.
      /// </summary>
      public Vertex2d GetMinAngleVert()
      {
         var sel = from i in vrtxs orderby i.AngleDeg select i;
         return sel.First();
      }

      /// <summary>
      ///Получение списка вершин с нулевым углом.
      /// </summary>
      public Vertex2d GetNullAngleVert()
      {
         var sel = from i in vrtxs orderby i.AngleDeg select i;
         if (Math.Round(sel.First().AngleDeg, 4) == 0)
         {
            return sel.First();
         }
         else return null;
      }

      /// <summary>
      ///Проверка попадания узла контура в треугольник, образованный смежным углом в вершине.
      /// </summary>
      /// <param name="v"></param>
      /// <param name="o"></param>
      /// <returns></returns>
      public bool CheckInTria(Vertex2d v, out Vertex2d o)
      {
         Triangle tria = new Triangle(v.Next, v, v.Prev);
         List<Vertex2d> sect = new List<Vertex2d>(vrtxs);
         sect.Remove(v.Next);
         sect.Remove(v);
         sect.Remove(v.Prev);

         var sel = from i in sect where Vertex2d.GetAngleDeg(v.Prev, v, i) < v.AngleDeg select i;
         sel = from i in sel where Vertex2d.GetAngleDeg(v.Prev, v, i) > 0 orderby i.LengthTo(v) select i;
         o = sel.First();
         return tria.IsPointIn(o);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="v"></param>
      /// <param name="o"></param>
      /// <param name="inTria"></param>
      /// <returns></returns>
      public bool CheckIn(Vertex2d v, out Vertex2d o, out bool inTria)
      {
         List<Vertex2d> sect = new List<Vertex2d>(vrtxs);
         sect.Remove(v.Next);
         sect.Remove(v);
         sect.Remove(v.Prev);

         var sel = from i in sect where Math.Round(Vertex2d.GetAngleDeg(v.Prev, v, i), 3) < Math.Round(v.AngleDeg, 3) select i;
         sel = from i in sel where Math.Round(Vertex2d.GetAngleDeg(v.Prev, v, i), 3) > 0 orderby i.LengthTo(v) select i;

         if (sel.Count() > 0) o = sel.First();
         else o = null;

         //double l = o.LengthTo(v);
         //double a = Vertex2d.GetAngleDeg(v.Prev, v, o);

         if (o != null)
         {
            Triangle tria = new Triangle(v.Prev, v, v.Next);
            inTria = tria.IsPointIn(o);
         }
         else inTria = false;

         double maxL = Math.Max(v.LengthTo(v.Prev), v.LengthTo(v.Next));
         if (o != null && (o.LengthTo(v) < maxL || o.LengthTo(v) < maxL)) return true;
         else return false;
      }

      /// <summary>
      /// Проверка на самопересечение по выбранной вершине.
      /// </summary>
      /// <param name="v"> Выбранная вершина.</param>
      /// <returns></returns>
      public bool CheckIntersect(Vertex2d v)
      {
         bool res = false;
         List<ICurve2d> ds = GetAllSegments();
         if (ds.Count > 0)
         {
            for (int i = 0; i < ds.Count; i++)
            {
               if (v.IsMatch(ds[i].StartPoint) || v.IsMatch(ds[i].EndPoint)) continue;
               Line2d ln = new Line2d(v, v.Next);
               Line2d lp = new Line2d(v, v.Prev);
               Line2d lch = (Line2d)ds[i];
               lp.IntersectionSegments(lch, out IntersectResult res1);
               ln.IntersectionSegments(lch, out IntersectResult res2);
               if(res1.res || res2.res)
               {
                  res = true;
                  break;
               }
            }
         }
         return res;
      }

      /// <summary>
      /// Проверка на пересечение контура с треугольником.
      /// </summary>
      /// <param name="v"> Выбранная вершина.</param>
      /// <returns></returns>
      public bool CheckIntersect(Triangle triangle, out ICurve2d edge)
      {
         edge = null;
         bool res = false;
         List<ICurve2d> ds = GetAllSegments();
         if (ds.Count > 0)
         {
            for (int i = 0; i < ds.Count; i++)
            {
               if (triangle.Intersect(ds[i], out ICoordinates[] pts))
               {
                  res = true;
                  edge = ds[i];
                  break;
               }
            }
         }
         return res;
      }

      /// <summary>
      /// Определение минимального расстояния по грани контура в заданной вершине.
      /// </summary>
      /// <param name="v"> Выбранная вершина.</param>
      /// <returns></returns>
      public double MinLengthSeg(Vertex2d v)
      {
         //List<ICurve2d> ds = GetAllSegments();
         //var sel = from i in ds orderby GetLengthToSeg(v, i) select i;       
         List<ICurve2d> ds = GetAllSegments();
         //List<ICurve2d> lcs = new List<ICurve2d>(ds);
         //if (ds.Count > 0)
         //{
         //   foreach (ICurve2d item in ds)
         //   {
         //      if (v.IsMatch(item.StartPoint) || v.IsMatch(item.EndPoint)) lcs.Remove(item);
         //   }
         //}
         var sel = from i in ds orderby GetLengthToSeg(v, i) select i;
         Line2d line = (Line2d)sel.First();
         return Math.Abs(line.A * v.X + line.B * v.Y + line.C) / Math.Sqrt(line.A * line.A + line.B * line.B);
      }

      double GetLengthToSeg(Vertex2d v, ICurve2d lin)
      {

         Line2d line = (Line2d)lin;
         return v.LengthTo(line.CenterPoint);
         //return Math.Abs(line.A * v.X + line.B * v.Y + line.C) / Math.Sqrt(line.A * line.A + line.B * line.B);
      }

      /// <summary>
      /// Триангуляция полигона по заданной величине шага.
      /// </summary>
      /// <param name="step"></param>
      /// <param name="stepType"></param>
      /// <returns></returns>
      public Mesh Triangulation(double step, ParamType stepType = ParamType.abs)
      {
         Mesh res = new Mesh();
         res.Out = new List<Node>(vrtxs.Count);
         Polygon2d poly = (Polygon2d)TesselationByStep(step, stepType);
         //Polygon2d master = (Polygon2d)TesselationByStep(step, stepType);
         for (int i = 0; i < poly.Vertices.Count; i++)
         {
            res.Out.Add(new Node(poly.Vertices[i], NodeType.border) { Id = i + 1 });
            poly.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(poly);


         int it = 1;
         int jn = res.Out.Count + 1;
         while (work.Count > 0)
         {
            poly = work.Pop();
            while (poly.Vertices.Count > 4)
            {
               Vertex2d v = poly.GetMinAngleVert();
               bool inquad = poly.CheckIn(v, out Vertex2d o, out bool intria);

               if (intria)
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                  it++;
                  res.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  work.Push(poly.Partition(o, v));
               }
               else if (!intria && inquad)
               {
                  Polygon2d t1 = new Polygon2d(new List<Vertex2d> { v, v.Next, o });
                  Polygon2d t2 = new Polygon2d(new List<Vertex2d> { v, o, v.Prev });
                  Polygon2d t3 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Prev });
                  Polygon2d t4 = new Polygon2d(new List<Vertex2d> { v.Next, o, v.Prev });
                  if (Math.Max(t1.MaxAngleDeg(), t2.MaxAngleDeg()) <= Math.Max(t3.MaxAngleDeg(), t4.MaxAngleDeg()))
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
                  else
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Next.Nref, o.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
                  work.Push(poly.Partition(o, v));
                  //if (poly.Vertices.Count > 6) work.Push(poly.Partition(o, v));
                  //else
                  //{
                  //   poly.RemoveVertex(v, false);
                  //   //poly.RemoveVertex(v.Next, false);
                  //   poly.RemoveVertex(v.Prev, false);
                  //   poly.CalcVertices();
                  //   poly.Open();
                  //   poly.Close();
                  //}
               }
               else if (v.AngleDeg <= 90)
               {
                  res.Simplexs.Add(new Tri(v.Prev.Nref, v.Nref, v.Next.Nref) { Id = it });
                  it++;
                  poly.RemoveVertex(v);
                  poly.Open();
                  poly.Close();
               }
               else if (v.AngleDeg > 90)
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  //double ml = 0.5 * (v1.Norma + v2.Norma);
                  double ml = Math.Min(v1.Norma, v2.Norma);
                  Vector2d v3 = v.GetBisector() * ml;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (poly.CheckIntersect(v))
                  {
                     v.X = x;
                     v.Y = y;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     poly.RemoveVertex(v);
                     poly.Open();
                     poly.Close();
                  }
                  else
                  {
                     res.Nodes.Add(node);
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     poly.Open();
                     poly.Close();
                  }

                  //Vector3d v1 = v.Prev - v;
                  //Vector3d v2 = v.Next - v;
                  //double ml = 0.5 * (v1.Norma + v2.Norma);
                  //Vector2d v3 = v.GetBisector() * ml;
                  //Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  //Triangle tr = new Triangle(v, v.Next, node);
                  //double x = v.X;
                  //double y = v.Y;

                  //if (poly.CheckIntersect(tr, out ICurve2d seg))
                  //{
                  //   Line2d line = new Line2d(v.Prev, v.Next);
                  //   node = new Node(line.CenterPoint, NodeType.interior) { Id = jn };
                  //   res.Nodes.Add(node);
                  //   res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                  //   it++;
                  //   res.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                  //   it++;

                  //   v.X = node.X;
                  //   v.Y = node.Y;
                  //}
                  //else
                  //{
                  //   res.Nodes.Add(node);
                  //   res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                  //   it++;
                  //   res.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                  //   it++;

                  //   v.X = node.X;
                  //   v.Y = node.Y;                   
                  //}

                  //poly.Open();
                  //poly.Close();
                  //v.Nref = jn;
                  //jn++;
               }
            }
            if (poly.Vertices.Count == 4)
            {
               var sel = from i in poly.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();

               Polygon2d t1 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Next.Next });
               Polygon2d t2 = new Polygon2d(new List<Vertex2d> { v, v.Next.Next, v.Prev });
               Polygon2d t3 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Prev });
               Polygon2d t4 = new Polygon2d(new List<Vertex2d> { v.Next, v.Next.Next, v.Prev });
               if (Math.Max(t1.MaxAngleDeg(), t2.MaxAngleDeg()) <= Math.Max(t3.MaxAngleDeg(), t4.MaxAngleDeg()))
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Next.Next.Nref) { Id = it });
                  it++;
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Next.Nref, v.Prev.Nref) { Id = it });
                  it++;
               }
               else
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  res.Simplexs.Add(new Tri(v.Next.Nref, v.Next.Next.Nref, v.Prev.Nref) { Id = it });
                  it++;
               }

               poly.RemoveVertex(v);
               poly.RemoveVertex(v.Next);
            }
            if (poly.Vertices.Count == 3)
            {
               res.Simplexs.Add(new Tri(poly.Vertices[0].Nref, poly.Vertices[1].Nref, poly.Vertices[2].Nref) { Id = it });
               it++;
            }
         }

         return res;
      }


      /// <summary>
      /// Триангуляция полигона по заданной величине шага c участками регулярной сетки.
      /// </summary>
      /// <param name="step"></param>
      /// <param name="stepType"></param>
      /// <returns></returns>
      public Mesh TriangulationRegular(double step, ParamType stepType = ParamType.abs)
      {
         Mesh res = new Mesh();
         res.Out = new List<Node>(vrtxs.Count);
         Polygon2d poly = (Polygon2d)TesselationByStep(step, stepType);
         Polygon2d master = (Polygon2d)TesselationByStep(step, stepType);
         for (int i = 0; i < poly.Vertices.Count; i++)
         {
            res.Out.Add(new Node(poly.Vertices[i], NodeType.border) { Id = i + 1 });
            poly.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(poly);


         int it = 1;
         int jn = res.Out.Count + 1;
         while (work.Count > 0)
         {
            poly = work.Pop();
            while (poly.Vertices.Count > 4)
            {
               Vertex2d v = poly.GetMinAngleVert();
               bool inquad = poly.CheckIn(v, out Vertex2d o, out bool intria);

               if (intria)
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                  it++;
                  res.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  work.Push(poly.Partition(o, v));
               }
               else if (!intria && inquad)
               {
                  Polygon2d t1 = new Polygon2d(new List<Vertex2d> { v, v.Next, o });
                  Polygon2d t2 = new Polygon2d(new List<Vertex2d> { v, o, v.Prev });
                  Polygon2d t3 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Prev });
                  Polygon2d t4 = new Polygon2d(new List<Vertex2d> { v.Next, o, v.Prev });
                  if (Math.Max(t1.MaxAngleDeg(), t2.MaxAngleDeg()) <= Math.Max(t3.MaxAngleDeg(), t4.MaxAngleDeg()))
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
                  else
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Next.Nref, o.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
                  work.Push(poly.Partition(o, v));
                  //if (poly.Vertices.Count > 6) work.Push(poly.Partition(o, v));
                  //else
                  //{
                  //   poly.RemoveVertex(v, false);
                  //   //poly.RemoveVertex(v.Next, false);
                  //   poly.RemoveVertex(v.Prev, false);
                  //   poly.CalcVertices();
                  //   poly.Open();
                  //   poly.Close();
                  //}
               }
               else if (v.AngleDeg < 85)
               {
                  res.Simplexs.Add(new Tri(v.Next.Nref, v.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  poly.RemoveVertex(v);
                  poly.Open();
                  poly.Close();
               }
               else if (v.AngleDeg >= 85 && v.AngleDeg <= 90)
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  Vector3d v3 = v1 + v2;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (poly.CheckIntersect(v) || master.MinLengthSeg(v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     poly.RemoveVertex(v);
                     poly.Open();
                     poly.Close();
                  }
                  else
                  {
                     res.Nodes.Add(node);
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Next.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     poly.Open();
                     poly.Close();
                  }

                  //poly.CalcVertices();

                  Vertex2d ds = poly.GetNullAngleVert();
                  if (ds != null)
                  {
                     poly.RemoveVertex(ds, false);
                     if(ds.Prev.IsMatch(ds.Next)) poly.RemoveVertex(ds.Next, false);
                     poly.CalcVertices();
                     poly.Open();
                     poly.Close();
                  }

               }
               else if (v.AngleDeg > 90 && v.AngleDeg <= 95)
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  Vector3d v3 = v1 + v2;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (poly.CheckIntersect(v) || master.MinLengthSeg(v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     poly.RemoveVertex(v);
                     poly.Open();
                     poly.Close();
                  }
                  else
                  {
                     res.Nodes.Add(node);
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     poly.Open();
                     poly.Close();
                  }

                  Vertex2d ds = poly.GetNullAngleVert();
                  if (ds != null)
                  {
                     poly.RemoveVertex(ds, false);
                     if (ds.Prev.IsMatch(ds.Next)) poly.RemoveVertex(ds.Next, false);
                     poly.CalcVertices();
                     poly.Open();
                     poly.Close();
                  }

               }
               else
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  double ml = 0.5 * (v1.Norma + v2.Norma);
                  Vector2d v3 = v.GetBisector() * ml;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (poly.CheckIntersect(v) || master.MinLengthSeg(v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     poly.RemoveVertex(v);
                     poly.Open();
                     poly.Close();
                  }
                  else
                  {
                     res.Nodes.Add(node);
                     //res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     //it++;
                     //res.Simplexs.Add(new Tri(v.Next.Nref, jn, v.Prev.Nref) { Id = it });
                     //it++;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     poly.Open();
                     poly.Close();
                  }

                  Vertex2d ds = poly.GetNullAngleVert();
                  if (ds != null)
                  {
                     poly.RemoveVertex(ds, false);
                     if (ds.Prev.IsMatch(ds.Next)) poly.RemoveVertex(ds.Next, false);
                     poly.CalcVertices();
                     poly.Open();
                     poly.Close();
                  }
               }
            }
            if (poly.Vertices.Count == 4)
            {
               var sel = from i in poly.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();

               if (v.AngleDeg < 90)
               {
                  Polygon2d t1 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Next.Next });
                  Polygon2d t2 = new Polygon2d(new List<Vertex2d> { v, v.Next.Next, v.Prev });
                  Polygon2d t3 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Prev });
                  Polygon2d t4 = new Polygon2d(new List<Vertex2d> { v.Next, v.Next.Next, v.Prev });
                  if (Math.Max(t1.MaxAngleDeg(), t2.MaxAngleDeg()) <= Math.Max(t3.MaxAngleDeg(), t4.MaxAngleDeg()))
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Next.Next.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
                  else
                  {
                     res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     res.Simplexs.Add(new Tri(v.Next.Nref, v.Next.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                  }
               }

               poly.RemoveVertex(v);
               poly.RemoveVertex(v.Next);
            }
            if (poly.Vertices.Count == 3)
            {
               res.Simplexs.Add(new Tri(poly.Vertices[0].Nref, poly.Vertices[1].Nref, poly.Vertices[2].Nref) { Id = it });
               it++;
            }
         }

         return res;
      }

      public Mesh TriangulationSimple()
      {
         Mesh res = new Mesh();
         res.Out = new List<Node>(vrtxs.Count);
         Polygon2d poly = new Polygon2d(vrtxs);
         for (int i = 0; i < vrtxs.Count; i++)
         {
            res.Out.Add(new Node(vrtxs[i], NodeType.border) { Id = i + 1 });
            poly.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(poly);

         int cntr = 1;

         while (work.Count > 0)
         {
            poly = work.Pop();

            while (poly.Vertices.Count > 3)
            {
               var sel = from i in poly.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();
               Triangle tria = new Triangle(v.Next, v, v.Prev);

               #region Проверка на самопересечение при генерации треугольника
               List<Vertex2d> sect = new List<Vertex2d>(poly.Vertices);
               sect.Remove(v.Next);
               sect.Remove(v);
               sect.Remove(v.Prev);
               sel = from i in sect orderby i.LengthTo(v) select i;
               if (tria.IsPointIn(sel.First()))
               {
                  res.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, sel.First().Nref) { Id = cntr });
                  cntr++;
                  res.Simplexs.Add(new Tri(v.Nref, sel.First().Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  work.Push(poly.Partition(sel.First(), v));
               }
               #endregion
               else
               {
                  res.Simplexs.Add(new Tri(v.Next.Nref, v.Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  poly.RemoveVertex(v);
               }
            }

            if (poly.Vertices.Count == 3)
            {
               res.Simplexs.Add(new Tri(poly.Vertices[0].Nref, poly.Vertices[1].Nref, poly.Vertices[2].Nref) { Id = cntr });
               cntr++;
            }
         }

         return res;
      }
   }
}
