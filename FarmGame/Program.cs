using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Entrada principal del juego.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            var farmService = new FarmService();
            var marketService = new MarketService();
            var shopService = new ShopService();
            var game = new Game(farmService, marketService);

            var ui = new ConsoleUI(game, shopService, farmService, marketService);
            ui.Run();
        }
    }
}