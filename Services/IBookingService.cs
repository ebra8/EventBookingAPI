// Services/IBookingService.cs
using EventBooking.Models;
using System.Collections.Generic;

namespace EventBooking.Services
{
    public interface IBookingService
    {
        IEnumerable<Event> GetAllEvents();
        ReservationViewModel GetReservation(string userId);
        ReservationViewModel AddEvent(string userId, string eventId, int ticketCount = 1);
        ReservationViewModel RemoveEvent(string userId, string eventId);
        BookingConfirmation CompleteReservation(string userId);
    }
}