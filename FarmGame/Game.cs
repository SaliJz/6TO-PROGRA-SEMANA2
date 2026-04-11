using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    internal class Game
    {
        #region Constantes de configuración

        public const int StartingGold = 100;
        public const int StartingPlots = 6;

        private readonly FarmService farmService;
        private readonly MarketService marketService;

        #endregion

        #region Constructor

        public Game(FarmService farmService, MarketService marketService)
        {
            this.farmService = farmService;
            this.marketService = marketService;
        }

        public Player CreatePlayer(string name)
        {
            var player = new Player
            {
                Name = name,
                Gold = StartingGold,
                Day = 1
            };

            for (int i = 1; i <= StartingPlots; i++)
            {
                player.Farm.Add(new FarmPlot { Id = i });
            }

            return player;
        }

        #endregion

        #region Lógica de turno

        public List<string> EndTurn(Player player)
        {
            var events = new List<string>();

            var salesMessages = marketService.ProcessPendingSales(player);
            if (salesMessages.Count > 0)
            {
                events.Add("===> Cobros del mercado <===");
                events.AddRange(salesMessages);
            }

            farmService.AdvanceGrowth(player);

            foreach (var plot in player.Farm)
            {
                if (plot.IsReady)
                {
                    events.Add($"  Parcela #{plot.Id} ({plot.Seed.Name}) esta lista para cosechar.");
                }
            }

            player.Day++;

            return events;
        }

        #endregion
    }
}