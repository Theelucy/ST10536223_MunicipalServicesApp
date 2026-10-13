using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.Models
{
    public class LocalEvent : IComparable<LocalEvent>
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public List<string> Tags { get; set; }

        public LocalEvent()
        {
            Tags = new List<string>();
        }

        public int CompareTo(LocalEvent other)
        {
            if (other == null)
            {
                return 1;
            }

            int priorityCompare = other.Priority.CompareTo(Priority);
            if (priorityCompare != 0)
            {
                return priorityCompare;
            }

            return EventDate.CompareTo(other.EventDate);
        }
    }

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> _items = new List<T>();

        public int Count
        {
            get { return _items.Count; }
        }

        public void Enqueue(T item)
        {
            _items.Add(item);
            _items.Sort();
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            T item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return item;
        }

        public List<T> ToList()
        {
            return new List<T>(_items);
        }
    }

    public static class LocalEventRepository
    {
        private static readonly List<LocalEvent> _events = new List<LocalEvent>();
        private static readonly Queue<string> _announcements = new Queue<string>();
        private static readonly SortedDictionary<DateTime, List<LocalEvent>> _eventsByDate =
            new SortedDictionary<DateTime, List<LocalEvent>>();
        private static readonly Dictionary<string, List<LocalEvent>> _eventsByCategory =
            new Dictionary<string, List<LocalEvent>>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> _categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static LocalEventRepository()
        {
            SeedData();
            RebuildIndexes();
        }

        private static void SeedData()
        {
            var seedEvents = new[]
            {
                new LocalEvent
                {
                    Title = "Spring Community Clean-Up",
                    Category = "Environment",
                    EventDate = DateTime.Today.AddDays(2),
                    Location = "Voortrekker Road Park",
                    Description = "Residents are invited to help clean parks, streets and public spaces across the area.",
                    Priority = 5,
                    Tags = new List<string> { "clean", "environment", "community" }
                },
                new LocalEvent
                {
                    Title = "Water Conservation Awareness Workshop",
                    Category = "Education",
                    EventDate = DateTime.Today.AddDays(5),
                    Location = "Municipal Hall",
                    Description = "Learn practical ways to save water and understand municipal water-saving initiatives.",
                    Priority = 4,
                    Tags = new List<string> { "water", "learning", "conservation" }
                },
                new LocalEvent
                {
                    Title = "Youth Sports Festival",
                    Category = "Recreation",
                    EventDate = DateTime.Today.AddDays(7),
                    Location = "City Stadium",
                    Description = "A day of football, athletics and arts activities for local youth and families.",
                    Priority = 4,
                    Tags = new List<string> { "sports", "youth", "family" }
                },
                new LocalEvent
                {
                    Title = "Road Safety Campaign",
                    Category = "Safety",
                    EventDate = DateTime.Today.AddDays(9),
                    Location = "Main Road Junction",
                    Description = "Interactive campaign focusing on pedestrian safety, cycling awareness and safer road use.",
                    Priority = 5,
                    Tags = new List<string> { "road", "safety", "traffic" }
                },
                new LocalEvent
                {
                    Title = "Local Business Expo",
                    Category = "Business",
                    EventDate = DateTime.Today.AddDays(12),
                    Location = "Town Centre",
                    Description = "Meet local vendors, learn about municipal procurement opportunities and support small businesses.",
                    Priority = 3,
                    Tags = new List<string> { "business", "market", "local" }
                },
                new LocalEvent
                {
                    Title = "Senior Citizens Wellness Day",
                    Category = "Health",
                    EventDate = DateTime.Today.AddDays(15),
                    Location = "Community Centre",
                    Description = "Free health screenings, wellness advice and social activities for senior residents.",
                    Priority = 4,
                    Tags = new List<string> { "health", "wellness", "seniors" }
                },
                new LocalEvent
                {
                    Title = "Library Story Time",
                    Category = "Education",
                    EventDate = DateTime.Today.AddDays(4),
                    Location = "Central Library",
                    Description = "An interactive reading session for children and parents with storytelling and literacy activities.",
                    Priority = 2,
                    Tags = new List<string> { "library", "children", "reading" }
                },
                new LocalEvent
                {
                    Title = "Street Lighting Upgrade Briefing",
                    Category = "Infrastructure",
                    EventDate = DateTime.Today.AddDays(18),
                    Location = "Old Main Road",
                    Description = "Residents can learn about the street-lighting upgrade project and expected service improvements.",
                    Priority = 5,
                    Tags = new List<string> { "lighting", "infrastructure", "service" }
                }
            };

            foreach (var localEvent in seedEvents)
            {
                _events.Add(localEvent);
            }

            _announcements.Enqueue("Road maintenance works will continue along Main Road this week.");
            _announcements.Enqueue("The municipal library is hosting a children’s reading day next Saturday.");
            _announcements.Enqueue("Public parks are open and available for community clean-up activities.");
            _announcements.Enqueue("Water-saving reminders are being shared across all municipal districts.");
        }

        private static void RebuildIndexes()
        {
            _eventsByDate.Clear();
            _eventsByCategory.Clear();
            _categories.Clear();

            foreach (var localEvent in _events)
            {
                _categories.Add(localEvent.Category);

                DateTime key = localEvent.EventDate.Date;
                if (!_eventsByDate.ContainsKey(key))
                {
                    _eventsByDate[key] = new List<LocalEvent>();
                }
                _eventsByDate[key].Add(localEvent);

                if (!_eventsByCategory.ContainsKey(localEvent.Category))
                {
                    _eventsByCategory[localEvent.Category] = new List<LocalEvent>();
                }
                _eventsByCategory[localEvent.Category].Add(localEvent);
            }

            foreach (var eventList in _eventsByDate.Values)
            {
                eventList.Sort((left, right) => left.EventDate.CompareTo(right.EventDate));
            }

            foreach (var eventList in _eventsByCategory.Values)
            {
                eventList.Sort((left, right) => right.Priority.CompareTo(left.Priority));
            }
        }

        public static List<LocalEvent> GetAllEvents()
        {
            return _events.OrderBy(e => e.EventDate).ThenByDescending(e => e.Priority).ToList();
        }

        public static List<string> GetCategories()
        {
            return _categories.OrderBy(c => c).ToList();
        }

        public static List<LocalEvent> FilterEvents(string searchText, string category)
        {
            IQueryable<LocalEvent> query = GetAllEvents().AsQueryable();
            string searchTerm = (searchText ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(lowerTerm) ||
                    e.Category.ToLower().Contains(lowerTerm) ||
                    e.Location.ToLower().Contains(lowerTerm) ||
                    e.Description.ToLower().Contains(lowerTerm) ||
                    e.Tags.Any(tag => tag.ToLower().Contains(lowerTerm))
                );
            }

            if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All Categories", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderBy(e => e.EventDate).ThenByDescending(e => e.Priority).ToList();
        }

        public static List<LocalEvent> GetRecommendedEvents(string searchText)
        {
            List<LocalEvent> events = GetAllEvents();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return events.OrderByDescending(e => e.Priority).ThenBy(e => e.EventDate).Take(3).ToList();
            }

            string input = searchText.Trim();
            var scoredEvents = new List<Tuple<LocalEvent, int>>();

            foreach (var localEvent in events)
            {
                int score = 0;
                string haystack = (localEvent.Title + " " + localEvent.Category + " " + localEvent.Location + " " +
                                   localEvent.Description + " " + string.Join(" ", localEvent.Tags)).ToLower();
                string lowerInput = input.ToLower();

                if (haystack.Contains(lowerInput))
                {
                    score += 12;
                }

                string[] terms = lowerInput.Split(new[] { ' ', ',', ';', '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string term in terms)
                {
                    if (localEvent.Category.ToLower().Contains(term) ||
                        localEvent.Title.ToLower().Contains(term) ||
                        localEvent.Location.ToLower().Contains(term) ||
                        localEvent.Tags.Any(tag => tag.ToLower().Contains(term)))
                    {
                        score += 5;
                    }
                }

                score += localEvent.Priority * 2;
                scoredEvents.Add(Tuple.Create(localEvent, score));
            }

            return scoredEvents
                .Where(result => result.Item2 > 0)
                .OrderByDescending(result => result.Item2)
                .ThenByDescending(result => result.Item1.Priority)
                .Select(result => result.Item1)
                .Take(3)
                .ToList();
        }

        public static string GetNextAnnouncement()
        {
            if (_announcements.Count == 0)
            {
                return "There are no current announcements.";
            }

            string announcement = _announcements.Dequeue();
            _announcements.Enqueue(announcement);
            return announcement;
        }

        public static PriorityQueue<LocalEvent> BuildPriorityQueue()
        {
            var orderedQueue = new PriorityQueue<LocalEvent>();
            foreach (var localEvent in GetAllEvents())
            {
                orderedQueue.Enqueue(localEvent);
            }

            return orderedQueue;
        }

        public static SortedDictionary<DateTime, List<LocalEvent>> GetEventsByDate()
        {
            return _eventsByDate;
        }

        public static Dictionary<string, List<LocalEvent>> GetEventsByCategory()
        {
            return _eventsByCategory;
        }
    }
}
