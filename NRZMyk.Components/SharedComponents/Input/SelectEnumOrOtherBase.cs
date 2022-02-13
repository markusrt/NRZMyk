using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.SharedComponents.Input
{
    public class SelectEnumOrOtherBase<T> : InputSelect<T>
    {
        [Parameter] public string Label { get; set; } = default!;
        [Parameter] public string OtherLabel { get; set; } = default!;
        [Parameter] public string Key { get; set; } = default!;
        [Parameter] public T Other { get; set; } = default!;
        [Parameter] public string OtherValue { get; set; } = default!;
        [Parameter] public EventCallback<string> OtherValueChanged { get; set; }  
        [Parameter] public Expression<Func<T>> ValidationFor { get; set; } = default!;
        [Parameter] public Expression<Func<string>> ValidationForOther { get; set; } = default!;
        [Parameter] public bool ShowDefaultOption { get; set; } = true;
        [Parameter] public bool AddFormRow { get; set; } = true;
        [Parameter] public string SelectClass { get; set; } = "col-sm-6";
        
        protected List<string> OtherValues { get; private set; } = new List<string>();

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        protected Guid DataListId { get; } = Guid.NewGuid();

        protected Task OnOtherValueChanged(ChangeEventArgs e)  
        {  
            OtherValue = e.Value!.ToString()!;  
            return OtherValueChanged.InvokeAsync(OtherValue);  
        }  

        protected bool IsOtherVisible()
        {
            var isOtherVisible = CurrentValueAsString == Enum.GetName(typeof(T), Other!);
            if (!isOtherVisible && OtherValue != default!)
            {
                OtherValue = default!;
                OtherValueChanged.InvokeAsync(OtherValue);
            }
            return isOtherVisible;
        }

        protected override async Task OnInitializedAsync()
        {
            OtherValues = await SentinelEntryService.Other(Key).ConfigureAwait(true);
            await base.OnInitializedAsync().ConfigureAwait(true);
        }
    }
}