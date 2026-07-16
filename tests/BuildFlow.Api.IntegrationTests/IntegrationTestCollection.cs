using Xunit;

namespace BuildFlow.Api.IntegrationTests;

// تجميعة تجمع كل اختبارات التكامل لتتشارك مصنعاً واحداً
[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection
    : ICollectionFixture<IntegrationTestFactory>
{
}