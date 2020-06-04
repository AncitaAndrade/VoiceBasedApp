
using VoiceBasedApp;
using System.Collections.Generic;
using Android.Views;
using Android.Content.PM;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Xamarin.Essentials;
using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
using System;
using Android.Runtime;
//using Xamarin.Forms;

namespace TestApp2.Android
{
    [Activity(Label = "Touch", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ISpeechToTextService SpeechToText;
        private bool isPermissionGranted;


        private bool isRecording;
        private readonly int VOICE = 10;
        private TextView _textBox;
        private Button _myButton;
        

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
           

            base.OnCreate(savedInstanceState);

            isRecording = false;

            SetContentView(Resource.Layout.Main);

            _myButton = FindViewById<Button>(Resource.Id.myView);
            _textBox = FindViewById<TextView>(Resource.Id.textYourText);


            try
            {
                
                CheckPermissionStatus();
                SpeakInitialInstruction();
            }

            catch (Exception ex)
            {
                _textBox.Text = ex.Message;
            }

            Xamarin.Forms.MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            
            _myButton.Click += delegate
            {
                try
                {
                    
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

        private void SpeakInitialInstruction()
        {
            TextToSpeech.SpeakAsync("To Speak Press and Hold the microphone Image. Release when done!");
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
            _textBox.Text = args;
        }

        private void MyButton_Pressed(object sender, EventArgs e)
        {
            try
            {
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

            
        }



        //protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        //{

        //    if (requestCode == VOICE)
        //    {
        //        if (resultVal == Result.Ok)
        //        {
        //            var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
        //            if (matches.Count != 0)
        //            {
        //                string textInput = _textBox.Text + matches[0];

        //                // limit the output to 500 characters
        //                if (textInput.Length > 500)
        //                    textInput = textInput.Substring(0, 500);
        //                _textBox.Text = textInput;
        //            }
        //            else
        //                _textBox.Text = "No speech was recognised";
        //            // change the text back on the button
        //            _myButton.Text = "Start Recording";
        //        }
        //    }

        //    base.OnActivityResult(requestCode, resultVal, data);
        //}

        


    }
}