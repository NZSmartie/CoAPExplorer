using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using CoAPExplorer.Database;
using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPNet;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.ViewModels
{
    public class DeviceViewModel : ReactiveObject, IRoutableViewModel, ISupportsActivation, ISupportsNavigatation
    {
        // CoAP Related Properties

        public Device Device { get; }

        private readonly CoapExplorerContext _dbContext;

        public ICoapEndpoint Endpoint => Device.Endpoint;

        public string Name
        {
            get => Device.Name;
            set
            {
                if (Device.Name == value)
                    return;
                Device.Name = value;
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public string Address => Device.Address;

        public DateTime LastSeen
        {
            get => Device.LastSeen;
            set
            {
                if (Device.LastSeen == value)
                    return;
                Device.LastSeen = value;
                this.RaisePropertyChanged(nameof(LastSeen));
            }
        }

        public bool IsFavourite
        {
            get => Device.IsFavourite;
            set
            {
                if (Device.IsFavourite == value)
                    return;
                Device.IsFavourite = value;
                this.RaisePropertyChanged(nameof(IsFavourite));
            }
        }

        private Message _message;
        public Message Message
        {
            get => _message ?? (_message = RecentMessages.FirstOrDefault() ?? new Message());
            set
            {
                if (_message == value)
                    return;

                if (RecentMessages.Contains(value))
                    RecentMessages.Move(RecentMessages.IndexOf(value), 0);
                else
                    RecentMessages.Insert(0, value);

                this.RaiseAndSetIfChanged(ref _message, value);
            }
        }

        public MessageViewModel _messageViewModel;
        public MessageViewModel MessageViewModel { get => _messageViewModel; set => this.RaiseAndSetIfChanged(ref _messageViewModel, value); }

        private ObservableCollection<Message> _recentMessages;
        public ObservableCollection<Message> RecentMessages 
            => _recentMessages ?? (_recentMessages = new ObservableCollection<Message>(_dbContext?.RecentMessages ?? Enumerable.Empty<Message>()));

        public ReactiveCommand OpenCommand { get; }

        ObservableAsPropertyHelper<bool> _isSending;
        public bool IsSending => _isSending.Value;

        public ReactiveCommand<Message, Message> SendCommand { get; }

        public ReactiveCommand<Unit, Unit> StopSendingCommand { get; }


        private CoapService _coapService;
        public CoapService CoapService { get => _coapService ?? (_coapService = new CoapService(Device.EndpointType)); }

        private string _someString;
        public string SomeString { get => _someString; set => this.RaiseAndSetIfChanged(ref _someString, value); }

        #region ViewModel Related Propertied


        private string _urlPathSegment;

        public string UrlPathSegment => _urlPathSegment ?? (_urlPathSegment = Endpoint.BaseUri.ToString());

        public IScreen HostScreen { get; private set; }

        public INavigationViewModel Navigation { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        #endregion

        public DeviceViewModel(Device device = null, IScreen hostScreen = null)
        {
            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            HostScreen = hostScreen;
            Device = device ?? new Device();

            Navigation = new DeviceNavigationViewModel(this);

            OpenCommand = ReactiveCommand.Create(() => hostScreen.Router.Navigate.Execute(this).Subscribe());

            SendCommand = ReactiveCommand.CreateFromObservable<Message, Message>(
                message =>
                {
                    var obs = CoapService.SendMessage(message, Device.Endpoint).TakeUntil(StopSendingCommand);

                    if (MessageViewModel.AutoIncrement)
                        MessageViewModel.MessageId++;

                    return obs;
                });

            StopSendingCommand = ReactiveCommand.Create(
                () => { }, SendCommand.IsExecuting);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.WhenAnyValue(vm => vm.Message)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(Message)))
                    .DisposeWith(disposables);

                _isSending = SendCommand.IsExecuting
                                        .ToProperty(this, x => x.IsSending, false)
                                        .DisposeWith(disposables);

                this.WhenAnyValue(vm => vm.Message)
                    .Select(message => new MessageViewModel(message))
                    .Subscribe(mvm => MessageViewModel = mvm)
                    .DisposeWith(disposables);
            });

        }


    }
}
