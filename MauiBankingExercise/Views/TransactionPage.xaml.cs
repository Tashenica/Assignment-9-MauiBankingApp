using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class TransactionPage : ContentPage
{
    public TransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}