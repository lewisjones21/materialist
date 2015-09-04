using UnityEngine;
using System.Collections;

public class LevelObjective : MonoBehaviour {

    protected bool isComplete = false;
    public TextMesh objectiveText;
    public Color winTextColour;
    protected Color defaultTextColour;

	protected virtual void Start()
    {
        isComplete = false;
        if (objectiveText != null)
        {
            defaultTextColour = objectiveText.color;
        }
	}
	
	void Update()
    {
	    
	}

    public virtual bool GetIsComplete()
    {
        if (objectiveText != null)
        {
            if (isComplete)
            {
                objectiveText.color = winTextColour;
            }
            else
            {
                objectiveText.color = defaultTextColour;
            }
        }
        return isComplete;
    }
}
