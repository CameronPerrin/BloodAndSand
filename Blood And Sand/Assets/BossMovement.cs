using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

// At start, set all portals to true, spawn boss at random portal, set portal that is occupied to false;
// Once player hits boss, start timer
// After timer, hide all enemies (scale down to 0 or renderer = false)
// Set all portals to true;

public class BossMovement : MonoBehaviour
{
    private GameObject[] bosses;
    public GameObject mainBoss;
    private GameObject portalController;
    public float teleportTime;
    private float teleportTimer;
    private bool hasTakenDamage;
    private bool collisionOccured;
    public bool cloneDamageTaken;
    int spawnLocation;

    private void Awake()
    {
        portalController = GameObject.Find("BossTeleportationController");
    }

    void Start()
    { 
        teleportTimer = teleportTime;
        spawnLocation = Random.Range(0, portalController.GetComponent<BossTeleportationController>().portals.Length - 1);
        // Causes the index out of range error, will fix later
        mainBoss.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(portalController.GetComponent<BossTeleportationController>().portals[spawnLocation].transform.position);
        portalController.GetComponent<BossTeleportationController>().portals[spawnLocation].GetComponent<BossPortal>().isVacant = false;
        //portal.boss = GameObject.FindGameObjectsWithTag("Boss");
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTakenDamage || cloneDamageTaken)
        {
            WaitUntilTeleport();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collisionOccured)
        {
            return;
        }
        if(collision.gameObject.tag == "Bullet" && !hasTakenDamage)
        {
            hasTakenDamage = true;
            collisionOccured = true;

        }
    }

    private void WaitUntilTeleport()
    {
        teleportTimer -= Time.deltaTime;
        if (teleportTimer <= 0f)
        {
            teleportTimer = teleportTime;
            bosses = GameObject.FindGameObjectsWithTag("Boss");
            portalController.GetComponent<BossTeleportationController>().boss = GameObject.FindGameObjectsWithTag("Boss");

            for(int i = 0; i < bosses.Length; i++)
            {
                bosses[i].transform.localScale = new Vector3(0, 0, 0);
            }
            
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(1);

        // Set all portals to vacant since All bosses teleports out.
        portalController.GetComponent<BossTeleportationController>().setAllPortalsVacant();

        List<int> uniqueNumbers = new List<int>();
        List<int> portalIndexes = new List<int>();

        for (int i = 0; i < portalController.GetComponent<BossTeleportationController>().portals.Length; i++)
        {
            uniqueNumbers.Add(i);
        }

        for (int i = 0; i < portalController.GetComponent<BossTeleportationController>().portals.Length; i++)
        {
            int ranNum = uniqueNumbers[Random.Range(0, uniqueNumbers.Count)];
            portalIndexes.Add(ranNum);
            uniqueNumbers.Remove(ranNum);
        }
        Debug.Log("Boss Length: " + portalController.GetComponent<BossTeleportationController>().boss.Length);
        for (int j = 0; j < portalController.GetComponent<BossTeleportationController>().boss.Length; j++)
            {
              Debug.Log("Iterating through boss length.");
              if (portalController.GetComponent<BossTeleportationController>().portals[portalIndexes[j]].GetComponent<BossPortal>().isVacant == true)
                  {
                        Debug.Log("Teleporting to portal...: " + portalIndexes[j]);
                        portalController.GetComponent<BossTeleportationController>().portals[portalIndexes[j]].GetComponent<BossPortal>().isVacant = false;

                for (int i = 0; i < bosses.Length; i++)
                {
                    bosses[i].transform.localScale = new Vector3(1, 1, 1);
                }
                        portalController.GetComponent<BossTeleportationController>().boss[j].GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(portalController.GetComponent<BossTeleportationController>().portals[portalIndexes[j]].transform.position);
                  }
            }
        //portalIndexes.Clear();
        hasTakenDamage = false;
        collisionOccured = false;
        cloneDamageTaken = false;
    }
}
