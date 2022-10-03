using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TimeSheetBD.Utils
{
    public class DateUtils
    {
        public static bool isBusinessDay(DateTime date)
        {
            DayOfWeek dayOfWeek = date.DayOfWeek;
            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }
		
		public static bool isBusinessDay(DayOfWeek dayOfWeek)
        {
            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }
		
		public static string toFrenchDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return "Dimanche";
                case DayOfWeek.Saturday: return "Samedi";
                case DayOfWeek.Monday: return "Lundi";
                case DayOfWeek.Tuesday: return "Mardi";
                case DayOfWeek.Wednesday: return "Mercredi";
                case DayOfWeek.Thursday: return "Jeudi";
                default: return "Vendredi";
            }
        }
		        /*
         * Cette méthode interne renvoie l'index associé au jour de semaine reçu en paramètre : 
         * 0 pour lundi, 1 pour mardi, etc.
         * En C#, les jours de la semaine sont des énumérations de type DayOfWeek. Les constantes des 
         * jours ont pour valeur 0 pour dimanche, 1 pour lundi, 2 pour mardi, et ainsi de suite
         * jusqu'à 6 pour samedi.
         * 
         * Il suffit alors de réaliser une petite manipulation afin d'exploiter cette propriété : 
         * pour dimanche, on retourne 6 (dernier jour de la semaine), et pour tout autre jour, on 
         * retourne la valeur de la constante de l'énumération diminuée de 1 (lundi aura pour valeur 0
         * car il est ici considéré comme le premier jour de la semaine)
         */
        public static int indexOfWeekDay(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
            {
                return 6;
            }
            else return (int)dayOfWeek - 1;
        }

        /*
         * Cette méthde renvoie la constante DayOfWeek associée au numéro de jour de la semaine
         * reçu en paramètre : 0 pour DayOfWeek.Monday, 1 pour DayOfWeek.Tuesday, et ainsi de suite
         * jusqu'à 6 pour DayOfWeek.Sunday. 
         * 
         * La méthode utilise un système de décalage analogue à indexOfWeekDay(DayOfWeek dayOfWeek)
         */
        public static  DayOfWeek? intToDayOfWeek(int index)
        {
            if (index >= 0 && index <= 5)
                return (DayOfWeek)(index + 1);
            if (index == 6)
                return DayOfWeek.Sunday;
            return null;
        }
		
        public static bool isHoliDay(DateTime date)
        {
            DateTime[] holiDaysDate = {
                new DateTime(2022, 1, 1),
                new DateTime(2022, 4, 15),
                new DateTime(2022, 4, 18),
                new DateTime(2022, 5, 23),
                new DateTime(2022, 6, 24),
                new DateTime(2022, 7, 1),
                new DateTime(2022, 8, 1),
                new DateTime(2022, 9, 5),
                new DateTime(2022, 9, 30),
                new DateTime(2022, 10, 10),
                new DateTime(2022, 11, 11),
                new DateTime(2022, 12, 25),
                new DateTime(2022, 12, 26)
            };
            /*
            * Pour comparer 2 dates, on peut uitliser la méthode static  int Compare (DateTime d1, DateTime d2).
            * Cette méthode renvoie un entier :
            * < 0 si d1 précède chronologiquement d2
            * == 0 si d1 et d2 représentent la même date
            * > 0 si d2 précède chronologiquement d1
            */
            foreach (DateTime holidayDate in holiDaysDate)
            {
                if (DateTime.Compare(date, holidayDate) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * La méthode est utilisée dans la classe TimeCardUI pour l'évènement handlePickDate()
         * et calcule la semaine en cours à partir d'une date selectionée dans le calendrier.
         * A semaine commence au lundi et fini au dimanche.
         * */
        public static void populateWeekDates(DateTime selectedDate, DateTime[] weekDates)
        {
            const int NB_DAYS_WEEK = 7;

            if (selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                for (int i = 0; i < NB_DAYS_WEEK; i++)
                {
                    weekDates[i] = selectedDate.AddDays(i - 6);
                }
            }
            else
            {
                for (int i = 1; i < NB_DAYS_WEEK; i++)
                {
                    if ((int)selectedDate.DayOfWeek == i)
                    {
                        for (int j = 0; j < NB_DAYS_WEEK; j++)
                        {
                            weekDates[j] = selectedDate.AddDays(j - i + 1);
                        }
                    }
                }
            }
        }

        /**
         * La méthode est utilisée dans la classe TimeCardUI pour l'évènement handlePickDate()
         * et affiche la date initiale et la date finale de la semaine en cours, après choisir une date sur le calendrier.
         * */
        public static void linkListviewToDaysOfWeek(DateTime[] weekDates, TextBlock txbStart, TextBlock txbEnd)
        {
            txbStart.Text = "Semaine du " + weekDates[0].ToString("dddd dd MMMM yyyy");//changer pas weekDays
            txbEnd.Text = " au " + weekDates[6].ToString("dddd dd MMMM yyyy");
        }

        /**
         * La méthode est utilisée dans la classe TimeCardUI pour l'évènement handlePickDate()
         * et crée une liste String avec les jours de la semaine du luni du dimanche avec la date correpondante pour chaque jour, après avoir selectionné une date de la semaine dans le calendrier.Cette liste servira à peupler le comboBox cbxWeekDays pour le choix du jour de la semaine à ajouter un projet dans la feuille de temps. 
         * */
        public static String [] populatCbxWeekDays(DateTime[] weekDates)
        {
            String [] daysList = new string[] {"Lundi " + weekDates[0].ToString("dd/MM/yyyy"),
                                             "Mardi " + weekDates[1].ToString("dd/MM/yyyy"),
                                             "Mercredi " + weekDates[2].ToString("dd/MM/yyyy"),
                                             "Jeudi " + weekDates[3].ToString("dd/MM/yyyy"),
                                             "Vendredi " + weekDates[4].ToString("dd/MM/yyyy"),
                                             "Samedi " + weekDates[5].ToString("dd/MM/yyyy"),
                                             "Dimanche " + weekDates[6].ToString("dd/MM/yyyy")};
            return daysList;
        }
    }
}
