// Services/BookingService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EventBooking.Models;
using EventBooking.Data;

namespace EventBooking.Services
{
    public class BookingService : IBookingService
    {
        // In-memory storage for user reservations
        private static readonly Dictionary<string, ReservationViewModel> _userReservations = new Dictionary<string, ReservationViewModel>();
        private readonly IEventRepository _eventRepository;

        // Reservation timeout in minutes
        private const int RESERVATION_TIMEOUT_MINUTES = 15;

        public BookingService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _eventRepository.GetAllEvents();
        }

        public ReservationViewModel GetReservation(string userId)
        {
            CleanExpiredReservations();

            if (!_userReservations.TryGetValue(userId, out var reservation))
            {
                // Create a new reservation for the user if one doesn't exist
                reservation = new ReservationViewModel 
                { 
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(RESERVATION_TIMEOUT_MINUTES)
                };
                _userReservations[userId] = reservation;
            }

            return reservation;
        }

        public ReservationViewModel AddEvent(string userId, string eventId, int ticketCount = 1)
        {
            if (ticketCount <= 0)
            {
                throw new ArgumentException("Ticket count must be greater than 0");
            }

            // Get event details
            var eventDetails = _eventRepository.GetEventById(eventId);
            if (eventDetails == null)
            {
                throw new ArgumentException($"Event with ID {eventId} not found");
            }

            // Check seat availability
            if (eventDetails.AvailableSeats < ticketCount)
            {
                throw new ArgumentException($"Not enough available seats. Requested: {ticketCount}, Available: {eventDetails.AvailableSeats}");
            }

            var reservation = GetReservation(userId);

            // Check if the event is already in the reservation
            var existingItem = reservation.Items.FirstOrDefault(i => i.EventId == eventId);
            if (existingItem != null)
            {
                // Update ticket count of existing item
                int additionalTickets = ticketCount;
                int newTotalTickets = existingItem.TicketCount + additionalTickets;
                
                if (eventDetails.AvailableSeats < additionalTickets)
                {
                    throw new ArgumentException($"Not enough available seats for additional tickets");
                }
                
                existingItem.TicketCount = newTotalTickets;
            }
            else
            {
                // Add new event to reservation
                reservation.Items.Add(new ReservationItem
                {
                    EventId = eventDetails.Id,
                    EventName = eventDetails.Name,
                    EventDate = eventDetails.EventDate,
                    Location = eventDetails.Location,
                    Price = eventDetails.TicketPrice,
                    TicketCount = ticketCount
                });
            }

            // Reset expiration time
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(RESERVATION_TIMEOUT_MINUTES);

            return reservation;
        }

        public ReservationViewModel RemoveEvent(string userId, string eventId)
        {
            var reservation = GetReservation(userId);
            var item = reservation.Items.FirstOrDefault(i => i.EventId == eventId);
            
            if (item == null)
            {
                throw new KeyNotFoundException($"Event {eventId} not found in reservation");
            }

            reservation.Items.Remove(item);
            
            // Reset expiration time if items still exist
            if (reservation.Items.Any())
            {
                reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(RESERVATION_TIMEOUT_MINUTES);
            }
            
            return reservation;
        }

        public BookingConfirmation CompleteReservation(string userId)
        {
            CleanExpiredReservations();
            
            if (!_userReservations.TryGetValue(userId, out var reservation) || !reservation.Items.Any())
            {
                throw new KeyNotFoundException("No active reservation found or reservation is empty");
            }

            // Check if reservation is expired
            if (DateTime.UtcNow > reservation.ExpiresAt)
            {
                _userReservations.Remove(userId);
                throw new InvalidOperationException("Reservation has expired");
            }

            // Verify that all events still have enough available seats
            foreach (var item in reservation.Items)
            {
                var eventDetails = _eventRepository.GetEventById(item.EventId);
                if (eventDetails == null)
                {
                    throw new InvalidOperationException($"Event {item.EventName} no longer exists");
                }
                
                if (eventDetails.AvailableSeats < item.TicketCount)
                {
                    throw new InvalidOperationException($"Event {item.EventName} no longer has enough available seats");
                }

                // Update available seats (in a real application, this would be done in a transaction)
                _eventRepository.UpdateAvailableSeats(item.EventId, eventDetails.AvailableSeats - item.TicketCount);
            }

            // Generate ticket codes
            var ticketCodes = new List<string>();
            foreach (var item in reservation.Items)
            {
                for (int i = 0; i < item.TicketCount; i++)
                {
                    ticketCodes.Add(GenerateTicketCode(item.EventId));
                }
            }

            // Create a confirmation
            var bookingConfirmation = new BookingConfirmation
            {
                BookingId = Guid.NewGuid().ToString(),
                UserId = userId,
                BookedEvents = new List<ReservationItem>(reservation.Items),
                TotalAmount = reservation.TotalPrice,
                BookingDate = DateTime.UtcNow,
                ConfirmationCode = GenerateConfirmationCode(),
                TicketCodes = ticketCodes
            };

            // Remove the reservation after booking is complete
            _userReservations.Remove(userId);

            return bookingConfirmation;
        }

        private void CleanExpiredReservations()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _userReservations.Where(kv => kv.Value.ExpiresAt < now).Select(kv => kv.Key).ToList();
            
            foreach (var key in expiredKeys)
            {
                _userReservations.Remove(key);
            }
        }

        private string GenerateConfirmationCode()
        {
            return $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private string GenerateTicketCode(string eventId)
        {
            return $"TIX-{eventId.Substring(0, Math.Min(4, eventId.Length))}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
    }
}