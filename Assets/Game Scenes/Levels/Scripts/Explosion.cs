using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem Main;
	public ParticleSystem Radial;

    public void Play()
    {
//		var Mainfx = Main.main;
//		Mainfx.loop = true;
//		var Radialfx = Radial.main;
//		Radialfx.loop = true;
        Main.Play();
		Radial.Play ();
    }

	public void Stop()	{
		var Mainfx = Main.main;
		Mainfx.loop = false;
		var Radialfx = Radial.main;
		Radialfx.loop = false;
	}
}

