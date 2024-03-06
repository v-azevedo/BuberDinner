using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.UserAggregate.ValueObjects;

namespace BuberDinner.Domain.HostAggregate.ValueObjects;

public sealed class HostId : ValueObject
{
    public string Value { get; private set; }

    private HostId(string value)
    {
        Value = value;
    }

    public static HostId Create(UserId userId)
    {
        return new($"Host-{userId}");
    }

    public static HostId Create(string hostId)
    {
        return new(hostId);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}