using CoAPExplorer.Database;
using CoAPExplorer.Extensions;
using CoAPNet;
using CoAPNet.Options;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CoAPExplorer.Models
{
    public class Message
    {
        private byte[] _token;
        private byte[] _payload;
        private Stream _payloadStream;

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public int MessageId { get; set; }


        [NotMapped]
        public byte[] Token
        {
            get => _token;
            set
            {
                if ((value?.Length ?? 0) > 8)
                    throw new ArgumentException($"{nameof(Token)} may have no moer than 8 bytes");
                _token = value;
            }
        }

        [NotMapped]
        public CoapMessageCode Code { get; set; } = CoapMessageCode.None;

        public string Url { get; set; } = string.Empty;

        [NotMapped]
        public IList<CoapOption> Options { get; set; } = new List<CoapOption>();

        [NotMapped]
        public ContentFormatType ContentFormat { get; set; } = null;

        public byte[] Payload
        {
            get => _payload;
            set
            {
                if (_payloadStream != null)
                    throw new InvalidOperationException($"Please set {nameof(PayloadStream)} to null before assigning to {nameof(Payload)}");
                _payload = value;
            }
        }

        [NotMapped]
        public Stream PayloadStream
        {
            get => _payloadStream;
            set
            {
                if (_payload != null)
                    throw new InvalidOperationException($"Please set {nameof(Payload)} to null before assigning to {nameof(PayloadStream)}");
                _payloadStream = value;
            }
        }

        [Column(nameof(Code))]
        public string _dbCode
        {
            get => Code?.ToString() ?? "0.00";
            set
            {
                var p = value.Split('.');
                Code = new CoapMessageCode(int.Parse(p[0]), int.Parse(p[1]));
            }
        }

        [Column(nameof(ContentFormat))]
        public string _dbContentFormat
        {
            get => ContentFormat != null ? $"{ContentFormat.Value} - {ContentFormat}" : null;
            set
            {
                if (value == null)
                {
                    ContentFormat = null;
                    return;
                }

                var p = value.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                ContentFormat = new ContentFormatType(int.Parse(p[0]), p[1]);
            }
        }

        [Column(nameof(Options))]
        public string _dbOptions
        {
            get => Newtonsoft.Json.JsonConvert.SerializeObject(Options, new CoapOptionConverter());
            set
            {
                var options = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<CoapOption>>(value, new CoapOptionConverter());
                Options = new ObservableCollection<CoapOption>(options);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Message message)
            {
                if (message.Url != Url)
                    return false;
                if (message.Code != Code)
                    return false;
                if (message.Options.Any(o => !Options.Contains(o)))
                    return false;
                if (message.Payload != Payload)
                    return false;
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            // Well, there aren't any immutable properties... so this will do.
            return 12425;
        }

        public override string ToString()
        {
            return this.ToCoapMessage().ToString();
        }

        public Message Clone()
        {
            return new Message
            {
                MessageId = MessageId,
                Token = Token?.Clone() as byte[],
                Url = Url?.Clone() as string,
                Code = Code,
                ContentFormat = ContentFormat,
                Options = new List<CoapOption>(Options),
                Payload = Payload?.Clone() as byte[],
            };
        }
    }
}
