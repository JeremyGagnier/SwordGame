using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : Panel
{
    [SerializeField] private Button splitScreen;
    [SerializeField] private Button online;
    
    void Start()
    {
        splitScreen.onClick.AddListener(SplitScreen);
        online.onClick.AddListener(Online);
    }

    private void SplitScreen()
    {
        UIManager.instance.OpenPanel(PanelType.SPLIT_SCREEN_SETUP);
    }

    private void Online()
    {
        if (Game.instance.GetNetwork().StartNetworking())
        {
            UIManager.instance.OpenPanel(PanelType.ONLINE_SETUP);
        }
    }
}
