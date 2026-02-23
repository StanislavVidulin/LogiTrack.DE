using LogiTrack.Application.DTOs;
using LogiTrack.Application.Interfaces;
using LogiTrack.Domain.Enums;
using LogiTrack.Domain.Entities;

namespace LogiTrack.Business.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _repository;

    public ShipmentService(IShipmentRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<ShipmentResponseDto> GetAll()
    {
        return _repository.GetAll()
            .Select(s => MapToResponseDto(s))
            .ToList();
    }

    public ShipmentResponseDto? GetById(int id)
    {
        var shipment = _repository.GetById(id);
        return shipment == null ? null : MapToResponseDto(shipment);
    }

    public ShipmentResponseDto Add(ShipmentCreateDto dto)
    {
        var entity = MapToEntity(dto);

        entity.Price = (decimal)(dto.Weight * 1.5) + 15;
        entity.Status = ShipmentStatus.Pending;

        _repository.Add(entity);
        _repository.Save();

        return MapToResponseDto(entity);
    }

    public ShipmentResponseDto? Update(int id, ShipmentCreateDto dto)
    {
        var shipment = _repository.GetById(id);
        if (shipment == null)
            return null;

        shipment.CityFrom = dto.CityFrom;
        shipment.CityTo = dto.CityTo;
        shipment.Weight = dto.Weight;
        shipment.Price = (decimal)(dto.Weight * 1.5) + 15;

        _repository.Update(shipment);
        _repository.Save();

        return MapToResponseDto(shipment);
    }

    public bool Delete(int id)
    {
        var shipment = _repository.GetById(id);
        if (shipment == null)
            return false;

        _repository.Delete(shipment);
        _repository.Save();
        return true;
    }

    public bool UpdateStatus(int id, string newStatus)
    {
        var shipment = _repository.GetById(id);
        if (shipment == null) return false;

        if (Enum.TryParse<ShipmentStatus>(newStatus, true, out var status))
        {
            shipment.Status = status;
            _repository.Update(shipment);
            _repository.Save();
            return true;
        }
        return false;
    }

    // Mapping Methods

    // CreateDto -> Entity
    private static Shipment MapToEntity(ShipmentCreateDto dto)
    {
        return new Shipment
        {
            CityFrom = dto.CityFrom,
            CityTo = dto.CityTo,
            Weight = dto.Weight
        };
    }

    // Entity -> ResponseDto 
    private static ShipmentResponseDto MapToResponseDto(Shipment s)
    {
        return new ShipmentResponseDto
        {
            Id = s.Id,
            CityFrom = s.CityFrom,
            CityTo = s.CityTo,
            Weight = s.Weight,
            Price = s.Price,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        };
    }

}