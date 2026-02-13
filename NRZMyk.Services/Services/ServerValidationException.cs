using System;
using System.Collections.Generic;

namespace NRZMyk.Services.Services;

/// <summary>
/// Exception thrown when the server returns validation errors.
/// Contains a dictionary of field names to error messages for display in the UI.
/// </summary>
public class ServerValidationException : Exception
{
    public Dictionary<string, string[]> ValidationErrors { get; }

    public ServerValidationException(Dictionary<string, string[]> validationErrors)
        : base(FormatMessage(validationErrors))
    {
        ValidationErrors = validationErrors;
    }

    private static string FormatMessage(Dictionary<string, string[]> validationErrors)
    {
        var messages = new List<string>();
        foreach (var kvp in validationErrors)
        {
            foreach (var error in kvp.Value)
            {
                messages.Add(error);
            }
        }
        return string.Join("; ", messages);
    }
}
