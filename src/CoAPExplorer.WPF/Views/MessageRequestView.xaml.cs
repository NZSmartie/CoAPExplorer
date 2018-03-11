﻿using System;
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
using System.Text.RegularExpressions;

namespace CoAPExplorer.WPF.Views
{

    /// <summary>
    /// Interaction logic for MessageRequestView.xaml
    /// </summary>
    public partial class MessageRequestView : UserControl, IViewFor<MessageViewModel>
    {
        private static readonly Regex regex = new Regex(@"^[a-fA-F0-9 ,x-]*$");
        private CompositeDisposable _viewModelDisposables;

        private static readonly HextoAsciiConverter _hextoAsciiConverter = new HextoAsciiConverter();

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

                        if (NewViewModel == null)
                            return;

                        this.Bind(NewViewModel, vm => vm.MessageId, v => v.MessageIdTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.Token, v => v.MessageToken.Text,
                                MessageToken.Events().LostFocus,
                                x => _hextoAsciiConverter.Convert(x, typeof(string), 8, CultureInfo.CurrentCulture),
                                x => _hextoAsciiConverter.ConvertBack(x, typeof(byte[]), 8, CultureInfo.CurrentCulture))
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.AutoIncrement, v => v.IncrementMessageIDCheckBox.IsChecked)
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

                        //PayloadHexEditor
                    })
                    .DisposeWith(disposables);

                MessageToken.Events().PreviewTextInput.Subscribe(e => e.Handled = !regex.IsMatch(e.Text))
                                     .DisposeWith(disposables);
            });
            
        }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MessageViewModel), typeof(MessageRequestView), new PropertyMetadata(null));

        public MessageViewModel ViewModel { get => GetValue(ViewModelProperty) as MessageViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as MessageViewModel; }

        private Random _random = new Random();

        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.MessageId = _random.Next(ushort.MaxValue);
        }

        private void MessageTokenRegen(object sender, MouseButtonEventArgs e)
        {
            var token = new byte[8];
            _random.NextBytes(token);
            ViewModel.Token = token;
        }
    }
}
