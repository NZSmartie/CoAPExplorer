using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using CoAPExplorer.ViewModels;
using ReactiveUI;
using ReactiveUI.AndroidSupport;

namespace CoAPExplorer.Droid.Activities
{
    [Activity(Label = "Device")]
    public class DeviceActivity : ReactiveAppCompatActivity<DeviceViewModel>
    {
        public Toolbar Toolbar { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DeviceView);

            this.WireUpControls();

            SetSupportActionBar(Toolbar);
            SupportActionBar.Title = "Coap Device Test";
        }
    }
}