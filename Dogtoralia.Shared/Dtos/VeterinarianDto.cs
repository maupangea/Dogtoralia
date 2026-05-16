namespace Dogtoralia.Shared.Dtos;

public class VeterinarianDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
}
