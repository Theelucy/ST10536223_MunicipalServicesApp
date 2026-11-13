# Municipal Services Application — Final PoE (Parts 1, 2 and 3)

A C# .NET Framework 4.8 Windows Forms application that lets residents of a South African
municipality **report service issues**, **find local events and announcements**, and **track the
status of their service requests**.

| Menu option | What it does | Built in |
|---|---|---|
| Report Issues | Capture an issue with location, category, description and attachments; live progress feedback | Part 1 |
| Local Events and Announcements | Browse, search and filter events; recommendations from search history | Part 2 |
| Service Request Status | Track requests by reference number; priority queue; area hierarchy; triage; network of wards | **Part 3** |

---

## 1. Requirements

- Windows 10 or 11
- Visual Studio 2019 or later (Community edition is fine) with the **.NET desktop development** workload
- .NET Framework 4.8 Developer Pack (Visual Studio offers to install it if missing)

No NuGet packages or external libraries are used. Every data structure in Part 3 is written from
scratch in the `DataStructures` folder (the only built-in collections used are `List`, `Dictionary`,
`HashSet`, `Stack` and `Queue`, as supporting tools).

## 2. How to compile and run

1. Copy the whole folder (the one containing `MunicipalServicesApp.sln`) to your Windows machine.
2. Double-click `MunicipalServicesApp.sln` to open it in Visual Studio.
3. Choose **Build → Build Solution** (Ctrl+Shift+B). It should finish with 0 errors.
4. Press **F5** (or Ctrl+F5 to run without debugging). The home dashboard opens.

The compiled program is also written to `MunicipalServicesApp\bin\Debug\MunicipalServicesApp.exe`.

## 3. How to use the application

### 3.1 Home dashboard
Three live stat cards (total / pending / resolved), three action cards, and a table of the five most
recently reported issues. Click an action card to open that feature. The numbers refresh every time
you return to the dashboard.

### 3.2 Report Issues (Part 1)
1. Enter a **Location**, choose a **Category** and write a **Description**.
2. Optionally click **Attach Image / Document** to attach files.
3. The progress bar fills as each required field is completed (the user-engagement feature).
4. Click **Submit Report**. You receive a reference number and the status moves through
   *Submitted → Under Review → In Progress → Resolved*.

### 3.3 Local Events and Announcements (Part 2)
Browse upcoming events, search by text, filter by category and date, and see events recommended from
your searches. Announcements are shown from a queue. Events are held in a sorted dictionary (by date),
a dictionary (by category) and a hash set (unique categories), with a priority queue for ordering.

### 3.4 Service Request Status (Part 3)

**First time:** if you have not reported any issues yet, click **Load Sample Requests** (bottom left).
It adds 22 realistic requests across 12 wards so every tab has data. They also appear on the home
dashboard. You can also report your own issues first; include the ward in the location (for example
*"12 Main Road, Ward 5"*) so the request can be placed in the area tree and the network.

The page has six tabs. The four stat cards at the top (Total, Pending, Resolved, Critical and open)
always reflect the current data.

| Tab | What you can do |
|---|---|
| **Track Requests** | Type a reference number (`12` or `#12`) and press **Track**. See a four-step progress tracker, the priority and how it was decided, the search cost in each tree, and *related requests*. Filter the list by status and by date range. |
| **Priority Queue** | See the requests waiting for a crew, most urgent first, and the same data drawn as a heap. Click **Assign next request to a crew** to take the top request (it becomes *In Progress*). |
| **Areas** | Browse *Municipality → Ward → Category → Request* as an expandable tree. Click any node for a summary; see depth-first and breadth-first orders. |
| **Triage** | See the binary decision tree that decides each request's priority. Pick a request to highlight its path. See the four tree traversals. |
| **Tree Lab** | The same reference numbers stored in a binary search tree, an AVL tree and a red-black tree, drawn side by side. Search for a number to see each tree's search path and comparison count. |
| **Area Network** | The ward road network as a graph. Show the minimum spanning tree (Kruskal or Prim), run a breadth-first or depth-first traversal from any ward, and see incident clusters. |

