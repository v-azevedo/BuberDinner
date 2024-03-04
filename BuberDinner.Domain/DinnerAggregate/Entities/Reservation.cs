using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.DinnerAggregate.Enums;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

namespace BuberDinner.Domain.DinnerAggregate.Entities;

public sealed class Reservation : Entity<ReservationId>
{
    public int GuestCount { get; }
    public ReservationStatus ReservationStatus { get; }
    public GuestId GuestId { get; }
    public BillId BillId { get; }
    public DateTime? ArrivalTime { get; }
    public DateTime CreatedDateTime { get; }
    public DateTime UpdatedDateTime { get; }

    public Reservation(ReservationId reservationId, GuestId guestId, BillId billId, DateTime createdDateTime, DateTime updatedDateTime)
        : base(reservationId)
    {
        GuestId = guestId;
        BillId = billId;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }
}