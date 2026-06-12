using FluentAssertions;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.IntegrationTests;

[Collection("Database")]
public class CategoryIntegrationTests
{
    private readonly DbFixture _fixture;

    public CategoryIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateCategory_Then_GetById_ReturnsSameData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new CategoryRepository(context);

        var created = await repo.CreateAsync(new Category
        {
            Name = "Transport",
            Description = "Gas, bus, taxi"
        });

        created.Id.Should().BeGreaterThan(0);

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Transport");
        fetched.Description.Should().Be("Gas, bus, taxi");
    }

    [Fact]
    public async Task UpdateCategory_ModifiesData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new CategoryRepository(context);

        var created = await repo.CreateAsync(new Category
        {
            Name = "Old Name",
            Description = "Old desc"
        });

        var updated = await repo.UpdateAsync(new Category
        {
            Id = created.Id,
            Name = "New Name",
            Description = "New desc"
        });

        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.Description.Should().Be("New desc");
    }

    [Fact]
    public async Task DeleteCategory_RemovesAndReturnsTrue()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new CategoryRepository(context);

        var created = await repo.CreateAsync(new Category
        {
            Name = "Temp"
        });

        var deleted = await repo.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().BeNull();
    }
}
