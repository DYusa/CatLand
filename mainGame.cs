using System;
using System.Collections.Generic;
using System.Linq;

class CatGame
{
    // Constants
    const int TotalRounds = 4;
    const int CatCardCount = 52;
    const int BonusCardCount = 24;
    const int TerrainSlots = 5;

    // Game Variables
    List<string> Challenges = new List<string>() { "Most Collections", "Most Food", "Most Cards Placed", "Most Cards in Terrain 1", "Most Cards in Terrain 2", "Most Cards in Terrain 3", "Most Unplaced Cards" };
    Stack<string> AvailableChallenges = new Stack<string>();
    List<Player> Players = new List<Player>();
    List<Card> CatCards = new List<Card>();
    List<Card> BonusCards = new List<Card>();
    Terrain[] Terrains = new Terrain[3];

    public CatGame(int playerCount)
    {
        InitializeCards();
        InitializeTerrains();
        ShuffleChallenges();
        InitializePlayers(playerCount);
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

    void ShuffleChallenges()
    {
        Random rnd = new Random();
        var shuffled = Challenges.OrderBy(x => rnd.Next()).ToList();
        foreach (var challenge in shuffled)
        {
            AvailableChallenges.Push(challenge);
        }
    }

    void InitializePlayers(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            Players.Add(new Player($"Player {i + 1}"));
        }
    }

    public void StartGame()
    {
        Console.WriteLine("Welcome to the Cat Game!");
        Console.WriteLine("Setting up players...");

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

        string currentChallenge = AvailableChallenges.Pop();
        Console.WriteLine($"This round's challenge: {currentChallenge}");

        for (int step = 1; step <= steps; step++)
        {
            foreach (var player in Players)
            {
                player.TakeAction(Terrains, CatCards);
            }
        }

        EvaluateChallenge(currentChallenge);
    }

    void EvaluateChallenge(string challenge)
    {
        Player winner = challenge switch
        {
            "Most Collections" => Players.OrderByDescending(p => p.Collections).First(),
            "Most Food" => Players.OrderByDescending(p => p.FoodCount).First(),
            "Most Cards Placed" => Players.OrderByDescending(p => p.CardsPlaced).First(),
            "Most Cards in Terrain 1" => Players.OrderByDescending(p => p.GetCardsInTerrain(0)).First(),
            "Most Cards in Terrain 2" => Players.OrderByDescending(p => p.GetCardsInTerrain(1)).First(),
            "Most Cards in Terrain 3" => Players.OrderByDescending(p => p.GetCardsInTerrain(2)).First(),
            "Most Unplaced Cards" => Players.OrderByDescending(p => p.UnplacedCards).First(),
            _ => null
        };

        if (winner != null)
        {
            Console.WriteLine($"{winner.Name} wins this round's challenge: {challenge}!");
            winner.AddScore(10); // Award points for winning the challenge
        }
    }

    void CalculateFinalScores()
    {
        Console.WriteLine("Calculating Final Scores...");
        foreach (var player in Players)
        {
            Console.WriteLine($"{player.Name} - Score: {player.Score}");
        }

        var winner = Players.OrderByDescending(p => p.Score).First();
        Console.WriteLine($"Winner is {winner.Name} with {winner.Score} points!");
    }
}

class Player
{
    public string Name { get; private set; }
    public int Score { get; private set; }
    public int FoodCount { get; private set; }
    public int Collections { get; private set; }
    public int CardsPlaced { get; private set; }
    public int UnplacedCards { get; private set; }
    private Dictionary<int, int> TerrainCards;

    public Player(string name)
    {
        Name = name;
        Score = 0;
        FoodCount = 0;
        Collections = 0;
        CardsPlaced = 0;
        UnplacedCards = 0;
        TerrainCards = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 } };
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

        if (FoodCount < terrain.RequiredFood())
        {
            Console.WriteLine("Not enough food to place a cat here.");
            return;
        }

        var catCard = catCards.First();
        catCards.Remove(catCard);
        terrain.PlaceCat(catCard);
        FoodCount -= terrain.RequiredFood();
        CardsPlaced++;
        TerrainCards[terrainIndex]++;
        Console.WriteLine($"Placed Cat {catCard.ID} in Terrain {terrainIndex}.");
    }

    void GetFood()
    {
        FoodCount++;
        Console.WriteLine("Collected 1 food.");
    }

    void CollectStuff()
    {
        Collections++;
        FoodCount += 1;
        Console.WriteLine("Collected 1 stuff and gained 1 food.");
    }

    void RecruitCats(List<Card> catCards)
    {
        Console.WriteLine("Recruiting new cats...");
        UnplacedCards += 1;
    }

    public void AddScore(int points)
    {
        Score += points;
    }

    public int GetCardsInTerrain(int terrainIndex)
    {
        return TerrainCards.ContainsKey(terrainIndex) ? TerrainCards[terrainIndex] : 0;
    }
}

class Card
{
    public string Type { get; private set; }
    public int ID { get; private set; }
    public int FoodRequirement { get; private set; }

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

    public int RequiredFood()
    {
        return placedCats.Count + 1; // Food scales based on cats in the terrain
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter number of players (2 or 3):");
        int playerCount = int.Parse(Console.ReadLine());
        if (playerCount < 2 || playerCount > 3)
        {
            Console.WriteLine("Invalid number of players. Defaulting to 2.");
            playerCount = 2;
        }

        CatGame game = new CatGame(playerCount);
        game.StartGame();
    }
}

