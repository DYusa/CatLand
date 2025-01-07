using System;
using System.Collections.Generic;
using System.Linq;

class CatGame
{
    // Constants
    public const int CatCardCount = 52;
    const int TotalRounds = 4;
    const int TerrainSlots = 5;

    // Game Variables
    List<string> Challenges = new List<string>()
    {
        "Most Collections",
        "Most Food",
        "Most Cards Placed",
        "Most Cards in Terrain 1",
        "Most Cards in Terrain 2",
        "Most Cards in Terrain 3",
        "Most Unplaced Cards"
    };
    Stack<string> AvailableChallenges = new Stack<string>();
    List<Player> Players = new List<Player>();
    List<Card> CatCards = new List<Card>();
    Terrain[] Terrains = new Terrain[4];

    public CatGame(int playerCount)
    {
        InitializeCards();
        InitializeTerrains();
        ShuffleChallenges();
        InitializePlayers(playerCount);
    }

    void InitializeCards()
    {
        string[] catNames = {
            "Stray Cat", "Tabby Cat", "Fat Cat", "Norwegian Forest Cat", "Siamese Cat",
            "Maine Coon", "Persian Cat", "Ragdoll Cat", "Bengal Cat", "Sphynx Cat",
            "Scottish Fold", "British Shorthair", "Abyssinian Cat", "Russian Blue", "Birman Cat",
            "Himalayan Cat", "Turkish Van", "Egyptian Mau", "Oriental Cat", "Devon Rex",
            "Cornish Rex", "Savannah Cat", "Manx Cat", "Balinese Cat", "Singapura Cat",
            "Burmese Cat", "Exotic Shorthair", "Chartreux Cat", "Birman Cat", "American Shorthair",
            "Japanese Bobtail", "Somali Cat", "Tonkinese Cat", "Turkish Angora", "Cymric Cat",
            "Singapura Cat", "LaPerm Cat", "Munchkin Cat", "Snowshoe Cat", "Australian Mist",
            "Korat Cat", "Peterbald Cat", "Selkirk Rex", "Singapura Cat", "Turkish Angora",
            "Javanese Cat", "Havana Brown", "Burmilla Cat", "Ocicat", "Khao Manee",
            "Chausie Cat", "Maine Coon", "American Curl", "Manx Cat", "Bengal Cat"
        };

        Random rnd = new Random();
        for (int i = 0; i < CatCardCount; i++)
        {
            int foodRequirement = rnd.Next(1, 4); // Random food requirement between 1 and 3
            CatCards.Add(new Card(catNames[i], i + 1, foodRequirement));
        }
    }

    void InitializeTerrains()
    {
        for (int i = 0; i < 5; i++)
        {
            Terrains[i] = new Terrain(TerrainSlots, i < 2 ? 0 : i < 3 ? 1 : 2); // Collection requirement: 0, 1, or 2
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
            var player = new Player($"Player {i + 1}");
            player.InitializeStartingResources(CatCards);
            Players.Add(player);
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
                player.DisplayStatus();
                player.TakeAction(Terrains, CatCards);
            }
        }

