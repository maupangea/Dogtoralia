using System.ComponentModel.DataAnnotations;

namespace Dogtoralia.Shared.Dtos;

public class SpecialityWriteDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
