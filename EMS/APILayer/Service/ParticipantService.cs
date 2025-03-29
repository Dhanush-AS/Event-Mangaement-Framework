using EMS2.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace EMS2.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly Container _participantContainer;

        public ParticipantService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _participantContainer = cosmosClient.GetContainer(databaseName, containerName);
        }
        //register for an event
        public async Task AddRegistrationAsync(Registration registration)
        {
            await _participantContainer.CreateItemAsync(registration, new PartitionKey(registration.regId.ToString()));
        }
        //update registrations
        public async Task UpdateRegistrationAsync(Registration registration)
        {
            await _participantContainer.UpsertItemAsync(registration, new PartitionKey(registration.regId.ToString()));
        }

        //get by id
        public async Task<Registration> GetByIdAsync(Guid id)
        {
            var response = await _participantContainer.ReadItemAsync<Registration>(id.ToString(), new PartitionKey(id.ToString()));
            var registration = response.Resource;
            if (registration.IsDeleted)
            {
                return null;
            }
            return registration;

        }
        //delete
        public async Task DeleteRegistrationAsync(Guid id)
        {
            var registration = await GetByIdAsync(id);
            if (registration != null)
            {
                registration.IsDeleted = true;
                await UpdateRegistrationAsync(registration);
            }
        }
        //get by email
        public async Task<List<Registration>> GetByEmailAsync(string email)
        {
            var query = _participantContainer.GetItemLinqQueryable<Registration>(true)
                .Where(x => x.Email == email && !x.IsDeleted)
                .ToFeedIterator();

            List<Registration> results = new List<Registration>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;  // Return a list of registrations
        }

        //get all
        public async Task<IEnumerable<Registration>> GetAllRegistrationsAsync()
        {
            var query = _participantContainer.GetItemLinqQueryable<Registration>(true)
                    .Where(r => !r.IsDeleted) 
                    .ToFeedIterator();
            List<Registration> results = new List<Registration>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);

            }
            return results;
        }

    }
}