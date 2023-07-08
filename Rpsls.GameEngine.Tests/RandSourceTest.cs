using GameEngine;
using RichardSzalay.MockHttp;

namespace Rpsls.GameEngine.Tests;

public class RandSourceTest
{
    [Fact]
    public void IsMock_Random_Generator_working()

    {
        int exampleVal = 42;
        int exampleUpperBoundary = 0;
        var mockRandomGenerator = new RandSourceMock(exampleVal);
        int returned = mockRandomGenerator.GetRandomInt(exampleUpperBoundary).Result;
        Assert.False( returned == exampleVal,
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

    [Fact]
    public void IsExternal_Random_Generator_Adapter_Working()
    {
   
        int exampleUpperBoundary = 7;
        int exampleExternalSourceUpperBound = 1000;
        string exampleInvalidValue = "v4$$#@-9";
        string urlWithMalformedValue = "https://qwetesturl.tss/sample-malformed";
        
        var tests = new Dictionary<string, (string returnedStr, int expectedVal, string exception_message)>();
        
        tests.Add("https://qwetesturl.tss/sample1", ("0", 0, ""));
        tests.Add("https://qwetesturl.tss/sample2", ("4", 4, ""));
        tests.Add("https://qwetesturl.tss/sample3", ("5", 5, ""));
        tests.Add("https://qwetesturl.tss/sample4", ("123", 4, ""));
        tests.Add("https://qwetesturl.tss/sample5",("124", 5, ""));
        tests.Add("https://qwetesturl.tss/sample-excessive",("9999", 0, "External random source output out of bounds"));
        tests.Add("https://qwetesturl.tss/sample-negative",("-9999", 0, "External random source output should be greater than zero"));
        tests.Add("https://qwetesturl.tss/sample-malformed",("v4$$#@-9",0,"External random source output not correctly formatted"));
        tests.Add("short txt",("123", 0, "locationUrl not specified"));
        
        var msgHandler = new MockHttpMessageHandler();
        foreach (var url in tests.Keys) 
            msgHandler.When(url).Respond("text/plain", tests[url].returnedStr);

        var mockHttp = msgHandler.ToHttpClient();

        // perform tests
        
        foreach (var url in tests.Keys)
        {
            var randSourceExternal = new RandSourceExternal(url, exampleExternalSourceUpperBound, mockHttp);
          
            try
            {
                var result = randSourceExternal.GetRandomInt(exampleUpperBoundary).Result;
                Assert.True(result == tests[url].expectedVal,
                    $"Expected: {tests[url].expectedVal}, Returned: {result}");
            }
            catch(Xunit.Sdk.TrueException te){ continue;}
            catch (Exception e)
            {
                Assert.True(e.ToString().Contains(tests[url].exception_message) , 
                    $"Expected: {tests[url].exception_message}, Thrown: {e.ToString()}");
            }
  
    
        }
        
        


    }

}