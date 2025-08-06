using System.Net.Http.Json;
using System.Net.Http.Headers;
using KanbanFlow.Client.Dtos;

public class KanbanApiService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiUrl;
    private string? _authToken;

    public KanbanApiService(HttpClient httpClient, string? apiUrl)
    {
        _httpClient = httpClient;
        _apiUrl = apiUrl;
    }

    // Authentication methods
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/auth/register", registerDto);
            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
            }
            
            return authResponse;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while registering: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/auth/login", loginDto);
            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
            }
            
            return authResponse;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while logging in: {ex.Message}");
            return null;
        }
    }

    public void Logout()
    {
        _authToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

    private void SetAuthToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<TaskItemDto>?> GetTasksAsync()
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        if (!IsAuthenticated) return null;
        
        try
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItemDto>>($"{_apiUrl}/api/v1.0/tasks");
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
        if (!IsAuthenticated) return null;
        
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/v1.0/tasks", newTask);
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
        if (!IsAuthenticated) return null;
        
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ColumnDto>>($"{_apiUrl}/api/v1.0/columns");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching columns: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ProjectDto>?> GetProjectsAsync()
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        if (!IsAuthenticated) return null;
        
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ProjectDto>>($"{_apiUrl}/api/v1.0/projects");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while fetching projects: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskItemDto?> UpdateTaskAsync(int taskId, CreateTaskItemDto updateTask)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return null;
        if (!IsAuthenticated) return null;
        
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/api/v1.0/tasks/{taskId}", updateTask);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskItemDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while updating the task: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return false;
        if (!IsAuthenticated) return false;
        
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/api/v1.0/tasks/{taskId}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while deleting the task: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ReorderTaskAsync(int taskId, int newPosition)
    {
        if (string.IsNullOrEmpty(_apiUrl)) return false;
        if (!IsAuthenticated) return false;
        
        try
        {
            var reorderDto = new { Position = newPosition };
            var response = await _httpClient.PatchAsJsonAsync($"{_apiUrl}/api/v1.0/tasks/{taskId}/reorder", reorderDto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while reordering the task: {ex.Message}");
            return false;
        }
    }
}
