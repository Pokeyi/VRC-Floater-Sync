// Copyright © 2021 Pokeyi - https://pokeyi.dev - pokeyi@pm.me - This work is licensed under the MIT License.

using System;
using UdonSharp;
using UnityEngine;
// using UnityEngine.UI;
// using VRC.SDKBase;
// using VRC.SDK3.Components;
// using VRC.Udon.Common.Interfaces;

namespace Pokeyi.UdonSharp
{
    [AddComponentMenu("Pokeyi.VRChat/P.VRC Floater Sync")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] // No networking.

    public class P_FloaterSync : UdonSharpBehaviour
    {   // UTC-synced bobbing and rocking for VRChat:
        [Header(":: VRC Floater Sync by Pokeyi ::")]
        [Space]
        [Tooltip("Floating game objects.")]
        [SerializeField] private GameObject[] floaterObjects;
        [Header("Positional:")]
        [Tooltip("Speed of positional bobbing motion.")]
        [SerializeField] private float bobSpeed = 0.5F;
        [Tooltip("Distance objects will move in each positional bobbing direction.")]
        [SerializeField] private float bobDistance = 0.125F;
        [Tooltip("Whether to apply positional motion to the X axis.")]
        [SerializeField] private bool bobAxisX = false;
        [Tooltip("Whether to apply positional motion to the Y axis.")]
        [SerializeField] private bool bobAxisY = false;
        [Tooltip("Whether to apply positional motion to the Z axis.")]
        [SerializeField] private bool bobAxisZ = false;
        [Header("Rotational:")]
        [Tooltip("Speed of forward-back pitch motion.")]
        [SerializeField] private float pitchSpeed = 0.5F;
        [Tooltip("Angle objects will rotate in each forward-back pitch direction.")]
        [SerializeField] private float pitchDegrees = 2.5F;
        [Tooltip("Speed of left-right roll motion.")]
        [SerializeField] private float rollSpeed = 0.5F;
        [Tooltip("Angle objects will rotate in each left-right roll direction.")]
        [SerializeField] private float rollDegrees = 2.5F;
        [Header("Modifiers:")]
        [Tooltip("Time offset if you want object groups synced separately.")]
        [SerializeField] private float timeOffset = 0F;
        [Tooltip("Whether to match base height of parent objects.")]
        [SerializeField] private bool matchParentHeight;
        [Tooltip("Whether to match base rotation of parent objects.")]
        [SerializeField] private bool matchParentRotation;

        private Quaternion[] startRot; // Original start rotation of floater objects.
        private Vector3[] startPos; // Original start position of floater objects.

        public void Start()
        {   // Record start rotation and height for all floater objects:
            if (floaterObjects == null) return;
            startRot = new Quaternion[floaterObjects.Length];
            startPos = new Vector3[floaterObjects.Length];
            for (int i = 0; i < floaterObjects.Length; i++) if (floaterObjects[i] != null)
                {
                    startRot[i] = floaterObjects[i].transform.rotation;
                    startPos[i] = floaterObjects[i].transform.position;
                }
        }

        public void LateUpdate()
        {   // Convert UTC ticks and align decimal spaces to retrieve time for sine wave calculations, animate objects per frame:
            double utcTime = Convert.ToDouble(DateTime.UtcNow.Ticks) / 1E+07D;
            double bob = Math.Sin((utcTime + timeOffset) * bobSpeed) * bobDistance;
            double pitch = Math.Sin((utcTime + timeOffset) * pitchSpeed) * pitchDegrees;
            double roll = Math.Sin((utcTime + timeOffset) * rollSpeed) * rollDegrees;
            for (int i = 0; i < floaterObjects.Length; i++) if (floaterObjects[i] != null)
                {
                    if (floaterObjects[i].transform.parent != null)
                    {
                        if (matchParentHeight) startPos[i].y = floaterObjects[i].transform.parent.position.y;
                        if (matchParentRotation) startRot[i] = floaterObjects[i].transform.parent.rotation;
                    }
                    Vector3 setPos = floaterObjects[i].transform.position;
                    if (bobAxisX) setPos.x = startPos[i].x + (float)bob;
                    if (bobAxisY) setPos.y = startPos[i].y + (float)bob;
                    if (bobAxisZ) setPos.z = startPos[i].z + (float)bob;
                    Quaternion setRot = startRot[i] * Quaternion.AngleAxis((float)roll, Vector3.forward) * Quaternion.AngleAxis((float)pitch, Vector3.left);
                    floaterObjects[i].transform.SetPositionAndRotation(setPos, setRot);
                }
        }
    }
}

/* MIT License

Copyright (c) 2021 Pokeyi - https://pokeyi.dev - pokeyi@pm.me

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */