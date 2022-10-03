using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TimeSheetBD.Controller;
using TimeSheetBD.Model;
using TimeSheetBD.Utils;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.Pdf.Grid;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace TimeSheetBD.View
{
    /// <summary>
    /// La fenêtre TimeCardUI est l'interface qui s'ouvre lorsqu'un employé se comnnecte en tant que "employé"
    /// pour remplir sa feuille de temps. C'est aussi à partir de cette fenêtre qu'il peut accéder à l'option 
    /// comme "modifier mon mot de passe".
    /// </summary>
    public partial class TimeCardUI : Window
    {
        private ProjectTimeEntryController projectTimeEntryController;
        private ProjectController projectController;
        
        /**
         * Les 3 structures ci-dessous travaillent ensemble, parce que les controles (add, modify et delete) 
         * se font toujours sur une date (weekDates), sur une journée de la semaine sélectionnée (daysList) 
         * et s'affichée sur une listView (daysListView). Les trois structures ont une taille de 7( NB_DAYS_WEEK) 
         * et sont appelées dans une boucle allant de l'indice [0] à [6].
         */
        const int NB_DAYS_WEEK = 7;
        private String[] daysList; /// Tableau pour peupler le combobox de choix du jour de la semaine
        private DateTime[] weekDates; /// Tableau des dates de la semaine de lundi à dimanche
        private ListView[] daysListView; /// Tableau des listViews (lundi à dimanche) 

        private List<Project> availableProjects;
        private Employee connectedEmployee;
        private ProjectTimeEntry selectedProjectTimeEntry;

        public TimeCardUI(Employee connectedEmployee)
        {
            projectTimeEntryController = new ProjectTimeEntryController();
            projectController = new ProjectController();

            daysList = new String[NB_DAYS_WEEK];
            weekDates = new DateTime[NB_DAYS_WEEK];
            daysListView = new ListView[NB_DAYS_WEEK];

            availableProjects = new List<Project>();
            this.connectedEmployee = connectedEmployee;
            selectedProjectTimeEntry = new ProjectTimeEntry();

            InitializeComponent();
        }

        /**
         * Au chargement de l'interface, l'employé est identifié au haut de la fenêtre par son ID. 
         * Les boutons de contrôles sont désactivé jusqu'à ce qu'une date soit sélectionnée. 
         * Ensuite, le tableau des listViews est instancié.
         */
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtId.Text = connectedEmployee.EmployeeId.ToString() + ", " +
                         connectedEmployee.FirstName.ToString() + " " +
                         connectedEmployee.LastName.ToString();
            disableControlButtons();
            populateDaysListView();
        }

        /**
         * Cette méthode créer et affiche la page ou l'utilisateur peut moifier son mot de passe  
         */
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            this.Effect = new BlurEffect();
            PasswordModifyUI passwordModifyUI = new PasswordModifyUI(connectedEmployee);
            passwordModifyUI.ShowDialog();
        }

        /**
         * "handlePickDate" est le principal contrôle de l'interface. Tant qu'une date n'est pas sélectionnée, 
         * les contrôles donnant accès à la base de données sont bloqués. Lorsque l'utilisateur choisi une date, 
         * les contrôles d'entrées de la feuille de temps deviennent disponibles. 
         * Si la BD possède déjà des entrées pour l'employée dans la semaine sélectionnée, celles-ci s'affichent 
         * dans les listViews au bas de la présente interface. 
        */
        private void handlePickDate(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)dtpSelectedDay.SelectedDate;

            DateUtils.populateWeekDates(selectedDate, weekDates);
            DateUtils.linkListviewToDaysOfWeek(weekDates, txbStart, txbEnd);

            daysList = DateUtils.populatCbxWeekDays(weekDates);
            availableProjects = projectController.getProjectList();

            cbxWeekDays.ItemsSource = daysList;
            cbxProject.ItemsSource = projectController.getStringProjectList();

            enableControlButtons();

            /// Affiche dans la listView les projets déjà entrées pour la semaine.
            for (int i = 0; i < NB_DAYS_WEEK; i++)
            { 
                daysListView[i].ItemsSource = projectTimeEntryController.
                                              getProjectListByEmployeeIdAndDate
                                              (connectedEmployee.EmployeeId, weekDates[i]); 
            }
        }

       /**
        *  Ajouter un projet
        */
        private void btnAddClick(object sender, RoutedEventArgs e)
        {            
            String input = txtDuration.Text;
            /// Si les champs sont invalides pour chaque jour choisi dans cbxWeekDays, on sort.
            if ((cbxWeekDays.SelectedItem == null) || (cbxProject.SelectedItem == null) || !validatePositiveInt(input))
            {
                MessageBox.Show("Vous devez choisir la journée, le projet et entrer une durée valide !", 
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
               return;
            }

            /// Le calendrier se bloque pour prévenir l'utilisateur d'entrer les dates de semaines différentes sur la même feuille de temps.
            if (dtpSelectedDay.IsEnabled)
            {
                dtpSelectedDay.IsEnabled = false;
                lblPickDate.Foreground = Brushes.Black;
            }
                           
            for (int indexDay = 0; indexDay < NB_DAYS_WEEK; indexDay++)
            {
                /// Un objet Project est créé à partir de la liste de projets, celui-ci a le même index du comboBox des projets cbxProjet
                Project project = new Project();
                int projectIndex = cbxProject.SelectedIndex;
                project = availableProjects[projectIndex];

                /// Vérifie si l'entrée existe déjà pour l'employee pour la date, si non, elle est ajoutée
                if (cbxWeekDays.Text == daysList[indexDay])
                {
                    if (projectTimeEntryController.ContainsProjectTimeEntry(project.ProjectId, connectedEmployee.EmployeeId, weekDates[indexDay]))
                    {
                        MessageBox.Show("Ce projet existe déjà! Merci d'utilisez l'option Modifier."); // nice to have : demander si l'usager veut ajouter cette durée au projet existant.
                        return;
                    }

                    /// Crée l'entée et l'insère dans la BD
                    ProjectTimeEntry projectTimeEntry = new ProjectTimeEntry(connectedEmployee, project, weekDates[0], weekDates[6], 
                                                                             weekDates[indexDay], Int32.Parse(txtDuration.Text));
                    projectTimeEntryController.createProjectTimeEntry(projectTimeEntry);
                            
                    /// Affiche les projets ajoutés (ainsi que les existants) dans la listView de la journée 
                    daysListView[indexDay].ItemsSource = projectTimeEntryController.
                                                         getProjectListByEmployeeIdAndDate
                                                         (connectedEmployee.EmployeeId, weekDates[indexDay]);
                }
            }
        }

         /**
          * Valide que "input" est un entier positif
          */
         private bool validatePositiveInt(String input)
         {
             // ne gère pas les doubles
             Regex myRegex = new Regex(@"[0-9]+$");
             Match match = myRegex.Match(input);
             if (match.Success && int.Parse(input) > 0)
             {
                 return true;
             }
             return false;
         }

        /**
         *  Modifier un projet
         */
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            /// si aucun projet n'est sélectionnée, on sort
            if (selectedProjectTimeEntry == null)
            {
                MessageBox.Show("Vous devez sélectionner un projet à modifier dans la liste", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// Le calendrier se bloque pour prévenir l'utilisateur d'entrer les dates de semaines différentes sur la même feuille de temps.
            if (dtpSelectedDay.IsEnabled)
            {
                dtpSelectedDay.IsEnabled = false;
                lblPickDate.Foreground = Brushes.Black;
            }

            int entryId = selectedProjectTimeEntry.EntryId;
            /// si aucun ID de projet n'est sélectionnée et et la durée est vide, on sort
            if (cbxProject.SelectedItem == null || txtDuration.Text == "")
            {
                MessageBox.Show("Vous devez sélectionner un projet ainsi que sa durée", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProjectTimeEntry modifiedEntry = new ProjectTimeEntry();
            modifiedEntry = projectTimeEntryController.getProjectTimeEntry(entryId);

            /// si aucun projet n'est sélectionnée, on sort
            if (modifiedEntry == null)
            {
                MessageBox.Show("Vous devez sélectionner un projet à modifier dans la liste", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                int projectIndex = cbxProject.SelectedIndex;

                modifiedEntry.Employee.EmployeeId = selectedProjectTimeEntry.Employee.EmployeeId;
                modifiedEntry.Project.ProjectId = availableProjects[projectIndex].ProjectId;
                modifiedEntry.StartDate = selectedProjectTimeEntry.StartDate;
                modifiedEntry.EndDate = selectedProjectTimeEntry.EndDate;
                modifiedEntry.Date = selectedProjectTimeEntry.Date;
                modifiedEntry.Duration = Int32.Parse(txtDuration.Text);

                if (projectTimeEntryController.ContainsProjectTimeEntry(modifiedEntry.Project.ProjectId, connectedEmployee.EmployeeId, modifiedEntry.Date) &&
                    modifiedEntry.Project.ProjectId != selectedProjectTimeEntry.Project.ProjectId)
                {
                    MessageBox.Show("Boom! You found the secret exception!");
                }
                else
                {
                    projectTimeEntryController.updateProjectTimeEntry(modifiedEntry);
                    MessageBox.Show("Projet modifié avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Projet ou durée invalide");
                txtDuration.Focus();
            }

            for (int i = 0; i < NB_DAYS_WEEK; i++)
            {
                daysListView[i].ItemsSource = projectTimeEntryController.getProjectListByEmployeeIdAndDate(connectedEmployee.EmployeeId, weekDates[i]);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            /// Le calendrier se bloque pour prévenir l'utilisateur d'entrer les dates de semaines différentes sur la même feuille de temps.
            if (dtpSelectedDay.IsEnabled)
            {
                dtpSelectedDay.IsEnabled = false;
            }

            if (selectedProjectTimeEntry == null) 
            { 
                MessageBox.Show("Vous devez sélectionner un projet dans la liste", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                if (cbxProject.SelectedItem == null)
                {
                    MessageBox.Show("Vous devez sélectionner un projet dans la liste", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                /// mise à jour de la BD
                projectTimeEntryController.deleteProjectTimeEntry(selectedProjectTimeEntry.EntryId);
                
                /// Mise à jour de l'affichage des listViews.
                for (int i = 0; i < NB_DAYS_WEEK; i++)
                {
                    daysListView[i].ItemsSource = projectTimeEntryController.getProjectListByEmployeeIdAndDate(connectedEmployee.EmployeeId, weekDates[i]);
                }
                MessageBox.Show("Projet supprimé avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

       /**
        * Cette méthode sert à supprimer toutes les entrées de l'employée pour la semaine en cours. 
        * Cela supprimer l'affichage mais également les entrées de cette semaine dans la base de données. 
        * Une confirmation est demandées les effacer. 
       */
        private void btnInitializeClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult message = MessageBox.Show("Toute donnée non enregistrée sera perdue. Voulez-vous continuer?",
                                                       "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (message == MessageBoxResult.Yes)
            {
                if (!dtpSelectedDay.IsEnabled)
                {
                    dtpSelectedDay.IsEnabled = true;
                }
                lblPickDate.Foreground = Brushes.Red;
                clearAllData();
            }
        }

        /**
         *  Cette méthode exporte la feuille de temps de l'employé connecté au format PDF dans le répertoire "PDF" créé à cette fin 
         */
        private void btnExportPdfClick(object sender, RoutedEventArgs e)
        {
            ProjectTimeEntryController fromControllers = new ProjectTimeEntryController();
            List<ProjectTimeEntry> listProjectTimeEntries = fromControllers.getProjectTimeEntryList();

            Employee employee = new Employee();
            employee.EmployeeId = connectedEmployee.EmployeeId;

            Project project = new Project();

            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 20);

                graphics.DrawString("Info sur la feuille de temps de l'employé no." + txtId.Text, font, PdfBrushes.Black, new PointF(0, 0));

                PdfGrid pdfGrid = new PdfGrid();

                pdfGrid.DataSource = projectTimeEntryController.getProjectListByEmployeeId(connectedEmployee.EmployeeId);

                pdfGrid.Draw(page, new PointF(0, 50));
                document.Save(@"pdf\Timecard_exporte" + connectedEmployee.EmployeeId + ".pdf");
                MessageBox.Show("PDF exporté avec succès !", "Informations", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Pour quitter
        private void btnQuitClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment quitter?",
                                                      "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        /**
         * La méthode crée une liste avec les 7 listViews à afficher dans l'interface. Facilite les controles d'ajout, 
         * modification, visualisation et suppression des projets dans la feuille de temps, en utilisant une simple boucle 
         * pour la liste au lieu de répéter la même fonction pour chaque listView. 
         * */
        private void populateDaysListView()
        {
            daysListView[0] = lstDay1;
            daysListView[1] = lstDay2;
            daysListView[2] = lstDay3;
            daysListView[3] = lstDay4;
            daysListView[4] = lstDay5;
            daysListView[5] = lstWeekend1;
            daysListView[6] = lstWeekend2;
        }

        /// Active les boutons de gestion de projet
        private void enableControlButtons()
        {
            btnAdd.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnModify.IsEnabled = true;
            cbxWeekDays.IsEnabled = true;
            cbxProject.IsEnabled = true;
            txtDuration.IsEnabled = true;
            btnExportPDF.IsEnabled = true;
            lblPickDate.Foreground = Brushes.Black;
            btnInitialize.IsEnabled = true;

        }

        /// Désactive les boutons de gestion de projet
        private void disableControlButtons()
        {
            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnModify.IsEnabled = false;
            cbxWeekDays.IsEnabled = false;
            cbxProject.IsEnabled = false;
            txtDuration.IsEnabled = false;
            btnExportPDF.IsEnabled = false;
            lblPickDate.Foreground = Brushes.Red;
            btnInitialize.IsEnabled = false;
        }

        /// Supprime les données d'entrées de temps et de dates de la BD
        private void clearAllData()
        {
            for (int i = 0; i < NB_DAYS_WEEK; i++)
            {
                projectTimeEntryController.deleteAllbyEmployeeDate(connectedEmployee.EmployeeId, weekDates[i]);
                daysListView[i].ItemsSource = projectTimeEntryController.
                                              getProjectListByEmployeeIdAndDate
                                              (connectedEmployee.EmployeeId, weekDates[i]);
            }
            clearEntries();
        }

        /// Vide les champs d'entrées de l'interface
        private void clearEntries()
        {
            dtpSelectedDay.Focus();
            cbxWeekDays.SelectedItem = null;
            cbxProject.SelectedItem = null;
            txtDuration.Clear();
        }

        /// Affiche les valeurs de la crate de temps dans les champs d'entrées de l'interface
        private void showValues(object sender, SelectionChangedEventArgs e)
        {
            int indexProject = 0;
            ListView list = (ListView)e.Source;
            selectedProjectTimeEntry = (ProjectTimeEntry)list.SelectedItem;
            if (selectedProjectTimeEntry != null)
            {
                indexProject = projectController.getProjectIndex(availableProjects, selectedProjectTimeEntry.Project.ProjectId);
                cbxWeekDays.SelectedIndex = getListViewIndex(list);
                cbxProject.SelectedIndex = indexProject;
                txtDuration.Text = selectedProjectTimeEntry.Duration.ToString();
            }
        }

        private int getListViewIndex(ListView list)
        {
            int index = -1;

            for (int i = 0; i < daysListView.Length; i++)
            {
                if (list == daysListView[i])
                {
                    index = i;
                }
            }

            return index;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Effect = null;
        }
    }
}