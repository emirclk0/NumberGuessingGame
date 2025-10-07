using Microsoft.AspNetCore.Mvc;
using NumberGuessingGame.Models;

namespace NumberGuessingGame.Controllers
{
    public class GameController : Controller
    {
        private const int MaxGuesses = 10;

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("RandomNumber") == null)
            {
                return View(new GameModel { GameStarted = false });
            }

            int randomNumber = HttpContext.Session.GetInt32("RandomNumber") ?? 0;
            int score = HttpContext.Session.GetInt32("Score") ?? 0;
            int remaining = HttpContext.Session.GetInt32("RemainingGuesses") ?? MaxGuesses;
            int minRange = HttpContext.Session.GetInt32("MinRange") ?? 1;
            int maxRange = HttpContext.Session.GetInt32("MaxRange") ?? 100;
            DateTime startTime = DateTime.Parse(HttpContext.Session.GetString("StartTime") ?? DateTime.Now.ToString());

            var model = new GameModel
            {
                Guess = null,
                RandomNumber = randomNumber,
                Score = score,
                RemainingGuesses = remaining,
                MinRange = minRange,
                MaxRange = maxRange,
                ElapsedTime = DateTime.Now - startTime,
                GameStarted = true
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult StartGame(int minRange, int maxRange)
        {
            var rnd = new Random();
            int number = rnd.Next(minRange, maxRange + 1);

            HttpContext.Session.SetInt32("RandomNumber", number);
            HttpContext.Session.SetInt32("Score", 0);
            HttpContext.Session.SetInt32("RemainingGuesses", MaxGuesses);
            HttpContext.Session.SetInt32("MinRange", minRange);
            HttpContext.Session.SetInt32("MaxRange", maxRange);
            HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SubmitGuess(int? guess)
        {
            int randomNumber = HttpContext.Session.GetInt32("RandomNumber") ?? 0;
            int remaining = HttpContext.Session.GetInt32("RemainingGuesses") ?? MaxGuesses;
            int minRange = HttpContext.Session.GetInt32("MinRange") ?? 1;
            int maxRange = HttpContext.Session.GetInt32("MaxRange") ?? 100;
            DateTime startTime = DateTime.Parse(HttpContext.Session.GetString("StartTime") ?? DateTime.Now.ToString());

            if (!guess.HasValue)
            {
                var modelEmpty = new GameModel
                {
                    Guess = null,
                    RandomNumber = randomNumber,
                    Score = HttpContext.Session.GetInt32("Score") ?? 0,
                    RemainingGuesses = remaining,
                    MinRange = minRange,
                    MaxRange = maxRange,
                    ElapsedTime = DateTime.Now - startTime,
                    GameStarted = true,
                    Message = "Please enter a number!"
                };
                return View("Index", modelEmpty);
            }

            int actualGuess = guess.Value;
            remaining--;

            string message;
            int score = 0;

            // Score = kalan tahmin sayısına göre
            if (actualGuess == randomNumber)
            {
                message = "🎉 Correct! You guessed the number!";
                score = (remaining + 1) * 10; // remaining azaltıldıktan sonra +1
                remaining = 0;
            }
            else if (actualGuess > randomNumber)
            {
                message = "Too high!";
                score = 0; // yanlış tahminlerde puan yok
            }
            else
            {
                message = "Too low!";
                score = 0;
            }

            // Score’u session’da sakla (doğru tahmin için)
            HttpContext.Session.SetInt32("Score", score);
            HttpContext.Session.SetInt32("RemainingGuesses", remaining);

            var model = new GameModel
            {
                Guess = actualGuess,
                RandomNumber = randomNumber,
                Score = score,
                RemainingGuesses = remaining,
                MinRange = minRange,
                MaxRange = maxRange,
                ElapsedTime = DateTime.Now - startTime,
                GameStarted = true,
                Message = message
            };

            if (remaining <= 0)
            {
                model.Message += $" Game over! The correct number was {randomNumber}.";
                HttpContext.Session.Clear();
            }

            return View("Index", model);
        }
    }
}
