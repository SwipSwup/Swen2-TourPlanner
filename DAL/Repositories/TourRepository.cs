using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class TourRepository : ITourRepository
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TourRepository));

    private readonly TourPlannerContext context;

    public TourRepository(TourPlannerContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Tour>> GetAllToursAsync()
    {
        Logger.Info("Fetching all tours with logs.");
        return await context.Tours.Include(t => t.TourLogs).ToListAsync();
    }

    public async Task<Tour?> GetTourByIdAsync(int id)
    {
        Logger.Info($"Fetching tour by ID: {id}");
        return await context.Tours.Include(t => t.TourLogs).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddTourAsync(Tour tour)
    {
        Logger.Info($"Adding new tour: {tour.Name}");
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        Logger.Info("Tour added successfully.");
    }

    public async Task UpdateTourAsync(Tour updatedTour)
    {
        Logger.Info($"Updating tour with ID: {updatedTour.Id}");
        Tour? existingTour = await context.Tours.FindAsync(updatedTour.Id);

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
            Logger.Info("Tour updated successfully.");
        }
        else
        {
            string msg = $"Tour with ID {updatedTour.Id} not found.";
            Logger.Error(msg);
            throw new Exception(msg);
        }
    }

    public async Task DeleteTourAsync(int id)
    {
        Logger.Info($"Deleting tour with ID: {id}");
        Tour? tour = await context.Tours.FindAsync(id);

        if (tour is null)
        {
            Logger.Warn($"Tour with ID {id} not found. Nothing to delete.");
            return;
        }

        context.Tours.Remove(tour);
        await context.SaveChangesAsync();
        Logger.Info("Tour deleted successfully.");
    }

    public async Task AddTourLogAsync(TourLog log)
    {
        Logger.Info($"Adding new tour log for TourId: {log.TourId}");
        context.TourLogs.Add(log);

        try
        {
            await context.SaveChangesAsync();
            Logger.Info("Tour log added successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error("Error adding tour log.", ex);
        }
    }

    public async Task<List<TourLog>> GetLogsForTourAsync(int tourId)
    {
        Logger.Info($"Fetching logs for tour ID: {tourId}");
        return await context.TourLogs.Where(t => t.TourId == tourId).ToListAsync();
    }

    public async Task UpdateTourLogAsync(TourLog updatedLog)
    {
        Logger.Info($"Updating tour log with ID: {updatedLog.Id}");
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
            Logger.Info("Tour log updated successfully.");
        }
        else
        {
            string msg = $"TourLog with ID {updatedLog.Id} not found.";
            Logger.Error(msg);
            throw new Exception(msg);
        }
    }

    public async Task DeleteTourLogAsync(int logId)
    {
        Logger.Info($"Deleting tour log with ID: {logId}");
        TourLog? log = await context.TourLogs.FindAsync(logId);
        if (log is null)
        {
            Logger.Warn($"TourLog with ID {logId} not found. Nothing to delete.");
            return;
        }

        context.TourLogs.Remove(log);
        await context.SaveChangesAsync();
        Logger.Info("Tour log deleted successfully.");
    }

    public async Task<List<Tour>> SearchToursAsync(string query)
    {
        Logger.Info($"Searching tours with query: {query}");
        return await context.Tours
            .Include(t => t.TourLogs)
            .Where(t =>
                EF.Functions.ILike(t.Name, $"%{query}%") ||
                EF.Functions.ILike(t.Description, $"%{query}%") ||
                t.TourLogs.Any(l => EF.Functions.ILike(l.Comment, $"%{query}%"))
            )
            .ToListAsync();
    }

    public async Task<int> GetTourPopularityAsync(int tourId)
    {
        Logger.Info($"Getting popularity for tour ID: {tourId}");
        return await context.TourLogs.CountAsync(l => l.TourId == tourId);
    }

    public async Task<bool> IsChildFriendlyAsync(int tourId)
    {
        Logger.Info($"Checking if tour ID {tourId} is child-friendly.");
        List<TourLog> logs = await context.TourLogs.Where(l => l.TourId == tourId).ToListAsync();

        bool isChildFriendly = logs.Count > 0 && logs.All(l =>
            l is { Difficulty: <= 2, TotalTime.TotalHours: <= 2, TotalDistance: <= 5 });

        Logger.Info($"Tour ID {tourId} child-friendly: {isChildFriendly}");
        return isChildFriendly;
    }
}
