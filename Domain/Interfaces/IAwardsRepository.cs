using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAwardsRepository
    {
        Task AddRangeAsync(IEnumerable<Award> awards);
    }
}
