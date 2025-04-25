// Models/EventModels.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBooking.Models
{
    public class Event
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class ReservationViewModel
    {
        public required string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<ReservationItem> Items { get; set; } = new List<ReservationItem>();
        public decimal TotalPrice => Items.Sum(item => item.Price * item.TicketCount);
        public int TotalTickets => Items.Sum(item => item.TicketCount);
    }

    public class ReservationItem
    {
        public required string EventId { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }
        public int TicketCount { get; set; }
        public decimal Price { get; set; }
    }

    public class AddEventRequest
    {
        public required string EventId { get; set; }
        public int TicketCount { get; set; } = 1;
    }

    public class BookingConfirmation
    {
        public required string BookingId { get; set; }
        public required string UserId { get; set; }
        public required List<ReservationItem> BookedEvents { get; set; } = new List<ReservationItem>();
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public required string ConfirmationCode { get; set; }
        public List<string> TicketCodes { get; set; } = new List<string>();
    }
}