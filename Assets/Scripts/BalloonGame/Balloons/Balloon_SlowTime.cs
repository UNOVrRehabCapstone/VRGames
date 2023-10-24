using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_SlowTime : Balloon_General
{
	public override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("DartPoint"))
        {
            base.OnTriggerEnter(other);
			SpecialBalloonManager.Instance.StartCoroutine(SpecialBalloonManager.Instance.SlowBalloonEffect());
        }
	}

	/* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public override void OnMouseDown()
    {   
		base.OnMouseDown();
		SpecialBalloonManager.Instance.StartCoroutine(SpecialBalloonManager.Instance.SlowBalloonEffect());
    }
}
