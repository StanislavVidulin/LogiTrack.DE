using LogiTrack.Application.DTOs;

namespace LogiTrack.Application.Interfaces;

public interface IShipmentService
{
    IEnumerable<ShipmentResponseDto> GetAll();
    ShipmentResponseDto? GetById(int id);
    ShipmentResponseDto Add(ShipmentCreateDto dto);
    ShipmentResponseDto? Update(int id, ShipmentCreateDto dto);
    bool Delete(int id);

    bool UpdateStatus(int id, string newStatus);
}