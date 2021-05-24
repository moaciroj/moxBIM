using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    public class IFCClass
    {
        public string Teste { get; private set; }
        //Sucesso
        public bool Success { get; private set; }

        public IFCClass()
        {
            Teste = "Testando";
            Success = true;
        }
    }
}
