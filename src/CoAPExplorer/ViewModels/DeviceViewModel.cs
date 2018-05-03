using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CoAPExplorer.Database;
using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPNet;
using ReactiveUI;
using ReactiveUI.Routing;
using Splat;

namespace CoAPExplorer.ViewModels
{
    public class DeviceViewModel : ReactiveObject, ISupportsActivation
    {
        private readonly CoapExplorerContext _dbContext;

        private Message _message;
        private MessageViewModel _messageViewModel;
        private ObservableCollection<Message> _recentMessages;
        private CoapService _coapService;
        private ObservableAsPropertyHelper<bool> _isSending;

        public Device Device { get; }

        public IReactiveDerivedList<DeviceResource> Resources { get; }

        public ICoapEndpoint Endpoint => Device.Endpoint;

        public string Address => Device.Address;

        public MessageViewModel MessageViewModel { get => _messageViewModel; set => this.RaiseAndSetIfChanged(ref _messageViewModel, value); }

        public ReactiveCommand OpenCommand { get; }

        public bool IsSending => _isSending.Value;

        public ReactiveCommand<Message, Message> SendCommand { get; }

        public ReactiveCommand<Unit, Unit> StopSendingCommand { get; }

        public ReactiveCommand<Message, Message> DuplicateMessageCommand { get; }

        public CoapService CoapService { get => _coapService ?? (_coapService = new CoapService(Device.EndpointType)); }

        public IReactiveRouter Router { get; private set; }

        public DeviceNavigationViewModel Navigation { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

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

        public ObservableCollection<Message> RecentMessages
        {
            get
            {
                if (_recentMessages != null)
                    return _recentMessages;

                _recentMessages = new ObservableCollection<Message>(_dbContext?.RecentMessages ?? Enumerable.Empty<Message>());
                _recentMessages.CollectionChanged += RecentMessagesChanged;

                return _recentMessages;
            }
        }

        public DeviceViewModel(Device device = null, IReactiveRouter router = null)
        {
            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            Router = router;
            Device = device ?? new Device();

            Resources = Device.KnownResources.CreateDerivedCollection(x => x);

            Navigation = new DeviceNavigationViewModel(this);

            OpenCommand = ReactiveCommand.CreateFromObservable(() => router.Navigate(NavigationRequest.Forward(this)));

            SendCommand = ReactiveCommand.CreateFromObservable<Message, Message>(
                message =>
                {
                    var obs = CoapService.SendMessage(message, Device.Endpoint).TakeUntil(StopSendingCommand);

                    // Fire and forget
                    Task.Run(async () => await _dbContext?.SaveChangesAsync());

                    if (MessageViewModel.AutoIncrement)
                        MessageViewModel.MessageId++;

                    return obs;
                });

            StopSendingCommand = ReactiveCommand.Create(
                () => { }, SendCommand.IsExecuting);

            DuplicateMessageCommand = ReactiveCommand.Create<Message, Message>(message =>
            {
                var newMessage = message.Clone();
                Message = newMessage;
                return newMessage;
            });

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

                this.WhenAnyValue(vm => vm.Navigation.SelectedResource)
                    .Where(x => x != null)
                    .Subscribe(resource =>
                    {
                        Message = new Message
                        {
                            Code = CoapMessageCode.Get,
                            Url = resource.Url,
                            ContentFormat = resource.ContentFormat,
                        };
                    })
                    .DisposeWith(disposables);
            });

        }

        private async void RecentMessagesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_dbContext == null)
                return;

            // Break awaiy from the main trhead incase any db operation blocks
            await Task.Yield();

            if (e.NewItems != null)
                await _dbContext.RecentMessages.AddRangeAsync(e.NewItems.Cast<Message>().Where(m => m.Id == 0 && !string.IsNullOrWhiteSpace(m.Url)));
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
