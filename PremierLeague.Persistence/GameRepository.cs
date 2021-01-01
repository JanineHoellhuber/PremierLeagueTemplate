using Microsoft.EntityFrameworkCore;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.DataTransferObjects;
using PremierLeague.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PremierLeague.Persistence
{
  public class GameRepository : IGameRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public GameRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

        public async Task AddRangeAsync(IEnumerable<Game> games)
        {
            _dbContext.AddRangeAsync(games);
        }

        public async Task<IEnumerable<TeamTableRowDto>> GetAllAsync()
        {
            var teams = await _dbContext.Teams
                .Select(t => new TeamTableRowDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Matches = t.AwayGames.Count + t.HomeGames.Count,
                    Won = t.AwayGames.Count(game => game.GuestGoals > game.HomeGoals) + t.HomeGames.Count(game => game.HomeGoals > game.GuestGoals),
                    Lost = t.AwayGames.Count(game => game.GuestGoals < game.HomeGoals) + t.HomeGames.Count(game => game.HomeGoals < game.GuestGoals),
                    Plus = t.AwayGames.Sum(game => game.GuestGoals) + t.HomeGames.Sum(game => game.HomeGoals),
                    Minus = t.AwayGames.Sum(game => game.HomeGoals) + t.HomeGames.Sum(game => game.GuestGoals)

                }).ToArrayAsync();

            var orderTeams = teams
               .OrderByDescending(t => t.Points)
               .ThenByDescending(t => t.PlusMinus)
               .Select((ttrd, idx) =>
               {
                   ttrd.Rank = idx + 1;
                   return ttrd;
               }).ToArray();

            return orderTeams;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return _dbContext.Games.ToList();
        }
        public async Task AddAsync(Game games)
        {
            await _dbContext.Games.AddAsync(games);
        }
    }
}