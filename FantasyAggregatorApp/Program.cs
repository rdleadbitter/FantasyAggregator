using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Repositories;
using FantasyAggregatorApp.Models;

class Program
{
    // repositories used across the program
    static UserRepository userRepo = new UserRepository();
    static PlatformRepository platformRepo = new PlatformRepository();
    static TeamRepository teamRepo = new TeamRepository();
    static PlayerRepository playerRepo = new PlayerRepository();
    static TeamPlayerRepository teamPlayerRepo = new TeamPlayerRepository();

    static void Main(string[] args)
    {
        // load appsettings.json (requires the Microsoft.Extensions.Configuration.* packages)
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // use binder or indexer; both work if you added packages
        var conn = config.GetValue<string>("ConnectionString") ?? config["ConnectionString"];
        DbConnector.Init(conn);

        Console.WriteLine("FantasyAggregator console started.");
        MainMenu();
    }

    static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine(" 1) List Players");
            Console.WriteLine(" 2) Search Players by Name");
            Console.WriteLine(" 3) Create Player");
            Console.WriteLine(" 4) List Teams");
            Console.WriteLine(" 5) Show Team Roster");
            Console.WriteLine(" 6) Add Team");
            Console.WriteLine(" 7) Add Player to Team");
            Console.WriteLine(" 8) Export Team Roster to CSV");
            Console.WriteLine(" 9) List Users");
            Console.WriteLine("10) List Platforms");
            Console.WriteLine("11) Exit");
            Console.Write("Choose: ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1": ListPlayers(); break;
                case "2": SearchPlayers(); break;
                case "3": CreatePlayer(); break;
                case "4": ListTeams(); break;
                case "5": ShowTeamRoster(); break;
                case "6": AddTeam(); break;
                case "7": AddPlayerToTeam(); break;
                case "8": ExportRosterCsv(); break;
                case "9": ListUsers(); break;
                case "10": ListPlatforms(); break;
                case "11": return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    // --- Player related ---
    static void ListPlayers()
    {
        Console.WriteLine("\nPlayers:");
        var players = playerRepo.GetAll().ToList();
        if (!players.Any()) { Console.WriteLine("(no players found)"); return; }
        Console.WriteLine("Id\tName\t\tPos\tTeam\tActive");
        foreach (var p in players)
        {
            Console.WriteLine($"{p.PlayerId}\t{p.FullName}\t{p.Position}\t{p.TeamAbbrev}\t{p.Active}");
        }
    }

    static void SearchPlayers()
    {
        Console.Write("\nEnter name or partial name to search: ");
        var q = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(q)) { Console.WriteLine("Empty query."); return; }
        var all = playerRepo.GetAll();
        var found = all.Where(p => p.FullName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        Console.WriteLine($"\nFound {found.Count} player(s) matching '{q}':");
        foreach (var p in found)
        {
            Console.WriteLine($"{p.PlayerId}\t{p.FullName}\t{p.Position}\t{p.TeamAbbrev}\tActive:{p.Active}");
        }
    }

    static void CreatePlayer()
    {
        Console.Write("\nFull name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name required."); return; }
        Console.Write("Position: ");
        var pos = Console.ReadLine();
        Console.Write("Team Abbrev: ");
        var ta = Console.ReadLine();
        var p = new Player { FullName = name, Position = pos, TeamAbbrev = ta, Active = true };
        var id = playerRepo.Create(p);
        Console.WriteLine($"Created player id: {id}");
    }

    // --- Teams & roster ---
    static void ListTeams()
    {
        Console.WriteLine("\nTeams:");
        var teams = teamRepo.GetAll().ToList();
        if (!teams.Any()) { Console.WriteLine("(no teams found)"); return; }
        Console.WriteLine("TeamId\tUserId\tPlatformId\tTeamName\tLeagueName");
        foreach (var t in teams)
        {
            Console.WriteLine($"{t.TeamId}\t{t.UserId}\t{t.PlatformId}\t{t.TeamName}\t{t.LeagueName}");
        }
    }

    static void ShowTeamRoster()
    {
        Console.Write("\nEnter TeamId to show roster: ");
        if (!int.TryParse(Console.ReadLine(), out var teamId)) { Console.WriteLine("Bad TeamId."); return; }

        var roster = teamPlayerRepo.GetRosterWithNames(teamId).ToList();
        if (!roster.Any()) { Console.WriteLine("(no roster entries)"); return; }

        Console.WriteLine($"\nRoster for Team {teamId}:");
        Console.WriteLine("TPId\tPlayerId\tPlayerName\tSlot\tAcquiredOn");
        foreach (var (tp, name) in roster)
        {
            Console.WriteLine($"{tp.TeamPlayerId}\t{tp.PlayerId}\t{name}\t{tp.RosterSlot}\t{(tp.AcquiredOn?.ToString("yyyy-MM-dd") ?? "")}");
        }
    }

