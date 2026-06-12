using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.IntegrationTests;

[Collection("Database")]
public class ExpenseIntegrationTests
{
    private readonly DbFixture _fixture;

    public ExpenseIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateExpense_Then_GetById_ReturnsSameData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var categoryRepo = new CategoryRepository(context);
        var expenseRepo = new ExpenseRepository(context);

        var category = new Category { Name = "Test Cat", Description = "Test" };
        await categoryRepo.CreateAsync(category);

        var created = await expenseRepo.CreateAsync(new Expense
        {
            Amount = 99.99m,
            Date = new DateTime(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc),
            Description = "Integration test expense",
            Type = ExpenseType.Variable,
            CategoryId = category.Id
        });

        created.Id.Should().BeGreaterThan(0);

        var fetched = await expenseRepo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Amount.Should().Be(99.99m);
        fetched.Description.Should().Be("Integration test expense");
        fetched.Type.Should().Be(ExpenseType.Variable);
        fetched.CategoryId.Should().Be(category.Id);
        fetched.Category.Name.Should().Be("Test Cat");
    }

    [Fact]
    public async Task UpdateExpense_ModifiesData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var categoryRepo = new CategoryRepository(context);
        var expenseRepo = new ExpenseRepository(context);

        var category = new Category { Name = "Test" };
        await categoryRepo.CreateAsync(category);

        var created = await expenseRepo.CreateAsync(new Expense
        {
            Amount = 50m,
            Date = DateTime.UtcNow,
            Description = "Original",
            Type = ExpenseType.Fixed,
            CategoryId = category.Id
        });

        var updated = await expenseRepo.UpdateAsync(new Expense
        {
            Id = created.Id,
            Amount = 75m,
            Date = created.Date,
            Description = "Updated",
            Type = ExpenseType.Variable,
            CategoryId = category.Id
        });

        updated.Should().NotBeNull();
        updated!.Amount.Should().Be(75m);
        updated.Description.Should().Be("Updated");
        updated.Type.Should().Be(ExpenseType.Variable);
    }

    [Fact]
    public async Task DeleteExpense_RemovesAndReturnsTrue()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var categoryRepo = new CategoryRepository(context);
        var expenseRepo = new ExpenseRepository(context);

        var category = new Category { Name = "Test" };
        await categoryRepo.CreateAsync(category);

        var created = await expenseRepo.CreateAsync(new Expense
        {
            Amount = 10m,
            Date = DateTime.UtcNow,
            CategoryId = category.Id
        });

        var deleted = await expenseRepo.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var fetched = await expenseRepo.GetByIdAsync(created.Id);
        fetched.Should().BeNull();
    }

    [Fact]
    public async Task DeleteExpense_ReturnsFalse_WhenNotFound()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var expenseRepo = new ExpenseRepository(context);

        var result = await expenseRepo.DeleteAsync(999);
        result.Should().BeFalse();
    }
}
