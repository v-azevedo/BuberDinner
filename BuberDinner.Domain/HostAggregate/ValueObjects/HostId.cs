using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.HostAggregate.ValueObjects;

public sealed class HostId : ValueObject
{
    public Guid Value { get; }

    public HostId(Guid value)
    {
        Value = value;
    }

    public static HostId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static HostId ConvertFrom(string value)
    {
        return new(Guid.Parse(value));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}