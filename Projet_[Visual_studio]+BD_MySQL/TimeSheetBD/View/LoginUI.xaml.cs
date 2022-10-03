using System;
using System.Windows;
using TimeSheetBD.Controller;
using TimeSheetBD.Model;

namespace TimeSheetBD.View
{
    /// <ClasseLoginUI>
    /// La classe LoginUI une référence vers un objet LoginController (en plus des composants graphiques).
    /// Cette référence est instanciée dans le constructeur de cette classe.
    /// </ClasseLoginUI>
    public partial class LoginUI : Window
    {
        LoginController loginController;
        public LoginUI()
        {
            InitializeComponent();
            loginController = new LoginController();
        }

        /**
         *  Le code de l'évènement du bouton Se connecter vérifie que le login et le mot de passe sont présent. 
         *  Un message d'erreur apparaît si c'est non. 
         */
        private void btnConnectClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtLogin.Text) || 
               string.IsNullOrEmpty(pbxPassword.Password))
            {
                MessageBox.Show("Le login et le mot de passe sont obligatoires!", "Attention!", 
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            /// Apelle la méthode connect(login, password) de l'objet loginController pour vérifier l'existance 
            /// de l'employé dans la BD
            Employee connectedEmployee = loginController.connect(txtLogin.Text, pbxPassword.Password);
            if(connectedEmployee == null)
            {
                MessageBox.Show("Login ou mot de passe invalide", "Erreur!", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// En cas de l'existence du login et du mot de passe dans bdd, la DAO renvoie un objet Employee
            if (connectedEmployee.Role.Equals("Employee"))
            {
                TimeCardUI timeCardUI = new TimeCardUI(connectedEmployee);
                this.Close();
                timeCardUI.ShowDialog();
            }
            else
            {
                EmployeeUI employeeUI = new EmployeeUI();
                this.Close();
                employeeUI.ShowDialog();
            }
        }

        /**
         * Fermer le programme
         */
        private void btnQuitClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("L'application sera fermée.","Attention", 
                                                      MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.OK)
            {
               Environment.Exit(0);
            }
        }
    }
}
