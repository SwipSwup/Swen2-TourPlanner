using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BL.DTOs;
using BL.External;
using BL.TourImage;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using log4net;
using System.Reflection;

namespace BL.Services;

public class TourService(
    IConfiguration config,
    ITourRepository repo,
    IRouteService routeService,
    IMapImageGenerator mapImageGenerator) : ITourService
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public async Task<List<TourDto>> GetAllToursAsync()
    {
        log.Info("Fetching all tours.");

        List<TourDto> dtos = new();

        try
        {
            foreach (Tour tour in await repo.GetAllToursAsync())
            {
                dtos.Add(new TourDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    From = tour.From,
                    To = tour.To,
                    TransportType = tour.TransportType,
                    Distance = tour.Distance,
                    EstimatedTime = tour.EstimatedTime,
                    ImagePath = tour.ImagePath,
                    Popularity = await repo.GetTourPopularityAsync(tour.Id),
                    IsChildFriendly = await repo.IsChildFriendlyAsync(tour.Id),
                    TourLogs = tour.TourLogs.Select(l => new TourLogDto
                    {
                        Id = l.Id,
                        Comment = l.Comment,
                        DateTime = l.DateTime,
                        Difficulty = l.Difficulty,
                        TotalDistance = l.TotalDistance,
                        TotalTime = l.TotalTime,
                        Rating = l.Rating
                    }).ToList()
                });
            }

            log.Info($"Fetched {dtos.Count} tours successfully.");
            return dtos;
        }
        catch (Exception ex)
        {
            log.Error("Error fetching all tours.", ex);
            throw;
        }
    }

    public async Task CreateTourAsync(TourDto dto)
    {
        log.Info($"Creating tour '{dto.Name}'.");

        try
        {
            RouteResult route = await routeService.GetRouteAsync(dto.From, dto.To);

            Tour tour = new()
            {
                Name = dto.Name,
                Description = dto.Description,
                From = dto.From,
                To = dto.To,
                TransportType = dto.TransportType,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime,
                ImagePath = await mapImageGenerator.GenerateMapImageWithLeaflet(route)
            };

            await repo.AddTourAsync(tour);

            log.Info($"Tour '{dto.Name}' created successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to create tour '{dto.Name}'.", ex);
            throw;
        }
    }

    public async Task CreateToursAsync(List<TourDto> dtos)
    {
        log.Info($"Creating {dtos.Count} tours.");

        foreach (TourDto dto in dtos)
        {
            await CreateTourAsync(dto);
        }

        log.Info("All tours created successfully.");
    }

    public async Task AddTourLogAsync(int tourId, TourLogDto logDto)
    {
        log.Info($"Adding tour log to tour ID {tourId}.");

        try
        {
            TourLog tourlog = new()
            {
                TourId = tourId,
                Comment = logDto.Comment,
                DateTime = logDto.DateTime,
                Difficulty = logDto.Difficulty,
                TotalDistance = logDto.TotalDistance,
                TotalTime = logDto.TotalTime,
                Rating = logDto.Rating
            };

            await repo.AddTourLogAsync(tourlog);

            log.Info($"Tour log added to tour ID {tourId} successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to add tour log to tour ID {tourId}.", ex);
            throw;
        }
    }

    public async Task UpdateTourAsync(TourDto dto)
    {
        log.Info($"Updating tour '{dto.Name}' (ID: {dto.Id}).");

        try
        {
            RouteResult route = await routeService.GetRouteAsync(dto.From, dto.To);

            var tour = new Tour
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                From = dto.From,
                To = dto.To,
                TransportType = dto.TransportType,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime,
                ImagePath = await mapImageGenerator.GenerateMapImageWithLeaflet(route)
            };

            await repo.UpdateTourAsync(tour);

            log.Info($"Tour '{dto.Name}' updated successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to update tour '{dto.Name}'.", ex);
            throw;
        }
    }

    public async Task UpdateTourLogAsync(TourLogDto logDto)
    {
        log.Info($"Updating tour log ID {logDto.Id}.");

        try
        {
            TourLog tourlog = new()
            {
                Id = logDto.Id,
                Comment = logDto.Comment,
                DateTime = logDto.DateTime,
                Difficulty = logDto.Difficulty,
                TotalDistance = logDto.TotalDistance,
                TotalTime = logDto.TotalTime,
                Rating = logDto.Rating
            };

            await repo.UpdateTourLogAsync(tourlog);

            log.Info($"Tour log ID {logDto.Id} updated successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to update tour log ID {logDto.Id}.", ex);
            throw;
        }
    }

    public async Task DeleteTourLogAsync(int logId)
    {
        log.Info($"Deleting tour log ID {logId}.");

        try
        {
            await repo.DeleteTourLogAsync(logId);
            log.Info($"Tour log ID {logId} deleted successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to delete tour log ID {logId}.", ex);
            throw;
        }
    }

    public async Task DeleteTourAsync(int tourId)
    {
        log.Info($"Deleting tour ID {tourId}.");

        try
        {
            await repo.DeleteTourAsync(tourId);
            log.Info($"Tour ID {tourId} deleted successfully.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to delete tour ID {tourId}.", ex);
            throw;
        }
    }

    public async Task<List<TourLogDto>> GetLogsForTourAsync(int tourId)
    {
        log.Info($"Fetching logs for tour ID {tourId}.");

        try
        {
            var logs = (await repo.GetLogsForTourAsync(tourId)).Select(l => new TourLogDto
            {
                Id = l.Id,
                Comment = l.Comment,
                DateTime = l.DateTime,
                Difficulty = l.Difficulty,
                TotalDistance = l.TotalDistance,
                TotalTime = l.TotalTime,
                Rating = l.Rating
            }).ToList();

            log.Info($"Fetched {logs.Count} logs for tour ID {tourId}.");
            return logs;
        }
        catch (Exception ex)
        {
            log.Error($"Failed to fetch logs for tour ID {tourId}.", ex);
            throw;
        }
    }

    public async Task ImportToursFromJsonAsync(string filePath)
    {
        log.Info($"Importing tours from JSON file '{filePath}'.");

        try
        {
            List<TourDto>? tours = JsonSerializer.Deserialize<List<TourDto>>(await File.ReadAllTextAsync(filePath));
            if (tours != null)
            {
                await CreateToursAsync(tours);
                log.Info($"Imported {tours.Count} tours from JSON successfully.");
            }
            else
            {
                log.Warn("No tours found to import in the JSON file.");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Failed to import tours from JSON file '{filePath}'.", ex);
            throw;
        }
    }
}
