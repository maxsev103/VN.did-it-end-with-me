using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

namespace TESTING
{
    public class TestVNGameSave : MonoBehaviour
    {
        public VNGameSave save;

        // Start is called before the first frame update
        void Start()
        {
            VNGameSave.activeFile = new VNGameSave();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                VNGameSave.activeFile.Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                save = VNGameSave.Load($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}", activateOnLoad: true);
            }
        }
    }
}