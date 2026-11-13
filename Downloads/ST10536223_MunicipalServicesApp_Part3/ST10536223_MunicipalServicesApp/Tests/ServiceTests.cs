// Optional console test program for the service layer (not part of the Visual Studio build).
// It checks the triage rules, indexes, heap, area tree, related-requests graph and ward network
// against the 22 sample requests.
//
// From a "Developer Command Prompt for VS" in the folder that contains MunicipalServicesApp.sln:
//   csc /r:System.Drawing.dll /r:System.Core.dll /out:ServiceTests.exe ^
//       Tests\ServiceTests.cs MunicipalServicesApp\Models\Issue.cs ^
//       MunicipalServicesApp\DataStructures\*.cs MunicipalServicesApp\Services\*.cs
//   ServiceTests.exe
//
// Expected last line: ALL 48 SERVICE CHECKS PASSED
using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

class ServiceTests
{
    static int pass = 0, fail = 0;
    static void Check(string name, bool ok) { if (ok) pass++; else { fail++; Console.WriteLine("FAIL  " + name); } }

    static void Main()
    {
        // empty
        var empty = new ServiceRequestService();
        Check("empty: counts zero", empty.TotalCount == 0 && empty.TriageQueue.IsEmpty && empty.AssignNextToCrew() == null);
        Check("empty: lookup not found", !empty.Lookup(1).Found);
        Check("empty: area tree is just the root", empty.AreaTree.Count == 1);
        Check("empty: no clusters", empty.IncidentClusters().Count == 0);

        int added = SampleRequestLoader.Load();
        Check("sample: 22 loaded", added == 22 && SampleRequestLoader.Load() == 0);
        var svc = new ServiceRequestService();
        Check("service: total", svc.TotalCount == 22);

        // expected priorities worked out by hand from the rules
        int[] expected = { 3, 1, 2, 4, 3, 2, 2, 1, 3, 1, 4, 1, 2, 3, 3, 1, 2, 2, 4, 3, 2, 3 };
        for (int i = 0; i < 22; i++)
        {
            var r = svc.Lookup(i + 1).Info;
            Check("triage #" + (i + 1) + " expected P" + expected[i] + " got P" + r.Priority + " (" + string.Join(">", r.Triage.Codes) + ")", r.Priority == expected[i]);
        }

        // lookups
        var lk = svc.Lookup(22);
        Console.WriteLine("lookup #22 comparisons: BST " + lk.BstComparisons + ", AVL " + lk.AvlComparisons + ", RB " + lk.RbComparisons + " | heights BST " + svc.BstById.Height + ", AVL " + svc.AvlById.Height + ", RB " + svc.RbById.Height);
        Check("lookup found with correct request", lk.Found && lk.Info.Id == 22);
        Check("BST is a chain for sequential ids", svc.BstById.Height == 22 && lk.BstComparisons == 22);
        Check("AVL/RB far cheaper", lk.AvlComparisons <= 6 && lk.RbComparisons <= 8);
        Check("lookup missing id", !svc.Lookup(999).Found && svc.Lookup(999).Info == null);

        // chronological and range
        var chrono = svc.Chronological();
        bool inOrder = true;
        for (int i = 1; i < chrono.Count; i++) if (chrono[i].Issue.DateReported < chrono[i - 1].Issue.DateReported) inOrder = false;
        Check("chronological is oldest first", chrono.Count == 22 && inOrder);
        var range = svc.ReportedBetween(DateTime.Today.AddDays(-5), DateTime.Today);
        var rangeExpected = svc.All().Where(r => r.Issue.DateReported >= DateTime.Today.AddDays(-5) && r.Issue.DateReported < DateTime.Today.AddDays(1)).Select(r => r.Id).OrderBy(x => x).ToList();
        Check("date range matches a plain filter", range.Select(r => r.Id).OrderBy(x => x).SequenceEqual(rangeExpected) && range.Count > 0);
        Check("date range in the far future is empty", svc.ReportedBetween(DateTime.Today.AddDays(5), DateTime.Today.AddDays(9)).Count == 0);

        // heap: waiting = Submitted/UnderReview, ordered priority then age
        var waiting = svc.All().Where(r => r.Issue.Status == IssueStatus.Submitted || r.Issue.Status == IssueStatus.UnderReview).ToList();
        Check("heap holds exactly the waiting requests", svc.TriageQueue.Count == waiting.Count && svc.TriageQueue.IsValidHeap());
        var sorted = svc.TriageQueue.ToSortedList();
        bool orderOk = true;
        for (int i = 1; i < sorted.Count; i++)
        {
            if (sorted[i].Priority < sorted[i - 1].Priority) orderOk = false;
            if (sorted[i].Priority == sorted[i - 1].Priority && sorted[i].Issue.DateReported < sorted[i - 1].Issue.DateReported) orderOk = false;
        }
        Check("heap order: priority then oldest first", orderOk);
        Console.WriteLine("queue order: " + string.Join(", ", sorted.Take(8).Select(r => "#" + r.Id + "(P" + r.Priority + ", " + r.AgeDays + "d)")));
        int before = svc.TriageQueue.Count;
        var first = svc.AssignNextToCrew();
        Check("assign: top is a critical request, now in progress, heap shrinks", first.Priority == 1 && first.Issue.Status == IssueStatus.InProgress && svc.TriageQueue.Count == before - 1);
        Check("assign: the one taken was the oldest critical", first.Id == sorted[0].Id);

        // area tree
        var tree = svc.AreaTree;
        int leaves = tree.DepthFirst().Count(n => n.Value.Kind == AreaNodeKind.Request);
        Check("area tree: one leaf per request", leaves == 22 && tree.Height == 4);
        Check("area tree: wards in numeric order", string.Join(",", tree.Root.Children.Select(c => c.Value.Text.Split(' ')[1].Split(' ')[0])).StartsWith("1,2,3"));

        // related / clusters
        var related = svc.RelatedTo(2); // Ward 5 water burst; also #7 Ward 5 water, #5 Ward 5 waste...
        Console.WriteLine("related to #2: " + string.Join(", ", related.Take(8).Select(r => "L" + r.Level + ":#" + r.Info.Id + "(w" + r.LinkWeight + ")")));
        Check("related: includes same-ward same-category #7 at level 1 weight 1", related.Any(r => r.Info.Id == 7 && r.Level == 1 && r.LinkWeight == 1));
        Check("related: never includes itself", related.All(r => r.Info.Id != 2));
        var clusters = svc.IncidentClusters();
        Console.WriteLine("clusters: " + string.Join(" | ", clusters.Select(c => c.Category + " x" + c.Requests.Count + " in " + string.Join("+", c.Areas))));
        Check("clusters: each has 2+ requests of one category", clusters.All(c => c.Requests.Count >= 2 && c.Requests.All(r => r.Issue.Category == c.Category)));

        // area network
        var g = AreaNetwork.Graph;
        var k = g.MinimumSpanningTreeKruskal(); var p = g.MinimumSpanningTreePrim(AreaNetwork.Depot);
        Console.WriteLine("network: " + g.VertexCount + " vertices, " + g.EdgeCount + " edges; MST Kruskal " + k.TotalWeight.ToString("0.0") + " km, Prim " + p.TotalWeight.ToString("0.0") + " km, edges " + k.Edges.Count);
        Check("network: connected, 13 vertices, 12 MST edges", g.VertexCount == 13 && k.Edges.Count == 12 && k.SpansAllVertices && p.SpansAllVertices);
        Check("network: Kruskal == Prim", Math.Abs(k.TotalWeight - p.TotalWeight) < 1e-9);
        Check("network: single component", g.ConnectedComponents().Count == 1);
        Check("area parsing", AreaNetwork.AreaOf("Cnr Main Road and 5th Avenue, Ward 12") == "Ward 12" && AreaNetwork.AreaOf("somewhere") == "Unassigned" && AreaNetwork.AreaOf("Ward 99") == "Unassigned" && AreaNetwork.AreaOf("ward 3") == "Ward 3");
        Console.WriteLine(fail == 0 ? "ALL " + pass + " SERVICE CHECKS PASSED" : fail + " FAILED, " + pass + " passed");
    }
}
