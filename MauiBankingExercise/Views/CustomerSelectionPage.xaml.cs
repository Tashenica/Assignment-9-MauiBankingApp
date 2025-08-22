using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerSelectionPage : ContentPage
{
    public CustomerSelectionPage(CustomerSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CustomerSelectionViewModel viewModel)
        {
            await viewModel.LoadCustomersAsync();
        }
    }
}