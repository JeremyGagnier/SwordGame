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
        UIManager.instance.OpenPanel("Split Screen Setup");
    }

    private void Online()
    {
        // TODO: Add online panel
    }
}