**A suggested walk-through (about two minutes):**
1. Open **Service Request Status** → **Load Sample Requests**.
2. **Track Requests:** type `22` and press Track. Note *"Found #22 in 5 comparisons"* and the search-cost box (BST 22, AVL 5, red-black 7).
3. **Tree Lab:** search for `22` again and compare the three pictures: the plain tree is a 22-level chain.
4. **Priority Queue:** press **Assign next request to a crew** twice. Watch the heap and the stat cards change.
5. **Triage:** choose different requests and watch the orange path change.
6. **Area Network:** choose *Kruskal*, then *Prim* (same 37.7 km), then set the traversal to *Breadth-first* from *Depot*.

---

## 4. Data structures in the Service Request Status feature

All of the structures below are in `MunicipalServicesApp/DataStructures/` and are used by
`Services/ServiceRequestService.cs`, which builds them from the list of reported issues each time the
page refreshes. *n* is the number of service requests.

### 4.1 Summary

| Structure | Source file | Used for | Key cost |
|---|---|---|---|
| General (basic) tree | `GeneralTree.cs` | Municipality → Ward → Category → Request hierarchy | Visit all nodes O(n) |
| Binary tree | `BinaryTree.cs` | Triage decision tree; four traversals | Classify in at most 3 questions |
| Binary search tree | `BinarySearchTree.cs` | Reference-number index (baseline) | O(height); O(n) worst case |
| AVL tree | `AvlTree.cs` | Reference-number index (balanced) | O(log n) guaranteed |
| Red-black tree | `RedBlackTree.cs` | Reference-number index, and the date index | O(log n) guaranteed |
| Binary heap | `BinaryHeap.cs` | Priority queue of requests waiting for a crew; Prim's algorithm | Insert / extract O(log n), peek O(1) |
| Graph (adjacency list) | `Graph.cs` | Ward road network; related-requests network | Neighbours in O(degree) |
| Graph traversal | `Graph.cs` | BFS, DFS, connected components | O(V + E) |
| Minimum spanning tree | `Graph.cs` | Kruskal (union-find) and Prim (heap) | O(E log E) |

`SearchTreeBase.cs` holds everything the three search trees share (search, in-order traversal, range
query, height, drawing). Each tree subclass implements only its own `Insert`.

### 4.2 General tree

**Role.** Organises requests by where they are and what they are about:
*Municipality → Ward → Category → Request*. Every request is a leaf; every ward and category is an
internal node that groups the nodes below it.

**Contribution to efficiency.** A question such as *"what water problems are open in Ward 5?"* is
answered by walking down two branches (Ward 5 → Water and Sewage) instead of checking every request.
The tab shows both traversals: depth-first (finish a ward before the next) and breadth-first (all
wards, then all categories, then all requests).

**Example.** In the sample data, Ward 5 holds 4 requests in 3 categories. Selecting the *Ward 5* node
summarises them by priority without touching the other 18 requests.

### 4.3 Binary tree (the triage decision tree)

**Role.** Every internal node asks one yes/no question; *left = Yes*, *right = No*; each leaf is a
priority. There is no ordering between nodes: it is a plain binary tree, which is what makes it
different from a search tree.

```
            Q1  Is it a safety or health hazard?
           /Yes                                  \No
   Q2  Has the service failed completely?      Q3  Does it affect many residents?
    /Yes            \No                          /Yes              \No
 Critical          High                       Medium       Q4  Open for more than 7 days?
                                                              /Yes             \No
                                                        Medium+ (escalated)    Low
```

**Contribution to efficiency.** A request is classified by following one path, so at most three
questions are asked however many rules are added further down. The four classic traversals are
implemented and displayed (pre-order, in-order, post-order, level-order); each visits every node once.

**Example.** *"Burst pipe is flooding the street and there is no water in the whole street"* →
Q1 Yes (Water and Sewage) → Q2 Yes ("no water") → **Critical**.
*"Pothole outside the house is getting bigger"* (8 days old) → Q1 No → Q3 No → Q4 Yes → **Medium (escalated)**.

### 4.4 Binary search tree (BST)

**Role.** The first of three indexes that find a request by its unique reference number: smaller keys
go left, larger keys go right.

**Contribution to efficiency, and its weakness.** Search takes as many steps as the tree is tall.
Reference numbers are issued in order (1, 2, 3, ...), so each new number is larger than all before it
and goes to the right every time. The tree becomes a chain.

