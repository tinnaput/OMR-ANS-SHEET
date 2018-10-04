﻿using System;
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

namespace AnswerSheetChecker
{
    /// <summary>
    /// Interaction logic for AddDataGroup.xaml
    /// </summary>
    public partial class AddDataGroup : Window
    {
        public enum Type
        {
            Info,
            Ans,
        }
        private Type type;
        private Template template;
        private Action<Template.TemplateData> callback;

        public AddDataGroup(Type type, Template template, Action<Template.TemplateData> f)
        {
            this.type = type;
            this.template = template;
            callback = f;
            InitializeComponent();
            if (type == Type.Info)
            {
                TextBlockNumberChoice.Text = "";
                TextBoxNumberChoice.IsEnabled = false;
                TextBoxNumberChoice.Text = "10";
            }
            else
            {
                TextBlockName.Text = "";
                TextBoxName.IsEnabled = false;
                TextBlockNumberAns.Text = "จำนวนข้อ";
            }
            ImagePreview.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                template.Image.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(template.Image.Width, template.Image.Height));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxNumberChoice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ButtonOK != null) ButtonOK.IsEnabled = false;
        }

        private void TextBoxNumberAns_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ButtonOK != null) ButtonOK.IsEnabled = false;
        }

        private void TextBoxCol_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ButtonOK != null) ButtonOK.IsEnabled = false;
        }

        private void TextBoxRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ButtonOK != null) ButtonOK.IsEnabled = false;
        }
        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ButtonOK != null) ButtonOK.IsEnabled = false;
        }

        private void TextBoxNumberChoice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int r))
            {
                if (r < 1) ((TextBox)sender).Text = "1";
                return;
            }
            ((TextBox)sender).Text = "1";
        }

        private void TextBoxNumberAns_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int r))
            {
                if (r < 1) ((TextBox)sender).Text = "1";
                return;
            }
            ((TextBox)sender).Text = "1";
        }

        private void TextBoxCol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int r))
            {
                if (r < 0) ((TextBox)sender).Text = "0";
                return;
            }
            ((TextBox)sender).Text = "0";
        }

        private void TextBoxRow_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int r))
            {
                if (r < 0) ((TextBox)sender).Text = "0";
                return;
            }
            ((TextBox)sender).Text = "0";
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            ButtonOK.IsEnabled = false;
            if (type == Type.Info)
            {
                if (TextBoxName.Text.Length == 0) return;
            }
            int w = (bool)RadioButtonVertical.IsChecked ? int.Parse(TextBoxNumberChoice.Text) : int.Parse(TextBoxNumberAns.Text);
            int h = (bool)RadioButtonVertical.IsChecked ? int.Parse(TextBoxNumberAns.Text) : int.Parse(TextBoxNumberChoice.Text);
            int x = int.Parse(TextBoxCol.Text);
            int y = int.Parse(TextBoxRow.Text);
            if (y + h > template.RowSize.Count) return;
            for (int i = y; i < y + h; i++)
            {
                if (x + w > template.RowSize[i]) return;
            }
            ButtonOK.IsEnabled = true;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            callback(new AnswerSheetChecker.Template.TemplateData(TextBoxName.Text, (bool)RadioButtonVertical.IsChecked ? AnswerSheetChecker.Template.TemplateData.Type.Vertical : AnswerSheetChecker.Template.TemplateData.Type.Horizontal, int.Parse(TextBoxNumberChoice.Text), int.Parse(TextBoxNumberAns.Text), int.Parse(TextBoxCol.Text), int.Parse(TextBoxRow.Text)));
            Close();
        }
    }
}