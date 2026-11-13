using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Represents the possible lifecycle stages of a reported issue.
    /// Used to drive the "real-time feedback" engagement feature.
    /// </summary>
    public enum IssueStatus
    {
        Submitted,
        UnderReview,
        InProgress,
        Resolved
    }

    /// <summary>
    /// Represents a single issue reported by a citizen.
    /// </summary>
    public class Issue
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> AttachmentPaths { get; set; }
        public DateTime DateReported { get; set; }
        public IssueStatus Status { get; set; }

        public Issue()
        {
            AttachmentPaths = new List<string>();
            DateReported = DateTime.Now;
            Status = IssueStatus.Submitted;
        }
    }


    public static class IssueRepository
    {
        private static readonly List<Issue> _issues = new List<Issue>();
        private static int _nextId = 1;

        public static Issue AddIssue(string location, string category, string description, List<string> attachments)
        {
            var issue = new Issue
            {
                Id = _nextId++,
                Location = location,
                Category = category,
                Description = description,
                AttachmentPaths = attachments ?? new List<string>()
            };

            _issues.Add(issue);
            return issue;
        }

        public static List<Issue> GetAllIssues()
        {
            return _issues;
        }

        public static int GetTotalCount()
        {
            return _issues.Count;
        }

        public static int GetCountByStatus(IssueStatus status)
        {
            int count = 0;
            foreach (Issue issue in _issues)
            {
                if (issue.Status == status)
                {
                    count++;
                }
            }
            return count;
        }
    }
    public static class IssueStatusHelper
    {
        public static string GetShortText(IssueStatus status)
        {
            switch (status)
            {
                case IssueStatus.Submitted: return "Submitted";
                case IssueStatus.UnderReview: return "Under Review";
                case IssueStatus.InProgress: return "In Progress";
                default: return "Resolved";
            }
        }

        public static System.Drawing.Color GetColor(IssueStatus status)
        {
            switch (status)
            {
                case IssueStatus.Submitted: return System.Drawing.Color.FromArgb(41, 128, 185);
                case IssueStatus.UnderReview: return System.Drawing.Color.FromArgb(243, 156, 18);
                case IssueStatus.InProgress: return System.Drawing.Color.FromArgb(211, 84, 0);
                default: return System.Drawing.Color.FromArgb(39, 174, 96);
            }
        }
    }
}
