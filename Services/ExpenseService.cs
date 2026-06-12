using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class ExpenseService(
    IExpenseRepository expenseRepository,
    ICategoryRepository categoryRepository) : IExpenseService
{
    public async Task<PagedResponse<ExpenseResponse>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var result = await expenseRepository.GetAllAsync(page, pageSize);
        return new PagedResponse<ExpenseResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<ExpenseResponse?> GetByIdAsync(int id)
    {
        var expense = await expenseRepository.GetByIdAsync(id);
        return expense is null ? null : MapToResponse(expense);
    }

    public async Task<PagedResponse<ExpenseResponse>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20)
    {
        var result = await expenseRepository.GetByCategoryAsync(categoryId, page, pageSize);
        return new PagedResponse<ExpenseResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {request.CategoryId} not found.");

        var expense = new Expense
        {
            Amount = request.Amount,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Description = request.Description,
            Type = request.Type,
            CategoryId = request.CategoryId
        };

        var created = await expenseRepository.CreateAsync(expense);
        return MapToResponse(created);
    }

    public async Task<ExpenseResponse?> UpdateAsync(int id, UpdateExpenseRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {request.CategoryId} not found.");

        var expense = new Expense
        {
            Id = id,
            Amount = request.Amount,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Description = request.Description,
            Type = request.Type,
            CategoryId = request.CategoryId
        };

        var updated = await expenseRepository.UpdateAsync(expense);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await expenseRepository.DeleteAsync(id);
    }

    public async Task<decimal> GetTotalByDayAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<decimal> GetTotalByWeekAsync(DateTime date)
    {
        var daysSinceMonday = (int)date.DayOfWeek - 1;
        if (daysSinceMonday < 0) daysSinceMonday += 7;
        var monday = date.Date.AddDays(-daysSinceMonday);
        var nextMonday = monday.AddDays(7);
        return await expenseRepository.GetTotalByDateRangeAsync(monday, nextMonday);
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<decimal> GetTotalByYearAsync(int year)
    {
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);
        return await expenseRepository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<decimal> GetTotalFixedAsync()
    {
        return await expenseRepository.GetTotalByTypeAsync(ExpenseType.Fixed);
    }

    public async Task<decimal> GetTotalVariableAsync()
    {
        return await expenseRepository.GetTotalByTypeAsync(ExpenseType.Variable);
    }

    public async Task<ExpenseSummaryResponse> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var dayStart = now.Date;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek + 1);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalByDay = await expenseRepository.GetTotalByDateRangeAsync(dayStart, dayStart.AddDays(1));
        var totalByWeek = await expenseRepository.GetTotalByDateRangeAsync(weekStart, weekStart.AddDays(7));
        var totalByMonth = await expenseRepository.GetTotalByDateRangeAsync(monthStart, monthStart.AddMonths(1));
        var totalByYear = await expenseRepository.GetTotalByDateRangeAsync(yearStart, yearStart.AddYears(1));
        var totalFixed = await expenseRepository.GetTotalByTypeAsync(ExpenseType.Fixed);
        var totalVariable = await expenseRepository.GetTotalByTypeAsync(ExpenseType.Variable);

        return new ExpenseSummaryResponse(totalByDay, totalByWeek, totalByMonth, totalByYear, totalFixed, totalVariable);
    }

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
