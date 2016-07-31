using UnityEngine;

public class Panel : MonoBehaviour
{
    public virtual void Open()
    {
        Show();
    }
    public virtual void Close()
    {
        Destroy(this.gameObject);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

}
