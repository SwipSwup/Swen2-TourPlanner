using BL.DTOs;

namespace BL.Services;

public interface ITourService
{
    Task<List<TourDto>> GetAllToursAsync();
    Task CreateTourAsync(TourDto dto);
    Task CreateToursAsync(List<TourDto> dtos);
    Task AddTourLogAsync(int tourId, TourLogDto logDto);
    Task UpdateTourLogAsync(TourLogDto logDto);
    Task DeleteTourAsync(int tourId);
    Task DeleteTourLogAsync(int logId);
    Task<List<TourLogDto>> GetLogsForTourAsync(int tourId);
    Task ImportToursFromJsonAsync(string filePath);
}