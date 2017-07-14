using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Dynamo.LibraryUI.Handlers
{
    class CompositeResourceProvider : ResourceProviderBase
    {
        private IEnumerable<IResourceProvider> providers;

        public CompositeResourceProvider(IEnumerable<IResourceProvider> providers, string scheme = "http")
            : base(providers.All(x => x.IsStaticResource), scheme)
        {
            this.providers = providers;
        }

        public override Stream GetResource(IRequest request, out string extension)
        {
            foreach (var item in providers)
            {
                var stream = item.GetResource(request, out extension);
                if (stream != null) return stream;
            }
            extension = "";
            return null;
        }
    }
}
