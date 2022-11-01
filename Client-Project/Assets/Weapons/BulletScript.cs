using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 targetPoint;
    private float time;
    private float startTime;
    private Vector3 direction;


    public void Initialize(Vector3 direction, float speed, Vector3 target)
    {
        targetPoint = target;
        direction.Normalize();
        this.direction = direction * speed;
        time = Vector3.Distance(transform.position, targetPoint) / speed;
        transform.rotation = Quaternion.LookRotation(direction);
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        transform.Translate(direction * Time.fixedDeltaTime, Space.World);
        if(startTime + time <= Time.time)
        {
            //Reached target...
            transform.position = targetPoint;
            Destroy(gameObject);
        }
    }
}
