using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AwardsService : IAwardsService
    {
        private readonly IAwardsQuery _awardsQuery;
        private readonly IAwardsRepository _awardsRepository;

        public AwardsService(IAwardsQuery awardsQuery, IAwardsRepository awardsRepository)
        {
            _awardsQuery = awardsQuery;
            _awardsRepository = awardsRepository;
        }

        public async Task<AwardsResponseDTO?> GetProducerIntervalsAsync()
        {
            var producerIntervals = await _awardsQuery.GetProducersWithIntervalsAsync();

            if (producerIntervals == null || !producerIntervals.Any())
                return null;

            var minInterval = producerIntervals
                .Where(x => x.Interval == producerIntervals.Min(p => p.Interval))
                .ToList();

            var maxInterval = producerIntervals
                .Where(x => x.Interval == producerIntervals.Max(p => p.Interval))
                .ToList();

            return new AwardsResponseDTO
            {
                Min = minInterval,
                Max = maxInterval
            };
        }

        public async Task LoadDataFromCsvAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var awardsList = new List<Award>();

            foreach (var line in lines.Skip(1)) // Ignora o cabeçalho.
            {
                var columns = line.Split(';');
                if (columns.Length >= 5)
                {
                    awardsList.Add(new Award
                    {
                        Year = int.Parse(columns[0]),
                        Title = columns[1],
                        Studios = columns[2],
                        Producers = columns[3],
                        IsWinner = columns[4].Trim().ToLower() == "yes"
                    });
                }
            }

            await _awardsRepository.AddRangeAsync(awardsList);
        }
    }
}
