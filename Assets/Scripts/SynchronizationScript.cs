using UnityEngine;
using Photon.Pun;

public class SynchronizationScript : MonoBehaviourPun, IPunObservable
{
    //private Vector3 networkPosition;
    private Vector3 networkRotation;
    private Vector3 networkScale;

    private Touch touch;
    private float speedModifier = 0.5f; // adjust the rotation speed

    void Start()
    {
        //networkPosition = transform.position;
        networkRotation = transform.rotation.eulerAngles;
        networkScale = transform.localScale;
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
            // Check for touch input
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                // Check if touch has just started
                if (touch.phase == TouchPhase.Began)
                {
                    // Do nothing
                }
                // Check if touch is moving
                else if (touch.phase == TouchPhase.Moved)
                {
                    // Get the touch delta position and rotate the GameObject
                    Vector2 deltaPosition = touch.deltaPosition;
                    transform.Rotate(new Vector3(-deltaPosition.y, deltaPosition.x, 0) * speedModifier, Space.World);
                }
                // Check if touch has ended
                else if (touch.phase == TouchPhase.Ended)
                {
                    // Do nothing
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
