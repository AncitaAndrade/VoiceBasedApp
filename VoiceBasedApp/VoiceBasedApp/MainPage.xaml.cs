using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace VoiceBasedApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private ISpeechToTextService _speechRecongnitionService;

        public MainPage()
        {
            InitializeComponent();
            try
            {
                _speechRecongnitionService = DependencyService.Get<ISpeechToTextService>();
                MyButton.ImageSource = ImageSource.FromResource("VoiceBasedApp.Images.mic.png");
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

            MessagingCenter.Subscribe<ISpeechToTextService>(this, "Final", (sender) =>
            {
                Button.IsEnabled = true;
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                   var canProceed = await GetPermissionStatusAsync();

                    if (canProceed)
                    {
                        await TextToSpeech.SpeakAsync("You Can give command after the beep");
                        Button.Source = ImageSource.FromResource("VoiceBasedApp.Images.MicrophoneOnMute.png");
                        _speechRecongnitionService.StartSpeechToText();

                    }
                    else
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                        {
                            await DisplayAlert("Need Mic", "Need Microphone Access", "OK");
                        }

                        var status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
                        if (status == PermissionStatus.Granted)
                            Button_Clicked(sender, e);
                    }
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                Button.IsEnabled = false;
            }
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
            var canProceed = await GetPermissionStatusAsync();

            if (canProceed)
            {
                MyButton.ImageSource= ImageSource.FromResource("VoiceBasedApp.Images.MicrophoneOnMute.png");
                _speechRecongnitionService.StartSpeechToText();

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

        private void MyButton_Released(object sender, EventArgs e)
        {
            MyButton.ImageSource = ImageSource.FromResource("VoiceBasedApp.Images.mic.png");
            _speechRecongnitionService.StopSpeechToText();
        }
    }
}
