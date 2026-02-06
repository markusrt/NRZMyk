using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    /// <summary>
    /// Provides clinical breakpoints from the embedded JSON resource.
    /// </summary>
    public class ClinicalBreakpointProvider : IClinicalBreakpointProvider
    {
        private readonly List<ClinicalBreakpoint> _breakpoints;

        public ClinicalBreakpointProvider()
        {
            _breakpoints = LoadBreakpointsFromEmbeddedResource();
        }

        public List<ClinicalBreakpoint> GetBreakpoints()
        {
            return _breakpoints;
        }

        private static List<ClinicalBreakpoint> LoadBreakpointsFromEmbeddedResource()
        {
            using var stream = typeof(ClinicalBreakpointProvider).Assembly
                .GetManifestResourceStream("NRZMyk.Services.Data.ClinicalBreakpoints.json");
            
            if (stream == null)
            {
                throw new FileNotFoundException("Embedded resource 'NRZMyk.Services.Data.ClinicalBreakpoints.json' not found.");
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Converters = { new JsonStringEnumConverter() }
            };
            
            return JsonSerializer.Deserialize<List<ClinicalBreakpoint>>(json, options) ?? new List<ClinicalBreakpoint>();
        }
    }
}
