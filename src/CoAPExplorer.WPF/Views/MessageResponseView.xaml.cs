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
using System.Text.RegularExpressions;
using System.Linq;
using CoAPExplorer.WPF.Services;

namespace CoAPExplorer.WPF.Views
{

    /// <summary>
    /// Interaction logic for MessageResponseView.xaml
    /// </summary>
    public partial class MessageResponseView : UserControl, IViewFor<MessageViewModel>
    {
        private CompositeDisposable _viewModelDisposables;

        private static readonly HextoAsciiConverter _hextoAsciiConverter = new HextoAsciiConverter();

        public MessageResponseView()
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

                        this.OneWayBind(NewViewModel, vm => vm.MessageId, v => v.MessageIdTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        this.OneWayBind(NewViewModel, vm => vm.Token, v => v.MessageToken.Text,
                                x => _hextoAsciiConverter.Convert(x, typeof(string), 8, CultureInfo.CurrentCulture))
                            .DisposeWith(_viewModelDisposables);

                        this.OneWayBind(NewViewModel, vm => vm.Code, v => v.MessageCodeTextBox.Text,
                                x => Consts.MessageCodes.SingleOrDefault(c => c.Item2 == x)?.Item1 ?? x.ToString())
                            .DisposeWith(_viewModelDisposables);

                        this.OneWayBind(NewViewModel, vm => vm.ContentFormat, v => v.ContentTypeTextBox.Text,
                                x => Consts.ContentTypes.Single(c => c.Item2 == x).Item1)
                            .DisposeWith(_viewModelDisposables);

                        this.OneWayBind(NewViewModel, vm => vm.Payload, v => v.MessageTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        this.WhenAnyValue(v => v.DisplayUnicode.IsSelected)
                            .InvokeCommand(NewViewModel, vm => vm.EscapePayload)
                            .DisposeWith(_viewModelDisposables);

                        this.OneWayBind(NewViewModel, vm => vm.FormattedPayload, v => v.FormattedTextEditor.Text)
                            .DisposeWith(_viewModelDisposables);

                        NewViewModel.WhenAnyValue(vm => vm.ContentFormat)
                                    .Select(cf => CoapFormatHighlightingManager.Default.GetDefinition(cf))
                                    .Subscribe(d => FormattedTextEditor.SyntaxHighlighting = d)
                                    .DisposeWith(_viewModelDisposables);
                    })
                    .DisposeWith(disposables);
            });
            
        }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MessageViewModel), typeof(MessageResponseView), new PropertyMetadata(null));

        public MessageViewModel ViewModel { get => GetValue(ViewModelProperty) as MessageViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as MessageViewModel; }
    }
}
