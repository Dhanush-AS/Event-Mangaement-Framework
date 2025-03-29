using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS2.Models;

namespace EMS2.Services
{
    public class EventService : IEventService
    {
        private readonly Container _eventContainer;

        public EventService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _eventContainer = cosmosClient.GetContainer(databaseName, containerName);
        }

        // Add a new event
        public async Task AddEventAsync(EventDetails eventDetails)
        {
            await _eventContainer.CreateItemAsync(eventDetails, new PartitionKey(eventDetails.eventId.ToString()));
        }


        // Get event by EventId
        public async Task<EventDetails> GetEventByIdAsync(Guid eventId)
        {
            try
            {
                var response = await _eventContainer.ReadItemAsync<EventDetails>(eventId.ToString(), new PartitionKey(eventId.ToString()));
                var eventDetails = response.Resource;

                // Return null if the event is soft deleted
                if (eventDetails.IsDeleted)
                {
                    return null;
                }

                return eventDetails;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }


        // Get all events
        public async Task<IEnumerable<EventDetails>> GetAllEventsAsync()
        {
            var query = _eventContainer.GetItemQueryIterator<EventDetails>();
            List<EventDetails> events = new List<EventDetails>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                // Exclude events that are marked as deleted
                events.AddRange(response.Where(e => !e.IsDeleted));
            }

            return events;
        }


        // Update an event
        public async Task UpdateEventAsync(EventDetails eventDetails)
        {
            await _eventContainer.UpsertItemAsync(eventDetails, new PartitionKey(eventDetails.Id));
        }

        // Delete an event
        public async Task DeleteEventAsync(Guid eventId)
        {
            var eventDetails = await GetEventByIdAsync(eventId);

            if (eventDetails != null)
            {
                eventDetails.IsDeleted = true; // Soft delete by setting IsDeleted to true
                await UpdateEventAsync(eventDetails); // Save the update
            }
        }

    }
}
