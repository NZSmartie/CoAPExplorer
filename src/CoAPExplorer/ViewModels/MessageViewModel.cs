using CoAPExplorer.Database;
using CoAPExplorer.Extensions;
using CoAPExplorer.Models;
using CoAPExplorer.Utils;
using CoAPNet;
using CoAPNet.Options;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class MessageViewModel : ReactiveObject, ISupportsActivation
    {
        private Message _message;

        public int MessageId
        {
            get => _message.MessageId;
            set
            {

                if (_message.MessageId == value)
                    return;

                _message.MessageId = value;
                this.RaisePropertyChanged(nameof(MessageId));
            }
        }

        public byte[] Token
        {
            get => _message.Token;
            set
            {
                if (_message.Token == value)
                    return;
                _message.Token = value;
                this.RaisePropertyChanged(nameof(Token));
            }
        }

        public CoapMessageCode Code
        {
            get => _message.Code; set
            {
                if (_message.Code == Code)
                    return;
                _message.Code = value;
                this.RaisePropertyChanged(nameof(Code));
            }
        }

        public string Url
        {
            get => _message.Url; set
            {
                if (_message.Url == value)
                    return;
                _message.Url = value;
                this.RaisePropertyChanged(nameof(Url));
            }
        }

        private ObservableCollection<CoapOption> _options;
        private string _test;

        public ObservableCollection<CoapOption> Options
        {
            get => _options;
            set
            {
                if (_options == value)
                    return;

                if (_options != null)
                    _options.CollectionChanged -= OptionsCollectionChanged;

                _options = value;
                _message.Options = _options.ToList();

                _options.CollectionChanged += OptionsCollectionChanged;

                this.RaisePropertyChanged(nameof(Options));
            }
        }

        private void OptionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems.Cast<CoapOption>())
                    _options.Add(item);

            if (e.OldItems != null)
                foreach (var item in e.OldItems.Cast<CoapOption>())
                    _options.Remove(item);
        }

        public ContentFormatType ContentFormat
        {
            get => _message.ContentFormat;
            set
            {
                if (_message.ContentFormat == value)
                    return;

                _message.ContentFormat = value;
                this.RaisePropertyChanged(nameof(ContentFormat));
            }
        }

        public ReactiveCommand<bool, bool> EscapePayload { get; }

        private bool _isPayloadEscaped;
        private string _payload;

        public bool IsPayloadEscaped { get => _isPayloadEscaped; set => this.RaiseAndSetIfChanged(ref _isPayloadEscaped, value); }

        public string Payload { get => _payload; set => this.RaiseAndSetIfChanged(ref _payload, value); }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public MessageViewModel(Message message = null)
        {
            _message = message ?? new Message();

            Payload = _message.Payload != null
                ? Encoding.UTF8.GetString(_message.Payload)
                : string.Empty;

            EscapePayload = ReactiveCommand.Create<bool,bool>(escape =>
            {
                if (IsPayloadEscaped && !escape)
                {
                    Payload = StringEscape.Unescape(Payload); ;
                }
                else if (!IsPayloadEscaped && escape)
                {
                    Payload = StringEscape.Escape(Payload); ;
                }

                var changed = IsPayloadEscaped ^ escape;
                IsPayloadEscaped = escape;

                return changed;
            });

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.WhenAnyValue(vm => vm.Payload, vm => vm.IsPayloadEscaped)
                    .TakeLastBuffer(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                    .Subscribe(l =>
                    {
                        (var payload, var isEscaped) = l.Last();

                        if (isEscaped)
                            payload = StringEscape.Unescape(payload);

                        _message.Payload = Encoding.UTF8.GetBytes(payload);
                    })
                    .DisposeWith(disposables);
            });
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj is Message message)
        //    {
        //        if (message.Url != Url)
        //            return false;
        //        if (message.Code != Code)
        //            return false;
        //        if (message.Options.Any(o => !Options.Contains(o)))
        //            return false;
        //        if (message.Payload != Payload)
        //            return false;
        //        return true;
        //    }
        //    return base.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    // Well, there aren't any immutable properties... so this will do.
        //    return 32053;
        //}
    }
}
