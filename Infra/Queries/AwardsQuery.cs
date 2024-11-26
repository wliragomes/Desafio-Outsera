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

            var producerAwards = new Dictionary<string, List<int>>();

            foreach (var award in awards)
            {
                var producers = award.Producers?
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                if (producers == null) continue;

                foreach (var producer in producers)
                {
                    if (!producerAwards.ContainsKey(producer))
                        producerAwards[producer] = new List<int>();

                    producerAwards[producer].Add(award.Year);
                }
            }

            // Calcula os intervalos entre prêmios consecutivos
            var producerIntervals = producerAwards
                .SelectMany(pair =>
                {
                    var producer = pair.Key;
                    var years = pair.Value.OrderBy(y => y).ToList();

                    var intervals = new List<ProducerIntervalDTO>();
                    for (int i = 1; i < years.Count; i++)
                    {
                        intervals.Add(new ProducerIntervalDTO
                        {
                            Producer = producer,
                            Interval = years[i] - years[i - 1],
                            PreviousWin = years[i - 1],
                            FollowingWin = years[i]
                        });
                    }

                    return intervals;
                })
                .ToList();

            return producerIntervals;
        }
    }
}