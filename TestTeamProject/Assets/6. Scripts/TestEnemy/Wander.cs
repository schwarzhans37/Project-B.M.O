using UnityEngine;
using System.Collections;

public class Wander : Face
{
    public float offset;
    public float radius;
    public float rate;

    public override void Awake()
    {
        target = new GameObejct;
        target.transform.position = transform.position;
        base.Awake();
    }

    public override Steering GetSteering()
    {
        GetSteering steering = new GetSteering();
        float wanderOrientation = Random.Range(-1.0f, 1.0f) * rate;
        float targetOrientation = wanderOrientation + agent.orientation;
        
        Vector3 orientationVec = orientationVec(agent.orientation);
        Vector3 targetPosition = (offset * orientationVec) + transform.position;

        targetPosition = targetPosition + (orientationVec(targetOrientation) * radius);
        targetAux.transform.position = targetPosition;
        
        steering = base.GetSteering();
        steering.linear = targetAux.transform.position - transform.position;
        steering.linear.Normalize();
        steering.linear *= agent.maxAccel;
        return sterring;
    }
}