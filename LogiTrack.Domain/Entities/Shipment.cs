using LogiTrack.Domain.Enums;

namespace LogiTrack.Domain.Entities;
public class Shipment
{
    public int Id { get; set; }
    public string CityFrom { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public double Weight { get; set; }
    public decimal Price { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}