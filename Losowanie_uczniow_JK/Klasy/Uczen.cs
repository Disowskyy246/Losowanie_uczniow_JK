using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Losowanie_uczniow_JK.Klasy
{
    public class Uczen
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int Numer { get; set; }

        public Uczen(string imie, string nazwisko, int numer)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Numer = numer;
        }
    }
}
