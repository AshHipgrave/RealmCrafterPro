namespace RealmCrafter_GE
{
    partial class TreeEditorProperties
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Trunks");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Leaves");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Physics Volumes");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Sway Root");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Tree Properties", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.TreePropertiesGroup = new System.Windows.Forms.GroupBox();
            this.Seed = new System.Windows.Forms.NumericUpDown();
            this.LeafCount = new System.Windows.Forms.NumericUpDown();
            this.ScaleZ = new System.Windows.Forms.NumericUpDown();
            this.RotationZ = new System.Windows.Forms.NumericUpDown();
            this.PositionZ = new System.Windows.Forms.NumericUpDown();
            this.ScaleY = new System.Windows.Forms.NumericUpDown();
            this.RotationY = new System.Windows.Forms.NumericUpDown();
            this.PositionY = new System.Windows.Forms.NumericUpDown();
            this.ScaleX = new System.Windows.Forms.NumericUpDown();
            this.RotationX = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.NormalLabel = new System.Windows.Forms.Label();
            this.DiffuseLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TextureNormal = new System.Windows.Forms.Button();
            this.TextureDiffuse = new System.Windows.Forms.Button();
            this.PositionX = new System.Windows.Forms.NumericUpDown();
            this.TreeComponentsGroup = new System.Windows.Forms.GroupBox();
            this.TreeComponents = new System.Windows.Forms.TreeView();
            this.TreeNodesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTrunkMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLeafGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLeafToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleLeadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leafPlacerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cubeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capsuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sphereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.finalizeObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TextureFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MeshOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.TreePropertiesGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Seed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeafCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionX)).BeginInit();
            this.TreeComponentsGroup.SuspendLayout();
            this.TreeNodesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreePropertiesGroup
            // 
            this.TreePropertiesGroup.Controls.Add(this.Seed);
            this.TreePropertiesGroup.Controls.Add(this.LeafCount);
            this.TreePropertiesGroup.Controls.Add(this.ScaleZ);
            this.TreePropertiesGroup.Controls.Add(this.RotationZ);
            this.TreePropertiesGroup.Controls.Add(this.PositionZ);
            this.TreePropertiesGroup.Controls.Add(this.ScaleY);
            this.TreePropertiesGroup.Controls.Add(this.RotationY);
            this.TreePropertiesGroup.Controls.Add(this.PositionY);
            this.TreePropertiesGroup.Controls.Add(this.ScaleX);
            this.TreePropertiesGroup.Controls.Add(this.RotationX);
            this.TreePropertiesGroup.Controls.Add(this.label7);
            this.TreePropertiesGroup.Controls.Add(this.NormalLabel);
            this.TreePropertiesGroup.Controls.Add(this.DiffuseLabel);
            this.TreePropertiesGroup.Controls.Add(this.label4);
            this.TreePropertiesGroup.Controls.Add(this.label3);
            this.TreePropertiesGroup.Controls.Add(this.label2);
            this.TreePropertiesGroup.Controls.Add(this.label1);
            this.TreePropertiesGroup.Controls.Add(this.TextureNormal);
            this.TreePropertiesGroup.Controls.Add(this.TextureDiffuse);
            this.TreePropertiesGroup.Controls.Add(this.PositionX);
            this.TreePropertiesGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TreePropertiesGroup.Location = new System.Drawing.Point(0, 369);
            this.TreePropertiesGroup.Name = "TreePropertiesGroup";
            this.TreePropertiesGroup.Size = new System.Drawing.Size(371, 247);
            this.TreePropertiesGroup.TabIndex = 1;
            this.TreePropertiesGroup.TabStop = false;
            this.TreePropertiesGroup.Text = "Properties";
            // 
            // Seed
            // 
            this.Seed.Location = new System.Drawing.Point(62, 189);
            this.Seed.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.Seed.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.Seed.Name = "Seed";
            this.Seed.Size = new System.Drawing.Size(58, 20);
            this.Seed.TabIndex = 24;
            this.Seed.Visible = false;
            this.Seed.ValueChanged += new System.EventHandler(this.Seed_ValueChanged);
            // 
            // LeafCount
            // 
            this.LeafCount.Location = new System.Drawing.Point(62, 160);
            this.LeafCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.LeafCount.Name = "LeafCount";
            this.LeafCount.Size = new System.Drawing.Size(58, 20);
            this.LeafCount.TabIndex = 23;
            this.LeafCount.Visible = false;
            this.LeafCount.ValueChanged += new System.EventHandler(this.Coverage_ValueChanged);
            // 
            // ScaleZ
            // 
            this.ScaleZ.DecimalPlaces = 3;
            this.ScaleZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleZ.Location = new System.Drawing.Point(190, 102);
            this.ScaleZ.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ScaleZ.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.ScaleZ.Name = "ScaleZ";
            this.ScaleZ.Size = new System.Drawing.Size(58, 20);
            this.ScaleZ.TabIndex = 22;
            this.ScaleZ.ValueChanged += new System.EventHandler(this.Scale_ValueChanged);
            // 
            // RotationZ
            // 
            this.RotationZ.DecimalPlaces = 3;
            this.RotationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RotationZ.Location = new System.Drawing.Point(190, 76);
            this.RotationZ.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationZ.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationZ.Name = "RotationZ";
            this.RotationZ.Size = new System.Drawing.Size(58, 20);
            this.RotationZ.TabIndex = 21;
            this.RotationZ.ValueChanged += new System.EventHandler(this.Rotation_ValueChanged);
            // 
            // PositionZ
            // 
            this.PositionZ.DecimalPlaces = 3;
            this.PositionZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionZ.Location = new System.Drawing.Point(190, 50);
            this.PositionZ.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PositionZ.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PositionZ.Name = "PositionZ";
            this.PositionZ.Size = new System.Drawing.Size(58, 20);
            this.PositionZ.TabIndex = 20;
            this.PositionZ.ValueChanged += new System.EventHandler(this.Position_ValueChanged);
            // 
            // ScaleY
            // 
            this.ScaleY.DecimalPlaces = 3;
            this.ScaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleY.Location = new System.Drawing.Point(126, 102);
            this.ScaleY.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ScaleY.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.ScaleY.Name = "ScaleY";
            this.ScaleY.Size = new System.Drawing.Size(58, 20);
            this.ScaleY.TabIndex = 19;
            this.ScaleY.ValueChanged += new System.EventHandler(this.Scale_ValueChanged);
            // 
            // RotationY
            // 
            this.RotationY.DecimalPlaces = 3;
            this.RotationY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RotationY.Location = new System.Drawing.Point(126, 76);
            this.RotationY.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationY.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationY.Name = "RotationY";
            this.RotationY.Size = new System.Drawing.Size(58, 20);
            this.RotationY.TabIndex = 18;
            this.RotationY.ValueChanged += new System.EventHandler(this.Rotation_ValueChanged);
            // 
            // PositionY
            // 
            this.PositionY.DecimalPlaces = 3;
            this.PositionY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionY.Location = new System.Drawing.Point(126, 50);
            this.PositionY.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PositionY.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PositionY.Name = "PositionY";
            this.PositionY.Size = new System.Drawing.Size(58, 20);
            this.PositionY.TabIndex = 17;
            this.PositionY.ValueChanged += new System.EventHandler(this.Position_ValueChanged);
            // 
            // ScaleX
            // 
            this.ScaleX.DecimalPlaces = 3;
            this.ScaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleX.Location = new System.Drawing.Point(62, 102);
            this.ScaleX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ScaleX.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.ScaleX.Name = "ScaleX";
            this.ScaleX.Size = new System.Drawing.Size(58, 20);
            this.ScaleX.TabIndex = 16;
            this.ScaleX.ValueChanged += new System.EventHandler(this.Scale_ValueChanged);
            // 
            // RotationX
            // 
            this.RotationX.DecimalPlaces = 3;
            this.RotationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RotationX.Location = new System.Drawing.Point(62, 76);
            this.RotationX.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationX.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationX.Name = "RotationX";
            this.RotationX.Size = new System.Drawing.Size(58, 20);
            this.RotationX.TabIndex = 15;
            this.RotationX.ValueChanged += new System.EventHandler(this.Rotation_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Transform:";
            // 
            // NormalLabel
            // 
            this.NormalLabel.AutoSize = true;
            this.NormalLabel.Location = new System.Drawing.Point(6, 191);
            this.NormalLabel.Name = "NormalLabel";
            this.NormalLabel.Size = new System.Drawing.Size(43, 13);
            this.NormalLabel.TabIndex = 11;
            this.NormalLabel.Text = "Normal:";
            // 
            // DiffuseLabel
            // 
            this.DiffuseLabel.AutoSize = true;
            this.DiffuseLabel.Location = new System.Drawing.Point(6, 162);
            this.DiffuseLabel.Name = "DiffuseLabel";
            this.DiffuseLabel.Size = new System.Drawing.Size(43, 13);
            this.DiffuseLabel.TabIndex = 10;
            this.DiffuseLabel.Text = "Diffuse:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Textures:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Scale:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Rotation:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Position:";
            // 
            // TextureNormal
            // 
            this.TextureNormal.Location = new System.Drawing.Point(62, 186);
            this.TextureNormal.Name = "TextureNormal";
            this.TextureNormal.Size = new System.Drawing.Size(186, 23);
            this.TextureNormal.TabIndex = 5;
            this.TextureNormal.Text = "None";
            this.TextureNormal.UseVisualStyleBackColor = true;
            this.TextureNormal.Click += new System.EventHandler(this.TextureNormal_Click);
            // 
            // TextureDiffuse
            // 
            this.TextureDiffuse.Location = new System.Drawing.Point(62, 157);
            this.TextureDiffuse.Name = "TextureDiffuse";
            this.TextureDiffuse.Size = new System.Drawing.Size(186, 23);
            this.TextureDiffuse.TabIndex = 4;
            this.TextureDiffuse.Text = "None";
            this.TextureDiffuse.UseVisualStyleBackColor = true;
            this.TextureDiffuse.Click += new System.EventHandler(this.TextureDiffuse_Click);
            // 
            // PositionX
            // 
            this.PositionX.DecimalPlaces = 3;
            this.PositionX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionX.Location = new System.Drawing.Point(62, 50);
            this.PositionX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PositionX.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PositionX.Name = "PositionX";
            this.PositionX.Size = new System.Drawing.Size(58, 20);
            this.PositionX.TabIndex = 0;
            this.PositionX.ValueChanged += new System.EventHandler(this.Position_ValueChanged);
            // 
            // TreeComponentsGroup
            // 
            this.TreeComponentsGroup.Controls.Add(this.TreeComponents);
            this.TreeComponentsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeComponentsGroup.Location = new System.Drawing.Point(0, 0);
            this.TreeComponentsGroup.Name = "TreeComponentsGroup";
            this.TreeComponentsGroup.Size = new System.Drawing.Size(371, 369);
            this.TreeComponentsGroup.TabIndex = 0;
            this.TreeComponentsGroup.TabStop = false;
            this.TreeComponentsGroup.Text = "Components";
            // 
            // TreeComponents
            // 
            this.TreeComponents.ContextMenuStrip = this.TreeNodesMenu;
            this.TreeComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeComponents.Location = new System.Drawing.Point(3, 16);
            this.TreeComponents.Name = "TreeComponents";
            treeNode1.Name = "TrunkTreeNode";
            treeNode1.Text = "Trunks";
            treeNode2.Name = "LeavesTreeNode";
            treeNode2.Text = "Leaves";
            treeNode3.Name = "PhysicsTreeNode";
            treeNode3.Text = "Physics Volumes";
            treeNode4.Name = "TreeSwayTreeNode";
            treeNode4.Text = "Sway Root";
            treeNode5.Name = "PropertiesTreeNode";
            treeNode5.Text = "Tree Properties";
            this.TreeComponents.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode5});
            this.TreeComponents.Size = new System.Drawing.Size(365, 350);
            this.TreeComponents.TabIndex = 0;
            this.TreeComponents.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeComponents_AfterSelect);
            this.TreeComponents.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TreeComponents_KeyUp);
            // 
            // TreeNodesMenu
            // 
            this.TreeNodesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTrunkMeshToolStripMenuItem,
            this.addLeafGroupToolStripMenuItem,
            this.addLeafToolStripMenuItem,
            this.addCollisionToolStripMenuItem,
            this.toolStripSeparator1,
            this.finalizeObjectToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.TreeNodesMenu.Name = "TreeNodesMenu";
            this.TreeNodesMenu.Size = new System.Drawing.Size(181, 142);
            // 
            // addTrunkMeshToolStripMenuItem
            // 
            this.addTrunkMeshToolStripMenuItem.Name = "addTrunkMeshToolStripMenuItem";
            this.addTrunkMeshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addTrunkMeshToolStripMenuItem.Text = "Add Trunk Mesh";
            this.addTrunkMeshToolStripMenuItem.Click += new System.EventHandler(this.addTrunkMeshToolStripMenuItem_Click);
            // 
            // addLeafGroupToolStripMenuItem
            // 
            this.addLeafGroupToolStripMenuItem.Name = "addLeafGroupToolStripMenuItem";
            this.addLeafGroupToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addLeafGroupToolStripMenuItem.Text = "Add Leaf Group";
            this.addLeafGroupToolStripMenuItem.Click += new System.EventHandler(this.addLeafGroupToolStripMenuItem_Click);
            // 
            // addLeafToolStripMenuItem
            // 
            this.addLeafToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singleLeadToolStripMenuItem,
            this.leafPlacerToolStripMenuItem});
            this.addLeafToolStripMenuItem.Name = "addLeafToolStripMenuItem";
            this.addLeafToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addLeafToolStripMenuItem.Text = "Add Leaf";
            // 
            // singleLeadToolStripMenuItem
            // 
            this.singleLeadToolStripMenuItem.Name = "singleLeadToolStripMenuItem";
            this.singleLeadToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.singleLeadToolStripMenuItem.Text = "Single Leaf";
            this.singleLeadToolStripMenuItem.Click += new System.EventHandler(this.singleLeadToolStripMenuItem_Click);
            // 
            // leafPlacerToolStripMenuItem
            // 
            this.leafPlacerToolStripMenuItem.Name = "leafPlacerToolStripMenuItem";
            this.leafPlacerToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.leafPlacerToolStripMenuItem.Text = "Leaf Placer";
            this.leafPlacerToolStripMenuItem.Click += new System.EventHandler(this.leafPlacerToolStripMenuItem_Click);
            // 
            // addCollisionToolStripMenuItem
            // 
            this.addCollisionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cubeToolStripMenuItem,
            this.capsuleToolStripMenuItem,
            this.sphereToolStripMenuItem});
            this.addCollisionToolStripMenuItem.Name = "addCollisionToolStripMenuItem";
            this.addCollisionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addCollisionToolStripMenuItem.Text = "Add Collision Shape";
            // 
            // cubeToolStripMenuItem
            // 
            this.cubeToolStripMenuItem.Name = "cubeToolStripMenuItem";
            this.cubeToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.cubeToolStripMenuItem.Text = "Capsule";
            this.cubeToolStripMenuItem.Click += new System.EventHandler(this.cubeToolStripMenuItem_Click);
            // 
            // capsuleToolStripMenuItem
            // 
            this.capsuleToolStripMenuItem.Name = "capsuleToolStripMenuItem";
            this.capsuleToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.capsuleToolStripMenuItem.Text = "Cube";
            this.capsuleToolStripMenuItem.Click += new System.EventHandler(this.capsuleToolStripMenuItem_Click);
            // 
            // sphereToolStripMenuItem
            // 
            this.sphereToolStripMenuItem.Name = "sphereToolStripMenuItem";
            this.sphereToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.sphereToolStripMenuItem.Text = "Sphere";
            this.sphereToolStripMenuItem.Click += new System.EventHandler(this.sphereToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // finalizeObjectToolStripMenuItem
            // 
            this.finalizeObjectToolStripMenuItem.Name = "finalizeObjectToolStripMenuItem";
            this.finalizeObjectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.finalizeObjectToolStripMenuItem.Text = "Finalize Object";
            this.finalizeObjectToolStripMenuItem.Click += new System.EventHandler(this.finalizeObjectToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // TextureFileDialog
            // 
            this.TextureFileDialog.Filter = "Supported Formats(*.bmp, *.jpg, *.png, *.dds, *.tga)|*.bmp*;*.jpg;*.jpeg;*.png;*." +
                "dds;*.tga";
            this.TextureFileDialog.SupportMultiDottedExtensions = true;
            this.TextureFileDialog.Title = "Find a texture...";
            // 
            // MeshOpenDialog
            // 
            this.MeshOpenDialog.Filter = "Supported Formats(*.b3d, *.eb3d)|*.b3d*;*.eb3d";
            this.MeshOpenDialog.RestoreDirectory = true;
            this.MeshOpenDialog.SupportMultiDottedExtensions = true;
            // 
            // TreeEditorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 616);
            this.Controls.Add(this.TreeComponentsGroup);
            this.Controls.Add(this.TreePropertiesGroup);
            this.Name = "TreeEditorProperties";
            this.ShowIcon = false;
            this.TabText = "TreeEditorProperties";
            this.Text = "Tree Properties";
            this.TreePropertiesGroup.ResumeLayout(false);
            this.TreePropertiesGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Seed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeafCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionX)).EndInit();
            this.TreeComponentsGroup.ResumeLayout(false);
            this.TreeNodesMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox TreePropertiesGroup;
        private System.Windows.Forms.GroupBox TreeComponentsGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label NormalLabel;
        private System.Windows.Forms.Label DiffuseLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button TextureNormal;
        public System.Windows.Forms.Button TextureDiffuse;
        public System.Windows.Forms.NumericUpDown PositionX;
        public System.Windows.Forms.NumericUpDown ScaleZ;
        public System.Windows.Forms.NumericUpDown RotationZ;
        public System.Windows.Forms.NumericUpDown PositionZ;
        public System.Windows.Forms.NumericUpDown ScaleY;
        public System.Windows.Forms.NumericUpDown RotationY;
        public System.Windows.Forms.NumericUpDown PositionY;
        public System.Windows.Forms.NumericUpDown ScaleX;
        public System.Windows.Forms.NumericUpDown RotationX;
        public System.Windows.Forms.TreeView TreeComponents;
        public System.Windows.Forms.OpenFileDialog TextureFileDialog;
        private System.Windows.Forms.ContextMenuStrip TreeNodesMenu;
        private System.Windows.Forms.ToolStripMenuItem addTrunkMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLeafGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCollisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cubeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capsuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sphereToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog MeshOpenDialog;
        public System.Windows.Forms.NumericUpDown Seed;
        public System.Windows.Forms.NumericUpDown LeafCount;
        public System.Windows.Forms.ToolStripMenuItem addLeafToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem singleLeadToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem leafPlacerToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem finalizeObjectToolStripMenuItem;
    }
}