**Example.** With the 22 sample requests the tree is 22 levels tall and finding #22 takes **22
comparisons**. With 1,000 requests it would take 1,000. This is why the Tree Lab includes the BST: it
shows the problem that the next two trees solve.

### 4.5 AVL tree

**Role.** The same index, but self-balancing: after each insert, every node's two subtrees are
compared and if their heights differ by more than 1 the node is rotated (single or double rotation).

**Contribution to efficiency.** Height is guaranteed to stay near log2(n), so search is O(log n)
even for keys that arrive in order. It is the tree the Track tab uses to find requests.

**Example.** The 22 requests give a tree 5 levels tall (the smallest possible), so #22 is found in
**5 comparisons** instead of 22. For 1,000 sequential requests the AVL height is 10 against 1,000 for
the plain BST.

### 4.6 Red-black tree

**Role.** A second self-balancing tree. Nodes are coloured red or black under three rules: the root is
black; a red node never has a red child; every path to a missing child has the same number of black
nodes. Inserts fix violations by recolouring and rotating.

**Contribution to efficiency.** The rules keep the longest path under twice the shortest, so search is
O(log n). It is balanced a little less strictly than an AVL tree, so lookups can need a few more
comparisons (7 against 5 for #22 in the sample data), but on random keys it needs fewer rotations.

**Measured:** inserting 500 random keys averaged 288 rotations for the red-black tree against 346 for
the AVL tree (about 17% fewer), with about the same height (11.0 against 10.9). With sequential keys the
two needed almost the same number of rotations (983 against 990 for 1,000 keys), but the AVL tree was
shorter (10 levels against 17).

**A second use: the date index.** A separate red-black tree is keyed by *date reported*. An in-order
walk gives requests oldest to newest with no sorting, and the *Reported between* filter is a **range
query** that skips every subtree outside the range, so the cost is O(log n + matches) instead of
checking all *n* requests.

### 4.7 Binary heap (priority queue)

**Role.** Keeps the requests that are waiting for a crew ordered by urgency: priority first
(Critical before Low), then the longest-waiting. It is stored in an array where the children of
position *i* are at 2*i*+1 and 2*i*+2, and the Priority Queue tab draws it as a tree.

**Contribution to efficiency.** The most urgent request is always at the top: *peek* is O(1). Adding a
request or removing the top one is O(log n); a plain sorted list would need O(n) to insert. The heap is
built from all waiting requests at once in O(n) (build-heap) rather than by *n* separate inserts.

**Example.** 14 of the 22 sample requests are waiting. Pressing **Assign next request to a crew**
removes #16 (Critical, a blocked sewage drain) and marks it *In Progress*; the heap re-orders itself and
#10 (Critical, a sinkhole) becomes the top. The same heap class also drives Prim's algorithm (4.9).

### 4.8 Graph, traversal and connected components

**Role.** Two graphs are stored as adjacency lists (each vertex keeps the list of edges touching it):

1. **The ward network**: 13 vertices (12 wards and the depot) and 23 roads; edge weight = kilometres.
2. **The related-requests network**: each request is a vertex; two requests are linked when they
   probably share a cause (same ward and category; same ward; or same category in neighbouring wards).

**Contribution to efficiency.** Finding a vertex's neighbours costs only its own number of edges, not a
scan of every pair. Traversals visit every vertex and edge once, O(V + E):

- **Breadth-first search (queue)** gives the fewest hops from the start. On the Track tab it lists the
  requests related to the one you are viewing, nearest first. On the network tab it gives the fewest
  roads from the depot.
- **Depth-first search (stack)** follows one road as far as it goes before backtracking, which gives a
  plausible inspection route.
- **Connected components** find groups of linked vertices. The network tab lists *incident clusters*:
  same-category requests linked within or across neighbouring wards, which may share one underlying fault.

**Example.** Request #2 (burst pipe, Ward 5) has #7 (also water, Ward 5) one hop away, and #17 (water,
Ward 6) one hop away because Ward 6 is next to Ward 5. The cluster *"Water and Sewage: 3 requests in
Ward 5, Ward 6"* suggests a single failing pipe, so one crew could fix all three.

### 4.9 Minimum spanning tree

