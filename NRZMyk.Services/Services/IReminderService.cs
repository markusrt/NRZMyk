using NRZMyk.Services.Data.Entities;
using System;
using System.Threading.Tasks;

namespace NRZMyk.Services.Services;

public interface IReminderService
{
    public string HumanReadableExpectedNextSending(Organization organization);
    public DateTime? CalculateExpectedNextSending(Organization organization);
    public Task CheckDataAndSendReminders(Organization organization);
}