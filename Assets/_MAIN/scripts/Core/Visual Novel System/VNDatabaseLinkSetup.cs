using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    public class VNDatabaseLinkSetup : MonoBehaviour
    {
        public void SetupExternalLinks()
        {
            VariableStore.CreateVariable("VN.SPValue", 0, () => VNGameSave.activeFile.SPValue, value => VNGameSave.activeFile.SPValue = value);
        }
    }
}