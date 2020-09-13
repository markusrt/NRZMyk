using Microsoft.AspNetCore.Components.Forms;

namespace NRZMyk.Components.SharedComponents.Input
{
    public class InputSelectNumber<T> : InputSelect<T>
    {
        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            if (typeof(T) == typeof(int) && int.TryParse(value, out var resultInt))
            {
                result = (T)(object)resultInt;
                validationErrorMessage = null;
                return true;
            }
            if (typeof(T) == typeof(float) && float.TryParse(value, out var resultFloat))
            {
                result = (T)(object)resultFloat;
                validationErrorMessage = null;
                return true;
            }

            result = default;
            validationErrorMessage = $"The field '{FieldIdentifier.FieldName}' does not contain a valid number.";
            return false;
        }
    }

}