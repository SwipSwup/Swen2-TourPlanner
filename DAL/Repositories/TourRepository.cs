using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class TourRepository(TourPlannerContext context) : ITourRepository
{
    public async Task<List<Tour>> GetAllToursAsync()
        => await context.Tours.Include(t => t.TourLogs).ToListAsync();

    public async Task<Tour?> GetTourByIdAsync(int id)
        => await context.Tours.Include(t => t.TourLogs).FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddTourAsync(Tour tour)
    {
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTourAsync(Tour tour)
    {
        context.Tours.Update(tour);
        await context.SaveChangesAsync();
    }

    public async Task DeleteTourAsync(int id)
    {
        Tour? tour = await context.Tours.FindAsync(id);
        Console.WriteLine("TEST");
        if (tour is null) return;

        context.Tours.Remove(tour);
        await context.SaveChangesAsync();
    }

    public async Task AddTourLogAsync(TourLog log)
    {
        context.TourLogs.Add(log);
        await context.SaveChangesAsync();
    }

    public async Task<List<TourLog>> GetLogsForTourAsync(int tourId)
        => await context.TourLogs.Where(t => t.TourId == tourId).ToListAsync();

    public async Task UpdateTourLogAsync(TourLog log)
    {
        context.TourLogs.Update(log);
        await context.SaveChangesAsync();
    }

    public async Task DeleteTourLogAsync(int logId)
    {
        TourLog? log = await context.TourLogs.FindAsync(logId);
        if (log is null) return;

        context.TourLogs.Remove(log);
        await context.SaveChangesAsync();
    }

    public async Task<List<Tour>> SearchToursAsync(string query)
        => await context.Tours
            .Include(t => t.TourLogs)
            .Where(t =>
                EF.Functions.ILike(t.Name, $"%{query}%") ||
                EF.Functions.ILike(t.Description, $"%{query}%") ||
                t.TourLogs.Any(l =>
                    EF.Functions.ILike(l.Comment, $"%{query}%")
                ))
            .ToListAsync();

    public async Task<int> GetTourPopularityAsync(int tourId)
        => await context.TourLogs.CountAsync(l => l.TourId == tourId);

    public async Task<bool> IsChildFriendlyAsync(int tourId)
    {
        List<TourLog> logs = await context.TourLogs.Where(l => l.TourId == tourId).ToListAsync();

        return logs.Count > 0 && logs.All(l =>
            l is { Difficulty: <= 2, TotalTime.TotalHours: <= 2, TotalDistance: <= 5 });
    }
}