using Microsoft.EntityFrameworkCore;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PremierLeague.Persistence
{
  public class TeamRepository : ITeamRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public TeamRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

        public async Task AddRangeAsync(IEnumerable<Team> teams)
        {
            await _dbContext.AddRangeAsync(teams);
        }

        public async Task<Team> GetById(int id)
        {
           return await _dbContext.Teams.FirstOrDefaultAsync(t => t.Id == id);
        }


        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _dbContext.Teams.ToArrayAsync();
        }


    }
}