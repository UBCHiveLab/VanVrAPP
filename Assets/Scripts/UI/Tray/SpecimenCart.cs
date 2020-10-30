using UnityEngine;

public class SpecimenCart : MonoBehaviour
{
    public GameObject trayPrefab;
    public Vector3 singleTrayLocalOffset = new Vector3(-0.25f, 2.25f, 0.5f);
    public Vector3 compareTray1LocalOffset = new Vector3(-0.75f, 2.25f, 0.5f);
    public Vector3 compareTray2LocalOffset = new Vector3(0.5f, 2.25f, 0.5f);
    //public Vector3 newcomparisonmodeTray1Offset = new Vector3(-0.25f, 2.25f, 0.5f);
    //public Vector3 newcomparisonmodeTray2Offset = new Vector3(-1.50f, 2.25f, 0.5f);
    public GameObject tray1;
    public GameObject tray2;
    public Vector3 specimenTrayOffset = new Vector3(0, 0, 0.005f);
    public bool hidden;

    void OnEnable()
    {
        SpawnTray1();
    }

    public void AddSpecimenPrimary(GameObject specimen)
    {
        AddSpecimen(specimen, tray1);
    }

    public void AddSpecimenCompare(GameObject specimen)
    {
        AddSpecimen(specimen, tray2);
    }

    public void AddSpecimen(GameObject specimen, GameObject tray)
    {
        ClearTray(tray);
        if (specimen != null)
        {
            Vector3 specimenOldPos = specimen.transform.localPosition;
            GameObject holder = new GameObject("SpecimenHolder");

            holder.transform.SetParent(tray.transform);
            holder.transform.localPosition = specimenTrayOffset;
            specimen.transform.SetParent(holder.transform);

            // make the specimen's local position, it's position before the holder was made its parent
            specimen.transform.localPosition = specimenOldPos;
        }
    }

    public void SpawnTray1() {
        if (tray1 != null) return;
        tray1 = Instantiate(trayPrefab, transform);
        if (tray2 == null) {
            tray1.transform.localPosition = singleTrayLocalOffset;
        } else {
            tray1.transform.localPosition = compareTray1LocalOffset;
        }

        if (hidden)
        {
            tray1.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void SpawnTray2() {
        if (tray2 != null) return;
        tray2 = Instantiate(trayPrefab, transform);
        tray2.transform.localPosition = compareTray2LocalOffset;
        tray1.transform.localPosition = compareTray1LocalOffset;

        if (hidden) {
            tray2.GetComponent<MeshRenderer>().enabled = false;
        }
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

    //the specimen positions in new comparison mode
    public void ComprisonModePosition()
    {
        if (tray2 == null) {
            Debug.Log("no trAy2");
            return;

        }
        tray1.transform.localPosition = new Vector3(-0.25f, 2.25f, 0.5f);
        //Debug.Log("tray1");
        tray2.transform.localPosition = new Vector3(-1.50f, 2.25f, 0.5f);
        //Debug.Log("tray2");
    }

    // Resets a specimen location in terms of its tray offset.
    public void ResetPosition(GameObject specimen)
    {
        specimen.transform.localPosition = Vector3.zero;
    }

    public void SetTrayVisibility(bool hide)
    {
        hidden = hide;
        tray1.GetComponent<MeshRenderer>().enabled = !hidden;
        if (tray2 != null)
        {
            tray2.GetComponent<MeshRenderer>().enabled = !hidden;
        }
    }

    private void ClearTray(GameObject tray)
    {
        foreach (Transform child in tray.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
