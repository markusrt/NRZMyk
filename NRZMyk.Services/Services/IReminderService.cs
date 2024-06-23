using NRZMyk.Services.Data.Entities;
using System;

namespace NRZMyk.Services.Services;

public interface IReminderService
{
    public string HumanReadableExpectedNextSending(Organization organization);
    public DateTime CalculateExpectedNextSending(Organization organization);
}