using Microsoft.AspNetCore.Components;

namespace NRZMyk.Components.Pages
{
    public class CounterBase : ComponentBase
    {
        public int CurrentCount { get; set; }

        public void IncrementCount()
        {
            CurrentCount++;
        }
    }
}
