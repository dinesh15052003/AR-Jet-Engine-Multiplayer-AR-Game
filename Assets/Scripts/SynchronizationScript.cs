using UnityEngine;
using Photon.Pun;

public class SynchronizationScript : MonoBehaviourPun, IPunObservable
{
    //private Vector3 networkPosition;
    private Vector3 networkRotation;
    private Vector3 networkScale;

    private Touch touch;
    private float speedModifier = 0.5f; // adjust the rotation speed
    private Vector2 initialTouchPosition;
    private Vector3 initialScale;

    void Start()
    {
        //networkPosition = transform.position;
        networkRotation = transform.rotation.eulerAngles;
        networkScale = transform.localScale;
        initialScale = networkScale;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Synchronize position
            //transform.position = networkPosition;

            // Synchronize rotation
            transform.rotation = Quaternion.Euler(networkRotation);

            // Synchronize scale
            transform.localScale = networkScale;
        }
        else
        {
            //// Check for touch input
            //if (Input.touchCount > 0)
            //{
            //    touch = Input.GetTouch(0);

            //    // Check if touch has just started
            //    if (touch.phase == TouchPhase.Began)
            //    {
            //        // Do nothing
            //    }
            //    // Check if touch is moving
            //    else if (touch.phase == TouchPhase.Moved)
            //    {
            //        // Get the touch delta position and rotate the GameObject
            //        Vector2 deltaPosition = touch.deltaPosition;
            //        transform.Rotate(new Vector3(-deltaPosition.y, deltaPosition.x, 0) * speedModifier, Space.World);
            //    }
            //    // Check if touch has ended
            //    else if (touch.phase == TouchPhase.Ended)
            //    {
            //        // Do nothing
            //    }
            //}

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    initialTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPosition = touch.deltaPosition;
                    transform.Rotate(new Vector3(-deltaPosition.y, deltaPosition.x, 0) * speedModifier, Space.World);
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    initialScale = transform.localScale;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float previousDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);
                    float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                    float scaleFactor = currentDistance / previousDistance;
                    transform.localScale = initialScale * scaleFactor;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Write position, rotation and scale data to the stream
            //stream.SendNext(transform.position);
            stream.SendNext(transform.rotation.eulerAngles);
            stream.SendNext(transform.localScale);
        }
        else
        {
            // Read position, rotation and scale data from the stream
            //networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Vector3)stream.ReceiveNext();
            networkScale = (Vector3)stream.ReceiveNext();
        }
    }
}
