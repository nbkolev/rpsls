using GameEngine;
namespace Rpsls.GameEngine.Tests;

public class PlayerTest
{
    [Fact]
    void IsExternalPlayer_Abstraction_Working_Correctly()
    {
        //Choice id is too high 
        Assert.Throws<ArgumentException>(() => ExternalPlayerFactory.getFixedChoicePlayerFromRequestArg("666"));
        //Choice id is negative 
        Assert.Throws<ArgumentException>(() => ExternalPlayerFactory.getFixedChoicePlayerFromRequestArg("-1"));
        //Choice id is malformed text
        Assert.Throws<ArgumentException>(() => ExternalPlayerFactory.getFixedChoicePlayerFromRequestArg("!<dgvm>^@t3248o"));

        //Assert player moves are correct
        for (int i = 0; i < Enum.GetValues<PlayerMove>().Length; i++)
        {
            IPlayer ip = ExternalPlayerFactory.getFixedChoicePlayerFromRequestArg(i.ToString());
            Assert.True(ip.GetMove().Result == (PlayerMove)i, 
                "Player move does not correspond to choice id provided.");
        }
        
        
    }
    
    [Fact]
    void IsBotPlayer_Functioning()
    {
        //Assert player moves are correct
        for (int i = 0; i < Enum.GetValues<PlayerMove>().Length; i++)
        {
            IRandSource mockSource = new RandSourceMock(i);
            IPlayer ip = new BotPlayer(mockSource);
            Assert.True(ip.GetMove().Result == (PlayerMove)i, 
                "Player move does not correspond to random choice generated.");
        }
    }
}