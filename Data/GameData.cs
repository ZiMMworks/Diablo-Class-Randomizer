using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DiabloClassRandomizer.Data
{
    // One Diablo game: its name, its logo banner, its results background and every playable class.
    public class DiabloGame
    {
        public DiabloGame(string name, Image logo, string backgroundFile, params string[] classes)
        {
            Name = name;
            Logo = logo;
            BackgroundFile = backgroundFile;
            Classes = classes.ToList().AsReadOnly();
        }

        // Game name shown to the player. Also used as the game tile's Tag.
        public string Name { get; }

        // Logo banner (built into the .exe through Properties\Resources).
        public Image Logo { get; }

        // Results screen background (embedded from Resources\Images), tinted to match the logo.
        public string BackgroundFile { get; }

        public IReadOnlyList<string> Classes { get; }
    }

    // Every supported Diablo game and its class roster (updated October 2026).
    public static class GameData
    {
        public static readonly IReadOnlyList<DiabloGame> Games = new List<DiabloGame>
        {
            // Bard and Barbarian are Hellfire's hidden classes (playable in DevilutionX or with command.txt).
            new DiabloGame("Diablo", Properties.Resources.diablo1, "result_diablo1.png",
                "Bard", "Barbarian", "Monk", "Rogue", "Sorcerer", "Warrior"),

            // Warlock added with the Reign of the Warlock DLC (2026).
            new DiabloGame("Diablo II: Resurrected", Properties.Resources.diablo2, "result_diablo2.png",
                "Amazon", "Assassin", "Barbarian", "Druid", "Necromancer", "Paladin", "Sorceress", "Warlock"),

            new DiabloGame("Diablo III", Properties.Resources.diablo3, "result_diablo3.png",
                "Barbarian", "Crusader", "Demon Hunter", "Monk", "Necromancer", "Witch Doctor", "Wizard"),

            // Paladin and Warlock added with the Lord of Hatred expansion (2026).
            new DiabloGame("Diablo IV", Properties.Resources.diablo4, "result_diablo4.png",
                "Barbarian", "Druid", "Necromancer", "Paladin", "Rogue", "Sorcerer", "Spiritborn", "Warlock"),

            // Tempest (2024), Druid (2025) and Warlock (2026) added after launch.
            new DiabloGame("Diablo Immortal", Properties.Resources.diabloimmortal, "result_diabloimmortal.png",
                "Barbarian", "Blood Knight", "Crusader", "Demon Hunter", "Druid",
                "Monk", "Necromancer", "Tempest", "Warlock", "Wizard"),
        }.AsReadOnly();

        // Finds a game by name (the game tiles store the name in their Tag).
        public static DiabloGame GetGame(string name)
        {
            return Games.FirstOrDefault(game => game.Name == name);
        }
    }
}
