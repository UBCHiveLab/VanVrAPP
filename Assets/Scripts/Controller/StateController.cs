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
        public string currentSpecimenId
        {
            get => _currentSpecimenId;
            set
            {
                RemoveCurrentSpecimen();
                AddNewSpecimen(value);
            }
        }

        private ViewMode _mode;
        private string _currentSpecimenId;

        void Awake()
        {
            modeToPage = new Dictionary<ViewMode, IPage>
            {
                {ViewMode.LANDING, landingPage},
                {ViewMode.TRAY, trayPage },
                {ViewMode.ANALYSIS, analysisPage }
            };
        }

        private void RemoveCurrentSpecimen()
        {
            _currentSpecimenId = null;
            // TODO: trigger animations etc.
        }

        private void AddNewSpecimen(string id)
        {
            _currentSpecimenId = id;
            // TODO: trigger animations etc.
        }
    }
}