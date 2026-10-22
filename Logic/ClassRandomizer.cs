using System;
using System.Collections.Generic;
using DiabloClassRandomizer.Data;

namespace DiabloClassRandomizer.Logic
{
    // Picks a random class for a game. It's static so it remembers the last pick between screens,
    // which means you never get the exact same class twice in a row for the same game.
    public static class ClassRandomizer
    {
        private static readonly Random _random = new Random();
        private static DiabloGame _lastGame;
        private static string _lastClass;

        public static string Roll(DiabloGame game)
        {
            string pickedClass;
            do
            {
                pickedClass = PickAny(game.Classes);
            } while (game.Classes.Count > 1 && game == _lastGame && pickedClass == _lastClass);

            _lastGame = game;
            _lastClass = pickedClass;
            return pickedClass;
        }

        // Random helper the UI uses for the spinning "slot machine" animation.
        public static T PickAny<T>(IReadOnlyList<T> items)
        {
            return items[_random.Next(items.Count)];
        }
    }
}
