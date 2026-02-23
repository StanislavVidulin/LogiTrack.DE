namespace LogiTrack.Application.DTOs;

public class ShipmentCreateDto
{
    public string CityFrom { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public double Weight { get; set; }
}