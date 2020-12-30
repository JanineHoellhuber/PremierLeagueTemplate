using PremierLeague.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace PremierLeague.ImportConsole
{
  public static class ImportController
  {
    public async static Task<IEnumerable<Game>> ReadFromCsvAsync()
    {
            string[][] matrix = await MyFile.ReadStringMatrixFromCsvAsync("PremierLeague.csv", false);  // keine Titelzeile
                                                                                                        // Einlesen der Spiele und der Teams
                                                                                                        // Zuerst die Teams
            var teams = matrix
                    .Select(t => t[1])
                    .Union(matrix.Select(line => line[2]))
                    .Select(team => new Team
                    {
                        Name = team
                    }).ToArray();

            var games = matrix
                .Select(g => new Game
                {
                    Round = Convert.ToInt32(g[0]),
                    HomeTeam = teams.Single(s => s.Name == g[1]),
                    GuestTeam = teams.Single(s => s.Name == g[2]),
                    GuestGoals = Convert.ToInt32(g[4]),
                    HomeGoals = Convert.ToInt32(g[3])
                }).ToArray();

            return games;
        }

  }
}
