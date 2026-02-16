using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    /// <summary>
    /// Provides clinical breakpoints from the embedded JSON resource.
    /// </summary>
    public interface IClinicalBreakpointProvider
    {
        /// <summary>
        /// Gets all clinical breakpoints from the embedded resource.
        /// </summary>
        List<ClinicalBreakpoint> GetBreakpoints();
    }
}
