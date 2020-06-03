using System;
using System.ComponentModel;
using Xamarin.Forms;
using VoiceBasedApp;
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace TestApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        private ISpeechToTextService speechToTextService;
        private bool isPermissionGranted;
        
        public MainPage()
        {
            InitializeComponent();
            try
            {
                speechToTextService = DependencyService.Get<ISpeechToTextService>();
                MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.mic.png");
                CheckPermissionStatus();
                SpeakInitialInstruction();
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }
            MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });
        }

        private async void CheckPermissionStatus()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            if(PermissionStatus.Granted== permissionStatus)
            {
                isPermissionGranted = true;
            }
            else
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                {
                    await DisplayAlert("Need Mic", "Need Microphone Access", "OK");
                }

                var status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
                if (status == PermissionStatus.Granted)
                {
                    isPermissionGranted = true;
                }
                    
            }
        }

        private void SpeakInitialInstruction()
        {
            TextToSpeech.SpeakAsync("To Speak Press and Hold the microphone Image. Release when done!");
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
           recon.Text = args;
        }

        private void MyButton_Pressed(object sender, EventArgs e)
        {
            try
            {
                if (isPermissionGranted)
                {
                    MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.MicrophoneOnMute.png");
                    speechToTextService.StartSpeechToText();
                }
                else
                {
                    CheckPermissionStatus();
                }
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                MyButton.IsEnabled = false;
            }
        }

        private void MyButton_Released(object sender, EventArgs e)
        {
            MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.mic.png");
            speechToTextService.StopSpeechToText();
        }
    }
}
