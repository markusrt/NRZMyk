using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Models
{
    public class ConnectedAccount
    {
        public RemoteAccount Account { get; set; }

        public bool IsGuest { get; set; } = true;
    }
}