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

    public async Task UpdateTourAsync(Tour updatedTour)
    {

        var existingTour = await context.Tours.FindAsync(updatedTour.Id);

        if (existingTour != null)
        {
            existingTour.Name = updatedTour.Name;
            existingTour.Description = updatedTour.Description;
            existingTour.From = updatedTour.From;
            existingTour.To = updatedTour.To;
            existingTour.TransportType = updatedTour.TransportType;
            existingTour.Distance = updatedTour.Distance;
            existingTour.EstimatedTime = updatedTour.EstimatedTime;
            existingTour.ImagePath = updatedTour.ImagePath;

            await context.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Tour with ID {updatedTour.Id} not found.");
        }
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

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding tour log: {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
        }
    }



    public async Task<List<TourLog>> GetLogsForTourAsync(int tourId)
        => await context.TourLogs.Where(t => t.TourId == tourId).ToListAsync();

    public async Task UpdateTourLogAsync(TourLog updatedLog)
    {
        var existingLog = await context.TourLogs.FindAsync(updatedLog.Id);

        if (existingLog != null)
        {

            existingLog.DateTime = updatedLog.DateTime;
            existingLog.TotalTime = updatedLog.TotalTime;
            existingLog.TotalDistance = updatedLog.TotalDistance;
            existingLog.Comment = updatedLog.Comment;
            existingLog.Difficulty = updatedLog.Difficulty;
            existingLog.Rating = updatedLog.Rating;

            await context.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"TourLog with ID {updatedLog.Id} not found.");
        }
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