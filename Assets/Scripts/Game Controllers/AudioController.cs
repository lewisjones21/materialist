using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour {

	static AudioController instance;

    public float menuVolume = 0.5f, gameVolume = 0.3f;
#if !UNITY_WEBGL
    public float menuPitch = -0.8f, gamePitch = 1.0f, quitPitch = -0.5f;
#else
    public float menuPitch = 0.4f, gamePitch = 1.0f, quitPitch = 0.1f;
#endif

    private AudioSource audioSourceMusic, audioSourceEvaporate, audioSourceSolidify;
    public float fadeFactor = 0.02f, sfxFadeFactor = 0.35f;
    private float stayFactor, sfxStayFactor;

    float targetEvaporationVolume = 0.0f;
    float targetSolidificationVolume = 0.0f;

    public int updates = 0;

	void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            AudioSource[] sources = GetComponents<AudioSource>();
            audioSourceMusic = sources[0];
            audioSourceEvaporate = sources[1];
            audioSourceSolidify = sources[2];
            stayFactor = 1 - fadeFactor;
            sfxStayFactor = 1 - sfxFadeFactor;

            SceneManager.sceneLoaded += HandleSceneLoaded;

            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }
	}
	
	public void OnApplicationQuit()
	{
		instance = null;
		Destroy(this);
	}

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        updates = 0;
    }

    void Update()
    {
        if (MenuController.inMenu)
        {
            audioSourceMusic.volume = audioSourceMusic.volume * stayFactor + menuVolume * fadeFactor;
            if (!SceneManager.GetActiveScene().name.Contains("Quit"))
            {
                audioSourceMusic.pitch = audioSourceMusic.pitch * stayFactor + menuPitch * fadeFactor;
            }
            else
            {
                audioSourceMusic.pitch = audioSourceMusic.pitch * stayFactor + quitPitch * fadeFactor;
            }
        }
        else
        {
            audioSourceMusic.volume = audioSourceMusic.volume * stayFactor + gameVolume * fadeFactor;
            audioSourceMusic.pitch = audioSourceMusic.pitch * stayFactor + gamePitch * fadeFactor;
        }
        audioSourceEvaporate.volume = audioSourceEvaporate.volume * sfxStayFactor + targetEvaporationVolume * sfxFadeFactor;
        if (audioSourceEvaporate.volume < 0.01f && audioSourceEvaporate.isPlaying)
        {
            audioSourceEvaporate.Stop();
        }
        if (audioSourceEvaporate.volume > 0.01f && !audioSourceEvaporate.isPlaying)
        {
            audioSourceEvaporate.Play();
        }
        targetEvaporationVolume *= 0.95f;
        audioSourceEvaporate.panStereo *= 0.975f;

        audioSourceSolidify.volume = audioSourceSolidify.volume * sfxStayFactor + targetSolidificationVolume * sfxFadeFactor;
        if (audioSourceSolidify.volume < 0.01f && audioSourceSolidify.isPlaying)
        {
            audioSourceSolidify.Stop();
        }
        if (audioSourceSolidify.volume > 0.01f && !audioSourceSolidify.isPlaying)
        {
            audioSourceSolidify.Play();
        }
        targetSolidificationVolume *= 0.95f;
        audioSourceSolidify.panStereo *= 0.975f;

        updates++;
    }

    public static void IncreaseEvaporationVolume(float deltaPan = 0.0f)
    {
        if (instance != null && instance.updates > 30)
        {
            instance.targetEvaporationVolume += 0.3f;
            instance.audioSourceEvaporate.panStereo += deltaPan;
        }
    }
    public static void IncreaseSolidificationVolume(int type = 0, float deltaPan = 0.0f)
    {
        if (instance != null && instance.updates > 30)
        {
            instance.targetSolidificationVolume += 0.2f;
            instance.audioSourceSolidify.pitch = instance.audioSourceSolidify.pitch * 0.8f + (1.0f + type) * 0.2f;
            instance.audioSourceSolidify.panStereo += deltaPan;
        }
    }
}
