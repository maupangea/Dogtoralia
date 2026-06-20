using Dogtoralia.Maui.Views;

namespace Dogtoralia.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ClinicDetailPage), typeof(ClinicDetailPage));
            Routing.RegisterRoute(nameof(VeterinarianDetailPage), typeof(VeterinarianDetailPage));
            Routing.RegisterRoute(nameof(PetDetailPage), typeof(PetDetailPage));
            Routing.RegisterRoute(nameof(PetEditPage), typeof(PetEditPage));
        }
    }
}
