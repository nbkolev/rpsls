using GameEngine;
namespace Rpsls.GameEngine.Tests;

public class PlayerTest
{
    [Fact]
    public void IsExternalPlayer_Abstraction_Working_Correctly()
    {
        //Choice id is too high 
        Assert.Throws<ArgumentException>(() => ExternalPlayerFactory.getFixedChoicePlayer(666));
        //Choice id is negative 
        Assert.Throws<ArgumentException>(() => ExternalPlayerFactory.getFixedChoicePlayer(-1));
        
        //Assert player moves are correct
        for (int i = 0; i < Enum.GetValues<PlayerMove>().Length; i++)
        {
            IPlayer ip = ExternalPlayerFactory.getFixedChoicePlayer(i);
            Assert.True(ip.GetMove().Result == (PlayerMove)i, 
                "Player move does not correspond to choice id provided.");
        }
    }

    [Fact]
    public async void Has_Only_One_Choice__Available_In_ExternalPlayer()
    {
        IPlayer ip =ExternalPlayerFactory.getFixedChoicePlayer(0);
        await ip.GetMove();
        Assert.Throws<NotSupportedException>(() => ip.GetMove().Result);
    }

    [Fact]
    public void IsCanonicalMoveNameCorrect()
    {
        Assert.True(PlayerMove.Lizard.CanonicalName()=="lizard", "Canonical names incorrect.");
    }
    
    [Fact]
    public void AvailableMovesCountCorrect()
    {
        Assert.True(PossibleMovePresentation.GetAvalableMoves().Count()>0, 
            "Possible should be available.");
    }
    
    [Fact]
    public void Possible_Move_Presentation_Correct()
    {
        var presented = PossibleMovePresentation.FromMove(PlayerMove.Lizard);
        Assert.True(presented.name == "lizard" && presented.id ==(int)PlayerMove.Lizard,
            "Move presentation incorrect.");
    }

    [Fact]
    public void IsBotPlayer_Functioning()
    {
        //Assert player moves are correct
        for (int i = 0; i < Enum.GetValues<PlayerMove>().Length; i++)
        {
            IRandSource mockSource = new RandSourceMock(i);
            IPlayer ip = new BotPlayer(new RandomMoveGenerator(mockSource));
            Assert.True(ip.GetMove().Result == (PlayerMove)i, 
                "Player move does not correspond to random choice generated.");
        }
    }
}