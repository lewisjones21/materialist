using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleBondManager : MonoBehaviour {

    public static ParticleBondManager instance;

    public GameObject bondPrefab;
    public static GameObject staticBondPrefab;

    public static Queue<ParticleBond> bonds;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            staticBondPrefab = bondPrefab;
            bonds = new Queue<ParticleBond>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StoreBond(ParticleBond bond)
    {
        if (bond != null)
        {
            //bond.lr.enabled = false;
            bond.lr.SetPosition(0, Vector3.zero);
            bond.lr.SetPosition(1, Vector3.zero);
            bond.gameObject.SetActive(false);
            bonds.Enqueue(bond);
        }
    }

    public static ParticleBond RetrieveBond()
    {
        ParticleBond bond;
        if (bonds.Count > 0)
        {
            bond = bonds.Dequeue();
            bond.gameObject.SetActive(true);
        }
        else
        {
            bond = ((GameObject)Instantiate(staticBondPrefab, Vector2.zero, Quaternion.identity)).GetComponent<ParticleBond>();
        }
        //bond.lr.enabled = true;
        return bond;
    }
}
