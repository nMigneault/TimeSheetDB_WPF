using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TimeSheetBD.Model;
namespace TimeSheetBD.Utils
{
    public class RulesValidation
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
 * Cette méthode interne calcule le nombre d'entrée de projets pour un jour de la semaine.
 * Elle est utile pour toute règle vérifiant que durant un jour donné, l'employé n'a pas 
 * dépassé un certain nombre de projets sur lesquel il a travaillé 
 */
        private static int getNbProjectEntries(DayOfWeek dayOfWeek, List<ProjectTimeEntry> projectsEntries)
        {
            int nbProjectEntries = 0;
            foreach (ProjectTimeEntry projectEntry in projectsEntries)
            {
                if (projectEntry.Date.DayOfWeek == dayOfWeek)
                {
                    nbProjectEntries++;
                }
            }
            return nbProjectEntries;
        }

        /*
         * Règles 1 et 2 : Cette methode verifie si l'employé a travaillé le nombre d'heures
         * minimales au bureau par semaine excluant le télétravail.
         * Remarque : les règles 1 et 2 sont mutuellement exclusive, et c'est ce qui justifie
         * leur vérification dans une même méthode
         */
        public static void verifyMinWorkHoursPerWeek(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            //Semaine de travail minimale pour les employés de production et d'exploitation
            const double MIN_WORK_HOURS_A_WEEK_EMPLOYEE = 38;

            //Semaine de travail minimale pour les employés d'administration
            const double MIN_WORK_HOURS_A_WEEK_ADMIN_EMPLOYEE = 36;
            double totalHours = Calculation.getOfficeWorkTimePerWeek(projectsEntries);
            if (employee.EmployeeId >= 1000 && totalHours < MIN_WORK_HOURS_A_WEEK_EMPLOYEE)
            {
                messages.addMessage("Nombre minimal d'heures de travail au bureau par semaine NON ATTEINT");
            }
            else if (employee.EmployeeId < 1000 && totalHours < MIN_WORK_HOURS_A_WEEK_ADMIN_EMPLOYEE)
            {
                messages.addMessage("Nombre minimal d'heures de travail au bureau par semaine NON ATTEINT");
            }

        }

        /**
         * Règle 3 : Cette methode vérifie qu'aucun employé ne dépasse plus de 43 heures au bureau par semaine.
         */
        public static void verifyMaxWorkHoursPerWeek(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            const double MAXIMUM_WORK_HOURS_A_WEEK = 43;
            double totalHours = Calculation.getOfficeWorkTimePerWeek(projectsEntries);

            if (totalHours > MAXIMUM_WORK_HOURS_A_WEEK)
            {
                messages.addMessage("Nombre maximal d'heures de travail au bureau par semaine DÉPASSÉ");
            }
        }

