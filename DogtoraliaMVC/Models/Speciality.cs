using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Models;

public class Speciality
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
}
