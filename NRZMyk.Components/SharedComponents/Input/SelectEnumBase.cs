using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace NRZMyk.Components.SharedComponents.Input
{
    public class SelectEnumBase<T> : InputSelect<T>
    {
        [Parameter] public string Label { get; set; } = default!;
        [Parameter] public Expression<Func<T>> ValidationFor { get; set; } = default!;
        [Parameter] public bool ShowDefaultOption { get; set; } = true;
        [Parameter] public bool AddFormRow { get; set; } = true;
        [Parameter] public string SelectClass { get; set; } = "col-sm-6";
    }
}