namespace GameEngine;


public class GameRound
{
    static async Task<GameResult> PlaySingleGame(IPlayer player, IPlayer opponent)
    {
        var playerMove = await player.GetMove();
        var opponentMove = await opponent.GetMove();
        return GameRules.CheckOutcome(playerMove, opponentMove);
    }
}