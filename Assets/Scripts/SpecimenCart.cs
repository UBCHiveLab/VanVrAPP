using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenCart : MonoBehaviour
{

    public GameObject trayPrefab;
    public Vector3 singleTrayLocalOffset = new Vector3(-0.25f, 2.25f, 0.5f);
    public Vector3 compareTray1LocalOffset = new Vector3(-0.75f, 2.25f, 0.5f);
    public Vector3 compareTray2LocalOffset = new Vector3(0.5f, 2.25f, 0.5f);
    public GameObject tray1;
    public GameObject tray2;
    public Vector3 specimenTrayOffset = new Vector3(0, 0, 0.005f);

    void Start()
    {
        SpawnTray1();
    }

    public void AddSpecimenPrimary(GameObject specimen)
    {
        specimen.transform.SetParent(tray1.transform);
        ResetPosition(specimen);
    }

    public void AddSpecimenCompare(GameObject specimen)
    {
        specimen.transform.SetParent(tray2.transform);
        ResetPosition(specimen);
    }

    public void SpawnTray1() {
        if (tray1 != null) return;
        tray1 = Instantiate(trayPrefab, transform);
        if (tray2 == null) {
            tray1.transform.localPosition = singleTrayLocalOffset;
        } else {
            tray1.transform.localPosition = compareTray1LocalOffset;
        }
    }

    public void SpawnTray2() {
        if (tray2 != null) return;
        tray2 = Instantiate(trayPrefab, transform);
        tray2.transform.localPosition = compareTray2LocalOffset;
        tray1.transform.localPosition = compareTray1LocalOffset;
    }

    public void RemoveTray1() {
        Destroy(tray1);
        tray1 = tray2;
        tray1.transform.localPosition = singleTrayLocalOffset;
        tray2 = null;
    }

    public void RemoveTray2() {
        Destroy(tray2);
        tray2 = null;
        tray1.transform.localPosition = singleTrayLocalOffset;
    }

    public void ResetPosition(GameObject specimen)
    {
        specimen.transform.localPosition = specimenTrayOffset;
    }
}
