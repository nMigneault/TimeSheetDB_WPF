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
using TimeSheetBD.Model;
using TimeSheetBD.Controller;
using System.Collections.ObjectModel;
using TimeSheetBD.Utils;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.Pdf.Grid;

namespace TimeSheetBD.View
{
    /// <summary>
    /// Logique d'interaction pour TimeCardManagerUI.xaml
    /// </summary>
    public partial class TimeCardManagerUI : Window
    {
        TimeCard selectedTimeCard = null;
        List<ProjectTimeEntry> projectTimeCardEntries; //= new List<ProjectTimeEntry>();
        ListView[] daysListView;   /// Un tableau de listView afin d'en faciliter la manipulation
        private Employee currentEmployee;
        private ProjectTimeEntryService projectTimeEntryService;
        public TimeCardManagerUI(Employee currentEmployee)
        {
            InitializeComponent();
            daysListView = new ListView[7];
            this.currentEmployee = currentEmployee;
            projectTimeEntryService = new ProjectTimeEntryService();
        }

        private void initUI(object sender, RoutedEventArgs e)
        {
            txtEmployeeId.Text = currentEmployee.EmployeeId.ToString();
            txtLastName.Text = currentEmployee.LastName;
            txtFirstName.Text = currentEmployee.FirstName;
            initListViews();
            /**
             * On désactive le bouton de validation de la carte de temps. Il sera réactivé lorsque 
             * l'utilisateur affiche les détails d'une carte de données qu'il a sélectionné au 
             * préalable. Tnadis que le bouton Exporter en PDF sera activé lorsque l'utilisateur 
             * aura validé la feuille de temps.
             */
            btnValidate.IsEnabled = false;
            btnExportPDF.IsEnabled = false;
            try
            {
                lsvTimeCard.ItemsSource = projectTimeEntryService.getEmployeeTimeCards(currentEmployee.EmployeeId);
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnShowDetailsClick(object sender, RoutedEventArgs e)
        {
            selectedTimeCard = (TimeCard)lsvTimeCard.SelectedItem;
            if (selectedTimeCard == null)
            {
                MessageBox.Show("Vous devez sélectionner une carte de temps", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            btnValidate.IsEnabled = true;
           /**
            * On charge la liste des entrées de projets de la carte de temps, et on met à jour 
            * les listes des projets des jours de la semaine. 
            * 
            * La mise à jour doit se faire avec une collection de type ObservableCollection afin 
            * de mettre à jour la liste avec les bonnes données lors du chargement d'une nouvelle 
            *  feuille de temps
            */
            for (int i = 0; i < daysListView.Length; i++)
            {
                /// Récupère le jour de la semaine correspondant à l'index courant (i)
                DayOfWeek dayOfWeek =(DayOfWeek)DateUtils.intToDayOfWeek(i);
                if(dayOfWeek != null)
                {
                   /**
                    * On récupère les entrées de projets associées au jour courant dans une liste, 
                    * qu'on convertira à une Observale
                    */
                    try
                    {
                        List<ProjectTimeEntry> dayProjectTimeEntries = projectTimeEntryService.getDayProjectTimeEntries(
                                        currentEmployee.EmployeeId, selectedTimeCard, dayOfWeek);

                        //ObservableCollection < ProjectTimeEntry > sourceCollection = new ObservableCollection<ProjectTimeEntry>();
                       /**
                        * On remet à 0 la listeView courante : très important lors de la sélection d'un nouvelle
                        * carte de temps (éviter de mélanger les entrées des cartes)
                        */
                        daysListView[i].ItemsSource = null;
                        //daysListView[i].ItemsSource = sourceCollection;
                        daysListView[i].ItemsSource = dayProjectTimeEntries;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void lsvTimeCard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnShowDetailsClick(sender, e);
        }

        private void initListViews()
        {
            daysListView[0] = lstDay1;
            daysListView[1] = lstDay2;
            daysListView[2] = lstDay3;
            daysListView[3] = lstDay4;
            daysListView[4] = lstDay5;
            daysListView[5] = lstWeekend1;
            daysListView[6] = lstWeekend2;
        }

        private void btnValidateClick(object sender, RoutedEventArgs e)
        {
            /**
             * Validation de la carte de temps : on commence par récupérer la liste
             * des entrées de projets pour la carte de temps sélectionnée, puis on 
             * lance le processus de validation
             */
            txbResults.Text = string.Empty;

            Message messages = new Message();
            projectTimeCardEntries = projectTimeEntryService.getTimeCardProjectTimeEntries(
                currentEmployee.EmployeeId, selectedTimeCard);
            String result = projectTimeEntryService.validateTimeCard(currentEmployee, 
                projectTimeCardEntries, messages);
            if (string.IsNullOrEmpty(result))
            {
                txbResults.Text = "Feuille de temps valide";
            }
            else
            {
                txbResults.Text = result;
            }
            btnExportPDF.IsEnabled = true;
        }


        private void btnExportPdfClick(object sender, RoutedEventArgs e)
        {
            if (projectTimeCardEntries == null)
            {
                MessageBox.Show("Vous devez valider la carte pour exporter les résultats en PDF");
                return;
            }

            Message messages = new Message();
            String result = projectTimeEntryService.validateTimeCard(currentEmployee, projectTimeCardEntries, messages);
           
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;
                PdfFont font1 = new PdfStandardFont(PdfFontFamily.TimesRoman, 15);
                PdfFont font2 = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
                PdfGrid pdfGrid = new PdfGrid();
                pdfGrid.DataSource = lsvTimeCard.ItemsSource;

                if (!string.IsNullOrEmpty(result))
                {
                    graphics.DrawString("Validation des règles feuille de temps de l'employé no." + currentEmployee.EmployeeId.ToString(), font1, PdfBrushes.Black, new PointF(0, 0));
                    pdfGrid.Draw(page, new PointF(0, 25));
                    graphics.DrawString("Résultat de la validation : ", font2, PdfBrushes.Black, new PointF(0, 70));
                    graphics.DrawString(result, font2, PdfBrushes.Black, new PointF(0, 100));
                }
                else
                {
                    MessageBox.Show("La feuille de temps est vide", "ATTENTION", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                document.Save("validations\\Feuille_de_validation.pdf");
                MessageBox.Show("PDF exporté avec succès", "PDF exporté", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
