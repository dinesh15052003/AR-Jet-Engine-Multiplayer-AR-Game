using UnityEngine;
using Photon.Pun;
public class TouchInput : MonoBehaviourPun
{
    private float rotateSpeed = 0.5f; // Adjust rotation speed
    private float pinchSpeed = 0.0005f; // Adjust pinch speed
    private float minScale = 0.1f; // Minimum scale value
    private float maxScale = 2f; // Maximum scale value

    private Vector2 initialTouchPosition;
    private Vector3 initialRotation;
    private float initialDistance;

    void Update()
    {
        if (!photonView.IsMine)
        {

        }
        else
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    initialTouchPosition = touch.position;
                    initialRotation = transform.rotation.eulerAngles;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPosition = touch.deltaPosition;
                    transform.Rotate(new Vector3(-deltaPosition.y, deltaPosition.x, 0) * rotateSpeed, Space.World);
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                    float deltaDistance = initialDistance - currentDistance;

                    float scaleFactor = 1f - deltaDistance * pinchSpeed;

                    Vector3 newScale = transform.localScale * scaleFactor;
                    newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
                    newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
                    newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

                    transform.localScale = newScale;
                }
            }
        }
        
    }
}
