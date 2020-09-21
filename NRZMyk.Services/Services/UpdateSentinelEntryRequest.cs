using System.ComponentModel.DataAnnotations;

namespace NRZMyk.Services.Services
{
    public class UpdateSentinelEntryRequest : CreateSentinelEntryRequest
    {
        [Required(ErrorMessage = "Das Feld Id ist erforderlich")]
        public int Id { get; set; }
    }
}