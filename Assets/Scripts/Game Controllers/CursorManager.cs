using UnityEngine;

public class CursorManager : MonoBehaviour
{

    static CursorManager instance;

    public Texture2D cursorNormal, cursorCanGrab, cursorHasGrabbed;
    public Vector2 hotspotNormal = new Vector2(95, 38),
        hotspotCanGrab = new Vector2(105, 80),
        hotspotHasGrabbed = new Vector2(125, 100);
    public bool canGrab = false, hasGrabbed = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
        Destroy(this);
    }

    void Update()
    {
        if (hasGrabbed)
        {
            SetCursor(2);
        }
        else if(canGrab)
        {
            SetCursor(1);
        }
        else
        {
            SetCursor(0);
        }
    }

    public static void SetCanGrab(bool value)
    {
        if (instance != null) instance.canGrab = value;
    }
    public static void SetHasGrabbed(bool value)
    {
        if (instance != null) instance.hasGrabbed = value;
    }

    void SetCursor(int type = 0)
    {
        switch (type)
        {
            case (1):
                Cursor.SetCursor(instance.cursorCanGrab, instance.hotspotCanGrab, CursorMode.Auto);
                break;
            case (2):
                Cursor.SetCursor(instance.cursorHasGrabbed, instance.hotspotHasGrabbed, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(instance.cursorNormal, instance.hotspotNormal, CursorMode.Auto);
                break;
        }
    }
}
