using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerDashboardPage : ContentPage
{
    public CustomerDashboardPage(CustomerDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}