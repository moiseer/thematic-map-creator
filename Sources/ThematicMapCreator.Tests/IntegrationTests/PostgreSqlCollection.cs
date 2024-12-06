using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

[CollectionDefinition("PostgreSqlTests")]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>;
