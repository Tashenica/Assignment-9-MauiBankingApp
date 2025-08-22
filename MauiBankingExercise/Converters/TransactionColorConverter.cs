using System.Globalization;

namespace MauiBankingExercise.Converters
{
    public class TransactionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int transactionTypeId)
            {
                // 1 = Deposit (Green), 2 = Withdrawal (Red)
                return transactionTypeId == 1 ? Colors.Green : Colors.Red;
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
