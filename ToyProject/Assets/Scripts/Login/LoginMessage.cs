using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginMessage : MonoBehaviour
{
    [SerializeField]
    string result;
    public enum LogInMsg
    {
        WrongPassword,
        NonExistent,
        LogInSucess,
        Error
    }    

    public LogInMsg TryLogin()
    {
        switch(result)
        {
            case "WrongPassword":
                return LogInMsg.WrongPassword;
            case "NonExistent":
                return LogInMsg.NonExistent;
            case "LogInSucess":
                return LogInMsg.LogInSucess;
            default:
                return LogInMsg.Error;
        }
    }
}
