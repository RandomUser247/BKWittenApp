using ContentDB.Migrations;

namespace BackendServer.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task CreateEventAsync(Event newEvent);
    }

}
