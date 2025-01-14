# CatGame

CatGame is a turn-based card game where players compete to score the most points by placing cards, collecting food, and completing challenges across multiple rounds.

## Table of Contents
- [How to Play](#how-to-play)
- [Game Rules](#game-rules)
- [Features](#features)
- [Classes Overview](#classes-overview)
  - [CatGame](#catgame)
  - [Player](#player)
  - [Card](#card)
  - [Terrain](#terrain)
- [Setup and Execution](#setup-and-execution)

## How to Play
1. Each player starts with 10 resources, which they can allocate between food, collections, and cards in hand.
2. Players take turns performing actions:
   - Place cards in terrains.
   - Collect food.
   - Draw new cards.
3. Complete challenges to earn points. Challenges are evaluated at the end of each round.
4. The game lasts for 4 rounds, and the player with the most points at the end wins!

## Game Rules
- Players can place cards only if they meet the terrain's collection requirement.
- Placing cards consumes food equal to the card's `FoodRequirement`.
- Each terrain has a limited number of slots for cards.
- Players score points by completing challenges and placing cards effectively.

## Features
- **Customizable Players:** Each player can adjust their starting resources.
- **Unique Cat Cards:** 52 unique cat cards, each with a name and food requirement.
- **Dynamic Challenges:** Players compete to complete a randomly shuffled set of challenges.
- **Interactive Gameplay:** Players choose actions in turn-based rounds.

## Classes Overview

### CatGame
The main class that orchestrates the game logic, initializes game components, and handles the flow of the game.

#### Key Methods:
- `InitializeCards`: Creates 52 unique cards with random food requirements.
- `InitializeTerrains`: Sets up terrains with slot capacities and collection requirements.
- `ShuffleChallenges`: Randomizes the challenges for the game.
- `StartGame`: Handles the main game loop.
- `PlayRound`: Executes a round of the game.
- `EvaluateChallenge`: Determines the winner of a challenge.
- `CalculateFinalScores`: Displays the final scores and announces the winner.

### Player
Manages player-specific resources, actions, and scoring.

#### Key Methods:
- `InitializeStartingResources`: Allocates starting resources to the player.
- `TakeAction`: Allows the player to perform an action (place card, collect food, draw card).
- `PlaceCardInTerrain`: Places a card in a terrain, consuming food.
- `AddScore`: Adds points to the player's score.

### Card
Represents a cat card with attributes like name, ID, and food requirement.

#### Constructor:
- `Card(string name, int id, int foodRequirement)`: Initializes a card with specified attributes.

### Terrain
Represents a terrain where cards can be placed.

#### Key Methods:
- `AddCard`: Adds a card to the terrain if slots are available and requirements are met.

## Setup and Execution

1. Clone the repository.
   ```bash
   git clone https://github.com/DYusa/CatLand.git
   cd catgame
   ```
2. Open the project in your IDE of choice (e.g., Visual Studio). I personally used the console of Programiz (https://www.programiz.com/csharp-programming/online-compiler/).
3. Build and run the project.
4. Follow the prompts to specify the number of players and start the game.

## Future Enhancements
- Multiplayer support.
- Save and load game state.
- Point logic to be added. (DONE)
---
Enjoy playing CatGame! Feedback and contributions are welcome. Feel free to submit an issue or pull request on GitHub.
