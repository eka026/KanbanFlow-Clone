using System.Net.Http.Json;
using KanbanFlow.Core;

public class KanbanApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public KanbanApiService(string apiUrl)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
    }

    public async Task<List<TaskItem>?> GetTasksAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItem>>($"{_apiUrl}/api/tasks");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching tasks: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskItem?> CreateTaskAsync(TaskItem newTask)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/tasks", newTask);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskItem>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while creating the task: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Column>?> GetColumnsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Column>>($"{_apiUrl}/api/columns");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching columns: {ex.Message}");
            return null;
        }
    }
}