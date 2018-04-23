using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallManager : MonoBehaviour {

    public Stack<GameObject> unassigned;
    public List<GameObject> targets;
    public List<GameObject> distractors;

    public int numTotal = 8;
    public int numTargets = 3;
    public Color distractorColor = Color.white;
    public Color targetColor = Color.red;
    public GameObject Prefab;

    public int[] sendInfo;
    public ToLSL LSLManager;

    // Use this for initialization
    void Start () {
        sendInfo = new int[1];
        unassigned = new Stack<GameObject>();
        targets = new List<GameObject>();
        distractors = new List<GameObject>();
        InstantiateBalls();
        StartCoroutine(Disappear());

	}
	
	// Update is called once per frame
	void Update () {

	}

    private void InstantiateBalls()
    {
        List<Vector3> initialPositions = new List<Vector3>();
        while (unassigned.Count < numTotal)
        {
            bool overlap = false;
            var position = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 10);
            foreach (Vector3 pos in initialPositions)
                {
                if (Vector3.Distance(position, pos) <= Prefab.GetComponent<SphereCollider>().radius)
                    overlap = true;
                }
            if (!overlap)
            {
                initialPositions.Add(position);
                GameObject newBall = Instantiate(Prefab, position, Quaternion.identity);
                newBall.transform.parent = gameObject.transform;
                unassigned.Push(newBall);
            }
        }
         
    }

    private IEnumerator Disappear()
    {

        AddTargets(numTargets);
        sendInfo[0] = 5;
        LSLManager.pushSample();
        AddDistractors();
        yield return new WaitForSeconds(3f);
        HideTargets();
        sendInfo[0] = 3;
        LSLManager.pushSample();
        yield return new WaitForSeconds(8f);
        RemoveTarget();
        sendInfo[0] = 2;
        LSLManager.pushSample();
        yield return new WaitForSeconds(5f);
        RemoveTarget();
        sendInfo[0] = 1;
        LSLManager.pushSample();
        yield return new WaitForSeconds(5f);
        RemoveTarget();
        sendInfo[0] = 0;
        LSLManager.pushSample();
        yield return new WaitForSeconds(5f);
    }

    private void AddTargets(int num)
    {
        for (int i = 0; i < num; i++)
        {
            targets.Add(unassigned.Pop());
            targets.Last().GetComponent<Renderer>().material.color = Color.red;
            targets.Last().tag = "Target";
        }
    }

    private void AddDistractors()
    {
        while (unassigned.Count != 0)
            distractors.Add(unassigned.Pop());
    }

    private void HideTargets()
    { 
        foreach(GameObject target in targets)
            target.GetComponent<Renderer>().material.color = distractorColor;
    }

    private void RemoveTarget()
    {
        Destroy(targets.Last());
        targets.Remove(targets.Last());
    }

    private void RemoveDistractor()
    {

    }

}
