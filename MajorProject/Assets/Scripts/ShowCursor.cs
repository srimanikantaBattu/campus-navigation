using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Keep enforcing this every frame in case some other script changes it
        if (!Cursor.visible || Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
