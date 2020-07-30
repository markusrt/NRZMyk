using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum Material
    {
        [Description("Isolat")] Isolate = 0,
        [Description("Isolierte DNA")] IsolatedDna = 1,
        [Description("Nativmaterial")] NativeMaterial = 2
    }
}