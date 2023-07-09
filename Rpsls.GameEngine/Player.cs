using System.Runtime.InteropServices.JavaScript;

namespace GameEngine;


#region Interfaces

/// <summary>
/// <c> IMoveGenerator</c> abstracts move generation from decision source
/// </summary>
public interface IMoveGenerator
{
    public Task<PlayerMove> GetRandomMove();
}
/// <summary>
/// <c> IPlayer</c> Abstracts external and bot players
/// </summary>
public interface IPlayer
{
    Task<PlayerMove> GetMove();
}

public interface ICompetitor : IPlayer { };

#endregion

#region Implementation

public enum PlayerMove : int
{
    Rock = 0,
    Paper = 1,
    Scissors = 2,
    Lizard =3,
    Spock = 4
};


public static class PlayerMoveExtensions
{
    public static string CanonicalName(this PlayerMove move) 
        => Enum.GetName(move)?.ToLower() ?? throw new InvalidOperationException("Impossible condition detected.");
    
}

public class ExternalPlayerFactory
{
    /// <summary>
    /// <c>getFixedChoicePlayerFromRequestArg</c> Converts choiceID input to player object with specified choice
    /// </summary>
    public static IPlayer getFixedChoicePlayer(int choiceId)
    {
        return new ExternalPlayer(choiceId);
    }
}



/// <summary>
/// Class <c>ExternalPlayer</c> is used to process incoming choice
/// it is an optional abstraction to simplify the overral code.
/// This architecture supports also bot vs bot game or interactive mode.
/// </summary>
public class ExternalPlayer : IPlayer
{
    private bool _movesSpent = false;
    private PlayerMove _choiceId;
    
    
    
    public ExternalPlayer(int choiceId)
    {
        var limitOfPossibleChoices = Enum.GetValues<PlayerMove>().Length;
        if (choiceId < 0)
            throw new ArgumentException("Sanity check: choice id ought not to be a negative number");
        if (choiceId >= limitOfPossibleChoices)
            throw new ArgumentException(
                $"Sanity check: choice id must be in the range of 0 to {limitOfPossibleChoices}");

        _choiceId = (PlayerMove)choiceId;

    }

    public Task<PlayerMove> GetMove()
    {
        if (_movesSpent)
            throw new NotSupportedException(
                "The current implementation of external player supports only one stated move per request.");
        _movesSpent = true;
        
        return Task.FromResult<PlayerMove>(_choiceId);
    }
    
}
/// <summary>
/// Class <c>RandomMoveGenerator </c> is used to generate moves for players that are based on random choice
/// </summary>
public class RandomMoveGenerator : IMoveGenerator
{
    private IRandSource _choiceGenerator;
    public RandomMoveGenerator(IRandSource choiceGenerator) => _choiceGenerator = choiceGenerator;

    public async Task<PlayerMove> GetRandomMove()
    {
        var limitOfPossibleChoices = Enum.GetValues<PlayerMove>().Length;
        return (PlayerMove)await _choiceGenerator.GetRandomInt(limitOfPossibleChoices);
    }
}
/// <summary>
/// Class <c>BotPlayer </c> is and AI player implementation.
/// </summary>
public class BotPlayer : ICompetitor
{
    private IMoveGenerator _moveGenerator;
    public BotPlayer(IMoveGenerator moveGenerator) => _moveGenerator = moveGenerator;

    public  Task<PlayerMove> GetMove()
    {
        return _moveGenerator.GetRandomMove();
    }

}
/// <summary>
/// Used to present possible game moves as "choices list" for the external API
/// </summary>
public class PossibleMovePresentation
{
    public int id {get;}
    public string name{get;}

    public PossibleMovePresentation(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public static IEnumerable<PossibleMovePresentation> GetAvalableMoves()
    {
        return Enum.GetValues<PlayerMove>().Select(
            x => new PossibleMovePresentation((int)x, x.CanonicalName()));;
    }

    public static PossibleMovePresentation FromMove(PlayerMove pm)
    {
        return new PossibleMovePresentation((int)pm, pm.CanonicalName());
    }
}


#endregion