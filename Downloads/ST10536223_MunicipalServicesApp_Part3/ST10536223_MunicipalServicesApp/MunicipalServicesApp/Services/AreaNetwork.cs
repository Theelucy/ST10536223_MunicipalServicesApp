using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// The municipality's road network as a weighted graph: each ward and the
    /// municipal depot is a vertex, each road link is an edge, and the edge
    /// weight is the driving distance in kilometres. The minimum spanning tree
    /// of this graph is the cheapest set of roads that still connects every
    /// ward to the depot - the backbone for planning inspection routes.
    /// </summary>
    public static class AreaNetwork
    {
        public const string Depot = "Depot";
        public const string Unassigned = "Unassigned";

        // Map positions (0..1) used only to draw the network.
        private static readonly Dictionary<string, PointF> Positions = new Dictionary<string, PointF>
        {
            { "Ward 1", new PointF(0.10f, 0.14f) }, { "Ward 2", new PointF(0.33f, 0.10f) },
            { "Ward 3", new PointF(0.58f, 0.16f) }, { "Ward 4", new PointF(0.86f, 0.12f) },
            { "Ward 5", new PointF(0.13f, 0.46f) }, { "Ward 6", new PointF(0.33f, 0.37f) },
            { Depot,    new PointF(0.52f, 0.50f) }, { "Ward 7", new PointF(0.73f, 0.40f) },
            { "Ward 8", new PointF(0.91f, 0.50f) }, { "Ward 9", new PointF(0.10f, 0.84f) },
            { "Ward 10", new PointF(0.33f, 0.76f) }, { "Ward 11", new PointF(0.58f, 0.86f) },
            { "Ward 12", new PointF(0.85f, 0.80f) }
        };

        private static readonly object[][] Roads =
        {
            new object[] { Depot, "Ward 6", 3.1 },  new object[] { Depot, "Ward 7", 3.4 },
            new object[] { Depot, "Ward 10", 4.0 }, new object[] { Depot, "Ward 11", 3.8 },
            new object[] { Depot, "Ward 3", 4.6 },
            new object[] { "Ward 1", "Ward 2", 2.8 },  new object[] { "Ward 2", "Ward 3", 3.5 },
            new object[] { "Ward 3", "Ward 4", 3.9 },  new object[] { "Ward 1", "Ward 5", 3.2 },
            new object[] { "Ward 2", "Ward 6", 3.0 },  new object[] { "Ward 3", "Ward 7", 3.7 },
            new object[] { "Ward 4", "Ward 8", 4.1 },  new object[] { "Ward 5", "Ward 6", 2.6 },
            new object[] { "Ward 6", "Ward 7", 5.2 },  new object[] { "Ward 7", "Ward 8", 2.9 },
            new object[] { "Ward 5", "Ward 9", 3.6 },  new object[] { "Ward 6", "Ward 10", 3.3 },
            new object[] { "Ward 9", "Ward 10", 2.7 }, new object[] { "Ward 10", "Ward 11", 3.1 },
            new object[] { "Ward 11", "Ward 12", 3.4 }, new object[] { "Ward 7", "Ward 12", 4.8 },
            new object[] { "Ward 8", "Ward 12", 4.3 }, new object[] { "Ward 4", "Ward 7", 5.0 }
        };

        private static readonly Graph<string> Network = BuildNetwork();

        private static Graph<string> BuildNetwork()
        {
            var g = new Graph<string>();
            foreach (string name in Positions.Keys) g.AddVertex(name);
            foreach (object[] road in Roads) g.AddEdge((string)road[0], (string)road[1], (double)road[2]);
            return g;
        }

        public static Graph<string> Graph
        {
            get { return Network; }
        }

        public static PointF PositionOf(string area)
        {
            PointF p;
            return Positions.TryGetValue(area, out p) ? p : new PointF(0.5f, 0.5f);
        }

        /// <summary>Works out which ward a free-text location belongs to ("... Ward 5" -> "Ward 5").</summary>
        public static string AreaOf(string location)
        {
            if (string.IsNullOrWhiteSpace(location)) return Unassigned;
            Match m = Regex.Match(location, @"ward\s*(\d+)", RegexOptions.IgnoreCase);
            if (!m.Success) return Unassigned;

            string ward = "Ward " + int.Parse(m.Groups[1].Value);
            return Network.ContainsVertex(ward) ? ward : Unassigned;
        }

        public static bool AreAdjacent(string a, string b)
        {
            return Network.ContainsVertex(a) && Network.ContainsVertex(b) && Network.HasEdge(a, b);
        }
    }
}
