using BlazorApplicationInsights.Interfaces;
using BlazorApplicationInsights.Models;
using Microsoft.JSInterop;

namespace NRZMyk.Components.Playground
{
    internal class NullApplicationInsights : IApplicationInsights
    {
        public CookieMgr GetCookieMgr()
        {
            return null;
        }

        public Task TrackEvent(EventTelemetry @event)
        {
            return Task.CompletedTask;
        }

        public Task TrackPageView(PageViewTelemetry pageView = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackException(ExceptionTelemetry exception)
        {
            return Task.CompletedTask;
        }

        public Task TrackTrace(TraceTelemetry trace)
        {
            return Task.CompletedTask;
        }

        public Task TrackMetric(MetricTelemetry metric)
        {
            return Task.CompletedTask;
        }

        public Task StartTrackPage(string name = null)
        {
            return Task.CompletedTask;
        }

        public Task StopTrackPage(string name = null, string url = null, Dictionary<string, object> customProperties = null,
            Dictionary<string, decimal> measurements = null)
        {
            return Task.CompletedTask;
        }

        public Task StartTrackEvent(string name)
        {
            return Task.CompletedTask;
        }

        public Task StopTrackEvent(string name, Dictionary<string, object> properties = null, Dictionary<string, decimal> measurements = null)
        {
            return Task.CompletedTask;
        }

        public Task AddTelemetryInitializer(TelemetryItem telemetryItem)
        {
            return Task.CompletedTask;
        }

        public Task TrackPageViewPerformance(PageViewPerformanceTelemetry pageViewPerformance)
        {
            return Task.CompletedTask;
        }

        public Task TrackDependencyData(DependencyTelemetry dependency)
        {
            return Task.CompletedTask;
        }

        public Task<TelemetryContext> Context()
        {
            return Task.FromResult(new TelemetryContext());
        }

        public Task Flush()
        {
            return Task.CompletedTask;
        }

        public Task ClearAuthenticatedUserContext()
        {
            return Task.CompletedTask;
        }

        public Task SetAuthenticatedUserContext(string authenticatedUserId, string accountId = null, bool? storeInCookie = null)
        {
            return Task.CompletedTask;
        }

        public Task UpdateCfg(Config newConfig, bool mergeExisting = true)
        {
            return Task.CompletedTask;
        }

        public void InitJSRuntime(IJSRuntime jSRuntime)
        {
        }
    }
}