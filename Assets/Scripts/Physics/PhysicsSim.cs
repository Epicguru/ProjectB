using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSim : MonoBehaviour
{
    public static bool Paused
    {
        get
        {
            return _paused;
        }
        set
        {
            if (value == _paused)
                return;

            _paused = value;
        }
    }

    private static bool _paused;

    private float timer;

    private void Update()
    {
        if (Physics2D.autoSimulation)
            return;

        timer += Time.deltaTime;
        while(timer >= Time.fixedDeltaTime)
        {
            timer -= Time.fixedDeltaTime;

            // Run the simulation when not paused.
            if(!Paused)
                Physics2D.Simulate(Time.fixedDeltaTime);
        }
    }
}
