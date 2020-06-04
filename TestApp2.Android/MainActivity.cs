

using System.Collections.Generic;
using Android.Views;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech;

namespace TestApp2.Android
{
    [Activity(Label = "Touch", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity , View.IOnTouchListener
    {
        


        private bool isRecording;
        private readonly int VOICE = 10;
        private TextView _textBox;
        private Button _myButton;
        private float _viewX;

        
        protected override void OnCreate(Bundle bundle)
        {
           

            base.OnCreate(bundle);

            isRecording = false;

            SetContentView(Resource.Layout.Main);

            _myButton = FindViewById<Button>(Resource.Id.myView);
            _textBox = FindViewById<TextView>(Resource.Id.textYourText);
            _myButton.SetOnTouchListener(this);

            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
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
            else
                _myButton.Click += delegate
                {
                    // change the text on the button
                    _myButton.Text = "End Recording";
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        // create the intent and start the activity
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                        // voiceIntent = new Intent(RecognizerIntent.ActionVoiceSearchHandsFree);


                        // put a message on the modal dialog
                       // voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.messageSpeakNow));

                        // if there is more then 1.5s of silence, consider the speech over
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                        // you can specify other languages recognised here, for example
                        // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
                        // if you wish it to recognise the default Locale language and German
                        // if you do use another locale, regional dialects may not be recognised very well

                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);

                    }
                };


        }


        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _viewX = e.GetX();
                    break;
                case MotionEventActions.Move:
                    var left = (int)(e.RawX - _viewX);
                    var right = (left + v.Width);
                    v.Layout(left, v.Top, right, v.Bottom);
                    break;
            }
            return true;
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = _textBox.Text + matches[0];

                        // limit the output to 500 characters
                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);
                        _textBox.Text = textInput;
                    }
                    else
                        _textBox.Text = "No speech was recognised";
                    // change the text back on the button
                    _myButton.Text = "Start Recording";
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }


    }
}