using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnlineSetup : Panel
{
    [SerializeField] private InputField nameField;
    private OnlineNetwork network;

    void Awake()
    {
        network = Game.instance.GetNetwork();
    }

    public void SetUsername()
    {
        network.SetUsername(nameField.text);
    }

    public void Ready()
    {
        network.StartSearching(new List<int>() { 2 });
    }
}
