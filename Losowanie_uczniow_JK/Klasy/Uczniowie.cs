using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Losowanie_uczniow_JK.Klasy
{
    public class Uczniowie
    {
        private List<Uczen> uczniowie;
        private List<Uczen> aktywniUczniowie;
        private Dictionary<Uczen, int> licznikWykluczen;
        private bool czyWlaczonySzczesliwyNumer;
        private HashSet<Uczen> nieobecniUczniowie;

        public Uczniowie()
        {
            uczniowie = new List<Uczen>();
            aktywniUczniowie = new List<Uczen>();
            licznikWykluczen = new Dictionary<Uczen, int>();
            czyWlaczonySzczesliwyNumer = false;
            nieobecniUczniowie = new HashSet<Uczen>();
        }

        public bool CzyNieobecnyUczen(Uczen uczen)
        {
            return nieobecniUczniowie.Contains(uczen);
        }

        public void DodajUcznia(string imie, string nazwisko, int numer)
        {
            var uczen = new Uczen(imie, nazwisko, numer);

            uczniowie.Add(uczen);
            aktywniUczniowie.Add(uczen);
            licznikWykluczen[uczen] = 0;
        }

        private Uczen LosujUcznia(int index, List<Uczen> listaUczniow)
        {
            var wylosowanyUczen = listaUczniow[index];
            aktywniUczniowie.Remove(wylosowanyUczen);
            return wylosowanyUczen;
        }

        public Uczen LosowyUczen()
        {
            if (aktywniUczniowie.Any())
            {
                Random losowy = new Random();
                int index = losowy.Next(0, aktywniUczniowie.Count);

                var wylosowanyUczen = LosujUcznia(index, aktywniUczniowie);

                licznikWykluczen[wylosowanyUczen]++;

                if (licznikWykluczen[wylosowanyUczen] == 3)
                {
                    PrzywrocWszystkichUczniow();
                }

                return wylosowanyUczen;
            }

            PrzywrocWszystkichUczniow();
            return null;
        }

        public bool SprawdzCzySzczesliwyNumer()
        {
            return czyWlaczonySzczesliwyNumer;
        }

        public void WlaczSzczesliwyNumer()
        {
            czyWlaczonySzczesliwyNumer = true;
        }

        public void WylaczSzczesliwyNumer()
        {
            czyWlaczonySzczesliwyNumer = false;
        }

        public void DodajNieobecnegoUcznia(string imie, string nazwisko)
        {
            var nieobecnyUczen = uczniowie.FirstOrDefault(u => u.Imie == imie && u.Nazwisko == nazwisko);
            if (nieobecnyUczen != null && !nieobecniUczniowie.Contains(nieobecnyUczen))
            {
                nieobecniUczniowie.Add(nieobecnyUczen);
            }
        }

        public void WlaczUczniowNieobecnych()
        {
            aktywniUczniowie.AddRange(nieobecniUczniowie);
        }

        public void WylaczUczniowNieobecnych()
        {
            aktywniUczniowie = aktywniUczniowie.Except(nieobecniUczniowie).ToList();
        }

        private void PrzywrocWszystkichUczniow()
        {
            licznikWykluczen.Keys.ToList().ForEach(uczen =>
            {
                if (!aktywniUczniowie.Contains(uczen))
                {
                    aktywniUczniowie.Add(uczen);
                }
            });
        }

        public List<Uczen> GetUczniowie()
        {
            return uczniowie;
        }

        public List<string> GetUczniowieZeStatusamiObecnosci()
        {
            List<string> uczniowieZeStatusem = new List<string>();
            foreach (var uczen in uczniowie)
            {
                string statusObecnosci = nieobecniUczniowie.Contains(uczen) ? "Nieobecny" : "Obecny";
                uczniowieZeStatusem.Add($"{uczen.Imie} {uczen.Nazwisko} - {statusObecnosci}");
            }
            return uczniowieZeStatusem;
        }

        public void EdytujUcznia(string stareImie, string stareNazwisko, string noweImie, string noweNazwisko)
        {
            var uczen = uczniowie.FirstOrDefault(u => u.Imie == stareImie && u.Nazwisko == stareNazwisko);
            if (uczen != null)
            {
                uczen.Imie = noweImie;
                uczen.Nazwisko = noweNazwisko;
            }
        }

        public void ZapiszDoPliku(string sciezkaPliku)
        {
            File.WriteAllLines(sciezkaPliku, uczniowie.Select(u => $"{u.Imie},{u.Nazwisko},{u.Numer}"));
        }

        public void WczytajZPliku(string sciezkaPliku)
        {
            if (File.Exists(sciezkaPliku))
            {
                uczniowie.Clear();
                aktywniUczniowie.Clear();
                foreach (var linia in File.ReadAllLines(sciezkaPliku))
                {
                    var czesci = linia.Split(',');
                    if (czesci.Length == 3)
                    {
                        DodajUcznia(czesci[0], czesci[1], int.Parse(czesci[2]));
                    }
                }
            }
        }
    }
}
