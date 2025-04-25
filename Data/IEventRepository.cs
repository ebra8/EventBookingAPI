// Data/IEventRepository.cs
using EventBooking.Models;
using System.Collections.Generic;

namespace EventBooking.Data
{
    public interface IEventRepository
    {
        Event? GetEventById(string id);
        IEnumerable<Event> GetAllEvents();
        void UpdateAvailableSeats(string eventId, int newAvailableSeats);
    }
}