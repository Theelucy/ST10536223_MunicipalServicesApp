using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>A weighted, undirected connection between two vertices.</summary>
    public class Edge<T>
    {
        public Edge(T from, T to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public T From { get; private set; }
        public T To { get; private set; }
        public double Weight { get; private set; }
    }

    /// <summary>One vertex reached by a traversal: how many hops from the start and which vertex it was reached from.</summary>
    public class TraversalStep<T>
    {
        public T Vertex { get; set; }
        public int Level { get; set; }
        public bool HasParent { get; set; }
        public T Parent { get; set; }
    }

    public class MstResult<T>
    {
        public string Algorithm { get; set; }
        public List<Edge<T>> Edges { get; set; }
        public double TotalWeight { get; set; }

        /// <summary>True if the graph is connected, so the result is a single tree touching every vertex.</summary>
        public bool SpansAllVertices { get; set; }
    }

    /// <summary>
    /// A weighted undirected graph stored as an adjacency list: each vertex
    /// keeps a list of the edges touching it, so visiting a vertex's
    /// neighbours costs only its own degree. Used twice in the Service Request
    /// Status feature - for the ward road network (weights = kilometres) and for
    /// the "related requests" network (weights = how loosely two requests are linked).
    /// </summary>
    public class Graph<T>
    {
        private readonly List<T> _vertices = new List<T>();
        private readonly Dictionary<T, List<Edge<T>>> _adjacency = new Dictionary<T, List<Edge<T>>>();
        private readonly List<Edge<T>> _edges = new List<Edge<T>>();
        private readonly EqualityComparer<T> _equality = EqualityComparer<T>.Default;

        public IList<T> Vertices
        {
            get { return _vertices.AsReadOnly(); }
        }

        public IList<Edge<T>> Edges
        {
            get { return _edges.AsReadOnly(); }
        }

        public int VertexCount
        {
            get { return _vertices.Count; }
        }

        public int EdgeCount
        {
            get { return _edges.Count; }
        }

        public bool ContainsVertex(T vertex)
        {
            return _adjacency.ContainsKey(vertex);
        }

        public void AddVertex(T vertex)
        {
            if (!_adjacency.ContainsKey(vertex))
            {
                _adjacency[vertex] = new List<Edge<T>>();
                _vertices.Add(vertex);
            }
        }

        /// <summary>Connects two vertices (adding them if needed). Self-loops and repeated edges are ignored.</summary>
        public void AddEdge(T a, T b, double weight)
        {
            if (_equality.Equals(a, b)) return;
            AddVertex(a);
            AddVertex(b);
            if (HasEdge(a, b)) return;

            var edge = new Edge<T>(a, b, weight);
            _edges.Add(edge);
            _adjacency[a].Add(edge);
            _adjacency[b].Add(edge);
        }

        public bool HasEdge(T a, T b)
        {
            if (!_adjacency.ContainsKey(a)) return false;
            foreach (Edge<T> e in _adjacency[a])
            {
                if (_equality.Equals(Other(e, a), b)) return true;
            }
            return false;
        }

        public List<T> Neighbours(T vertex)
        {
            var result = new List<T>();
            foreach (Edge<T> e in _adjacency[vertex])
            {
                result.Add(Other(e, vertex));
            }
            return result;
        }

        public int Degree(T vertex)
        {
            return _adjacency[vertex].Count;
        }

        private T Other(Edge<T> edge, T vertex)
        {
            return _equality.Equals(edge.From, vertex) ? edge.To : edge.From;
        }

        // ------------------------------------------------------------
        // Traversal
        // ------------------------------------------------------------

        /// <summary>
        /// Breadth-first: the start, then everything one hop away, then two hops,
        /// and so on (a queue). Level = the fewest hops from the start.
        /// </summary>
        public List<TraversalStep<T>> BreadthFirst(T start)
        {
            var result = new List<TraversalStep<T>>();
            if (!_adjacency.ContainsKey(start)) return result;

            var visited = new HashSet<T> { start };
            var queue = new Queue<TraversalStep<T>>();
            queue.Enqueue(new TraversalStep<T> { Vertex = start, Level = 0 });

            while (queue.Count > 0)
            {
                TraversalStep<T> step = queue.Dequeue();
                result.Add(step);

                foreach (T next in Neighbours(step.Vertex))
                {
                    if (visited.Add(next))
                    {
                        queue.Enqueue(new TraversalStep<T>
                        {
                            Vertex = next,
                            Level = step.Level + 1,
                            HasParent = true,
                            Parent = step.Vertex
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Depth-first: follow one branch as far as it goes, then backtrack (a stack).
        /// Level = depth in the search tree.
        /// </summary>
        public List<TraversalStep<T>> DepthFirst(T start)
        {
            var result = new List<TraversalStep<T>>();
            if (!_adjacency.ContainsKey(start)) return result;

            var visited = new HashSet<T>();
            var stack = new Stack<TraversalStep<T>>();
            stack.Push(new TraversalStep<T> { Vertex = start, Level = 0 });

            while (stack.Count > 0)
            {
                TraversalStep<T> step = stack.Pop();
                if (!visited.Add(step.Vertex)) continue; // already reached by a shorter branch

                result.Add(step);

                List<T> neighbours = Neighbours(step.Vertex);
                for (int i = neighbours.Count - 1; i >= 0; i--) // reversed: first neighbour is explored first
                {
                    if (!visited.Contains(neighbours[i]))
                    {
                        stack.Push(new TraversalStep<T>
                        {
                            Vertex = neighbours[i],
                            Level = step.Level + 1,
                            HasParent = true,
                            Parent = step.Vertex
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>Groups of vertices that are connected to each other (found with repeated breadth-first searches).</summary>
        public List<List<T>> ConnectedComponents()
        {
            var components = new List<List<T>>();
            var seen = new HashSet<T>();
            foreach (T v in _vertices)
            {
                if (seen.Contains(v)) continue;

                var group = new List<T>();
                foreach (TraversalStep<T> step in BreadthFirst(v))
                {
                    seen.Add(step.Vertex);
                    group.Add(step.Vertex);
                }
                components.Add(group);
            }
            return components;
        }

        // ------------------------------------------------------------
        // Minimum spanning tree
        // ------------------------------------------------------------

        /// <summary>
        /// Kruskal's algorithm: sort every edge by weight, then take the next
        /// cheapest edge unless it would form a loop. A union-find structure
        /// answers "are these two already connected?" in near-constant time.
        /// O(E log E).
        /// </summary>
        public MstResult<T> MinimumSpanningTreeKruskal()
        {
            var sorted = new List<Edge<T>>(_edges);
            // List.Sort is not stable, so break weight ties by original position.
            var position = new Dictionary<Edge<T>, int>();
            for (int i = 0; i < _edges.Count; i++) position[_edges[i]] = i;
            sorted.Sort((x, y) =>
            {
                int byWeight = x.Weight.CompareTo(y.Weight);
                return byWeight != 0 ? byWeight : position[x].CompareTo(position[y]);
            });

            var index = new Dictionary<T, int>();
            for (int i = 0; i < _vertices.Count; i++) index[_vertices[i]] = i;
            var sets = new UnionFind(_vertices.Count);

            var chosen = new List<Edge<T>>();
            double total = 0;
            foreach (Edge<T> e in sorted)
            {
                if (sets.Union(index[e.From], index[e.To]))
                {
                    chosen.Add(e);
                    total += e.Weight;
                }
            }

            return new MstResult<T>
            {
                Algorithm = "Kruskal",
                Edges = chosen,
                TotalWeight = total,
                SpansAllVertices = _vertices.Count == 0 || chosen.Count == _vertices.Count - 1
            };
        }

        /// <summary>
        /// Prim's algorithm: grow one tree outwards from the start vertex,
        /// always adding the cheapest edge that reaches a new vertex. A binary
        /// heap supplies that cheapest edge in O(log E), giving O(E log E)
        /// overall. If the graph is disconnected a tree is grown in each part.
        /// </summary>
        public MstResult<T> MinimumSpanningTreePrim(T start)
        {
            var chosen = new List<Edge<T>>();
            double total = 0;
            var visited = new HashSet<T>();

            var order = new List<T>();
            if (_adjacency.ContainsKey(start)) order.Add(start);
            order.AddRange(_vertices);

            bool firstTree = true;
            bool connected = true;
            foreach (T root in order)
            {
                if (visited.Contains(root)) continue;
                if (!firstTree) connected = false;
                firstTree = false;

                var heap = new BinaryHeap<Edge<T>>((a, b) => a.Weight.CompareTo(b.Weight));
                visited.Add(root);
                foreach (Edge<T> e in _adjacency[root]) heap.Insert(e);

                while (!heap.IsEmpty)
                {
                    Edge<T> cheapest = heap.ExtractTop();
                    bool fromSeen = visited.Contains(cheapest.From);
                    bool toSeen = visited.Contains(cheapest.To);
                    if (fromSeen && toSeen) continue; // would create a loop

                    T newVertex = fromSeen ? cheapest.To : cheapest.From;
                    visited.Add(newVertex);
                    chosen.Add(cheapest);
                    total += cheapest.Weight;

                    foreach (Edge<T> e in _adjacency[newVertex])
                    {
                        if (!visited.Contains(Other(e, newVertex))) heap.Insert(e);
                    }
                }
            }

            return new MstResult<T>
            {
                Algorithm = "Prim",
                Edges = chosen,
                TotalWeight = total,
                SpansAllVertices = connected
            };
        }

        /// <summary>Disjoint-set (union-find) with path compression and union by rank.</summary>
        private class UnionFind
        {
            private readonly int[] _parent;
            private readonly int[] _rank;

            public UnionFind(int size)
            {
                _parent = new int[size];
                _rank = new int[size];
                for (int i = 0; i < size; i++) _parent[i] = i;
            }

            private int Find(int x)
            {
                while (_parent[x] != x)
                {
                    _parent[x] = _parent[_parent[x]]; // path halving
                    x = _parent[x];
                }
                return x;
            }

            /// <summary>Joins the two sets; returns false if they were already the same set.</summary>
            public bool Union(int a, int b)
            {
                int rootA = Find(a);
                int rootB = Find(b);
                if (rootA == rootB) return false;

                if (_rank[rootA] < _rank[rootB])
                {
                    _parent[rootA] = rootB;
                }
                else if (_rank[rootA] > _rank[rootB])
                {
                    _parent[rootB] = rootA;
                }
                else
                {
                    _parent[rootB] = rootA;
                    _rank[rootA]++;
                }
                return true;
            }
        }
    }
}
