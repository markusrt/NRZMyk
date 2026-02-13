using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Server.Controllers.SentinelEntries;

public static class Utils
{
    public static async Task<bool> ResolvePredecessor(SentinelEntryRequest request, SentinelEntry newEntry,
        IAsyncRepository<SentinelEntry>  repository, string organizationId, ModelStateDictionary modelState)
    {
        if (string.IsNullOrEmpty(request.PredecessorLaboratoryNumber)) return false;
        
        var predecessor = await repository.FirstOrDefaultAsync(
            new SentinelEntryByLaboratoryNumberSpecification(request.PredecessorLaboratoryNumber, organizationId)).ConfigureAwait(false);

        if (predecessor == null)
        {
            modelState.AddModelError($"{nameof(SentinelEntryRequest.PredecessorLaboratoryNumber)}", "Die Labornummer wurde nicht gefunden");
            return true;
        }

        newEntry.PredecessorEntryId = predecessor.Id;
        return false;
    }
}