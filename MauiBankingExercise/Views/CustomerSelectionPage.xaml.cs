using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerSelectionPage : ContentPage
{
    public CustomerSelectionPage(CustomerSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}