namespace LogiTrack.Domain.Constants;

public static class ErrorMessages
{
    public static string ShipmentNotFound(int id) => $"Shipment with ID {id} was not found.";
    public const string UpdateFailed = "Failed to update: item does not exist.";
    public const string InvalidWeight = "Weight must not be less than or equal to zero.";
    public const string InvalidWeightFormat = "Weight must be a valid number.";
    public const string InvalidCityFormat = "City names must contain only letters.";
}