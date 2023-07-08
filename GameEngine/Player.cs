namespace GameEngine;


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
        => Enum.GetName(move).ToLower() ?? throw new InvalidOperationException("Impossible condition detected.");
    
}


public interface IPlayer
{
    Task<PlayerMove> GetMove();
}


class ExternalPlayerFactory
{
    static IPlayer getFixedChoicePlayerFromRequestArg(string requestArg)
    {
        int choiceId = -1; //invalid choice 
        try
        {
            choiceId = int.Parse(requestArg);
        }
        catch (FormatException e)
        {
            throw  new ArgumentException(
                $"choice ID is not valid number in range 0 to {Enum.GetValues<PlayerMove>().Length}");
        }

        return new ExternalPlayer(choiceId);
    }
}


/// <summary>
/// Class <c>ExternalPlayer</c> is used to process incoming choice
/// it is an optional abstraction to simplify the overral code.
/// This architecture supports also bot vs bot game or interactive mode.
/// </summary>
class ExternalPlayer : IPlayer
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
            throw new NotImplementedException(
                "The current implementation of external player supports only one stated move per request.");
        _movesSpent = true;
        
        return Task.FromResult<PlayerMove>(_choiceId);
    }
    
}

class AiPlayer : IPlayer
{
    private IRandSource _choiceGenerator;
    public AiPlayer(IRandSource choiceGenerator) => _choiceGenerator = choiceGenerator;

    public async Task<PlayerMove> GetMove()
    {
        var limitOfPossibleChoices = Enum.GetValues<PlayerMove>().Length;
        return (PlayerMove)await _choiceGenerator.GetRandomInt(limitOfPossibleChoices);
    }

}