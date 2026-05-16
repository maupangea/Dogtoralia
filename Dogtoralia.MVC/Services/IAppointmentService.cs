using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;

namespace Dogtoralia.MVC.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync(int? clinicId = null, AppointmentStatus? status = null);
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<AppointmentDto?> CreateAsync(AppointmentWriteDto dto);
    Task<bool> UpdateAsync(int id, AppointmentWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
