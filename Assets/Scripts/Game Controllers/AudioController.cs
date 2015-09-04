using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	static AudioController instance;

    public float menuVolume = 0.5f, gameVolume = 0.3f;
    public float menuPitch = -0.8f, gamePitch = 1.0f, quitPitch = -0.5f;

    private AudioSource[] sources;
    public float fadeFactor = 0.02f, sfxFadeFactor = 0.2f;
    private float stayFactor, sfxStayFactor;

    float targetEvaporationVolume = 0.0f;

	void Awake()
    {
        //Debug.Log(instance);
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Assigning instance of Audio Controller");
            instance = this;
            transform.SetParent(null);
            sources = GetComponents<AudioSource>();
            stayFactor = 1 - fadeFactor;
            sfxStayFactor = 1 - sfxFadeFactor;
            DontDestroyOnLoad(this);
        }
	}
	
	public void OnApplicationQuit()
	{
		//Debug.Log("Audio Controller destroyed");
		instance = null;
		Destroy(this);
	}

    void Update()
    {
        if (MenuController.inMenu)
        {
            sources[0].volume = sources[0].volume * stayFactor + menuVolume * fadeFactor;
            if (!Application.loadedLevelName.Contains("Quit"))
            {
                sources[0].pitch = sources[0].pitch * stayFactor + menuPitch * fadeFactor;
            }
            else
            {
                sources[0].pitch = sources[0].pitch * stayFactor + quitPitch * fadeFactor;
            }
        }
        else
        {
            sources[0].volume = sources[0].volume * stayFactor + gameVolume * fadeFactor;
            sources[0].pitch = sources[0].pitch * stayFactor + gamePitch * fadeFactor;
        }
        targetEvaporationVolume *= 0.95f;
        sources[1].volume = sources[1].volume * sfxStayFactor + targetEvaporationVolume * sfxFadeFactor;
        if (sources[1].volume < 0.01f)
        {
            sources[1].Stop();
        }
        if (sources[1].volume > 0.01f && !sources[1].isPlaying)
        {
            sources[1].Play();
        }
    }

    public static void PlayEvaporationSound()
    {
        if (instance != null)
        {
            instance.targetEvaporationVolume += 0.4f;
        }
    }
}
