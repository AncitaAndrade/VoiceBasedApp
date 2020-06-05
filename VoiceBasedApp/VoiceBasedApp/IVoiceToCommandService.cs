﻿using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace VoiceBasedApp
{
    public interface IVoiceToCommandService
    {
        void StartListening();
        void StopListening();

        bool IsListening(); //prio 1

        //void RegisterCommand(string commandString,IVoiceCommand commandToBeExecuted); // prio 2 

        //List<string> GetAvailableCommands(); //prio 3

        //List<string> GetExecutableCommands();  // prio 3

        //void DeregisterCommand(string commandString);  // prio 4



        //void RegisterListeningCompletedCallBack(Action callBack); //prio 5

        // void DeregisterListeningCompletedCallBack(Action callBack); //prio 5

        // void RegisterUnrecognizableCommandCallBack(Action callBack); //prio 6


        // void DeregisterUnrecognizableCommandCallBack(Action callBack); //prio 6

        // void RegisterUnexecuatbleCallBack(Action callBack); //prio 7

        // void DeregisterUnexecuatbleCallBack(Action callBack); //prio 7



    }

    public interface IVoiceCommand
    {
        void Execute();

        bool CanExecute();

       

    }

    

    

   
}