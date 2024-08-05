using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;

/* -- 추격 -- /*/

public class Seek : AgentBehaviour
{
    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        steering.linear = target.transform.position - transform.position;
        steering.linear.Normalize();
        steering.linear = steering.linear * agent.maxAccel;

        return steering;
    }
}
