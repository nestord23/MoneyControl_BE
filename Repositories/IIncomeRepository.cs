using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface IIncomeRepository
{
    Task<IEnumerable<Income>> GetAllAsync();
    Task<Income?> GetByIdAsync(int id);
    Task<Income> CreateAsync(Income income);
    Task<Income?> UpdateAsync(Income income);
    Task<bool> DeleteAsync(int id);
}
