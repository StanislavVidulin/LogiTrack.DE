using LogiTrack.Domain.Entities;

namespace LogiTrack.Application.Interfaces
{
    public interface IShipmentRepository
    {
        IEnumerable<Shipment> GetAll();
        Shipment? GetById(int id);
        void Add(Shipment shipment);
        void Update(Shipment shipment);
        void Delete(Shipment shipment);
        void Save();
    }
}