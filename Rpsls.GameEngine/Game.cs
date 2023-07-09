using System.Numerics;
using System.Runtime.CompilerServices;
using static GameEngine.PlayerMove;

namespace GameEngine;

public class GameResultSummary
{
    public GameResultSummary(GameResult results, PlayerMove player, PlayerMove opponent)
    {
        this.results = results.CanonicalName();
        this.player = (int)player;
        bot = (int)opponent;
    }
    public string results { get; }
    public int player { get; }
    public int bot { get; }
}

public enum GameResult
{
    Win,
    Lose,
    Tie
}
public static class  GameResultExtensions
{
    public static string CanonicalName(this GameResult gameResult)
        => Enum.GetName(gameResult)?.ToLower() ?? throw new Exception();
}
public class Game
{

    /// <summary>
    /// Method <c>IsStrategyWinning</c> performs game outcome check according to the specification on:
    /// https://www.wikihow.com/Play-Rock-Paper-Scissors-Lizard-Spock
    /// </summary>
    ///

    private static bool IsStrategyWinning(PlayerMove player, PlayerMove opponent)
    {
        // *** Scissors cuts paper.
        if (player == Scissors && opponent == Paper) return true;
        
        // *** Paper covers rock.
        if (player == Paper && opponent == Rock) return true;
        
        // *** Rock crushes lizard.
        if (player == Rock && opponent == Lizard) return true;
        // *** Lizard poisons Spock.
        if (player == Lizard && opponent == Spock) return true;
        
        // *** Spock smashes scissors.
        if (player == Spock && opponent == Scissors) return true;
        
        // *** Scissors decapitates lizard.
        if (player == Scissors && opponent == Lizard) return true;
        
        // *** Lizard eats paper.
        if (player == Lizard && opponent == Paper) return true;
        
        // *** Paper disproves Spock.
        if (player == Paper && opponent == Spock) return true;
        
        // *** Spock vaporizes rock.
        if (player == Spock && opponent == Rock) return true;
        
        // *** Rock crushes scissors.
        if (player == Rock && opponent == Scissors) return true;

        // Everything else is not winning strategy for this player
        return false;
    }
    
    public static GameResult CheckOutcome(PlayerMove player, PlayerMove opponent)
    {
        //Check according to known winning moves
        if (IsStrategyWinning(player, opponent)) return GameResult.Win;
        if (IsStrategyWinning(opponent, player)) return GameResult.Lose;
        
        // Everything else is a tie
        return GameResult.Tie;
    }

  
    
    public class Round
    {
        public static async Task<GameResultSummary> PlaySingleGame(IPlayer player, IPlayer opponent)
        {
            var playerMove = await player.GetMove();
            var opponentMove = await opponent.GetMove();
            var outcome = CheckOutcome(playerMove, opponentMove);
            return new GameResultSummary(outcome, playerMove, opponentMove);
        }
    }
    
}