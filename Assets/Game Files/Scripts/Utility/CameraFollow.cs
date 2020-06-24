using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Public and Serialized variables

    [SerializeField]
    private Transform targetToFollow;
    [Range(0f,1f)]
    [SerializeField]
    private float smoothSpeed = 0.125f; // Smaller values will reach the target faster
    [SerializeField]
    private Vector3 offset;

    #endregion

    #region Private variables

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _offsetPosition;

    #endregion

    #region Monobehaviour methods

    // Start is called before the first frame update
    void Start()
    {
        // Sets the camera to the target's position + the offset
        transform.position = targetToFollow.position + offset;
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    #endregion

    #region Execution methods

    private void FollowTarget()
    {
        // Follows the target using a small smoothing dampening movement
        Vector3 desiredPosition = targetToFollow.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }

    #endregion

}
