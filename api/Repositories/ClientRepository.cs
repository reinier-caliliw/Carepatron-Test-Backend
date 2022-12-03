using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public interface IClientRepository
    {
        Task<Client[]> Get();
        Task<Client[]> Search(string name);
        Task<IResult> Create(Client client);
        Task<IResult> Update(Client client);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly DataContext dataContext;

        

        public ClientRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Client[]> Get()
        {
            return await dataContext.Clients.ToArrayAsync();
        }

        public async Task<Client[]> Search(string name)
        {
            var lowerName = name.ToLower();
            return await dataContext.Clients.Where(x => x.FirstName.ToLower().StartsWith(lowerName) || x.LastName.ToLower().StartsWith(lowerName)).ToArrayAsync();
        }

        public async Task<IResult> Create(Client client)
        {
            try
            {
                await dataContext.AddAsync(client);
                await dataContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> Update(Client client)
        {
            var existingClient = await dataContext.Clients.FirstOrDefaultAsync(x => x.Id == client.Id);

            if (existingClient == null)
                return Results.NotFound();

            existingClient.FirstName = client.FirstName;
            existingClient.LastName = client.LastName;
            existingClient.Email = client.Email;
            existingClient.PhoneNumber = client.PhoneNumber;
            try
            {
                await dataContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }   
        }
    }
}

