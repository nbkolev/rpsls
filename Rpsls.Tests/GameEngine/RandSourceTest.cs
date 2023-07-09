using System.Data;
using GameEngine;
using RichardSzalay.MockHttp;

namespace Rpsls.GameEngine.Tests;

public class RandSourceTest
{
    
    
    [Theory]
    [InlineData("https://qwetesturl.tss/sample1", "{\"random\" : 0}" , 0,7,1000)]
    [InlineData("https://qwetesturl.tss/sample2", "{\"random\" : 4}", 4,7,1000)]
    [InlineData("https://qwetesturl.tss/sample3", "{\"random\" : 5}", 5,7,1000)]
    [InlineData("https://qwetesturl.tss/sample4", "{\"random\" : 123}", 4,7,1000)]
    [InlineData("https://qwetesturl.tss/sample5", "{\"random\" : 124}", 5,7,1000)]

    public void Test_External_Random_Generator_With_Simulated_Service(
        string mockUrl, string returnedJson, int expectedVal, int exampleUpperBoundary, int exampleExternalSourceUpperBound)
    {
        var msgHandler = new MockHttpMessageHandler();
        msgHandler.When(mockUrl).Respond("application/json", returnedJson);
        var mockHttp = msgHandler.ToHttpClient();
        
        var randSourceExternal = new RandSourceExternal(mockUrl, exampleExternalSourceUpperBound, mockHttp);
        
        var result = randSourceExternal.GetRandomInt(exampleUpperBoundary).Result;
        Assert.True(result == expectedVal,
            $"Expected: {expectedVal}, Returned: {result}");
        
    }

    [Fact]
    public void Test_External_Random_Generator_Exception_Handling()
    {
        //External random source output out of bounds"
        Assert.Throws<AggregateException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "https://qwetesturl.tss/sample-excessive", "{\"random\" : 9999}", 0, 7, 1000));
        //External random source output should be greater than zero
        Assert.Throws<AggregateException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "https://qwetesturl.tss/sample-negative", "{\"random\" : -9999}", 0, 7, 1000));
        //External random source output not correctly formatted
        Assert.Throws<AggregateException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "https://qwetesturl.tss/sample-malformed", "{v4$$}#@::-9", 0, 7, 1000));
        //LocationUrl not specified
        Assert.Throws<AggregateException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "short txt", "locationUrl not specified", 0, 7, 1000));
        // Incorrect external source boundary
        
        Assert.Throws<InvalidConstraintException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "https://qwetesturl.tss/sample-excessive", "{\"random\" : 9999}", 0, 4, -1));
        // Too little external source boundary
        
        Assert.Throws<InvalidConstraintException>(() =>
            Test_External_Random_Generator_With_Simulated_Service(
                "https://qwetesturl.tss/sample-excessive", "{\"random\" : 9999}", 0, 1, 5));
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