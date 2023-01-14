using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherApp.Fragments
{
    public class ProgressDialogFragment : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        string thisStatus;
        public ProgressDialogFragment(string status)
        {
            thisStatus = status;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView statustextview = (TextView)view.FindViewById(Resource.Id.progressstatus);
            statustextview.Text = thisStatus;
            return view;
        }
    }
}