using BackendServer.Services.Interfaces;
using ContentDB.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Services
{
    public class EventService : IEventService
    {
        private readonly ContentDBContext _context;

        public EventService(ContentDBContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task CreateEventAsync(Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
        }
    }
}
