using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnlineSetup : Panel
{
    //TODO: Figure out how to get the network routed into this panel
    [SerializeField] private InputField nameField;

    public void SetUsername()
    {
        //NetworkingManager.SetUsername(nameField.text);
    }

    public void Ready()
    {
        //NetworkingManager.StartSearching(new List<int>() { 2 });
    }
}
