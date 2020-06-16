using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToCommandLibrary
{
   abstract public class CommonCode : IVoiceToCommandService
    {
        public bool isRecording;
        public IDictionary<string, IVoiceCommand> AllRegisteredCommands;

        public void DeregisterCommand(string commandString)
        {
            AllRegisteredCommands.Remove(commandString);
        }
        

        public void DeregisterListeningCompletedCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public void DeregisterUnexecuatbleCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public void DeregisterUnrecognizableCommandCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAvailableCommands()
        {
            return AllRegisteredCommands.Keys.ToList();
        }

        public List<string> GetExecutableCommands()
        {
            return (AllRegisteredCommands.Where(item => item.Value.CanExecute()).Select(item => item.Key)).ToList();
        }

        public bool IsListening()
        {
            return isRecording;
        }

        public void RegisterCommand(string commandString, IVoiceCommand commandToBeExecuted)
        {
            AllRegisteredCommands.Add(commandString.ToLower(), commandToBeExecuted);
        }

        public void RegisterListeningCompletedCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public void RegisterUnexecuatbleCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public void RegisterUnrecognizableCommandCallBack(Action callBack)
        {
            throw new NotImplementedException();
        }

        public abstract void StartListening();


        public abstract void StopListening();
       
    }
}
