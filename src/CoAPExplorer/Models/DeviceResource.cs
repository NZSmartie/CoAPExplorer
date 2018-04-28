using CoAPNet.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoAPExplorer.Models
{
    public class DeviceResource
    {
        [Key]
        public int Id { get; set; }

        public Device Device { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public ContentFormatType ContentFormat { get; set; } = null;

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
    }
}