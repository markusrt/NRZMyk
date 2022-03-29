using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NRZMyk.Services.Services;

public class LoggingJsonHttpClient : IHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LoggingJsonHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public LoggingJsonHttpClient(HttpClient httpClient, ILogger<LoggingJsonHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };
    }

    public Task<TResponse> Get<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="")
    {
        async Task<HttpResponseMessage> DoGet() =>
            await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return TryHttpClientCall<TResponse>(
            DoGet, HttpMethod.Get, requestUri, callingMethod, cancellationToken, typeof(TResponse));
    }

    public async Task<string> GetBytesAsBase64(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="")
    {
        try
        {
            var data = await _httpClient.GetByteArrayAsync(requestUri, cancellationToken).ConfigureAwait(false);
            return Convert.ToBase64String(data);
        }
        catch (Exception exception)
        {
            LogError(HttpMethod.Get, requestUri, callingMethod, "Base64", exception);
            return string.Empty;
        }
    }

    public Task<TResponse> Post<TRequest, TResponse>(
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="")
    {
        async Task<HttpResponseMessage> DoPost() =>
            await _httpClient.PostAsJsonAsync(requestUri, value, _jsonOptions, cancellationToken).ConfigureAwait(false);

        return TryHttpClientCall<TResponse>(
            DoPost, HttpMethod.Post, requestUri, callingMethod, cancellationToken, typeof(TRequest));
    }

    public Task<TResponse> Put<TRequest, TResponse>(
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="")
    {
        async Task<HttpResponseMessage> DoPut() =>
            await _httpClient.PutAsJsonAsync(requestUri, value, _jsonOptions, cancellationToken).ConfigureAwait(false);

        return TryHttpClientCall<TResponse>(
            DoPut, HttpMethod.Put, requestUri, callingMethod, cancellationToken, typeof(TRequest));
    }
    public async Task Delete<TDeleteTarget>(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="")
    {
        HttpResponseMessage response = null;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            LogError(HttpMethod.Delete, requestUri, callingMethod, typeof(TDeleteTarget).Name, exception, response?.StatusCode);
            throw;
        }

    }

    private async Task<TResponse> TryHttpClientCall<TResponse>(Func<Task<HttpResponseMessage>> httpClientCall,
        HttpMethod method, string requestUri, string callingMethod, CancellationToken cancellationToken, Type requestType = null)
    {
        HttpResponseMessage response = null;
        try
        {
            response = await httpClientCall();
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new Exception($"Remote call failed with status {response.StatusCode}, content: {content}");
            }
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            LogError(method, requestUri, callingMethod, requestType?.Name, exception, response?.StatusCode);
            throw;
        }
    }

    private void LogError(HttpMethod method, string requestUri, string callingMethod, string target,
        Exception exception, HttpStatusCode? statusCode=null)
    {
        var uri = new Uri(_httpClient.BaseAddress, requestUri);
        var status = statusCode?.ToString() ?? "?";
        _logger.LogError(exception, "{method} {request} on {uri} failed with status {status} during {callingMethod}",
            method, target, uri, status, callingMethod);
    }
}