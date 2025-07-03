namespace BL.TourImage;

public interface IMapImageGenerator
{
    Task<string> GenerateMapImageWithLeaflet(RouteResult route, string outputPath);
}