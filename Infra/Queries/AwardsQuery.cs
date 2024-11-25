using Application.DTOs;
using Application.Interfaces.Queries;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Queries
{
    public class AwardsQuery : IAwardsQuery
    {
        private readonly ApplicationDbContext _dbContext;

        public AwardsQuery(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProducerIntervalDTO>?> GetProducersWithIntervalsAsync()
        {
            var awards = await _dbContext.Awards
                .AsNoTracking() 
                .Where(a => a.IsWinner)
                .ToListAsync();

            var producerIntervals = awards
                .GroupBy(a => a.Producers)
                .SelectMany(group =>
                {
                    var wins = group.OrderBy(a => a.Year).ToList();
                    var intervals = new List<ProducerIntervalDTO>();

                    for (int i = 1; i < wins.Count; i++)
                    {
                        intervals.Add(new ProducerIntervalDTO
                        {
                            Producer = group.Key,
                            Interval = wins[i].Year - wins[i - 1].Year,
                            PreviousWin = wins[i - 1].Year,
                            FollowingWin = wins[i].Year
                        });
                    }

                    return intervals;
                })
                .ToList();

            return producerIntervals;
        }
    }
}
