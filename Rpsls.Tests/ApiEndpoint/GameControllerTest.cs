using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using Rpsls.ApiEndpoint.Controllers;
using Moq;

namespace Rpsls.GameEngine.Tests.ApiEndpoint;

public class MockOptionsWrapper: IOptions<GameControlerOptions>{
    public GameControlerOptions Value { get;}
    public MockOptionsWrapper(GameControlerOptions v) => Value = v;
}

public class MockHttpFactory : IHttpClientFactory
{
    private HttpClient x;
    public MockHttpFactory(HttpClient x) => this.x = x;
    public HttpClient CreateClient(string name)
    {
        return x;
    }
}



public class GameControllerTest
{
   
    
    [Theory]
    [InlineData("http://test.text/random",true,true,3,"{\"random\" : 0}")]
    [InlineData("http://test.text/random",false,false,0,"{\"random\" : 5}")]
    public async Task Controller_Configuration_Valid2(
        string extUrl, bool extGenBypass, bool mockPlayer, int mockPlayerChoice, string mockedExternalReply )
    {
        
        var msgHandler = new MockHttpMessageHandler();
        msgHandler.When(extUrl).Respond("application/json", mockedExternalReply);
        var mockHttp = msgHandler.ToHttpClient();
        
        var gco = new GameControlerOptions();
        gco.Debug_MockBotPlayer = mockPlayer;
        gco.Debug_MockBotPlayerChoice = mockPlayerChoice;
        gco.ExternalRandomSourceRangeUpperBound = 10;
        gco.ExternalRandomSourceUrl = extUrl;
        gco.ExternalRandomSourceBypass = extGenBypass;
        
        var gameController = new GameController(
            new MockOptionsWrapper(gco),
            new MockHttpFactory(mockHttp) );

        var botMove = await gameController.BotPlayer.GetMove();
        
        Assert.True((int)botMove == mockPlayerChoice, $"Mock player choice mismatch:" +
                                                      $" Expected {mockPlayerChoice} Got {(int)botMove}");
    }

}