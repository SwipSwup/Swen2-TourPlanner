using System.Text.Json;
using BL.DTOs;
using BL.External;
using BL.TourImage;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace BL.Services;

public class TourService(
    IConfiguration config,
    ITourRepository repo,
    IRouteService routeService,
    IMapImageGenerator mapImageGenerator) : ITourService
{
    public async Task<List<TourDto>> GetAllToursAsync()
    {
        List<TourDto> dtos = new();

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
            ImagePath = await mapImageGenerator.GenerateMapImageWithLeaflet(route)
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

    public async Task UpdateTourAsync(TourDto dto)
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

    public async Task DeleteTourAsync(int tourId)
    {
        await repo.DeleteTourAsync(tourId);
    }


    public async Task<List<TourLogDto>> GetLogsForTourAsync(int tourId)
    {
        return (await repo.GetLogsForTourAsync(tourId)).Select(l => new TourLogDto
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
        List<TourDto>? tours = JsonSerializer.Deserialize<List<TourDto>>(await File.ReadAllTextAsync(filePath));
        if (tours != null)
            await CreateToursAsync(tours);
    }
}