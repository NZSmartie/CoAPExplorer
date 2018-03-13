using CoAPNet.Options;
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

            try
            {
                if (contentFormat.Value == ContentFormatType.ApplicationJson.Value)
                    return JToken.Parse(Encoding.UTF8.GetString(payload)).ToString(Newtonsoft.Json.Formatting.Indented);
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

            if (contentFormat.Value == ContentFormatType.ApplicationJson.Value)
                return Encoding.UTF8.GetBytes(JToken.Parse(payload).ToString());

            return null;
        }
    }
}
