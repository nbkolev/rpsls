using System.Data;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace GameEngine;

public interface IRandSource
{
    Task<int> GetRandomInt(int upperBound);
}

public class ExternalRandSourceData
{
    //used for JSON parsing in RandSourceExternal
    public int random { get; set; } 
}
public class RandSourceExternal : IRandSource
{
    private string? _locationUrl = null;
    private int _externalSourceUpperBound = 0;
    private HttpClient _client = null!;

    public RandSourceExternal(string locationUrl, int externalSourceUpperBound, HttpClient httpClient)
    {
        if (externalSourceUpperBound <= 0)
        {
            throw new InvalidConstraintException("A random source with zero upper bound will return only zeros.");
        }
        var limitOfPossibleChoices = Enum.GetValues<PlayerMove>().Length;
        if (externalSourceUpperBound <= limitOfPossibleChoices)
        {
            throw new InvalidConstraintException(
                $"This generator is unsuitable for this game. Expected generator with upper bound of at least {limitOfPossibleChoices}");
        }
        _externalSourceUpperBound = externalSourceUpperBound;
        _locationUrl = locationUrl;
        _client = httpClient;
    }

    public async Task<int>  GetRandFromExternalSrc(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        ExternalRandSourceData? output = await response.Content.ReadFromJsonAsync<ExternalRandSourceData>();
        
        if (output == null){
            throw new InvalidDataException(
                "Returned data incorrect. Expected: {\"random\": <integer>}");
        }

        return output.random;
    }

    public async Task<int> GetRandomInt(int upperBound)
    {
        if (_locationUrl == null || _locationUrl.Length < "https://*/random".Length)
            throw new ArgumentException("locationUrl not specified.");
        
        var returnedInt = await GetRandFromExternalSrc(_locationUrl);
        if (returnedInt > _externalSourceUpperBound)
            throw new ExternalException("External random source output out of bounds.");
        if (returnedInt < 0)
            throw new ExternalException("External random source output should be greater than zero.");
            
        var normalisedInt = returnedInt % upperBound; 
        return normalisedInt;
    }
    
}

/// <summary>
/// Class <c>RandSourceMock</c> is dummy generator
/// returning constant number every time. Useful for testing purposes.
/// </summary>
public class RandSourceMock : IRandSource
{
    private int _desiredOutput = 0;
    public RandSourceMock(int desiredOutput) => _desiredOutput = desiredOutput;
    public Task<int> GetRandomInt(int upperBound) => Task.FromResult(_desiredOutput);
}

/// <summary>
/// Class <c>RandSourceInternal</c> is a variant that does not call external sources. Useful for testing purposes.
/// </summary>
public class RandSourceInternal : IRandSource
{
    private Random _seed = new Random();
    public Task<int> GetRandomInt(int upperBound) => Task.FromResult(_seed.Next(0, upperBound));
    
}