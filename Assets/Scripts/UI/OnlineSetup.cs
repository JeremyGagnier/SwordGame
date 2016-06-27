using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OnlineSetup : Panel
{
    [SerializeField] private InputField nameField;

    public void SetUsername()
    {
        NetworkingManager.SetUsername(nameField.text);
    }

    public void Ready()
    {
        NetworkingManager.StartSearching(new List<int>() { 2 });
    }
}
