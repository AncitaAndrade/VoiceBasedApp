using Android.Content;
using Android.OS;
using Android.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.Net;
using Android.Support.Design.Animation;
using Android.Views.Animations;
//using VoiceToCommandLibrary;
using VoiceBasedApp;
using VoiceToCommandLib.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Animation = Android.Views.Animations.Animation;
using Application = Android.App.Application;
using AnimationUtils = Android.Views.Animations.AnimationUtils;


//[assembly: Dependency(typeof(SpeechToTextService))]
namespace VoiceToCommandLib.Android
{
    public class SpeechToTextService : CommonCode
    {
       

        private SpeechRecognizer Recognizer { get; set; }
        private Intent SpeechIntent { get; set; }
       
        //private bool isRecording;

        //private IDictionary<string, IVoiceCommand> AllRegisteredCommands;


        public SpeechToTextService()
        {
            AllRegisteredCommands = new Dictionary<string, IVoiceCommand>();
        }

        //public Boolean isOnline()
        //{
        //    ConnectivityManager conMgr = (ConnectivityManager).getSystemService(Context.ConnectivityService);
        //    NetworkInfo netInfo = conMgr.getActiveNetworkInfo();

        //    if (netInfo == null || !netInfo.isConnected() || !netInfo.isAvailable())
        //    {
        //        Toast.MakeText(context, "No Internet connection!", Toast.LENGTH_LONG).show();
        //        return false;
        //    }
        //    return true;
        //}
        public override void StartListening()
        {

            
            StartRecordingAndRecognizing();
        }

        private void StartRecordingAndRecognizing()
        {
            ActivityIndicatorRenderer activity = new ActivityIndicatorRenderer(Application.Context);
            //var animation = new Animation(v => image.Scale = v, 1, 2);
            //Animation myAnimation = AnimationUtils.LoadAnimation(Resource.Animation.hyperSpace);


            var recListener = new RecognitionListener();
            recListener.BeginSpeech += RecListener_BeginSpeech;
            recListener.EndSpeech += RecListener_EndSpeech;
            recListener.Error += RecListener_Error;
            recListener.Ready += RecListener_Ready;
            recListener.Recognized += RecListener_Recognized;

            Recognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
            Recognizer.SetRecognitionListener(recListener);

            SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            Recognizer.StartListening(SpeechIntent);
           // activity.StartAnimation(myAnimation);

        }


        private void RecListener_Ready(object sender, Bundle e) => System.Diagnostics.Debug.WriteLine(nameof(RecListener_Ready));

        private void RecListener_BeginSpeech()
        {
            isRecording = true;

            System.Diagnostics.Debug.WriteLine(nameof(RecListener_BeginSpeech));

        }

        private void RecListener_EndSpeech()
        {
            isRecording = false;

            System.Diagnostics.Debug.WriteLine(nameof(RecListener_EndSpeech));
        }

        private void RecListener_Error(object sender, SpeechRecognizerError e)
        {
            isRecording = false;
            MessagingCenter.Send<IVoiceToCommandService, string>(this, "STT", e.ToString());
        }

        private void RecListener_Recognized(object sender, string recognized)
        {
            isRecording = false;
            recognized = recognized.ToLower();
            var displayCommand = AllRegisteredCommands["type"];
            displayCommand.ExecuteWithResult(recognized);
            if (AllRegisteredCommands.ContainsKey(recognized))
            {
                var command = AllRegisteredCommands[recognized];
                if (command.CanExecute())
                {
                    command.Execute();
                }
            }
            else
            {
               var result =  Demo.demoMethod(recognized);
               System.Diagnostics.Debug.WriteLine(result);
            }


            MessagingCenter.Send<IVoiceToCommandService, string>(this, "STT", recognized);
        }

        public override void StopListening()
        {
            if (isRecording)
            {
                isRecording = false;
                Recognizer.StopListening();
               
            }

        }


        //public bool IsListening()
        //{
        //    return isRecording;
        //}

        //public void RegisterCommand(string commandString, IVoiceCommand commandToBeExecuted)
        //{
        //    AllRegisteredCommands.Add(commandString.ToLower(), commandToBeExecuted);
        //}

        //public void DeregisterCommand(string commandString)
        //{
        //    AllRegisteredCommands.Remove(commandString);
        //}

        //public List<string> GetAvailableCommands()
        //{
        //    return AllRegisteredCommands.Keys.ToList();
        //}

        //public List<string> GetExecutableCommands()
        //{
        //    return (AllRegisteredCommands.Where(item => item.Value.CanExecute()).Select(item => item.Key)).ToList();
        //}

        //public void RegisterListeningCompletedCallBack(Action callBack)
        //{

        //}

        //public void DeregisterListeningCompletedCallBack(Action callBack)
        //{
        //}

        //public void RegisterUnrecognizableCommandCallBack(Action callBack)
        //{

        //}

        //public void DeregisterUnrecognizableCommandCallBack(Action callBack)
        //{

        //}

        //public void RegisterUnexecuatbleCallBack(Action callBack)
        //{

        //}

        //public void DeregisterUnexecuatbleCallBack(Action callBack)
        //{

        //}




    }
}