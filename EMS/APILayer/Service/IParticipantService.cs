using EMS2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMS2.Services
{
    public interface IParticipantService
    {
        Task AddRegistrationAsync(Registration registration);
        Task DeleteRegistrationAsync(Guid id);
        Task UpdateRegistrationAsync(Registration registration);
        Task<IEnumerable<Registration>> GetAllRegistrationsAsync();
        Task<Registration> GetByIdAsync(Guid id);
        Task<List<Registration>> GetByEmailAsync(string email);

    }
}
