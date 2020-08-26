using Geo.Calc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public static class FTriangulator
   {

      /// <summary>
      ///Получение первой вершины полигона с минимальным углом.
      /// </summary>
      static Vertex2d GetMinAngleVert(Polygon2d front)
      {
         var sel = from i in front.Vertices orderby i.AngleDeg select i;
         return sel.First();
      }

      /// <summary>
      ///Получение первой вершины с нулевым углом.
      /// </summary>
      static Vertex2d GetNullAngleVert(Polygon2d front)
      {
         var sel = from i in front.Vertices orderby i.AngleDeg select i;
         if (Math.Round(sel.First().AngleDeg, 4) == 0)
         {
            return sel.First();
         }
         else return null;
      }

      /// <summary>
      ///Проверка попадания узла контура в треугольник, образованный смежным углом в вершине.
      /// </summary>
      /// <param name="front">Проверяемый полигон.</param>
      /// <param name="v">Выбранная вершина для анализа.</param>
      /// <param name="o">Результирующая целевая вершина.</param>
      /// <returns></returns>
      static bool CheckInTria(Polygon2d front, Vertex2d v, out Vertex2d o)
      {
         Triangle tria = new Triangle(v.Next, v, v.Prev);
         List<Vertex2d> sect = new List<Vertex2d>(front.Vertices);
         sect.Remove(v.Next);
         sect.Remove(v);
         sect.Remove(v.Prev);

         var sel = from i in sect where Vertex2d.GetAngleDeg(v.Prev, v, i) < v.AngleDeg select i;
         sel = from i in sel where Vertex2d.GetAngleDeg(v.Prev, v, i) > 0 orderby i.LengthTo(v) select i;
         o = sel.First();
         return tria.IsPointIn(o);
      }

      /// <summary>
      /// Получение расстояния от вершины до средней точки ребра.
      /// </summary>
      /// <param name="v">Выбранная вершина.</param>
      /// <param name="e">Выбранное ребро.</param>
      /// <returns></returns>
      static double GetLengthToSeg(Vertex2d v, ICurve2d e)
      {
         Line2d line = (Line2d)e;
         return v.LengthTo(line.CenterPoint);
      }

      /// <summary>
      /// Определение минимального расстояния по грани контура в заданной вершине.
      /// </summary>
      /// <param name="front">Проверяемый полигон.</param>
      /// <param name="v"> Выбранная вершина.</param>
      /// <returns></returns>
      static double MinLengthSeg(Polygon2d front, Vertex2d v)
      {
         List<ICurve2d> ds = front.GetAllSegments();
         var sel = from i in ds orderby GetLengthToSeg(v, i) select i;
         Line2d line = (Line2d)sel.First();
         return Math.Abs(line.A * v.X + line.B * v.Y + line.C) / Math.Sqrt(line.A * line.A + line.B * line.B);
      }

      /// <summary>
      /// Проверка на пересечение полигона с треугольником.
      /// </summary>
      /// <param name="front">Проверяемый полигон.</param>
      /// <param name="t">Проверяемый треугольник.</param>
      /// <param name="e">Возвращаемый объект ребра с которым было найдено пересечение.</param>
      /// <returns></returns>
      static bool CheckIntersect(Polygon2d front, Triangle t, out ICurve2d e)
      {
         e = null;
         bool res = false;
         List<ICurve2d> ds = front.GetAllSegments();
         if (ds.Count > 0)
         {
            for (int i = 0; i < ds.Count; i++)
            {
               if (t.Intersect(ds[i], out ICoordinates[] pts))
               {
                  res = true;
                  e = ds[i];
                  break;
               }
            }
         }
         return res;
      }

      /// <summary>
      /// Проверка на самопересечение по выбранной вершине.
      /// </summary>
      /// <param name="front">Проверяемый полигон.</param>
      /// <param name="v"> Выбранная вершина.</param>
      /// <returns></returns>
      static bool CheckIntersect(Polygon2d front, Vertex2d v)
      {
         bool res = false;
         List<ICurve2d> ds = front.GetAllSegments();
         if (ds.Count > 0)
         {
            for (int i = 0; i < ds.Count; i++)
            {
               if (v.IsMatch(ds[i].StartPoint) || v.IsMatch(ds[i].EndPoint)) continue;
               if (v.Prev.IsMatch(ds[i].StartPoint) || v.Prev.IsMatch(ds[i].EndPoint)) continue;
               if (v.Next.IsMatch(ds[i].StartPoint) || v.Next.IsMatch(ds[i].EndPoint)) continue;
               Line2d ln = new Line2d(v, v.Next);
               Line2d lp = new Line2d(v, v.Prev);
               Line2d lch = (Line2d)ds[i];
               lp.IntersectionSegments(lch, out IntersectResult res1);
               ln.IntersectionSegments(lch, out IntersectResult res2);
               if (res1.res || res2.res)
               {
                  res = true;
                  break;
               }
            }
         }
         return res;
      }

      /// <summary>
      /// Проверка попадания произвольной вершины полигона в сектор анализа, 
      /// образованный смежным углом выбранной вершины при фронтальной триангуляции.
      /// </summary>
      /// <param name="front">Проверяемый полигон.</param>
      /// <param name="v">Выбранная вершина для анализа.</param>
      /// <param name="o">Результирующая целевая вершина.</param>
      /// <param name="inTria">
      /// Результат попадания целевой вершины в треугольник, 
      /// образованный анализируемой вершиной и двумя смежными с ней вершинами.
      /// </param>
      /// <returns></returns>
      static bool CheckIn(Polygon2d front, Vertex2d v, out Vertex2d o, out bool inTria)
      {
         List<Vertex2d> sect = new List<Vertex2d>(front.Vertices);
         sect.Remove(v.Next);
         sect.Remove(v.Next.Next);
         sect.Remove(v);
         sect.Remove(v.Prev);
         sect.Remove(v.Prev.Prev);

         var sel = from i in sect where Math.Round(Vertex2d.GetAngleDeg(v.Prev, v, i), 3) < Math.Round(v.AngleDeg, 3) select i;
         sel = from i in sel where Math.Round(Vertex2d.GetAngleDeg(v.Prev, v, i), 3) > 0 orderby i.LengthTo(v) select i;

         if (sel.Count() > 0) o = sel.First();
         else o = null;

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
      /// 
      /// </summary>
      /// <param name="v"></param>
      /// <param name="o"></param>
      /// <returns></returns>
      static Tri[] BuildSelect(Vertex2d v, Vertex2d o)
      {
         Tri[] res = new Tri[] { null, null };
         Polygon2d t1 = new Polygon2d(new List<Vertex2d> { v, v.Next, o });
         Polygon2d t2 = new Polygon2d(new List<Vertex2d> { v, o, v.Prev });
         Polygon2d t3 = new Polygon2d(new List<Vertex2d> { v, v.Next, v.Prev });
         Polygon2d t4 = new Polygon2d(new List<Vertex2d> { v.Next, o, v.Prev });
         if (Math.Max(t1.MaxAngleDeg(), t2.MaxAngleDeg()) <= Math.Max(t3.MaxAngleDeg(), t4.MaxAngleDeg()))
         {
            res[0] = new Tri(v.Nref, v.Next.Nref, o.Nref);
            res[1] = new Tri(v.Nref, o.Nref, v.Prev.Nref);
         }
         else
         {
            res[0] = new Tri(v.Nref, v.Next.Nref, v.Prev.Nref);
            res[1] = new Tri(v.Next.Nref, o.Nref, v.Prev.Nref);
         }
         return res;
      }

      /// <summary>
      /// Триангуляция полигона по заданной величине шага.
      /// </summary>
      /// <param name="poly"></param>
      /// <param name="step"></param>
      /// <param name="stepType"></param>
      /// <returns></returns>
      public static Mesh Triangulation(Polygon2d poly, double step, ParamType stepType = ParamType.abs)
      {
         Mesh msh = new Mesh();
         msh.Out = new List<Node>(poly.Vertices.Count);
         Polygon2d front = (Polygon2d)poly.TesselationByStep(step, stepType);
         for (int i = 0; i < front.Vertices.Count; i++)
         {
            msh.Out.Add(new Node(front.Vertices[i], NodeType.border) { Id = i + 1 });
            front.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(front);

         int it = 1;
         int jn = msh.Out.Count + 1;
         while (work.Count > 0)
         {
            front = work.Pop();
            while (front.Vertices.Count > 4)
            {
               Vertex2d v = GetMinAngleVert(front);
               bool inquad = CheckIn(front, v, out Vertex2d o, out bool intria);

               if (intria)
               {
                  msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                  it++;
                  msh.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  work.Push(front.Partition(o, v));
               }
               else if (!intria && inquad)
               {
                  Tri[] tris = BuildSelect(v, o);
                  tris[0].Id = it; it++;
                  tris[1].Id = it; it++;
                  msh.Simplexs.AddRange(tris);
                  work.Push(front.Partition(o, v));
               }
               else if (v.AngleDeg <= 90)
               {
                  msh.Simplexs.Add(new Tri(v.Prev.Nref, v.Nref, v.Next.Nref) { Id = it });
                  it++;
                  front.RemoveVertex(v);
                  front.Open();
                  front.Close();
               }
               else if (v.AngleDeg > 90)
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  Vector2d v3 = v.GetBisector() * step;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (CheckIntersect(front, v))
                  {
                     v.X = x;
                     v.Y = y;
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     front.RemoveVertex(v);
                     front.Open();
                     front.Close();
                  }
                  else
                  {
                     msh.Nodes.Add(node);
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     msh.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     front.Open();
                     front.Close();
                  }
               }
            }
            if (front.Vertices.Count == 4)
            {
               var sel = from i in front.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();

               Tri[] tris = BuildSelect(v, v.Next.Next);
               tris[0].Id = it; it++;
               tris[1].Id = it; it++;
               msh.Simplexs.AddRange(tris);

               front.RemoveVertex(v);
               front.RemoveVertex(v.Next);
            }
            if (front.Vertices.Count == 3)
            {
               msh.Simplexs.Add(new Tri(front.Vertices[0].Nref, front.Vertices[1].Nref, front.Vertices[2].Nref) { Id = it });
               it++;
            }
         }

         return msh;
      }

      /// <summary>
      /// Фронтальная триангуляция полигона по заданной величине шага c участками регулярной сетки.
      /// </summary>
      /// <param name="step"></param>
      /// <param name="stepType"></param>
      /// <returns></returns>
      public static Mesh TriangulationRegular(Polygon2d poly, double step, ParamType stepType = ParamType.abs)
      {
         Mesh msh = new Mesh();
         msh.Out = new List<Node>(poly.Vertices.Count);
         Polygon2d front = (Polygon2d)poly.TesselationByStep(step, stepType);
         Polygon2d master = (Polygon2d)poly.TesselationByStep(step, stepType);
         for (int i = 0; i < front.Vertices.Count; i++)
         {
            msh.Out.Add(new Node(front.Vertices[i], NodeType.border) { Id = i + 1 });
            front.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(front);

         int it = 1;
         int jn = msh.Out.Count + 1;
         while (work.Count > 0)
         {
            front = work.Pop();
            while (front.Vertices.Count > 4)
            {
               Vertex2d v = GetMinAngleVert(front);
               bool inquad = CheckIn(front, v, out Vertex2d o, out bool intria);

               if (intria)
               {
                  msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = it });
                  it++;
                  msh.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  work.Push(front.Partition(o, v));
               }
               else if (!intria && inquad)
               {
                  Tri[] tris = BuildSelect(v, o);
                  tris[0].Id = it; it++;
                  tris[1].Id = it; it++;
                  msh.Simplexs.AddRange(tris);
                  work.Push(front.Partition(o, v));
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
                  msh.Simplexs.Add(new Tri(v.Next.Nref, v.Nref, v.Prev.Nref) { Id = it });
                  it++;
                  front.RemoveVertex(v);
                  front.Open();
                  front.Close();
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

                  if (CheckIntersect(front, v) || MinLengthSeg(master, v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     front.RemoveVertex(v);
                     front.Open();
                     front.Close();
                  }
                  else
                  {
                     msh.Nodes.Add(node);
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     msh.Simplexs.Add(new Tri(v.Next.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     front.Open();
                     front.Close();
                  }

                  //poly.CalcVertices();

                  Vertex2d ds = GetNullAngleVert(front);
                  if (ds != null)
                  {
                     front.RemoveVertex(ds, false);
                     if (ds.Prev.IsMatch(ds.Next)) front.RemoveVertex(ds.Next, false);
                     front.Open();
                     front.Close();
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

                  if (CheckIntersect(front, v) || MinLengthSeg(master, v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     front.RemoveVertex(v);
                     front.Open();
                     front.Close();
                  }
                  else
                  {
                     msh.Nodes.Add(node);
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     msh.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     front.Open();
                     front.Close();
                  }

                  Vertex2d ds = GetNullAngleVert(front);
                  if (ds != null)
                  {
                     front.RemoveVertex(ds, false);
                     if (ds.Prev.IsMatch(ds.Next)) front.RemoveVertex(ds.Next, false);
                     front.Open();
                     front.Close();
                  }

               }
               else
               {
                  Vector3d v1 = v.Prev - v;
                  Vector3d v2 = v.Next - v;
                  Vector2d v3 = v.GetBisector() * step;
                  Node node = new Node(v.X + v3.Vx, v.Y + v3.Vy, 0, NodeType.interior) { Id = jn };
                  double x = v.X;
                  double y = v.Y;
                  v.X = node.X;
                  v.Y = node.Y;

                  if (CheckIntersect(front, v) || MinLengthSeg(master, v) < 0.25 * step)
                  {
                     v.X = x;
                     v.Y = y;
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, v.Prev.Nref) { Id = it });
                     it++;
                     front.RemoveVertex(v);
                     front.Open();
                     front.Close();
                  }
                  else
                  {
                     msh.Nodes.Add(node);
                     msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, jn) { Id = it });
                     it++;
                     msh.Simplexs.Add(new Tri(v.Nref, jn, v.Prev.Nref) { Id = it });
                     it++;

                     v.Nref = jn;
                     jn++;

                     front.Open();
                     front.Close();
                  }

                  Vertex2d ds = GetNullAngleVert(front);
                  if (ds != null)
                  {
                     front.RemoveVertex(ds, false);
                     if (ds.Prev.IsMatch(ds.Next)) front.RemoveVertex(ds.Next, false);

                     front.Open();
                     front.Close();
                  }
               }
            }
            if (front.Vertices.Count == 4)
            {
               var sel = from i in front.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();

               if (v.AngleDeg < 90)
               {
                  Tri[] tris = BuildSelect(v, v.Next.Next);
                  tris[0].Id = it; it++;
                  tris[1].Id = it; it++;
                  msh.Simplexs.AddRange(tris);
               }

               front.RemoveVertex(v);
               front.RemoveVertex(v.Next);
            }
            if (front.Vertices.Count == 3)
            {
               msh.Simplexs.Add(new Tri(front.Vertices[0].Nref, front.Vertices[1].Nref, front.Vertices[2].Nref) { Id = it });
               it++;
            }
         }

         return msh;
      }

      /// <summary>
      /// Разделение полигона на треугольники.
      /// </summary>
      /// <param name="poly">Разделяемый полигон.</param>
      /// <returns></returns>
      public static Mesh TriangulationSubdiv(Polygon2d poly)
      {
         Mesh msh = new Mesh();
         msh.Out = new List<Node>(poly.Vertices.Count);
         Polygon2d front = new Polygon2d(poly.Vertices);
         for (int i = 0; i < front.Vertices.Count; i++)
         {
            msh.Out.Add(new Node(front.Vertices[i], NodeType.border) { Id = i + 1 });
            front.Vertices[i].Nref = i + 1;
         }
         Stack<Polygon2d> work = new Stack<Polygon2d>();
         work.Push(front);

         int cntr = 1;

         while (work.Count > 0)
         {
            front = work.Pop();

            while (front.Vertices.Count > 3)
            {
               var sel = from i in front.Vertices orderby i.AngleDeg select i;
               Vertex2d v = sel.First();

               if (CheckInTria(front, v, out Vertex2d o))
               {
                  msh.Simplexs.Add(new Tri(v.Nref, v.Next.Nref, o.Nref) { Id = cntr });
                  cntr++;
                  msh.Simplexs.Add(new Tri(v.Nref, o.Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  work.Push(front.Partition(sel.First(), v));
               }
               else
               {
                  msh.Simplexs.Add(new Tri(v.Next.Nref, v.Nref, v.Prev.Nref) { Id = cntr });
                  cntr++;
                  front.RemoveVertex(v);
               }
            }

            if (front.Vertices.Count == 3)
            {
               msh.Simplexs.Add(new Tri(front.Vertices[0].Nref, front.Vertices[1].Nref, front.Vertices[2].Nref) { Id = cntr });
               cntr++;
            }
         }

         return msh;
      }

   }
}
