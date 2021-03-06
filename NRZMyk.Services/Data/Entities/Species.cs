﻿using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum Species
    {
        [Description("-")] None = 0,
        [Description("Candida albicans")] CandidaAlbicans = 100,
        [Description("Candida dubliniensis")] CandidaDubliniensis = 200,
        [Description("Candida glabrata")] CandidaGlabrata = 300,
        [Description("Candida krusei")] CandidaKrusei = 400,
        [Description("Candida parapsilosis")] CandidaParapsilosis = 500,
        [Description("Candida tropicalis")] CandidaTropicalis = 600,
        [Description("Candida guilliermondii")] CandidaGuilliermondii = 700,
        [Description("Andere")] Other = 10000,
    }
}