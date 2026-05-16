namespace Dogtoralia.Shared.Dtos;

public class PetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PetOwnerId { get; set; }
    public string PetOwnerName { get; set; } = string.Empty;
    public string PetOwnerEmail { get; set; } = string.Empty;
    public string PetOwnerPhone { get; set; } = string.Empty;
}
