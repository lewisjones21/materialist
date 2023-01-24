using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static string startMenuName = "Start Menu",
        manipulationSelectMenuName = "Manipulation Select Menu",
        destructionSelectMenuName = "Destruction Select Menu",
        constructionSelectMenuName = "Construction Select Menu",
        helpMenuName = "Help Menu",
        aboutMenuName = "About Menu",
        quitMenuName = "Quit Menu",
        websiteText = "http://divf.eng.cam.ac.uk/gam2eng/Main/WebHome";

    public static bool inMenu;

    void Start()
    {
        Cursor.visible = true;
        inMenu = SceneManager.GetActiveScene().name.Contains("Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == startMenuName)
            {
                GoToQuitMenu();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Menu"))
            {
                GoToStartMenu();
            }
            else
            {
                if (SceneManager.GetActiveScene().name.Contains("Manip"))//Manipulation
                {
                    GoToManipulationSelectMenu();
                }
                else if (SceneManager.GetActiveScene().name.Contains("Destr"))//Destruction
                {
                    GoToDestructionSelectMenu();
                }
                else if (SceneManager.GetActiveScene().name.Contains("Const"))//Construction
                {
                    GoToConstructionSelectMenu();
                }
                else
                {
                    GoToStartMenu();
                }
            }
        }
    }

    public static void LoadLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            // Disable all button sounds
            foreach (ButtonController buttonContoller in FindObjectsOfType<ButtonController>())
            {
                buttonContoller.enabled = false;
            }

            inMenu = levelName.Contains("Menu");
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("Attempted to load non-existent level: " + levelName + " (it may need to be added to the build settings)");
        }
    }

    public void GoToStartMenu()
    {
        LoadLevel(startMenuName);
    }
    public void GoToManipulationSelectMenu()
    {
        LoadLevel(manipulationSelectMenuName);
    }
    public void GoToDestructionSelectMenu()
    {
        LoadLevel(destructionSelectMenuName);
    }
    public void GoToConstructionSelectMenu()
    {
        LoadLevel(constructionSelectMenuName);
    }
    public void GoToHelpMenu()
    {
        LoadLevel(helpMenuName);
    }
    public void GoToAboutMenu()
    {
        LoadLevel(aboutMenuName);
    }
    public void GoToQuitMenu()
    {
        LoadLevel(quitMenuName);
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
