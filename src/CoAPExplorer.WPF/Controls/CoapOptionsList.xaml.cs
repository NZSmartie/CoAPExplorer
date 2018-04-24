using CoAPNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoAPExplorer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CoapOptionsList.xaml
    /// </summary>
    public partial class CoapOptionsList : UserControl
    {
        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            nameof(Options), typeof(ObservableCollection<CoAPNet.CoapOption>), typeof(CoapOptionsList), new PropertyMetadata(null));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            nameof(IsReadOnly), typeof(bool), typeof(CoapOptionsList), new PropertyMetadata(false));

        public ObservableCollection<CoAPNet.CoapOption> Options
        {
            get => GetValue(OptionsProperty) as ObservableCollection<CoAPNet.CoapOption>;
            set => SetValue(OptionsProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public CoapOptionsList()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (Options is null)
                return;

            Options.Add(new CoAPNet.Options.Accept());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Options is null)
                return;

            var option = (sender as Control)?.DataContext as CoapOption;
            if (option == null)
                return;

            Options.Remove(option);
        }

        private void OptionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null)
                return;

            var option = (sender as Control)?.DataContext as CoapOption;
            if (option == null)
                return;

            var newOptionType = e.AddedItems.Cast<Tuple<string, Type>>().Single().Item2;
            if (option.GetType() == newOptionType)
                return;

            var newOption = Activator.CreateInstance(newOptionType) as CoapOption;

            var index = Options.IndexOf(option);
            Options.Remove(option);
            Options.Insert(index, newOption);
        }
    }
}
