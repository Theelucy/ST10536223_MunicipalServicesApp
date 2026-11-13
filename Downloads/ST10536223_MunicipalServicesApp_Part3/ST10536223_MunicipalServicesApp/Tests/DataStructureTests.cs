// Optional console test program for the data structures (not part of the Visual Studio build).
//
// From a "Developer Command Prompt for VS" in the folder that contains MunicipalServicesApp.sln:
//   csc /r:System.Drawing.dll /r:System.Core.dll /out:DataStructureTests.exe ^
//       Tests\DataStructureTests.cs MunicipalServicesApp\DataStructures\*.cs
//   DataStructureTests.exe
//
// Expected last line: ALL 2554 CHECKS PASSED
using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.DataStructures;

class DataStructureTests
{
    static int pass = 0, fail = 0;
    static void Check(string name, bool ok)
    {
        if (ok) pass++; else { fail++; Console.WriteLine("FAIL  " + name); }
    }

    static void Main()
    {
        var rnd = new Random(12345);

        // ---------------- heap ----------------
        {
            bool allValid = true, sortedOk = true, buildOk = true, nonDestructive = true;
            for (int round = 0; round < 50; round++)
            {
                var h = new BinaryHeap<int>((a, b) => a.CompareTo(b));
                var list = new List<int>();
                int n = rnd.Next(0, 200);
                for (int i = 0; i < n; i++) { int v = rnd.Next(1000); h.Insert(v); list.Add(v); if (!h.IsValidHeap()) allValid = false; }
                var h2 = new BinaryHeap<int>(list, (a, b) => a.CompareTo(b));
                if (!h2.IsValidHeap() || h2.Count != n) buildOk = false;
                var sortedCopy = h.ToSortedList();
                if (h.Count != n) nonDestructive = false;
                list.Sort();
                if (!sortedCopy.SequenceEqual(list)) sortedOk = false;
                var drained = new List<int>();
                while (!h.IsEmpty) drained.Add(h.ExtractTop());
                if (!drained.SequenceEqual(list)) sortedOk = false;
            }
            Check("heap: valid after every insert", allValid);
            Check("heap: extract order is sorted", sortedOk);
            Check("heap: build-heap valid", buildOk);
            Check("heap: ToSortedList leaves heap intact", nonDestructive);
            bool threw = false;
            try { new BinaryHeap<int>((a, b) => a.CompareTo(b)).ExtractTop(); } catch (InvalidOperationException) { threw = true; }
            Check("heap: empty extract throws", threw);
        }

        // ---------------- search trees vs reference ----------------
        for (int round = 0; round < 60; round++)
        {
            var bst = new BinarySearchTree<int, string>();
            var avl = new AvlTree<int, string>();
            var rb = new RedBlackTree<int, string>();
            var reference = new SortedDictionary<int, string>();
            int n = rnd.Next(0, 300);
            bool avlOk = true, rbOk = true;
            for (int i = 0; i < n; i++)
            {
                int k = rnd.Next(0, 400);
                string v = "v" + i;
                bst.Insert(k, v); avl.Insert(k, v); rb.Insert(k, v); reference[k] = v;
                if (!avl.IsBalanced()) avlOk = false;
                if (!rb.IsValidRedBlackTree()) rbOk = false;
            }
            Check("avl balanced after every insert (round " + round + ")", avlOk);
            Check("rb valid after every insert (round " + round + ")", rbOk);

            var expected = reference.ToList();
            foreach (var tree in new SearchTreeBase<int, string>[] { bst, avl, rb })
            {
                Check(tree.Name + " count", tree.Count == reference.Count);
                var inorder = tree.InOrder();
                Check(tree.Name + " in-order equals sorted reference", inorder.SequenceEqual(expected));
                Check(tree.Name + " valid search tree", tree.IsValidSearchTree());
                bool findOk = true;
                for (int k = -5; k < 410; k++)
                {
                    string val; int cmp;
                    bool found = tree.TryFind(k, out val, out cmp);
                    if (found != reference.ContainsKey(k)) findOk = false;
                    else if (found && val != reference[k]) findOk = false;
                    if (found && cmp < 1) findOk = false;
                }
                Check(tree.Name + " find agrees with reference", findOk);

                bool rangeOk = true;
                for (int t = 0; t < 10; t++)
                {
                    int lo = rnd.Next(-10, 410), hi = rnd.Next(-10, 410);
                    if (lo > hi) { int tmp = lo; lo = hi; hi = tmp; }
                    var got = tree.Range(lo, hi);
                    var want = reference.Where(kv => kv.Key >= lo && kv.Key <= hi).Select(kv => kv.Value).ToList();
                    if (!got.SequenceEqual(want)) rangeOk = false;
                }
                Check(tree.Name + " range query matches reference", rangeOk);
                Check(tree.Name + " layout node count", tree.GetLayout(k => k.ToString(), null).Count == reference.Count);
            }
            if (n > 1)
            {
                Check("avl height bound", avl.Height <= 1.45 * Math.Log(reference.Count + 2, 2) + 1);
                Check("rb height bound", rb.Height <= 2 * Math.Log(reference.Count + 1, 2) + 1);
            }
        }

        // ---------------- sequential keys (the real-world worst case) ----------------
        {
            var bst = new BinarySearchTree<int, int>();
            var avl = new AvlTree<int, int>();
            var rb = new RedBlackTree<int, int>();
            const int N = 1000;
            for (int i = 1; i <= N; i++) { bst.Insert(i, i); avl.Insert(i, i); rb.Insert(i, i); }
            Check("sequential: BST degenerates to a chain (height = n)", bst.Height == N);
            Check("sequential: AVL height ~ log n", avl.Height <= 11);
            Check("sequential: RB height <= 2 log n", rb.Height <= 20);
            int c1, c2, c3; int v;
            bst.TryFind(N, out v, out c1); avl.TryFind(N, out v, out c2); rb.TryFind(N, out v, out c3);
            Check("sequential: BST needs n comparisons for the newest key", c1 == N);
            Check("sequential: AVL/RB need few comparisons", c2 <= 11 && c3 <= 20);
            Check("sequential: AVL valid", avl.IsBalanced() && avl.IsValidSearchTree());
            Check("sequential: RB valid", rb.IsValidRedBlackTree());
            Console.WriteLine("  sequential 1.." + N + ": BST height " + bst.Height + ", AVL " + avl.Height + " (rot " + avl.RotationCount + "), RB " + rb.Height + " (rot " + rb.RotationCount + ", black-height " + rb.BlackHeight + ")");
        }

        // ---------------- duplicate key replaces value ----------------
        {
            var t = new RedBlackTree<int, string>();
            t.Insert(5, "a"); t.Insert(5, "b");
            string v; int c;
            t.TryFind(5, out v, out c);
            Check("duplicate key replaces value, count stays 1", t.Count == 1 && v == "b");
        }

        // ---------------- general tree ----------------
        {
            var tree = new GeneralTree<string>("R");
            var a = tree.Root.AddChild("A"); var b = tree.Root.AddChild("B");
            a.AddChild("A1"); a.AddChild("A2"); b.AddChild("B1");
            Check("general tree DFS", string.Join(",", tree.DepthFirst().Select(n => n.Value)) == "R,A,A1,A2,B,B1");
            Check("general tree BFS", string.Join(",", tree.BreadthFirst().Select(n => n.Value)) == "R,A,B,A1,A2,B1");
            Check("general tree count/leaves/height", tree.Count == 6 && tree.LeafCount == 3 && tree.Height == 3);
            Check("general tree path", string.Join(">", tree.PathFromRoot(tree.Find(x => x == "A2"))) == "R>A>A2");
            Check("general tree depth", tree.Find(x => x == "B1").Depth == 2);
        }

        // ---------------- binary tree traversals ----------------
        {
            var root = new BinaryTreeNode<int>(1);
            root.Left = new BinaryTreeNode<int>(2); root.Right = new BinaryTreeNode<int>(3);
            root.Left.Left = new BinaryTreeNode<int>(4); root.Left.Right = new BinaryTreeNode<int>(5);
            var t = new BinaryTree<int>(root);
            Check("binary tree pre-order", string.Join("", t.PreOrder()) == "12453");
            Check("binary tree in-order", string.Join("", t.InOrder()) == "42513");
            Check("binary tree post-order", string.Join("", t.PostOrder()) == "45231");
            Check("binary tree level-order", string.Join("", t.LevelOrder()) == "12345");
            Check("binary tree count/leaves/height", t.Count == 5 && t.LeafCount == 3 && t.Height == 3);
            var layout = t.GetLayout(x => x.ToString(), x => System.Drawing.Color.Blue, x => x == 4, "Y", "N");
            Check("binary tree layout", layout.Count == 5 && layout.Select(l => l.Order).OrderBy(o => o).SequenceEqual(Enumerable.Range(0, 5)) && layout.Count(l => l.Highlight) == 1);
            var empty = new BinaryTree<int>(null);
            Check("empty binary tree", empty.Height == 0 && empty.PreOrder().Count == 0 && empty.PostOrder().Count == 0);
        }

        // ---------------- graph traversal ----------------
        {
            var g = new Graph<string>();
            g.AddEdge("A", "B", 1); g.AddEdge("A", "C", 1); g.AddEdge("B", "D", 1); g.AddEdge("C", "D", 1); g.AddEdge("D", "E", 1);
            g.AddVertex("Z");
            var bfs = g.BreadthFirst("A");
            Check("bfs order", string.Join("", bfs.Select(s => s.Vertex)) == "ABCDE");
            Check("bfs levels", string.Join("", bfs.Select(s => s.Level)) == "01123");
            var dfs = g.DepthFirst("A");
            Check("dfs order", string.Join("", dfs.Select(s => s.Vertex)) == "ABDCE");
            Check("components", g.ConnectedComponents().Count == 2);
            Check("unknown start gives empty", g.BreadthFirst("Q").Count == 0 && g.DepthFirst("Q").Count == 0);
            g.AddEdge("A", "B", 9); g.AddEdge("A", "A", 1);
            Check("duplicate/self edges ignored", g.EdgeCount == 5);
        }

        // ---------------- MST: Kruskal vs Prim vs brute force ----------------
        for (int round = 0; round < 300; round++)
        {
            int n = rnd.Next(2, 8);
            var g = new Graph<int>();
            for (int i = 0; i < n; i++) g.AddVertex(i);
            // random connected backbone then extra edges
            for (int i = 1; i < n; i++) g.AddEdge(i, rnd.Next(0, i), rnd.Next(1, 20));
            int extra = rnd.Next(0, n * 2);
            for (int i = 0; i < extra; i++) g.AddEdge(rnd.Next(n), rnd.Next(n), rnd.Next(1, 20));

            var k = g.MinimumSpanningTreeKruskal();
            var p = g.MinimumSpanningTreePrim(rnd.Next(n));
            Check("mst: Kruskal == Prim total (round " + round + ")", Math.Abs(k.TotalWeight - p.TotalWeight) < 1e-9);
            Check("mst: n-1 edges", k.Edges.Count == n - 1 && p.Edges.Count == n - 1);
            Check("mst: spans flag", k.SpansAllVertices && p.SpansAllVertices);

            // brute force minimum
            var edges = g.Edges.ToList();
            double best = double.MaxValue;
            int m = edges.Count;
            for (int mask = 0; mask < (1 << m); mask++)
            {
                if (Bits(mask) != n - 1) continue;
                var parent = Enumerable.Range(0, n).ToArray();
                Func<int, int> find = null; find = x => parent[x] == x ? x : (parent[x] = find(parent[x]));
                bool ok = true; double w = 0;
                for (int i = 0; i < m && ok; i++) if ((mask & (1 << i)) != 0)
                {
                    int a = find(edges[i].From), b = find(edges[i].To);
                    if (a == b) ok = false; else { parent[a] = b; w += edges[i].Weight; }
                }
                if (ok && w < best) best = w;
            }
            if (m <= 18) Check("mst: equals brute-force minimum (round " + round + ")", Math.Abs(k.TotalWeight - best) < 1e-9);
        }

        // disconnected graph -> forest
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2, 3); g.AddEdge(2, 3, 1); g.AddEdge(1, 3, 2); g.AddEdge(10, 11, 5); g.AddVertex(99);
            var k = g.MinimumSpanningTreeKruskal(); var p = g.MinimumSpanningTreePrim(1);
            Check("forest: edge count = n - components", k.Edges.Count == 6 - 3 && p.Edges.Count == 3);
            Check("forest: totals agree and flagged disconnected", Math.Abs(k.TotalWeight - p.TotalWeight) < 1e-9 && !k.SpansAllVertices && !p.SpansAllVertices);
            Check("forest: total", Math.Abs(k.TotalWeight - 8) < 1e-9);
        }

        Console.WriteLine(fail == 0 ? "ALL " + pass + " CHECKS PASSED" : fail + " FAILED, " + pass + " passed");
    }

    static int Bits(int x) { int c = 0; while (x != 0) { c += x & 1; x >>= 1; } return c; }
}
