using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace EnglishUIChanger
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //form1
            //
            //original width=582, height=465.
            this.Height = 180;
            this.Text = "LOL Gadget";
            //button
            //
            button1.Text = "英文介面";
            button2.Text = "中文介面";
            button3.Text = "偵測路徑";
            button4.Text = "儲存更改";
            button5.Text = "Diamond HUD";
            button6.Text = "Challenger HUD";
            button7.Text = "Champ. Thresh HUD";
            button8.Text = "原始介面";
            btnSelectTGAfile.Text = "選取檔案";
            button1.Click += new EventHandler(changeLanguege);
            button2.Click += new EventHandler(changeLanguege);
            button5.Click += new EventHandler(changeUI);
            button6.Click += new EventHandler(changeUI);
            button7.Click += new EventHandler(changeUI);
            button8.Click += new EventHandler(changeUI);
            btnSelectTGAfile.Click += new EventHandler(changeUI);
            //checkbox
            //
            checkBox1.Text = "是否備份檔案(第一次使用請勾選)";
            //groupbox
            //
            groupBox1.Text = "";
            groupBox1.Enabled = false;
            groupBox2.Text = "GarenaLOLTW路徑";
            groupBox3.Text = "";
            groupBox3.Enabled = false;
            groupBox4.Text = "精選UI";
            groupBox4.Enabled = false;
            groupBox5.Text = "自定義";
            groupBox5.Enabled = false;
            //textbox
            //
            textBox1.Text = "英雄聯盟(League of Legends)小工具\r\n\r\n\r\n" +
                            "*請確認遊戲路徑是否正確(完整)\r\n" +
                            "*每次遊戲更新時，請再做一次轉換介面\r\n" +
                            "\r\n\r\n※僅適用於Windows OS";
            //label
            //
            label1.Text = "金錢";
            label2.Text = "經驗值";
            label3.Text = "我方傷害\r\n(含真實傷害、技能傷害)";
            label4.Text = "我方爆擊";
            label5.Text = "※建議數值範圍 20~70";
            label6.Text = "補魔力值";
            label7.Text = "治療";
            lblAboutMe.Text = "by JackWang";
            lblGroup1Description.Text = "";
            lblGroup5Description.Text = "請選取自己欲更換的.tga檔";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //call detectLOLPath method to find user's LOL path
            if ((textBox2.Text = detectLOLPath()) == "")
            {
                MessageBox.Show("偵測不到路徑，請手動選取", "LOL Gadget");

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.ShowDialog();
                textBox2.Text = fbd.SelectedPath;
            }

            groupBox1.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
            groupBox5.Enabled = true;
            this.Height = 465;

            loadFontResolutions();
            checkStates();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //驗證數值
            Regex reg = new Regex("^[0-9]{2}$");
            Match m1 = reg.Match(textBox3.Text.ToString());
            Match m2 = reg.Match(textBox4.Text.ToString());
            Match m3 = reg.Match(textBox5.Text.ToString());
            Match m4 = reg.Match(textBox6.Text.ToString());
            Match m5 = reg.Match(textBox7.Text.ToString());
            Match m6 = reg.Match(textBox8.Text.ToString());

            if (!m1.Success || !m2.Success || !m3.Success || !m4.Success || !m5.Success || !m6.Success)
            {
                MessageBox.Show("請輸入正確數值！", "LOL Gadget");
                return;
            }

            //load FontResolutions.xml
            string path = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\CFG\defaults\FontResolutions.xml");
            if (!File.Exists(path))
                return;

            try 
	        {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNode node_Gold = doc.SelectSingleNode("//FontResolution[@Name='gold_toast']/locale/Resolution");
                XmlNode node_Experience = doc.SelectSingleNode("//FontResolution[@Name='19_1_Auto']/locale/Resolution");
                XmlNode node_Damage = doc.SelectSingleNode("//FontResolution[@Name='27_1_Auto']/locale/Resolution");
                XmlNode node_Critical = doc.SelectSingleNode("//FontResolution[@Name='36_4_Auto']/locale/Resolution");
                XmlNode node_ManaHeal = doc.SelectSingleNode("//FontResolution[@Name='9_1_Auto']/locale/Resolution");
                XmlNode node_Heal = doc.SelectSingleNode("//FontResolution[@Name='24_2_Auto']/locale/Resolution");

                XmlElement element_Gold = (XmlElement)node_Gold;
                XmlElement element_Experience = (XmlElement)node_Experience;
                XmlElement element_Damage = (XmlElement)node_Damage;
                XmlElement element_Critical = (XmlElement)node_Critical;
                XmlElement element_ManaHeal = (XmlElement)node_ManaHeal;
                XmlElement element_Heal = (XmlElement)node_Heal;

                element_Gold.SetAttribute("FontSize", textBox3.Text);
                element_Experience.SetAttribute("FontSize", textBox4.Text);
                element_Damage.SetAttribute("FontSize", textBox5.Text);
                element_Critical.SetAttribute("FontSize", textBox6.Text); //36_4_Auto
                element_ManaHeal.SetAttribute("FontSize", textBox7.Text);
                element_Heal.SetAttribute("FontSize", textBox8.Text);

                doc.Save(path);
	        }
	        catch (Exception ex)
	        {
                MessageBox.Show(ex.StackTrace);
	        }

            MessageBox.Show("修改成功", "LOL Gadget");
        }

        /// <summary>
        /// 偵測LOL路徑
        /// </summary>
        /// <returns>LOL's path</returns>
        private string detectLOLPath()
        {
            //Get all driversInfo
            DriveInfo[] listDrivesInfo = DriveInfo.GetDrives();

            try
            {
                foreach (DriveInfo drives in listDrivesInfo)
                {
                    string[] dir;

                    //優先搜尋 C:\
                    if (drives.Name == @"C:\")
                    {
                        //搜尋最有可能之路徑 C:\Program Files (x86)
                        dir = Directory.GetDirectories(drives.Name + "Program Files (x86)");
                        foreach (string dirs in dir)
                        {
                            if (dirs.Substring(dirs.LastIndexOf("\\") + 1) == "GarenaLoLTW")
                            {
                                return dirs;
                            }
                        }

                        //搜尋最有可能之路徑 C:\Program Files
                        dir = Directory.GetDirectories(drives.Name + "Program Files");
                        foreach (string dirs in dir)
                        {
                            if (dirs.Substring(dirs.LastIndexOf("\\") + 1) == "GarenaLoLTW")
                            {
                                return dirs;
                            }
                        }
                    }
                    else
                    {
                        dir = Directory.GetDirectories(drives.Name);
                        foreach (string dirs in dir)
                        {
                            if (dirs.Substring(dirs.LastIndexOf("\\") + 1) == "GarenaLoLTW")
                            {
                                return dirs;
                            }
                        }
                    }

                }//end foreach
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "";
        }

        //讀取浮體字數值
        private void loadFontResolutions()
        {
            //load FontResolutions.xml
            string path = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\CFG\defaults\FontResolutions.xml");
            if (!File.Exists(path))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode node_Gold = doc.SelectSingleNode("//FontResolution[@Name='gold_toast']/locale/Resolution");
            XmlNode node_Experience = doc.SelectSingleNode("//FontResolution[@Name='19_1_Auto']/locale/Resolution");
            XmlNode node_Damage = doc.SelectSingleNode("//FontResolution[@Name='27_1_Auto']/locale/Resolution");
            XmlNode node_Critical = doc.SelectSingleNode("//FontResolution[@Name='36_4_Auto']/locale/Resolution");
            XmlNode node_TrueDamage = doc.SelectSingleNode("//FontResolution[@Name='27_1_Auto']/locale/Resolution");
            XmlNode node_Heal = doc.SelectSingleNode("//FontResolution[@Name='24_2_Auto']/locale/Resolution");

            textBox3.Text = node_Gold.Attributes["FontSize"].Value;
            textBox4.Text = node_Experience.Attributes["FontSize"].Value;
            textBox5.Text = node_Damage.Attributes["FontSize"].Value;
            textBox6.Text = node_Critical.Attributes["FontSize"].Value;
            textBox7.Text = node_TrueDamage.Attributes["FontSize"].Value;
            textBox8.Text = node_Heal.Attributes["FontSize"].Value;  
        }

        //判斷現在的介面狀況
        private void checkStates()
        {
            string path = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\CFG\Locale.cfg");
            if (!File.Exists(path))
                return;

            using (var sr = new StreamReader(path))
            {
                string line = sr.ReadToEnd();

                if (line.Contains("zh_TW"))
                {
                    lblGroup1Description.Text = "目前為＜中文介面＞";
                    button1.Enabled = true;
                    button2.Enabled = false;
                    checkBox1.Enabled = true;
                }
                else if(line.Contains("en_US"))
                {
                    lblGroup1Description.Text = "目前為＜英文介面＞";
                    button1.Enabled = false;
                    button2.Enabled = true;
                    checkBox1.Enabled = false;
                }
                else
	            {
                    MessageBox.Show("目前介面不明！", "LOL Gadget");
                    groupBox2.Enabled = false;
	            }
            }
        }

        //更換UI
        private void changeUI(object sender,EventArgs e)
        {
            Button btn = (Button)sender;
            string selectedUIName = "";
            string src, des;   

            switch (btn.Name)
            {
                case "button5":
                    selectedUIName = "Diamond.tga";
                    break;
                case "button6":
                    selectedUIName = "Challenger.tga";
                    break;
                case "button7":
                    selectedUIName = "ChampionshipThresh.tga";
                    break;
                case "button8":
                    selectedUIName = "Origin.tga";
                    break;
                //自定義
                case "btnSelectTGAfile":
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.InitialDirectory = "C:\\";
                        ofd.Filter = "tga files (*.tga)|*.tga";

                        if (ofd.ShowDialog() == DialogResult.OK && Path.GetExtension(ofd.FileName) == ".tga")
                        {
                            string fileName = Path.GetFileName(ofd.FileName);
                            DialogResult result = MessageBox.Show("確定要換成" + fileName + "？", "LOL Gadget", MessageBoxButtons.YesNo);

                            if (result != DialogResult.Yes)
                                return;

                            src = ofd.FileName;
                            des = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\Menu\Textures\HUDAtlas.tga");

                            File.Copy(src, des, true);

                            MessageBox.Show(fileName + " 轉換成功！", "LOL Gadget");
                        }
                        return ;
                default:
                    return;
            }

            src = @"UI\" + selectedUIName;
            des = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\Menu\Textures\HUDAtlas.tga");

            if (File.Exists(src))
            {
                File.Copy(src, des, true);
                MessageBox.Show(btn.Text + " 轉換成功！", "LOL Gadget");
            }
            else
                MessageBox.Show("請確認檔案路徑：" + src, "LOL Gadget");
        }

        //更改UI語言
        private void changeLanguege(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            //check 
            DialogResult result;
            if (btn.Name == "button1")
                result = MessageBox.Show("確定要轉為英文介面？", "LOL Gadget", MessageBoxButtons.YesNo);
            else
                result = MessageBox.Show("確定要轉為中文介面？", "LOL Gadget", MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes) return;

            //Step 1.
            //modify Locale.cfg in the directory -> "GarenaLoLTW\GameData\Apps\LoLTW\Game\DATA\CFG"
            string filepath = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\CFG\Locale.cfg");

            if (File.Exists(filepath))
            {
                //轉英文
                if (btn.Name == "button1")
                {
                    File.WriteAllText(filepath, File.ReadAllText(filepath).Replace("zh_TW", "en_US"));
                    //(Champions Pick)
                    filepath = @"C:\Program Files (x86)\GarenaLoLTW\GameData\Apps\LoLTW\Air\locale.properties";
                    File.WriteAllText(filepath, File.ReadAllText(filepath).Replace("zh_TW", "en_US"));
                }
                else//轉中文
                {
                    File.WriteAllText(filepath, File.ReadAllText(filepath).Replace("en_US", "zh_TW"));
                    filepath = @"C:\Program Files (x86)\GarenaLoLTW\GameData\Apps\LoLTW\Air\locale.properties";
                    File.WriteAllText(filepath, File.ReadAllText(filepath).Replace("en_US", "zh_TW"));
                }
            }
            else
            {
                MessageBox.Show("請確認檔案路徑：" + textBox2.Text + @"\GameData\Apps\LoLTW\Game\DATA\CFG\Locale.cfg", "LOL Gadget");
                return;
            }

            //Step 2. (In Game Fonts)
            //modify fontconfig_en_US.txt in the directory -> "GarenaLoLTW\GameData\Apps\LoLTW\Game\DATA\Menu"
            filepath = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Game\DATA\Menu\fontconfig_en_US.txt");

            if (File.Exists(filepath))
            {
                string temp = Path.GetTempFileName();
                using (var sr = new StreamReader(filepath))
                using (var sw = new StreamWriter(temp))
                {
                    string str;

                    if (btn.Name == "button1")
                    {
                        str = "[FontConfig \"English\"]" + Environment.NewLine +
                                "fontlib \"fonts_tw.swf\"" + Environment.NewLine +
                                "map \"$ButtonFont\" = \"FZXHYSZK\"" + Environment.NewLine +
                                "map \"$NormalFont\" = \"FZXHYSZK\"" + Environment.NewLine +
                                "map \"$TitleFont\" = \"FZXHYSZK\"" + Environment.NewLine +
                                "map \"$IMECandidateListFont\" = \"FZXHYSZK\"";
                    }
                    else
                    {
                        str = "[FontConfig \"English\"]" + Environment.NewLine +
                                "fontlib \"fonts_latin.swf\"" + Environment.NewLine +
                                "map \"$ButtonFont\" = \"Gill Sans MT Pro Medium\"" + Environment.NewLine +
                                "map \"$NormalFont\" = \"Gill Sans MT Pro Medium\"" + Environment.NewLine +
                                "map \"$TitleFont\" = \"Gill Sans MT Pro Medium\"" + Environment.NewLine +
                                "map \"$IMECandidateListFont\" = \"Gill Sans MT Pro Medium\"";
                    }

                    sw.WriteLine(str);

                    int index = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (++index > 6)
                            sw.WriteLine(line);
                    }
                }
                File.Delete(filepath);
                File.Move(temp, filepath);
            }
            else
            {
                MessageBox.Show("請確認檔案路徑：" + textBox2.Text + @"\GameData\Apps\LoLTW\Game\DATA\Menu\fontconfig_en_US.txt", "LOL Gadget");
                return;
            }
            //Step 3. (Champions Pick)
            //Copy fonts.swf and font_zh_TW.swf to the directory -> "GarenaLOLTW\GameData\Apps\LoLTW\Air\css"

            //轉英
            string src = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts_zh_TW.swf");
            string des = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts.swf");

            //轉中
            string file1 = @"Backups\fonts.swf"; //從Backups中複製
            string file2 = @"Backups\fonts_zh_TW.swf";//同上
                
            try
            {
                if (!File.Exists(des))
                    return;

                if (btn.Name == "button1")
                {
                    //if BackUp clicked
                    if (checkBox1.Checked)
                    {
                        string backupFile1 = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts.swf");
                        string backupFile2 = Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts_zh_TW.swf");
                        string BackupDes = @"Backups";

                        if (!Directory.Exists(BackupDes))
                            Directory.CreateDirectory(BackupDes);

                        if(!File.Exists(Path.Combine(BackupDes, "fonts.swf")))
                            File.Copy(backupFile1, Path.Combine(BackupDes, Path.GetFileName(backupFile1)), true);
                        if(!File.Exists(Path.Combine(BackupDes, "fonts_zh_TW.swf")))
                            File.Copy(backupFile2, Path.Combine(BackupDes, Path.GetFileName(backupFile2)), true);

                        MessageBox.Show("已備份檔案至Backups資料夾下", "LOL Gadget");
                    }

                    if (!File.Exists(src))
                        src = @"Backups\fonts_zh_TW.swf";

                    File.Copy(src, des, true);
                }
                else
                {
                    File.Copy(file1, Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts.swf"), true);
                    File.Copy(file2, Path.Combine(textBox2.Text, @"GameData\Apps\LoLTW\Air\css\fonts_zh_TW.swf"), true);
                }

                MessageBox.Show("已切換為" + btn.Text, "LOL Gadget");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }

            checkStates();
         }
    }
}
