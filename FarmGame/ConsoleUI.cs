using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Maneja toda la interacción por consola con el jugador.
    /// </summary>
    internal class ConsoleUI
    {
        #region Dependencias

        private readonly Game game;
        private readonly ShopService shop;
        private readonly FarmService farm;
        private readonly MarketService market;

        #endregion

        #region Estado interno

        private Player player;

        public ConsoleUI(Game game, ShopService shop, FarmService farm, MarketService market)
        {
            this.game = game;
            this.shop = shop;
            this.farm = farm;
            this.market = market;
        }

        #endregion

        #region Punto de entrada

        public void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            ShowTitle();
            player = AskForPlayerName();
            ShowMessage($"\nBienvenido, {player.Name}! Tu granja te espera.", ConsoleColor.Green);
            Pause();
            GameLoop();
        }

        #endregion

        #region Ciclos de juego

        private void GameLoop()
        {
            while (true)
            {
                Console.Clear();
                ShowHeader();
                ShowFarmGrid();

                int choice = ShowMainMenu();

                switch (choice)
                {
                    case 1: DoShop(); break;
                    case 2: DoPlant(); break;
                    case 3: DoHarvest(); break;
                    case 4: DoSell(); break;
                    case 5: ShowInventory(); break;
                    case 6: ShowPendingSales(); break;
                    case 7: DoExpandFarm(); break;
                    case 8: DoEndTurn(); break;
                    case 9: DoQuit(); return;
                }
            }
        }

        #endregion

        #region Pantallas de visualización

        private void ShowTitle()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Yellow, new[]
            {
                "=====================================",
                "        > JUEGO DE GRANJA <          ",
                "====================================="
            });
            Console.WriteLine();
        }

        private void ShowHeader()
        {
            WriteColor(ConsoleColor.Cyan,
                $"Dia {player.Day}  |  Granjero: {player.Name}  " +
                $"|  Oro: {player.Gold}g  " +
                $"|  Parcelas: {player.TotalPlots} (libres: {player.EmptyPlots})");
            Console.WriteLine(new string('─', 60));
        }

        private void ShowFarmGrid()
        {
            Console.WriteLine("[ GRANJA ]");
            Console.WriteLine();

            var plots = player.Farm;
            int cols = 3;

            for (int i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                string label = $"#{plot.Id,2}";
                string crop = plot.Seed != null ? $"{plot.Seed.Icon} {plot.Seed.Name,-9}" : "           ";

                ConsoleColor color = plot.State == PlotState.ReadyToHarvest ? ConsoleColor.Green
                                   : plot.State == PlotState.Growing ? ConsoleColor.Yellow
                                   : ConsoleColor.DarkGray;

                Console.Write("  ");
                WriteColor(color, $"[{label} {crop} {plot.DisplayStatus,16}]");

                if ((i + 1) % cols == 0 || i == plots.Count - 1) Console.WriteLine();
            }
            Console.WriteLine();
        }

        #endregion

        #region Menú principal
        
        private int ShowMainMenu()
        {
            WriteColor(ConsoleColor.White, "¿Que deseas hacer hoy?");
            Console.WriteLine();
            WriteColor(ConsoleColor.Cyan, "[1] Ir a la tienda (comprar semillas)");
            WriteColor(ConsoleColor.Yellow, "[2] Plantar semillas");
            WriteColor(ConsoleColor.Green, "[3] Cosechar cultivos listos");
            WriteColor(ConsoleColor.Magenta, "[4] Vender cosecha al mercado");
            WriteColor(ConsoleColor.White, "[5] Ver inventario");
            WriteColor(ConsoleColor.White, "[6] Ver ventas pendientes");
            WriteColor(ConsoleColor.DarkYellow, "[7] Expandir granja  (-50g, +4 parcelas)");
            WriteColor(ConsoleColor.Blue, "[8] Terminar turno (pasar al dia siguiente)");
            WriteColor(ConsoleColor.Red, "[9] Salir del juego");
            Console.WriteLine();
            return ReadInt("Opcion: ", 1, 9);
        }

        #endregion

        #region Gestión de tienda

        private void DoShop()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Cyan, "===> TIENDA DE SEMILLAS <===");
            Console.WriteLine();
            WriteColor(ConsoleColor.White, $"Tu oro: {player.Gold}g");
            Console.WriteLine();

            WriteColor(ConsoleColor.Yellow,
                $"{"#",-3} {"Nombre",-12} {"Compra",7} {"Crece",6} {"Venta",7} {"Demora",7}  {"Descripcion",-35}");
            Console.WriteLine(new string('─', 75));

            var seeds = SeedCatalog.All;
            for (int i = 0; i < seeds.Count; i++)
            {
                var s = seeds[i];
                Console.WriteLine(
                    $"{i + 1,-3} {s.Name,-12} {s.BuyCost,5}g  {s.GrowthDays,4}d  " +
                    $"{s.SellPrice,5}g  {s.SellDelayDays,5}d  {s.Description,-35}");
            }

            Console.WriteLine();
            WriteColor(ConsoleColor.DarkGray, "[0] Volver");
            Console.WriteLine();

            int seedChoice = ReadInt("Elegir semilla (#): ", 0, seeds.Count);
            if (seedChoice == 0) return;

            var chosen = SeedCatalog.GetByIndex(seedChoice);
            int qty = ReadInt($"Cantidad de {chosen.Name} (precio: {chosen.BuyCost}g c/u): ", 1, 999);

            var (ok, msg) = shop.BuySeeds(player, chosen.Name, qty);
            ShowResult(ok, msg);
        }

        #endregion

        #region Gestión de plantación

        private void DoPlant()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Yellow, "===> PLANTAR SEMILLAS <===");
            Console.WriteLine();

            if (player.SeedInventory.Count == 0)
            {
                ShowMessage("No tienes semillas en tu inventario. Ve a la tienda primero.", ConsoleColor.Red);
                Pause();
                return;
            }

            var emptyPlots = player.Farm.Where(p => p.IsEmpty).ToList();
            if (emptyPlots.Count == 0)
            {
                ShowMessage("No tienes parcelas vacias. Cosecha primero o expande tu granja.", ConsoleColor.Red);
                Pause();
                return;
            }

            Console.WriteLine("Semillas disponibles:");
            var seedList = player.SeedInventory.ToList();
            for (int i = 0; i < seedList.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {seedList[i].Key}  x{seedList[i].Value}");
            }

            Console.WriteLine();
            WriteColor(ConsoleColor.DarkGray, "[0] Volver");
            int seedChoice = ReadInt("Elegir semilla: ", 0, seedList.Count);
            if (seedChoice == 0) return;
            string seedName = seedList[seedChoice - 1].Key;

            Console.WriteLine();
            Console.WriteLine("Parcelas vacias:");
            for (int i = 0; i < emptyPlots.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] Parcela #{emptyPlots[i].Id}");
            }

            Console.WriteLine();
            int plotChoice = ReadInt("Elegir parcela: ", 1, emptyPlots.Count);
            int plotId = emptyPlots[plotChoice - 1].Id;

            var (ok, msg) = farm.PlantSeed(player, plotId, seedName);
            ShowResult(ok, msg);
        }

        #endregion

        #region Gestión de cosecha

        private void DoHarvest()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Green, "===> COSECHAR <===");
            Console.WriteLine();

            var readyPlots = player.Farm.Where(p => p.IsReady).ToList();
            if (readyPlots.Count == 0)
            {
                ShowMessage("No hay parcelas listas para cosechar.", ConsoleColor.Red);
                Pause();
                return;
            }

            Console.WriteLine("Parcelas listas:");
            for (int i = 0; i < readyPlots.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] Parcela #{readyPlots[i].Id}  -> {readyPlots[i].Seed.Name}");
            }

            Console.WriteLine();
            WriteColor(ConsoleColor.DarkGray, "[0] Cosechar todas");
            Console.WriteLine();
            int choice = ReadInt("Elegir parcela (0 = todas): ", 0, readyPlots.Count);

            if (choice == 0)
            {
                int count = 0;
                foreach (var plot in readyPlots)
                {
                    var (ok, msg) = farm.HarvestPlot(player, plot.Id);
                    if (ok) count++;
                    Console.WriteLine($"{(ok ? "[OK]" : "[!!]")} {msg}");
                }
                ShowMessage($"\nCosechaste {count} cultivo(s).", ConsoleColor.Green);
            }
            else
            {
                int plotId = readyPlots[choice - 1].Id;
                var (ok, msg) = farm.HarvestPlot(player, plotId);
                ShowResult(ok, msg);
            }
        }

        #endregion

        #region Gestión de venta

        private void DoSell()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Magenta, "===> VENDER AL MERCADO <===");
            Console.WriteLine();

            if (player.HarvestInventory.Count == 0)
            {
                ShowMessage("No tienes cosecha en tu inventario para vender.", ConsoleColor.Red);
                Pause();
                return;
            }

            Console.WriteLine("Cosecha disponible:");

            var cropList = player.HarvestInventory.ToList();
            for (int i = 0; i < cropList.Count; i++)
            {
                var def = SeedCatalog.GetByName(cropList[i].Key);
                string priceInfo = def != null
                    ? $"{def.SellPrice}g c/u  (demora {def.SellDelayDays} turno(s))"
                    : "precio desconocido";
                Console.WriteLine($"[{i + 1}] {cropList[i].Key,-12} x{cropList[i].Value,-4}  -> {priceInfo}");
            }

            Console.WriteLine();

            WriteColor(ConsoleColor.DarkGray, "[0] Volver");
            int choice = ReadInt("Elegir cultivo: ", 0, cropList.Count);
            if (choice == 0) return;

            string cropName = cropList[choice - 1].Key;
            int maxQty = cropList[choice - 1].Value;
            int qty = ReadInt($"Cantidad a vender (max {maxQty}): ", 1, maxQty);

            var (ok, msg) = market.SellCrops(player, cropName, qty);
            ShowResult(ok, msg);
        }

        #endregion

        #region Gestión de inventario

        private void ShowInventory()
        {
            Console.Clear();
            WriteColor(ConsoleColor.White, "===> INVENTARIO <===");
            Console.WriteLine();

            WriteColor(ConsoleColor.Cyan, "== Semillas ==");
            if (player.SeedInventory.Count == 0) Console.WriteLine("(ninguna)");
            else
            {
                foreach (var kv in player.SeedInventory)
                {
                    Console.WriteLine($"    {kv.Key,-15} x{kv.Value}");
                }
            }

            Console.WriteLine();

            WriteColor(ConsoleColor.Green, "== Cosecha (lista para vender) ==");
            if (player.HarvestInventory.Count == 0) Console.WriteLine("(ninguna)");
            else
            {
                foreach (var kv in player.HarvestInventory)
                {
                    Console.WriteLine($"{kv.Key,-15} x{kv.Value}");
                }
            }
            Console.WriteLine();
            Pause();
        }

        #endregion

        #region Gestión de ventas pendientes

        private void ShowPendingSales()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Magenta, "===> VENTAS PENDIENTES EN EL MERCADO <===");
            Console.WriteLine();

            if (player.PendingSales.Count == 0)
            {
                ShowMessage("No tienes ventas pendientes.", ConsoleColor.DarkGray);
            }
            else
            {
                int total = 0;
                foreach (var order in player.PendingSales.OrderBy(o => o.TurnsRemaining))
                {
                    Console.WriteLine($"{order.DisplayInfo}");
                    total += order.TotalValue;
                }
                Console.WriteLine();
                WriteColor(ConsoleColor.Yellow, $"Total por cobrar: {total}g");
            }

            Console.WriteLine();
            Pause();
        }

        #endregion

        #region Gestión de expansión de granja

        private void DoExpandFarm()
        {
            Console.Clear();
            WriteColor(ConsoleColor.DarkYellow, "===> EXPANDIR GRANJA <===");
            Console.WriteLine();
            Console.WriteLine($"Costo: {ShopService.ExpansionCost}g  |  " +
                              $"Nuevas parcelas: +{ShopService.ExpansionPlots}");
            Console.WriteLine($"Tu oro actual: {player.Gold}g");
            Console.WriteLine();

            string confirm = ReadString("Confirmar expansion? (s/n): ");
            if (confirm.ToLower() != "s") return;

            var (ok, msg) = shop.ExpandFarm(player);
            ShowResult(ok, msg);
        }

        #endregion

        #region Fin de turno y salida
        
        private void DoEndTurn()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Blue, $"===> FIN DEL DIA {player.Day} <===");
            Console.WriteLine();

            var events = game.EndTurn(player);

            if (events.Count > 0)
            {
                WriteColor(ConsoleColor.White, "Eventos del turno:");
                foreach (var ev in events)
                    Console.WriteLine(ev);
                Console.WriteLine();
            }
            else
            {
                ShowMessage("Dia tranquilo, sin novedades.", ConsoleColor.DarkGray);
            }

            WriteColor(ConsoleColor.Blue, $"=> Ahora es el Dia {player.Day}. Oro: {player.Gold}g");
            Console.WriteLine();
            Pause();
        }

        private void DoQuit()
        {
            Console.Clear();
            WriteColor(ConsoleColor.Yellow, new[]
            {
                "=============================",
                "     ¡Gracias por jugar!     ",
                "============================="
            });
            Console.WriteLine($"\n{player.Name} termino la partida en el Dia {player.Day}.");
            Console.WriteLine($"Oro final: {player.Gold}g");
            Console.WriteLine($"Parcelas totales: {player.TotalPlots}");
            Console.WriteLine();
        }

        #endregion

        #region Creación de jugador

        private Player AskForPlayerName()
        {
            Console.WriteLine();
            WriteColor(ConsoleColor.White, "Ingresa el nombre de tu granjero: ");
            Console.Write("> ");
            string name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "Granjero";
            return game.CreatePlayer(name);
        }

        #endregion

        #region Helpers de consola

        private void ShowResult(bool ok, string message)
        {
            Console.WriteLine();
            ShowMessage(message, ok ? ConsoleColor.Green : ConsoleColor.Red);
            Pause();
        }

        private void ShowMessage(string message, ConsoleColor color)
        {
            WriteColor(color, message);
        }

        private void WriteColor(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private void WriteColor(ConsoleColor color, string[] lines)
        {
            Console.ForegroundColor = color;
            foreach (var line in lines) Console.WriteLine(line);
            Console.ResetColor();
        }

        private void Pause()
        {
            WriteColor(ConsoleColor.DarkGray, "[Presiona Enter para continuar...]");
            Console.ReadLine();
        }

        private int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }

                WriteColor(ConsoleColor.Red,
                    $"Por favor ingresa un numero entre {min} y {max}.");
            }
        }

        private string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        #endregion
    }
}