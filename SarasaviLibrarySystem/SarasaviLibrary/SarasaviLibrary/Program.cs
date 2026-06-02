using SarasaviLibrary.Forms;

namespace SarasaviLibrary
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new DashboardForm());
        }
    }
}
