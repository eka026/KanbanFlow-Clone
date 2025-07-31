using KanbanFlow.Client.Dtos;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string? apiUrl = configuration["ApiUrl"];

if (string.IsNullOrEmpty(apiUrl))
{
    Console.WriteLine("API URL is not configured in appsettings.json. Please add it and try again.");
    return;
}

Console.WriteLine("KanbanFlow Client");
Console.WriteLine("-----------------");

var apiService = new KanbanApiService(apiUrl);

while (true)
{
    Console.WriteLine("\nChoose an option:");
    Console.WriteLine("1. List all tasks");
    Console.WriteLine("2. Create a new task");
    Console.WriteLine("3. Exit");
    Console.Write("Enter your choice: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await ListAllTasks();
            break;
        case "2":
            await CreateNewTask();
            break;
        case "3":
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}

async Task ListAllTasks()
{
    Console.WriteLine("\nFetching tasks...");
    var tasks = await apiService.GetTasksAsync();

    if (tasks == null || !tasks.Any())
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    foreach (var task in tasks)
    {
        Console.WriteLine($" - ID: {task.Id}, Title: {task.Title}, Status: {task.Status}");
    }
}

async Task CreateNewTask()
{
    Console.WriteLine("\nFetching available columns...");
    var columns = await apiService.GetColumnsAsync();

    if (columns == null || !columns.Any())
    {
        Console.WriteLine("Could not fetch the columns, or no column exists. Please add columns via the API.");
        return;
    }

    Console.WriteLine("Please choose a column for the new task:");
    for (int i = 0; i < columns.Count; i++)
    {
        Console.WriteLine($" {i + 1}. {columns[i].Name}");
    }

    int chosenColumnId;
    while (true)
    {
        Console.Write("Enter the number of the column: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= columns.Count)
        {
            chosenColumnId = columns[choice - 1].Id;
            break;
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter a number from the list.");
        }
    }

    Console.Write("Enter task title: ");
    var title = Console.ReadLine();

    Console.Write("Enter task description: ");
    var description = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    var newTask = new CreateTaskItemDto(title, description, chosenColumnId);

    var createdTask = await apiService.CreateTaskAsync(newTask);

    if (createdTask != null)
    {
        Console.WriteLine($"Succesfully created task with ID: {createdTask.Id}");
    }
    else
    {
        Console.WriteLine("Failed to create task.");
    }
}
