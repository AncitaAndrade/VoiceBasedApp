using Android.Content;
using Android.OS;
using Android.Speech;
using System.Collections.Generic;
using System.Linq;
using VoiceBasedApp.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpeechToTextService))]
namespace VoiceBasedApp.Droid
{
    public class SpeechToTextService : IVoiceToCommandService
    {
        private SpeechRecognizer Recognizer { get; set; }
        private Intent SpeechIntent { get; set; }

        private bool isRecording;

        private IDictionary<string, IVoiceCommand> AllRegisteredCoomands;

        public SpeechToTextService()
        {
            AllRegisteredCoomands = new Dictionary<string, IVoiceCommand>();
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

            Recognizer = SpeechRecognizer.CreateSpeechRecognizer(Android.App.Application.Context);
            Recognizer.SetRecognitionListener(recListener);

            SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Android.App.Application.Context.PackageName);
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
            isRecording=false;
            recognized = recognized.ToLower();
            if(AllRegisteredCoomands.ContainsKey(recognized))
            {
                var command = AllRegisteredCoomands[recognized];
                if(command.CanExecute())
                {
                    command.Execute();
                }
            }


        MessagingCenter.Send<IVoiceToCommandService, string>(this, "STT", recognized); 
        }

        public void StopListening()
        {
            if(isRecording)
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
            AllRegisteredCoomands.Add(commandString.ToLower(), commandToBeExecuted);
        }

        public void DeregisterCommand(string commandString)
        {
            AllRegisteredCoomands.Remove(commandString);
        }

        public List<string> GetAvailableCommands()
        {
            //List<string> AvailableCommands = new List<string>();


            //foreach(var command in AllRegisteredCoomands.Keys)
            //{
            //    AvailableCommands.Add(command);
            //}

            //return AvailableCommands;

            List<string> availableCommand = AllRegisteredCoomands.Keys.ToList();
            return availableCommand;
        }
    }
}