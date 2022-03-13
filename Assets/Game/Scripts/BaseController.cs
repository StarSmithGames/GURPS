using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public abstract bool IsGrounded { get; protected set; }
    public abstract Vector3 GetVelocity();
    public abstract Vector3 GetMovementVelocity();
}