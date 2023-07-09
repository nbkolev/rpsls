
# Rock paper scissors lizard spock

An endpoint implementation

## Description
The implementation, following the provided requirements, that consist of:
* ApiEndpoint: a simple Web service Endpoint that supports different configuration options to ease integration tests:
  * External random source URL and range
  * Option to bypass the external source.
  * Mock player and option to specify tha mock player choice
* GameEngine: Decoupled game engine based on the  [official game specification](https://www.wikihow.com/Play-Rock-Paper-Scissors-Lizard-Spock) that features:
  * High level of abstraction
  * External, Internal and Mocked randomness sources
  * External and Bot Players
  * 100% unit tests
* Tests: xUnit based test battery
  * It covers every aspect of GameEngine.
  * Partial test on ApiEndpoint to check whether configuration was applied correctly.

## External dependencies
* RichardSzalay.MockHttp - a library to mock HttpRequest used to simplify testing code.
  
## The focus of the solution
As it was requested `to demonstrate your ability to produce brilliant code` my implementation aims
 reach The Super-objective of programming - a future-proof architecture :)

Methods to achieve this:
* Harmonize naming conventions differences
  * Requirements are using words like "choice" and "result", but game specification is about "game move", 
and "round outcome". In order to minimize confusion. 
  The internal `Rpsls.GameEngine` is following the terms of the specification 
  and the `Rpsls.Apiendpoint` is following the requirements. 

* Analyse specifications for possible use case generalisation:
  The game specification states:_The game adds the lizard and Spock so **there are fewer ties** when you play._
   _Try out the game the next time you need to **settle a simple conflict** or just want something easy to **play with a partner**!_
  * The required API interface is explicit that there is one move per game. But the possibility for a tie in a single round is significant 
  and this is a conflict with the objective to reduce ties. That is why the interface `IMoveGenerator` was introduced and the result of the game is processed by `PlaySingleGame` this way in future `PlayMultipleGames` could be implemented by recursively calling `PlaySingleGame`. The `IPlayer` interface is designed that will allow multiple move choices.
  * Initial requirements were to be able to play against bot.
  Nevertheless it is possible to play agains other players that is why 
  `IPlayer` interface is implemented as `ExternalPlayer` and `BotPlayer`

* Good test coverage and less external libraries
  * The abstraction between GameEngine and ApiEndpoint simplifies unit testing.
  * Unit tests of ApiInterface are omitted as it is better suited for integration test.
  * External libraries are used only in the tests (in order to write a bit cleaner code).
  