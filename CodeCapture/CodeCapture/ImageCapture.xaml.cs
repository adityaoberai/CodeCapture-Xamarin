﻿using System;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.Media;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CodeCapture.ReadModels;

namespace CodeCapture
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCapture : ContentPage
    {
        private string imagePath;

        public ImageCapture()
        {
            InitializeComponent();
        }

        //Captures images from user's camera
        private async void clickImage_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera Available", "Function To Be Built Yet", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                SaveToAlbum = true,
                //Directory = "Sample",
                //Name = "test.jpg",
                AllowCropping = false
            });

            if (file == null)
                return;

            imagePath = file.Path;

            //await DisplayAlert("File Location", file.Path, "OK");

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        //Selects images from the user's device
        private async void selectImage_Clicked(object sender, EventArgs e)
        {
            //Debug.WriteLine("Select The Image Button Works!!!");
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported",
                           "Sorry! Permission not granted to photos.", "OK");
                //return null;
            }

            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new
                Plugin.Media.Abstractions.PickMediaOptions
            { });

            //await DisplayAlert("File Location", file.Path, "OK");

            if (file == null)
                return;

            imagePath = file.Path;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        //Sends the image to the Read API for the text to be extracted
        private async void extractText_Clicked(object sender, EventArgs e)
        {
            if (langPicker.SelectedItem == null)
            {
                await DisplayAlert("Language Not Selected!!!", "Go Pick A Language!!!", "OK");
                return;
            }

            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                await DisplayAlert("No Network Available", "Please Connect To Your Wifi Or Turn on Mobile Data", "OK");
            }

            //await DisplayAlert("Send To CodeSpace Button Works!!!","Function To Be Built Yet\n\nLanguage Selected => "+langPicker.SelectedItem,"OK");
            //ExtractText = new NavigationPage(new CodeCapture.ExtractText(imagePath));
            await Navigation.PushModalAsync(new ExtractText(imagePath));
        }
        
    }
}