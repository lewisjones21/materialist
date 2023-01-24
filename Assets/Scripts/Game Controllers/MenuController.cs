using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static string startScreenName = "Start Screen",
        manipulationSelectScreenName = "Manipulation Select Screen",
        destructionSelectScreenName = "Destruction Select Screen",
        constructionSelectScreenName = "Construction Select Screen",
        helpScreenName = "Help Screen",
        aboutScreenName = "About Screen",
        quitScreenName = "Quit Screen",
        websiteText = "http://divf.eng.cam.ac.uk/gam2eng/Main/WebHome";

    public static bool inMenu;

    void Start()
    {
        Cursor.visible = true;
        inMenu = SceneManager.GetActiveScene().name.Contains("Screen");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == startScreenName)
            {
                GoToQuitScreen();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Screen"))
            {
                GoToStartScreen();
            }
            else
            {
                if (SceneManager.GetActiveScene().name.Contains("Manip"))//Manipulation
                {
                    GoToManipulationSelectScreen();
                }
                else if (SceneManager.GetActiveScene().name.Contains("Destr"))//Destruction
                {
                    GoToDestructionSelectScreen();
                }
                else if (SceneManager.GetActiveScene().name.Contains("Const"))//Construction
                {
                    GoToConstructionSelectScreen();
                }
                else
                {
                    GoToStartScreen();
                }
            }
        }
    }

    public static void LoadLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            inMenu = levelName.Contains("Screen");
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("Attempted to load non-existent level: " + levelName + " (it may need to be added to the build settings)");
        }
    }

    public void GoToStartScreen()
    {
        LoadLevel(startScreenName);
    }
    public void GoToManipulationSelectScreen()
    {
        LoadLevel(manipulationSelectScreenName);
    }
    public void GoToDestructionSelectScreen()
    {
        LoadLevel(destructionSelectScreenName);
    }
    public void GoToConstructionSelectScreen()
    {
        LoadLevel(constructionSelectScreenName);
    }
    public void GoToHelpScreen()
    {
        LoadLevel(helpScreenName);
    }
    public void GoToAboutScreen()
    {
        LoadLevel(aboutScreenName);
    }
    public void GoToQuitScreen()
    {
        LoadLevel(quitScreenName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartLevel(GameObject callingButton)
    {
        string levelName = callingButton.GetComponentInChildren<Text>().text;
        LoadLevel(levelName);
    }
    public void CopyWebsiteText()
    {
        //To be filled in at some point maybe?
    }
}
