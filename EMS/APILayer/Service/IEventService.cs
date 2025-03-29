using EMS2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMS2.Services
{
    public interface IEventService
    {
        Task AddEventAsync(EventDetails eventDetails);
        Task<EventDetails> GetEventByIdAsync(Guid eventId);
        Task<IEnumerable<EventDetails>> GetAllEventsAsync();
        Task UpdateEventAsync(EventDetails eventDetails);
        Task DeleteEventAsync(Guid eventId);
    }
}
