using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Disposables;
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

using ReactiveUI;

using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Converters;

namespace CoAPExplorer.WPF.Views
{

    /// <summary>
    /// Interaction logic for MessageRequestView.xaml
    /// </summary>
    public partial class MessageRequestView : UserControl, IViewFor<MessageViewModel>
    {
        private CompositeDisposable _viewModelDisposables;

        public MessageRequestView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .Subscribe(NewViewModel =>
                    {
                        _viewModelDisposables?.Dispose();
                        _viewModelDisposables = new CompositeDisposable();

                        this.Bind(NewViewModel, vm => vm.MessageId, v => v.MessageIdTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.Code, v => v.MessageCodeComboBox.SelectedValue)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.ContentFormat, v => v.ContentTypeComboBox.SelectedValue)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.Payload, v => v.MessageTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        this.WhenAnyValue(v => v.DisplayUnicode.IsSelected)
                            .InvokeCommand(NewViewModel, vm => vm.EscapePayload)
                            .DisposeWith(_viewModelDisposables);
                    })
                    .DisposeWith(disposables);
            });
            
        }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MessageViewModel), typeof(MessageRequestView), new PropertyMetadata(null));

        public MessageViewModel ViewModel { get => GetValue(ViewModelProperty) as MessageViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as MessageViewModel; }

        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.MessageId = new Random().Next(ushort.MaxValue);
        }
    }
}
