using System;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Models
{
    
    [Flags]
    public enum Role
    {
        None = 0,
        
        /// <summary>
        /// Not assigned to an organization, hence not authorized to access any data
        /// </summary>
        Guest = 1,
        /// <summary>
        /// Assigned an organization, allowed to access his organizations data
        /// </summary>
        User = 2,
        /// <summary>
        /// Member of owning laboratory allowed to see all data but only to edit his organizations
        /// </summary>
        SuperUser = 4,
        /// <summary>
        /// Administrator allowed to manage users
        /// </summary>
        Admin = 8
    }

    public static class Roles
    {
        public const string RegularUsers = nameof(Role.User) + "," + nameof(Role.Admin) + "," + nameof(Role.SuperUser);
    }
}