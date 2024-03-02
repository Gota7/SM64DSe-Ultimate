namespace SM64DSe
{
    partial class MainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
	        this.components = new System.ComponentModel.Container();
	        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
	        this.tsToolBar = new System.Windows.Forms.ToolStrip();
	        this.btnOpenROM = new System.Windows.Forms.ToolStripButton();
	        this.btnHalp = new System.Windows.Forms.ToolStripButton();
	        this.btnOptions = new System.Windows.Forms.ToolStripDropDownButton();
	        this.btnUpdateODB = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnMore = new System.Windows.Forms.ToolStripDropDownButton();
	        this.mnitAdditionalPatches = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitDumpAllOvls = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitDecompressOverlaysWithinGame = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitHexDumpToBinaryFile = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitEditSDATINFOBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.fixMultiplayerChecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.importPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnASMHacking = new System.Windows.Forms.ToolStripDropDownButton();
	        this.mnitASMHackingCompilation = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitASMHackingCompilationCodeCompiler = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitASMHackingCompilationFixCodeOffsets = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitASMHackingGeneration = new System.Windows.Forms.ToolStripMenuItem();
	        this.platformEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.fileHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.soundHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.tssASMHacking001 = new System.Windows.Forms.ToolStripSeparator();
	        this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.dLPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnTools = new System.Windows.Forms.ToolStripDropDownButton();
	        this.mnitToolsModelAndCollisionMapImporter = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitToolsCollisionMapEditor = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitToolsModelAnimationEditor = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitToolsBTPEditor = new System.Windows.Forms.ToolStripMenuItem();
	        this.particleTextureSPTEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.particleArchiveSPAEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitToolsTextEditor = new System.Windows.Forms.ToolStripMenuItem();
	        this.mnitToolsSoundBrowser = new System.Windows.Forms.ToolStripMenuItem();
	        this.bMDKLCEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.kuppaScriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.editFileSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.editOverlaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnOpenFilesystem = new System.Windows.Forms.ToolStripButton();
	        this.btnRunROM = new System.Windows.Forms.ToolStripButton();
	        this.btnBuildROM = new System.Windows.Forms.ToolStripButton();
	        this.extractROMButton = new System.Windows.Forms.ToolStripButton();
	        this.mnitToolsImageEditor = new System.Windows.Forms.ToolStripMenuItem();
	        this.ofdOpenFile = new System.Windows.Forms.OpenFileDialog();
	        this.sfdSaveFile = new System.Windows.Forms.SaveFileDialog();
	        this.ssStatusBar = new System.Windows.Forms.StatusStrip();
	        this.slStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
	        this.spbStatusProgress = new System.Windows.Forms.ToolStripProgressBar();
	        this.updateLabel = new System.Windows.Forms.ToolStripStatusLabel();
	        this.tbcMainFormTabControl = new System.Windows.Forms.TabControl();
	        this.tbpLevels = new System.Windows.Forms.TabPage();
	        this.splitContainerLevels = new System.Windows.Forms.SplitContainer();
	        this.btnRefresh = new System.Windows.Forms.Button();
	        this.cbLevelListDisplay = new System.Windows.Forms.ComboBox();
	        this.lbxLevels = new System.Windows.Forms.ListBox();
	        this.btnEditCollisionMap = new System.Windows.Forms.Button();
	        this.btnEditLevel = new System.Windows.Forms.Button();
	        this.btnEditLevelNames = new System.Windows.Forms.Button();
	        this.tbpFileSystem = new System.Windows.Forms.TabPage();
	        this.spcFileSystemTab = new System.Windows.Forms.SplitContainer();
	        this.tvFileList = new System.Windows.Forms.TreeView();
	        this.pnlFileOptions = new System.Windows.Forms.Panel();
	        this.btnLZForceCompression = new System.Windows.Forms.Button();
	        this.btnLZCompressWithHeader = new System.Windows.Forms.Button();
	        this.btnLZForceDecompression = new System.Windows.Forms.Button();
	        this.btnLZDecompressWithHeader = new System.Windows.Forms.Button();
	        this.btnReplaceRaw = new System.Windows.Forms.Button();
	        this.btnOpenFile = new System.Windows.Forms.Button();
	        this.btnExtractRaw = new System.Windows.Forms.Button();
	        this.tbpARM9Overlays = new System.Windows.Forms.TabPage();
	        this.spcARM9Overlays = new System.Windows.Forms.SplitContainer();
	        this.tvARM9Overlays = new System.Windows.Forms.TreeView();
	        this.btnReplaceOverlay = new System.Windows.Forms.Button();
	        this.btnExtractOverlay = new System.Windows.Forms.Button();
	        this.btnDecompressOverlay = new System.Windows.Forms.Button();
	        this.addons = new System.Windows.Forms.TabPage();
	        this.btnOpenAddonFolder = new System.Windows.Forms.Button();
	        this.btnInstall = new System.Windows.Forms.Button();
	        this.addonsChoice = new System.Windows.Forms.ComboBox();
	        this.btn_open_github = new System.Windows.Forms.Button();
	        this.btnDownloadAddon = new System.Windows.Forms.Button();
	        this.addons_list = new System.Windows.Forms.ListView();
	        this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
	        this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
	        this.addons_image_list = new System.Windows.Forms.ImageList(this.components);
	        this.btnRefreshAddons = new System.Windows.Forms.Button();
	        this.tsToolBar.SuspendLayout();
	        this.ssStatusBar.SuspendLayout();
	        this.tbcMainFormTabControl.SuspendLayout();
	        this.tbpLevels.SuspendLayout();
	        ((System.ComponentModel.ISupportInitialize)(this.splitContainerLevels)).BeginInit();
	        this.splitContainerLevels.Panel1.SuspendLayout();
	        this.splitContainerLevels.Panel2.SuspendLayout();
	        this.splitContainerLevels.SuspendLayout();
	        this.tbpFileSystem.SuspendLayout();
	        ((System.ComponentModel.ISupportInitialize)(this.spcFileSystemTab)).BeginInit();
	        this.spcFileSystemTab.Panel1.SuspendLayout();
	        this.spcFileSystemTab.Panel2.SuspendLayout();
	        this.spcFileSystemTab.SuspendLayout();
	        this.pnlFileOptions.SuspendLayout();
	        this.tbpARM9Overlays.SuspendLayout();
	        ((System.ComponentModel.ISupportInitialize)(this.spcARM9Overlays)).BeginInit();
	        this.spcARM9Overlays.Panel1.SuspendLayout();
	        this.spcARM9Overlays.Panel2.SuspendLayout();
	        this.spcARM9Overlays.SuspendLayout();
	        this.addons.SuspendLayout();
	        this.SuspendLayout();
	        // 
	        // tsToolBar
	        // 
	        this.tsToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.btnOpenROM, this.btnHalp, this.btnOptions, this.btnMore, this.btnASMHacking, this.btnTools, this.btnOpenFilesystem, this.btnRunROM, this.btnBuildROM, this.extractROMButton });
	        this.tsToolBar.Location = new System.Drawing.Point(0, 0);
	        this.tsToolBar.Name = "tsToolBar";
	        this.tsToolBar.Size = new System.Drawing.Size(576, 25);
	        this.tsToolBar.TabIndex = 0;
	        this.tsToolBar.Text = "toolStrip1";
	        this.tsToolBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsToolBar_ItemClicked);
	        // 
	        // btnOpenROM
	        // 
	        this.btnOpenROM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnOpenROM.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenROM.Image")));
	        this.btnOpenROM.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnOpenROM.Name = "btnOpenROM";
	        this.btnOpenROM.Size = new System.Drawing.Size(70, 22);
	        this.btnOpenROM.Text = "Open ROM";
	        this.btnOpenROM.Click += new System.EventHandler(this.btnOpenROM_Click);
	        // 
	        // btnHalp
	        // 
	        this.btnHalp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnHalp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnHalp.Image = ((System.Drawing.Image)(resources.GetObject("btnHalp.Image")));
	        this.btnHalp.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnHalp.Name = "btnHalp";
	        this.btnHalp.Size = new System.Drawing.Size(23, 22);
	        this.btnHalp.Text = "?";
	        this.btnHalp.ToolTipText = "Help, about, etc...";
	        this.btnHalp.Click += new System.EventHandler(this.btnHalp_Click);
	        // 
	        // btnOptions
	        // 
	        this.btnOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.btnUpdateODB, this.btnEditorSettings });
	        this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
	        this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnOptions.Name = "btnOptions";
	        this.btnOptions.Size = new System.Drawing.Size(62, 22);
	        this.btnOptions.Text = "Options";
	        // 
	        // btnUpdateODB
	        // 
	        this.btnUpdateODB.Name = "btnUpdateODB";
	        this.btnUpdateODB.Size = new System.Drawing.Size(198, 22);
	        this.btnUpdateODB.Text = "Update object database";
	        this.btnUpdateODB.Visible = false;
	        this.btnUpdateODB.Click += new System.EventHandler(this.btnUpdateODB_Click);
	        // 
	        // btnEditorSettings
	        // 
	        this.btnEditorSettings.Name = "btnEditorSettings";
	        this.btnEditorSettings.Size = new System.Drawing.Size(198, 22);
	        this.btnEditorSettings.Text = "Editor settings";
	        this.btnEditorSettings.Click += new System.EventHandler(this.btnEditorSettings_Click);
	        // 
	        // btnMore
	        // 
	        this.btnMore.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnMore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnMore.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnitAdditionalPatches, this.mnitDumpAllOvls, this.mnitDecompressOverlaysWithinGame, this.mnitHexDumpToBinaryFile, this.mnitEditSDATINFOBlockToolStripMenuItem, this.fixMultiplayerChecksToolStripMenuItem, this.importPatchToolStripMenuItem });
	        this.btnMore.Enabled = false;
	        this.btnMore.Image = ((System.Drawing.Image)(resources.GetObject("btnMore.Image")));
	        this.btnMore.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnMore.Name = "btnMore";
	        this.btnMore.Size = new System.Drawing.Size(48, 22);
	        this.btnMore.Text = "More";
	        // 
	        // mnitAdditionalPatches
	        // 
	        this.mnitAdditionalPatches.Name = "mnitAdditionalPatches";
	        this.mnitAdditionalPatches.Size = new System.Drawing.Size(259, 22);
	        this.mnitAdditionalPatches.Text = "Additional Patches";
	        this.mnitAdditionalPatches.Click += new System.EventHandler(this.mnitAdditionalPatches_Click);
	        // 
	        // mnitDumpAllOvls
	        // 
	        this.mnitDumpAllOvls.Name = "mnitDumpAllOvls";
	        this.mnitDumpAllOvls.Size = new System.Drawing.Size(259, 22);
	        this.mnitDumpAllOvls.Text = "Dump All Overlays";
	        this.mnitDumpAllOvls.Click += new System.EventHandler(this.mnitDumpAllOvls_Click);
	        // 
	        // mnitDecompressOverlaysWithinGame
	        // 
	        this.mnitDecompressOverlaysWithinGame.Name = "mnitDecompressOverlaysWithinGame";
	        this.mnitDecompressOverlaysWithinGame.Size = new System.Drawing.Size(259, 22);
	        this.mnitDecompressOverlaysWithinGame.Text = "Decompress Overlays Within Game";
	        this.mnitDecompressOverlaysWithinGame.Click += new System.EventHandler(this.mnitDecompressOverlaysWithinGame_Click);
	        // 
	        // mnitHexDumpToBinaryFile
	        // 
	        this.mnitHexDumpToBinaryFile.Name = "mnitHexDumpToBinaryFile";
	        this.mnitHexDumpToBinaryFile.Size = new System.Drawing.Size(259, 22);
	        this.mnitHexDumpToBinaryFile.Text = "Hex Dump to Binary File";
	        this.mnitHexDumpToBinaryFile.Click += new System.EventHandler(this.mnitHexDumpToBinaryFile_Click);
	        // 
	        // mnitEditSDATINFOBlockToolStripMenuItem
	        // 
	        this.mnitEditSDATINFOBlockToolStripMenuItem.Name = "mnitEditSDATINFOBlockToolStripMenuItem";
	        this.mnitEditSDATINFOBlockToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
	        this.mnitEditSDATINFOBlockToolStripMenuItem.Text = "Edit SDAT INFO Block";
	        this.mnitEditSDATINFOBlockToolStripMenuItem.Click += new System.EventHandler(this.mnitEditSDATINFOBlockToolStripMenuItem_Click);
	        // 
	        // fixMultiplayerChecksToolStripMenuItem
	        // 
	        this.fixMultiplayerChecksToolStripMenuItem.Name = "fixMultiplayerChecksToolStripMenuItem";
	        this.fixMultiplayerChecksToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
	        this.fixMultiplayerChecksToolStripMenuItem.Text = "Fix Multiplayer Checks";
	        this.fixMultiplayerChecksToolStripMenuItem.Visible = false;
	        this.fixMultiplayerChecksToolStripMenuItem.Click += new System.EventHandler(this.fixMultiplayerChecksToolStripMenuItem_Click);
	        // 
	        // importPatchToolStripMenuItem
	        // 
	        this.importPatchToolStripMenuItem.Name = "importPatchToolStripMenuItem";
	        this.importPatchToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
	        this.importPatchToolStripMenuItem.Text = "Import Patch";
	        this.importPatchToolStripMenuItem.Click += new System.EventHandler(this.importPatchToolStripMenuItem_Click);
	        // 
	        // btnASMHacking
	        // 
	        this.btnASMHacking.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnASMHacking.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnASMHacking.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnitASMHackingCompilation, this.mnitASMHackingGeneration, this.tssASMHacking001, this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem, this.dLPatchToolStripMenuItem });
	        this.btnASMHacking.Enabled = false;
	        this.btnASMHacking.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnASMHacking.Name = "btnASMHacking";
	        this.btnASMHacking.Size = new System.Drawing.Size(92, 22);
	        this.btnASMHacking.Text = "ASM Hacking";
	        // 
	        // mnitASMHackingCompilation
	        // 
	        this.mnitASMHackingCompilation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnitASMHackingCompilationCodeCompiler, this.mnitASMHackingCompilationFixCodeOffsets });
	        this.mnitASMHackingCompilation.Name = "mnitASMHackingCompilation";
	        this.mnitASMHackingCompilation.Size = new System.Drawing.Size(302, 22);
	        this.mnitASMHackingCompilation.Text = "Compilation";
	        // 
	        // mnitASMHackingCompilationCodeCompiler
	        // 
	        this.mnitASMHackingCompilationCodeCompiler.Name = "mnitASMHackingCompilationCodeCompiler";
	        this.mnitASMHackingCompilationCodeCompiler.Size = new System.Drawing.Size(160, 22);
	        this.mnitASMHackingCompilationCodeCompiler.Text = "Code Compiler";
	        this.mnitASMHackingCompilationCodeCompiler.Click += new System.EventHandler(this.mnitASMHackingCompilationCodeCompiler_Click);
	        // 
	        // mnitASMHackingCompilationFixCodeOffsets
	        // 
	        this.mnitASMHackingCompilationFixCodeOffsets.Name = "mnitASMHackingCompilationFixCodeOffsets";
	        this.mnitASMHackingCompilationFixCodeOffsets.Size = new System.Drawing.Size(160, 22);
	        this.mnitASMHackingCompilationFixCodeOffsets.Text = "Fix Code Offsets";
	        this.mnitASMHackingCompilationFixCodeOffsets.Click += new System.EventHandler(this.mnitASMHackingCompilationFixCodeOffsets_Click);
	        // 
	        // mnitASMHackingGeneration
	        // 
	        this.mnitASMHackingGeneration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.platformEditorToolStripMenuItem, this.fileHeaderToolStripMenuItem, this.soundHeaderToolStripMenuItem });
	        this.mnitASMHackingGeneration.Name = "mnitASMHackingGeneration";
	        this.mnitASMHackingGeneration.Size = new System.Drawing.Size(302, 22);
	        this.mnitASMHackingGeneration.Text = "Generation";
	        // 
	        // platformEditorToolStripMenuItem
	        // 
	        this.platformEditorToolStripMenuItem.Name = "platformEditorToolStripMenuItem";
	        this.platformEditorToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
	        this.platformEditorToolStripMenuItem.Text = "Platform Editor";
	        this.platformEditorToolStripMenuItem.Click += new System.EventHandler(this.platformEditorToolStripMenuItem_Click);
	        // 
	        // fileHeaderToolStripMenuItem
	        // 
	        this.fileHeaderToolStripMenuItem.Name = "fileHeaderToolStripMenuItem";
	        this.fileHeaderToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
	        this.fileHeaderToolStripMenuItem.Text = "File Header";
	        this.fileHeaderToolStripMenuItem.Click += new System.EventHandler(this.fileHeaderToolStripMenuItem_Click);
	        // 
	        // soundHeaderToolStripMenuItem
	        // 
	        this.soundHeaderToolStripMenuItem.Name = "soundHeaderToolStripMenuItem";
	        this.soundHeaderToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
	        this.soundHeaderToolStripMenuItem.Text = "Sound Header";
	        this.soundHeaderToolStripMenuItem.Click += new System.EventHandler(this.soundHeaderToolStripMenuItem_Click);
	        // 
	        // tssASMHacking001
	        // 
	        this.tssASMHacking001.Name = "tssASMHacking001";
	        this.tssASMHacking001.Size = new System.Drawing.Size(299, 6);
	        // 
	        // mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem
	        // 
	        this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Name = "mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem";
	        this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
	        this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Text = "Toggle Suitability for NSMBe ASM Patching";
	        this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Click += new System.EventHandler(this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem_Click);
	        // 
	        // dLPatchToolStripMenuItem
	        // 
	        this.dLPatchToolStripMenuItem.Name = "dLPatchToolStripMenuItem";
	        this.dLPatchToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
	        this.dLPatchToolStripMenuItem.Text = "DL Patch";
	        this.dLPatchToolStripMenuItem.Click += new System.EventHandler(this.dLPatchToolStripMenuItem_Click);
	        // 
	        // btnTools
	        // 
	        this.btnTools.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnitToolsModelAndCollisionMapImporter, this.mnitToolsCollisionMapEditor, this.mnitToolsModelAnimationEditor, this.mnitToolsBTPEditor, this.particleTextureSPTEditorToolStripMenuItem, this.particleArchiveSPAEditorToolStripMenuItem, this.mnitToolsTextEditor, this.mnitToolsSoundBrowser, this.bMDKLCEditorToolStripMenuItem, this.kuppaScriptEditorToolStripMenuItem, this.editFileSystemToolStripMenuItem, this.editOverlaysToolStripMenuItem });
	        this.btnTools.Enabled = false;
	        this.btnTools.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnTools.Name = "btnTools";
	        this.btnTools.Size = new System.Drawing.Size(47, 22);
	        this.btnTools.Text = "Tools";
	        // 
	        // mnitToolsModelAndCollisionMapImporter
	        // 
	        this.mnitToolsModelAndCollisionMapImporter.Name = "mnitToolsModelAndCollisionMapImporter";
	        this.mnitToolsModelAndCollisionMapImporter.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsModelAndCollisionMapImporter.Text = "Model and Collision Map Importer";
	        this.mnitToolsModelAndCollisionMapImporter.Click += new System.EventHandler(this.mnitToolsModelAndCollisionMapImporter_Click);
	        // 
	        // mnitToolsCollisionMapEditor
	        // 
	        this.mnitToolsCollisionMapEditor.Name = "mnitToolsCollisionMapEditor";
	        this.mnitToolsCollisionMapEditor.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsCollisionMapEditor.Text = "Collision Map Editor";
	        this.mnitToolsCollisionMapEditor.Click += new System.EventHandler(this.mnitToolsCollisionMapEditor_Click);
	        // 
	        // mnitToolsModelAnimationEditor
	        // 
	        this.mnitToolsModelAnimationEditor.Name = "mnitToolsModelAnimationEditor";
	        this.mnitToolsModelAnimationEditor.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsModelAnimationEditor.Text = "Model Animation Editor";
	        this.mnitToolsModelAnimationEditor.Click += new System.EventHandler(this.mnitToolsModelAnimationEditor_Click);
	        // 
	        // mnitToolsBTPEditor
	        // 
	        this.mnitToolsBTPEditor.Name = "mnitToolsBTPEditor";
	        this.mnitToolsBTPEditor.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsBTPEditor.Text = "Texture Pattern (BTP) Editor";
	        this.mnitToolsBTPEditor.Click += new System.EventHandler(this.mnitToolsBTPEditor_Click);
	        // 
	        // particleTextureSPTEditorToolStripMenuItem
	        // 
	        this.particleTextureSPTEditorToolStripMenuItem.Name = "particleTextureSPTEditorToolStripMenuItem";
	        this.particleTextureSPTEditorToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.particleTextureSPTEditorToolStripMenuItem.Text = "Particle Texture (SPT) Editor";
	        this.particleTextureSPTEditorToolStripMenuItem.Click += new System.EventHandler(this.particleTextureSPTEditorToolStripMenuItem_Click);
	        // 
	        // particleArchiveSPAEditorToolStripMenuItem
	        // 
	        this.particleArchiveSPAEditorToolStripMenuItem.Name = "particleArchiveSPAEditorToolStripMenuItem";
	        this.particleArchiveSPAEditorToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.particleArchiveSPAEditorToolStripMenuItem.Text = "Particle Archive (SPA) Editor";
	        this.particleArchiveSPAEditorToolStripMenuItem.Click += new System.EventHandler(this.particleArchiveSPAEditorToolStripMenuItem_Click);
	        // 
	        // mnitToolsTextEditor
	        // 
	        this.mnitToolsTextEditor.Name = "mnitToolsTextEditor";
	        this.mnitToolsTextEditor.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsTextEditor.Text = "Text Editor";
	        this.mnitToolsTextEditor.Click += new System.EventHandler(this.mnitToolsTextEditor_Click);
	        // 
	        // mnitToolsSoundBrowser
	        // 
	        this.mnitToolsSoundBrowser.Name = "mnitToolsSoundBrowser";
	        this.mnitToolsSoundBrowser.Size = new System.Drawing.Size(256, 22);
	        this.mnitToolsSoundBrowser.Text = "Sound Browser";
	        this.mnitToolsSoundBrowser.Click += new System.EventHandler(this.mnitToolsSoundBrowser_Click);
	        // 
	        // bMDKLCEditorToolStripMenuItem
	        // 
	        this.bMDKLCEditorToolStripMenuItem.Name = "bMDKLCEditorToolStripMenuItem";
	        this.bMDKLCEditorToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.bMDKLCEditorToolStripMenuItem.Text = "BMD/KLC Editor";
	        this.bMDKLCEditorToolStripMenuItem.Click += new System.EventHandler(this.bMDKLCEditorToolStripMenuItem_Click);
	        // 
	        // kuppaScriptEditorToolStripMenuItem
	        // 
	        this.kuppaScriptEditorToolStripMenuItem.Name = "kuppaScriptEditorToolStripMenuItem";
	        this.kuppaScriptEditorToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.kuppaScriptEditorToolStripMenuItem.Text = "Kuppa Script Editor";
	        this.kuppaScriptEditorToolStripMenuItem.Click += new System.EventHandler(this.kuppaScriptEditorToolStripMenuItem_Click);
	        // 
	        // editFileSystemToolStripMenuItem
	        // 
	        this.editFileSystemToolStripMenuItem.Name = "editFileSystemToolStripMenuItem";
	        this.editFileSystemToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.editFileSystemToolStripMenuItem.Text = "Edit File System";
	        this.editFileSystemToolStripMenuItem.Click += new System.EventHandler(this.editFileSystemToolStripMenuItem_Click);
	        // 
	        // editOverlaysToolStripMenuItem
	        // 
	        this.editOverlaysToolStripMenuItem.Name = "editOverlaysToolStripMenuItem";
	        this.editOverlaysToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
	        this.editOverlaysToolStripMenuItem.Text = "Edit Overlays";
	        this.editOverlaysToolStripMenuItem.Click += new System.EventHandler(this.editOverlaysToolStripMenuItem_Click);
	        // 
	        // btnOpenFilesystem
	        // 
	        this.btnOpenFilesystem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.btnOpenFilesystem.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFilesystem.Image")));
	        this.btnOpenFilesystem.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnOpenFilesystem.Name = "btnOpenFilesystem";
	        this.btnOpenFilesystem.Size = new System.Drawing.Size(122, 22);
	        this.btnOpenFilesystem.Text = "Open Extracted ROM";
	        this.btnOpenFilesystem.ToolTipText = "Open An Extracted ROM";
	        this.btnOpenFilesystem.Click += new System.EventHandler(this.btnOpenFilesystem_Click);
	        // 
	        // btnRunROM
	        // 
	        this.btnRunROM.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnRunROM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
	        this.btnRunROM.Image = global::SM64DSe.Properties.Resources.Play;
	        this.btnRunROM.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnRunROM.Name = "btnRunROM";
	        this.btnRunROM.Size = new System.Drawing.Size(23, 22);
	        this.btnRunROM.Text = "toolStripButton2";
	        this.btnRunROM.ToolTipText = "Build and run the ROM.";
	        this.btnRunROM.Visible = false;
	        this.btnRunROM.Click += new System.EventHandler(this.btnRunROM_Click);
	        // 
	        // btnBuildROM
	        // 
	        this.btnBuildROM.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
	        this.btnBuildROM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
	        this.btnBuildROM.Image = global::SM64DSe.Properties.Resources.build;
	        this.btnBuildROM.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.btnBuildROM.Name = "btnBuildROM";
	        this.btnBuildROM.Size = new System.Drawing.Size(23, 22);
	        this.btnBuildROM.Text = "toolStripButton1";
	        this.btnBuildROM.ToolTipText = "Build the ROM.";
	        this.btnBuildROM.Visible = false;
	        this.btnBuildROM.Click += new System.EventHandler(this.btnBuildROM_Click);
	        // 
	        // extractROMButton
	        // 
	        this.extractROMButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
	        this.extractROMButton.Image = ((System.Drawing.Image)(resources.GetObject("extractROMButton.Image")));
	        this.extractROMButton.ImageTransparentColor = System.Drawing.Color.Magenta;
	        this.extractROMButton.Name = "extractROMButton";
	        this.extractROMButton.Size = new System.Drawing.Size(77, 22);
	        this.extractROMButton.Text = "Extract ROM";
	        this.extractROMButton.ToolTipText = "Extract ROM.";
	        this.extractROMButton.Visible = false;
	        this.extractROMButton.Click += new System.EventHandler(this.extractROMButton_Click);
	        // 
	        // mnitToolsImageEditor
	        // 
	        this.mnitToolsImageEditor.Name = "mnitToolsImageEditor";
	        this.mnitToolsImageEditor.Size = new System.Drawing.Size(241, 22);
	        this.mnitToolsImageEditor.Text = "Image Editor";
	        this.mnitToolsImageEditor.Click += new System.EventHandler(this.mnitToolsImageEditor_Click);
	        // 
	        // ofdOpenFile
	        // 
	        this.ofdOpenFile.DefaultExt = "nds";
	        this.ofdOpenFile.Filter = "Nintendo DS ROM|*.nds|Any file|*.*";
	        this.ofdOpenFile.Title = "Open ROM...";
	        // 
	        // sfdSaveFile
	        // 
	        this.sfdSaveFile.DefaultExt = "nds";
	        this.sfdSaveFile.Filter = "Nintendo DS ROM|*.nds|Any file|*.*";
	        this.sfdSaveFile.RestoreDirectory = true;
	        this.sfdSaveFile.Title = "Backup ROM...";
	        // 
	        // ssStatusBar
	        // 
	        this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.slStatusLabel, this.spbStatusProgress, this.updateLabel });
	        this.ssStatusBar.Location = new System.Drawing.Point(0, 443);
	        this.ssStatusBar.Name = "ssStatusBar";
	        this.ssStatusBar.Size = new System.Drawing.Size(576, 22);
	        this.ssStatusBar.TabIndex = 2;
	        this.ssStatusBar.Text = "statusStrip1";
	        // 
	        // slStatusLabel
	        // 
	        this.slStatusLabel.Name = "slStatusLabel";
	        this.slStatusLabel.Size = new System.Drawing.Size(51, 17);
	        this.slStatusLabel.Text = "lolstatus";
	        // 
	        // spbStatusProgress
	        // 
	        this.spbStatusProgress.Name = "spbStatusProgress";
	        this.spbStatusProgress.Size = new System.Drawing.Size(150, 16);
	        this.spbStatusProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
	        this.spbStatusProgress.Visible = false;
	        // 
	        // updateLabel
	        // 
	        this.updateLabel.Image = global::SM64DSe.Properties.Resources.github;
	        this.updateLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
	        this.updateLabel.IsLink = true;
	        this.updateLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
	        this.updateLabel.LinkColor = System.Drawing.Color.Black;
	        this.updateLabel.Margin = new System.Windows.Forms.Padding(6, 3, 0, 2);
	        this.updateLabel.Name = "updateLabel";
	        this.updateLabel.Size = new System.Drawing.Size(200, 26);
	        this.updateLabel.Text = "New Version Available";
	        this.updateLabel.ToolTipText = "Click for details";
	        this.updateLabel.Visible = false;
	        this.updateLabel.VisitedLinkColor = System.Drawing.Color.Black;
	        this.updateLabel.Click += new System.EventHandler(this.updateLabel_Click);
	        // 
	        // tbcMainFormTabControl
	        // 
	        this.tbcMainFormTabControl.AccessibleName = "";
	        this.tbcMainFormTabControl.Controls.Add(this.tbpLevels);
	        this.tbcMainFormTabControl.Controls.Add(this.tbpFileSystem);
	        this.tbcMainFormTabControl.Controls.Add(this.tbpARM9Overlays);
	        this.tbcMainFormTabControl.Controls.Add(this.addons);
	        this.tbcMainFormTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.tbcMainFormTabControl.Location = new System.Drawing.Point(0, 25);
	        this.tbcMainFormTabControl.Name = "tbcMainFormTabControl";
	        this.tbcMainFormTabControl.SelectedIndex = 0;
	        this.tbcMainFormTabControl.Size = new System.Drawing.Size(576, 418);
	        this.tbcMainFormTabControl.TabIndex = 3;
	        // 
	        // tbpLevels
	        // 
	        this.tbpLevels.Controls.Add(this.splitContainerLevels);
	        this.tbpLevels.Location = new System.Drawing.Point(4, 22);
	        this.tbpLevels.Name = "tbpLevels";
	        this.tbpLevels.Padding = new System.Windows.Forms.Padding(3);
	        this.tbpLevels.Size = new System.Drawing.Size(568, 392);
	        this.tbpLevels.TabIndex = 0;
	        this.tbpLevels.Text = "Levels";
	        this.tbpLevels.UseVisualStyleBackColor = true;
	        // 
	        // splitContainerLevels
	        // 
	        this.splitContainerLevels.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.splitContainerLevels.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
	        this.splitContainerLevels.IsSplitterFixed = true;
	        this.splitContainerLevels.Location = new System.Drawing.Point(3, 3);
	        this.splitContainerLevels.Name = "splitContainerLevels";
	        this.splitContainerLevels.Orientation = System.Windows.Forms.Orientation.Horizontal;
	        // 
	        // splitContainerLevels.Panel1
	        // 
	        this.splitContainerLevels.Panel1.Controls.Add(this.btnRefresh);
	        this.splitContainerLevels.Panel1.Controls.Add(this.cbLevelListDisplay);
	        this.splitContainerLevels.Panel1.Controls.Add(this.lbxLevels);
	        // 
	        // splitContainerLevels.Panel2
	        // 
	        this.splitContainerLevels.Panel2.Controls.Add(this.btnEditCollisionMap);
	        this.splitContainerLevels.Panel2.Controls.Add(this.btnEditLevel);
	        this.splitContainerLevels.Panel2.Controls.Add(this.btnEditLevelNames);
	        this.splitContainerLevels.Size = new System.Drawing.Size(562, 386);
	        this.splitContainerLevels.SplitterDistance = 357;
	        this.splitContainerLevels.TabIndex = 0;
	        // 
	        // btnRefresh
	        // 
	        this.btnRefresh.AllowDrop = true;
	        this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
	        this.btnRefresh.Enabled = false;
	        this.btnRefresh.Location = new System.Drawing.Point(271, 0);
	        this.btnRefresh.Name = "btnRefresh";
	        this.btnRefresh.Size = new System.Drawing.Size(75, 21);
	        this.btnRefresh.TabIndex = 4;
	        this.btnRefresh.Text = "Refresh";
	        this.btnRefresh.UseVisualStyleBackColor = true;
	        this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
	        // 
	        // cbLevelListDisplay
	        // 
	        this.cbLevelListDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
	        this.cbLevelListDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
	        this.cbLevelListDisplay.Enabled = false;
	        this.cbLevelListDisplay.FormattingEnabled = true;
	        this.cbLevelListDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
	        this.cbLevelListDisplay.Items.AddRange(new object[] { "Default", "Actual Name", "Id+Internal Name", "ShortActual+InternalString", "Optimized Internal String" });
	        this.cbLevelListDisplay.Location = new System.Drawing.Point(352, 0);
	        this.cbLevelListDisplay.Name = "cbLevelListDisplay";
	        this.cbLevelListDisplay.Size = new System.Drawing.Size(205, 21);
	        this.cbLevelListDisplay.TabIndex = 3;
	        this.cbLevelListDisplay.SelectedIndexChanged += new System.EventHandler(this.cbLevelListDisplay_SelectedIndexChanged);
	        // 
	        // lbxLevels
	        // 
	        this.lbxLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
	        this.lbxLevels.Font = new System.Drawing.Font("Consolas", 8F);
	        this.lbxLevels.FormattingEnabled = true;
	        this.lbxLevels.IntegralHeight = false;
	        this.lbxLevels.Location = new System.Drawing.Point(0, 24);
	        this.lbxLevels.Margin = new System.Windows.Forms.Padding(0);
	        this.lbxLevels.Name = "lbxLevels";
	        this.lbxLevels.Size = new System.Drawing.Size(557, 333);
	        this.lbxLevels.TabIndex = 2;
	        this.lbxLevels.SelectedIndexChanged += new System.EventHandler(this.lbxLevels_SelectedIndexChanged);
	        this.lbxLevels.DoubleClick += new System.EventHandler(this.lbxLevels_DoubleClick);
	        // 
	        // btnEditCollisionMap
	        // 
	        this.btnEditCollisionMap.Enabled = false;
	        this.btnEditCollisionMap.Location = new System.Drawing.Point(85, 3);
	        this.btnEditCollisionMap.Name = "btnEditCollisionMap";
	        this.btnEditCollisionMap.Size = new System.Drawing.Size(100, 23);
	        this.btnEditCollisionMap.TabIndex = 1;
	        this.btnEditCollisionMap.Text = "Edit Collision Map";
	        this.btnEditCollisionMap.UseVisualStyleBackColor = true;
	        this.btnEditCollisionMap.Click += new System.EventHandler(this.btnEditCollisionMap_Click);
	        // 
	        // btnEditLevel
	        // 
	        this.btnEditLevel.Enabled = false;
	        this.btnEditLevel.Location = new System.Drawing.Point(3, 3);
	        this.btnEditLevel.Name = "btnEditLevel";
	        this.btnEditLevel.Size = new System.Drawing.Size(75, 23);
	        this.btnEditLevel.TabIndex = 0;
	        this.btnEditLevel.Text = "Edit Level";
	        this.btnEditLevel.UseVisualStyleBackColor = true;
	        this.btnEditLevel.Click += new System.EventHandler(this.btnEditLevel_Click);
	        // 
	        // btnEditLevelNames
	        // 
	        this.btnEditLevelNames.Enabled = false;
	        this.btnEditLevelNames.Location = new System.Drawing.Point(192, 3);
	        this.btnEditLevelNames.Name = "btnEditLevelNames";
	        this.btnEditLevelNames.Size = new System.Drawing.Size(100, 23);
	        this.btnEditLevelNames.TabIndex = 0;
	        this.btnEditLevelNames.Text = "Edit Level Names / Overlays";
	        this.btnEditLevelNames.UseVisualStyleBackColor = true;
	        this.btnEditLevelNames.Click += new System.EventHandler(this.btnEditLevelNames_Click);
	        // 
	        // tbpFileSystem
	        // 
	        this.tbpFileSystem.Controls.Add(this.spcFileSystemTab);
	        this.tbpFileSystem.Location = new System.Drawing.Point(4, 22);
	        this.tbpFileSystem.Name = "tbpFileSystem";
	        this.tbpFileSystem.Padding = new System.Windows.Forms.Padding(3);
	        this.tbpFileSystem.Size = new System.Drawing.Size(568, 392);
	        this.tbpFileSystem.TabIndex = 1;
	        this.tbpFileSystem.Text = "File System";
	        this.tbpFileSystem.UseVisualStyleBackColor = true;
	        // 
	        // spcFileSystemTab
	        // 
	        this.spcFileSystemTab.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.spcFileSystemTab.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
	        this.spcFileSystemTab.IsSplitterFixed = true;
	        this.spcFileSystemTab.Location = new System.Drawing.Point(3, 3);
	        this.spcFileSystemTab.Name = "spcFileSystemTab";
	        this.spcFileSystemTab.Orientation = System.Windows.Forms.Orientation.Horizontal;
	        // 
	        // spcFileSystemTab.Panel1
	        // 
	        this.spcFileSystemTab.Panel1.Controls.Add(this.tvFileList);
	        // 
	        // spcFileSystemTab.Panel2
	        // 
	        this.spcFileSystemTab.Panel2.Controls.Add(this.pnlFileOptions);
	        this.spcFileSystemTab.Size = new System.Drawing.Size(562, 386);
	        this.spcFileSystemTab.SplitterDistance = 330;
	        this.spcFileSystemTab.TabIndex = 2;
	        // 
	        // tvFileList
	        // 
	        this.tvFileList.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.tvFileList.Location = new System.Drawing.Point(0, 0);
	        this.tvFileList.Name = "tvFileList";
	        this.tvFileList.Size = new System.Drawing.Size(562, 330);
	        this.tvFileList.TabIndex = 0;
	        this.tvFileList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFileList_AfterSelect);
	        this.tvFileList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFileList_NodeMouseDoubleClick);
	        // 
	        // pnlFileOptions
	        // 
	        this.pnlFileOptions.Controls.Add(this.btnLZForceCompression);
	        this.pnlFileOptions.Controls.Add(this.btnLZCompressWithHeader);
	        this.pnlFileOptions.Controls.Add(this.btnLZForceDecompression);
	        this.pnlFileOptions.Controls.Add(this.btnLZDecompressWithHeader);
	        this.pnlFileOptions.Controls.Add(this.btnReplaceRaw);
	        this.pnlFileOptions.Controls.Add(this.btnOpenFile);
	        this.pnlFileOptions.Controls.Add(this.btnExtractRaw);
	        this.pnlFileOptions.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.pnlFileOptions.Location = new System.Drawing.Point(0, 0);
	        this.pnlFileOptions.Name = "pnlFileOptions";
	        this.pnlFileOptions.Size = new System.Drawing.Size(562, 52);
	        this.pnlFileOptions.TabIndex = 1;
	        // 
	        // btnLZForceCompression
	        // 
	        this.btnLZForceCompression.Location = new System.Drawing.Point(357, 26);
	        this.btnLZForceCompression.Name = "btnLZForceCompression";
	        this.btnLZForceCompression.Size = new System.Drawing.Size(134, 23);
	        this.btnLZForceCompression.TabIndex = 7;
	        this.btnLZForceCompression.Text = "LZ Force Compression";
	        this.btnLZForceCompression.UseVisualStyleBackColor = true;
	        this.btnLZForceCompression.Click += new System.EventHandler(this.btnLZForceCompression_Click);
	        // 
	        // btnLZCompressWithHeader
	        // 
	        this.btnLZCompressWithHeader.Location = new System.Drawing.Point(357, 2);
	        this.btnLZCompressWithHeader.Name = "btnLZCompressWithHeader";
	        this.btnLZCompressWithHeader.Size = new System.Drawing.Size(134, 23);
	        this.btnLZCompressWithHeader.TabIndex = 6;
	        this.btnLZCompressWithHeader.Text = "LZ Compress with Hdr.";
	        this.btnLZCompressWithHeader.UseVisualStyleBackColor = true;
	        this.btnLZCompressWithHeader.Click += new System.EventHandler(this.btnLZCompressWithHeader_Click);
	        // 
	        // btnLZForceDecompression
	        // 
	        this.btnLZForceDecompression.Location = new System.Drawing.Point(217, 26);
	        this.btnLZForceDecompression.Name = "btnLZForceDecompression";
	        this.btnLZForceDecompression.Size = new System.Drawing.Size(137, 23);
	        this.btnLZForceDecompression.TabIndex = 5;
	        this.btnLZForceDecompression.Text = "LZ Force Decompression";
	        this.btnLZForceDecompression.UseVisualStyleBackColor = true;
	        this.btnLZForceDecompression.Click += new System.EventHandler(this.btnLZForceDecompression_Click);
	        // 
	        // btnLZDecompressWithHeader
	        // 
	        this.btnLZDecompressWithHeader.Location = new System.Drawing.Point(217, 2);
	        this.btnLZDecompressWithHeader.Name = "btnLZDecompressWithHeader";
	        this.btnLZDecompressWithHeader.Size = new System.Drawing.Size(137, 23);
	        this.btnLZDecompressWithHeader.TabIndex = 4;
	        this.btnLZDecompressWithHeader.Text = "LZ Decompress with Hdr.";
	        this.btnLZDecompressWithHeader.UseVisualStyleBackColor = true;
	        this.btnLZDecompressWithHeader.Click += new System.EventHandler(this.btnLZDecompressWithHeader_Click);
	        // 
	        // btnReplaceRaw
	        // 
	        this.btnReplaceRaw.Location = new System.Drawing.Point(110, 2);
	        this.btnReplaceRaw.Name = "btnReplaceRaw";
	        this.btnReplaceRaw.Size = new System.Drawing.Size(101, 23);
	        this.btnReplaceRaw.TabIndex = 2;
	        this.btnReplaceRaw.Text = "Replace (Raw)";
	        this.btnReplaceRaw.UseVisualStyleBackColor = true;
	        this.btnReplaceRaw.Click += new System.EventHandler(this.btnReplaceRaw_Click);
	        // 
	        // btnOpenFile
	        // 
	        this.btnOpenFile.Enabled = false;
	        this.btnOpenFile.Location = new System.Drawing.Point(3, 26);
	        this.btnOpenFile.Name = "btnOpenFile";
	        this.btnOpenFile.Size = new System.Drawing.Size(208, 23);
	        this.btnOpenFile.TabIndex = 1;
	        this.btnOpenFile.Text = "Open file";
	        this.btnOpenFile.UseVisualStyleBackColor = true;
	        this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
	        // 
	        // btnExtractRaw
	        // 
	        this.btnExtractRaw.Location = new System.Drawing.Point(3, 2);
	        this.btnExtractRaw.Name = "btnExtractRaw";
	        this.btnExtractRaw.Size = new System.Drawing.Size(101, 23);
	        this.btnExtractRaw.TabIndex = 0;
	        this.btnExtractRaw.Text = "Extract (Raw)";
	        this.btnExtractRaw.UseVisualStyleBackColor = true;
	        this.btnExtractRaw.Click += new System.EventHandler(this.btnExtractRaw_Click);
	        // 
	        // tbpARM9Overlays
	        // 
	        this.tbpARM9Overlays.Controls.Add(this.spcARM9Overlays);
	        this.tbpARM9Overlays.Location = new System.Drawing.Point(4, 22);
	        this.tbpARM9Overlays.Name = "tbpARM9Overlays";
	        this.tbpARM9Overlays.Padding = new System.Windows.Forms.Padding(3);
	        this.tbpARM9Overlays.Size = new System.Drawing.Size(568, 392);
	        this.tbpARM9Overlays.TabIndex = 2;
	        this.tbpARM9Overlays.Text = "ARM 9 Overlays";
	        this.tbpARM9Overlays.UseVisualStyleBackColor = true;
	        // 
	        // spcARM9Overlays
	        // 
	        this.spcARM9Overlays.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.spcARM9Overlays.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
	        this.spcARM9Overlays.IsSplitterFixed = true;
	        this.spcARM9Overlays.Location = new System.Drawing.Point(3, 3);
	        this.spcARM9Overlays.Name = "spcARM9Overlays";
	        this.spcARM9Overlays.Orientation = System.Windows.Forms.Orientation.Horizontal;
	        // 
	        // spcARM9Overlays.Panel1
	        // 
	        this.spcARM9Overlays.Panel1.Controls.Add(this.tvARM9Overlays);
	        // 
	        // spcARM9Overlays.Panel2
	        // 
	        this.spcARM9Overlays.Panel2.Controls.Add(this.btnReplaceOverlay);
	        this.spcARM9Overlays.Panel2.Controls.Add(this.btnExtractOverlay);
	        this.spcARM9Overlays.Panel2.Controls.Add(this.btnDecompressOverlay);
	        this.spcARM9Overlays.Size = new System.Drawing.Size(562, 386);
	        this.spcARM9Overlays.SplitterDistance = 355;
	        this.spcARM9Overlays.TabIndex = 0;
	        // 
	        // tvARM9Overlays
	        // 
	        this.tvARM9Overlays.Dock = System.Windows.Forms.DockStyle.Fill;
	        this.tvARM9Overlays.Location = new System.Drawing.Point(0, 0);
	        this.tvARM9Overlays.Name = "tvARM9Overlays";
	        this.tvARM9Overlays.Size = new System.Drawing.Size(562, 355);
	        this.tvARM9Overlays.TabIndex = 0;
	        this.tvARM9Overlays.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvARM9Overlays_AfterSelect);
	        // 
	        // btnReplaceOverlay
	        // 
	        this.btnReplaceOverlay.Location = new System.Drawing.Point(122, 3);
	        this.btnReplaceOverlay.Name = "btnReplaceOverlay";
	        this.btnReplaceOverlay.Size = new System.Drawing.Size(113, 23);
	        this.btnReplaceOverlay.TabIndex = 11;
	        this.btnReplaceOverlay.Text = "Replace Overlay";
	        this.btnReplaceOverlay.UseVisualStyleBackColor = true;
	        this.btnReplaceOverlay.Click += new System.EventHandler(this.btnReplaceOverlay_Click);
	        // 
	        // btnExtractOverlay
	        // 
	        this.btnExtractOverlay.Location = new System.Drawing.Point(3, 2);
	        this.btnExtractOverlay.Name = "btnExtractOverlay";
	        this.btnExtractOverlay.Size = new System.Drawing.Size(113, 23);
	        this.btnExtractOverlay.TabIndex = 10;
	        this.btnExtractOverlay.Text = "Extract Overlay";
	        this.btnExtractOverlay.UseVisualStyleBackColor = true;
	        this.btnExtractOverlay.Click += new System.EventHandler(this.btnExtractOverlay_Click);
	        // 
	        // btnDecompressOverlay
	        // 
	        this.btnDecompressOverlay.Location = new System.Drawing.Point(241, 3);
	        this.btnDecompressOverlay.Name = "btnDecompressOverlay";
	        this.btnDecompressOverlay.Size = new System.Drawing.Size(137, 23);
	        this.btnDecompressOverlay.TabIndex = 9;
	        this.btnDecompressOverlay.Text = "Decompress Overlay";
	        this.btnDecompressOverlay.UseVisualStyleBackColor = true;
	        // 
	        // addons
	        // 
	        this.addons.Controls.Add(this.btnOpenAddonFolder);
	        this.addons.Controls.Add(this.btnInstall);
	        this.addons.Controls.Add(this.addonsChoice);
	        this.addons.Controls.Add(this.btn_open_github);
	        this.addons.Controls.Add(this.btnDownloadAddon);
	        this.addons.Controls.Add(this.addons_list);
	        this.addons.Controls.Add(this.btnRefreshAddons);
	        this.addons.Location = new System.Drawing.Point(4, 22);
	        this.addons.Name = "addons";
	        this.addons.Padding = new System.Windows.Forms.Padding(3);
	        this.addons.Size = new System.Drawing.Size(568, 392);
	        this.addons.TabIndex = 3;
	        this.addons.Text = "🧩 addons";
	        this.addons.UseVisualStyleBackColor = true;
	        // 
	        // btnOpenAddonFolder
	        // 
	        this.btnOpenAddonFolder.AllowDrop = true;
	        this.btnOpenAddonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
	        this.btnOpenAddonFolder.Image = global::SM64DSe.Properties.Resources.folder;
	        this.btnOpenAddonFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
	        this.btnOpenAddonFolder.Location = new System.Drawing.Point(225, 3);
	        this.btnOpenAddonFolder.Name = "btnOpenAddonFolder";
	        this.btnOpenAddonFolder.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
	        this.btnOpenAddonFolder.RightToLeft = System.Windows.Forms.RightToLeft.No;
	        this.btnOpenAddonFolder.Size = new System.Drawing.Size(137, 21);
	        this.btnOpenAddonFolder.TabIndex = 11;
	        this.btnOpenAddonFolder.Text = "Open addons folder";
	        this.btnOpenAddonFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
	        this.btnOpenAddonFolder.UseMnemonic = false;
	        this.btnOpenAddonFolder.UseVisualStyleBackColor = false;
	        this.btnOpenAddonFolder.Visible = false;
	        this.btnOpenAddonFolder.Click += new System.EventHandler(this.btnOpenAddonFolder_Click);
	        // 
	        // btnInstall
	        // 
	        this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
	        this.btnInstall.Enabled = false;
	        this.btnInstall.Location = new System.Drawing.Point(482, 366);
	        this.btnInstall.Name = "btnInstall";
	        this.btnInstall.Size = new System.Drawing.Size(86, 23);
	        this.btnInstall.TabIndex = 10;
	        this.btnInstall.Text = "Install";
	        this.btnInstall.UseVisualStyleBackColor = true;
	        this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
	        // 
	        // addonsChoice
	        // 
	        this.addonsChoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
	        this.addonsChoice.FormattingEnabled = true;
	        this.addonsChoice.ImeMode = System.Windows.Forms.ImeMode.NoControl;
	        this.addonsChoice.Items.AddRange(new object[] { "Online", "Local" });
	        this.addonsChoice.Location = new System.Drawing.Point(449, 3);
	        this.addonsChoice.Name = "addonsChoice";
	        this.addonsChoice.Size = new System.Drawing.Size(116, 21);
	        this.addonsChoice.TabIndex = 9;
	        this.addonsChoice.SelectionChangeCommitted += new System.EventHandler(this.AddonsChoice_SelectionChangeCommitted);
	        // 
	        // btn_open_github
	        // 
	        this.btn_open_github.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
	        this.btn_open_github.Enabled = false;
	        this.btn_open_github.Image = global::SM64DSe.Properties.Resources.github;
	        this.btn_open_github.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
	        this.btn_open_github.Location = new System.Drawing.Point(92, 366);
	        this.btn_open_github.Name = "btn_open_github";
	        this.btn_open_github.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
	        this.btn_open_github.Size = new System.Drawing.Size(112, 23);
	        this.btn_open_github.TabIndex = 8;
	        this.btn_open_github.Text = "Open in GitHub";
	        this.btn_open_github.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
	        this.btn_open_github.UseMnemonic = false;
	        this.btn_open_github.UseVisualStyleBackColor = false;
	        this.btn_open_github.Click += new System.EventHandler(this.btn_open_github_Click);
	        // 
	        // btn_list_versions
	        // 
	        this.btnDownloadAddon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
	        this.btnDownloadAddon.Enabled = false;
	        this.btnDownloadAddon.Location = new System.Drawing.Point(0, 366);
	        this.btnDownloadAddon.Name = "btnDownloadAddon";
	        this.btnDownloadAddon.Size = new System.Drawing.Size(86, 23);
	        this.btnDownloadAddon.TabIndex = 7;
	        this.btnDownloadAddon.Text = "Download";
	        this.btnDownloadAddon.UseVisualStyleBackColor = true;
	        this.btnDownloadAddon.Click += new System.EventHandler(this.btnDownloadAddon_Click);
	        // 
	        // addons_list
	        // 
	        this.addons_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
	        this.addons_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.columnHeader1, this.columnHeader2 });
	        this.addons_list.HideSelection = false;
	        this.addons_list.LargeImageList = this.addons_image_list;
	        this.addons_list.Location = new System.Drawing.Point(0, 27);
	        this.addons_list.Margin = new System.Windows.Forms.Padding(0);
	        this.addons_list.MultiSelect = false;
	        this.addons_list.Name = "addons_list";
	        this.addons_list.Size = new System.Drawing.Size(565, 336);
	        this.addons_list.SmallImageList = this.addons_image_list;
	        this.addons_list.TabIndex = 6;
	        this.addons_list.UseCompatibleStateImageBehavior = false;
	        this.addons_list.View = System.Windows.Forms.View.Tile;
	        this.addons_list.SelectedIndexChanged += new System.EventHandler(this.addonsList_SelectedIndexChanged);
	        this.addons_list.Resize += new System.EventHandler(this.addonsList_Resize);
	        // 
	        // columnHeader1
	        // 
	        this.columnHeader1.Text = "Name";
	        // 
	        // columnHeader2
	        // 
	        this.columnHeader2.Text = "details";
	        // 
	        // addons_image_list
	        // 
	        this.addons_image_list.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
	        this.addons_image_list.ImageSize = new System.Drawing.Size(16, 16);
	        this.addons_image_list.TransparentColor = System.Drawing.Color.Transparent;
	        // 
	        // btnRefreshAddons
	        // 
	        this.btnRefreshAddons.AllowDrop = true;
	        this.btnRefreshAddons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
	        this.btnRefreshAddons.Location = new System.Drawing.Point(368, 3);
	        this.btnRefreshAddons.Name = "btnRefreshAddons";
	        this.btnRefreshAddons.Size = new System.Drawing.Size(75, 21);
	        this.btnRefreshAddons.TabIndex = 5;
	        this.btnRefreshAddons.Text = "Refresh";
	        this.btnRefreshAddons.UseVisualStyleBackColor = true;
	        this.btnRefreshAddons.Click += new System.EventHandler(this.btnRefreshAddons_Click);
	        // 
	        // MainForm
	        // 
	        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
	        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	        this.ClientSize = new System.Drawing.Size(576, 465);
	        this.Controls.Add(this.tbcMainFormTabControl);
	        this.Controls.Add(this.ssStatusBar);
	        this.Controls.Add(this.tsToolBar);
	        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
	        this.Name = "MainForm";
	        this.Text = "SM64DS Editor vlol";
	        this.Shown += new System.EventHandler(this.MainForm_Shown);
	        this.tsToolBar.ResumeLayout(false);
	        this.tsToolBar.PerformLayout();
	        this.ssStatusBar.ResumeLayout(false);
	        this.ssStatusBar.PerformLayout();
	        this.tbcMainFormTabControl.ResumeLayout(false);
	        this.tbpLevels.ResumeLayout(false);
	        this.splitContainerLevels.Panel1.ResumeLayout(false);
	        this.splitContainerLevels.Panel2.ResumeLayout(false);
	        ((System.ComponentModel.ISupportInitialize)(this.splitContainerLevels)).EndInit();
	        this.splitContainerLevels.ResumeLayout(false);
	        this.tbpFileSystem.ResumeLayout(false);
	        this.spcFileSystemTab.Panel1.ResumeLayout(false);
	        this.spcFileSystemTab.Panel2.ResumeLayout(false);
	        ((System.ComponentModel.ISupportInitialize)(this.spcFileSystemTab)).EndInit();
	        this.spcFileSystemTab.ResumeLayout(false);
	        this.pnlFileOptions.ResumeLayout(false);
	        this.tbpARM9Overlays.ResumeLayout(false);
	        this.spcARM9Overlays.Panel1.ResumeLayout(false);
	        this.spcARM9Overlays.Panel2.ResumeLayout(false);
	        ((System.ComponentModel.ISupportInitialize)(this.spcARM9Overlays)).EndInit();
	        this.spcARM9Overlays.ResumeLayout(false);
	        this.addons.ResumeLayout(false);
	        this.ResumeLayout(false);
	        this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripStatusLabel updateLabel;

        private System.Windows.Forms.Button btnOpenAddonFolder;

        private System.Windows.Forms.Button btnInstall;

        private System.Windows.Forms.ComboBox addonsChoice;

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;

        private System.Windows.Forms.Button btn_open_github;

        private System.Windows.Forms.Button btnDownloadAddon;

        private System.Windows.Forms.ImageList addons_image_list;

        private System.Windows.Forms.ListView addons_list;

        private System.Windows.Forms.Button btnRefreshAddons;

        private System.Windows.Forms.TabPage addons;

        private System.Windows.Forms.TabPage tabPage1;

        #endregion

        private System.Windows.Forms.ToolStrip tsToolBar;
        private System.Windows.Forms.ToolStripButton btnOpenROM;
        private System.Windows.Forms.OpenFileDialog ofdOpenFile;
        private System.Windows.Forms.SaveFileDialog sfdSaveFile;
        private System.Windows.Forms.ToolStripDropDownButton btnMore;
        private System.Windows.Forms.StatusStrip ssStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel slStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar spbStatusProgress;
        private System.Windows.Forms.ToolStripDropDownButton btnOptions;
        private System.Windows.Forms.ToolStripMenuItem btnUpdateODB;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripButton btnHalp;
        private System.Windows.Forms.ToolStripMenuItem mnitDumpAllOvls;
        private System.Windows.Forms.ToolStripMenuItem mnitDecompressOverlaysWithinGame;
        private System.Windows.Forms.ToolStripMenuItem mnitHexDumpToBinaryFile;
        private System.Windows.Forms.ToolStripMenuItem mnitAdditionalPatches;
        private System.Windows.Forms.TabControl tbcMainFormTabControl;
        private System.Windows.Forms.TabPage tbpLevels;
        private System.Windows.Forms.TabPage tbpFileSystem;
        private System.Windows.Forms.Panel pnlFileOptions;
        private System.Windows.Forms.TreeView tvFileList;
        private System.Windows.Forms.Button btnReplaceRaw;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnExtractRaw;
        private System.Windows.Forms.Button btnLZForceCompression;
        private System.Windows.Forms.Button btnLZCompressWithHeader;
        private System.Windows.Forms.Button btnLZForceDecompression;
        private System.Windows.Forms.Button btnLZDecompressWithHeader;
        private System.Windows.Forms.SplitContainer spcFileSystemTab;
        private System.Windows.Forms.ToolStripMenuItem mnitEditSDATINFOBlockToolStripMenuItem;
        private System.Windows.Forms.TabPage tbpARM9Overlays;
        private System.Windows.Forms.SplitContainer spcARM9Overlays;
        private System.Windows.Forms.Button btnReplaceOverlay;
        private System.Windows.Forms.Button btnExtractOverlay;
        private System.Windows.Forms.Button btnDecompressOverlay;
        private System.Windows.Forms.TreeView tvARM9Overlays;
        private System.Windows.Forms.ToolStripMenuItem mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerLevels;
        private System.Windows.Forms.Button btnEditCollisionMap;
        private System.Windows.Forms.Button btnEditLevel;
        private System.Windows.Forms.Button btnEditLevelNames;
        private System.Windows.Forms.ToolStripDropDownButton btnTools;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsModelAndCollisionMapImporter;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsModelAnimationEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsImageEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsTextEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsCollisionMapEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsBTPEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitToolsSoundBrowser;
        private System.Windows.Forms.ToolStripDropDownButton btnASMHacking;
        private System.Windows.Forms.ToolStripMenuItem mnitASMHackingCompilation;
        private System.Windows.Forms.ToolStripMenuItem mnitASMHackingCompilationCodeCompiler;
        private System.Windows.Forms.ToolStripMenuItem mnitASMHackingCompilationFixCodeOffsets;
        private System.Windows.Forms.ToolStripMenuItem mnitASMHackingGeneration;
        private System.Windows.Forms.ToolStripMenuItem platformEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator tssASMHacking001;
        private System.Windows.Forms.ToolStripMenuItem bMDKLCEditorToolStripMenuItem;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbLevelListDisplay;
        private System.Windows.Forms.ListBox lbxLevels;
        private System.Windows.Forms.ToolStripMenuItem dLPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kuppaScriptEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFileSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editOverlaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixMultiplayerChecksToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnOpenFilesystem;
        private System.Windows.Forms.ToolStripButton btnBuildROM;
        private System.Windows.Forms.ToolStripButton btnRunROM;
        private System.Windows.Forms.ToolStripButton extractROMButton;
        private System.Windows.Forms.ToolStripMenuItem particleTextureSPTEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem particleArchiveSPAEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileHeaderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem soundHeaderToolStripMenuItem;
	}
}
