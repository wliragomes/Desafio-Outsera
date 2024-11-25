using Application.DTOs;

namespace Application.Interfaces.Queries
{
    public interface IAwardsQuery
    {
        Task<List<ProducerIntervalDTO>?> GetProducersWithIntervalsAsync();
    }
}
