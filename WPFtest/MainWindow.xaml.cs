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
        bool SetIsShow = false;
        string oldT;

        public MainWindow()
        {
            InitializeComponent();
            

        }


        private void buttonRun_Click(object sender, RoutedEventArgs e)

        {
            //string strCmdText;
            //strCmdText = "-ExecutionPolicy Bypass -File 247admin.ps1";
            //System.Diagnostics.Process.Start("Powershell.exe", strCmdText);
            var environmentPath = Directory.GetCurrentDirectory();//System.Environment.GetEnvironmentVariable("PATH");
            
            int termNum = int.Parse(TInput.Text) + 70;
            String AdapName = "\"Wi - Fi\"";
            String arg1 = "";
            if(!AdvMode)
            {
                arg1 = "/C netsh interface ip set address " + AdapName + " static 192.168.5." + termNum + " 255.255.255.0 192.168.5.1 1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no";
            }
            else
            {
                string CIP = IPInput1 + "." + IPInput2 + "." + IPInput3 + "." + IPInput4;
                string GIP = IPInput1 + "." + IPInput2 + "." + IPInput3 + ".1";
                arg1 = "/C netsh interface ip set address " + AdapName + " static " + CIP + " 255.255.255.0 192.168.5.1 1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no ";
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


            System.Diagnostics.ProcessStartInfo myProcessInfo2 = new System.Diagnostics.ProcessStartInfo
            {
                FileName = /*Environment.ExpandEnvironmentVariables("%SystemRoot%") + @"\System32\*/"cmd.exe", //Sets the FileName property of myProcessInfo to %SystemRoot%\System32\cmd.exe where %SystemRoot% is a system variable which is expanded using Environment.ExpandEnvironmentVariables
                //Arguments = " /C Powershell.exe -ExecutionPolicy Bypass -File 247admin.ps1", //Sets the arguments to cd..
                UseShellExecute = true,
                Arguments = "/C CD /D " + environmentPath + " & netsh wlan add profile filename=\"Tools\\ConInfo.xml\"",
                Verb = "runas" //The process should start with elevated permissions
            }; 
            System.Diagnostics.Process.Start(myProcessInfo2); 



        }



        private void buttonSettings_Click(object sender, RoutedEventArgs e)

        {
            
            
                setAdv();
           

        }

        private void setAdv()
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
