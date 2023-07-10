
# Rock paper scissors lizard spock

An endpoint implementation according to requirements provided by Patrice Egue on 07/07/2023 16:41 EET.

### Description
The implementation, following the provided requirements, that consist of:
* ApiEndpoint: a simple Web Service Endpoint that supports different configuration options to ease integration tests:
  * External random source URL and range
  * Option to bypass the external source.
  * Mock player and option to specify the mock player choice
  * Able to Play from Swagger in development mode.
* GameEngine: Decoupled game engine based on the  [official game specification](https://www.wikihow.com/Play-Rock-Paper-Scissors-Lizard-Spock) that features:
  * High level of abstraction
  * External, Internal and Mocked randomness sources
  * External and Bot Players
  * 100% unit tests
* Tests: xUnit based test battery
  * It covers every aspect of GameEngine.
  * Partial test on ApiEndpoint to check whether configuration was applied correctly.
  
### The focus of the solution
As it was requested `to demonstrate your ability to produce brilliant code` my implementation aims
to achieve it by implementing a future-proof architecture.

### Methods used
#### 1. Harmonize naming conventions differences
  * Requirements are using words like "choice" and "result", but game specification is about "game move", 
and "round outcome". In order to minimize confusion. 
  The internal `Rpsls.GameEngine` is following the terms of the specification 
  and the `Rpsls.Apiendpoint` is following the requirements. 

#### 2. Possible use case generalisation to ease implementation of new features
  The game specification Paragraph I states: _The game adds the lizard and Spock so **there are fewer ties**(#1) when you play._
   _Try out the game the next time you need to **settle a simple conflict**(#2) or just want something easy to **play with a partner**(#3)!_
  * The required API interface is explicit that there is one move per game. But the possibility for a tie (see #1) in a single round is significant 
  and this is a conflict with objective to reduce ties (see #2 and #3). 
  * That is why the interface `IMoveGenerator` was introduced and the result of the game is calculated by `PlaySingleGame`. 
  * In future a `PlayMultipleGames` could be implemented by recursively calling `PlaySingleGame`. The `IPlayer` interface is designed that will allow multiple move choices.
  * Although initial requirements were to be able to play against bot. In future it is reasonable to assume the requirement to play against other players. In this train of thought
  `IPlayer` interface is implemented as `ExternalPlayer` and `BotPlayer`.
  

#### 3. Good test coverage and less external libraries
  * The abstraction between GameEngine and ApiEndpoint simplifies unit testing.
  * Unit tests of ApiInterface are omitted as it is better suited for integration test.
  * External libraries are used only in the tests (in order to write a bit cleaner code).
  * Writing as self-documenting code as possible.


### External dependencies
  * RichardSzalay.MockHttp - a library to mock HttpRequest used to simplify testing code.

### Docker information
  * docker version 24.0.4, build 3713ee1
  * docker-compose version 1.29.2, build 5becea4c
### Build & run command
`git clone https://github.com/nbkolev/rpsls.git && cd rpsls && docker-compose build && docker compose up`
