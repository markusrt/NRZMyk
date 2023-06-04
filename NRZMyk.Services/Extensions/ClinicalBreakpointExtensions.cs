using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Extensions;

public static class ClinicalBreakpointExtensions
{
    public static bool IsResistantAccordingToEucastDefinition(this float? mic, ClinicalBreakpoint breakpoint)
    {
        return mic > breakpoint.MicBreakpointResistent && breakpoint.Standard == BrothMicrodilutionStandard.Eucast;
    }

    public static bool IsResistantAccordingToClsiDefinition(this float? mic, ClinicalBreakpoint breakpoint)
    {
        return mic >= breakpoint.MicBreakpointResistent && breakpoint.Standard == BrothMicrodilutionStandard.Clsi;
    }

    public static bool IsSusceptibleAccordingToBothDefinitions(this float? mic, ClinicalBreakpoint breakpoint)
    {
        return mic <= breakpoint.MicBreakpointSusceptible;
    }
}