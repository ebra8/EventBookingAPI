// Controllers/EventBookingController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EventBooking.Models;
using EventBooking.Services;
using System;

namespace EventBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public EventBookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("events")]
        public ActionResult<IEnumerable<Event>> GetAllEvents()
        {
            return Ok(_bookingService.GetAllEvents());
        }

        [HttpGet("reservations/{userId}")]
        public ActionResult<ReservationViewModel> GetUserReservation(string userId)
        {
            var reservation = _bookingService.GetReservation(userId);
            if (reservation == null)
            {
                return NotFound($"No active reservation found for user {userId}");
            }

            return Ok(reservation);
        }

        [HttpPost("reservations/{userId}/events")]
        public ActionResult<ReservationViewModel> AddEventToReservation(string userId, [FromBody] AddEventRequest request)
        {
            if (string.IsNullOrEmpty(request.EventId))
            {
                return BadRequest("Event ID is required");
            }

            try
            {
                var reservation = _bookingService.AddEvent(userId, request.EventId, request.TicketCount);
                return Ok(reservation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("reservations/{userId}/events/{eventId}")]
        public ActionResult<ReservationViewModel> RemoveEventFromReservation(string userId, string eventId)
        {
            try
            {
                var reservation = _bookingService.RemoveEvent(userId, eventId);
                return Ok(reservation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Event {eventId} not found in user's reservation");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reservations/{userId}/confirm")]
        public ActionResult<BookingConfirmation> ConfirmReservation(string userId)
        {
            try
            {
                var confirmation = _bookingService.CompleteReservation(userId);
                return Ok(confirmation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No active reservation found for user {userId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}