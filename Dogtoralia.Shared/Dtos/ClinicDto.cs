namespace Dogtoralia.Shared.Dtos;

public class ClinicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int SpecialityId { get; set; }
    public string SpecialityName { get; set; } = string.Empty;
    public int VeterinarianCount { get; set; }
}
