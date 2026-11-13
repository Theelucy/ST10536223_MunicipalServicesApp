using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>A service request plus everything worked out about it: ward, priority and triage path.</summary>
    public class RequestInfo
    {
        public Issue Issue { get; set; }
        public string Area { get; set; }
        public TriageOutcome Triage { get; set; }

        public int Id { get { return Issue.Id; } }
        public int Priority { get { return Triage.Priority; } }
        public string PriorityLabel { get { return Triage.Label; } }
        public int AgeDays { get { return (int)(DateTime.Now - Issue.DateReported).TotalDays; } }
    }

    /// <summary>Sort key for the date index: reported time, then Id so two requests at the same instant stay distinct.</summary>
    public struct DateKey : IComparable<DateKey>
    {
        public long Ticks;
        public int Id;

        public DateKey(DateTime time, int id)
        {
            Ticks = time.Ticks;
            Id = id;
        }

        public int CompareTo(DateKey other)
        {
            int byTime = Ticks.CompareTo(other.Ticks);
            return byTime != 0 ? byTime : Id.CompareTo(other.Id);
        }
    }

    /// <summary>The outcome of tracking a request by reference number, with the cost in each tree.</summary>
    public class LookupResult
    {
        public int SearchedId { get; set; }
        public bool Found { get; set; }
        public RequestInfo Info { get; set; }
        public int BstComparisons { get; set; }
        public int AvlComparisons { get; set; }
        public int RbComparisons { get; set; }
    }

    public class RelatedRequest
    {
        public RequestInfo Info { get; set; }
        public int Level { get; set; }          // hops from the request being tracked
        public double LinkWeight { get; set; }  // 1 = same ward and category, 2 = same ward, 3 = same category in a neighbouring ward
    }

    /// <summary>A set of linked requests of the same category: possibly one underlying fault.</summary>
    public class IncidentCluster
    {
        public string Category { get; set; }
        public List<RequestInfo> Requests { get; set; }
        public List<string> Areas { get; set; }
    }

    public enum AreaNodeKind { Municipality, Ward, Category, Request }

    public class AreaNode
    {
        public string Text { get; set; }
        public AreaNodeKind Kind { get; set; }
        public RequestInfo Request { get; set; }
    }

    /// <summary>
    /// Builds every data structure the Service Request Status page needs from
    /// the current list of issues, and answers the page's questions with them:
    ///
    ///   Binary search tree   index by reference number (baseline)
    ///   AVL tree             index by reference number (balanced)
    ///   Red-black tree       index by reference number, and a second one by date reported
    ///   Binary heap          the queue of requests waiting for a crew, most urgent first
    ///   General tree         Municipality -> Ward -> Category -> Request
    ///   Binary tree          the triage decision tree (see TriageTree)
    ///   Graph                which requests are related to which (BFS / DFS / components)
    ///
    /// A fresh service is built each time the page refreshes, so it always
    /// reflects the latest statuses.
    /// </summary>
    public class ServiceRequestService
    {
        private readonly List<RequestInfo> _requests = new List<RequestInfo>();

        public BinarySearchTree<int, RequestInfo> BstById { get; private set; }
        public AvlTree<int, RequestInfo> AvlById { get; private set; }
        public RedBlackTree<int, RequestInfo> RbById { get; private set; }
        public RedBlackTree<DateKey, RequestInfo> RbByDate { get; private set; }
        public BinaryHeap<RequestInfo> TriageQueue { get; private set; }
        public GeneralTree<AreaNode> AreaTree { get; private set; }
        public Graph<int> RelatedGraph { get; private set; }

        public ServiceRequestService()
        {
            BstById = new BinarySearchTree<int, RequestInfo>();
            AvlById = new AvlTree<int, RequestInfo>();
            RbById = new RedBlackTree<int, RequestInfo>();
            RbByDate = new RedBlackTree<DateKey, RequestInfo>();

            // Requests arrive in reference-number order, so insert them that way.
            foreach (Issue issue in IssueRepository.GetAllIssues().OrderBy(i => i.Id))
            {
                var info = new RequestInfo
                {
                    Issue = issue,
                    Area = AreaNetwork.AreaOf(issue.Location),
                    Triage = TriageTree.Assess(issue)
                };
                _requests.Add(info);

                BstById.Insert(info.Id, info);
                AvlById.Insert(info.Id, info);
                RbById.Insert(info.Id, info);
                RbByDate.Insert(new DateKey(issue.DateReported, info.Id), info);
            }

            TriageQueue = BuildTriageQueue();
            AreaTree = BuildAreaTree();
            RelatedGraph = BuildRelatedGraph();
        }

        // ------------------------------------------------------------
        // Summary
        // ------------------------------------------------------------

        public int TotalCount { get { return _requests.Count; } }

        public int ResolvedCount
        {
            get { return _requests.Count(r => r.Issue.Status == IssueStatus.Resolved); }
        }

        public int PendingCount { get { return TotalCount - ResolvedCount; } }

        public int CriticalOpenCount
        {
            get { return _requests.Count(r => r.Priority == 1 && r.Issue.Status != IssueStatus.Resolved); }
        }

        public int OpenCountInArea(string area)
        {
            return _requests.Count(r => r.Area == area && r.Issue.Status != IssueStatus.Resolved);
        }

        // ------------------------------------------------------------
        // Tracking by unique identifier (the three Id indexes)
        // ------------------------------------------------------------

        public LookupResult Lookup(int id)
        {
            var result = new LookupResult { SearchedId = id };
            RequestInfo viaBst, viaAvl, viaRb;
            int bst, avl, rb;

            bool inBst = BstById.TryFind(id, out viaBst, out bst);
            bool inAvl = AvlById.TryFind(id, out viaAvl, out avl);
            bool inRb = RbById.TryFind(id, out viaRb, out rb);

            result.BstComparisons = bst;
            result.AvlComparisons = avl;
            result.RbComparisons = rb;
            result.Found = inAvl && inBst && inRb;
            result.Info = viaAvl; // the balanced tree is the one the page relies on
            return result;
        }

        // ------------------------------------------------------------
        // Ordered retrieval (the date index)
        // ------------------------------------------------------------

        /// <summary>Every request, oldest first - simply an in-order walk of the red-black date index.</summary>
        public List<RequestInfo> Chronological()
        {
            return RbByDate.InOrder().Select(kv => kv.Value).ToList();
        }

        /// <summary>Requests reported on or between the two dates - a range query that skips every other subtree.</summary>
        public List<RequestInfo> ReportedBetween(DateTime from, DateTime to)
        {
            var low = new DateKey(from.Date, int.MinValue);
            var high = new DateKey(to.Date.AddDays(1).AddTicks(-1), int.MaxValue);
            return RbByDate.Range(low, high);
        }

        // ------------------------------------------------------------
        // Priority queue (the heap)
        // ------------------------------------------------------------

        private BinaryHeap<RequestInfo> BuildTriageQueue()
        {
            // Requests still waiting for a crew. Built in O(n) with build-heap.
            IEnumerable<RequestInfo> waiting = _requests.Where(r =>
                r.Issue.Status == IssueStatus.Submitted || r.Issue.Status == IssueStatus.UnderReview);

            return new BinaryHeap<RequestInfo>(waiting, (a, b) =>
            {
                int byPriority = a.Priority.CompareTo(b.Priority);          // 1 (Critical) first
                if (byPriority != 0) return byPriority;
                int byAge = a.Issue.DateReported.CompareTo(b.Issue.DateReported); // then the longest-waiting
                return byAge != 0 ? byAge : a.Id.CompareTo(b.Id);
            });
        }

        /// <summary>Hands the most urgent waiting request to a crew: removes it from the heap and marks it In Progress.</summary>
        public RequestInfo AssignNextToCrew()
        {
            if (TriageQueue.IsEmpty) return null;

            RequestInfo next = TriageQueue.ExtractTop();
            next.Issue.Status = IssueStatus.InProgress;
            return next;
        }

        // ------------------------------------------------------------
        // Area tree (general tree)
        // ------------------------------------------------------------

        private GeneralTree<AreaNode> BuildAreaTree()
        {
            var tree = new GeneralTree<AreaNode>(new AreaNode
            {
                Kind = AreaNodeKind.Municipality,
                Text = "Municipality (" + _requests.Count + " requests)"
            });

            // Wards in numeric order with "Unassigned" last, then categories A-Z.
            var wards = _requests.Select(r => r.Area).Distinct()
                .OrderBy(a => a == AreaNetwork.Unassigned ? int.MaxValue : int.Parse(a.Substring(5)));

            foreach (string ward in wards)
            {
                List<RequestInfo> inWard = _requests.Where(r => r.Area == ward).ToList();
                TreeNode<AreaNode> wardNode = tree.Root.AddChild(new AreaNode
                {
                    Kind = AreaNodeKind.Ward,
                    Text = ward + " (" + inWard.Count + ")"
                });

                foreach (string category in inWard.Select(r => r.Issue.Category).Distinct().OrderBy(c => c))
                {
                    List<RequestInfo> inCategory = inWard.Where(r => r.Issue.Category == category).ToList();
                    TreeNode<AreaNode> categoryNode = wardNode.AddChild(new AreaNode
                    {
                        Kind = AreaNodeKind.Category,
                        Text = category + " (" + inCategory.Count + ")"
                    });

                    foreach (RequestInfo r in inCategory.OrderBy(x => x.Id))
                    {
                        categoryNode.AddChild(new AreaNode
                        {
                            Kind = AreaNodeKind.Request,
                            Request = r,
                            Text = "#" + r.Id + "  " + Shorten(r.Issue.Description, 40) + "  [" +
                                   IssueStatusHelper.GetShortText(r.Issue.Status) + "]"
                        });
                    }
                }
            }
            return tree;
        }

        // ------------------------------------------------------------
        // Related requests (graph)
        // ------------------------------------------------------------

        /// <summary>
        /// Requests are vertices. Two are linked if they probably share a cause:
        ///   weight 1  same ward and same category
        ///   weight 2  same ward, different category
        ///   weight 3  same category in neighbouring wards (per the road network)
        /// </summary>
        private Graph<int> BuildRelatedGraph()
        {
            var graph = new Graph<int>();
            foreach (RequestInfo r in _requests) graph.AddVertex(r.Id);

            for (int i = 0; i < _requests.Count; i++)
            {
                for (int j = i + 1; j < _requests.Count; j++)
                {
                    RequestInfo a = _requests[i];
                    RequestInfo b = _requests[j];
                    bool sameCategory = a.Issue.Category == b.Issue.Category;
                    bool sameWard = a.Area == b.Area && a.Area != AreaNetwork.Unassigned;

                    if (sameWard && sameCategory) graph.AddEdge(a.Id, b.Id, 1);
                    else if (sameWard) graph.AddEdge(a.Id, b.Id, 2);
                    else if (sameCategory && AreaNetwork.AreAdjacent(a.Area, b.Area)) graph.AddEdge(a.Id, b.Id, 3);
                }
            }
            return graph;
        }

        /// <summary>Everything connected to this request, nearest first (a breadth-first search).</summary>
        public List<RelatedRequest> RelatedTo(int id)
        {
            var related = new List<RelatedRequest>();
            if (!RelatedGraph.ContainsVertex(id)) return related;

            foreach (TraversalStep<int> step in RelatedGraph.BreadthFirst(id))
            {
                if (step.Vertex == id) continue;

                RequestInfo info;
                int cmp;
                AvlById.TryFind(step.Vertex, out info, out cmp);

                double weight = 0;
                foreach (Edge<int> e in RelatedGraph.Edges)
                {
                    if ((e.From == step.Vertex && e.To == step.Parent) || (e.To == step.Vertex && e.From == step.Parent))
                    {
                        weight = e.Weight;
                        break;
                    }
                }
                related.Add(new RelatedRequest { Info = info, Level = step.Level, LinkWeight = weight });
            }
            return related;
        }

        /// <summary>
        /// Groups of two or more requests of the same category that are linked within a ward or
        /// across neighbouring wards (links of weight 1 and 3). Found as the connected components of
        /// that sub-graph; a cluster spanning several wards hints at one underlying fault, such as
        /// a failing water main feeding all of them.
        /// </summary>
        public List<IncidentCluster> IncidentClusters()
        {
            var sameCategory = new Graph<int>();
            foreach (RequestInfo r in _requests) sameCategory.AddVertex(r.Id);
            foreach (Edge<int> e in RelatedGraph.Edges)
            {
                if (e.Weight != 2) sameCategory.AddEdge(e.From, e.To, e.Weight);
            }

            var clusters = new List<IncidentCluster>();
            foreach (List<int> component in sameCategory.ConnectedComponents())
            {
                if (component.Count < 2) continue;

                var group = new List<RequestInfo>();
                foreach (int id in component)
                {
                    RequestInfo info;
                    int cmp;
                    AvlById.TryFind(id, out info, out cmp);
                    group.Add(info);
                }
                group = group.OrderBy(g => g.Id).ToList();

                clusters.Add(new IncidentCluster
                {
                    Category = group[0].Issue.Category,
                    Requests = group,
                    Areas = group.Select(g => g.Area).Distinct().OrderBy(a => a).ToList()
                });
            }
            return clusters.OrderByDescending(c => c.Requests.Count).ToList();
        }

        public List<RequestInfo> All()
        {
            return _requests.ToList();
        }

        private static string Shorten(string text, int max)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= max) return text;
            return text.Substring(0, max).TrimEnd() + "...";
        }
    }
}
