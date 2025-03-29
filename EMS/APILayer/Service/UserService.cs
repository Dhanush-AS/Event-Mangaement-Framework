using EMS2.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS2.Services
{
    public class UserService : IUserService
    {
        private readonly Container _userContainer;

        public UserService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _userContainer = cosmosClient.GetContainer(databaseName, containerName);
        }
        public async Task AddUserAsync(Users user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _userContainer.CreateItemAsync(user, new PartitionKey(user.Userid.ToString()));
        }
        public async Task<Users> GetUserByIdAsync(Guid id)
        {
            var response = await _userContainer.ReadItemAsync<Users>(id.ToString(), new PartitionKey(id.ToString()));
            var user = response.Resource;
            if (user.IsDeleted)
            {
                return null;
            }
            return user;
        }
        public async Task UpdateUserAsync(Users user)
        {
            await _userContainer.UpsertItemAsync(user,new PartitionKey(user.Userid.ToString()));
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if(user != null)
            {
                user.IsDeleted = true;
                await UpdateUserAsync(user);
            }
        }
        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            var query = _userContainer.GetItemQueryIterator<Users>();
            List<Users> user = new List<Users>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                // Exclude events that are marked as deleted
                user.AddRange(response.Where(e => !e.IsDeleted));
            }

            return user;
        }

        public async Task<List<Users>> GetByEmailAsync(string email)
        {
            var query = _userContainer.GetItemLinqQueryable<Users>(true)
                .Where(x => x.Email == email && !x.IsDeleted)
                .ToFeedIterator();

            List<Users> results = new List<Users>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;  // Return a list of registrations
        }
    }
}