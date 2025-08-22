using MauiBankingExercise.Views;

namespace MauiBankingExercise
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("CustomerDashboard", typeof(CustomerDashboardPage));
            Routing.RegisterRoute("TransactionPage", typeof(TransactionPage));
        }
    }
}