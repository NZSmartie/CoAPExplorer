using CoAPNet;
using CoAPNet.Options;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CoAPExplorer.Models
{
    public class Message : ReactiveObject
    {
        private int _id;
        public int Id { get => _id; set => this.RaiseAndSetIfChanged(ref _id, value); }

        private byte[] _token;
        public byte[] Token
        {
            get => _token;
            set {
                if ((value?.Length ?? 0) > 8)
                    throw new ArgumentException($"{nameof(Token)} may have no moer than 8 bytes");
                this.RaiseAndSetIfChanged(ref _token, value);
            }
        }

        private CoapMessageCode _code;
        public CoapMessageCode Code { get => _code; set => this.RaiseAndSetIfChanged(ref _code, value); }

        private string _url;
        public string Url { get => _url; set => this.RaiseAndSetIfChanged(ref _url, value); }

        public ObservableCollection<CoAPNet.CoapOption> Options { get; set; } 
            = new ObservableCollection<CoapOption>();

        private ContentFormatType _contentFormat;
        public ContentFormatType ContentFormat { get => _contentFormat; set => this.RaiseAndSetIfChanged(ref _contentFormat, value); }

        private string _payload;
        public string Payload { get => _payload; set => this.RaiseAndSetIfChanged(ref _payload, value); }
    }
}
