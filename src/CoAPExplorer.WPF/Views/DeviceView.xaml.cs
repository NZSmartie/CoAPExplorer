using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Converters;
using CoAPNet;
using CoAPNet.Options;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeviceView.xaml
    /// </summary>
    public partial class DeviceView : UserControl, IViewFor<DeviceViewModel>
    {
        private static readonly BooleanToVisibilityConverter _visibilityConverter = new BooleanToVisibilityConverter();

        public DeviceView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.Message, v => v.Url.SelectedItem,
                        this.WhenAnyValue(v => v.Url.SelectedItem).Where(i => i != null))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.RecentMessages, v => v.Url.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.SendCommand, v => v.SendButton, vm => vm.Message)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.StopSendingCommand, v => v.StopButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.IsSending,
                        v => v.StopButton.Visibility,
                        x => _visibilityConverter.Convert(x, typeof(Visibility), null, CultureInfo.CurrentCulture))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.IsSending,
                        v => v.SendButton.Visibility,
                        x => _visibilityConverter.Convert(!x, typeof(Visibility), null, CultureInfo.CurrentCulture))
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.MessageViewModel, v => v.MessageRequest.ViewModel)
                    .DisposeWith(disposables);

                ViewModel.SendCommand
                         .Subscribe(response =>
                         {
                             MessageResponse.ViewModel = new MessageViewModel(response);
                             ReponseTab.Focus();
                         })
                         .DisposeWith(disposables);

                Url.Events().KeyUp.Where(k => k.Key == Key.Enter).Select(k => { k.Handled = true; return false; })
                    .Merge(Url.Events().LostKeyboardFocus.Select(_ => false))
                    .Subscribe(_ => CreateMessage());

            });
        }

        private void CreateMessage()
        {
            if (ViewModel == null)
                return;

            if(Url.SelectedItem == null)
            {
                var message = ViewModel.Message.Clone();
                message.Url = Url.Text;

                ViewModel.Message = message;
            }
        }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(DeviceViewModel), typeof(DeviceView), new PropertyMetadata(null));

        public DeviceViewModel ViewModel { get => GetValue(ViewModelProperty) as DeviceViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as DeviceViewModel; }
    }
}
