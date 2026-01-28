namespace Local_Backup_Creator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            textBoxSrcFolder = new TextBox();
            checkBoxCompMeta = new CheckBox();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            buttonCancel = new Button();
            label2 = new Label();
            textBoxDstFolder = new TextBox();
            checkBoxLOG = new CheckBox();
            pictureBoxORNAMENT = new PictureBox();
            label4 = new Label();
            textBoxLocationLOG = new TextBox();
            checkBoxSubfolder = new CheckBox();
            label3 = new Label();
            textBoxSubfolder = new TextBox();
            checkBoxSrcMTP = new CheckBox();
            progressBarBackup = new ProgressBar();
            labelProgress = new Label();
            pictureBoxGIF = new PictureBox();
            checkBoxDstMTP = new CheckBox();
            checkBoxLOGMTP = new CheckBox();
            toolTip1 = new ToolTip(components);
            buttonSaveConfig = new Button();
            buttonStart = new Button();
            buttonSelectSrcFolder = new Button();
            buttonSelectSubfolder = new Button();
            buttonSelectDstFolder = new Button();
            buttonSelectLOG = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBoxORNAMENT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGIF).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSrcFolder
            // 
            textBoxSrcFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSrcFolder.BackColor = SystemColors.Window;
            textBoxSrcFolder.Location = new Point(34, 52);
            textBoxSrcFolder.Margin = new Padding(2);
            textBoxSrcFolder.MaximumSize = new Size(6554, 27);
            textBoxSrcFolder.MinimumSize = new Size(370, 27);
            textBoxSrcFolder.Name = "textBoxSrcFolder";
            textBoxSrcFolder.PlaceholderText = "Example : C:\\Users\\public or C:\\My Backup";
            textBoxSrcFolder.Size = new Size(373, 27);
            textBoxSrcFolder.TabIndex = 3;
            toolTip1.SetToolTip(textBoxSrcFolder, "Displays the selected source folder path.");
            // 
            // checkBoxCompMeta
            // 
            checkBoxCompMeta.AutoSize = true;
            checkBoxCompMeta.BackColor = Color.Transparent;
            checkBoxCompMeta.Location = new Point(34, 21);
            checkBoxCompMeta.Margin = new Padding(2);
            checkBoxCompMeta.Name = "checkBoxCompMeta";
            checkBoxCompMeta.Size = new Size(147, 19);
            checkBoxCompMeta.TabIndex = 4;
            checkBoxCompMeta.Text = "Compare file metadata";
            toolTip1.SetToolTip(checkBoxCompMeta, "If checked, the program compares file details (size, modification date, etc.) before copying to make the backup more accurate.");
            checkBoxCompMeta.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(34, 10);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(159, 15);
            label1.TabIndex = 7;
            label1.Text = "Location of the source folder";
            toolTip1.SetToolTip(label1, "Choose the main folder you want to back up.");
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.Fuchsia;
            linkLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Transparent;
            linkLabel1.DisabledLinkColor = Color.MidnightBlue;
            linkLabel1.LinkBehavior = LinkBehavior.AlwaysUnderline;
            linkLabel1.LinkColor = Color.DarkSlateBlue;
            linkLabel1.Location = new Point(365, 7);
            linkLabel1.Margin = new Padding(2, 0, 2, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(117, 15);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "View the instructions";
            toolTip1.SetToolTip(linkLabel1, "Opens the documentation file explaining in depth how to use this program.");
            linkLabel1.LinkClicked += OpenInstructions;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonCancel.BackColor = SystemColors.Window;
            buttonCancel.ForeColor = Color.FromArgb(192, 0, 0);
            buttonCancel.Location = new Point(211, 17);
            buttonCancel.Margin = new Padding(2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 26);
            buttonCancel.TabIndex = 9;
            buttonCancel.Text = "Cancel";
            toolTip1.SetToolTip(buttonCancel, "Closes the program and interrupts the backup process");
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += ButtonCancelClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(34, 15);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(183, 15);
            label2.TabIndex = 11;
            label2.Text = "Location of the destination folder";
            toolTip1.SetToolTip(label2, "Choose where the backup files will be copied to. This can be a local folder, an external drive, or a connected MTP device.");
            // 
            // textBoxDstFolder
            // 
            textBoxDstFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDstFolder.Location = new Point(34, 55);
            textBoxDstFolder.Margin = new Padding(2);
            textBoxDstFolder.MaximumSize = new Size(6554, 27);
            textBoxDstFolder.MinimumSize = new Size(370, 27);
            textBoxDstFolder.Name = "textBoxDstFolder";
            textBoxDstFolder.PlaceholderText = "Example : D:\\My Backup or \\\\NAS-01\\shared-files\\backup";
            textBoxDstFolder.Size = new Size(373, 27);
            textBoxDstFolder.TabIndex = 10;
            toolTip1.SetToolTip(textBoxDstFolder, "Displays the path where the backup will be created.");
            // 
            // checkBoxLOG
            // 
            checkBoxLOG.AutoSize = true;
            checkBoxLOG.BackColor = Color.Transparent;
            checkBoxLOG.Location = new Point(34, 29);
            checkBoxLOG.Margin = new Padding(2);
            checkBoxLOG.Name = "checkBoxLOG";
            checkBoxLOG.Size = new Size(159, 19);
            checkBoxLOG.TabIndex = 12;
            checkBoxLOG.Text = "Generate a .log file report";
            toolTip1.SetToolTip(checkBoxLOG, "Enable this to create a detailed text log of the backup process.");
            checkBoxLOG.UseVisualStyleBackColor = false;
            checkBoxLOG.CheckStateChanged += CheckBoxLOGChanged;
            // 
            // pictureBoxORNAMENT
            // 
            pictureBoxORNAMENT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBoxORNAMENT.BackColor = Color.Transparent;
            pictureBoxORNAMENT.Image = (Image)resources.GetObject("pictureBoxORNAMENT.Image");
            pictureBoxORNAMENT.Location = new Point(7, 7);
            pictureBoxORNAMENT.Margin = new Padding(2);
            pictureBoxORNAMENT.MaximumSize = new Size(81, 410);
            pictureBoxORNAMENT.MinimumSize = new Size(81, 410);
            pictureBoxORNAMENT.Name = "pictureBoxORNAMENT";
            pictureBoxORNAMENT.Size = new Size(81, 410);
            pictureBoxORNAMENT.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxORNAMENT.TabIndex = 13;
            pictureBoxORNAMENT.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(34, 11);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(92, 15);
            label4.TabIndex = 15;
            label4.Text = "Log file location";
            toolTip1.SetToolTip(label4, "Specifies where the backup log (.log) file will be saved. The log records which files were copied, skipped, or failed.");
            // 
            // textBoxLocationLOG
            // 
            textBoxLocationLOG.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxLocationLOG.Location = new Point(34, 48);
            textBoxLocationLOG.Margin = new Padding(2);
            textBoxLocationLOG.MaximumSize = new Size(6554, 27);
            textBoxLocationLOG.MinimumSize = new Size(370, 27);
            textBoxLocationLOG.Name = "textBoxLocationLOG";
            textBoxLocationLOG.PlaceholderText = "Example : D:\\ or \\\\192.168.0.10\\data\\logs";
            textBoxLocationLOG.Size = new Size(373, 27);
            textBoxLocationLOG.TabIndex = 14;
            toolTip1.SetToolTip(textBoxLocationLOG, "Opens a folder picker or device selector to choose where to store the log file.");
            // 
            // checkBoxSubfolder
            // 
            checkBoxSubfolder.AutoSize = true;
            checkBoxSubfolder.BackColor = Color.Transparent;
            checkBoxSubfolder.Location = new Point(34, 26);
            checkBoxSubfolder.Margin = new Padding(2);
            checkBoxSubfolder.Name = "checkBoxSubfolder";
            checkBoxSubfolder.Size = new Size(284, 19);
            checkBoxSubfolder.TabIndex = 26;
            checkBoxSubfolder.Text = "Select specific subfolders inside the source folder";
            toolTip1.SetToolTip(checkBoxSubfolder, "Enable this if you only want to include certain subfolders from the source in the backup.");
            checkBoxSubfolder.UseVisualStyleBackColor = false;
            checkBoxSubfolder.CheckStateChanged += CheckBoxSubfolderChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(34, 8);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(203, 15);
            label3.TabIndex = 25;
            label3.Text = "Subfolders to include (separated by ;)";
            toolTip1.SetToolTip(label3, "If you only want specific subfolders inside the source folder.");
            // 
            // textBoxSubfolder
            // 
            textBoxSubfolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSubfolder.Location = new Point(34, 48);
            textBoxSubfolder.Margin = new Padding(2);
            textBoxSubfolder.MaximumSize = new Size(6554, 27);
            textBoxSubfolder.MinimumSize = new Size(370, 27);
            textBoxSubfolder.Name = "textBoxSubfolder";
            textBoxSubfolder.PlaceholderText = "Example : Documents;Downloads;Pictures;Videos";
            textBoxSubfolder.Size = new Size(372, 27);
            textBoxSubfolder.TabIndex = 24;
            toolTip1.SetToolTip(textBoxSubfolder, "Type the names of the subfolders to include, separated by the ; character.");
            // 
            // checkBoxSrcMTP
            // 
            checkBoxSrcMTP.AutoSize = true;
            checkBoxSrcMTP.BackColor = Color.Transparent;
            checkBoxSrcMTP.BackgroundImageLayout = ImageLayout.None;
            checkBoxSrcMTP.Location = new Point(34, 30);
            checkBoxSrcMTP.Margin = new Padding(2);
            checkBoxSrcMTP.Name = "checkBoxSrcMTP";
            checkBoxSrcMTP.Size = new Size(212, 19);
            checkBoxSrcMTP.TabIndex = 31;
            checkBoxSrcMTP.Text = "Source is a media device (use MTP)";
            toolTip1.SetToolTip(checkBoxSrcMTP, "Enable this if your source files are stored on a device connected by USB.");
            checkBoxSrcMTP.UseVisualStyleBackColor = false;
            checkBoxSrcMTP.CheckedChanged += CheckBoxSrcMTPChanged;
            // 
            // progressBarBackup
            // 
            progressBarBackup.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBarBackup.BackColor = Color.Blue;
            progressBarBackup.Location = new Point(34, 46);
            progressBarBackup.Margin = new Padding(2);
            progressBarBackup.MaximumSize = new Size(6554, 80);
            progressBarBackup.MinimumSize = new Size(447, 33);
            progressBarBackup.Name = "progressBarBackup";
            progressBarBackup.Size = new Size(448, 33);
            progressBarBackup.TabIndex = 28;
            // 
            // labelProgress
            // 
            labelProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelProgress.AutoSize = true;
            labelProgress.BackColor = Color.Transparent;
            labelProgress.Location = new Point(34, 80);
            labelProgress.Margin = new Padding(2, 0, 2, 0);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(172, 15);
            labelProgress.TabIndex = 29;
            labelProgress.Text = "Press Start to create the backup";
            toolTip1.SetToolTip(labelProgress, "Current status of the Backup.");
            // 
            // pictureBoxGIF
            // 
            pictureBoxGIF.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBoxGIF.BackColor = Color.Transparent;
            pictureBoxGIF.Image = (Image)resources.GetObject("pictureBoxGIF.Image");
            pictureBoxGIF.Location = new Point(338, 78);
            pictureBoxGIF.Margin = new Padding(2);
            pictureBoxGIF.MaximumSize = new Size(153, 39);
            pictureBoxGIF.MinimumSize = new Size(153, 39);
            pictureBoxGIF.Name = "pictureBoxGIF";
            pictureBoxGIF.Size = new Size(153, 39);
            pictureBoxGIF.TabIndex = 30;
            pictureBoxGIF.TabStop = false;
            // 
            // checkBoxDstMTP
            // 
            checkBoxDstMTP.AutoSize = true;
            checkBoxDstMTP.BackColor = Color.Transparent;
            checkBoxDstMTP.Location = new Point(34, 33);
            checkBoxDstMTP.Margin = new Padding(2);
            checkBoxDstMTP.Name = "checkBoxDstMTP";
            checkBoxDstMTP.Size = new Size(236, 19);
            checkBoxDstMTP.TabIndex = 32;
            checkBoxDstMTP.Text = "Destination is a media device (use MTP)";
            toolTip1.SetToolTip(checkBoxDstMTP, "Enable this if you want to back up directly to a connected device.");
            checkBoxDstMTP.UseVisualStyleBackColor = false;
            checkBoxDstMTP.CheckedChanged += CheckBoxDstMTPChanged;
            // 
            // checkBoxLOGMTP
            // 
            checkBoxLOGMTP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxLOGMTP.AutoSize = true;
            checkBoxLOGMTP.BackColor = Color.Transparent;
            checkBoxLOGMTP.Location = new Point(228, 29);
            checkBoxLOGMTP.Margin = new Padding(2);
            checkBoxLOGMTP.Name = "checkBoxLOGMTP";
            checkBoxLOGMTP.Size = new Size(178, 19);
            checkBoxLOGMTP.TabIndex = 33;
            checkBoxLOGMTP.Text = "Location is in a media device";
            toolTip1.SetToolTip(checkBoxLOGMTP, "Check this if you want to save the log file directly on a connected device instead of your computer.");
            checkBoxLOGMTP.UseVisualStyleBackColor = false;
            checkBoxLOGMTP.CheckedChanged += CheckBoxLOGMTPChanged;
            // 
            // buttonSaveConfig
            // 
            buttonSaveConfig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSaveConfig.BackColor = SystemColors.Window;
            buttonSaveConfig.ForeColor = Color.MediumBlue;
            buttonSaveConfig.Location = new Point(290, 17);
            buttonSaveConfig.Margin = new Padding(2);
            buttonSaveConfig.Name = "buttonSaveConfig";
            buttonSaveConfig.Size = new Size(114, 26);
            buttonSaveConfig.TabIndex = 34;
            buttonSaveConfig.Text = "Save configuration";
            toolTip1.SetToolTip(buttonSaveConfig, "Saves your current settings (source, destination, log location, and options) for next time you open the program.");
            buttonSaveConfig.UseVisualStyleBackColor = false;
            buttonSaveConfig.Click += SaveConfigClick;
            // 
            // buttonStart
            // 
            buttonStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonStart.BackColor = SystemColors.Window;
            buttonStart.ForeColor = Color.Green;
            buttonStart.Location = new Point(406, 17);
            buttonStart.Margin = new Padding(2);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(75, 26);
            buttonStart.TabIndex = 35;
            buttonStart.Text = "Start";
            toolTip1.SetToolTip(buttonStart, "Starts the backup process.");
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += StartBackup;
            // 
            // buttonSelectSrcFolder
            // 
            buttonSelectSrcFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSelectSrcFolder.BackColor = SystemColors.Window;
            buttonSelectSrcFolder.ForeColor = Color.RoyalBlue;
            buttonSelectSrcFolder.Location = new Point(408, 51);
            buttonSelectSrcFolder.Margin = new Padding(2);
            buttonSelectSrcFolder.Name = "buttonSelectSrcFolder";
            buttonSelectSrcFolder.Size = new Size(75, 23);
            buttonSelectSrcFolder.TabIndex = 39;
            buttonSelectSrcFolder.Text = "Select";
            toolTip1.SetToolTip(buttonSelectSrcFolder, "Select the corresponding path.");
            buttonSelectSrcFolder.UseVisualStyleBackColor = false;
            buttonSelectSrcFolder.Click += SelectFolder;
            // 
            // buttonSelectSubfolder
            // 
            buttonSelectSubfolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSelectSubfolder.BackColor = SystemColors.Window;
            buttonSelectSubfolder.ForeColor = Color.RoyalBlue;
            buttonSelectSubfolder.Location = new Point(408, 47);
            buttonSelectSubfolder.Margin = new Padding(2);
            buttonSelectSubfolder.Name = "buttonSelectSubfolder";
            buttonSelectSubfolder.Size = new Size(75, 23);
            buttonSelectSubfolder.TabIndex = 40;
            buttonSelectSubfolder.Text = "Select";
            toolTip1.SetToolTip(buttonSelectSubfolder, "Select the corresponding path.");
            buttonSelectSubfolder.UseVisualStyleBackColor = false;
            buttonSelectSubfolder.Click += SelectSubfolder;
            // 
            // buttonSelectDstFolder
            // 
            buttonSelectDstFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSelectDstFolder.BackColor = SystemColors.Window;
            buttonSelectDstFolder.ForeColor = Color.RoyalBlue;
            buttonSelectDstFolder.Location = new Point(408, 54);
            buttonSelectDstFolder.Margin = new Padding(2);
            buttonSelectDstFolder.Name = "buttonSelectDstFolder";
            buttonSelectDstFolder.Size = new Size(75, 23);
            buttonSelectDstFolder.TabIndex = 41;
            buttonSelectDstFolder.Text = "Select";
            toolTip1.SetToolTip(buttonSelectDstFolder, "Select the corresponding path.");
            buttonSelectDstFolder.UseVisualStyleBackColor = false;
            buttonSelectDstFolder.Click += SelectFolder;
            // 
            // buttonSelectLOG
            // 
            buttonSelectLOG.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSelectLOG.BackColor = SystemColors.Window;
            buttonSelectLOG.ForeColor = Color.RoyalBlue;
            buttonSelectLOG.Location = new Point(408, 47);
            buttonSelectLOG.Margin = new Padding(2);
            buttonSelectLOG.Name = "buttonSelectLOG";
            buttonSelectLOG.Size = new Size(75, 23);
            buttonSelectLOG.TabIndex = 42;
            buttonSelectLOG.Text = "Select";
            toolTip1.SetToolTip(buttonSelectLOG, "Select the corresponding path.");
            buttonSelectLOG.UseVisualStyleBackColor = false;
            buttonSelectLOG.Click += SelectFolder;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = SystemColors.GradientInactiveCaption;
            panel1.Controls.Add(buttonSelectSrcFolder);
            panel1.Controls.Add(textBoxSrcFolder);
            panel1.Controls.Add(checkBoxSrcMTP);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(linkLabel1);
            panel1.Location = new Point(69, -2);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(493, 81);
            panel1.TabIndex = 40;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.White;
            panel2.Controls.Add(buttonSelectSubfolder);
            panel2.Controls.Add(textBoxSubfolder);
            panel2.Controls.Add(checkBoxSubfolder);
            panel2.Controls.Add(label3);
            panel2.Location = new Point(69, 77);
            panel2.Margin = new Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new Size(493, 76);
            panel2.TabIndex = 41;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel3.BackColor = SystemColors.GradientInactiveCaption;
            panel3.Controls.Add(buttonSelectDstFolder);
            panel3.Controls.Add(textBoxDstFolder);
            panel3.Controls.Add(checkBoxDstMTP);
            panel3.Controls.Add(label2);
            panel3.Location = new Point(69, 152);
            panel3.Margin = new Padding(2);
            panel3.Name = "panel3";
            panel3.Size = new Size(493, 82);
            panel3.TabIndex = 43;
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel4.BackColor = Color.White;
            panel4.Controls.Add(buttonSelectLOG);
            panel4.Controls.Add(textBoxLocationLOG);
            panel4.Controls.Add(checkBoxLOGMTP);
            panel4.Controls.Add(label4);
            panel4.Controls.Add(checkBoxLOG);
            panel4.Location = new Point(69, 234);
            panel4.Margin = new Padding(2);
            panel4.Name = "panel4";
            panel4.Size = new Size(493, 76);
            panel4.TabIndex = 44;
            // 
            // panel5
            // 
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel5.BackColor = SystemColors.GradientInactiveCaption;
            panel5.Controls.Add(buttonStart);
            panel5.Controls.Add(pictureBoxGIF);
            panel5.Controls.Add(labelProgress);
            panel5.Controls.Add(buttonCancel);
            panel5.Controls.Add(buttonSaveConfig);
            panel5.Controls.Add(progressBarBackup);
            panel5.Controls.Add(checkBoxCompMeta);
            panel5.Location = new Point(71, 309);
            panel5.Margin = new Padding(2);
            panel5.Name = "panel5";
            panel5.Size = new Size(491, 125);
            panel5.TabIndex = 44;
            // 
            // panel6
            // 
            panel6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel6.BackColor = SystemColors.GradientInactiveCaption;
            panel6.ForeColor = SystemColors.ActiveCaptionText;
            panel6.Location = new Point(0, 0);
            panel6.Margin = new Padding(2);
            panel6.Name = "panel6";
            panel6.Size = new Size(72, 432);
            panel6.TabIndex = 45;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.Control;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(562, 431);
            Controls.Add(pictureBoxORNAMENT);
            Controls.Add(panel6);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(panel3);
            Controls.Add(panel4);
            Controls.Add(panel5);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximumSize = new Size(6557, 6559);
            MinimumSize = new Size(578, 470);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Local Backup Creator";
            TransparencyKey = Color.Cyan;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxORNAMENT).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGIF).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        //From top left to down right
        private PictureBox pictureBoxORNAMENT;

        //Zone 1: source folder
        private Label label1;
        private LinkLabel linkLabel1;
        private TextBox textBoxSrcFolder;
        private Button buttonSelectSrcFolder;

        //Zone 2: destination folder
        private Label label2;
        private TextBox textBoxDstFolder;

        //Zone 4: txt file
        private CheckBox checkBoxLOG;
        private Label label4;
        private TextBox textBoxLocationLOG;

        //Down row:
        private CheckBox checkBoxCompMeta;

        private Button buttonCancel;
        private CheckBox checkBoxSubfolder;
        private Label label3;
        private TextBox textBoxSubfolder;
        private ProgressBar progressBarBackup;
        private Label labelProgress;
        private PictureBox pictureBoxGIF;
        private CheckBox checkBoxSrcMTP;
        private CheckBox checkBoxDstMTP;
        private CheckBox checkBoxLOGMTP;
        private ToolTip toolTip1;
        private Button buttonSaveConfig;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Button buttonStart;
        private Button buttonSelectSubfolder;
        private Button buttonSelectDstFolder;
        private Button buttonSelectLOG;
    }
}
