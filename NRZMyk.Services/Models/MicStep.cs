using System;
using System.ComponentModel.DataAnnotations;

namespace NRZMyk.Services.Models
{
    public class MicStep
    {
        public string Title { get; set; }
        
        [Range(0, 1,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public float Value { get; set; }
    }
}