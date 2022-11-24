using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace vezbe6_2deo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string file;
        int saved = 0;       // 0 nije sacuvano 1 jeste sacuvano
        int prazanFile = 1;  // 1 jeste prazan 0 nije prazan

        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source); //lambda izraz  sistemski fontovi koji se nalaze na sistemu systemfontfamilies
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 }; //cmb za font
            cmbFontColor.ItemsSource = typeof(Colors).GetProperties();

            rtbEditor.FontFamily= new FontFamily("Arial");
            cmbFontFamily.SelectedItem = new FontFamily("Arial");
            rtbEditor.FontSize = 14;
            cmbFontSize.Text = "14";
            rtbEditor.Foreground = Brushes.Blue;
            cmbFontColor.SelectedItem = typeof(Colors).GetProperties()[9];
            textBox.Text = "Broj reci: 0";
            lblCursorPosition.Text = "Red: " + "1" + "Kolona: " + "1";
        }

        //NOVI DOKUMENT
        private void New_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;

            if (saved == 0 && !String.IsNullOrWhiteSpace(text))
            {
                MessageBoxResult result = MessageBox.Show("Da li zelite da sacuvate pre pravljenja novog dokumenta?", "Potvrda", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    if (prazanFile != 1)
                    {
                        save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                    else
                    {
                        saveas.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                    rtbEditor.Document.Blocks.Clear();
                    saved = 0;
                    prazanFile = 1;
                    textBox.Text = "Broj reci: 0";
                    lblCursorPosition.Text = "Red: " + "1" + "Kolona: " + "1";

                }
                else if(result==MessageBoxResult.No)
                {
                    rtbEditor.Document.Blocks.Clear();
                    saved = 0;
                    prazanFile = 1;
                    textBox.Text = "Broj reci: 0";
                    lblCursorPosition.Text = "Red: " + "1" + "Kolona: " + "1";
                }
                else if(result==MessageBoxResult.Cancel)
                {

                }
                prazanFile = 1;
            }

            
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;

            if (saved == 0 && !String.IsNullOrWhiteSpace(text))
            {
                MessageBoxResult result = MessageBox.Show("Da li zelite da sacuvate pre otvaranja novog dokumenta?", "Potvrda", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (prazanFile != 1)
                    {
                        save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                    else
                    {
                        saveas.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);

                        file = fileStream.Name;
                        saved = 1; //sacuvaj
                        prazanFile = 0; //nije prazno
                        fileStream.Close();
                    }                      
                }
                else if(result==MessageBoxResult.No)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);

                        file = fileStream.Name;
                        saved = 1; //sacuvaj
                        prazanFile = 0; //nije prazno
                        fileStream.Close();
                    }
                }
                else if(result==MessageBoxResult.Cancel)
                {
                    //ako stisnem cancel ostajem na istom fajlu
                }
            }

            
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (prazanFile == 0)
            {               
                 FileStream fileStream = new FileStream(file, FileMode.Create);
                 TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                 range.Save(fileStream, DataFormats.Rtf);
                 file = fileStream.Name;
                 saved = 1; //sacuvaj
                 fileStream.Close();
            }
            else
            {
                MessageBox.Show("Sacuvajte kao!");
                saveas.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        private void Saveas_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);

                file = fileStream.Name;
                saved = 1; //sacuvaj
                prazanFile = 0; //nije prazno
                fileStream.Close();
            }
        }
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;            //za ispisivanje getPropertyValue, za promenu applyPropertyValue


            //sacuvaj
            saved = 0;

            //Status bar
            string str = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;
            char[] ogranicenja = new char[] { ' ', '\r', '\n'}; // razmak,enter,novi red
            string[] pomocna = str.Split(ogranicenja, StringSplitOptions.RemoveEmptyEntries);
            textBox.Text = "Broj reci: " + pomocna.Length.ToString();

            TextPointer tp1 = rtbEditor.Selection.Start.GetLineStartPosition(0);
            TextPointer tp2 = rtbEditor.Selection.Start;

            int kolona = tp1.GetOffsetToPosition(tp2) + 1;

            int vrednostIntegera = int.MaxValue;
            int pomerenaLinija;
            int trenutniBrojLinije;
            rtbEditor.Selection.Start.GetLineStartPosition(-vrednostIntegera, out pomerenaLinija);
            trenutniBrojLinije = -pomerenaLinija + 1;

            lblCursorPosition.Text = "Line: " + trenutniBrojLinije.ToString() + " Column: " + kolona.ToString();


        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbFontFamily.SelectedItem!=null)
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);//menjanje fonta
            }
        }
        private void CmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }
        private void CmbFontColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbFontColor.SelectedItem != null)
            {
                var selectedItem = (PropertyInfo)cmbFontColor.SelectedItem;
                var color = (Color)selectedItem.GetValue(null, null);

                rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, color.ToString());
                Color boja= (Color)System.Windows.Media.ColorConverter.ConvertFromString(color.ToString());
            }
        }

       
      
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = findR.Text;
            string newString = replace.Text;

            TextRange text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            if (findR.Text.Trim() != "" && replace.Text.Trim() != "")
            {
                while (current != null)
                {
                    string textInRun = current.GetTextInRun(LogicalDirection.Forward);

                    if (!string.IsNullOrWhiteSpace(textInRun))
                    {
                        int index = textInRun.IndexOf(keyword);
                        if (index != -1)
                        {
                            TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                            TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                            TextRange selection = new TextRange(selectionStart, selectionEnd);


                            var bold = selection.GetPropertyValue(FontWeightProperty);
                            var italic = selection.GetPropertyValue(FontStyleProperty);
                            var underline = selection.GetPropertyValue(Inline.TextDecorationsProperty);
                            var font = selection.GetPropertyValue(FontFamilyProperty);
                            var size = selection.GetPropertyValue(FontSizeProperty);
                            var color = selection.GetPropertyValue(ForegroundProperty);

                            selection.Text = newString;

                            selection.ApplyPropertyValue(TextElement.FontWeightProperty, bold);
                            selection.ApplyPropertyValue(TextElement.FontStyleProperty, italic);
                            selection.ApplyPropertyValue(Inline.TextDecorationsProperty, underline);
                            selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
                            selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                            selection.ApplyPropertyValue(TextElement.ForegroundProperty, color);

                            rtbEditor.Selection.Select(selection.Start, selection.End);
                            rtbEditor.Focus();
                        }
                    }
                    current = current.GetNextContextPosition(LogicalDirection.Forward);
                    findR.Text = "";
                    replace.Text = "";
                }
            }
        }
        private void ButtonDT_Click(object sender, RoutedEventArgs e)
        {
                string Datum = DateTime.Now.ToString("dd/MM/yyyy");
                string Vreme = DateTime.Now.ToString("hh:mm:ss tt");
                rtbEditor.CaretPosition.InsertTextInRun(" " + DateTime.Now.ToString() + " ");
                rtbEditor.CaretPosition = rtbEditor.Document.ContentEnd;            
        }
        private void RtbEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) //ako je pritisnuto dugme jednako 
            {
                //ovo
                //buttonDT.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                //ili ovo
                string Datum = DateTime.Now.ToString("dd/MM/yyyy");
                string Vreme = DateTime.Now.ToString("hh:mm:ss tt");
                rtbEditor.CaretPosition.InsertTextInRun(" " + DateTime.Now.ToString() + " ");
                rtbEditor.CaretPosition = rtbEditor.Document.ContentEnd;
            }
        }
        //ovo je sa casa
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;
            if (saved == 0 && !String.IsNullOrWhiteSpace(text))
            {
                MessageBoxResult result = MessageBox.Show("Da li zelite da sacuvate pre zatvaranja dokumenta?", "Potvrda", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    if (prazanFile != 1) //ako ima nesto u njemu sacuvaj kad je pritisnuto dugme yes
                    {
                        save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        this.Close();
                    }
                    else
                    {
                        saveas.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        this.Close();
                    }
                }
                else if(result==MessageBoxResult.No)//ako je pritisnuto no samo zatvori
                {
                    this.Close();
                }
                else if(result ==MessageBoxResult.Cancel)
                {
                    //samo ostani na istom fajlu
                }
            }
            else // samo zatvori kad je prazan
            {
                this.Close();
            }
        }
    }
}
/*
implementirati (na kraju je pricao)
    new
    open
    save
    italic
    underline
    promena boje teksta
    velicine slova 
    find & replace 
    Insert date & time(F5)
    treba dodati status bar prikazati broj reci u rich text boxu
    obratiti paznju na situacije koje zahtevaju cuvanje
    obavezno promeniti oblik prozora u odnosu na onaj koji je uradjen na vezbama

*/