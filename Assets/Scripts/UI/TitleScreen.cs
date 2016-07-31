using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : Panel
{
    //TODO: Figure out how to get the network routed into this panel
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
        /*
        if (NetworkingManager.StartNetworking())
        {
            UIManager.instance.OpenPanel("Online Setup");
        }
         */
    }
}
