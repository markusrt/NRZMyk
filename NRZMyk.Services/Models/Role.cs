using System;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Models
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

    public static class Roles
    {
        public const string RegularUsers = nameof(Role.User) + "," + nameof(Role.Admin) + "," + nameof(Role.Admin);
    }
}