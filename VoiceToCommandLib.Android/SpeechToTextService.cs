using Android.Content;
using Android.OS;
using Android.App;
using Android.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceToCommandLibrary;
using VoiceToCommandLib.Android;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(SpeechToTextService))]
namespace VoiceToCommandLib.Android
{
    public class SpeechToTextService : IVoiceToCommandService
    {
        private SpeechRecognizer Recognizer { get; set; }
        private Intent SpeechIntent { get; set; }

        private bool isRecording;

        private Action _callBack;

        private Action ListeningCompletion;

        private Action UnrecognizableCommand;

        private Action UnexecutableCallBack;

        private IDictionary<string, IVoiceCommand> AllRegisteredCommands;

        public SpeechToTextService()
        {
            AllRegisteredCommands = new Dictionary<string, IVoiceCommand>();
        }

        public void StartListening()
        {
            StartRecordingAndRecognizing();
        }

        private void StartRecordingAndRecognizing()
        {
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
            if (AllRegisteredCommands.ContainsKey(recognized))
            {
                var command = AllRegisteredCommands[recognized];
                if (command.CanExecute())
                {
                    command.Execute();
                }
            }


            MessagingCenter.Send<IVoiceToCommandService, string>(this, "STT", recognized);
        }

        public void StopListening()
        {
            if (isRecording)
            {
                isRecording = false;
                Recognizer.StopListening();
            }

        }

        public bool IsListening()
        {
            return isRecording;
        }

        public void RegisterCommand(string commandString, IVoiceCommand commandToBeExecuted)
        {
            AllRegisteredCommands.Add(commandString.ToLower(), commandToBeExecuted);
        }

        public void DeregisterCommand(string commandString)
        {
            AllRegisteredCommands.Remove(commandString);
        }

        public List<string> GetAvailableCommands()
        {
            return AllRegisteredCommands.Keys.ToList();
        }

        public List<string> GetExecutableCommands()
        {
            return (AllRegisteredCommands.Where(item => item.Value.CanExecute()).Select(item => item.Key)).ToList();
        }

        public void RegisterListeningCompletedCallBack(Action callBack)
        {
            _callBack = callBack;
            _callBack += ListeningCompletion;

            // callBack += ListeningCompleted;

            ListeningCompletion += callBack;

        }

        public void DeregisterListeningCompletedCallBack(Action callBack)
        {
            // _callBack -= callBack;

            callBack -= ListeningCompletion;

        }

        public void RegisterUnrecognizableCommandCallBack(Action callBack)
        {
            callBack += UnrecognizableCommand;
        }

        public void DeregisterUnrecognizableCommandCallBack(Action callBack)
        {
            callBack -= UnrecognizableCommand;
        }

        public void RegisterUnexecuatbleCallBack(Action callBack)
        {
            callBack += UnexecutableCallBack;
        }

        public void DeregisterUnexecuatbleCallBack(Action callBack)
        {
            callBack -= UnexecutableCallBack;
        }




    }
}