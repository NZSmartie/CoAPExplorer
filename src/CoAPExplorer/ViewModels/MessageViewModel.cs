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
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class MessageViewModel : ReactiveObject, ISupportsActivation
    {
        private readonly Message _message;
        private bool _isPayloadEscaped;
        private string _payload = string.Empty;
        private string _formattedPayload = string.Empty;
        private bool _autoIncrement = true;

        public Message Message => _message;

        public bool AutoIncrement { get => _autoIncrement; set => this.RaiseAndSetIfChanged(ref _autoIncrement ,value); }

        public enum UpdatePayloadSource
        {
            Original,
            Text,
            Formatted,
            Binary
        }

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
            get => _message.Code;
            set
            {
                if (_message.Code == value)
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
                    _message.Options.Add(item);

            if (e.OldItems != null)
                foreach (var item in e.OldItems.Cast<CoapOption>())
                    _message.Options.Remove(item);
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

        public bool IsPayloadEscaped { get => _isPayloadEscaped; set => this.RaiseAndSetIfChanged(ref _isPayloadEscaped, value); }

        public string Payload { get => _payload; set => this.RaiseAndSetIfChanged(ref _payload, value); }

        public string FormattedPayload { get => _formattedPayload; set => this.RaiseAndSetIfChanged(ref _formattedPayload, value); }

        public ObservableCollection<FormattedTextException> FormattedPayloadErrors { get; }
            = new ObservableCollection<FormattedTextException>();

        public ReactiveCommand<UpdatePayloadSource, Unit> UpdatePayloads { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public MessageViewModel(Message message = null)
        {
            _message = message ?? new Message();

            Options = new ObservableCollection<CoapOption>(_message.Options);

            // Initial setup of Text and Formatted Text
            UpdatePayloads = ReactiveCommand.Create<UpdatePayloadSource>(source =>
            {
               if (source != UpdatePayloadSource.Text)
                   Payload = _message.Payload != null
                       ? Encoding.UTF8.GetString(_message.Payload)
                       : string.Empty;

               if (source != UpdatePayloadSource.Formatted)
                   FormattedPayload = _message.Payload != null
                       ? CoapPayloadFormater.Format(_message.Payload, _message.ContentFormat)
                       : string.Empty;
            });

            // Command to swap between Escapped and non-escapped payloads
            EscapePayload = ReactiveCommand.Create<bool, bool>(escape =>
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

            // Initial setup of Text and Formatted Text
            Observable.Return(UpdatePayloadSource.Original).InvokeCommand(UpdatePayloads);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.WhenAnyValue(vm => vm.Payload, vm => vm.IsPayloadEscaped)
                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                    .Subscribe(l =>
                    {
                        (var payload, var isEscaped) = l;

                        if (isEscaped)
                            payload = StringEscape.Unescape(payload);

                        _message.Payload = Encoding.UTF8.GetBytes(payload);
                    })
                    .DisposeWith(disposables);

                this.WhenAnyValue(vm => vm.FormattedPayload, vm => vm.ContentFormat)
                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                    .Subscribe(t =>
                    {
                        (var payload, var contentFormat) = t;
                        try
                        {
                            FormattedPayloadErrors.Clear();
                            var bytes = CoapPayloadFormater.RemoveFormat(payload, contentFormat);

                            if (bytes != null)
                                _message.Payload = bytes;
                        }
                        catch (FormattedTextException ex)
                        {
                            FormattedPayloadErrors.Add(ex);
                        }
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
