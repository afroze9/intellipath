using System.ComponentModel.DataAnnotations;

namespace IntelliPath.Desktop.ViewModels;

public class MemoryViewModel
{
    [Required]
    public string? Description { get; set; }

    [Required]
    public IEnumerable<TagViewModel> Tags { get; set; } = [];
}

public class TagViewModel
{
    [Required] public string Name { get; set; } = string.Empty;
}