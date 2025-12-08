using System;

namespace KalapujSol
{
    public class ValorInvalidoException : Exception
    {
        public ValorInvalidoException(string mensaje) : base(mensaje) { }
    }
}
