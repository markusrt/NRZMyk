using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.SharedComponents.Input
{
    public class SelectEnumOrOtherBase<T> : InputSelect<T>
    {
        [Parameter] public string Label { get; set; }
        [Parameter] public string OtherLabel { get; set; }
        [Parameter] public string Key { get; set; }
        [Parameter] public T Other { get; set; }
        [Parameter] public string OtherValue { get; set; }  
        [Parameter] public EventCallback<string> OtherValueChanged { get; set; }  
        [Parameter] public Expression<Func<T>> ValidationFor { get; set; }
        [Parameter] public Expression<Func<string>> ValidationForOther { get; set; }
        [Parameter] public bool ShowDefaultOption { get; set; } = true;
        [Parameter] public bool AddFormRow { get; set; } = true;
        [Parameter] public string SelectClass { get; set; } = "col-sm-6";
        
        protected List<string> OtherValues { get; private set; } = new List<string>();

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }

        protected Guid DataListId { get; } = Guid.NewGuid();

        protected Task OnOtherValueChanged(ChangeEventArgs e)  
        {  
            OtherValue = e.Value.ToString();  
            return OtherValueChanged.InvokeAsync(OtherValue);  
        }  

        protected bool IsOtherVisible()
        {
            var isOtherVisible = CurrentValueAsString == Enum.GetName(typeof(T), Other);
            if (!isOtherVisible && OtherValue != null)
            {
                OtherValue = null;
                OtherValueChanged.InvokeAsync(OtherValue);
            }
            return isOtherVisible;
        }

        protected override async Task OnInitializedAsync()
        {
            OtherValues = await SentinelEntryService.Other(Key);
            await base.OnInitializedAsync();
        }
    }
}