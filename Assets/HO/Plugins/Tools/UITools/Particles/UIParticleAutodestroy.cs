using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleAutodestroy :UIParticleAutodisable
{
    protected override void OnEndAnimation()
    {
        Destroy( gameObject);
    }
}
