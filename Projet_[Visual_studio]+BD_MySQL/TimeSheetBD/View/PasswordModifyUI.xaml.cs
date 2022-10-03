using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TimeSheetBD.Controller;
using TimeSheetBD.Model;

namespace TimeSheetBD.View
{
    /// <summary>
    /// Logique d'interaction pour PasswordModifyUI.xaml
    /// </summary>
    public partial class PasswordModifyUI : Window
    {
        private Employee connectedEmployee;

        public PasswordModifyUI(Employee connectedEmployee)
        {
            this.connectedEmployee = connectedEmployee;
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtIdEmployee.Text = connectedEmployee.EmployeeId.ToString() + ", " +
                                 connectedEmployee.FirstName.ToString() + " " +
                                 connectedEmployee.LastName.ToString();
        }

        private void btnModifyClick(object sender, RoutedEventArgs e)
        {
            if (passBoxNewPassword.Password.ToString() == "" || 
                passBoxConfirmNewPassword.Password.ToString() == "" ||
                passBoxNewPassword.Password.ToString() != passBoxConfirmNewPassword.Password.ToString())
            {            
                MessageBox.Show("Vous devez saisir deux mots de passe identiques.",
                                "Mise à jour impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            EmployeeController employeeToModifyControllers = new EmployeeController();
            connectedEmployee.Password = passBoxNewPassword.Password.ToString();
            employeeToModifyControllers.updateEmployee(connectedEmployee);

            MessageBox.Show("Le mot de passe a été mis à jour !",
                            "Succès !", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show("Voulez-vous vraiment annuler?",
                                                        "Confirmation de l'action", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (response == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
