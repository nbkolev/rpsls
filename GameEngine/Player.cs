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
        => Enum.GetName(move) ?? throw new InvalidOperationException("Impossible condition detected.");
}


public interface IPlayer
{
    PlayerMove GetMove();
}