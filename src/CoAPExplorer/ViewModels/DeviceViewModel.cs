using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CoAPExplorer.Models;
using CoAPNet;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.ViewModels
{
    public class DeviceViewModel : ReactiveObject, IRoutableViewModel, ISupportsNavigatation
    {
        // CoAP Related Properties

        public Device Device { get; }

        public ICoapEndpoint Endpoint => Device.Endpoint;

        public string Name => Device.Name;

        public string Address => Device.Address;

        public DateTime LastSeen => Device.LastSeen;

        private Message _message;
        public Message Message
        {
            get => _message;
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

        public ObservableCollection<Message> RecentMessages { get; } = new ObservableCollection<Message>();

        public ReactiveCommand OpenCommand { get; }

        #region ViewModel Related Propertied


        private string _urlPathSegment;
        public string UrlPathSegment => _urlPathSegment ?? (_urlPathSegment = Endpoint.BaseUri.ToString());

        public IScreen HostScreen { get; private set; }

        public INavigationViewModel Navigation { get; }


        #endregion

        public DeviceViewModel(Device device = null, IScreen hostScreen = null)
        {
            HostScreen = hostScreen;
            Device = device ?? new Device();

            Message = new Message();

            Navigation = new DeviceNavigationViewModel(this);

            OpenCommand = ReactiveCommand.Create(() => hostScreen.Router.Navigate.Execute(this).Subscribe());
        }
    }
}
