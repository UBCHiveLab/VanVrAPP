using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    /**
     * The StateController holds and manages transitions between the modes/views and different specimens.
     */
    public class StateController : MonoBehaviour
    {
        public float TransitionSpeed;
        public List<ViewModeData> ViewModes;

        /**
        * Set Mode to initiate transition between two modes.
        */
        public ViewMode Mode
        {
            get => _mode;
            set => StartModeTransition(_mode, value);
        }

        /**
        * Set CurrentSpecimenId to change current specimen
        */
        public string CurrentSpecimenId
        {
            get => _currentSpecimenId;
            set
            {
                RemoveCurrentSpecimen();
                AddNewSpecimen(value);
            }
        }

        private ViewMode _mode;
        private ViewModeData _fromData;
        private ViewModeData _toData;
        private string _currentSpecimenId;

        private void StartModeTransition(ViewMode from, ViewMode to)
        {
            if (_fromData != null || _toData != null) return; // Still in transition.

            _fromData = ViewModes.FirstOrDefault(m => m.Mode == from);
            _toData = ViewModes.FirstOrDefault(m => m.Mode == to);

            if (_fromData == null || _toData == null)
            {
                throw new Exception(
                    "Undefined view mode. Make sure all modes are defined in StateController.ViewModes");
            }


            StartCoroutine(ModeTransitioning());
        }

        private IEnumerator ModeTransitioning()
        {
            float progress = 0f;

            while (progress < 1f)
            {
                Camera.main.transform.position =
                    Vector3.Lerp(_fromData.CameraPosition, _toData.CameraPosition, progress);
                Camera.main.transform.rotation =
                    Quaternion.Slerp(Quaternion.Euler(_fromData.CameraEulerRotation),
                        Quaternion.Euler(_toData.CameraEulerRotation), progress);
                // TODO: add any other animations here.

                progress += Time.deltaTime * TransitionSpeed;
                yield return null;
            }

            _mode = _toData.Mode;
            _fromData = _toData = null;
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