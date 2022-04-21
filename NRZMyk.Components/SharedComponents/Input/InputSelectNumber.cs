using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Forms;

namespace NRZMyk.Components.SharedComponents.Input
{
    public class InputSelectNumber<T> : InputSelect<T>
    {
        /// <inheritdoc />
        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out T result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            validationErrorMessage = null;
            if ((typeof(T) == typeof(int) || typeof(T) == typeof(int?)) && int.TryParse(value, out var resultInt))
            {
                result = (T)(object)resultInt;
                return true;
            }
            if ((typeof(T) == typeof(float) || typeof(T) == typeof(float?)) && float.TryParse(value, out var resultFloat))
            {
                result = (T)(object)resultFloat;
                return true;
            }
            if ((typeof(T) == typeof(int?) || typeof(T) == typeof(float?)) && string.IsNullOrEmpty(value))
            {
                result = default;
                return result != null;
            }

            result = default;
            validationErrorMessage = $"The field '{FieldIdentifier.FieldName}' does not contain a valid number.";
            return false;
        }
    }

}