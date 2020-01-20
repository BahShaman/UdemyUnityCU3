using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,0f);
    [SerializeField] float period = 2f;
    [SerializeField] bool oldStyle = false;

    float movementFactor = 0f;
    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon) { return; }
        //todo: deal with NaN
        float cycles = Time.time / period;
        const float tau = 2f * Mathf.PI;
        float rawSinWave = Mathf.Sin(tau * cycles);
        if (oldStyle)
        {
            movementFactor = rawSinWave;
        }
        else
        {
            movementFactor = rawSinWave / 2f + .5f;
        }
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
