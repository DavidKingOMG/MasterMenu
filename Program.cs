using System;
using System.Collections.Specialized;
using System.Drawing.Printing;
using System.Security.Cryptography;
using MamboDMA.Games;
using MamboDMA.Games.ABI;
using MamboDMA.Games.ArcRaiders;
using MamboDMA.Games.DayZ;
using MamboDMA.Games.Deadlock;
using MamboDMA.Games.Example;
using MamboDMA.Games.Reforger;
using MamboDMA.Services;
using Raylib_cs;
using static MamboDMA.Misc;
using static MamboDMA.OverlayUI;
// using static MamboDMA.OverlayUI; // ← remove this

namespace MamboDMA
{
    internal static class Program
    {
        private enum UiChoice { Advanced, Simple, Game}

        private static UiChoice AskUiChoice(string[] args)
        {
            Console.WriteLine("******* Welcome to MasterMenuDMA!!! ****** ");
            Console.WriteLine("******* Welcome to MasterMenuDMA!!! ****** ");
            Console.WriteLine("******* Welcome to MasterMenuDMA!!! ****** ");

            Console.WriteLine("Choose an option");
            Console.WriteLine("0. Advanced UI");
            Console.WriteLine("1. Simple UI");
            Console.WriteLine("2. Game UI");

            Console.WriteLine("Select UI Mode: ");
            string choice = Console.ReadLine();

            switch (choice)
            {

                case "0":
                    Console.WriteLine("Advanced UI selected.");
                    return UiChoice.Advanced;
                    break;
                case "1":
                    Console.WriteLine("Simple UI selected.");
                    return UiChoice.Simple;
                    break;
                case "2":
                    Console.WriteLine("Game UI Selected.");
                    return UiChoice.Game;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Defaulting to Advanced UI.");
                    return UiChoice.Simple;
                    break;
            }

            
        }




        private static (string title, Action draw) ResolveUi(UiChoice choice)
            => choice switch
            {
                UiChoice.Simple   => ("MamboDMA · Simple",   ServiceDemoUI.Draw),
                UiChoice.Advanced => ("MamboDMA · Advanced", OverlayUI.Draw),
                UiChoice.Game     => ("MamboDMA · Game", () => GameSelector.Draw()),
            };

        private static void Main(string[] args)
        {
            JobSystem.Start(workers: 3);

            // Use CLI args:  --ui adv  /  --ui simple  /  --ui game
            var choice = AskUiChoice(args);
            var (title, drawLoop) = ResolveUi(choice);

            using var win = new OverlayWindow(title, 1100, 700);
            OverlayWindowApi.Bind(win);

            // Register all game plugins here:
            GameRegistry.Register(new ReforgerGame());
            GameRegistry.Register(new DayZGame());
            GameRegistry.Register(new ExampleGame());
            GameRegistry.Register(new ABIGame());
            GameRegistry.Register(new ArcRaidersGame());
            GameRegistry.Register(new DeadlockGame());

            Image icon = Raylib.LoadImage("Assets/Img/Logo.png");
            Raylib.SetWindowIcon(icon);
            Raylib.UnloadImage(icon);
            Win32IconHelper.SetWindowIcons("Assets/Img/Logo.ico");

            try { win.Run(drawLoop); }
            finally
            {
                try { VmmService.DisposeVmm(); } catch { }
                try { JobSystem.Stop(); } catch { }
                try { DayZUpdater.Stop(); } catch { }
            }
        }

    }
}
