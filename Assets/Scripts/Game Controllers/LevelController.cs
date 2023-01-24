using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    AudioSource objectiveAudioSource;

    public List<LevelObjective> objectives;
    public AudioClip winSound;
    int numberComplete, numberToComplete;
    public bool levelComplete = false;
    public float completionDelay = 3.0f;
    float currentCompletionDelay;
    bool havePlayedWinSound;

    CanvasGroup winPanelGroup;

	void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("Screen"))
        {
            Destroy(gameObject);
            return;
        }

        objectiveAudioSource = GetComponent<AudioSource>();

        levelComplete = false;

        Canvas[] canvasses = GameObject.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvasses)
        {
            canvas.enabled = true;
        }

        winPanelGroup = GameObject.Find("Win Panel").GetComponent<CanvasGroup>();
        winPanelGroup.alpha = 0.0f;

        havePlayedWinSound = false;
	}
	
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (!levelComplete)
        {
            int lastNumberComplete = numberComplete;
            numberComplete = 0;
            numberToComplete = objectives.Count;
            foreach (LevelObjective objective in objectives)
            {
                if (objective.GetIsComplete())
                {
                    numberComplete++;
                }
            }
            levelComplete = (numberComplete == numberToComplete);

            //Play objective sound if an objective has been completed or uncompleted
            if (lastNumberComplete < numberComplete)
            {
                objectiveAudioSource.timeSamples = 0;
                objectiveAudioSource.pitch = 0.5f + numberComplete * 0.1f;
                objectiveAudioSource.Play();
            }
            if (lastNumberComplete > numberComplete)
            {
                objectiveAudioSource.timeSamples = objectiveAudioSource.clip.samples - 1;
                objectiveAudioSource.pitch = -0.5f - numberComplete * 0.1f;
                objectiveAudioSource.Play();
            }
        }
        if (objectives.Count == 0) levelComplete = false;
        if (!levelComplete) currentCompletionDelay = -completionDelay * 0.75f;


        if (levelComplete)
        {
            if (currentCompletionDelay < 0.0f)
            {
                currentCompletionDelay += Time.deltaTime;
                levelComplete = false;
            }
        }

        if (levelComplete)
        {
            if (!havePlayedWinSound)
            {
                AudioSource.PlayClipAtPoint(winSound, Vector3.zero, 0.65f);
                havePlayedWinSound = true;
            }
            winPanelGroup.alpha = 1.0f - (currentCompletionDelay / completionDelay);
            currentCompletionDelay += Time.deltaTime;
        }
        if (currentCompletionDelay > completionDelay)
        {
            GoToNextLevel();
        }
	}

    void GoToNextLevel()
    {
        string currentLevelName = SceneManager.GetActiveScene().name;
        int currentLevelNumber = 0;
        //Parse number from name after removing letters:
        int.TryParse(Regex.Replace(currentLevelName, "[^0-9]", ""), out currentLevelNumber);
        currentLevelNumber++;//Increment the extracted level number
        string currentLevelTypeName = Regex.Replace(currentLevelName, "[0-9]", "");//Parse root name after removing numbers
        currentLevelName = currentLevelTypeName + currentLevelNumber.ToString();//Combine type name and incremented number
        if (!Application.CanStreamedLevelBeLoaded(currentLevelName))//If the next numbered level doesn't exist
        {
            currentLevelName = currentLevelTypeName + "Select Screen";//Go back to the selection screen
        }
        MenuController.LoadLevel(currentLevelName);//Recombine name and incremented number
    }
}
