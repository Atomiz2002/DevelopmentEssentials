using System;

namespace DevelopmentEssentials.Extensions.CS {

    public static class RandomExtensions {

        public static float Next(this Random _, float max)            => (float) new Random().NextDouble() * max;
        public static float Next(this Random _, float min, float max) => (float) new Random().NextDouble() * (max - min) + min;

        public static int Next(this Random _, int max)          => new Random().Next() * max;
        public static int Next(this Random _, int min, int max) => new Random().Next() * (max - min) + min;

        public static int Random(this int max)          => new Random().Next(max);
        public static int Random(this int min, int max) => new Random().Next(min, max);

    }

}