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
        public string currentSpecimenId => _currentSpecimenData?.Id;

        private ViewMode _mode;
        private string _currentSpecimenId;
        private SpecimenData _currentSpecimenData;
        public GameObject CurrentSpecimenObject;

        public SpecimenData GetCurrentSpecimenData()
        {
            return _currentSpecimenData;
        }

        public void RemoveCurrentSpecimen() {
            _currentSpecimenId = null;
            // TODO: trigger animations etc.
            if (CurrentSpecimenObject != null)
            {
                Destroy(CurrentSpecimenObject);
            }
        }

        public void AddNewSpecimen(SpecimenData data) {
            RemoveCurrentSpecimen();
            _currentSpecimenData = data;
            Debug.Log($"Specimen added: {data.Id}");
            CurrentSpecimenObject = Instantiate(data.Prefab);
            CurrentSpecimenObject.gameObject.transform.localScale = new Vector3(7, 7, 7);
            CurrentSpecimenObject.gameObject.SetActive(true);
            CurrentSpecimenObject.transform.position = new Vector3(0.5f, 2, 14);

            // TODO: trigger animations etc.
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