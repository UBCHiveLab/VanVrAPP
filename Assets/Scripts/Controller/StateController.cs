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

        private readonly Vector3 _primarySpecimenPosition = new Vector3(0.5f, 2, 14);
        private readonly Vector3 _compareSpecimenPosition = new Vector3(1f, 2, 14);

        /**
        * Set Mode to initiate transition between two modes.
        */
        public ViewMode mode
        {
            get => _mode;
            set
            {
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
        public string currentSpecimenId => CurrentSpecimenData?.Id;

        // Mode state
        private ViewMode _mode;

        // Specimen state
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

        public void AddNewSpecimen(SpecimenData data) {
            RemoveCurrentSpecimen();
            CurrentSpecimenData = data;
            Debug.Log($"Specimen added: {data.Id}");
            CurrentSpecimenObject = Instantiate(data.Prefab);
            CurrentSpecimenObject.gameObject.SetActive(true);
            // TODO: actually child to tray object and offset
            CurrentSpecimenObject.transform.position = _primarySpecimenPosition;

            // TODO: trigger animations etc.
        }

        public void AddCompareSpecimen(SpecimenData data)
        {
            RemoveCompareSpecimen();
            CompareSpecimenData = data;
            Debug.Log($"Specimen added: {data.Id}");  
            CompareSpecimenObject = Instantiate(data.Prefab);
            CompareSpecimenObject.gameObject.SetActive(true);
            // TODO: actually child to tray object and offset

            CompareSpecimenObject.transform.position = _compareSpecimenPosition;
        }

        public void SwapSpecimens()
        {
            SpecimenData tempData = CompareSpecimenData;
            GameObject tempObject = CompareSpecimenObject;

            CompareSpecimenData = CurrentSpecimenData;
            CompareSpecimenObject = CurrentSpecimenObject;

            CurrentSpecimenData = tempData;
            CurrentSpecimenObject = tempObject;

            CurrentSpecimenObject.transform.position = _primarySpecimenPosition;
            CompareSpecimenObject.transform.position = _compareSpecimenPosition;
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