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
using CoAPNet.Options;
using CoAPExplorer.WPF.Services;
using ICSharpCode.AvalonEdit.Document;

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
        private readonly AvalonEditTextMarkerService _formattedTextMarkerService;

        public MessageRequestView()
        {
            InitializeComponent();

            _formattedTextMarkerService = new AvalonEditTextMarkerService(FormattedTextEditor);

            var textView = FormattedTextEditor.TextArea.TextView;
            textView.BackgroundRenderers.Add(_formattedTextMarkerService);
            textView.LineTransformers.Add(_formattedTextMarkerService);
            textView.Services.AddService(typeof(AvalonEditTextMarkerService), _formattedTextMarkerService);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .Subscribe(NewViewModel =>
                    {
                        _viewModelDisposables?.Dispose();
                        _viewModelDisposables = new CompositeDisposable();

                        _formattedTextMarkerService.Clear();

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

                        this.Bind(NewViewModel, vm => vm.Options, v => v.OptionsList.Options)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.Payload, v => v.MessageTextBox.Text)
                            .DisposeWith(_viewModelDisposables);

                        MessageTextBox.Events()
                                      .LostKeyboardFocus
                                      .Select(_ => MessageViewModel.UpdatePayloadSource.Text)
                                      .InvokeCommand(NewViewModel.UpdatePayloads)
                                      .DisposeWith(_viewModelDisposables);

                        this.WhenAnyValue(v => v.DisplayUnicode.IsSelected)
                            .InvokeCommand(NewViewModel, vm => vm.EscapePayload)
                            .DisposeWith(_viewModelDisposables);

                        this.Bind(NewViewModel, vm => vm.FormattedPayload, v=> v.FormattedTextEditor.Document.Text)
                            .DisposeWith(_viewModelDisposables);

                        FormattedTextEditor.Events()
                                           .LostKeyboardFocus
                                           .Select(_ => MessageViewModel.UpdatePayloadSource.Formatted)
                                           .InvokeCommand(NewViewModel.UpdatePayloads)
                                           .DisposeWith(_viewModelDisposables);

                        NewViewModel.WhenAnyValue(vm => vm.ContentFormat)
                                    .Select(cf => CoapFormatHighlightingManager.Default.GetDefinition(cf))
                                    .Subscribe(d => FormattedTextEditor.SyntaxHighlighting = d)
                                    .DisposeWith(_viewModelDisposables);

                        var formattedTextMarkers = NewViewModel.FormattedPayloadErrors.CreateDerivedCollection(ex =>
                                    {
                                        var marker = AvalonEditTextMarkerService.TextMarker.Create(FormattedTextEditor, ex.Offset, ex.Line);
                                        marker.ToolTip = ex.Message;
                                        marker.MarkerColor = ((TryFindResource("SecondaryAccentBrush") as SolidColorBrush)?.Color) ?? Colors.Magenta;
                                        return marker;
                                    })
                                    .DisposeWith(_viewModelDisposables);

                        formattedTextMarkers.ItemsAdded
                                            .Subscribe(m => _formattedTextMarkerService.Add(m))
                                            .DisposeWith(_viewModelDisposables);

                        formattedTextMarkers.ShouldReset
                                            .Subscribe(_ =>
                                            {
                                                _formattedTextMarkerService.Clear();
                                                foreach (var m in formattedTextMarkers)
                                                    _formattedTextMarkerService.Add(m);
                                            })
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
