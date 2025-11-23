using UnityEngine;

public class SimpleLook : MonoBehaviour
{
    public float sensitivity = 2f;
    private float rotationX;
    private float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
}
