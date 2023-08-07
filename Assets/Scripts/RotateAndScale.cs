using UnityEngine;

public class RotateAndScale : MonoBehaviour
{
    private Vector2 initialTouchPosition;
    private Vector3 initialScale;
    private float initialRotation;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            // Get the two touches
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch2.phase == TouchPhase.Began)
            {
                // Record the initial touch positions and transform properties
                initialTouchPosition = touch2.position - touch1.position;
                initialScale = transform.localScale;
                initialRotation = transform.rotation.eulerAngles.z;
            }
            else if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                // Calculate the rotation and scale based on the current touch positions
                Vector2 currentTouchPosition = touch2.position - touch1.position;
                float rotation = initialRotation - Vector2.SignedAngle(initialTouchPosition, currentTouchPosition);
                float scale = currentTouchPosition.magnitude / initialTouchPosition.magnitude;

                // Apply the rotation and scale to the transform
                transform.rotation = Quaternion.Euler(0f, 0f, rotation);
                transform.localScale = initialScale * scale;
            }
        }
    }
}
