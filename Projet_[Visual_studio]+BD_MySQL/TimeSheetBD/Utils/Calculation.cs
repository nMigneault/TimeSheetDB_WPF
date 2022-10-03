using System;
using System.Collections.Generic;
using System.Windows;
using TimeSheetBD.Model;


namespace TimeSheetBD.Utils
{
    public class Calculation
    {
        private static DayOfWeek[] daysOfWeek =
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };
        /*
         * Cette méthode calcule le total d'heures de travail de bureau par jour.
         * DayOfWeek dayOfWeek : jour de travail de l'employé.
         * List<ProjectTimeEntry> projectTimeEntries : liste des entrées de projets pour la semaine
         */
        public static double getOfficeWorkTimePerDay(DayOfWeek dayOfWeek, List<ProjectTimeEntry> projectTimeEntries)
        {

            /*
            * Ici, on comptabilise à la fois les projet de type Bureau (projetctId <= 900), les
            * projets de code 999 et 998, ainsi que ceux de code 996 et 997. 
            * Une règle à respecter : seule la moitié d'un projet de code 996 est comptabilisée 
            * comme temps de bureau
            */
            double total = 0.0;
            if (projectTimeEntries.Count != 0)
            {
                foreach(ProjectTimeEntry projectTimeEntry in projectTimeEntries)
                {
                    int projectId = projectTimeEntry.Project.ProjectId;
                    DayOfWeek entryDayOfWeek = projectTimeEntry.Date.DayOfWeek;
                    if((projectId <= 900 || (projectId >= 996 && projectId <= 999)) && 
                        (entryDayOfWeek == dayOfWeek))
                    {
                        if(projectId == 996)
                        {
                            total += projectTimeEntry.Duration / 2.0;
                        }
                        else
                        {
                            total += projectTimeEntry.Duration;
                        }
                    }
                }
                total /= 60;
            }
            //MessageBox.Show("Total bureau pour " + weekDay.DayValue + " : " +  total + "H");
            return total;
        }

        /*
         * Cette méthode calcule le total d'heures de travail de bureau pour la semaine
         * DayOfWeek[] daysOfWeek : tableau des jours de la semaine
         * List<ProjectTimeEntry> projectTimeEntries : liste des entrées de projets pour la semaine
         */
        //public static double getOfficeWorkTimePerWeek(DayOfWeek[] daysOfWeek, List<ProjectTimeEntry> projectTimeEntries)
        public static double getOfficeWorkTimePerWeek(List<ProjectTimeEntry> projectTimeEntries)
        {
            /*Attention : ici on comptabilise le total des HEURES de bureau par jour
            * Le résultat est déjà en heures, non en minutes.
            */
            double total = 0.0;
            foreach(DayOfWeek dayOfWeek in daysOfWeek)
            {
                total += getOfficeWorkTimePerDay(dayOfWeek, projectTimeEntries);
            }
            //MessageBox.Show("Total bureau par semaine : " + total + "H");
            return total;
        }

        /*
         * Cette méthode calcule le total d'heures de télétravail par jour
         * DayOfWeek dayOfWeek : jour de travail de l'employé.
         * List<ProjectTimeEntry> projectTimeEntries : liste des entrées de projets pour la semaine
         */
        public static double getRemoteWorkTimePerDay(DayOfWeek dayOfWeek, List<ProjectTimeEntry> projectTimeEntries)
        {
           /*
            * Comme les projets de code 995, 996, 997 998 et 998 correspondent à des projets 
            * de type télétravail (code > 900), il ne faut pas les comptabliser dans le calcul. Il faut 
            * de rappeler que de tels code représente plutôt des congés, et qu'ils sont, sauf le code 995, 
            * comptabilisés comme temps de bureau
            */
            double total = 0.0;
            if(projectTimeEntries.Count != 0)
            {
                foreach (ProjectTimeEntry projectTimeEntry in projectTimeEntries)
                {
                    int projectId = projectTimeEntry.Project.ProjectId;
                    DayOfWeek entryDayOfWeek = projectTimeEntry.Date.DayOfWeek;
                    if ((projectId > 900 && projectId != 995 && (projectId < 996 || projectId > 999)) &&
                       (entryDayOfWeek == dayOfWeek))
                    {
                        total += projectTimeEntry.Duration;
                    }
                }
                total /= 60;
            }
            //MessageBox.Show("Total télétravail par jour : " + total);
            //MessageBox.Show("Total télétravail pour " + weekDay.DayValue + " : " + total + "H");
            return total;
        }

        /*
         * Cette méthode calcule le total d'heures de télétravail pour la semaine
         * DayOfWeek[] daysOfWeek : tableau des jours de la semaine
         * List<ProjectTimeEntry> projectTimeEntries : liste des entrées de projets pour la semaine
         */
        //public static double getRemoteWorkTimePerWeek(DayOfWeek[] daysOfWeek, List<ProjectTimeEntry> projectTimeEntries)
        public static double getRemoteWorkTimePerWeek(List<ProjectTimeEntry> projectTimeEntries)
        {
            /*Attention : ici on comptabilise le total des HEURES de télétravail par jour
            * Le résultat est déjà en heures, non en minutes.
            */        
            double total = 0.0;
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                total += getRemoteWorkTimePerDay(dayOfWeek, projectTimeEntries);
            }
            //MessageBox.Show("Total télétravail par semaine : " + total + "H");
            return total;
        
        }

        /*
         * Cette méthode calcule le total d'heures de travail de bureau pour les jours ouvrables
         * DayOfWeek[] daysOfWeek : tableau des jours de la semaine
         * List<ProjectTimeEntry> projectTimeEntries : liste des entrées de projets pour la semaine
         */
        //public static double getOfficeWorkTimeOnWorkdays(DayOfWeek[] daysOfWeek, List<ProjectTimeEntry> projectTimeEntries)
        public static double getOfficeWorkTimeOnWorkdays(List<ProjectTimeEntry> projectTimeEntries)
        {
            /*Attention : ici on comptabilise le total des HEURES de travail de bureau par jour
            * ouvrable. Le résultat est déjà en heures, non en minutes.
            */
            double total = 0.0;
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                if(DateUtils.isBusinessDay(dayOfWeek))
                {
                    total += getOfficeWorkTimePerDay(dayOfWeek, projectTimeEntries);
                }
            // MessageBox.Show("Total bureau par jour ouvrable " + weekDay.DayValue + " : " + total + "H");
            }
            //MessageBox.Show("Total bureau par jour ouvrable: " + total);
        
            return total;
        }
    }
}
