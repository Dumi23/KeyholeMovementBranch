#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mousesens = 100f;

    float xrot = 0f;

    public Transform playBody;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mousesens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mousesens * Time.deltaTime;

        xrot -= mouseY;
        xrot = Mathf.Clamp(xrot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xrot, 0f, 0f);
        playBody.Rotate(Vector3.up * mouseX);



    }
}
#endif  