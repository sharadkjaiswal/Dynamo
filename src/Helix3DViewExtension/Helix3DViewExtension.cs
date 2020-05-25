using Dynamo.Controls;
using Dynamo.PackageManager;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dynamo.Views.Helix3D
{
    /// <summary>
    /// The View layer of the packageManagerExtension.
    /// Currently its only responsibility is to request the loading of ViewExtensions which it finds in packages.
    /// In the future packageManager functionality should be moved from DynamoCoreWPF to this ViewExtension.
    /// </summary>
    public class Helix3DViewExtension : IViewExtension, IViewExtensionSource
    {
        private readonly List<IViewExtension> requestedExtensions = new List<IViewExtension>();
        public string Name
        {
            get
            {
                return "Helix3DViewExtension";
            }
        }

        public IEnumerable<IViewExtension> RequestedExtensions
        {
            get
            {
                return this.requestedExtensions;
            }
        }

        public string UniqueId
        {
            get
            {
                return "2ABFEEF3-794D-4E16-AD28-5B7E341943FE";
            }
        }

        public event Action<IViewExtension> RequestAddExtension;
        public event Func<string, IViewExtension> RequestLoadExtension;

        public void Dispose()
        {
            
        }

        public void Loaded(ViewLoadedParams p)
        {
           
        }

        public void Shutdown()
        {
            Dispose();
        }

        public void Startup(ViewStartupParams p)
        {
        }

        private void requestLoadViewExtensionsForLoadedPackages(IEnumerable<Package> packages)
        {
            foreach (var package in packages)
            {
                //if package was previously loaded then additional files are already cached.
                if (package.Loaded)
                {
                    var vieweExtensionManifests = package.AdditionalFiles.Where(file => file.Model.Name.Contains("ViewExtensionDefinition.xml")).ToList();
                    foreach (var extPath in vieweExtensionManifests)
                    {
                        var viewExtension = RequestLoadExtension?.Invoke(extPath.Model.FullName);
                        if (viewExtension != null)
                        {
                            RequestAddExtension?.Invoke(viewExtension);
                        }
                        this.requestedExtensions.Add(viewExtension);
                    }
                }
            }
        }

        private void packageLoadedHandler(Package package)
        {
            //when a package is loaded with packageManager, this extension should inspect it for viewExtensions.
            this.requestLoadViewExtensionsForLoadedPackages(new List<Package>() { package });
        }
    }
}
