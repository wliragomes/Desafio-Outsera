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
            {
                return new AwardsResponseDTO
                {
                    Min = new List<ProducerIntervalDTO>(),
                    Max = new List<ProducerIntervalDTO>()
                };
            }

            var minIntervalValue = producerIntervals.Min(p => p.Interval);
            var maxIntervalValue = producerIntervals.Max(p => p.Interval);

            var minInterval = producerIntervals
                .Where(x => x.Interval == minIntervalValue)
                .ToList();

            var maxInterval = producerIntervals
                .Where(x => x.Interval == maxIntervalValue)
                .ToList();

            return new AwardsResponseDTO
            {
                Min = minInterval,
                Max = maxInterval
            };
        }

        public async Task LoadDataFromCsvAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException($"O arquivo {filePath} não foi encontrado.");

            var awardsList = new List<Award>();

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);

                foreach (var line in lines.Skip(1)) // Ignora o cabeçalho.
                {
                    var columns = line.Split(';');
                    if (columns.Length >= 5 &&
                        int.TryParse(columns[0], out int year))
                    {
                        awardsList.Add(new Award
                        {
                            Year = year,
                            Title = columns[1],
                            Studios = columns[2],
                            Producers = columns[3],
                            IsWinner = columns[4].Trim().ToLower() == "yes"
                        });
                    }
                }

                await _awardsRepository.AddRangeAsync(awardsList);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar o arquivo CSV.", ex);
            }
        }
    }
}