**Role.** The cheapest set of roads that still connects every ward and the depot: the backbone for
planning inspection routes. Two algorithms are implemented and each is checked against the other.

- **Kruskal's algorithm:** sort all roads by length, then take the next shortest unless it would form
  a loop. A **union-find** structure (path compression, union by rank) answers *"are these two wards
  already connected?"* in almost constant time.
- **Prim's algorithm:** grow one tree outwards from the depot, always adding the shortest road to a
  new ward. The **binary heap** supplies the shortest road in O(log E).

**Contribution to efficiency.** Both run in O(E log E). Of the 23 roads, **12** are needed.

**Example.** Keeping all roads is 84.0 km; the minimum spanning tree is **37.7 km (55% less)**, and
Kruskal and Prim both give 37.7 km, which confirms the result. The tab draws the chosen roads thick and
green and lists them in the order the algorithm picked them.

---

## 5. Parts 1 and 2 in brief

- **Part 1: Report Issues.** Form with location text box, category list, rich-text description,
  file-dialog attachments, a progress bar (the engagement feature), message-box feedback, and a
  `List<Issue>` that stores every report. A grid of "My Reported Issues" updates live.
- **Part 2: Local Events and Announcements.** Events held in a sorted dictionary (by date), a
  dictionary (by category) and a hash set (unique categories); announcements in a queue; a priority
  queue; search and filter; recommendations based on previous searches.
- **Shared.** `AppTheme.cs` keeps colours and fonts consistent; `IssueRepository` is the single
  store the dashboard, Report Issues and Service Request Status all read, so a report made in Part 1
  appears straight away in Part 3.

## 6. Project structure

```
MunicipalServicesApp.sln
README.md
MunicipalServicesApp/
  Program.cs, AppTheme.cs, App.config
  Models/            Issue.cs (Issue, IssueRepository, status helpers), LocalEvent.cs
  DataStructures/    GeneralTree.cs  BinaryTree.cs  SearchTreeBase.cs  BinarySearchTree.cs
                     AvlTree.cs  RedBlackTree.cs  BinaryHeap.cs  Graph.cs  TreeLayout.cs
  Services/          ServiceRequestService.cs (builds every structure from the issue list)
                     TriageTree.cs  AreaNetwork.cs  SampleRequestLoader.cs
  Controls/          BinaryTreeView.cs  GraphView.cs  StatusStepper.cs   (custom-drawn controls)
  Forms/             MainMenuForm, ReportIssueForm, LocalEventsForm
                     ServiceRequestStatusForm (+ .Track / .Priority / .Areas / .Triage / .TreeLab / .Network)
Tests/               Optional console test programs (not part of the Visual Studio build)
```

## 7. Testing carried out

The data structures were tested separately from the interface with automated checks:

- **2,554 checks on the structures**, including: 60 rounds of random inserts compared with
  `SortedDictionary`; the AVL balance rule and all red-black rules verified after **every** insert;
  search and range queries compared with a plain filter; heap order after every insert; all four tree
  traversals on a known tree; BFS and DFS orders on a known graph; and, on 300 random connected graphs,
  Kruskal, Prim and a brute-force search all producing the same minimum total (plus a separate disconnected-graph case).
- **48 checks on the service layer**, including the priority of each of the 22 sample requests worked
  out by hand from the triage rules, heap ordering, lookups, date ranges and clusters.
- The interface was run with no requests, with the sample requests, and through the full path
  *menu → Service Request Status → load samples → back to the menu*.

The test programs are in the `Tests` folder (see the comment at the top of each file for the compile
command). They are console programs and are not part of the Visual Studio solution.

## 8. Known limitations

- Data is held in memory only; closing the application clears it. A database would be the next step.
- Statuses advance by simulation (Report Issues) or when a request is assigned to a crew; there is no
  back-office for staff to update them.
- Search trees support insert, search, traversal and range queries but not delete, because requests are
  never removed: they are closed by changing their status.
- The related-requests graph compares every pair of requests (O(n^2)); this is fine for hundreds of
  requests but a spatial index would be needed for many thousands.
- Wards are read from the location text ("Ward 5"); a request with no recognisable ward is grouped as
  *Unassigned* and is not placed on the network map.
