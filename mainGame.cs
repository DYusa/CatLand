using System;
using System.Collections.Generic;
using System.Linq;

class CatGame
{
    // Constants
    const int MaxPlayers = 3;
    const int TotalRounds = 4;
    const int CatCardCount = 52;
    const int BonusCardCount = 24;

    // Game Variables
    List<string> Rounds = new List<string>() { "Round 1", "Round 2", "Round 3", "Round 4", "Round 5", "Round 6", "Round 7", "Round 8" };
    Stack<string> AvailableRounds = new Stack<string>();
    List<Player> Players = new List<Player>();
    List<Card> CatCards = new List<Card>();
    List<Card> BonusCards = new List<Card>();
    
    // Initialization
    public CatGame()
    {
        InitializeCards();
        ShuffleRounds();
    }

    void InitializeCards()
    {
        for (int i = 0; i < CatCardCount; i++)
        {
            CatCards.Add(new Card("Cat", i + 1));
        }

        for (int i = 0; i < BonusCardCount; i++)
        {
            BonusCards.Add(new Card("Bonus", i + 1));
        }
    }

    void ShuffleRounds()
    {
        Random rnd = new Random();
        var shuffled = Rounds.OrderBy(x => rnd.Next()).ToList();
        foreach (var round in shuffled)
        {
            AvailableRounds.Push(round);
        }
    }

    public void StartGame()
    {
        Console.WriteLine("Welcome to the Cat Game!");
        Console.WriteLine("Setting up players...");
        for (int i = 0; i < MaxPlayers; i++)
        {
            Players.Add(new Player($"Player {i + 1}"));
        }

        for (int round = 1; round <= TotalRounds; round++)
        {
            PlayRound(round);
        }

        CalculateFinalScores();
    }

    void PlayRound(int roundNumber)
    {
        int steps = 8 - roundNumber + 1;
        Console.WriteLine($"Starting Round {roundNumber} with {steps} steps.");

        string currentRound = AvailableRounds.Pop();
        Console.WriteLine($"This round's challenge: {currentRound}");

        for (int step = 1; step <= steps; step++)
        {
            foreach (var player in Players)
            {
                player.TakeAction();
            }
        }
    }

    void CalculateFinalScores()
    {
        Console.WriteLine("Calculating Final Scores...");
        foreach (var player in Players)
        {
            player.CalculateScore();
            Console.WriteLine($"{player.Name} - Score: {player.Score}");
        }

        var winner = Players.OrderByDescending(p => p.Score).First();
        Console.WriteLine($"Winner is {winner.Name} with {winner.Score} points!");
    }
}

class Player
{
    public string Name { get; set; }
    public int Score { get; private set; }

    public Player(string name)
    {
        Name = name;
        Score = 0;
    }

    public void TakeAction()
    {
        // Simplified actions - to be implemented with gameplay logic
        Console.WriteLine($"{Name} is taking an action...");
    }

    public void CalculateScore()
    {
        // Placeholder for score calculation logic
        Score = new Random().Next(10, 100); // Temporary random score
    }
}

class Card
{
    public string Type { get; set; }
    public int ID { get; set; }

    public Card(string type, int id)
    {
        Type = type;
        ID = id;
    }
}

class Program
{
    static void Main(string[] args)
    {
        CatGame game = new CatGame();
        game.StartGame();
    }
}
