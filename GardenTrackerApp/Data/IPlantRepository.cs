using GardenTrackerApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPlantRepository
{
    Task<List<Plant>> GetAllAsync();
    Task<Plant?> GetByIdAsync(int id);
    Task AddAsync(Plant plant);
    Task UpdateAsync(Plant plant);
    Task DeleteAsync(int id);
}
