using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform followedTransform;
    public bool ignoreZ = false;
    [SerializeField] private float pos_smoothTime = 0.025f;
    [SerializeField] private float rot_lerp = 0.5f;

    [SerializeField] private bool followPosition = true, followRotation = true;

    public bool FollowPosition { get => followPosition; set => followPosition = value; }
    public bool FollowRotation { get => followRotation; set => followRotation = value; }
    public float RotationInterpolationConstant { get => rot_lerp; set => rot_lerp = value; }
    public float PositionSmoothtime { get => pos_smoothTime; set => pos_smoothTime = value; }
    public Transform FollowedTransform { get => followedTransform; set => followedTransform = value; }

    void Update()
    {
        if(followPosition)
        {
            Vector3 prevPos = transform.position;

            Vector3 vel = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, followedTransform.position, ref vel, pos_smoothTime);

            if (ignoreZ) transform.position = new Vector3(transform.position.x, transform.position.y, prevPos.z);
        }

        if(followRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, followedTransform.rotation, rot_lerp);
        }
    }

    public void ToggleFollowPosition()
    {
        followPosition = !followPosition;
    }
    public void ToggleFollowRotation()
    {
        followRotation = !followRotation;
    }
}
