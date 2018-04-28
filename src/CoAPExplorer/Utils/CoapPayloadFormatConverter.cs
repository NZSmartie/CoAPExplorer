using CoAPExplorer.Models;
using CoAPNet.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPExplorer.Utils
{
    public class CoapPayloadFormater
    {
        public static string Format(byte[] payload, ContentFormatType contentFormat)
        {
            if (payload == null || contentFormat == null)
                return string.Empty;

            if(payload.Length == 0)
                return string.Empty;

            try
            {
                if (contentFormat.Value == ContentFormatType.ApplicationJson.Value)
                    return JToken.Parse(Encoding.UTF8.GetString(payload)).ToString(Newtonsoft.Json.Formatting.Indented);

                // TODO: Don't simply replace all commas with new line. Need to be context aware to ensure we're not splitting a link format in two.
                if (contentFormat.Value == ContentFormatType.ApplicationLinkFormat.Value)
                    return Encoding.UTF8.GetString(payload).Replace(",", ",\r\n");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
#if DEBUG
                return ex.ToString();
#endif
            }

            return string.Empty;
        }

        public static byte[] RemoveFormat(string payload, ContentFormatType contentFormat)
        {
            if (string.IsNullOrEmpty(payload) || contentFormat == null)
                return null;

            try
            {

            if (contentFormat.Value == ContentFormatType.ApplicationJson.Value)
                return Encoding.UTF8.GetBytes(JToken.Parse(payload).ToString());

                if (contentFormat.Value == ContentFormatType.ApplicationLinkFormat.Value)
                    return Encoding.UTF8.GetBytes(payload.Replace("\n,",",").Replace("\r",""));
            }
            catch (JsonReaderException ex)
            {
                throw new FormattedTextException(ex.Message, ex.LineNumber, ex.LinePosition, ex);
            }

            return null;
        }
    }
}
