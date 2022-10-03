using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// La fenêtre EmployeeModifyUI est l'interface qui s'ouvre lorsque l'utilisateur clique sur "Modifier un employé" 
    /// à partir de la fenêtre de gestionnaire des employés (EmploeeUI). Toutes les fonctionalités pour procéder à la 
    /// modifications d'un employé sont rassemblés ici.
    /// </summary>
    public partial class EmployeeModifyUI : Window
    {
        private EmployeeController employeeController;
        private Employee employee;

        public EmployeeModifyUI(EmployeeController employeeController, Employee employeeToModify)
        {
            InitializeComponent();
            this.employeeController = employeeController;
            this.employee = employeeToModify;
        }

      /**
       * Cette méthode remplis tous les champs modifiables avec les informations de l'employé
       * contenue dans la BD.
       */
        private void initUI(object sender, RoutedEventArgs e)
        {
            txtEmployeeId.Text = employee.EmployeeId.ToString();
            txtEmployeeId.IsEnabled = false;
            txtLastName.Text = employee.LastName;
            txtFirstName.Text = employee.FirstName;
            txtAddress.Text = employee.Address;
            if (employee.Gender == 'M')
            {
                rbtMale.IsChecked = true;
            }
            else
            {
                rbtFemale.IsChecked = true;
            }
        }

        /**
         * Modifie un employé.  
         */
        private void btnModifyClick(object sender, RoutedEventArgs e)
        {
            if (!validateData())
            {
                MessageBox.Show("Vous devez renseigner tous les champs", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Employee modifiedEmployee = new Employee()
            {
                EmployeeId = employee.EmployeeId,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Address = txtAddress.Text,
                Gender = employee.Gender,
                Login = employee.Login,
                Password = employee.Password,
                Role = employee.Role
            };
            employeeController.updateEmployee(modifiedEmployee);
            MessageBox.Show("Employé modifié avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        /**
         * Annule l'action et ferme la fenêtre.  
         */
        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show("Voulez-vous vraiment annuler ?",
                                        "Confirmation de l'action", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (response == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

       /**
        * Cette méthode s'assure que le responsable a renseigné tous les champs modifiables.
        * Elle évite donc de mettre à jour l'employé sélectionné avec des informations vide 
        * pour le prénom, le nom, l'adresse et le genre.
        */
        private bool validateData()
        {
            return !string.IsNullOrEmpty(txtLastName.Text) && 
                   !string.IsNullOrEmpty(txtFirstName.Text) && 
                   !string.IsNullOrEmpty(txtLastName.Text) && 
                   (rbtFemale.IsChecked == true || rbtMale.IsChecked == true);
        }
    }
}
