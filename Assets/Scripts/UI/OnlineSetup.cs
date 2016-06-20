using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnlineSetup : Panel
{
    [SerializeField] private InputField nameField;

    public void SetUsername()
    {
        NetworkingManager.SetUsername(nameField.text);
    }
}
