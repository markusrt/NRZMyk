using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Validation;

namespace NRZMyk.Services.Services
{
    public class CryoArchiveRequest
    {
        public int Id { get; set; }

        public DateTime? CryoDate { get; set; }

        public string CryoRemark { get; set; }
    }
}