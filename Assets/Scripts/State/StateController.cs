using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.State
{
    /**
     * StateController.cs is responsible for managing all information relevant to the current global state
     * of the application — what mode the application is in and what specimens are selected.
     */
    public class StateController : MonoBehaviour
    {
        public LandingPage landingPage;
        public TrayPage trayPage;
        public AnalysisPage analysisPage;
        //public ComparisonMode comparisonMode;
        public SpecimenStore store;
        public MainCameraEvents cameraEvents; 

        public bool loadingPrimarySpecimen;
        public bool loadingCompareSpecimen;
        
        public Dictionary<ViewMode, IPage> modeToPage;

        Shader newunlit,standard,vertexColor;

        /**
        * Set mode to initiate transition between two modes.
        */
        public ViewMode mode
        {
            get => _mode;
            set
            {
                if (value == _mode) return;
                modeToPage[_mode].Deactivate();
                _mode = value;
                if (modeToPage.ContainsKey(_mode) && modeToPage[_mode] != null)
                {
                    modeToPage[_mode].Activate();
                }
            }
        }

        /**
        * Set CurrentSpecimenId to change current specimen
        */
        public string currentSpecimenId => CurrentSpecimenData?.id;

        // Mode on
        private ViewMode _mode;

        // Specimen on
        public SpecimenData CurrentSpecimenData;
        // The instantiated current specimen object
        public GameObject CurrentSpecimenObject; 

        public SpecimenData CompareSpecimenData;
        // The instantiated compare specimen object
        public GameObject CompareSpecimenObject;

        /**
         * Startup functions
         */
        private void Awake() {
            modeToPage = new Dictionary<ViewMode, IPage>
            {
                {ViewMode.LANDING, landingPage},
                {ViewMode.TRAY, trayPage },
                {ViewMode.ANALYSIS, analysisPage }
                //{ViewMode.COMPARE, comparisonMode }
            };
        }

        private void Start() {
            newunlit = Shader.Find("Unlit/NewUnlit");
            standard = Shader.Find("Standard");
            //vertexColor = Shader.Find("Custom/VertexColor");
            try {
                trayPage.Activate();
            } catch (Exception e) {
                Debug.LogWarning($"Error deactivating trayPage: {e}");
            }

            try {
                analysisPage.Deactivate();
            } catch (Exception e) {
                Debug.LogWarning($"Error deactivating analysisPage: {e}");
            }

            try {
                landingPage.Deactivate();
            } catch (Exception e) {
                Debug.LogWarning($"Error activating landingPage: {e}");
            }
        }

        /**
         * Public interface
         */
        public void RemovePrimarySpecimen() {
            CurrentSpecimenData = null;
            if (CurrentSpecimenObject != null)
            {
                Destroy(CurrentSpecimenObject);
            }

            if (CompareSpecimenData != null)
            {
                SwapSpecimens();
            }
            else
            {
                mode = ViewMode.TRAY;
            }

        }

        public void RemoveCompareSpecimen() {
            CompareSpecimenData = null;
            if (CompareSpecimenObject != null) { 
                Destroy(CompareSpecimenObject);
            }
        }

        public IEnumerator AddPrimarySpecimen(SpecimenData data, Action<GameObject> callback) {
            RemovePrimarySpecimen();
            CurrentSpecimenData = data;
            StartCoroutine(InstantiateSpecimen(data, true)); 
            while (loadingPrimarySpecimen) yield return null;
            callback(CurrentSpecimenObject);
           // analysisPage.ResetCameraPosition();
        }

        public IEnumerator AddCompareSpecimen(SpecimenData data, Action<GameObject> callback)
        {
            RemoveCompareSpecimen();
            CompareSpecimenData = data;
            CompareSpecimenObject = null;
            StartCoroutine(InstantiateSpecimen(data, false));
            while (loadingCompareSpecimen) yield return null;
            callback(CompareSpecimenObject);
        }

        public void SwapSpecimens()
        {
            SpecimenData tempData = CompareSpecimenData;
            GameObject tempObject = CompareSpecimenObject;

            CompareSpecimenData = CurrentSpecimenData;
            CompareSpecimenObject = CurrentSpecimenObject;

            CurrentSpecimenData = tempData;
            CurrentSpecimenObject = tempObject;
        }

        /**
         * Internal functions
         */
        private IEnumerator InstantiateSpecimen(SpecimenData data, bool primary) {
            GameObject spObj = null;

            if (!data.dataLoaded) {
                loadingPrimarySpecimen = primary;
                loadingCompareSpecimen = !primary;

                store.LoadSpecimen(data.id);
                while (!store.specimens[data.id].dataLoaded) yield return null;
                store.LoadingPopUp();
                Debug.Log("loading pop-up"); 
                data = store.specimens[data.id];
               
            }

            // If prefab found, instantiate that
            if (data.prefab != null) {
                spObj = Instantiate(data.prefab);
            } else {
                spObj = new GameObject(data.name);
                try {
                    spObj.AddComponent<MeshFilter>().mesh = data.mesh;
                    spObj.AddComponent<MeshRenderer>().material = data.material;
                    spObj.AddComponent<MeshCollider>();
                    spObj.layer = LayerMask.NameToLayer("Specimens");
                    //spObj.GetComponentInChildren<Renderer>().material.shader = newunlit;
                    Renderer[] rendererss = spObj.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in rendererss)
                    {
                        renderer.material.shader = standard ? standard : newunlit;
                    }

                } catch (Exception e) {
                    Debug.LogWarning(e);
                }
            }

            // Else fallback to old way using meshes and mats
            spObj.transform.localPosition = new Vector3(0, data.yPos, 0);
            spObj.transform.localScale = Vector3.one * data.scale;
            spObj.gameObject.SetActive(true);
            spObj.layer = 9;
            // Keep the one uses standard shader
            Renderer[] renderers = spObj.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in renderers)
            {
                renderer.material.shader = standard ? standard : newunlit;
            }
            //spObj.GetComponentInChildren<Renderer>().material.shader = standard ? standard : newunlit;
            //spObj.GetComponentInChildren<Renderer>().material.shader = standard ? standard : ( vertexColor ? vertexColor : newunlit);

            if (primary) {
                CurrentSpecimenObject = spObj;
                loadingPrimarySpecimen = false;
                
                
            } else {
                CompareSpecimenObject = spObj;
                loadingCompareSpecimen = false;
            }

            spObj.SetActive(true);

            yield break;
        }

    }
}