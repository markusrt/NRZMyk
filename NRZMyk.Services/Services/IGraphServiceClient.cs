using Microsoft.Graph;

namespace NRZMyk.Services.Services;

public interface IGraphServiceClient
{
    IGraphServiceUsersCollectionRequestBuilder Users { get; }
}