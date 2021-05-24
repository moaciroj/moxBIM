using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using SimpleFileBrowser;

namespace IFC
{
    public class UnityIFC : MonoBehaviour
    {
        private IFCProjectClass ifcproject = new IFCProjectClass();

        public static UnityIFC Instance_IFC { get; private set; }

        //Fazer aqui a classe IFC
        void Start()
        {
            //Abrir aqui as varíáveis do sistema
            //Abrir aqui os IFC que estão na pasta do projeto
            if (Instance_IFC != null && Instance_IFC != this)
            {
                GameObject.Destroy(Instance_IFC);
            }
            else
            {
                Instance_IFC = this;
            }
        }

        public IFCFileClass UnitIFC_GetLastIFC()
        {
            if (ifcproject.IFCLastError)
            {
                return null;
            }
            else
            {
                return ifcproject.IFCProject_GetLastIFC;
            }
        }

        public bool UnityIFC_AddFile()
        {
            string s = "C:\\CADHIDRO9.ini";
            ifcproject.IFCProject_AddFile(s);
            Debug.Log("AddFile IFC");
            return !ifcproject.IFCLastError;
        }

            /*
            //SimpleFileBrowser
            public bool UnityIFC_AddFile()
            {
                UnityIFC_Read = false;

                FileBrowser.SetFilters(true, new FileBrowser.Filter("IFC Files", ".ifc"), new FileBrowser.Filter("IFC Files", ".ifc"));
                FileBrowser.SetDefaultFilter(" .ifc ");
                FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe",".dwg",".dxf",".bat",".tqs");
                FileBrowser.AddQuickLink("Users", "C:\\Users", null);

                FileBrowser.ShowLoadDialog((paths) => { Debug.Log("Selected: " + paths[0]); },
                                           () => { Debug.Log( "Canceled" ); },
                                           FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );
                StartCoroutine(ShowLoadDialogCoroutine());
                if (FileBrowser.Success && FileBrowser.Result.Length != 1)
                {
                    UnityIFC_Read = ifcproject.IFCProject_AddFile(FileBrowser.Result[0]);
                }

                return UnityIFC_Read;
            }

            IEnumerator ShowLoadDialogCoroutine()
            {
                // Show a load file dialog and wait for a response from user
                // Load file/folder: both, Allow multiple selection: true
                // Initial path: default (Documents), Initial filename: empty
                // Title: "Load File", Submit button text: "Load"
                yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

                // Dialog is closed
                // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)

                if (FileBrowser.Success)
                {
                    // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
                    for (int i = 0; i < FileBrowser.Result.Length; i++)
                        Debug.Log(FileBrowser.Result[i]);

                    // Read the bytes of the first file via FileBrowserHelpers
                    // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

                    // Or, copy the first file to persistentDataPath
                    //string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
                    //FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);
                }
            }
            */
        }
}