        EvaluateChallenge(currentChallenge);
    }

    void EvaluateChallenge(string challenge)
    {
        var potentialWinners = Players.GroupBy(player =>
        {
            return challenge switch
            {
                "Most Collections" => player.Collections,
                "Most Food" => player.FoodCount,
                "Most Cards Placed" => player.CardsPlaced,
                "Most Cards in Terrain 1" => player.GetCardsInTerrain(0),
                "Most Cards in Terrain 2" => player.GetCardsInTerrain(1),
                "Most Cards in Terrain 3" => player.GetCardsInTerrain(2),
                "Most Unplaced Cards" => player.UnplacedCards,
                _ => 0
            };
        }).OrderByDescending(g => g.Key).FirstOrDefault();

        if (potentialWinners == null || potentialWinners.Key == 0 || potentialWinners.Count() > 1)
        {
            Console.WriteLine("It's a tie or no valid winner for this challenge. No points awarded.");
        }
        else
        {
            Player winner = potentialWinners.First();
            Console.WriteLine($"{winner.Name} wins this round's challenge: {challenge}!");
            winner.AddScore(10);
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
    private List<Card> Hand;

    public Player(string name)
    {
        Name = name;
        Score = 0;
        FoodCount = 0;
        Collections = 0;
        CardsPlaced = 0;
        UnplacedCards = 0;
        TerrainCards = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 } };
        Hand = new List<Card>();
    }

    public void InitializeStartingResources(List<Card> catCards)
    {
        Random rnd = new Random();
        FoodCount = 5;
        Collections = 5;
        for (int i = 0; i < 5; i++)
        {
            var card = catCards[rnd.Next(catCards.Count)];
            Hand.Add(card);
            catCards.Remove(card);
        }

        Console.WriteLine($"{Name}, choose 10 total resources to start: (Food, Collections, and Cards in Hand)");
        AdjustStartingResources();
        UpdateUnplacedCards();
    }

    private void AdjustStartingResources()
{
    while (true)
    {
        Console.WriteLine($"Current Resources - Food: {FoodCount}, Collections: {Collections}, Cards in Hand: {Hand.Count}");
        
        // Display cards in hand with details
        if (Hand.Count > 0)
        {
            Console.WriteLine("Cards in Hand:");
            foreach (var card in Hand)
            {
                Console.WriteLine($"Card ID: {card.ID}, Name: {card.Name}, Food Requirement: {card.FoodRequirement}");
            }
        }

        Console.WriteLine("Choose an adjustment:");
        Console.WriteLine("0 - Remove Food");
        
        Console.WriteLine("1 - Remove Collection");
        
        Console.WriteLine("2 - Remove Card");
        
        Console.WriteLine("3 - Add Food");
        
        Console.WriteLine("4 - Add Collection");

        Console.WriteLine("--------------");
        int adjustment = int.Parse(Console.ReadLine());
        switch (adjustment)
        {
            case 0:
                if (FoodCount > 0)
                {
                    FoodCount--;
                    Console.WriteLine("Removed 1 Food.");
                }
                else
                {
                    Console.WriteLine("No food to remove.");
                }
                break;
            case 1:
                if (Collections > 0)
                {
                    Collections--;
                    Console.WriteLine("Removed 1 Collection.");
                }
                else
                {
                    Console.WriteLine("No collections to remove.");
                }
                break;
            case 2:
                if (Hand.Count > 0)
                {
                    Console.WriteLine("Enter the ID of the card you want to remove:");
                    int cardId = int.Parse(Console.ReadLine());
                    var cardToRemove = Hand.FirstOrDefault(c => c.ID == cardId);
                    if (cardToRemove != null)
                    {
                        Hand.Remove(cardToRemove);
                        Console.WriteLine($"Removed Card ID: {cardId}, Name: {cardToRemove.Name}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid Card ID. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("No cards to remove.");
                }
                break;
            case 3:
                if (FoodCount + Collections + Hand.Count < 10)
                {
                    FoodCount++;
                    Console.WriteLine("Added 1 Food.");
                }
                else
                {
                    Console.WriteLine("Cannot exceed a total of 10 resources.");
                }
                break;
            case 4:
                if (FoodCount + Collections + Hand.Count < 10)
                {
                    Collections++;
                    Console.WriteLine("Added 1 Collection.");
                }
                else
                {
                    Console.WriteLine("Cannot exceed a total of 10 resources.");
                }
                break;
            default:
                Console.WriteLine("Invalid choice. Try again.");
                break;
        }

        if (FoodCount + Collections + Hand.Count == 10)
        {
            Console.WriteLine($"Final Resources - Food: {FoodCount}, Collections: {Collections}, Cards in Hand: {Hand.Count}");
            Console.WriteLine("Your resources now sum to 10. Are you happy with this allocation? (yes/no)");
            string confirmation = Console.ReadLine().ToLower();
            if (confirmation == "yes")
            {
                break;
            }
            else
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("Let's adjust your resources again.");
                Console.WriteLine("-------------------------");
            }
        }
        else
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("Your total resources must sum to 10. Please adjust.");
            Console.WriteLine("-------------------------");
            
        }
    }
}


    public void DisplayStatus()
    {
        Console.WriteLine($"Player: {Name}, Score: {Score}, Food: {FoodCount}, Collections: {Collections}, Cards in Hand: {Hand.Count}, Unplaced Cards: {UnplacedCards}");
    }

    public void TakeAction(Terrain[] terrains, List<Card> catCards)
    {
        DisplayStatus();
        Console.WriteLine($"{Name}, choose an action:");
        Console.WriteLine("1 - Place a card in a terrain");
        Console.WriteLine("2 - Collect food");
        Console.WriteLine("3 - Draw a card");
        
        if (Hand.Count > 0)
    {
        Console.WriteLine("Cards in Hand:");
        foreach (var card in Hand)
        {
            Console.WriteLine($"Card ID: {card.ID}, Name: {card.Name}, Food Requirement: {card.FoodRequirement}");
        }
    }
    else
    {
        Console.WriteLine("No cards in hand.");
    }

        int action = int.Parse(Console.ReadLine());
        switch (action)
        {
            case 1:
                PlaceCardInTerrain(terrains);
                break;
            case 2:
                CollectFood();
                break;
            case 3:
                DrawCard(catCards);
                break;
            default:
                Console.WriteLine("Invalid action. Try again.");
                break;
        }
    }

    private void PlaceCardInTerrain(Terrain[] terrains)
{
    if (Hand.Count == 0)
    {
        Console.WriteLine("No cards in hand to place.");
        return;
    }

    // Display cards in hand
    Console.WriteLine("Cards in Hand:");
    foreach (var card in Hand)
    {
        Console.WriteLine($"Card ID: {card.ID}, Name: {card.Name}, Food Requirement: {card.FoodRequirement}");
    }

    Console.WriteLine("Enter the ID of the card you want to place:");
    int cardId = int.Parse(Console.ReadLine());
    var cardToPlace = Hand.FirstOrDefault(c => c.ID == cardId);

    if (cardToPlace == null)
    {
        Console.WriteLine("Invalid Card ID.");
        return;
    }

    Console.WriteLine("Choose a terrain to place the card (0-4):");
    int terrainIndex = int.Parse(Console.ReadLine());

    if (terrainIndex < 0 || terrainIndex >= terrains.Length)
    {
        Console.WriteLine("Invalid terrain choice.");
        return;
    }

    var terrain = terrains[terrainIndex];
    if (!terrain.AddCard(cardToPlace))
    {
        Console.WriteLine("Terrain is full or does not meet the card's requirements.");
        return;
    }

    Hand.Remove(cardToPlace);
    TerrainCards[terrainIndex]++;
    CardsPlaced++;
    UpdateUnplacedCards();
    Console.WriteLine($"Placed Card ID: {cardId}, Name: {cardToPlace.Name} in Terrain {terrainIndex}.");
}


    private void CollectFood()
    {
        FoodCount += 2;
        Console.WriteLine("Collected 2 Food.");
    }

    private void DrawCard(List<Card> catCards)
    {
        if (catCards.Count == 0)
        {
            Console.WriteLine("No more cards to draw.");
            return;
        }

        Random rnd = new Random();
        var card = catCards[rnd.Next(catCards.Count)];
        Hand.Add(card);
        catCards.Remove(card);

        Console.WriteLine($"Drew Card ID: {card.ID}, Name: {card.Name}, Food Requirement: {card.FoodRequirement}");
        UpdateUnplacedCards();
    }

    public int GetCardsInTerrain(int terrainIndex)
    {
        return TerrainCards.ContainsKey(terrainIndex) ? TerrainCards[terrainIndex] : 0;
    }

    public void AddScore(int points)
    {
        Score += points;
    }

    private void UpdateUnplacedCards()
    {
        UnplacedCards = Hand.Count;
    }
}

class Card
{
    public string Name { get; private set; }
    public int ID { get; private set; }
    public int FoodRequirement { get; private set; }

    public Card(string name, int id, int foodRequirement)
    {
        Name = name;
        ID = id;
        FoodRequirement = foodRequirement;
    }
}

class Terrain
{
    private int Slots;
    private int CollectionRequirement;
    private List<Card> PlacedCards;

    public Terrain(int slots, int collectionRequirement)
    {
        Slots = slots;
        CollectionRequirement = collectionRequirement;
        PlacedCards = new List<Card>();
    }

    public bool AddCard(Card card)
    {
        if (PlacedCards.Count >= Slots)
            return false;

        if (card.FoodRequirement > CollectionRequirement)
            return false;

        PlacedCards.Add(card);
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter number of players (2, 3 or 4):");
        int playerCount = int.Parse(Console.ReadLine());
        if (playerCount < 2 || playerCount > 4)
        {
            Console.WriteLine("Invalid number of players. Defaulting to 1 for test.");
            playerCount = 1;
        }

        CatGame game = new CatGame(playerCount);
        game.StartGame();
    }
}
