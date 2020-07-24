using System;

namespace NRZMyk.Server.Model
{
    [Flags]
    public enum Role
    {
        None = 0,
        Guest = 1,
        User = 2,
        Admin = 4,
        SuperUser = 8
    }
}