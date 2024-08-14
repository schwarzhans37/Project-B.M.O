using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : AgentBehaviour
{
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        Steering sterring = new Steering();
        float targetOrientation = target.GetComponent<Agent>().orientation;
        float rotation = targetOrientation - agent.orientation;
        rotation = MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);
        if (rotationSize < targetRadius)
            return sterring;
        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = agent.maxRotation;
        else
            targetRotation = agent.maxRotation * rotationSize / slowRadius;
        targetRotation *= rotation / rotationSize;
        steering.angular = targetRotation - agent.rotation;
        steering.angular /= timeToTarget;
        float angluarAccel = Mathf.Abs(steering.angular);

        if (angluarAccel > agent.maxAngularAccel) {
            steering.angular /= angluarAccel;
            steering.angular *= agent.maxAngularAccel;
        }
        return steering;
    }
}
