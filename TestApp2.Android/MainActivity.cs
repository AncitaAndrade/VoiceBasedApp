using Android.Content.PM;
using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Essentials;
using Plugin.Permissions;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
using System;
using Android.Runtime;
using VoiceBasedApp.Droid;
using VoiceBasedApp;

namespace TestApp2.Android
{
    [Activity(Label = "Touch", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ISpeechToTextService SpeechToText;
        private bool isPermissionGranted;

        private TextView _textBox;
        private Button _myButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            SpeechToText = new SpeechToTextService();

            _myButton = FindViewById<Button>(Resource.Id.myView);
            _textBox = FindViewById<TextView>(Resource.Id.textYourText);

            Xamarin.Forms.MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            
            _myButton.Click += delegate
            {
                try
                {
                    CheckPermissionStatus();
                    
                    if (isPermissionGranted)
                    {

                        SpeechToText.StartSpeechToText();
                    }
                    else
                    {
                        CheckPermissionStatus();
                    }
                }
                catch (Exception ex)
                {
                    _textBox.Text = ex.Message;
                }
            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void CheckPermissionStatus()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Microphone);
            if (PermissionStatus.Granted == permissionStatus)
            {
                isPermissionGranted = true;
            }
            else
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Microphone))
                {
                    //await Alert("Need Mic", "Need Microphone Access", "OK");
                    var alert = new AlertDialog.Builder(_myButton.Context);
                    alert.SetTitle("You don't seem to have a microphone to record with");
                    alert.SetPositiveButton("OK", (sender, e) =>
                    {
                        _textBox.Text = "No microphone present";
                        _myButton.Enabled = false;
                        return;
                    });

                    alert.Show();
                }

                var status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
                if (status == PermissionStatus.Granted)
                {
                    isPermissionGranted = true;
                }

            }
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
            _textBox.Text = args;
        }

       
    }
}