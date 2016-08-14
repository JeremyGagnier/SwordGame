using UnityEngine;
using UnityEngine.UI;

public class SplitScreenSetup : Panel
{
    [SerializeField] private Button twoPlayers;
    [SerializeField] private Button threePlayers;
    [SerializeField] private Button fourPlayers;
    
    void Start()
    {
        twoPlayers.onClick.AddListener(() => { StartGame(2); });
        threePlayers.onClick.AddListener(() => { StartGame(3); });
        fourPlayers.onClick.AddListener(() => { StartGame(4); });
    }

    private void StartGame(int numPlayers)
    {
        UIManager.instance.ClosePanel(PanelType.SPLIT_SCREEN_SETUP);
        UIManager.instance.ClosePanel(PanelType.TITLE_SCREEN);
        Game.instance.StartGame(numPlayers, null);
    }
}
