using System;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Adds a realistic set of service requests so the Service Request Status
    /// page can be explored without typing dozens of reports by hand. The
    /// requests go into the normal IssueRepository, so the dashboard and the
    /// Report Issues screen see them too.
    /// </summary>
    public static class SampleRequestLoader
    {
        public static bool HasLoaded { get; private set; }

        private class Sample
        {
            public string Location, Category, Description;
            public double DaysAgo;
            public IssueStatus Status;
        }

        private static Sample S(string location, string category, string description, double daysAgo, IssueStatus status)
        {
            return new Sample { Location = location, Category = category, Description = description, DaysAgo = daysAgo, Status = status };
        }

        /// <summary>Loads the sample requests once; returns how many were added (0 if already loaded).</summary>
        public static int Load()
        {
            if (HasLoaded) return 0;

            Sample[] samples =
            {
                S("Main Road, taxi rank entrance, Ward 3", "Roads and Potholes", "Large pothole at the taxi rank entrance is damaging vehicles and affects all commuters.", 12, IssueStatus.Resolved),
                S("14 Acacia Street, Ward 5", "Water and Sewage", "Burst pipe is flooding the street and there is no water in the whole street since this morning.", 2, IssueStatus.InProgress),
                S("Cnr Church St and 4th Ave, Ward 7", "Electricity", "Exposed sparking cable hanging low next to the school gate.", 1, IssueStatus.UnderReview),
                S("Riverside Park, Ward 2", "Parks and Public Spaces", "Broken swing and damaged fence in the playground.", 6, IssueStatus.Submitted),
                S("22 Protea Drive, Ward 5", "Waste Management", "Refuse has not been collected for two weeks and bags are piling up.", 9, IssueStatus.UnderReview),
                S("Ward 9 Community Hall, Ward 9", "Electricity", "Streetlights are out along the whole road since last week.", 8, IssueStatus.InProgress),
                S("8 Mimosa Lane, Ward 5", "Water and Sewage", "Slow leak under the pavement and the wet patch is growing.", 4, IssueStatus.Submitted),
                S("Market Square, Ward 1", "Sanitation", "Public toilets are blocked and overflowing, which is a health risk.", 3, IssueStatus.InProgress),
                S("Hillcrest Road, Ward 6", "Roads and Potholes", "Several potholes along the stretch near the garage.", 11, IssueStatus.Submitted),
                S("Station Road, Ward 3", "Roads and Potholes", "A sinkhole is opening near the intersection and the lane is partly blocked.", 1, IssueStatus.Submitted),
                S("17 Jacaranda Avenue, Ward 8", "Waste Management", "Illegal dumping on the empty plot next door.", 5, IssueStatus.Submitted),
                S("Ward 11 Clinic, Ward 11", "Electricity", "Power outage at the clinic with no electricity since yesterday.", 1, IssueStatus.InProgress),
                S("5 Baobab Close, Ward 12", "Water and Sewage", "Public standpipe is dripping constantly.", 7, IssueStatus.Submitted),
                S("Oak Avenue, Ward 10", "Parks and Public Spaces", "Overgrown grass near the park entrance.", 14, IssueStatus.Resolved),
                S("Cnr Elm and Pine, Ward 7", "Roads and Potholes", "Faded road markings and a missing stop sign in the area.", 3, IssueStatus.UnderReview),
                S("Ward 4 Library, Ward 4", "Sanitation", "Sewage smell and a blocked drain behind the library.", 2, IssueStatus.Submitted),
                S("30 Willow Way, Ward 6", "Water and Sewage", "Low water pressure for residents in the street.", 6, IssueStatus.UnderReview),
                S("Pine Crescent, Ward 9", "Electricity", "Faulty street light flickering at night.", 10, IssueStatus.Resolved),
                S("Lake View Road, Ward 2", "Waste Management", "Bin is overflowing at the bus stop.", 2, IssueStatus.Submitted),
                S("Ward 8 Soccer Field, Ward 8", "Parks and Public Spaces", "Floodlights are broken so neighbourhood teams cannot train.", 13, IssueStatus.Resolved),
                S("Cnr Main Road and 9th Street, Ward 3", "Water and Sewage", "Water leak from the main line is spreading across the road.", 3, IssueStatus.Submitted),
                S("40 Cedar Road, Ward 5", "Roads and Potholes", "Pothole outside the house is getting bigger.", 8, IssueStatus.Submitted)
            };

            foreach (Sample s in samples)
            {
                Issue issue = IssueRepository.AddIssue(s.Location, s.Category, s.Description, null);
                issue.DateReported = DateTime.Now.AddDays(-s.DaysAgo);
                issue.Status = s.Status;
            }

            HasLoaded = true;
            return samples.Length;
        }
    }
}
