using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public class Mesh
   {
      public List<Node> Nodes { get; set; }
      public List<ISimplex> Simplexs { get; set; }
      public List<Node> Out { get; set; }
      public List<Node> Holes { get; set; }
      public List<Node> Pathes { get; set; }
      public List<Node> Points { get; set; }

      public Mesh()
      {
         Nodes = new List<Node>();
         Simplexs = new List<ISimplex>();
      }

      public void SmoothTri(int number)
      {
         List<Node> nodes = new List<Node>(Nodes);
         nodes.AddRange(Out);
         for (int i = 0; i < number; i++)
         {
            foreach (Node item in Nodes)
            {
               var sel = from s in Simplexs where ((Tri)s).A == item.Id || ((Tri)s).B == item.Id || ((Tri)s).C == item.Id select s;
               List<ISimplex> simplices = new List<ISimplex>(sel);
               double xc = 0;
               double yc = 0;
               foreach (Tri t in simplices)
               {
                  List<Node> selt = (from n in nodes where t.A == n.Id || t.B == n.Id || t.C == n.Id select n).ToList();
                  List<Node> selA = (from n in selt where t.A == n.Id select n).ToList();
                  List<Node> selB = (from n in selt where t.B == n.Id select n).ToList();
                  List<Node> selC = (from n in selt where t.C == n.Id select n).ToList();
                  Triangle tria = new Triangle(selA[0], selB[0], selC[0]);
                  xc += tria.Xc;
                  yc += tria.Yc;
               }

               item.X = xc / simplices.Count;
               item.Y = yc / simplices.Count;
            }
         }
      }


   }
}
