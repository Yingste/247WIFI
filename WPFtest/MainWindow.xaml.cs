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

            //iterate through the list, throw out blank and add them to the drop down
            int numAdap = 0;
            foreach(String a in result)
            {
                numAdap++;
                if(a != "")
                {
                    IntPick.Items.Add(a);
                }
                if (numAdap == 2) { SetAdv(); }
            }
            IntPick.SelectedIndex = 0;


            //Lets start grabbing a list of all connection profiles that we have
            string tDir = Directory.GetCurrentDirectory() + "\\Tools";
            string fOut = "";
            string[] dir = Directory.GetFiles(tDir);//Grab the contents of the tools folder

            //Cycle through each file in the dir
            foreach(string file in dir)
            {
                //we only need the file name and not the full path
                string[] aTemp = file.Split('\\');
                string temp = "";
                foreach (string a in aTemp)
                {
                    temp = a;
                }

                fOut += temp + "\n";
                WifiPick.Items.Add(temp);//Add each file to the dropdown
            }
            WifiPick.SelectedIndex = 0;

            //Debugging
            //SettingsWin console3 = new SettingsWin();
            //console3.Show();
            //console3.COut.Text = "\n" + fOut;
        }

        private void buttonRun_Click(object sender, RoutedEventArgs e)

        {
            
            var environmentPath = Directory.GetCurrentDirectory();//System.Environment.GetEnvironmentVariable("PATH");
            
            int termNum = 0;
            String AdapName = "\"" + IntPick.Text + "\"";
            String arg1 = "";
            String arg3 = "";
            String arg4 = "";
            System.Diagnostics.Process Process1 = new System.Diagnostics.Process();
            System.Diagnostics.Process Process2 = new System.Diagnostics.Process();
            System.Diagnostics.Process Process3 = new System.Diagnostics.Process();
            System.Diagnostics.Process Process4 = new System.Diagnostics.Process();
            if (!AdvMode)
            {
                int.TryParse(TInput.Text, out termNum);
                termNum += 70;
                if (termNum >= 255) { termNum = 37;}
                //termNum = int.Parse(TInput.Text) + 70;//Moved to safer way of try parse. Should prevent errors
                arg1 = "/C netsh interface ip set address " + AdapName + " static 192.168.5." + termNum + "  255.255.255.0  192.168.5.1  1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no";
            }
            else
            {
                //If any field is empty we will fill it in with part of the default IP
                if (IPInput1.Text == "") { IPInput1.Text = "192";}
                if (IPInput2.Text == "") { IPInput2.Text = "168"; }
                if (IPInput3.Text == "") { IPInput3.Text = "5"; }
                if (IPInput4.Text == "") { IPInput4.Text = "37"; }
                //Compile the ip sub-parts into a single address
                string CIP = IPInput1.Text + "." + IPInput2.Text + "." + IPInput3.Text + "." + IPInput4.Text;
                string GIP = IPInput1.Text + "." + IPInput2.Text + "." + IPInput3.Text + ".1";
                arg1 = "/C netsh interface ip set address " + AdapName + "  static  " + CIP + "  255.255.255.0  " + GIP + "  1  ";
                arg1 += "& netsh interface ip add dns name=" + AdapName + " addr=1.1.1.1 validate=no & netsh interface ip add dns name=" + AdapName + " addr=8.8.8.8 index=2 validate=no ";
                
            }
            
            if((bool)LWifi.IsChecked)
            {
                //Check to see if we should be loading an IP address or not
                string wifiCmd = "CD /D " + environmentPath + " & netsh wlan add profile filename=\"Tools\\" + WifiPick.Text + "\"";
                if(DHCP.IsChecked == false)
                {
                    arg1 += "& " + wifiCmd;
                }
                else
                {
                    //if no IP address is needed clear out our previous command queue 
                    arg1 = "/C " + wifiCmd;
                }
                
            }

            System.Diagnostics.ProcessStartInfo myProcessInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                Arguments = arg1 + "& exit", 
                Verb = "runas" //The process should start with elevated permissions
            };

            //need wifi connect command to process first
            System.Diagnostics.ProcessStartInfo myProcessInfo2 = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                Arguments = "/c netsh wlan connect name=Brinks & netsh wlan connect name=Brinks & ping 127.0.0.1 -n 6 > nul" + "& exit",
                Verb = "runas" //The process should start with elevated permissions
            };
            myProcessInfo2.CreateNoWindow = true;

            //I need to check to see if we have internet after we run the tool
            //General idea is that if no response from ping after 10 sec set to dhcp and reconnect to old wifi
            //ping -n 1 192.168.1.1 | find "TTL=" >nul & if errorlevel 1 (netsh interface ip set address "adapName" dhcp) else (echo host reachable)
            arg3 += "/c ping -n 1 8.8.8.8 | find \"TTL=\" >nul & if errorlevel 1 (netsh interface ip set address " + AdapName + " dhcp) else (echo host reachable)";
            arg4 += "/c ping -n 1 8.8.8.8 | find \"TTL=\" >nul & if errorlevel 1 (netsh wlan delete profile name=Brinks & echo \"deleted\") else (echo host reachable)";


            System.Diagnostics.ProcessStartInfo myProcessInfo3 = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                Arguments = arg3 + "& exit",
                Verb = "runas" //The process should start with elevated permissions
            };
            myProcessInfo3.CreateNoWindow = true;
            System.Diagnostics.ProcessStartInfo myProcessInfo4 = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                Arguments = arg4 + "& exit",
                Verb = "runas" //The process should start with elevated permissions
            };
            myProcessInfo4.CreateNoWindow = true;
            //debuging window
            //In debug mode do not run the script just display it in a new window
            if (CBDebug.IsChecked == true)
            {
                SettingsWin console2 = new SettingsWin();
                console2.Show();
                console2.COut.Text = "\n" + arg3;
            }
            else//If not in debug mode run the script
            {
                Process1.StartInfo = myProcessInfo;
                Process1.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Process1.Start();
                Process1.WaitForExit();
                //System.Diagnostics.Process.Start(myProcessInfo);//run the settings through
                Process2.StartInfo = myProcessInfo2;
                Process2.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                if ((bool)LWifi.IsChecked) { Process2.Start(); Process2.WaitForExit(); };//connec to the wifi

                //System.Diagnostics.Process.Start(myProcessInfo);//check to make sure the con worked
                Process3.StartInfo = myProcessInfo3;
                Process3.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Process3.Start();
                Process3.WaitForExit();

                Process4.StartInfo = myProcessInfo4;
                Process4.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Process4.Start();
                Process4.WaitForExit();
            }
            
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
