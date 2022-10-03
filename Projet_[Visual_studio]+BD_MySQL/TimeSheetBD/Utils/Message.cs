using System;
using System.Collections.Generic;
namespace TimeSheetBD.Utils;

public class Message
{
    private List<String> messageList;
    public List<String> MessageList 
    { 
        get 
        { 
            return messageList; 
        } 
    }

    public Message()
    {
        messageList = new List<String>();
    }

    public void addMessage(String message)
    {
        messageList.Add(message);
    }
    public override string ToString()
    {
        string output = "";
        foreach(String message in messageList)
        {
            output += message + "\n";
        }
        return output;
    }
}
