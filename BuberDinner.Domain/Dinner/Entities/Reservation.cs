using BuberDinner.Domain.Bill.ValueObjects;
using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.Dinner.Enums;
using BuberDinner.Domain.Dinner.ValueObjects;
using BuberDinner.Domain.Guest.ValueObjects;

namespace BuberDinner.Domain.Dinner.Entities;

public sealed class Reservation : Entity<ReservationId>
{
    public int GuestCount { get; private set; }
    public ReservationStatus ReservationStatus { get; private set; }
    public GuestId GuestId { get; private set; }
    public BillId BillId { get; private set; }
    public DateTime? ArrivalTime { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public Reservation(ReservationId reservationId, GuestId guestId, BillId billId, DateTime createdDateTime, DateTime updatedDateTime)
        : base(reservationId)
    {
        GuestId = guestId;
        BillId = billId;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }
}