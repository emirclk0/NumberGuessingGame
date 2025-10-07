using System;

namespace NumberGuessingGame.Models
{
    public class GameModel
    {
        public int? Guess { get; set; }  
        public int Score { get; set; }
        public int RandomNumber { get; set; }
        public int RemainingGuesses { get; set; }
        public string Message { get; set; }

        public int MinRange { get; set; }
        public int MaxRange { get; set; }

        public TimeSpan ElapsedTime { get; set; }
        public bool GameStarted { get; set; }
    }
}
