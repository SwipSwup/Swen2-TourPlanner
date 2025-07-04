﻿using BL.DTOs.Report;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BL.Reports;

public static class TourReportGenerator
{
    public static void Generate(TourReportDto tour, string outputPath)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));
                
                page.Header().Text($"Tour Report: {tour.Name}").FontSize(20).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().Text($"From: {tour.From}   To: {tour.To}");
                    col.Item().Text($"Transport: {tour.TransportType}   Distance: {tour.Distance} km   Est. Time: {tour.EstimatedTime}");
                    col.Item().Text($"Description: {tour.Description}");
                    
                    if (File.Exists(tour.ImagePath))
                        col.Item().Image(tour.ImagePath, ImageScaling.FitWidth);

                    col.Item().Text("Logs").FontSize(16).Bold().Underline();

                    foreach (var log in tour.Logs)
                    {
                        col.Item().Text($"Date: {log.DateTime:g} | Rating: {log.Rating}/5 | Distance: {log.TotalDistance} km | Time: {log.TotalTime} | Difficulty: {log.Difficulty}");
                        col.Item().Text($"Comment: {log.Comment}");
                        col.Item().LineHorizontal(0.5f);
                    }
                });

                page.Footer().AlignCenter().Text(x => x.Span("Generated by TourPlanner").FontSize(10));
            });
        });

        document.GeneratePdf(outputPath);
    }
}