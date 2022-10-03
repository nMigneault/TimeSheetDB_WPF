using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using TimeSheetBD.Controller;
using TimeSheetBD.Model;

namespace TimeSheetBD.View
{
    /// <summary>
    /// Logique d'interaction pour EmployeeUI.xaml
    /// </summary>
    public partial class EmployeeUI : Window
    {
        private EmployeeController employeeController;
        private ProjectTimeEntryController projectTimeEntryController;

        public EmployeeUI()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
            projectTimeEntryController = new ProjectTimeEntryController();
        }

        /**
         * Ouvre la fenêtre de gestion des projets. 
         */
        private void btnProject_Click(object sender, RoutedEventArgs e)
        {
            this.Effect = new BlurEffect();
            ProjectUI projectManagement = new ProjectUI();
            projectManagement.ShowDialog();
        }

        /**
         * Ajoute un employé. 
         */
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (!validateData())
            {
                MessageBox.Show("Vous devez renseigner tous les champs", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!validatePositiveInt(txtEmployeeId.Text))
            {
                MessageBox.Show("Vous devez saisir un ID valide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int employeeId = Int32.Parse(txtEmployeeId.Text);
            char genre = ' ';
            if(rbtFemale.IsChecked == true)
            {
                genre = 'F';
            }
            else
            {
                genre = 'M';
            }

            Employee employee = new Employee(
                                employeeId, 
                                txtLastName.Text, 
                                txtFirstName.Text, 
                                txtAddress.Text, 
                                genre, 
                                txtLogin.Text, 
                                pbxPassword.Password, 
                                "Employee");

            employeeController.createEmployee(employee);
            lsvEmployee.ItemsSource = employeeController.getEmployeeList();
            MessageBox.Show("Employé créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            clearFields();
        }

        private bool validateData()
        {
            return !string.IsNullOrEmpty(txtLastName.Text) &&
                   !string.IsNullOrEmpty(txtFirstName.Text) &&
                   !string.IsNullOrEmpty(txtLastName.Text) &&
                   (rbtFemale.IsChecked == true || rbtMale.IsChecked == true);
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

        private void clearFields()
        {
            txtEmployeeId.Clear();
            txtLastName.Clear();
            txtFirstName.Clear();
            txtAddress.Clear();
            rbtFemale.IsChecked = false;
            rbtFemale.IsChecked = false;
            txtLogin.Clear();
            pbxPassword.Clear();
        }

        private void btnModifyClick(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = (Employee)lsvEmployee.SelectedItem;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Vous devez sélectionner un employé dans la liste", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            /*
            * Le responsable à sélectionné l'employé à modifier. On le passe en paramètre à l'interface de 
            * mofification d'un employé (on passe aussi une référence vers l'objet employeeController associé
            * à l'interface courante
            */
            this.Effect = new BlurEffect();
            EmployeeModifyUI employeeModifyUI = new EmployeeModifyUI(employeeController, selectedEmployee);
            employeeModifyUI.ShowDialog();
            lsvEmployee.ItemsSource = employeeController.getEmployeeList();
        }

        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = (Employee)lsvEmployee.SelectedItem;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Vous devez sélectionner un employé dans la liste.", 
                                "Aucun employé sélectionné", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
            /*
            * On supprime de la BD l'employé dont le employeeId qui correpond à l'identifiant de l'employé
            * sélectionné dans l'interface.
            * 
            *  On demande au responsable de confirmer l'opération avant de procéder à la suppression.
            */
            MessageBoxResult response = MessageBox.Show("Voulez-vous vraiment supprimer l'employé sélectionné?",
                                        "Confirmation de l'action", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if(response == MessageBoxResult.Yes)
            {
                employeeController.deleteEmployee(selectedEmployee.EmployeeId);
                lsvEmployee.ItemsSource = employeeController.getEmployeeList();
                MessageBox.Show("Employé supprimé.", "information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /**
        *  Rempli la liste des employés avec le contenu de la BD.
        */
        private void initUI(object sender, RoutedEventArgs e)
        {
            lsvEmployee.ItemsSource = employeeController.getEmployeeList();
        }

       /**
        * Affiche une listeview contenant les cartes de temps pour l'employé sélectionné. 
        * L'affichage se fait dans une nouvelle fenêtre.
        */
        private void btnShowClick(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = (Employee)lsvEmployee.SelectedItem;
            if(selectedEmployee == null)
            {
                MessageBox.Show("Vous devez sélectionner un employé dans la liste", "Attention",
                                 MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.Effect = new BlurEffect();
            TimeCardManagerUI timeCardManagerUI = new TimeCardManagerUI(selectedEmployee);
            timeCardManagerUI.ShowDialog();
        }

        /**
        * Affiche une listeview contenant les cartes de temps pour l'employé sélectionné. 
        * L'affichage se fait dans une nouvelle fenêtre.
        */
        private void lsvEmployee_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnShowClick(sender, e);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Votre session sera fermée",
                                                      "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                LoginUI login = new LoginUI();
                this.Close();
                login.ShowDialog();
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment quitter?",
                                                      "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        /// annulae l'effet activé lors de l'ouverture d'une autre fene^tre.
        private void Window_Activated(object sender, EventArgs e)
        {
            this.Effect = null;
        }

    }
}
