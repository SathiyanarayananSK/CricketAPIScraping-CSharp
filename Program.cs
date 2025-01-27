internal class Program
{
    // List of JSON endpoints to fetch data
    private static readonly List<string> JsonEndpoints = new List<string>
    {
        "scoreboard.json",
        "innings.json",
        "details.json",
        "players.json",
        "coaches.json",
        "currentbatsmen.json",
        "currentbowlers.json",
        "bowlingscorecards.json",
        "partnerships.json",
        "battingscorecards.json",
        "fallofwicket.json",
        "notes.json",
        "fullballbyball.json",
        "overbyover.json"
    };

    private static async Task Main(string[] args)
    {
        // URL and user key definition
        string baseUrl = "https://statsapi.foxsports.com.au/3.0/api/sports/cricket/matches/";
        string userKey = Environment.GetEnvironmentVariable("CricketApiFoxSports");

        // Unique identifier for the match
        string matchId = "BBL2024-250101";

        // Directory to store match data
        string dirName = "CricketData"; 

        // Create the directory structure for the match data
        string matchDirectory = CreateMatchDirectory(dataDirName: dirName, matchDir: matchId);

        // Fetch and save each JSON file
        foreach (string endpoint in JsonEndpoints)
        {
            try
            {
                // Build URL
                string url = $"{baseUrl}{matchId}/{endpoint}?userkey={userKey}";

                // Fetch JSON data from the API
                string jsonResponse = await GetDataFromAPIAsync(endpoint, url);

                // Save the JSON data to a file
                CreateJsonFile(endpoint, jsonResponse, matchDirectory);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during fetching or saving
                Console.WriteLine($"Error processing {endpoint}: {ex.Message}");
            }
        }
    }

    // Fetches JSON data from the API taking the endpoint and the url as inputs
    public static async Task<string> GetDataFromAPIAsync(string endpoint,string url)
    {

        using (var client = new HttpClient())
        {
            try
            {
                // Fetch the JSON data
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();

                return jsonResponse;
            }
            catch (HttpRequestException e)
            {
                // Handle HTTP request errors
                Console.WriteLine($"Failed to fetch {endpoint}: {e.Message}");
                throw; // Re-throw the exception to be handled in the calling method
            }
        }
    }

    // Creates the directory structure for storing match data
    public static string CreateMatchDirectory(string dataDirName, string matchDir)
    {
        // Get the project's root directory
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName!;

        // Create the CricketData directory if it doesn't exist
        string dataDirectory = Path.Combine(projectDirectory, dataDirName);
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        // Create the match-specific directory if it doesn't exist
        string matchDirectory = Path.Combine(dataDirectory, matchDir);
        if (!Directory.Exists(matchDirectory))
        {
            Directory.CreateDirectory(matchDirectory);
        }

        // Return the match directory path
        return matchDirectory;
    }

    // Saves JSON data to a file taking endpoint, json string and path of match directory as input
    public static void CreateJsonFile(string endpoint, string jsonResponse, string matchDirectory)
    {
        // Construct the file path
        string filePath = Path.Combine(matchDirectory, endpoint);

        // Write the JSON data to the file
        File.WriteAllText(filePath, jsonResponse);

        // Log the file creation
        Console.WriteLine($"Endpoint: {endpoint} Saved to: {filePath}");
    }
}