using System.Data;
using System.Runtime.InteropServices;

namespace GameEngine;

public interface IRandSource
{
    Task<int> GetRandomInt(int upperBound);
}


public class RandSourceExternal : IRandSource
{
    private string? _locationUrl = null;
    private int _externalSourceUpperBound = 0;
    private static HttpClient _client = new HttpClient();

    public RandSourceExternal(string locationUrl, int externalSourceUpperBound)
    {
        if (externalSourceUpperBound <= 0)
        {
            throw new InvalidConstraintException("a Random source with zero upper bound will return only zeros");
        }
        _externalSourceUpperBound = externalSourceUpperBound;
        _locationUrl = locationUrl;
    }

    public async Task<int>  GetRandFromExternalSrc(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        string output = await response.Content.ReadAsStringAsync();

        try
        {
            return int.Parse(output);
        }
        catch (FormatException e)
        {
            throw  new ExternalException(
                "External random source output not correctly formatted. Expected integer.");
        }
    }

    public async Task<int> GetRandomInt(int upperBound)
    {
        if (_locationUrl == null || _locationUrl.Length < "https://*/random".Length)
            throw new ArgumentException("locationUrl not specified.");
        
        var returnedInt = await GetRandFromExternalSrc(_locationUrl);
        if (returnedInt > _externalSourceUpperBound)
            throw new ExternalException("External random source output out of bounds");
            
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
    public RandSourceMock(int desiredOutput) => _desiredOutput = 0;
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

