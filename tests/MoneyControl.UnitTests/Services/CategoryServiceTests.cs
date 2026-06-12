using FluentAssertions;
using Moq;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;
using MoneyControl.Services;

namespace MoneyControl.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepo;
    private readonly Mock<IExpenseRepository> _expenseRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _categoryRepo = new Mock<ICategoryRepository>();
        _expenseRepo = new Mock<IExpenseRepository>();
        _service = new CategoryService(_categoryRepo.Object, _expenseRepo.Object);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenCategoryHasExpenses()
    {
        _expenseRepo
            .Setup(r => r.ExistsByCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.Invoking(s => s.DeleteAsync(1))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*expenses*");

        _categoryRepo.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepoDelete_WhenNoExpenses()
    {
        _expenseRepo
            .Setup(r => r.ExistsByCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _categoryRepo
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        _categoryRepo.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedResult()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Food", Description = "Groceries" },
            new() { Id = 2, Name = "Transport", Description = null }
        };

        _categoryRepo
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Category>(categories, 2, 1, 20, 1));

        var result = await _service.GetAllAsync();

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);

        var first = result.Items.First();
        first.Id.Should().Be(1);
        first.Name.Should().Be("Food");
        first.Description.Should().Be("Groceries");
    }

    [Fact]
    public async Task AllMethods_PropagateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _categoryRepo
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Category>([], 0, 1, 20, 0));

        _categoryRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await _service.GetAllAsync(page: 1, pageSize: 20, cancellationToken: token);
        await _service.GetByIdAsync(1, cancellationToken: token);

        _categoryRepo.Verify(r => r.GetAllAsync(1, 20, token), Times.Once);
        _categoryRepo.Verify(r => r.GetByIdAsync(1, token), Times.Once);
    }
}
