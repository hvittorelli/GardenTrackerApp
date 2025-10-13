using GardenTrackerApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Threading;

public class JsonPlantRepository : IPlantRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

    public JsonPlantRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "gardenSeedData.json");

        // If file missing create initial seed (optional)
        if (!File.Exists(_filePath))
        {
            var seed = GetDefaultSeed();
            File.WriteAllText(_filePath, JsonSerializer.Serialize(seed, _jsonOptions));
        }
    }

    private List<Plant> GetDefaultSeed() => new List<Plant>
    {
        new Plant { Id = 1, Name = "Basil", Type = "Herb", WaterFrequency = "Daily", Notes = "Pinch flowers to encourage leaves" },
        new Plant { Id = 2, Name = "Spider Plant", Type = "Houseplant", WaterFrequency = "Daily", Notes = "Likes humidity" },
        new Plant { Id = 3, Name = "Tomato", Type = "Vegetable", WaterFrequency = "Every other", Notes = "Stake early" }
    };

    private async Task<List<Plant>> ReadAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_filePath)) return new List<Plant>();
            using var stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<Plant>>(stream) ?? new List<Plant>();
        }
        finally { _semaphore.Release(); }
    }

    private async Task WriteAsync(List<Plant> data)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, data, _jsonOptions);
        }
        finally { _semaphore.Release(); }
    }

    public async Task<List<Plant>> GetAllAsync() => await ReadAsync();

    public async Task<Plant?> GetByIdAsync(int id) => (await ReadAsync()).FirstOrDefault(p => p.Id == id);

    public async Task AddAsync(Plant plant)
    {
        var data = await ReadAsync();
        plant.Id = data.Any() ? data.Max(p => p.Id) + 1 : 1;
        data.Add(plant);
        await WriteAsync(data);
    }

    public async Task UpdateAsync(Plant plant)
    {
        var data = await ReadAsync();
        var idx = data.FindIndex(p => p.Id == plant.Id);
        if (idx == -1) throw new KeyNotFoundException($"Plant {plant.Id} not found");
        data[idx] = plant;
        await WriteAsync(data);
    }

    public async Task DeleteAsync(int id)
    {
        var data = await ReadAsync();
        var removed = data.RemoveAll(p => p.Id == id);
        if (removed > 0) await WriteAsync(data);
    }
}
