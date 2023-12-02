using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

public class Todo
{
    public string Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
    public bool? IsComplete { get; set; }
    public bool? IsVisible { get; set; }
    public int Position { get; set; }

    public string ParentId { get; set; }


    public static Todo FromGoogleTask(Google.Apis.Tasks.v1.Data.Task task)
    {
        return new()
        {
            Id = task.Id,
            Title = task.Title,
            IsComplete = !task.Completed.IsNullOrEmpty(),
            IsVisible = !task.Hidden,
            Position = Int32.Parse(task.Position),
            ParentId = task.Parent,
        };
    }
}
