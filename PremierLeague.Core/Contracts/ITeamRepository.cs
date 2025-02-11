﻿using PremierLeague.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PremierLeague.Core.Contracts
{
  public interface ITeamRepository
  {
    Task AddRangeAsync(IEnumerable<Team> teams);
        Task<Team> GetById(int id);
        Task<IEnumerable<Team>> GetAllTeamsAsync();
    }
}