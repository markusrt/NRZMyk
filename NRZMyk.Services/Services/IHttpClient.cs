using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NRZMyk.Services.Services;

public interface IHttpClient
{
    Task<TResponse> Post<TRequest, TResponse>(
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="");

    Task<TResponse> Put<TRequest, TResponse>(
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="");

    Task<TResponse> Get<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="");

    Task<string> GetBytesAsBase64(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="");

    Task Delete<TDeleteTarget>(
        string requestUri,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callingMethod="");
}