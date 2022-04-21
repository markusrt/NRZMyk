using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Models
{
    public class ConnectedAccount
    {
        public RemoteAccount Account { get; init; }

        public bool IsGuest { get; init; } = true;
    }
}