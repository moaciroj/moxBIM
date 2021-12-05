using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MoxGraphics;
using MoxGraphics.Geometry;
using MoxBIM.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace MoxProject
{
    public class MoxProjectClass
    {
        public string ProjectName;
        public int ProjectOwner;
        public DateTime ProjectCreate;
        public int ProjectUserChange;
        public DateTime ProjectDateChange;

        /// <summary>
        /// Inicializar a lista de classes de arquivos IFC
        /// </summary>
        public List<IFCFileClass> IFCFileList = new List<IFCFileClass>();
        
        /// <summary>
        /// Verificar se exite algum erro no projeto 
        /// </summary>
        public bool IFCProjectError { get; private set; }

        /// <summary>
        /// IFC Class Current
        /// </summary>
        public IFCFileClass IfcFile { get; private set; }
        
        /// <summary>
        /// IFC Class Current with success
        /// </summary>
        public IFCFileClass LastIfcFile { get; private set; }

        /// <summary>
        /// Construtor do projeto quando novo
        /// </summary>
        /// <param name="Nome">Nome do projeto</param>
        /// <param name="Owner">Proprietário do projeto</param>
        public MoxProjectClass(string Nome, int Owner)
        {
            ProjectName = Nome;
            ProjectOwner = Owner;
            ProjectCreate = DateTime.Now;
            ProjectUserChange = Owner;
            ProjectDateChange = DateTime.Now;
            IFCProjectError = false;
            LastIfcFile = null;
        }

        /// <summary>
        /// Adicionar arquivo IFC ao projeto
        /// </summary>
        /// <param name="fileifcname">Arquivo IFC</param>
        /// <returns></returns>
        public bool AddFile(String fileifcname)
        {
            try
            {

                if (fileifcname.Trim() == "")
                {
                    return false;
                }

                if (!File.Exists(fileifcname.Trim()))
                {
                    return false;
                }

                //Criar uma nova instancia da classe IFC
                IfcFile = new IFCFileClass(fileifcname);

                //Verificar se o nome existe
                for (int i = 0; i < IFCFileList.Count; i++)
                {
                    if (IFCFileList[i].FileName.ToUpper() == IfcFile.FileName.ToUpper()) { return false; }
                }

                if (IfcFile.Success)
                {
                    //Order by file name
                    IFCFileList = IFCFileList.OrderBy(x => x.FileName).ToList();

                    //Read IFC File
                    var geometry = IfcFile.IFCFileOpen();

                    if (IfcFile.Success)
                    {
                        IFCFileList.Add(IfcFile);
                        LastIfcFile = IfcFile;

                        MoxIOFile geometries = new MoxIOFile();
                        geometries.AddGeometry(geometry);
                        geometries.WriteFileGeometry(fileifcname, null);

                        /*
                        MoxIOFile TESTE = new MoxIOFile();
                        var sw1 = Stopwatch.StartNew();
                        MoxGeometry g = TESTE.ReadFileGeometry(fileifcname);
                        long t1 = sw1.ElapsedMilliseconds;

                        var sw2 = Stopwatch.StartNew();
                        MoxGeometry y = TESTE.ReadFileGeometryMemory(fileifcname);
                        long t2 = sw2.ElapsedMilliseconds;
                        MessageBox.Show("Tempo do algorítimo 1 = " + t1 + "\r\nTempo do algorítimo 2 = " + t2);
                        */

                        MoxGraphicsClass graphics = new MoxGraphicsClass();
                        graphics.SendToUnity(fileifcname);

                        return true;
                    }
                }
            }
            catch
            {
                
                //Do here
            }
            return false;
        }
    }
}
