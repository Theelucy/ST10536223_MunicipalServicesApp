using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>One node of the triage decision tree: a yes/no question or a final priority.</summary>
    public class TriageNode
    {
        public string Code { get; set; }                 // short name used in the diagram (Q1, Critical...)
        public string Text { get; set; }                 // the full question / outcome
        public Func<Issue, bool> Test { get; set; }      // null for a leaf
        public int Priority { get; set; }                // leaves only: 1 = Critical ... 4 = Low

        public bool IsLeaf
        {
            get { return Test == null; }
        }
    }

    public class TriageOutcome
    {
        public int Priority { get; set; }
        public string Label { get; set; }
        public List<string> Codes { get; set; }          // nodes visited, e.g. Q1, Q2, Critical
        public List<string> Steps { get; set; }          // readable path: "Is it a hazard? Yes"
    }

    /// <summary>
    /// Decides how urgent a service request is by walking a binary decision
    /// tree. Each question has exactly two answers (left = yes, right = no),
    /// so a request is classified in at most three questions no matter how
    /// many rules the municipality later adds below.
    ///
    ///            Q1 safety / health hazard?
    ///           /yes                      \no
    ///   Q2 service failed completely?   Q3 affects many residents?
    ///    /yes        \no                  /yes          \no
    ///  Critical     High               Medium     Q4 open over 7 days?
    ///                                              /yes        \no
    ///                                        Medium+(escalated)  Low
    /// </summary>
    public static class TriageTree
    {
        public static readonly string[] PriorityNames = { "", "Critical", "High", "Medium", "Low" };

        // Words that mark a safety or health hazard (matched at the start of a word, so "leak" also finds "leaking").
        private static readonly string[] HazardWords =
        {
            "burst", "leak", "flooding", "flooded", "sewage", "spill", "exposed", "spark", "fire",
            "gas leak", "collapse", "sinkhole", "danger", "hazard", "injur", "electrocut", "health risk"
        };

        // Words that show a service has stopped altogether.
        private static readonly string[] FailureWords =
        {
            "no water", "no power", "no electricity", "outage", "blackout", "blocked",
            "cut off", "completely", "not working", "sinkhole"
        };

        // Words that show many people are affected.
        private static readonly string[] WideImpactWords =
        {
            "street", "area", "everyone", "residents", "taxi rank", "school", "clinic",
            "whole", "entire", "neighbourhood", "community", "commuters"
        };

        public static readonly BinaryTree<TriageNode> Tree = Build();

        private static BinaryTree<TriageNode> Build()
        {
            var q1 = new BinaryTreeNode<TriageNode>(new TriageNode
            {
                Code = "Q1",
                Text = "Is it a safety or health hazard?",
                Test = IsHazard
            });
            var q2 = new BinaryTreeNode<TriageNode>(new TriageNode
            {
                Code = "Q2",
                Text = "Has the service failed completely?",
                Test = i => ContainsAny(i, FailureWords)
            });
            var q3 = new BinaryTreeNode<TriageNode>(new TriageNode
            {
                Code = "Q3",
                Text = "Does it affect many residents?",
                Test = i => ContainsAny(i, WideImpactWords)
            });
            var q4 = new BinaryTreeNode<TriageNode>(new TriageNode
            {
                Code = "Q4",
                Text = "Has it been open for more than 7 days?",
                Test = i => (DateTime.Now - i.DateReported).TotalDays > 7
            });

            q1.Left = q2;
            q1.Right = q3;
            q2.Left = Leaf("Critical", 1, "Critical - dispatch immediately");
            q2.Right = Leaf("High", 2, "High - attend within 24 hours");
            q3.Left = Leaf("Medium", 3, "Medium - schedule this week");
            q3.Right = q4;
            q4.Left = Leaf("Medium+", 3, "Medium (escalated because it has waited over a week)");
            q4.Right = Leaf("Low", 4, "Low - routine maintenance");

            return new BinaryTree<TriageNode>(q1);
        }

        private static BinaryTreeNode<TriageNode> Leaf(string code, int priority, string text)
        {
            return new BinaryTreeNode<TriageNode>(new TriageNode { Code = code, Text = text, Priority = priority });
        }

        /// <summary>Walks the tree from the root, answering each question for this request.</summary>
        public static TriageOutcome Assess(Issue issue)
        {
            var outcome = new TriageOutcome { Codes = new List<string>(), Steps = new List<string>() };
            BinaryTreeNode<TriageNode> node = Tree.Root;

            while (node != null)
            {
                outcome.Codes.Add(node.Value.Code);

                if (node.Value.IsLeaf)
                {
                    outcome.Priority = node.Value.Priority;
                    outcome.Label = PriorityNames[node.Value.Priority];
                    outcome.Steps.Add("Result: " + node.Value.Text);
                    break;
                }

                bool answer = node.Value.Test(issue);
                outcome.Steps.Add(node.Value.Code + ": " + node.Value.Text + "  " + (answer ? "Yes" : "No"));
                node = answer ? node.Left : node.Right;
            }
            return outcome;
        }

        // Water and electricity faults are treated as hazards by default; anything else
        // is a hazard only if the description says so.
        private static bool IsHazard(Issue issue)
        {
            return issue.Category == "Water and Sewage" ||
                   issue.Category == "Electricity" ||
                   ContainsAny(issue, HazardWords);
        }

        // Only the description is searched: the location always contains words such as
        // "Ward" or "Road" that say nothing about how serious the problem is.
        private static bool ContainsAny(Issue issue, string[] words)
        {
            string text = (issue.Description ?? "").ToLowerInvariant();
            foreach (string w in words)
            {
                if (Regex.IsMatch(text, @"\b" + Regex.Escape(w)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
