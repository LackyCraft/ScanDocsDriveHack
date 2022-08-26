using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using IronOcr;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickDowloandScan(object sender, RoutedEventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();

            open.Title = "Select a picture";
            open.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (open.ShowDialog() == true)
            {
                imageBox.Source = new BitmapImage(new Uri(open.FileName));
                TextBoxPathText.Text = open.FileName;
                //Loader.Visibility = Visibility.Visible;
                grid.Visibility = Visibility.Visible;
                MessageBox.Show(checks(TextBoxPathText.Text));
                //Loader.Visibility = Visibility.Collapsed;
            }
        }

        public string checks(string path)
        {
            
            var Ocr = new IronTesseract(); // nothing to configure
            Ocr.Language = OcrLanguage.Russian;
            Ocr.Configuration.ReadBarCodes = false;
            Ocr.Configuration.BlackListCharacters = "`ë|^";
            Ocr.Configuration.RenderSearchablePdfsAndHocr = false;
            Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.AutoOsd;
            Ocr.Configuration.TesseractVariables["tessedit_parallelize"] = false;

            var img = Pix.LoadFromFile(TextBoxPathText.Text);

            try
            {
                using (var Input = new OcrInput(path))
                {
                    Input.AddImage(TextBoxPathText.Text);
                    var Result = Ocr.Read(Input).Text.ToLower();


                    TextBoxTextInDocs.Text = Result;

                    if (Result.Length < 10)
                    {
                        TextBoxTextInDocs.Text = "Документ не был распознан!";
                        StatusScanDocs.Text = "";
                        return "Не удалось распознать документ, возможно скан нечеткий";
                    }
                    else if(Result.Contains("счет на оплат"))
                    {
                        StatusScanDocs.Text = "Тип документа: счет";
                        return "Данный документ является \"Счетом\"";
                    }
                    else if(Result.Contains("фактур"))
                    {
                        StatusScanDocs.Text = "Тип документа: счет-фактура";
                        return "Данный документ является \"Счет-фактурой\""; ;
                    }
                    else if ((Result.Contains("цена") || Result.Contains("сумма") || Result.Contains("товар") || Result.Contains("счет")) && !Result.Contains("акт"))
                    {
                        if (Result.Contains("адресс") && Result.Contains("ИНН") && Result.Contains("КПП") && Result.Contains("грузоотправитель") && Result.Contains("грузополучатель"))
                        {
                            StatusScanDocs.Text = "Тип документа: счет-фактура";
                            return "Данный документ является \"Счет-фактурой\"";
                        }
                        StatusScanDocs.Text = "Тип документа: счет";
                        return "Данный документ является \"Счетом\"";
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла непредвиденная ошибка", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            StatusScanDocs.Text = "";
            return "Документ не был распознан";
        }

        private void ClicCloseGrid(object sender, RoutedEventArgs e)
        {
            grid.Visibility = Visibility.Collapsed;
        }
    }
}
