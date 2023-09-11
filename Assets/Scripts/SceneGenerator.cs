using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SceneGenerator : MonoBehaviour
{
	public bool isDay;
	public bool isOcean;
	public bool isRain;

	public Cubemap daySkyboxMaterial;
	public Cubemap nightSkyboxMaterial;

	public GameObject ocean;
	public GameObject city;
	public GameObject rain;

    public Volume skybox;

    // Start is called before the first frame update
    void Update()
	{
		isDay = MixedUp.Client.day;
		isOcean = MixedUp.Client.ocean;
		isRain = MixedUp.Client.rain;

        //skybox.profile.TryGet(out sky);
        //if (isDay)
        //{
        //    sky.hdriSky.Override(daySkyboxMaterial);
        //    sky.exposure.value = 0f;
        //}
        //else
        //{
        //    sky.hdriSky.Override(nightSkyboxMaterial);
        //    sky.exposure.value = -1.50f;
        //}
        if (isOcean)
		{
			//Debug.Log("OceanSpawned, no City");
			ocean.SetActive(true);
			city.SetActive(false);
		}
		else
		{
			ocean.SetActive(false);
			city.SetActive(true);
			//Debug.Log("NoOcean, just city ");
		}
		if (isRain)
		{
			//.Log("isRaining");
			rain.SetActive(true);
		}
		else
		{
			//.Log("NotRaining");
			rain.SetActive(false);
		}

	}
}
