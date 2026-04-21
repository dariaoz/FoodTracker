using FoodTracker.Application.Services.Interfaces;
using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Application.Services;

public class FoodLogService : IFoodLogService
{
    private readonly INotionUnitOfWork _uow;
    private readonly IValidator<FoodLog> _validator;

    public FoodLogService(INotionUnitOfWork uow, IValidator<FoodLog> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default) => _uow.FoodLogs.GetAllAsync(ct);
    public Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default) => _uow.FoodLogs.GetByDateAsync(date, ct);
    public Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default) => _uow.FoodLogs.GetByIdAsync(id, ct);

    public Task<FoodLog> CreateAsync(FoodLog log, CancellationToken ct = default)
    {
        _validator.Validate(log);
        return _uow.FoodLogs.CreateAsync(log, ct);
    }

    public Task<FoodLog> UpdateAsync(FoodLog log, CancellationToken ct = default)
    {
        _validator.Validate(log);
        return _uow.FoodLogs.UpdateAsync(log, ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _uow.FoodLogs.DeleteAsync(id, ct);
}