    static void AddTeam()
    {
        Console.WriteLine("\nCreate new Team");
        Console.Write("UserId (owner): ");
        if (!int.TryParse(Console.ReadLine(), out var userId)) { Console.WriteLine("Bad UserId"); return; }
        Console.Write("PlatformId: ");
        if (!int.TryParse(Console.ReadLine(), out var platformId)) { Console.WriteLine("Bad PlatformId"); return; }
        Console.Write("PlatformTeamId (remote id): ");
        var ptid = Console.ReadLine();
        Console.Write("TeamName: ");
        var name = Console.ReadLine();
        Console.Write("LeagueName (optional): ");
        var league = Console.ReadLine();

        var t = new Team { UserId = userId, PlatformId = platformId, PlatformTeamId = ptid, TeamName = name, LeagueName = league };
        var id = teamRepo.Create(t);
        Console.WriteLine($"Created TeamId: {id}");
    }

    static void AddPlayerToTeam()
    {
        Console.WriteLine("\nAdd player to team");
        Console.Write("TeamId: ");
        if (!int.TryParse(Console.ReadLine(), out var teamId)) { Console.WriteLine("Bad TeamId"); return; }
        Console.Write("PlayerId: ");
        if (!int.TryParse(Console.ReadLine(), out var playerId)) { Console.WriteLine("Bad PlayerId"); return; }
        Console.Write("RosterSlot (e.g., QB1, RB2): ");
        var slot = Console.ReadLine();
        Console.Write("AcquiredOn (yyyy-MM-dd) or leave blank: ");
        var acq = Console.ReadLine();
        DateTime? acqDate = null;
        if (!string.IsNullOrWhiteSpace(acq))
        {
            if (DateTime.TryParse(acq, out var d)) acqDate = d;
            else { Console.WriteLine("Bad date; leaving empty."); }
        }
        var tp = new TeamPlayer { TeamId = teamId, PlayerId = playerId, RosterSlot = slot, AcquiredOn = acqDate };
        try
        {
            var id = teamPlayerRepo.Create(tp);
            Console.WriteLine($"Created TeamPlayerId: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding player to team: " + ex.Message);
        }
    }

    static void ExportRosterCsv()
    {
        Console.Write("\nEnter TeamId to export roster: ");
        if (!int.TryParse(Console.ReadLine(), out var teamId)) { Console.WriteLine("Bad TeamId"); return; }
        var roster = teamPlayerRepo.GetRosterWithNames(teamId).ToList();
        if (!roster.Any()) { Console.WriteLine("No roster entries to export."); return; }

        var fileName = $"roster_{teamId}.csv";
        using var sw = new StreamWriter(fileName);
        sw.WriteLine("TeamPlayerId,PlayerId,PlayerName,RosterSlot,AcquiredOn");
        foreach (var (tp, name) in roster)
        {
            var acq = tp.AcquiredOn?.ToString("yyyy-MM-dd") ?? "";
            var safeName = name?.Replace(",", ""); // very basic CSV-safety
            sw.WriteLine($"{tp.TeamPlayerId},{tp.PlayerId},{safeName},{tp.RosterSlot},{acq}");
        }
        Console.WriteLine($"Exported {roster.Count} rows to {fileName} (project root).");
    }

    // --- Users & platforms ---
    static void ListUsers()
    {
        Console.WriteLine("\nUsers:");
        var users = userRepo.GetAll().ToList();
        if (!users.Any()) { Console.WriteLine("(no users)"); return; }
        Console.WriteLine("UserId\tUsername\tEmail\tCreatedAt");
        foreach (var u in users)
            Console.WriteLine($"{u.UserId}\t{u.Username}\t{u.Email}\t{u.CreatedAt:yyyy-MM-dd}");
    }

    static void ListPlatforms()
    {
        Console.WriteLine("\nPlatforms:");
        var plats = platformRepo.GetAll().ToList();
        if (!plats.Any()) { Console.WriteLine("(no platforms)"); return; }
        Console.WriteLine("PlatformId\tName\tUrl");
        foreach (var p in plats)
            Console.WriteLine($"{p.PlatformId}\t{p.Name}\t{p.Url}");
    }
}
