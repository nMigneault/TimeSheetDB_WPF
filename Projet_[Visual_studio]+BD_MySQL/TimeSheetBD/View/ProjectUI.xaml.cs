using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeSheetBD.Controller;
using TimeSheetBD.Model;


namespace TimeSheetBD.View
{
    /// <summary>
    /// Logique d'interaction pour ProjectsUI.xaml
    /// </summary>
    public partial class ProjectUI : Window
    {
        private ProjectController projectController;
        //private Project project;
        public ProjectUI()
        {
            projectController = new ProjectController();
            InitializeComponent();
        }

        /// Rempli/initialise le Listview avec la liste des projets contenus dans la base de données.
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            lsvProject.ItemsSource = projectController.getProjectList();
        }


        // Ajoute un projet à la bdd si le projet n'existe pas déjà.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtProjectId.Text == String.Empty)
            {
                MessageBox.Show("Vous devez entrer un numéro de projet valide.", 
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!validatePositiveInt(txtProjectId.Text))
            {
                MessageBox.Show("Vous devez saisir un ID valide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int projectId = Int32.Parse(txtProjectId.Text);
            Project project = new Project(projectId, txtProjectName.Text, txtProjectDescription.Text);
            
            bool projectExist = projectController.projectExist(projectId);
            if (projectExist)
            {
                MessageBox.Show("Ce projet existe déjà. Veuiller inscrire un nouveau numéro de projet.", 
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtProjectName.Text == "")
            {
                txtProjectName.Text = "n/d";
            }

            if (txtProjectDescription.Text == "")
            {
                txtProjectDescription.Text = "n/d";
            }

            projectController.createProject(project);
            // Mise à jour la ListView des projets.
            // Nice to have : Cela peut être évité en implémentant INotifyPropertyChanged au niveau de la classe : Project
            lsvProject.ItemsSource = projectController.getProjectList();
            MessageBox.Show("Projet créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            clearFields();
        }

        /// s'assure que la valeur est positive
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

        /*
         * Pour l'instant, le bouton 'modifier' est désactivé.
         * Il est impossible de modifier un projet car celui-ci ne contient qu'un id lequel est un PK.
         * D'ici peu un projet contiendra un nom de projet et une description, lesquelles seront modifiables.
         */
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            Project selectedProject = (Project)lsvProject.SelectedItem;
            if (selectedProject == null)
            {
                MessageBox.Show("Vous devez sélectionner le projet à modifier", 
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtProjectName.Text == "" || txtProjectDescription.Text == "")
            {
                MessageBox.Show("Merci d'entrer le nom et la description du projet à modifier.",
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int projectId = Int32.Parse(txtProjectId.Text);
            Project modifiedProject = new Project(projectId, txtProjectName.Text, txtProjectDescription.Text);
            projectController.updateProject(modifiedProject);

            // Mise à jour la ListView des projets.
            lsvProject.ItemsSource = projectController.getProjectList();
            MessageBox.Show("Projet modifié avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            clearFields();
        }

        // Ajout d'un projet
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Project selectedProject = (Project)lsvProject.SelectedItem;
            if (selectedProject == null)
            {
                MessageBox.Show("Vous devez sélectionner le projet à supprimer", 
                                "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // confirmer la suppression du projet auprès de l'usager
            MessageBoxResult message = MessageBox.Show("Voulez-vous vraiment supprimer ce projet?",
                                                        "Attention", MessageBoxButton.YesNo, 
                                                        MessageBoxImage.Question);
            if (message == MessageBoxResult.No)
            {
                return;
            }

            // Suppression du projet dans la bdd qui correpond au projectId du projet sélectionné dans la ListView.
            try
            {
                projectController.deleteProject(selectedProject.ProjectId);
                clearFields();
                // Mise à jour la ListView des projets.
                // Nice to have : Cela peut être évité en implémentant INotifyPropertyChanged au niveau de la classe : Project
                lsvProject.ItemsSource = projectController.getProjectList();
                MessageBox.Show("Employé supprimé.", "information", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /**
         *  Afficher les valeurs du projet deans les champs
         * */
        private void showProjectValues(object sender, SelectionChangedEventArgs e)
        {
            Project selectedProject = (Project)lsvProject.SelectedItem;
            if(selectedProject != null)
            {
                txtProjectId.IsEnabled = false; // désactive la possibilité de modification du ID de projet
                txtProjectName.Text = selectedProject.ProjectName;
                txtProjectId.Text = selectedProject.ProjectId.ToString();
                txtProjectDescription.Text = selectedProject.ProjectDescription;
            }
        }

        private void clearFields()
        {
            txtProjectId.IsEnabled = true;
            txtProjectId.Clear();
            txtProjectName.Clear();
            txtProjectDescription.Clear();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (txtProjectId.Text != "")
            {
                clearFields();
            }
            lsvProject.SelectedItem = null;
        }
    }
}
