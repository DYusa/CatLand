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
    const int TerrainSlots = 5;

    // Game Variables
    List<string> Rounds = new List<string>() { "Round 1", "Round 2", "Round 3", "Round 4", "Round 5", "Round 6", "Round 7", "Round 8" };
    Stack<string> AvailableRounds = new Stack<string>();
    List<Player> Players = new List<Player>();
    List<Card> CatCards = new List<Card>();
    List<Card> BonusCards = new List<Card>();
    Terrain[] Terrains = new Terrain[3];

    public CatGame()
    {
        InitializeCards();
        InitializeTerrains();
        ShuffleRounds();
    }

    void InitializeCards()
    {
        Random rnd = new Random();
        for (int i = 0; i < CatCardCount; i++)
        {
            int foodRequirement = rnd.Next(1, 4);
            CatCards.Add(new Card("Cat", i + 1, foodRequirement));
        }

        for (int i = 0; i < BonusCardCount; i++)
        {
            BonusCards.Add(new Card("Bonus", i + 1, 0));
        }
    }

    void InitializeTerrains()
    {
        for (int i = 0; i < 3; i++)
        {
            Terrains[i] = new Terrain(TerrainSlots);
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
                player.TakeAction(Terrains, CatCards);
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

    public void TakeAction(Terrain[] terrains, List<Card> catCards)
    {
        Console.WriteLine($"{Name}, choose an action: 0 - Place Cat, 1 - Get Food, 2 - Collect Stuff, 3 - Recruit Cats");
        int action = int.Parse(Console.ReadLine());

        switch (action)
        {
            case 0:
                PlaceCat(terrains, catCards);
                break;
            case 1:
                GetFood();
                break;
            case 2:
                CollectStuff();
                break;
            case 3:
                RecruitCats(catCards);
                break;
            default:
                Console.WriteLine("Invalid action. Try again.");
                TakeAction(terrains, catCards);
                break;
        }
    }

    void PlaceCat(Terrain[] terrains, List<Card> catCards)
    {
        Console.WriteLine("Choose a terrain (0, 1, or 2):");
        int terrainIndex = int.Parse(Console.ReadLine());
        if (terrainIndex < 0 || terrainIndex >= terrains.Length)
        {
            Console.WriteLine("Invalid terrain. Try again.");
            PlaceCat(terrains, catCards);
            return;
        }

        var terrain = terrains[terrainIndex];
        if (!terrain.HasSpace())
        {
            Console.WriteLine("No space in this terrain. Try another action.");
            return;
        }

        if (!catCards.Any())
        {
            Console.WriteLine("No more cats available to place.");
            return;
        }

        var catCard = catCards.First();
        catCards.Remove(catCard);
        terrain.PlaceCat(catCard);
        Console.WriteLine($"Placed Cat {catCard.ID} in Terrain {terrainIndex}.");
    }

    void GetFood()
    {
        Console.WriteLine("Getting food...");
        // Food collection logic here
    }

    void CollectStuff()
    {
        Console.WriteLine("Collecting stuff...");
        // Collection logic here
    }

    void RecruitCats(List<Card> catCards)
    {
        Console.WriteLine("Recruiting new cats...");
        // Recruitment logic here
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
    public int FoodRequirement { get; set; }

    public Card(string type, int id, int foodRequirement)
    {
        Type = type;
        ID = id;
        FoodRequirement = foodRequirement;
    }
}

class Terrain
{
    private int capacity;
    private List<Card> placedCats;

    public Terrain(int capacity)
    {
        this.capacity = capacity;
        placedCats = new List<Card>();
    }

    public bool HasSpace()
    {
        return placedCats.Count < capacity;
    }

    public void PlaceCat(Card cat)
    {
        if (HasSpace())
        {
            placedCats.Add(cat);
        }
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
