using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Services;

public class SentinelEntryServiceTests
{
    [Test]
    public void Ctor_DoesNotThrow()
    {
        Action createSut = () => CreateSut(out _);

        createSut.Should().NotThrow();
    }
    
    [Test]
    public async Task WhenCreate_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        var request = new SentinelEntryRequest();
        
        await sut.Create(request).ConfigureAwait(true);

        await httpClient.Received(1).Post<SentinelEntryRequest, SentinelEntry>(
            "api/sentinel-entries", request, default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenUpdate_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        var request = new SentinelEntryRequest();
        
        await sut.Update(request).ConfigureAwait(true);

        await httpClient.Received(1).Put<SentinelEntryRequest, SentinelEntry>(
            "api/sentinel-entries", request, default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenCryoArchive_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        var request = new CryoArchiveRequest();
        
       await sut.CryoArchive(request).ConfigureAwait(true);

        await httpClient.Received(1).Put<CryoArchiveRequest, SentinelEntry>(
            "api/sentinel-entries/cryo-archive", request, default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenExport_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.Export().ConfigureAwait(true);

        await httpClient.Received(1).GetBytesAsBase64(
            "api/sentinel-entries/export", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenOther_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.Other("foo").ConfigureAwait(true);

        await httpClient.Received(1).Get<List<string>>(
            "api/sentinel-entries/other/foo", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenDelete_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.Delete(123).ConfigureAwait(true);

        await httpClient.Received(1).Delete<SentinelEntry>(
            "api/sentinel-entries/123", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenListPaged_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        var response = new PagedSentinelEntryResult()
        {
            PageCount = 677, SentinelEntries = new List<SentinelEntry> { new SentinelEntry() }
        };
        httpClient.Get<PagedSentinelEntryResult>(
            "api/sentinel-entries?PageSize=1", default, Arg.Any<string>()).Returns(Task.FromResult(response));
        
        var result = await sut.ListPaged(1).ConfigureAwait(true);

        await httpClient.Received(1).Get<PagedSentinelEntryResult>(
            "api/sentinel-entries?PageSize=1", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
        result.Should().HaveCount(1);
    }

    [Test]
    public async Task WhenListByOrganization_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.ListByOrganization(42).ConfigureAwait(true);

        await httpClient.Received(1).Get<List<SentinelEntry>>(
            "api/sentinel-entries/organization/42", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenGetById_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.GetById(833).ConfigureAwait(true);

        await httpClient.Received(1).Get<SentinelEntryResponse>(
            "api/sentinel-entries/833", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    private static SentinelEntryServiceImpl CreateSut(out IHttpClient httpClient)
    {
        httpClient = Substitute.For<IHttpClient>();
        return new SentinelEntryServiceImpl(httpClient);
    }
}