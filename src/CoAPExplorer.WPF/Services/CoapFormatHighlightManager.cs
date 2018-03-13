using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using CoAPNet.Options;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace CoAPExplorer.WPF.Services
{
    public class CoapFormatHighlightingManager : IHighlightingDefinitionReferenceResolver
    {
        public static readonly CoapFormatHighlightingManager Default
            = new CoapFormatHighlightingManager();

        private readonly Dictionary<ContentFormatType, Lazy<IHighlightingDefinition>> _registeredHighlighting 
            = new Dictionary<ContentFormatType, Lazy<IHighlightingDefinition>>();

        public CoapFormatHighlightingManager()
        {
            Register(ContentFormatType.ApplicationJson, "/Resources/JSONFormat.xml");
        }

        public void Register(ContentFormatType contentFormat, string resourceName)
        {
            _registeredHighlighting.Add(contentFormat, new Lazy<IHighlightingDefinition>(() => LoadHighlighting(resourceName)));
        }

        public IHighlightingDefinition GetDefinition(string name)
        {
            return null;
        }

        public IHighlightingDefinition GetDefinition(ContentFormatType contentFormat)
        {
            if (contentFormat is null)
                return null;

            if (!_registeredHighlighting.TryGetValue(contentFormat, out var definition))
                return null;

            try
            {
                return definition.Value;

            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                throw new InvalidOperationException($"The highlighting for '{contentFormat}' is invalid.", ex);
            }
        }

        private IHighlightingDefinition LoadHighlighting(string resourceName)
        {
            XshdSyntaxDefinition xshd;

            var resourceInfo = Application.GetResourceStream(new Uri(resourceName, UriKind.Relative));

            using (var s = resourceInfo.Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    // in release builds, skip validating the built-in highlightings
                    xshd = HighlightingLoader.LoadXshd(reader);
                }
            }
            return HighlightingLoader.Load(xshd, this);
        }
    }
}
