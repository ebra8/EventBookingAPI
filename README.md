# Event Booking API

A robust RESTful API for managing event bookings, built with .NET 8.0. This system handles event listings, reservations, and ticket management with real-time seat availability tracking.

## Table of Contents

- [Features](#features)
- [Technical Stack](#technical-stack)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Future Enhancements](#future-enhancements)

## Features

- **Event Management**: List and manage available events
- **Reservation System**: Handle event reservations with automatic timeout
- **Seat Availability**: Real-time tracking of available seats
- **Ticket Booking**: Book and confirm tickets for events
- **User Reservations**: Manage user-specific reservations
- **RESTful Design**: Follows REST API best practices

## Technical Stack

- **Backend**: .NET 8.0
- **Architecture**: REST API
- **Data Storage**: In-memory (with mock repository)
- **Documentation**: Swagger/OpenAPI
- **Development Tools**: Visual Studio Code, Postman

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or any .NET IDE
- Postman (for API testing)

### Installation

1. Clone the repository

```bash
git clone [your-repository-url]
```

2. Navigate to the project directory

```bash
cd EventBookingAPI
```

3. Restore dependencies

```bash
dotnet restore
```

4. Build the project

```bash
dotnet build
```

5. Run the application

```bash
dotnet run
```

The API will be available at:

- HTTP: http://localhost:5261
- HTTPS: https://localhost:7261

## API Documentation

### Base URL

```
http://localhost:5261/api/eventbooking
```

### Endpoints

#### 1. Get All Events

```http
GET /events
```

Returns a list of all available events.

**Response**

```json
[
  {
    "id": "evt-001",
    "name": "Tech Conference 2025",
    "description": "Annual technology conference...",
    "eventDate": "2025-06-26T01:31:03.123Z",
    "location": "San Francisco Convention Center",
    "ticketPrice": 299.99,
    "availableSeats": 500
  }
]
```

#### 2. Get User's Reservation

```http
GET /reservations/{userId}
```

Returns the current reservation for a specific user.

**Response**

```json
{
  "userId": "user123",
  "createdAt": "2024-04-26T01:31:03.123Z",
  "expiresAt": "2024-04-26T01:46:03.123Z",
  "items": [
    {
      "eventId": "evt-001",
      "eventName": "Tech Conference 2025",
      "eventDate": "2025-06-26T01:31:03.123Z",
      "location": "San Francisco Convention Center",
      "ticketCount": 2,
      "price": 299.99
    }
  ],
  "totalPrice": 599.98,
  "totalTickets": 2
}
```

#### 3. Add Event to Reservation

```http
POST /reservations/{userId}/events
```

Adds an event to the user's reservation.

**Request Body**

```json
{
  "eventId": "evt-001",
  "ticketCount": 2
}
```

#### 4. Remove Event from Reservation

```http
DELETE /reservations/{userId}/events/{eventId}
```

Removes an event from the user's reservation.

#### 5. Confirm Reservation

```http
POST /reservations/{userId}/confirm
```

Confirms the user's reservation and generates tickets.

**Response**

```json
{
    "bookingId": "guid",
    "userId": "user123",
    "bookedEvents": [...],
    "totalAmount": 599.98,
    "bookingDate": "2024-04-26T01:31:03.123Z",
    "confirmationCode": "BK-20240426-ABC123",
    "ticketCodes": ["TIX-001-ABC123", "TIX-001-DEF456"]
}
```

## Project Structure

```
EventBookingAPI/
├── Controllers/
│   └── EventBookingController.cs    # API endpoints
├── Models/
│   └── EventModels.cs              # Data models
├── Services/
│   └── BookingService.cs           # Business logic
├── Data/
│   ├── IEventRepository.cs         # Repository interface
│   └── MockEventRepository.cs      # Mock implementation
└── Program.cs                      # Application setup
```

## Testing

The API can be tested using Postman or any HTTP client. A test file `EventBookingAPI.http` is included in the project for easy testing.

## Future Enhancements

- Authentication and Authorization
- Database Integration
- Payment Processing
- Email Notifications
- Frontend Application
- Real-time Updates
- Analytics Dashboard
