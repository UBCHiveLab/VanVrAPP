using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    /**
     * The StateController holds and manages transitions between the modes/views and different specimens.
     */
    public class StateController : MonoBehaviour
    {
        public LandingPage landingPage;
        public TrayPage trayPage;
        public AnalysisPage analysisPage;
  

        public Dictionary<ViewMode, IPage> modeToPage;



        /**
        * Set Mode to initiate transition between two modes.
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

        public void RemoveCurrentSpecimen() {
            CurrentSpecimenData = null;
            // TODO: trigger animations etc.
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
            // TODO: trigger animations etc.
            if (CompareSpecimenObject != null) { 
                Destroy(CompareSpecimenObject);
            }
        }

        public GameObject AddNewSpecimen(SpecimenData data) {
            RemoveCurrentSpecimen();
            CurrentSpecimenData = data;
            CurrentSpecimenObject = InstantiateSpecimen(data);
            CurrentSpecimenObject.gameObject.SetActive(true);
     
            return CurrentSpecimenObject;
            // TODO: trigger animations etc.
        }

        public GameObject AddCompareSpecimen(SpecimenData data)
        {
            RemoveCompareSpecimen();
            CompareSpecimenData = data;

            Debug.Log($"Specimen added: {data.id}");
            CompareSpecimenObject = InstantiateSpecimen(data);
            CompareSpecimenObject.gameObject.SetActive(true);

            return CompareSpecimenObject;
        }

        private GameObject InstantiateSpecimen(SpecimenData data)
        {
            GameObject spObj;
            // If prefab found, instantiate that
            if (data.prefab != null)
            {
                spObj = Instantiate(data.prefab);
            }
            else
            {
                spObj = new GameObject(data.name);
                try
                {
                    spObj.AddComponent<MeshFilter>().mesh = data.mesh;
                    spObj.AddComponent<MeshRenderer>().material = data.material;
                    spObj.AddComponent<MeshCollider>();
                    spObj.layer = LayerMask.NameToLayer("Specimens");
                    
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }


            }

            // Else fallback to old way using meshes and mats

            spObj.transform.localScale = Vector3.one * data.scale;
            spObj.gameObject.SetActive(true);
            spObj.AddComponent<SpecimenOptions>();
            spObj.layer = 9;
            return spObj;

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


        void Awake()
        {
            modeToPage = new Dictionary<ViewMode, IPage>
            {
                {ViewMode.LANDING, landingPage},
                {ViewMode.TRAY, trayPage },
                {ViewMode.ANALYSIS, analysisPage }
            };
        }

        void Start()
        {
            try {
                trayPage.Deactivate();
            } catch (Exception e) {
                Debug.LogWarning($"Error deactivating trayPage: {e}");
            }

            try {
                analysisPage.Deactivate();
            } catch (Exception e) {
                Debug.LogWarning($"Error deactivating analysisPage: {e}");
            }

            try {
                landingPage.Activate();
            } catch (Exception e) {
                Debug.LogWarning($"Error activating landingPage: {e}");
            }


        }


    }
}