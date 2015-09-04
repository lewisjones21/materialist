using UnityEngine;
using System.Collections;

public class ConnectionObjective : LevelObjective {

    public ElectrodeController negativeElectrode;

	protected override void Start()
    {
        base.Start();
	}

    public override bool GetIsComplete()
    {
        isComplete = negativeElectrode.isConnected;
        return base.GetIsComplete();
    }
}
