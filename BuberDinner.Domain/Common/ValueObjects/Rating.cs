using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;

namespace BuberDinner.Domain.Common.ValueObjects;

public sealed class Rating : ValueObject
{
    public Guid Id { get; }
    public HostId HostId { get; }
    public DinnerId DinnerId { get; }
    public float Value { get; }
    public DateTime CreatedDateTime { get; }
    public DateTime UpdatedDateTime { get; }

    private Rating(Guid id, HostId hostId, DinnerId dinnerId, float value, DateTime createdDateTime, DateTime updatedDateTime)
    {
        Id = id;
        HostId = hostId;
        DinnerId = dinnerId;
        Value = value;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return HostId;
        yield return DinnerId;
        yield return Value;
        yield return CreatedDateTime;
        yield return UpdatedDateTime;
    }
}