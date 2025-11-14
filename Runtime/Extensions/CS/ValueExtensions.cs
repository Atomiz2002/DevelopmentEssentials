using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace DevelopmentEssentials.Extensions.CS {

    public static class ValueExtensions {

        // public static int  Clamp(this int value, int min, int max)                => Math.Clamp(value, min, max);
        public static void Clamp(this ref int value, int min, int max) => value = Mathf.Clamp(value, min, max);

        public static int Max(this int value, int competitor) => Mathf.Max(value, competitor);

        public static int Random(this ref int value, int min, int max) => new Random().Next(min, max);

        public static int RandomUnique(this int value, int max) => value.RandomUnique(0, max);

        public static int RandomUnique(this int value, int min, int max) {
            int randomUnique;

            do randomUnique = new Random().Next(min, max);
            while (randomUnique == value);

            return randomUnique;
        }

        public static float Next(this Random _, float max)            => (float) new Random().NextDouble() * max;
        public static float Next(this Random _, float min, float max) => (float) new Random().NextDouble() * (max - min) + min;

        public static bool IsWithin(this int value, int min, int max)             => value <= max && value >= min;
        public static bool IsWithin<T>(this int value, IEnumerable<T> enumerable) => 0 <= value && value < enumerable.Count();

    }

}