using System;
using System.ComponentModel;
using System.Threading.Tasks;
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

        ISpeechToTextService stt;
        
        public MainPage()
        {
            InitializeComponent();
            try
            {
                stt = DependencyService.Get<ISpeechToTextService>();
                MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.mic.png");
                SpeakInitialInstruction();
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }
            //stt = DependencyService.Get<ISpeechToTextService>();
            MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

           

        }

        private void SpeakInitialInstruction()
        {
            TextToSpeech.SpeakAsync("To Speak Press and Hold the microphone Image. Release when done!");
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
           recon.Text = args;
        }

        private async Task<bool> GetPermissionStatusAsync()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            if (PermissionStatus.Granted == permissionStatus)
            {
                return true;
            }
            return false;
        }

        private async void MyButton_Pressed(object sender, EventArgs e)
        {
            try
            {
                var canProceed = await GetPermissionStatusAsync();

                if (canProceed)
                {
                    MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.MicrophoneOnMute.png");
                    stt.StartSpeechToText();

                }
                else
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                    {
                        await DisplayAlert("Need Mic", "Need Microphone Access", "OK");
                    }

                    var status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
                    if (status == PermissionStatus.Granted)
                        MyButton_Pressed(sender, e);
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
            stt.StopSpeechToText();
        }
    }
}
