using Google.Apis.Tasks.v1.Data;
using System.ComponentModel.DataAnnotations;

public class TodoList
{
    public string Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
    public List<Todo> Tasks { get; set; } = new List<Todo>();

    public static TodoList FromGoogleTaskList(TaskList taskList)
    {
        return new()
        {
            Id = taskList.Id,
            Title = taskList.Title,
        };
    }

    public static TaskList ToGoogleTaskList(TodoList list)
    {
        return new()
        {
            Title = list.Title,
        };
    }
}
