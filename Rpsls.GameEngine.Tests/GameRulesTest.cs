using GameEngine;
using static GameEngine.GameResult;
using static GameEngine.PlayerMove;

namespace Rpsls.GameEngine.Tests;

public class GameRulesTest
{
    [Fact]
    void Is_Exshaustive_Game_Rules_Check_Passing()
    {
        //player, oppnent, outcome
       var positiveGames =  new(PlayerMove player, PlayerMove opponent, GameResult result) [] 
       {
           (Scissors, Paper , Win),
           (Paper, Rock , Win),
           (Rock, Lizard , Win),
           (Lizard, Spock , Win),
           (Spock, Scissors , Win),
           (Scissors, Lizard , Win),
           (Lizard, Paper , Win),
           (Paper, Spock , Win),
           (Spock, Rock , Win),
           (Rock, Scissors , Win),
       };
       
       //Switch opponent and player places
       var negativeGames = positiveGames.Select(game => (game.opponent, game.player, Lose));
       var gamesCount = Enum.GetValues<PlayerMove>().Length;
       
       //Generate all tied games
       var tiedGames =
           new (PlayerMove player, PlayerMove opponent, GameResult result) [gamesCount];
       for (int i = 0; i < gamesCount; i++)
       {
           tiedGames[i] = ((PlayerMove)i, (PlayerMove)i, Tie);
       }

       var allGames = new List<(PlayerMove player, PlayerMove opponent, GameResult result)>();
       allGames.AddRange(positiveGames);
       allGames.AddRange(negativeGames);
       allGames.AddRange(tiedGames);

       //test each generated game in the engine
       foreach (var game in from game in allGames 
                let result = GameRules.CheckOutcome(game.player, game.opponent) select game)
       {
           Assert.True(GameRules.CheckOutcome(game.player,game.opponent) == game.result, 
               $"Game player:{game.player} opponent: {game.opponent}" +
               " the outcome: {result} was not expected, should be: {game.result}");
       }
       
    }
}