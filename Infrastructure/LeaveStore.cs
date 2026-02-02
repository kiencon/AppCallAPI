using Contracts;

namespace Infrastructure;

public sealed class LeaveRequest
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = "";
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public Lang Language { get; init; }
}

public interface ILeaveRepository
{
    Task<Guid> CreateAsync(LeaveRequest req, CancellationToken ct);
    Task<LeaveRequest?> GetAsync(Guid id, CancellationToken ct);
}

public sealed class InMemoryLeaveRepository : ILeaveRepository
{
    private readonly Dictionary<Guid, LeaveRequest> _db = [];

    public Task<Guid> CreateAsync(LeaveRequest req, CancellationToken ct)
    {
        _db[req.Id] = req;
        return Task.FromResult(req.Id);
    }

    public Task<LeaveRequest?> GetAsync(Guid id, CancellationToken ct)
    {
        _db.TryGetValue(id, out var req);
        return Task.FromResult(req);
    }
}
