using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Losowanie_uczniow_JK.Klasy;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Losowanie_uczniow_JK
{
    public partial class MainPage : ContentPage
    {
        private Uczniowie uczniowie;
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "uczniowie.txt");
        public MainPage()
        {
            InitializeComponent();
            uczniowie = new Uczniowie();
        }

        private void DodajUczniaInterakcja(object sender, EventArgs e)
        {
            string imie = nameEntry.Text;
            string nazwisko = surnameEntry.Text;
            int numer = int.Parse(numberEntry.Text);
            if (!string.IsNullOrWhiteSpace(imie) && !string.IsNullOrWhiteSpace(nazwisko))
            {
                uczniowie.DodajUcznia(imie, nazwisko, numer);
                nameEntry.Text = string.Empty;
                surnameEntry.Text = string.Empty;
                numberEntry.Text = (uczniowie.GetUczniowie().Count() + 1).ToString();
                DisplayAlert("Sukces!", "Uczeń został dodany do listy", "OK");
            }
            else
            {
                DisplayAlert("Błąd!", "Upewnij się, że imię i nazwisko są wprowadzone poprawnie", "OK");
            }
        }

        private void LosowyUczenInterakcja(object sender, EventArgs e)
        {
            Uczen losowyUczen = uczniowie.LosowyUczen();
            if (losowyUczen != null)
            {
                DisplayAlert("Sukces!", $"Wylosowany uczeń: {losowyUczen.Numer}. {losowyUczen.Imie} {losowyUczen.Nazwisko}", "OK");
            }
            else
            {
                DisplayAlert("Błąd!", "Brak osoby możliwej do wylosowania (spróbuj ponownie)", "OK");
            }
        }

        private async void EdytujUczniaInterakcja(object sender, EventArgs e)
        {
            if (uczniowie.GetUczniowie().Count() != 0)
            {
                var listaUczniow = uczniowie.GetUczniowie().Select(u => $"{u.Numer}. {u.Imie} {u.Nazwisko}").ToList();
                var wynik = await DisplayActionSheet("Edytuj dane uczniów", "Anuluj", null, listaUczniow.ToArray());

                if (!wynik.Equals("Anuluj"))
                {
                    var czesci = wynik.Split(' ');

                    string imie = czesci[1];
                    string nazwisko = czesci[2];

                    var noweImie = await DisplayPromptAsync("Aktualizuj imię", "Wpisz imię ucznia", initialValue: imie);
                    var noweNazwisko = await DisplayPromptAsync("Aktualizuj nazwisko", "Wpisz nazwisko ucznia", initialValue: nazwisko);

                    if (!string.IsNullOrWhiteSpace(noweImie) && !string.IsNullOrWhiteSpace(noweNazwisko))
                    {
                        uczniowie.EdytujUcznia(imie, nazwisko, noweImie, noweNazwisko);
                        await DisplayAlert("Sukces!", "Dane ucznia zostały zaktualizowane", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Błąd!", "Lista nie zawiera żadnych uczniów", "OK");
            }
        }

        private void UpdateLuckyNumber()
        {
            if (uczniowie.SprawdzCzySzczesliwyNumer())
            {
                luckyNumberLabel.IsVisible = true;
                int szczesliwyNumer = new Random().Next(1, uczniowie.GetUczniowie().Count() + 1);
                luckyNumberLabel.Text = $"Szczęśliwy numerek to: {szczesliwyNumer}";
            }
        }

        private void WlaczSzczesliwyNumerekInterakcja(object sender, EventArgs e)
        {
            uczniowie.WlaczSzczesliwyNumer();
            if (uczniowie.GetUczniowie().Count() != 0)
            {
                luckyNumberLabel.IsVisible = true;
                DisplayAlert("Sukces!", "System szczęśliwego numerka został włączony", "OK");
                UpdateLuckyNumber();
            }
            else
            {
                DisplayAlert("Błąd!", "System szczęśliwego numerka został wyłączony z powodu braku uczniów na liście", "OK");
                luckyNumberLabel.IsVisible = false;
            }
        }

        private void WylaczSzczesliwyNumerekInterakcja(object sender, EventArgs e)
        {
            uczniowie.WylaczSzczesliwyNumer();
            luckyNumberLabel.IsVisible = false;
            DisplayAlert("Sukces!", "System szczęśliwego numerka został wyłączony", "OK");
            UpdateLuckyNumber();
        }

        private void WlaczNieobecnychUczniowInterakcja(object sender, EventArgs e)
        {
            if (absentLabel.Text == "Wyłącz z losowania nieobecnych uczniów")
            {
                uczniowie.WylaczUczniowNieobecnych();
                DisplayAlert("Sukces!", "Wyłączono możliwość losowania uczniów nieobecnych", "OK");
                absentLabel.Text = "Włącz do losowania nieobecnych uczniów";
            }
            else
            {
                uczniowie.WlaczUczniowNieobecnych();
                DisplayAlert("Sukces!", "Włączono możliwość losowania uczniów nieobecnych", "OK");
                absentLabel.Text = "Wyłącz z losowania nieobecnych uczniów";
            }
        }

        private async void DodajNieobecnychUczniowInterakcja(object sender, EventArgs e)
        {
            if (uczniowie.GetUczniowie().Count() != 0)
            {
                var listaUczniow = uczniowie.GetUczniowie().Select(u => $"{u.Numer}. {u.Imie} {u.Nazwisko}").ToList();
                var wynik = await DisplayActionSheet("Wybierz nieobecnych uczniów", "Anuluj", null, listaUczniow.ToArray());

                if (!wynik.Equals("Anuluj"))
                {
                    var czesci = wynik.Split(' ');

                    string imie = czesci[1];
                    string nazwisko = czesci[2];

                    uczniowie.DodajNieobecnegoUcznia(imie, nazwisko);

                    await DisplayAlert("Sukces!", "Dodano nieobecnego ucznia do listy", "OK");
                }
            }
            else
            {
                await DisplayAlert("Błąd!", "Lista nie zawiera żadnych uczniów", "OK");
            }
        }

        private void ZapiszDoPliku(object sender, EventArgs e)
        {
            uczniowie.ZapiszDoPliku(filePath);
            DisplayAlert("Sukces!", "Lista uczniów została zapisana", "OK");
        }

        private void ZaladujZPliku(object sender, EventArgs e)
        {
            uczniowie.WczytajZPliku(filePath);
            numberEntry.Text = (uczniowie.GetUczniowie().Count() + 1).ToString();
            DisplayAlert("Sukces!", "Lista uczniów została wczytana", "OK");
        }

        public void PokazUczniowInterakcja(object sender, EventArgs e)
        {
            List<string> uczniowieZStatusamiObecnosci = new List<string>();
            foreach (var uczen in uczniowie.GetUczniowie())
            {
                string statusObecnosci = uczniowie.CzyNieobecnyUczen(uczen) ? "Nieobecny" : "Obecny";
                uczniowieZStatusamiObecnosci.Add($"{uczen.Numer}. {uczen.Imie} {uczen.Nazwisko} - {statusObecnosci}");
            }
            string listaUczniow = string.Join("\n", uczniowieZStatusamiObecnosci);
            DisplayAlert("Lista uczniów", listaUczniow, "OK");
        }
    }
}
