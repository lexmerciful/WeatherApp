using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using WeatherApp.Fragments;

namespace WeatherApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText citynametext;
        Button checkweatherbutton;
        ImageView weatherimage;
        TextView temperaturetextview, weatherdescriptiontext, placetext;

        ProgressDialogFragment progressDialogue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            citynametext = FindViewById<EditText>(Resource.Id.citynametext);
            weatherimage = FindViewById<ImageView>(Resource.Id.weatherimage);
            temperaturetextview = FindViewById<TextView>(Resource.Id.temperaturetextview);
            weatherdescriptiontext = FindViewById<TextView>(Resource.Id.weatherdescriptiontext);
            placetext = FindViewById<TextView>(Resource.Id.placetext);
            checkweatherbutton = FindViewById<Button>(Resource.Id.checkweatherbutton);
             
            checkweatherbutton.Click += Checkweatherbutton_Click;

            GetWeather("Kwara");
        }

        private void Checkweatherbutton_Click(object sender, System.EventArgs e)
        {
            string place = citynametext.Text;
            GetWeather(place);

            if (CrossConnectivity.Current.IsConnected)
            {
                citynametext.Text = "";
            }
        }

        async void GetWeather(string place)
        {
            string apiKey = "6020e0a32e67397bbd208eba02b70a4d";
            string apiBase = "https://api.openweathermap.org/data/2.5/weather?q=";
            string unit = "metric";

            if (string.IsNullOrEmpty(place))
            {
                Toast.MakeText(this, "Please enter a valid city name", ToastLength.Short).Show();
                return;
            }

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "No internet connection!", ToastLength.Short).Show();
                return;
            }

            ShowProgressDialogue("Fetching Weather...");

            //Assychronous API call using HttpClient
            string url = apiBase + place + "&appid=" + apiKey + "&units=" + unit;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            string res= result.ToString();

            //if (!res.Contains("temp"))
            //{
            //    //Toast.MakeText(this, result.Length.ToString(), ToastLength.Short).Show();
            //    CloseProgressDialogue();
            //    Toast.MakeText(this, "temp is available", ToastLength.Short).Show();
            //    return;
            //}
            Console.WriteLine(result);

            var resultObject = JObject.Parse(result);
            string weatherDescription = resultObject["weather"][0]["description"].ToString();
            string icon = resultObject["weather"][0]["icon"].ToString();
            string temperature = resultObject["main"]["temp"].ToString();
            string placename = resultObject["name"].ToString();
            string country = resultObject["sys"]["country"].ToString();

            weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);

            weatherdescriptiontext.Text = weatherDescription;
            temperaturetextview.Text = temperature;
            placetext.Text = placename + ", " + country;

            //Download Image using WebRequest
            string imageUrl = "http://openweathermap.org/img/wn/" + icon + ".png";
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = WebRequest.Create(imageUrl);
            request.Timeout = int.MaxValue;
            request.Method = "GET";
            
            WebResponse response = default(WebResponse);
            response = await request.GetResponseAsync();
            MemoryStream ms = new MemoryStream();
            response.GetResponseStream().CopyTo(ms);
            byte[] imageData = ms.ToArray();

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            weatherimage.SetImageBitmap(bitmap);

            CloseProgressDialogue();

        }

        void ShowProgressDialogue(string staus)
        {
            progressDialogue = new ProgressDialogFragment(staus);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "progress");
        }

        void CloseProgressDialogue()
        {
            if(progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }
}