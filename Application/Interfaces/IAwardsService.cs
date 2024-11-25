using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAwardsService
    {
        Task<AwardsResponseDTO?> GetProducerIntervalsAsync();
        Task LoadDataFromCsvAsync(string filePath);
    }
}