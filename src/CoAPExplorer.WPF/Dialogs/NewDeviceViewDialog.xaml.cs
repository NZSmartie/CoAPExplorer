using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;
using ReactiveUI;

using CoAPExplorer.ViewModels;
using CoAPExplorer.Services;

namespace CoAPExplorer.WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for NewDeviceViewDialog.xaml
    /// </summary>
    public partial class NewDeviceViewDialog : UserControl, IViewFor<NewDeviceViewModel>
    {
        public NewDeviceViewDialog()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel.AddDeviceCommand
                         .Subscribe(p => DialogHost.CloseDialogCommand.Execute(p, this))
                         .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddDeviceCommand, v => v.AddButton)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Address, v => v.AddressTextBox.Text)
                    .DisposeWith(disposables);
            });
        }

        public NewDeviceViewModel ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as NewDeviceViewModel; }
    }
}
