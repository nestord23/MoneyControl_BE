using System.Globalization;
using MoneyControl.DTOs;
using MoneyControl.Helpers;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class ExpenseService(
    IExpenseRepository expenseRepository,
    ICategoryRepository categoryRepository) : IExpenseService
{
    public async Task<PagedResult<ExpenseResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await expenseRepository.GetAllAsync(page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await expenseRepository.GetByIdAsync(id, cancellationToken);
        return expense is null ? null : MapToResponse(expense);
    }

    public async Task<PagedResult<ExpenseResponse>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await expenseRepository.GetByCategoryAsync(categoryId, page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {request.CategoryId} not found.");

        var expense = new Expense
        {
            Amount = request.Amount,
            Date = ToUtc(request.Date),
            Description = request.Description,
            Type = request.Type,
            CategoryId = request.CategoryId
        };

        var created = await expenseRepository.CreateAsync(expense, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<ExpenseResponse?> UpdateAsync(int id, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {request.CategoryId} not found.");

        var expense = new Expense
        {
            Id = id,
            Amount = request.Amount,
            Date = ToUtc(request.Date),
            Description = request.Description,
            Type = request.Type,
            CategoryId = request.CategoryId
        };

        var updated = await expenseRepository.UpdateAsync(expense, cancellationToken);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await expenseRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<decimal> GetTotalByDayAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<decimal> GetTotalByWeekAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var weekStart = GetWeekStart(date);
        return await expenseRepository.GetTotalByDateRangeAsync(weekStart, weekStart.AddDays(7), cancellationToken);
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<decimal> GetTotalByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<decimal> GetTotalFixedAsync(CancellationToken cancellationToken = default)
    {
        return await expenseRepository.GetTotalByTypeAsync(ExpenseType.Fixed, cancellationToken);
    }

    public async Task<decimal> GetTotalVariableAsync(CancellationToken cancellationToken = default)
    {
        return await expenseRepository.GetTotalByTypeAsync(ExpenseType.Variable, cancellationToken);
    }

    public async Task<ExpenseSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var dayStart = now.Date;
        var weekStart = GetWeekStart(now);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var summary = await expenseRepository.GetAggregatedSummaryAsync(dayStart, weekStart, monthStart, yearStart, cancellationToken);

        return new ExpenseSummaryResponse(
            summary.TotalByDay,
            summary.TotalByWeek,
            summary.TotalByMonth,
            summary.TotalByYear,
            summary.TotalFixed,
            summary.TotalVariable
        );
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        var diff = (7 + (int)date.DayOfWeek - (int)firstDayOfWeek) % 7;
        return date.Date.AddDays(-diff);
    }

    private static DateTime ToUtc(DateTime date) => DateTimeHelper.ToUtc(date);

    private static ExpenseResponse MapToResponse(Expense expense)
    {
        return new ExpenseResponse(
            expense.Id,
            expense.Amount,
            expense.Date,
            expense.Description,
            expense.Type,
            expense.CategoryId,
            expense.Category?.Name ?? string.Empty
        );
    }
}
