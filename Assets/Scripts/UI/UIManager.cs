using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PanelType
{
    TITLE_SCREEN,
    SPLIT_SCREEN_SETUP,
    ONLINE_SETUP,
    GAME_GUI
}

public class UIManager : MonoBehaviour
{
    public const PanelType BASE_PANEL = PanelType.TITLE_SCREEN;
    public static UIManager instance = null;

    private Stack<Panel> panelStack = new Stack<Panel>();

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogException(new Exception("There are two UIManagers. This is extremely dangerous."));
        }
        instance = this;
    }

    void Start()
    {
        OpenPanel(BASE_PANEL);
    }

    private Panel GetPanel(PanelType p)
    {
        string panelName = Enum.GetName(typeof(PanelType), p).ToLower();
        GameObject prefab = Resources.Load<GameObject>("Panels/" + panelName);
        if (prefab == null)
        {
            Debug.LogError("Can't find panel with name: " + panelName);
        }

        GameObject panelObj = GameObject.Instantiate<GameObject>(prefab);
        panelObj.transform.SetParent(this.transform, false);
        panelObj.name = panelName;
        Panel panel = panelObj.GetComponent<Panel>();
        if (panel == null)
        {
            Debug.LogError(panelName + " doesn't have a panel component");
        }
        return panel;
    }

    public void OpenPanel(PanelType p)
    {
        Panel panel = GetPanel(p);
        if (panel == null) return;

        if (panelStack.Count > 0) panelStack.Peek().Hide();

        panelStack.Push(panel);
        panel.Open();
    }

    public void ClosePanel(PanelType p)
    {
        string panelName = Enum.GetName(typeof(PanelType), p).ToLower();
        Panel panel = panelStack.Peek();
        if (panel.name != panelName)
        {
            Debug.LogError(
                "You can only close panels at the top of the stack!" +
                string.Format("Current panel: {0}, panel to close: {1}", panel.name, panelName));
            return;
        }

        panelStack.Pop();
        panel.Close();

        if (panelStack.Count > 0) panelStack.Peek().Show();
    }
}
