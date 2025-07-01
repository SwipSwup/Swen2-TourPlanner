namespace BL.External;

public interface IRouteService
{
    Task<RouteResult> GetRouteAsync(string from, string to);
}