//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour
{
    public float maxDistance = 10;
    public float gazeTimeClick = 1.5f;
    public Image loader;

    private GameObject _gazedAtObject = null;
    private float _gazeTime = 0;

    private void Awake()
    {
        SetGazeTime(0);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                SetGazeTime(0);
                _gazedAtObject?.SendMessage("OnPointerExit");
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter");
            }
            else
            {
                // Still same GameObject
                SetGazeTime(_gazeTime + Time.deltaTime);
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            SetGazeTime(0);
            _gazedAtObject?.SendMessage("OnPointerExit");
            _gazedAtObject = null;
        }

        // Checks for screen touches.
        if (Google.XR.Cardboard.Api.IsTriggerPressed)
        {
            _gazedAtObject?.SendMessage("OnPointerClick");
        }

        if(_gazeTime >= gazeTimeClick)
        {
            _gazedAtObject?.SendMessage("OnPointerHover");
        }
    }

    private void SetGazeTime(float value)
    {
        if (value == 0 || _gazedAtObject?.GetComponent<VRClickableButton>())
        {
            _gazeTime = value;
            loader.fillAmount = Mathf.Clamp(_gazeTime / gazeTimeClick, 0, 1);    
        }
    }
}
