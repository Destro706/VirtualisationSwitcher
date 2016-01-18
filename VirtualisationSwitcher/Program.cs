using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualisationSwitcher {
    static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]

        private static bool isHyperVenabled() {
            bool enabled = false;
            string bcdeditTxt = "bcdedit.txt";
            string bcdeditTxtOutput = null;

            if (!File.Exists(bcdeditTxt)) {
                createBcdeditTxt();
            }

            bcdeditTxtOutput = File.ReadAllText(bcdeditTxt).ToLower();

            if (bcdeditTxtOutput.Contains("auto")) {
                enabled = true;
            } else if (bcdeditTxtOutput.Contains("off")) {
                enabled = false;
            } else {
                MessageBox.Show("Es ist ein Fehler aufgetreten, die Anwendung wird beendet!");
                Environment.Exit(0);
            }

            return enabled;            
        }

        private static void createBcdeditTxt() {
            string createBcdeditTxtFile = "bcdedit.exe > bcdedit.txt";

            Process process = new Process();
            ProcessStartInfo runCommandAsAdmin = new ProcessStartInfo();

            runCommandAsAdmin.FileName = "cmd.exe";
            runCommandAsAdmin.Arguments = "/c " + createBcdeditTxtFile;
            runCommandAsAdmin.UseShellExecute = false;
            runCommandAsAdmin.CreateNoWindow = true;
            process.StartInfo = runCommandAsAdmin;
            process.Start();
            process.WaitForExit();
        }

        private static void switchHyperV(string switchCommand) {
            string warning = "SPEICHERN SIE IHRE ARBEIT, DER PC WIRD NEU GESTARTET!";
            string caption = "Virtualisation Switcher";
            
            Process process = new Process();
            ProcessStartInfo runCommandAsAdmin = new ProcessStartInfo();

            runCommandAsAdmin.FileName = "cmd.exe";
            runCommandAsAdmin.Arguments = "/c " + switchCommand;
            runCommandAsAdmin.UseShellExecute = false;
            runCommandAsAdmin.CreateNoWindow = true;
            process.StartInfo = runCommandAsAdmin;
            process.Start();
            process.WaitForExit();

            MessageBox.Show(warning, caption, MessageBoxButtons.OK ,MessageBoxIcon.Warning);
            var restartWindowsCommand = new ProcessStartInfo("shutdown", "/s /t 0");
            restartWindowsCommand.UseShellExecute = false;
            restartWindowsCommand.CreateNoWindow = true;
            Process.Start(restartWindowsCommand);
        }

        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string hyper_V_on = "bcdedit.exe /set hypervisorlaunchtype auto";
            string hyper_V_off = "bcdedit.exe /set hypervisorlaunchtype off";

            string questionIfEnabled = "                    Hyper-V ist aktiviert!\n            Möchten Sie Hyper-V deaktivieren?\n                    (PC wird neu gestartet)";
            string questionIfDisabled = "                    Hyper-V ist deaktiviert!\n            Möchten Sie Hyper-V aktivieren?\n                    (PC wird neu gestartet)";
            
            string caption = "Virtualisation Switcher";

            createBcdeditTxt();
            var hyperVenabled = isHyperVenabled();

            if (hyperVenabled == true) {
                var result = MessageBox.Show(questionIfEnabled, caption, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) {
                    switchHyperV(hyper_V_off);
                } else if (result == DialogResult.No) {
                    Environment.Exit(0);
                }
            } else {
                var result = MessageBox.Show(questionIfDisabled, caption, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) {
                    switchHyperV(hyper_V_on);
                } else if (result == DialogResult.No) {
                    Environment.Exit(0);
                }
            }
        }
    }
}
