namespace RealmCrafter_GE.Forms
{
    partial class MediaManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label MediaMeshShaderLabel;
            System.Windows.Forms.Label MediaMeshScaleLabel;
            System.Windows.Forms.Label MediaPreviewSoundVolumeLabel;
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Meshes");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Textures");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Sounds");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Music");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ImportMusicFolder = new System.Windows.Forms.Button();
            this.ImportSoundFolder = new System.Windows.Forms.Button();
            this.ImportTextureFolder = new System.Windows.Forms.Button();
            this.ImportMeshFolder = new System.Windows.Forms.Button();
            this.BMediaAddMesh = new System.Windows.Forms.Button();
            this.MediaTree = new System.Windows.Forms.TreeView();
            this.BMediaShadersEdit = new System.Windows.Forms.Button();
            this.BMediaAddTexture = new System.Windows.Forms.Button();
            this.MediaPreviewIDLabel = new System.Windows.Forms.Label();
            this.BMediaAddMusic = new System.Windows.Forms.Button();
            this.BMediaAddSound = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.MediaPreviewGroup = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.MeshPropertiesGroup = new System.Windows.Forms.Panel();
            this.gbLOD = new System.Windows.Forms.GroupBox();
            this.gbHigh = new System.Windows.Forms.GroupBox();
            this.nDistHigh = new System.Windows.Forms.NumericUpDown();
            this.lbDistHigh = new System.Windows.Forms.Label();
            this.btnMeshHigh = new System.Windows.Forms.Button();
            this.gbMedium = new System.Windows.Forms.GroupBox();
            this.nDistMedium = new System.Windows.Forms.NumericUpDown();
            this.lbDistMedium = new System.Windows.Forms.Label();
            this.btnMeshMedium = new System.Windows.Forms.Button();
            this.gbLow = new System.Windows.Forms.GroupBox();
            this.nDistLow = new System.Windows.Forms.NumericUpDown();
            this.lbDistLow = new System.Windows.Forms.Label();
            this.btnMeshLow = new System.Windows.Forms.Button();
            this.ConfigureParametersButton = new System.Windows.Forms.Button();
            this.CheckRefractEnvironment = new System.Windows.Forms.CheckBox();
            this.CheckReflectEnvironment = new System.Windows.Forms.CheckBox();
            this.CheckMeshTangents = new System.Windows.Forms.CheckBox();
            this.MediaMeshShaderCombo = new System.Windows.Forms.ComboBox();
            this.MediaMeshScaleSpinner = new System.Windows.Forms.NumericUpDown();
            this.SoundPropertiesGroup = new System.Windows.Forms.Panel();
            this.MediaPreviewStopSound = new System.Windows.Forms.Button();
            this.MediaPreviewPlaySound = new System.Windows.Forms.Button();
            this.MediaPreviewSoundVolume = new System.Windows.Forms.TrackBar();
            this.MediaPreview3DPanel = new System.Windows.Forms.Panel();
            this.MediaAddDialog = new System.Windows.Forms.OpenFileDialog();
            this.MediaTreeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MediaMeshShaderLabel = new System.Windows.Forms.Label();
            MediaMeshScaleLabel = new System.Windows.Forms.Label();
            MediaPreviewSoundVolumeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.MediaPreviewGroup.SuspendLayout();
            this.panel5.SuspendLayout();
            this.MeshPropertiesGroup.SuspendLayout();
            this.gbLOD.SuspendLayout();
            this.gbHigh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistHigh)).BeginInit();
            this.gbMedium.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistMedium)).BeginInit();
            this.gbLow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MediaMeshScaleSpinner)).BeginInit();
            this.SoundPropertiesGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MediaPreviewSoundVolume)).BeginInit();
            this.MediaTreeMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MediaMeshShaderLabel
            // 
            MediaMeshShaderLabel.AutoSize = true;
            MediaMeshShaderLabel.Location = new System.Drawing.Point(15, 42);
            MediaMeshShaderLabel.Name = "MediaMeshShaderLabel";
            MediaMeshShaderLabel.Size = new System.Drawing.Size(76, 13);
            MediaMeshShaderLabel.TabIndex = 1;
            MediaMeshShaderLabel.Text = "Shader Profile:";
            // 
            // MediaMeshScaleLabel
            // 
            MediaMeshScaleLabel.AutoSize = true;
            MediaMeshScaleLabel.Location = new System.Drawing.Point(15, 15);
            MediaMeshScaleLabel.Name = "MediaMeshScaleLabel";
            MediaMeshScaleLabel.Size = new System.Drawing.Size(72, 13);
            MediaMeshScaleLabel.TabIndex = 0;
            MediaMeshScaleLabel.Text = "Default scale:";
            // 
            // MediaPreviewSoundVolumeLabel
            // 
            MediaPreviewSoundVolumeLabel.AutoSize = true;
            MediaPreviewSoundVolumeLabel.Location = new System.Drawing.Point(4, 62);
            MediaPreviewSoundVolumeLabel.Name = "MediaPreviewSoundVolumeLabel";
            MediaPreviewSoundVolumeLabel.Size = new System.Drawing.Size(78, 13);
            MediaPreviewSoundVolumeLabel.TabIndex = 4;
            MediaPreviewSoundVolumeLabel.Text = "Sound volume:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ImportMusicFolder);
            this.splitContainer1.Panel1.Controls.Add(this.ImportSoundFolder);
            this.splitContainer1.Panel1.Controls.Add(this.ImportTextureFolder);
            this.splitContainer1.Panel1.Controls.Add(this.ImportMeshFolder);
            this.splitContainer1.Panel1.Controls.Add(this.BMediaAddMesh);
            this.splitContainer1.Panel1.Controls.Add(this.MediaTree);
            this.splitContainer1.Panel1.Controls.Add(this.BMediaShadersEdit);
            this.splitContainer1.Panel1.Controls.Add(this.BMediaAddTexture);
            this.splitContainer1.Panel1.Controls.Add(this.MediaPreviewIDLabel);
            this.splitContainer1.Panel1.Controls.Add(this.BMediaAddMusic);
            this.splitContainer1.Panel1.Controls.Add(this.BMediaAddSound);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(973, 749);
            this.splitContainer1.SplitterDistance = 253;
            this.splitContainer1.TabIndex = 0;
            // 
            // ImportMusicFolder
            // 
            this.ImportMusicFolder.Location = new System.Drawing.Point(23, 162);
            this.ImportMusicFolder.Name = "ImportMusicFolder";
            this.ImportMusicFolder.Size = new System.Drawing.Size(200, 25);
            this.ImportMusicFolder.TabIndex = 20;
            this.ImportMusicFolder.Text = "Import Music Folder";
            this.ImportMusicFolder.UseVisualStyleBackColor = true;
            this.ImportMusicFolder.Click += new System.EventHandler(this.ImportMusicFolder_Click);
            // 
            // ImportSoundFolder
            // 
            this.ImportSoundFolder.Location = new System.Drawing.Point(23, 131);
            this.ImportSoundFolder.Name = "ImportSoundFolder";
            this.ImportSoundFolder.Size = new System.Drawing.Size(200, 25);
            this.ImportSoundFolder.TabIndex = 19;
            this.ImportSoundFolder.Text = "Import Sound Folder";
            this.ImportSoundFolder.UseVisualStyleBackColor = true;
            this.ImportSoundFolder.Click += new System.EventHandler(this.ImportSoundFolder_Click);
            // 
            // ImportTextureFolder
            // 
            this.ImportTextureFolder.Location = new System.Drawing.Point(23, 100);
            this.ImportTextureFolder.Name = "ImportTextureFolder";
            this.ImportTextureFolder.Size = new System.Drawing.Size(200, 25);
            this.ImportTextureFolder.TabIndex = 18;
            this.ImportTextureFolder.Text = "Import Texture Folder";
            this.ImportTextureFolder.UseVisualStyleBackColor = true;
            this.ImportTextureFolder.Click += new System.EventHandler(this.ImportTextureFolder_Click);
            // 
            // ImportMeshFolder
            // 
            this.ImportMeshFolder.Location = new System.Drawing.Point(23, 69);
            this.ImportMeshFolder.Name = "ImportMeshFolder";
            this.ImportMeshFolder.Size = new System.Drawing.Size(200, 25);
            this.ImportMeshFolder.TabIndex = 17;
            this.ImportMeshFolder.Text = "Import Mesh Folder";
            this.ImportMeshFolder.UseVisualStyleBackColor = true;
            this.ImportMeshFolder.Click += new System.EventHandler(this.ImportFolder_Click);
            // 
            // BMediaAddMesh
            // 
            this.BMediaAddMesh.Location = new System.Drawing.Point(23, 7);
            this.BMediaAddMesh.Name = "BMediaAddMesh";
            this.BMediaAddMesh.Size = new System.Drawing.Size(96, 25);
            this.BMediaAddMesh.TabIndex = 15;
            this.BMediaAddMesh.Text = "Add Mesh";
            this.BMediaAddMesh.UseVisualStyleBackColor = true;
            this.BMediaAddMesh.Click += new System.EventHandler(this.BMediaAddMesh_Click);
            // 
            // MediaTree
            // 
            this.MediaTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.MediaTree.Location = new System.Drawing.Point(23, 247);
            this.MediaTree.Name = "MediaTree";
            treeNode1.Name = "MediaMeshesRoot";
            treeNode1.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode1.Text = "Meshes";
            treeNode2.Name = "MediaTexturesRoot";
            treeNode2.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode2.Text = "Textures";
            treeNode3.Name = "MediaSoundsRoot";
            treeNode3.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode3.Text = "Sounds";
            treeNode4.Name = "MediaMusicRoot";
            treeNode4.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode4.Text = "Music";
            this.MediaTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.MediaTree.Size = new System.Drawing.Size(203, 490);
            this.MediaTree.TabIndex = 11;
            this.MediaTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MediaTree_AfterSelect);
            this.MediaTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MediaTree_KeyDown);
            this.MediaTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MediaTree_MouseDown);
            // 
            // BMediaShadersEdit
            // 
            this.BMediaShadersEdit.Location = new System.Drawing.Point(23, 193);
            this.BMediaShadersEdit.Name = "BMediaShadersEdit";
            this.BMediaShadersEdit.Size = new System.Drawing.Size(203, 25);
            this.BMediaShadersEdit.TabIndex = 16;
            this.BMediaShadersEdit.Text = "Manage Shaders and Profiles";
            this.BMediaShadersEdit.UseVisualStyleBackColor = true;
            this.BMediaShadersEdit.Click += new System.EventHandler(this.BMediaShadersEdit_Click);
            // 
            // BMediaAddTexture
            // 
            this.BMediaAddTexture.Location = new System.Drawing.Point(130, 7);
            this.BMediaAddTexture.Name = "BMediaAddTexture";
            this.BMediaAddTexture.Size = new System.Drawing.Size(96, 25);
            this.BMediaAddTexture.TabIndex = 13;
            this.BMediaAddTexture.Text = "Add Texture";
            this.BMediaAddTexture.UseVisualStyleBackColor = true;
            this.BMediaAddTexture.Click += new System.EventHandler(this.BMediaAddTexture_Click);
            // 
            // MediaPreviewIDLabel
            // 
            this.MediaPreviewIDLabel.AutoEllipsis = true;
            this.MediaPreviewIDLabel.AutoSize = true;
            this.MediaPreviewIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MediaPreviewIDLabel.Location = new System.Drawing.Point(20, 231);
            this.MediaPreviewIDLabel.Name = "MediaPreviewIDLabel";
            this.MediaPreviewIDLabel.Size = new System.Drawing.Size(162, 13);
            this.MediaPreviewIDLabel.TabIndex = 10;
            this.MediaPreviewIDLabel.Text = "Media ID: Nothing selected";
            // 
            // BMediaAddMusic
            // 
            this.BMediaAddMusic.Location = new System.Drawing.Point(130, 38);
            this.BMediaAddMusic.Name = "BMediaAddMusic";
            this.BMediaAddMusic.Size = new System.Drawing.Size(96, 25);
            this.BMediaAddMusic.TabIndex = 14;
            this.BMediaAddMusic.Text = "Add Music";
            this.BMediaAddMusic.UseVisualStyleBackColor = true;
            this.BMediaAddMusic.Click += new System.EventHandler(this.BMediaAddMusic_Click);
            // 
            // BMediaAddSound
            // 
            this.BMediaAddSound.Location = new System.Drawing.Point(23, 38);
            this.BMediaAddSound.Name = "BMediaAddSound";
            this.BMediaAddSound.Size = new System.Drawing.Size(96, 25);
            this.BMediaAddSound.TabIndex = 12;
            this.BMediaAddSound.Text = "Add Sound";
            this.BMediaAddSound.UseVisualStyleBackColor = true;
            this.BMediaAddSound.Click += new System.EventHandler(this.BMediaAddSound_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.MediaPreviewGroup);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(716, 749);
            this.panel1.TabIndex = 0;
            // 
            // MediaPreviewGroup
            // 
            this.MediaPreviewGroup.Controls.Add(this.panel5);
            this.MediaPreviewGroup.Controls.Add(this.MediaPreview3DPanel);
            this.MediaPreviewGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MediaPreviewGroup.Location = new System.Drawing.Point(0, 0);
            this.MediaPreviewGroup.Name = "MediaPreviewGroup";
            this.MediaPreviewGroup.Size = new System.Drawing.Size(716, 749);
            this.MediaPreviewGroup.TabIndex = 5;
            this.MediaPreviewGroup.TabStop = false;
            this.MediaPreviewGroup.Text = "Preview";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.MeshPropertiesGroup);
            this.panel5.Controls.Add(this.SoundPropertiesGroup);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(3, 581);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(710, 165);
            this.panel5.TabIndex = 7;
            // 
            // MeshPropertiesGroup
            // 
            this.MeshPropertiesGroup.Controls.Add(this.gbLOD);
            this.MeshPropertiesGroup.Controls.Add(this.ConfigureParametersButton);
            this.MeshPropertiesGroup.Controls.Add(this.CheckRefractEnvironment);
            this.MeshPropertiesGroup.Controls.Add(this.CheckReflectEnvironment);
            this.MeshPropertiesGroup.Controls.Add(this.CheckMeshTangents);
            this.MeshPropertiesGroup.Controls.Add(this.MediaMeshShaderCombo);
            this.MeshPropertiesGroup.Controls.Add(MediaMeshShaderLabel);
            this.MeshPropertiesGroup.Controls.Add(this.MediaMeshScaleSpinner);
            this.MeshPropertiesGroup.Controls.Add(MediaMeshScaleLabel);
            this.MeshPropertiesGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeshPropertiesGroup.Location = new System.Drawing.Point(0, 0);
            this.MeshPropertiesGroup.Name = "MeshPropertiesGroup";
            this.MeshPropertiesGroup.Size = new System.Drawing.Size(710, 165);
            this.MeshPropertiesGroup.TabIndex = 8;
            this.MeshPropertiesGroup.Visible = false;
            // 
            // gbLOD
            // 
            this.gbLOD.Controls.Add(this.gbHigh);
            this.gbLOD.Controls.Add(this.gbMedium);
            this.gbLOD.Controls.Add(this.gbLow);
            this.gbLOD.Location = new System.Drawing.Point(17, 71);
            this.gbLOD.Margin = new System.Windows.Forms.Padding(2);
            this.gbLOD.Name = "gbLOD";
            this.gbLOD.Padding = new System.Windows.Forms.Padding(2);
            this.gbLOD.Size = new System.Drawing.Size(504, 81);
            this.gbLOD.TabIndex = 11;
            this.gbLOD.TabStop = false;
            this.gbLOD.Text = "Level of Detail";
            // 
            // gbHigh
            // 
            this.gbHigh.Controls.Add(this.nDistHigh);
            this.gbHigh.Controls.Add(this.lbDistHigh);
            this.gbHigh.Controls.Add(this.btnMeshHigh);
            this.gbHigh.Location = new System.Drawing.Point(4, 14);
            this.gbHigh.Margin = new System.Windows.Forms.Padding(2);
            this.gbHigh.Name = "gbHigh";
            this.gbHigh.Padding = new System.Windows.Forms.Padding(2);
            this.gbHigh.Size = new System.Drawing.Size(163, 63);
            this.gbHigh.TabIndex = 2;
            this.gbHigh.TabStop = false;
            this.gbHigh.Text = "High";
            // 
            // nDistHigh
            // 
            this.nDistHigh.DecimalPlaces = 2;
            this.nDistHigh.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nDistHigh.Location = new System.Drawing.Point(79, 16);
            this.nDistHigh.Margin = new System.Windows.Forms.Padding(2);
            this.nDistHigh.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.nDistHigh.Name = "nDistHigh";
            this.nDistHigh.Size = new System.Drawing.Size(80, 20);
            this.nDistHigh.TabIndex = 3;
            this.nDistHigh.ValueChanged += new System.EventHandler(this.nDistHigh_ValueChanged);
            // 
            // lbDistHigh
            // 
            this.lbDistHigh.AutoSize = true;
            this.lbDistHigh.Location = new System.Drawing.Point(4, 18);
            this.lbDistHigh.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDistHigh.Name = "lbDistHigh";
            this.lbDistHigh.Size = new System.Drawing.Size(78, 13);
            this.lbDistHigh.TabIndex = 2;
            this.lbDistHigh.Text = "DistToMedium:";
            // 
            // btnMeshHigh
            // 
            this.btnMeshHigh.Enabled = false;
            this.btnMeshHigh.Location = new System.Drawing.Point(5, 37);
            this.btnMeshHigh.Margin = new System.Windows.Forms.Padding(2);
            this.btnMeshHigh.Name = "btnMeshHigh";
            this.btnMeshHigh.Size = new System.Drawing.Size(153, 21);
            this.btnMeshHigh.TabIndex = 0;
            this.btnMeshHigh.UseVisualStyleBackColor = true;
            this.btnMeshHigh.Click += new System.EventHandler(this.btnMeshHigh_Click);
            // 
            // gbMedium
            // 
            this.gbMedium.Controls.Add(this.nDistMedium);
            this.gbMedium.Controls.Add(this.lbDistMedium);
            this.gbMedium.Controls.Add(this.btnMeshMedium);
            this.gbMedium.Location = new System.Drawing.Point(172, 14);
            this.gbMedium.Margin = new System.Windows.Forms.Padding(2);
            this.gbMedium.Name = "gbMedium";
            this.gbMedium.Padding = new System.Windows.Forms.Padding(2);
            this.gbMedium.Size = new System.Drawing.Size(158, 63);
            this.gbMedium.TabIndex = 2;
            this.gbMedium.TabStop = false;
            this.gbMedium.Text = "Medium";
            // 
            // nDistMedium
            // 
            this.nDistMedium.DecimalPlaces = 2;
            this.nDistMedium.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nDistMedium.Location = new System.Drawing.Point(62, 16);
            this.nDistMedium.Margin = new System.Windows.Forms.Padding(2);
            this.nDistMedium.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.nDistMedium.Name = "nDistMedium";
            this.nDistMedium.Size = new System.Drawing.Size(92, 20);
            this.nDistMedium.TabIndex = 3;
            this.nDistMedium.ValueChanged += new System.EventHandler(this.nDistMedium_ValueChanged);
            // 
            // lbDistMedium
            // 
            this.lbDistMedium.AutoSize = true;
            this.lbDistMedium.Location = new System.Drawing.Point(4, 18);
            this.lbDistMedium.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDistMedium.Name = "lbDistMedium";
            this.lbDistMedium.Size = new System.Drawing.Size(61, 13);
            this.lbDistMedium.TabIndex = 2;
            this.lbDistMedium.Text = "DistToLow:";
            // 
            // btnMeshMedium
            // 
            this.btnMeshMedium.Location = new System.Drawing.Point(5, 37);
            this.btnMeshMedium.Margin = new System.Windows.Forms.Padding(2);
            this.btnMeshMedium.Name = "btnMeshMedium";
            this.btnMeshMedium.Size = new System.Drawing.Size(148, 21);
            this.btnMeshMedium.TabIndex = 0;
            this.btnMeshMedium.UseVisualStyleBackColor = true;
            this.btnMeshMedium.Click += new System.EventHandler(this.btnMeshMedium_Click);
            // 
            // gbLow
            // 
            this.gbLow.Controls.Add(this.nDistLow);
            this.gbLow.Controls.Add(this.lbDistLow);
            this.gbLow.Controls.Add(this.btnMeshLow);
            this.gbLow.Location = new System.Drawing.Point(334, 14);
            this.gbLow.Margin = new System.Windows.Forms.Padding(2);
            this.gbLow.Name = "gbLow";
            this.gbLow.Padding = new System.Windows.Forms.Padding(2);
            this.gbLow.Size = new System.Drawing.Size(160, 63);
            this.gbLow.TabIndex = 2;
            this.gbLow.TabStop = false;
            this.gbLow.Text = "Low";
            // 
            // nDistLow
            // 
            this.nDistLow.DecimalPlaces = 2;
            this.nDistLow.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nDistLow.Location = new System.Drawing.Point(70, 15);
            this.nDistLow.Margin = new System.Windows.Forms.Padding(2);
            this.nDistLow.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.nDistLow.Name = "nDistLow";
            this.nDistLow.Size = new System.Drawing.Size(85, 20);
            this.nDistLow.TabIndex = 3;
            this.nDistLow.ValueChanged += new System.EventHandler(this.nDistLow_ValueChanged);
            // 
            // lbDistLow
            // 
            this.lbDistLow.AutoSize = true;
            this.lbDistLow.Location = new System.Drawing.Point(4, 18);
            this.lbDistLow.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDistLow.Name = "lbDistLow";
            this.lbDistLow.Size = new System.Drawing.Size(63, 13);
            this.lbDistLow.TabIndex = 2;
            this.lbDistLow.Text = "DistToHide:";
            // 
            // btnMeshLow
            // 
            this.btnMeshLow.Location = new System.Drawing.Point(5, 37);
            this.btnMeshLow.Margin = new System.Windows.Forms.Padding(2);
            this.btnMeshLow.Name = "btnMeshLow";
            this.btnMeshLow.Size = new System.Drawing.Size(150, 21);
            this.btnMeshLow.TabIndex = 0;
            this.btnMeshLow.UseVisualStyleBackColor = true;
            this.btnMeshLow.Click += new System.EventHandler(this.btnMeshLow_Click);
            // 
            // ConfigureParametersButton
            // 
            this.ConfigureParametersButton.Location = new System.Drawing.Point(370, 21);
            this.ConfigureParametersButton.Name = "ConfigureParametersButton";
            this.ConfigureParametersButton.Size = new System.Drawing.Size(142, 35);
            this.ConfigureParametersButton.TabIndex = 10;
            this.ConfigureParametersButton.Text = "Shader Parameters";
            this.ConfigureParametersButton.UseVisualStyleBackColor = true;
            this.ConfigureParametersButton.Click += new System.EventHandler(this.ConfigureParametersButton_Click);
            // 
            // CheckRefractEnvironment
            // 
            this.CheckRefractEnvironment.AutoSize = true;
            this.CheckRefractEnvironment.Enabled = false;
            this.CheckRefractEnvironment.Location = new System.Drawing.Point(236, 46);
            this.CheckRefractEnvironment.Name = "CheckRefractEnvironment";
            this.CheckRefractEnvironment.Size = new System.Drawing.Size(123, 17);
            this.CheckRefractEnvironment.TabIndex = 9;
            this.CheckRefractEnvironment.Text = "Refract Environment";
            this.CheckRefractEnvironment.UseVisualStyleBackColor = true;
            this.CheckRefractEnvironment.CheckedChanged += new System.EventHandler(this.CheckRefractEnvironment_CheckedChanged);
            // 
            // CheckReflectEnvironment
            // 
            this.CheckReflectEnvironment.AutoSize = true;
            this.CheckReflectEnvironment.Enabled = false;
            this.CheckReflectEnvironment.Location = new System.Drawing.Point(236, 30);
            this.CheckReflectEnvironment.Name = "CheckReflectEnvironment";
            this.CheckReflectEnvironment.Size = new System.Drawing.Size(122, 17);
            this.CheckReflectEnvironment.TabIndex = 8;
            this.CheckReflectEnvironment.Text = "Reflect Environment";
            this.CheckReflectEnvironment.UseVisualStyleBackColor = true;
            this.CheckReflectEnvironment.CheckedChanged += new System.EventHandler(this.CheckReflectEnvironment_CheckedChanged);
            // 
            // CheckMeshTangents
            // 
            this.CheckMeshTangents.AutoSize = true;
            this.CheckMeshTangents.Location = new System.Drawing.Point(236, 13);
            this.CheckMeshTangents.Name = "CheckMeshTangents";
            this.CheckMeshTangents.Size = new System.Drawing.Size(122, 17);
            this.CheckMeshTangents.TabIndex = 7;
            this.CheckMeshTangents.Text = "Use Mesh Tangents";
            this.CheckMeshTangents.UseVisualStyleBackColor = true;
            this.CheckMeshTangents.CheckedChanged += new System.EventHandler(this.CheckMeshTangents_CheckedChanged);
            // 
            // MediaMeshShaderCombo
            // 
            this.MediaMeshShaderCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MediaMeshShaderCombo.Enabled = false;
            this.MediaMeshShaderCombo.FormattingEnabled = true;
            this.MediaMeshShaderCombo.Location = new System.Drawing.Point(98, 42);
            this.MediaMeshShaderCombo.Name = "MediaMeshShaderCombo";
            this.MediaMeshShaderCombo.Size = new System.Drawing.Size(126, 21);
            this.MediaMeshShaderCombo.Sorted = true;
            this.MediaMeshShaderCombo.TabIndex = 3;
            this.MediaMeshShaderCombo.SelectedIndexChanged += new System.EventHandler(this.MediaMeshShaderCombo_SelectedIndexChanged);
            // 
            // MediaMeshScaleSpinner
            // 
            this.MediaMeshScaleSpinner.DecimalPlaces = 3;
            this.MediaMeshScaleSpinner.Enabled = false;
            this.MediaMeshScaleSpinner.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.MediaMeshScaleSpinner.Location = new System.Drawing.Point(99, 13);
            this.MediaMeshScaleSpinner.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.MediaMeshScaleSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.MediaMeshScaleSpinner.Name = "MediaMeshScaleSpinner";
            this.MediaMeshScaleSpinner.Size = new System.Drawing.Size(126, 20);
            this.MediaMeshScaleSpinner.TabIndex = 2;
            this.MediaMeshScaleSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MediaMeshScaleSpinner.ValueChanged += new System.EventHandler(this.MediaMeshScaleSpinner_ValueChanged);
            // 
            // SoundPropertiesGroup
            // 
            this.SoundPropertiesGroup.Controls.Add(this.MediaPreviewStopSound);
            this.SoundPropertiesGroup.Controls.Add(MediaPreviewSoundVolumeLabel);
            this.SoundPropertiesGroup.Controls.Add(this.MediaPreviewPlaySound);
            this.SoundPropertiesGroup.Controls.Add(this.MediaPreviewSoundVolume);
            this.SoundPropertiesGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SoundPropertiesGroup.Location = new System.Drawing.Point(0, 0);
            this.SoundPropertiesGroup.Name = "SoundPropertiesGroup";
            this.SoundPropertiesGroup.Size = new System.Drawing.Size(710, 165);
            this.SoundPropertiesGroup.TabIndex = 7;
            this.SoundPropertiesGroup.Visible = false;
            // 
            // MediaPreviewStopSound
            // 
            this.MediaPreviewStopSound.Enabled = false;
            this.MediaPreviewStopSound.Image = global::RealmCrafter_GE.Properties.Resources.Stop;
            this.MediaPreviewStopSound.Location = new System.Drawing.Point(44, 4);
            this.MediaPreviewStopSound.Name = "MediaPreviewStopSound";
            this.MediaPreviewStopSound.Size = new System.Drawing.Size(35, 34);
            this.MediaPreviewStopSound.TabIndex = 2;
            this.MediaPreviewStopSound.UseVisualStyleBackColor = true;
            // 
            // MediaPreviewPlaySound
            // 
            this.MediaPreviewPlaySound.Enabled = false;
            this.MediaPreviewPlaySound.Image = global::RealmCrafter_GE.Properties.Resources.Play;
            this.MediaPreviewPlaySound.Location = new System.Drawing.Point(3, 4);
            this.MediaPreviewPlaySound.Name = "MediaPreviewPlaySound";
            this.MediaPreviewPlaySound.Size = new System.Drawing.Size(35, 34);
            this.MediaPreviewPlaySound.TabIndex = 2;
            this.MediaPreviewPlaySound.UseVisualStyleBackColor = true;
            // 
            // MediaPreviewSoundVolume
            // 
            this.MediaPreviewSoundVolume.LargeChange = 10;
            this.MediaPreviewSoundVolume.Location = new System.Drawing.Point(88, 49);
            this.MediaPreviewSoundVolume.Maximum = 100;
            this.MediaPreviewSoundVolume.Name = "MediaPreviewSoundVolume";
            this.MediaPreviewSoundVolume.Size = new System.Drawing.Size(244, 45);
            this.MediaPreviewSoundVolume.TabIndex = 3;
            this.MediaPreviewSoundVolume.TickFrequency = 10;
            this.MediaPreviewSoundVolume.Value = 100;
            // 
            // MediaPreview3DPanel
            // 
            this.MediaPreview3DPanel.AutoSize = true;
            this.MediaPreview3DPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MediaPreview3DPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MediaPreview3DPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MediaPreview3DPanel.Location = new System.Drawing.Point(3, 16);
            this.MediaPreview3DPanel.Name = "MediaPreview3DPanel";
            this.MediaPreview3DPanel.Size = new System.Drawing.Size(710, 730);
            this.MediaPreview3DPanel.TabIndex = 1;
            // 
            // MediaAddDialog
            // 
            this.MediaAddDialog.InitialDirectory = "Data\\Meshes\\";
            this.MediaAddDialog.Multiselect = true;
            this.MediaAddDialog.RestoreDirectory = true;
            this.MediaAddDialog.SupportMultiDottedExtensions = true;
            this.MediaAddDialog.Title = "Select file to add to database";
            // 
            // MediaTreeMenuStrip
            // 
            this.MediaTreeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.MediaTreeMenuStrip.Name = "MediaTreeMenuStrip";
            this.MediaTreeMenuStrip.Size = new System.Drawing.Size(216, 26);
            this.MediaTreeMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.MediaTreeMenuStrip_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.deleteToolStripMenuItem.Text = "Remove file from database";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // MediaManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 749);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MediaManager";
            this.Text = "MediaManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MediaManager_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.MediaPreviewGroup.ResumeLayout(false);
            this.MediaPreviewGroup.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.MeshPropertiesGroup.ResumeLayout(false);
            this.MeshPropertiesGroup.PerformLayout();
            this.gbLOD.ResumeLayout(false);
            this.gbHigh.ResumeLayout(false);
            this.gbHigh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistHigh)).EndInit();
            this.gbMedium.ResumeLayout(false);
            this.gbMedium.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistMedium)).EndInit();
            this.gbLow.ResumeLayout(false);
            this.gbLow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDistLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MediaMeshScaleSpinner)).EndInit();
            this.SoundPropertiesGroup.ResumeLayout(false);
            this.SoundPropertiesGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MediaPreviewSoundVolume)).EndInit();
            this.MediaTreeMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ImportSoundFolder;
        private System.Windows.Forms.Button ImportTextureFolder;
        private System.Windows.Forms.Button ImportMeshFolder;
        private System.Windows.Forms.Button BMediaAddMesh;
        public System.Windows.Forms.TreeView MediaTree;
        private System.Windows.Forms.Button BMediaShadersEdit;
        private System.Windows.Forms.Button BMediaAddTexture;
        private System.Windows.Forms.Label MediaPreviewIDLabel;
        private System.Windows.Forms.Button BMediaAddMusic;
        private System.Windows.Forms.Button BMediaAddSound;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox MediaPreviewGroup;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel MeshPropertiesGroup;
        private System.Windows.Forms.GroupBox gbLOD;
        private System.Windows.Forms.GroupBox gbHigh;
        private System.Windows.Forms.NumericUpDown nDistHigh;
        private System.Windows.Forms.Label lbDistHigh;
        private System.Windows.Forms.Button btnMeshHigh;
        private System.Windows.Forms.GroupBox gbMedium;
        private System.Windows.Forms.NumericUpDown nDistMedium;
        private System.Windows.Forms.Label lbDistMedium;
        private System.Windows.Forms.Button btnMeshMedium;
        private System.Windows.Forms.GroupBox gbLow;
        private System.Windows.Forms.NumericUpDown nDistLow;
        private System.Windows.Forms.Label lbDistLow;
        private System.Windows.Forms.Button btnMeshLow;
        private System.Windows.Forms.Button ConfigureParametersButton;
        private System.Windows.Forms.CheckBox CheckRefractEnvironment;
        private System.Windows.Forms.CheckBox CheckReflectEnvironment;
        private System.Windows.Forms.CheckBox CheckMeshTangents;
        public System.Windows.Forms.ComboBox MediaMeshShaderCombo;
        private System.Windows.Forms.NumericUpDown MediaMeshScaleSpinner;
        private System.Windows.Forms.Panel SoundPropertiesGroup;
        private System.Windows.Forms.Button MediaPreviewStopSound;
        private System.Windows.Forms.Button MediaPreviewPlaySound;
        private System.Windows.Forms.TrackBar MediaPreviewSoundVolume;
        public System.Windows.Forms.OpenFileDialog MediaAddDialog;
        private System.Windows.Forms.ContextMenuStrip MediaTreeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button ImportMusicFolder;
        public System.Windows.Forms.Panel MediaPreview3DPanel;
    }
}