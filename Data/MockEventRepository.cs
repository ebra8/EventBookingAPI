// Data/MockEventRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EventBooking.Models;

namespace EventBooking.Data
{
    public class MockEventRepository : IEventRepository
    {
        private readonly List<Event> _events = new List<Event>
        {
            new Event
            {
                Id = "evt-001",
                Name = "Tech Conference 2025",
                Description = "Annual technology conference featuring the latest innovations",
                EventDate = DateTime.UtcNow.AddMonths(2),
                Location = "San Francisco Convention Center",
                TicketPrice = 299.99m,
                AvailableSeats = 500
            },
            new Event
            {
                Id = "evt-002",
                Name = "Summer Music Festival",
                Description = "Three days of music from top artists across multiple genres",
                EventDate = DateTime.UtcNow.AddMonths(3),
                Location = "Golden Gate Park",
                TicketPrice = 149.99m,
                AvailableSeats = 2000
            },
            new Event
            {
                Id = "evt-003",
                Name = "Startup Pitch Competition",
                Description = "Emerging startups compete for investor funding",
                EventDate = DateTime.UtcNow.AddMonths(1),
                Location = "Downtown Business Center",
                TicketPrice = 79.99m,
                AvailableSeats = 200
            },
            new Event
            {
                Id = "evt-004",
                Name = "Digital Marketing Workshop",
                Description = "Learn the latest digital marketing strategies from industry experts",
                EventDate = DateTime.UtcNow.AddDays(14),
                Location = "Online Webinar",
                TicketPrice = 49.99m,
                AvailableSeats = 1000
            }
        };

        public Event? GetEventById(string id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _events;
        }

        public void UpdateAvailableSeats(string eventId, int newAvailableSeats)
        {
            var eventItem = _events.FirstOrDefault(e => e.Id == eventId);
            if (eventItem != null)
            {
                eventItem.AvailableSeats = newAvailableSeats;
            }
        }
    }
}