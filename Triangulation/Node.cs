using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public class Node : Point3d, ICoordinates
   {
      public int Id { get; set; }
      public NodeType Type { get; set; }
      public Domain Domain { get; set; }
      public Dictionary<string, double> Attribs { get;}

      public Node(double x, double y, double z = 0, NodeType type = NodeType.free) : base(x, y, z)
      {
         Type = type;
      }
      public Node(ICoordinates src, NodeType type = NodeType.free) : base(src)
      {
         Type = type;
      }
   }

   public enum NodeType
   {
      border,
      interior,
      special,
      free
   }
}
