using System;
using TemplateName.Shared.Abstractions.Time;

namespace TemplateName.Shared.Infrastructure.Time;

public class UtcClock : IClock
{
    public DateTime CurrentDate() => DateTime.UtcNow;
}

