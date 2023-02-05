using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostData 
{
    [SerializeField]
    string order;
    [SerializeField]
    string result;
    [SerializeField]
    string msg;
    [SerializeField]
    string value;

    public string Order { get => order; }
    public string Result { get => result; }
    public string Msg { get => msg; }
    public string Value { get => value; }
    //public ORDER GetOrder()
    //{
    //    switch(order)
    //    {
    //        case "login":
    //            return ORDER.LOGIN;
    //        case "register":
    //            return ORDER.REGISTER;
    //        default:
    //            {
    //                Debug.LogError("Wroing Order Message");
    //                return ORDER.NONE;
    //            }                
    //    }
    //}
    //public bool Result => result == "ok" ? true : false;
    
    //public MESSAGE GetMsg()
    //{
    //    switch (msg)
    //    {
    //        case "login":
    //            return MESSAGE.LOGINSUCCESS;
    //        case "register":
    //            return MESSAGE.REGISTERSUCCESS;
    //        case "duplication":
    //            return MESSAGE.DUPLICATION;
    //            case "idfail":
    //            return MESSAGE.IDFAIL;
    //        case "passwordfail":
    //            return MESSAGE.PASSWORDFAIL;
    //        default:
    //            {
    //                Debug.LogError("Wrong MSG ");
    //                return MESSAGE.PASSWORDFAIL;
    //            }                
    //    }
    //}
    //public enum ORDER
    //{
    //    LOGIN,
    //    REGISTER,
    //    NONE
    //}
    //public enum MESSAGE
    //{
    //    DUPLICATION,
    //    IDFAIL,
    //    PASSWORDFAIL,
    //    REGISTERSUCCESS,        
    //    LOGINSUCCESS,
    //    NONE
    //}    
}
