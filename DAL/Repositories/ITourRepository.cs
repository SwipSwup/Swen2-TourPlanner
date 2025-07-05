using DAL.Models;

namespace DAL.Repositories;

public interface ITourRepository
{
    Task<List<Tour>> GetAllToursAsync();
    Task<Tour?> GetTourByIdAsync(int id);
    Task<List<Tour>> SearchToursAsync(string query);
    Task AddTourAsync(Tour tour);
    Task UpdateTourAsync(Tour tour);
    Task DeleteTourAsync(int id);

    Task AddTourLogAsync(TourLog log);
    Task<List<TourLog>> GetLogsForTourAsync(int tourId);
    Task UpdateTourLogAsync(TourLog log);
    Task DeleteTourLogAsync(int logId);


    Task<int> GetTourPopularityAsync(int tourId);
    Task<bool> IsChildFriendlyAsync(int tourId);
}