using Microsoft.AspNetCore.Mvc;
using LogiTrack.Application.Interfaces;
using LogiTrack.Application.DTOs;
using LogiTrack.Domain.Constants;

namespace LogiTrack.Api.Controllers;

[ApiController]
[Route("api/shipment")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var shipments = _shipmentService.GetAll();
        return Ok(shipments);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var shipment = _shipmentService.GetById(id);

        if (shipment == null)
            return NotFound(new { message = ErrorMessages.ShipmentNotFound(id) });

        return Ok(shipment);    
    }

    [HttpPost]
    public IActionResult Add(ShipmentCreateDto dto)
    {
        var shipment = _shipmentService.Add(dto);

        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, ShipmentCreateDto dto)
    {
        var shipment = _shipmentService.Update(id, dto);

        if (shipment == null)
            return NotFound(new { message = ErrorMessages.UpdateFailed });

        return Ok(shipment);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deletedShipment = _shipmentService.Delete(id);

        if (!deletedShipment)
            return NotFound(new { message = ErrorMessages.ShipmentNotFound(id) });

        return NoContent();
    }

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {

        var result = _shipmentService.UpdateStatus(id, dto.Status);

        if (!result)
            return NotFound(new { message = "Shipment not found" });

        return Ok();
    }
}