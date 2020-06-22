using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized;

namespace VoiceBasedApp
{
    public interface IVoiceToCommandService
    {
        void StartListening();
        void StopListening();

        bool IsListening();

        void RegisterCommand(string commandString, IVoiceCommand commandToBeExecuted);

        List<string> GetAvailableCommands(); //prio 3

        List<string> GetExecutableCommands();  // prio 3

        void DeregisterCommand(string commandString);

        void RegisterListeningCompletedCallBack(Action callBack); //prio 5

        void DeregisterListeningCompletedCallBack(Action callBack); //prio 5

        void RegisterUnrecognizableCommandCallBack(Action callBack); //prio 6


        void DeregisterUnrecognizableCommandCallBack(Action callBack); //prio 6

        void RegisterUnexecuatbleCallBack(Action callBack); //prio 7

        void DeregisterUnexecuatbleCallBack(Action callBack); //prio 7



    }

    public interface IVoiceCommand
    {
        void Execute();

        bool CanExecute();


        void ExecuteWithResult(string str);
    }

    public static  class Demo
    {
        public static string demoMethod(string Recognized)
        {
            string[] targets =
            {
                "Back",
                "Red",
                "Home",
                "Decrease"
            };



            var matcher = new EnPhoneticFuzzyMatcher<string>(targets);

            var result = matcher.FindNearest("Right");

             return result.ToString();
        }
    }

    

    

   
}