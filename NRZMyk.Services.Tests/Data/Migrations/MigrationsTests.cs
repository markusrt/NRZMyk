using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Migrations;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Data.Migrations;

public class MigrationsTests
{
    [TestCaseSource(nameof(Migrations))]
    public void WhenUpOperationsAreBuilt_DoesNotCrash(Type migration)
    {
        var sut = Activator.CreateInstance(migration) as Migration;

        sut.Should().NotBeNull();
        sut!.UpOperations.Should().NotBeEmpty();
    }

    [TestCaseSource(nameof(Migrations))]
    public void WhenDownOperationsAreBuilt_DoesNotCrash(Type migration)
    {
        var sut = Activator.CreateInstance(migration) as Migration;

        sut.Should().NotBeNull();
        sut!.DownOperations.Should().NotBeEmpty();
    }

    [TestCaseSource(nameof(Migrations))]
    public void WhenTargetModelIsBuilt_DoesNotCrash(Type migration)
    {
        var sut = Activator.CreateInstance(migration) as Migration;

        sut.Should().NotBeNull();
        sut!.TargetModel.ToDebugString().Should().NotBeEmpty();
    }

    private static IEnumerable<Type> Migrations()
      => typeof(ApplicationDbContext).Assembly.GetTypes().Where(t => typeof(Migration).IsAssignableFrom(t));
}