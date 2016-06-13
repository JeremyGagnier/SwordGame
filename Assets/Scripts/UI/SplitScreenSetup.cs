using UnityEngine;
using UnityEngine.UI;

public class SplitScreenSetup : Panel
{
    [SerializeField] private Button twoPlayers;
    [SerializeField] private Button threePlayers;
    [SerializeField] private Button fourPlayers;
    
    void Start()
    {
        twoPlayers.onClick.AddListener(StartGame);
        threePlayers.onClick.AddListener(StartGame);
        fourPlayers.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        UIManager.instance.ClosePanel("Split Screen Setup");
        UIManager.instance.ClosePanel("Title Screen");
    }
}
