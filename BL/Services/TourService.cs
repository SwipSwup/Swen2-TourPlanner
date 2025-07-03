using System.Text.Json;
using BL.DTOs;
using BL.External;
using DAL.Models;
using DAL.Repositories;

namespace BL.Services;

public class TourService(ITourRepository repo, IRouteService routeService) : ITourService
{
    public async Task<List<TourDto>> GetAllToursAsync()
    {
        List<Tour> tours = await repo.GetAllToursAsync();
        List<TourDto> dtos = new();

        foreach (Tour tour in tours)
        {
            int popularity = await repo.GetTourPopularityAsync(tour.Id);
            bool isChildFriendly = await repo.IsChildFriendlyAsync(tour.Id);

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
                Popularity = popularity,
                IsChildFriendly = isChildFriendly,
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

        return dtos;
    }

    public async Task CreateTourAsync(TourDto dto)
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
        };

        await repo.AddTourAsync(tour);
    }

    public async Task CreateToursAsync(List<TourDto> dtos)
    {
        foreach (TourDto dto in dtos)
        {
            await CreateTourAsync(dto);
        }
    }

    public async Task AddTourLogAsync(int tourId, TourLogDto logDto)
    {
        TourLog log = new()
        {
            TourId = tourId,
            Comment = logDto.Comment,
            DateTime = logDto.DateTime,
            Difficulty = logDto.Difficulty,
            TotalDistance = logDto.TotalDistance,
            TotalTime = logDto.TotalTime,
            Rating = logDto.Rating
        };

        await repo.AddTourLogAsync(log);
    }

    public async Task UpdateTourLogAsync(TourLogDto logDto)
    {
        TourLog log = new()
        {
            Id = logDto.Id,
            Comment = logDto.Comment,
            DateTime = logDto.DateTime,
            Difficulty = logDto.Difficulty,
            TotalDistance = logDto.TotalDistance,
            TotalTime = logDto.TotalTime,
            Rating = logDto.Rating
        };

        await repo.UpdateTourLogAsync(log);
    }

    public async Task DeleteTourLogAsync(int logId)
    {
        await repo.DeleteTourLogAsync(logId);
    }

    public async Task<List<TourLogDto>> GetLogsForTourAsync(int tourId)
    {
        var logs = await repo.GetLogsForTourAsync(tourId);
        return logs.Select(l => new TourLogDto
        {
            Id = l.Id,
            Comment = l.Comment,
            DateTime = l.DateTime,
            Difficulty = l.Difficulty,
            TotalDistance = l.TotalDistance,
            TotalTime = l.TotalTime,
            Rating = l.Rating
        }).ToList();
    }
    public async Task ImportToursFromJsonAsync(string filePath)
    {
        string json = await File.ReadAllTextAsync(filePath);
        var tours = JsonSerializer.Deserialize<List<TourDto>>(json);
        if (tours != null)
            await CreateToursAsync(tours);
    }
}
