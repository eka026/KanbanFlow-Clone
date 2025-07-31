using System.Net.Http.Json;
using KanbanFlow.Client.Dtos;

public class KanbanApiService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiUrl;

    public KanbanApiService(string? apiUrl)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
    }

    public async Task<List<TaskItemDto>?> GetTasksAsync()
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        try
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItemDto>>($"{_apiUrl}/api/tasks");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching tasks: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskItemDto?> CreateTaskAsync(CreateTaskItemDto newTask)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/tasks", newTask);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskItemDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while creating the task: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ColumnDto>?> GetColumnsAsync()
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ColumnDto>>($"{_apiUrl}/api/columns");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching columns: {ex.Message}");
            return null;
        }
    }
}
