using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public const string BASE_PANEL = "Title Screen";
    public static UIManager instance = null;

    [SerializeField] private List<Panel> panels;

    private Stack<Panel> panelStack = new Stack<Panel>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OpenPanel(BASE_PANEL);
    }

    private Panel GetPanel(string panelName)
    {
        Panel p = null;
        foreach (Panel panel in panels)
        {
            if (panel.name == panelName)
            {
                p = panel;
                break;
            }
        }
        if (p == null)
        {
            Debug.LogError("Can't find panel with name: " + panelName);
        }
        return p;
    }

    public void AddPanel(Panel p)
    {
        panels.Add(p);
        if (p.name == BASE_PANEL)
        {
            OpenPanel(BASE_PANEL);
        }
    }

    public void OpenPanel(string panelName)
    {
        Panel p = GetPanel(panelName);
        if (p == null) return;

        if (panelStack.Count > 0) panelStack.Peek().Hide();

        panelStack.Push(p);
        p.Open();
    }

    public void ClosePanel(string panelName)
    {
        Panel p = GetPanel(panelName);
        if (p == null) return;

        Panel currPanel = panelStack.Peek();
        if (currPanel != p)
        {
            Debug.LogError(
                "You can only close panels at the top of the stack!" +
                string.Format("Current panel: {0}, panel to close: {1}", currPanel.name, panelName));
            return;
        }

        panelStack.Pop().Close();

        if (panelStack.Count > 0) panelStack.Peek().Show();
    }
}
