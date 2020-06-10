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
        public IPage trayPage;
        public IPage analysisPage;

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
        private GameObject _currentSpecimenObject;

        public SpecimenData GetCurrentSpecimenData()
        {
            return _currentSpecimenData;
        }

        public void RemoveCurrentSpecimen() {
            _currentSpecimenId = null;
            // TODO: trigger animations etc.
            if (_currentSpecimenObject != null)
            {
                Destroy(_currentSpecimenObject);
            }
        }

        public void AddNewSpecimen(SpecimenData data) {
            RemoveCurrentSpecimen();
            _currentSpecimenData = data;
            Debug.Log($"Specimen added: {data.Id}");
            _currentSpecimenObject = Instantiate(data.Prefab);
            _currentSpecimenObject.gameObject.SetActive(true);
            _currentSpecimenObject.transform.position = new Vector3(0, 2, 14);

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


    }
}