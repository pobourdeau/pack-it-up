using UnityEngine;

namespace Photon.Pun.Demo.Asteroids
{
    public class AsteroidsGame
    {
        public const float ASTEROIDS_MIN_SPAWN_TIME = 5.0f;
        public const float ASTEROIDS_MAX_SPAWN_TIME = 10.0f;

        public const float PLAYER_RESPAWN_TIME = 4.0f;

        public const int PLAYER_MAX_LIVES = 3;

        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public static int GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                /*case 0: return new Color(0.39f, 0.6f, 0.19f, 1f);
                case 1: return new Color(0.37f, 0.64f, 0.63f, 1f);
                case 2: return new Color(0.72f, 0.53f, 0.26f, 1f);
                case 3: return new Color(0.55f, 0.17f, 0.15f, 1f);*/
                case 0: return 0;
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
            }

            return 0;
        }
    }
}