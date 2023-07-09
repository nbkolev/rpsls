using System.Text.Json;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Rpsls.ApiEndpoint.Controllers;

public class GameControlerOptions
{
    public const string ConfigSectionName = "GameControler";
    
    /// <summary>
    /// External randomness service location
    /// If <c>ExternalRandomSourceUrl</c> uninitialized will raise an exception.
    /// </summary>
    public string ExternalRandomSourceUrl { get; set; } = null!;
    
    /// <summary>
    /// To specify random integers range 
    /// <c>ExternalRandomSourceRangeUpperBound</c> is used
    /// to check whether external generator is suitable for the game engine.
    /// </summary>
    public int ExternalRandomSourceRangeUpperBound{ get; set; } = 0;
        
    /// <summary>
    /// Useful for testing purposes.
    /// If <c>ExternalRandomSourceBypass</c> is true allows using internal random source.
    /// </summary>
    public bool ExternalRandomSourceBypass { get; set; } = false;
        
    /// <summary>
    /// For testing only!!!
    /// If <c>Debug_MockBotPlayer</c> is true allows using mock random generator for the
    /// bot that returns <c>Debug_MockBotPlayerChoice</c> output every time 
    /// </summary>
    public bool Debug_MockBotPlayer { get; set; } = false;
            
    /// <summary>
    /// For testing only!!!
    /// <c>Debug_MockBotPlayer</c> is constant choice that mock player will perform.
    /// <c>Debug_MockBotPlayer</c> have to be set to TRUE.
    /// </summary>
    public int Debug_MockBotPlayerChoice { get; set; } = (int)PlayerMove.Lizard;
        
}


// Arguments abstractions

public class ExternalPlayerChoice
{
    public int player { get; set; }//used for JSON input parsing
}




[ApiController]
[Route("/")]
[TypeFilter(typeof(ApiExceptionFilter))]
public class GameController : ControllerBase
{
    private IMoveGenerator _moveGenerator;
    private IPlayer _botPlayer;
    
    private readonly GameControlerOptions _options;

    public GameController(IOptions<GameControlerOptions> options, IHttpClientFactory clientFactory)
    {
        
        _options = options.Value;

        if (_options.ExternalRandomSourceBypass)
        {
            
            var randSourceInternal = new RandSourceInternal();
            _moveGenerator = new RandomMoveGenerator(randSourceInternal);
        }
        else
        {
            var randSourceExternal = new RandSourceExternal(_options.ExternalRandomSourceUrl,
                _options.ExternalRandomSourceRangeUpperBound,
                clientFactory.CreateClient());
            _moveGenerator = new RandomMoveGenerator(randSourceExternal);
        }
        if (_options.Debug_MockBotPlayer)
        {
            var randSourceMock = new RandSourceMock((int)_options.Debug_MockBotPlayerChoice);
            var mockMoveGenerator = new RandomMoveGenerator(randSourceMock);
            _botPlayer = new BotPlayer(mockMoveGenerator);
        }
        else
        {
            _botPlayer = new BotPlayer(_moveGenerator);
        }
    }
    
    [HttpGet]
    [Route("choices")]
    public IActionResult Choices()
    {
        var choices = GameEngine.PossibleMovePresentation.GetAvalableMoves();
        return new JsonResult(choices);
    }
    
    [HttpGet]
    [Route("choice")]
    public async Task<IActionResult> Choice()
    {
        
        var randomMove = await _moveGenerator.GetRandomMove();
        var randomChoice = PossibleMovePresentation.FromMove(randomMove);
        return new JsonResult(randomChoice);
    }
    
    [HttpPut]
    [Route("play")]
    public async Task<IActionResult> Play([FromBody] ExternalPlayerChoice choice)
    {
        var playerChoiceId = choice.player;
        var externalPlayer = new ExternalPlayer(playerChoiceId);
        var outcome = await Game.Round.PlaySingleGame(externalPlayer, _botPlayer);
        
        return new JsonResult(outcome);
    }
    
}