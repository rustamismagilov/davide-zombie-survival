using UnityEngine;

public class AxisRotator : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    public float x;
    public float y;
    public float z;

    void Update()
    {
        transform.Rotate(
            x * Time.deltaTime,
            y * Time.deltaTime,
            z * Time.deltaTime,
            Space.Self
        );
    }
}
