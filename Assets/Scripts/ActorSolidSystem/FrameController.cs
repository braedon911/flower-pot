using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSolidSystem
{
    public class FrameController : MonoBehaviour
    {
        void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }
    }

}
