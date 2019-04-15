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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;

namespace WPFtest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 






    public partial class MainWindow : Window
    {

        bool AdvMode = false;
        SettingsWin console = new SettingsWin();

        



    public MainWindow()
        {
            InitializeComponent();

            //Try to grab a list of the network adapters and put them into a list called result
            string NetAdArg = "/C @echo off & for /F \"skip=3 tokens=3* \" %G in ('netsh interface show interface') do echo %H";
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.FileName = "cmd.exe";
            pProcess.StartInfo.Arguments = NetAdArg;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
            List<string> result = strOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            //Add the adapter to the dropdown list
            for (int i = 0; i < result.Count; i++)
            {
                IntPick.Items.Add(result[i]);
            }

            IntPick.SelectedIndex = 0;


        }





        private void buttonRun_Click(object sender, RoutedEventArgs e)

        {
            
            var environmentPath = Directory.GetCurrentDirectory();//System.Environment.GetEnvironmentVariable("PATH");
            
            int termNum = 0;
            //String AdapName = "\"Wi - Fi\"";
            String AdapName = "\"" + IntPick.Text + "\"";
            String arg1 = "";
            if(!AdvMode)
            {
                termNum = int.Parse(TInput.Text) + 70;
                arg1 = "/C netsh interface ip set address " + AdapName + " static 192.168.5." + termNum + "  255.255.255.0  192.168.5.1  1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no";
            }
            else
            {
                string CIP = IPInput1.Text + "." + IPInput2.Text + "." + IPInput3.Text + "." + IPInput4.Text;
                string GIP = IPInput1.Text + "." + IPInput2.Text + "." + IPInput3.Text + ".1";
                arg1 = "/K netsh interface ip set address " + AdapName + "  static  " + CIP + "  255.255.255.0  192.168.5.1  1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no ";
                
            }
            
            if((bool)LWifi.IsChecked)
            {
                arg1 += "& CD /D " + environmentPath + " & netsh wlan add profile filename=\"Tools\\ConInfo.xml\"";
            }

            System.Diagnostics.ProcessStartInfo myProcessInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe", 
                //Arguments = " /C Powershell.exe -ExecutionPolicy Bypass -File 247admin.ps1", 
                Arguments = arg1, 
                Verb = "runas" //The process should start with elevated permissions
            };
            System.Diagnostics.Process.Start(myProcessInfo); 


            


            

            //debuging window
            //console.Show();
            //console.COut.Text = arg1;
            //console.COut.Text = strOutput;

        }



        private void buttonSettings_Click(object sender, RoutedEventArgs e)

        {
            SetAdv();
        }

        private void SetAdv()
        {
            AdvMode = !AdvMode;
            if (AdvMode)
            {
                


                MTitle.Text = "Enter IP Address of Terminal";
                AdvGroup.Visibility = Visibility.Visible;
                
            }
            else
            {
                MTitle.Text = "Enter the BSL Terminal Number";
                AdvGroup.Visibility = Visibility.Hidden;
                
            }


        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            //Show the info Window
            WinInfo info = new WinInfo();
            info.Show();

        }
    }
}








/*
           string psCommand1 = System.IO.File.ReadAllText(@"C:\Users\yings\test.ps1");
           using (PowerShell PowerShellInstance = PowerShell.Create())
           {
               // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
               // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
               PowerShellInstance.AddScript(psCommand1);

               Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
               foreach (PSObject outputItem in PSOutput)
               {
                   // if null object was dumped to the pipeline during the script then a null
                   // object may be present here. check for null to prevent potential NRE.
                   if (outputItem != null)
                   {
                       Console.WriteLine(outputItem.BaseObject.ToString() + "\n");
                   }
               }
               if (PowerShellInstance.Streams.Error.Count > 0)
               {
                   Console.Write("Error");
               }
               Console.ReadLine();
           }
           */
