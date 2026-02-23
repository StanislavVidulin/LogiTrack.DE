using LogiTrack.Application.Interfaces;
using LogiTrack.Domain.Entities;
using LogiTrack.Infrastructure.Data;

namespace LogiTrack.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _context;

        public ShipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Shipment> GetAll() => _context.Shipments.ToList();

        public Shipment? GetById(int id) => _context.Shipments.Find(id);

        public void Add(Shipment shipment) => _context.Shipments.Add(shipment);

        public void Update(Shipment shipment) => _context.Shipments.Update(shipment);

        public void Delete(Shipment shipment) => _context.Shipments.Remove(shipment);

        public void Save() => _context.SaveChanges();
    }
}