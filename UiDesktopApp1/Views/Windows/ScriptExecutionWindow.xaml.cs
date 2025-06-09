using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.Views.Windows;

public partial class ScriptExecutionWindow : Window
{
    private readonly ScriptInfo _info;
    private readonly Dictionary<string, Control> _controls = new();

    public ScriptExecutionWindow(ScriptInfo info)
    {
        InitializeComponent();
        _info = info;
        BuildInputs();
    }

    private void BuildInputs()
    {


        var inputs = _info.Definition?["inputs"] as JArray;
        if (inputs == null) return;
        foreach (var item in inputs)
        {
            var labelText = item.Value<string>("label") ?? item.Value<string>("name");
            var name = item.Value<string>("name") ?? string.Empty;
            var type = item.Value<string>("ControlType")?.ToLower();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) continue;

            var label = new TextBlock { Text = labelText, Margin = new Thickness(0,5,0,0) };
            InputsPanel.Children.Add(label);

            Control? control = null;
            switch (type)
            {
                case "textbox":
                    control = new TextBox { Text = item.Value<string>("default") ?? string.Empty };
                    break;
                case "checkedlistbox":
                    var lb = new ListBox { SelectionMode = SelectionMode.Multiple, Height = 100 };
                    foreach (var choice in item["items"] as JArray ?? new JArray())
                    {
                        var chk = new CheckBox { Content = choice["Label"]?.ToString() ?? choice.ToString(), Tag = choice["ID"]?.ToString() };
                        lb.Items.Add(chk);
                    }
                    control = lb;
                    break;
                case "combobox":
                    var cb = new ComboBox();
                    foreach (var choice in item["items"] as JArray ?? new JArray())
                    {
                        cb.Items.Add(choice.ToString());
                    }
                    cb.SelectedIndex = 0;
                    control = cb;
                    break;
                case "datetimepicker":
                    control = new DatePicker();
                    break;
            }
            if (control != null)
            {
                _controls[name] = control;
                InputsPanel.Children.Add(control);
            }
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void Execute_Click(object sender, RoutedEventArgs e)
    {
        ExecuteButton.IsEnabled = false;
        OutputBox.Clear();

        try
        {
            var client = new HttpClient();
            var script = await client.GetStringAsync(_info.SourceUrl);
            using var ps = PowerShell.Create();
            ps.AddScript(script);
            foreach (var kvp in _controls)
            {
                object? val = null;
                switch (kvp.Value)
                {
                    case TextBox tb:
                        val = tb.Text; break;
                    case ComboBox cb:
                        val = cb.SelectedItem; break;
                    case DatePicker dp:
                        val = dp.SelectedDate; break;
                    case ListBox lb:
                        val = lb.Items.OfType<CheckBox>().Where(c => c.IsChecked == true).Select(c => c.Tag ?? c.Content).ToArray();
                        break;
                }
                ps.AddParameter(kvp.Key, val);
            }
            var results = await Task.Run(() => ps.Invoke());
            foreach (var r in results)
            {
                OutputBox.AppendText(r?.ToString() + "\n");
            }
            foreach (var err in ps.Streams.Error)
            {
                OutputBox.AppendText(err.ToString() + "\n");
            }
        }
        catch (System.Exception ex)
        {
            OutputBox.AppendText(ex.ToString());
        }
        finally
        {
            ExecuteButton.IsEnabled = true;
        }
    }
}
