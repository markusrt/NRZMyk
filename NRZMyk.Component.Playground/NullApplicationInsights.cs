using BlazorApplicationInsights;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NRZMyk.Components.Playground
{
    internal class NullApplicationInsights : IApplicationInsights
    {
        public Task ClearAuthenticatedUserContext()
        {
            return Task.CompletedTask;
        }

        public Task Flush(bool? async = true)
        {
            return Task.CompletedTask;
        }

        public Task SetAuthenticatedUserContext(string authenticatedUserId, string accountId = null, bool storeInCookie = false)
        {
            return Task.CompletedTask;
        }

        public Task StartTrackPage(string name = null)
        {
            return Task.CompletedTask;
        }

        public Task StopTrackPage(string name = null, string url = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackDependencyData(string id, double responseCode, string absoluteUrl = null, bool? success = null, string commandName = null, double? duration = null, string method = null, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackEvent(string name, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackException(Error error, SeverityLevel? severityLevel = null, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackMetric(string name, double average, double? sampleCount = null, double? min = null, double? max = null, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackPageView(string name = null, string uri = null, string refUri = null, string pageType = null, bool? isLoggedIn = null, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }

        public Task TrackTrace(string message, SeverityLevel? severityLevel = null, Dictionary<string, object> properties = null)
        {
            return Task.CompletedTask;
        }
    }
}