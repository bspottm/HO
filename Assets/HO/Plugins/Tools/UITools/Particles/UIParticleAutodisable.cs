using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleAutodisable: MonoBehaviour
{
    ParticleSystem particles;

    private void OnEnable()
    {
        particles = GetComponent<ParticleSystem>();
        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();
        var duration = particles.main.duration;
        duration += particles.main.startLifetime.constantMax;
        duration += particles.main.startDelay.constantMax;
        Invoke( "OnEndAnimation", duration );
    }

    protected virtual void OnEndAnimation()
    {
        particles.Stop();
        gameObject.SetActive( false );

    }
}
