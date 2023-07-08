using GameEngine;
using RichardSzalay.MockHttp;

namespace Rpsls.GameEngine.Tests;

public class RandSourceTest
{
    
    
    [Theory]
    [InlineData("https://qwetesturl.tss/sample1", "0", 0, "")]
    [InlineData("https://qwetesturl.tss/sample2", "4", 4, "")]
    [InlineData("https://qwetesturl.tss/sample3", "5", 5, "")]
    [InlineData("https://qwetesturl.tss/sample4", "123", 4, "")]
    [InlineData("https://qwetesturl.tss/sample5","124", 5, "")]
    [InlineData("https://qwetesturl.tss/sample-excessive", "9999", 0, "External random source output out of bounds")]
    [InlineData("https://qwetesturl.tss/sample-negative", "-9999", 0, "External random source output should be greater than zero")]
    [InlineData("https://qwetesturl.tss/sample-malformed", "v4$$#@-9", 0, "External random source output not correctly formatted")]
    [InlineData("short txt", "123", 0, "locationUrl not specified")]
    public void Test_External_Random_Generator_With_Simulated_Service(string mockUrl, string returnedStr, int expectedVal,
        string aggregateExceptionMustContain)
    {
        int exampleUpperBoundary = 7;
        int exampleExternalSourceUpperBound = 1000;
        
        var msgHandler = new MockHttpMessageHandler();
        msgHandler.When(mockUrl).Respond("text/plain", returnedStr);
        var mockHttp = msgHandler.ToHttpClient();
        
        var randSourceExternal = new RandSourceExternal(mockUrl, exampleExternalSourceUpperBound, mockHttp);
          
        try
        {
            var result = randSourceExternal.GetRandomInt(exampleUpperBoundary).Result;
            Assert.True(result == expectedVal,
                $"Expected: {expectedVal}, Returned: {result}");
        }
        catch (Exception e)
        {
            Assert.True(e.ToString().Contains(aggregateExceptionMustContain) , 
                $"Expected: {aggregateExceptionMustContain}, Thrown: {e.ToString()}");
        }
        ;
    }
    
    [Fact]
    public void IsMock_Random_Generator_working()

    {
        int exampleVal = 42;
        int exampleUpperBoundary = 0;
        var mockRandomGenerator = new RandSourceMock(exampleVal);
        int returned = mockRandomGenerator.GetRandomInt(exampleUpperBoundary).Result;
        Assert.True( returned == exampleVal,
            "Mock generator should output same value every time.");
        
    }
    
    [Fact]
    public void IsInternal_Random_Generator_working()
    {
        int exampleUpperBoundary = 65536;
        var randSourceInternal = new RandSourceInternal();

        int last = -1;
        for (int i = 0; i < 1000; i++)
        {
            int returned = randSourceInternal.GetRandomInt(exampleUpperBoundary).Result;
            Assert.False(returned == last,
                "Internal generator should not repeat numbers for the provided seed for at least 1000 numbers.");
            Assert.True(returned < exampleUpperBoundary && returned > 0,
                $"returned:{returned}, Numbers have to be in the desired range 0 to {exampleUpperBoundary}");
            last = returned;
        }
    }
    
}