using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiBankingExercise.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _title = string.Empty;
    }
}