        /**
         * Règle 4 : Cette methode verifie que les employés de l’administration (employeeId < 1000 ne 
         * doivent pas faire plus de 10 heures de télétravail par semaine.
         */
        public static void verifyMaxRemoteHoursPerWeekAdmin(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {

            const double MAX_REMOTEWORK_HOURS_A_WEEK = 10;
            if (employee.EmployeeId < 1000)
            {
                double totalHours = Calculation.getRemoteWorkTimePerWeek(projectsEntries);

                if (totalHours > MAX_REMOTEWORK_HOURS_A_WEEK)
                {
                    messages.addMessage("Nombre maximal d'heures de télétravail par semaine DÉPASSÉ");
                }
            }
        }

        /**
         * Règles 6 et 7 : Cette methode verifie si l'employé a travaillé le nombre d'heures
         * minimales au bureau par jour ouvrable incluant les jours fériés.
         */
        public static void verifyMinimumWorkedHourPerBusinessDay(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            double MINIMUM_WORKED_HOURS_A_DAY = 6;
            if (employee.EmployeeId < 1000)
            {
                MINIMUM_WORKED_HOURS_A_DAY = 4;
            }
            for (int i = 0; i < daysOfWeek.Length; ++i)
            {
                DayOfWeek dayOfWeek = daysOfWeek[i];

                if (DateUtils.isBusinessDay(dayOfWeek))
                {
                    double totalHours = Calculation.getOfficeWorkTimePerDay(dayOfWeek, projectsEntries);
                    if (totalHours < MINIMUM_WORKED_HOURS_A_DAY)
                    {
                        String message = " : Nombre minimal d'heures de travail au bureau par jour ouvrable NON ATTEINT";
                        messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + message);
                    }
                }
            }
        }
        /*
         * Cette méthode vérifie qu'une journée de travail ne dépasse pas 24h
         */
        public static void verifyMaxTimePerDay(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            /*
             * Pour chaque jour de la semaine, on vérifie si le temps de travail de 
             * l'employé n'a pas dépassé 2h. On inscrit un message d'erreur avec 
             * le jour dans le cas constraire.
             */
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                double total = 0;
                foreach (ProjectTimeEntry projectTimeEntry in projectsEntries)
                {
                    if (projectTimeEntry.Date.DayOfWeek == dayOfWeek)
                    {
                        total += projectTimeEntry.Duration / 60;
                    }

                }

                if (total > 24)
                {
                    messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : les 24h permises ont été dépassées");
                }

            }
        }

        /*
         * Cette méthode vérfie la validité d'une journée de congé maladie (projet 999).
         * Règles à vérifier : 
         *  -La durrée d'un projet de code 999 doit être égale à 420 min (travail de bureau)
         *  -Une journée de congé maladie doit avoir un comme seul projet 999
         *  -Pas de journée de congé maladie en fin de semaine.
         */
        public static void verifySickLeave(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                foreach (ProjectTimeEntry projectEntry in projectsEntries)
                {
                    if (projectEntry.Project.ProjectId == 999 && projectEntry.Date.DayOfWeek == dayOfWeek)
                    {
                        if (getNbProjectEntries(dayOfWeek, projectsEntries) > 1)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(projectEntry.Date.DayOfWeek) + " : " +
                                "Une autre activité professionnelle A EU LIEU durant la journée de congé maladie");
                        }
                        if (projectEntry.Duration != 420)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(projectEntry.Date.DayOfWeek) + " : " +
                                "la journée de congé maladie a été chargé avec UNE DURÉE AUTRE QUE 420 minutes");
                        }
                        if (!DateUtils.isBusinessDay(projectEntry.Date.DayOfWeek))
                        {
                            messages.addMessage(DateUtils.toFrenchDay(projectEntry.Date.DayOfWeek) + " : " +
                                "la journée de congé maladie à ÉTÉ PRISE LA FIN DE LA SEMAINE");
                        }
                    }
                }
            }

        }

        /*
         * Cette méthode vérfie la validité d'une journée férié (projet 998)
         * Règles à vérifier : 
         *  -La journée DOIT être fériée (parmis les 13 jours fériés du Canada)
         *  -La durrée d'un projet de code 998 doit être égale à 420 min (travail de bureau)
         *  -Le télétravail est autorisé durant une telle journée
         *  -Pas de journée de congé férié en fin de semaine.
         */
        public static void verifyHoliday(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                foreach (ProjectTimeEntry projectEntry in projectsEntries)
                {
                    if (projectEntry.Project.ProjectId == 998 && projectEntry.Date.DayOfWeek == dayOfWeek)
                    {
                        if (!DateUtils.isHoliDay(projectEntry.Date))
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + ", le " +
                                projectEntry.Date.ToString("dd-MM-yyyy") +
                                " n'est pas une journée fériée du Canada");
                        }
                        else
                        {
                            /*
                             * C'est un jour férié du Canada. Il faut alors vérifier que le jour de congé
                             * ->n'a pas été utilisé en fin de semaine
                             * ->a été chargé avec une durée vallant 420 min (7h) 
                             * ->Il est permis de faire du télétravail en plus : on vérifie donc que les éventuels
                             * autres projets sont de type télétravail (projectId
                             */
                            if (!DateUtils.isBusinessDay(projectEntry.Date.DayOfWeek))
                            {
                                messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                    "la journée de congé fériée à ÉTÉ PRISE LA FIN DE LA SEMAINE");
                            }
                            if (projectEntry.Duration != 420)
                            {
                                messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                    "la journée de congé férié a été chargé avec UNE DURÉE AUTRE QUE " +
                                    "420 minutes");
                            }
                            if (getNbProjectEntries(dayOfWeek, projectsEntries) > 1 &&
                                Calculation.getOfficeWorkTimePerDay(dayOfWeek, projectsEntries) > 7)
                            {
                                messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                "Il n'est pas permis de travailler AU BUREAU durant un congé férié");
                            }
                        }
                    }
                }
            }
        }

        /*
         * Cette méthode vérifie si les règles liées au transport ont été responectées :
         * ->un maximum d'une heure par jour pour les employés utilisant le transport en commun (code 997)
         * ->un maximum de 30 minutes par jour pour employés utilisant leurs propres voiture (code 996)
         */
        public static void verifyTransportRules(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                foreach (ProjectTimeEntry projectEntry in projectsEntries)
                {
                    if(projectEntry.Date.DayOfWeek == dayOfWeek)
                    {
                        if(projectEntry.Project.ProjectId == 997 && projectEntry.Duration > 60)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                          "jour chargé avec plus d'une heure de transport en commun");
                        }
                        if (projectEntry.Project.ProjectId == 996 && projectEntry.Duration > 60)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                          "jour chargé avec plus d'une heure de transport en voiture");
                        }
                    }
                    
                }
            }
        }
        /*
         * Cette méthode vérifie si les règles liées au congé de maternité :
         * L'employée doit rentrer le code 995 avec une durée de 420 min pour tous les jours
         * de la semaine, sauf le weekend
         */
        public static void verifyParentalLeaveRules(Employee employee,
            List<ProjectTimeEntry> projectsEntries, Message messages)
        {
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                foreach (ProjectTimeEntry projectEntry in projectsEntries)
                {
                    if(projectEntry.Project.ProjectId == 995 && projectEntry.Date.DayOfWeek == dayOfWeek)
                    {
                        /*On s'assure que la durée du projet de code 995 est de 420 minutes*/
                        if(projectEntry.Duration != 420)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                   "jour de congé parental chargé avec une durée autre que 420 minutes");
                        }
                        /*On s'assure que la durée du projet de code 995 est le seul code de projet du jour */
                        if (getNbProjectEntries(dayOfWeek, projectsEntries) != 1)
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                   "jour de congé parental contenant un code de projet autre que le code 995");
                        }
                        /*On s'assure que le jour courant n'est pas une fin de semaine */
                        if (DateUtils.isBusinessDay(dayOfWeek))
                        {
                            messages.addMessage(DateUtils.toFrenchDay(dayOfWeek) + " : " +
                                   "projet de code 995 saisi pour un jour de fin de semaine");
                        }
                    }
                }
            }
        }
    }
}
