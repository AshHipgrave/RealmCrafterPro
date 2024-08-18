//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class ScriptForm : ScriptEditorForm
    {
        public string loadedFileName;
        public bool DontSave;
        public bool saved = true;
        public bool CanBeSaved = false;
        public int defaultZoom;
        
        private System.Text.Encoding enc = System.Text.Encoding.UTF8;
        private string Extension = Program.ScriptExtension;

        private String ReservedWords =
            "Each Else ElseIf End EndIf Error For Function If Include Is Loop New Next Set Switch Step Then Until Using Using Wend While";

        private String FunctionNames =
            "AbilityKnown AbilityLevel AbilityMemorised Abs ACos Actor ActorAmulet ActorAggressiveness ActorAIState ActorBackpack ActorBeard ActorBelt ActorCallForHelp ActorChest ActorClothes ActorDestinationX ActorDestinationZ ActorDistance ActorFace ActorFeet ActorGender ActorGlobal ActorGroup ActorHair ActorHands ActorHasEffect ActorHat ActorID ActorIDFromInstance ActorInTrigger ActorIsHuman ActorLeader ActorLegs ActorLevel ActorMount ActorOutdoors ActorPets ActorRider ActorRing ActorsInZone ActorShield ActorTarget ActorUnderWater ActorWeapon ActorX ActorXP ActorXPMultiplier ActorY ActorZ ActorZone AddAbility AddActorEffect AnimateActor Asc ASin ATan ATan2 Attribute BankSize BubbleOutput CallDLL ChangeActor ChangeFactionRating ChangeGold ChangeMoney Chr Class CloseDialog CloseFile CloseUDPStream CompleteQuest ContextActor CopyBank Cos CountHostIPs CountPartyMembers CreateBank CreateEmitter CreateFloatingNumber CreateProgressBar CreateUDPStream CreateZoneInstance Day DefaultFactionRating DeleteAbility DeleteActorEffect DeleteFile DeleteProgressBar DeleteQuest DialogInput DialogOutput DoEvents DottedIP End EoF Exp FactionRating FilePos FileSize FileType FindActor FireProjectile FirstActorInZone FreeBank FullTrim GiveItem GiveXP GiveKillXP Global Gold GoTo HasItem HomeFaction HostIP Hour Instr Int ItemArmor ItemAttribute ItemDamage ItemDamageType ItemHealth ItemMass ItemMiscData ItemName ItemRange ItemValue Left Len Log Log10 Lower KillActor MaxAttribute Mid MilliSecs Minute Money Month MoveActor MySQLClean MySQLFetchRow MySQLGetVar MySQLNumRows MySQLQuery SQLAccountID SQLActorID Name NewQuest NextActor NextActorInZone OpenDialog OpenFile OpenTrading Output Parameter PartyMember PeekByte PeekFloat PeekInt PeekShort Persistent Pi PlayerAccountEmail PlayerAccountName PlayerIsBanned PlayerIsDM PlayerIsGM PlayerInGame PlayersInZone PlayMusic PlaySound PlaySpeech PokeByte PokeFloat PokeInt PokeShort QuestComplete QuestStatus Race Rand ReadByte ReadFile ReadFloat ReadInt ReadLine ReadShort ReadString RealDate RealTime RecvUDPMsg Replace Reputation Resistance ResizeBank Return RemoveZoneInstance Right RotateActor Rnd RuntimeError SaveState SceneryOwner ScreenFlash ScriptingCommandIndex ScriptLog Season SeekFile SendUDPMsg SetAbilityLevel SetActorAIState SetActorBeard SetActorClothes SetActorDestination SetActorHair SetActorFace SetActorGender SetActorGlobal SetActorGroup SetActorLevel SetActorTarget SetAttribute SetFactionRating SetGlobal SetGold SetHomeFaction SetItemHealth SetLeader SetMaxAttribute SetMoney SetName SetOwner SetReputation SetResistance SetSuperGlobal SetTag Sign Sin Spawn Sqrt SuperGlobal SpawnItem Tag Tan ThreadExecute Trim UDPMsgIP UDPMsgPort UDPStreamIP UDPStreamPort UDPTimeouts UpdateQuest UpdateProgressBar UpdateXPBar Upper WaitItem WaitKill WaitSpeak WaitTime Warp WriteByte WriteFile WriteFloat WriteInt WriteLine WriteShort WriteString Year ZoneInstanceExists ZoneOutdoors";

        private List<string> AutoComplete = new List<string>();

        private string ACNames =
            "AbilityKnown AbilityLevel AbilityMemorised Abs ACos Actor ActorAmulet ActorAggressiveness ActorAIState ActorBackpack ActorBeard ActorBelt ActorCallForHelp ActorChest ActorClothes ActorDestinationX ActorDestinationZ ActorDistance ActorFace ActorFeet ActorGender ActorGlobal ActorGroup ActorHair ActorHands ActorHasEffect ActorHat ActorID ActorIDFromInstance ActorInTrigger ActorIsHuman ActorLeader ActorLegs ActorLevel ActorMount ActorOutdoors ActorPets ActorRider ActorRing ActorsInZone ActorShield ActorTarget ActorUnderWater ActorWeapon ActorX ActorXP ActorXPMultiplier ActorY ActorZ ActorZone AddAbility AddActorEffect AnimateActor Asc ASin ATan ATan2 Attribute BankSize BubbleOutput CallDLL ChangeActor ChangeFactionRating ChangeGold ChangeMoney Chr Class CloseDialog CloseFile CloseUDPStream CompleteQuest ContextActor CopyBank Cos CountHostIPs CountPartyMembers CreateBank CreateEmitter CreateFloatingNumber CreateProgressBar CreateUDPStream CreateZoneInstance Day DefaultFactionRating DeleteAbility DeleteActorEffect DeleteFile DeleteProgressBar DeleteQuest DialogInput DialogOutput DoEvents DottedIP End EoF Exp FactionRating FilePos FileSize FileType FindActor FireProjectile FirstActorInZone FreeBank FullTrim GiveItem GiveXP GiveKillXP Global Gold GoTo HasItem HomeFaction HostIP Hour Instr Int ItemArmor ItemAttribute ItemDamage ItemDamageType ItemHealth ItemMass ItemMiscData ItemName ItemRange ItemValue Left Len Log Log10 Lower KillActor MaxAttribute Mid MilliSecs Minute Money Month MoveActor MySQLClean MySQLFetchRow MySQLGetVar MySQLNumRows MySQLQuery SQLAccountID SQLActorID Name NewQuest NextActor NextActorInZone OpenDialog OpenFile OpenTrading Output Parameter PartyMember PeekByte PeekFloat PeekInt PeekShort Persistent Pi PlayerAccountEmail PlayerAccountName PlayerIsBanned PlayerIsDM PlayerIsGM PlayerInGame PlayersInZone PlayMusic PlaySound PlaySpeech PokeByte PokeFloat PokeInt PokeShort QuestComplete QuestStatus Race Rand ReadByte ReadFile ReadFloat ReadInt ReadLine ReadShort ReadString RealDate RealTime RecvUDPMsg Replace Reputation Resistance ResizeBank Return RemoveZoneInstance Right RotateActor Rnd RuntimeError SaveState SceneryOwner ScreenFlash ScriptingCommandIndex ScriptLog Season SeekFile SendUDPMsg SetAbilityLevel SetActorAIState SetActorBeard SetActorClothes SetActorDestination SetActorHair SetActorFace SetActorGender SetActorGlobal SetActorGroup SetActorLevel SetActorTarget SetAttribute SetFactionRating SetGlobal SetGold SetHomeFaction SetItemHealth SetLeader SetMaxAttribute SetMoney SetName SetOwner SetReputation SetResistance SetSuperGlobal SetTag Sign Sin Spawn Sqrt SuperGlobal SpawnItem Tag Tan ThreadExecute Trim UDPMsgIP UDPMsgPort UDPStreamIP UDPStreamPort UDPTimeouts UpdateQuest UpdateProgressBar UpdateXPBar Upper WaitItem WaitKill WaitSpeak WaitTime Warp WriteByte WriteFile WriteFloat WriteInt WriteLine WriteShort WriteString Year ZoneInstanceExists ZoneOutdoors>";

        private string CustomFunctions =  " ";
        private INativeScintilla native; //= ScriptText.NativeInterface;


        public override ScintillaNet.Scintilla ScriptText
        {
            get { return scriptText; }
            set { scriptText = value; }
        }

        public override int DefaultZoom
        {
            get { return defaultZoom; }
            set { defaultZoom = value; }
        }

        public override string LoadedFileName
        {
            get { return loadedFileName; }
            set { loadedFileName = value; }
        }

        public override bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }

        public ScriptForm()
        {
            InitializeComponent();
            ScriptText.ConfigurationManager.Language = "vbscript";
            native = ScriptText.NativeInterface;
        }

        public void UseBVMAlt()
        {
            ScriptText.ConfigurationManager.Language = "vbscript";
            ScriptText.AutoComplete.RegisterImages(imageList1, Color.Black);
            ScriptText.AutoComplete.ImageSeparator = '>';
            ScriptText.AutoComplete.List = AutoComplete;
            ScriptText.Margins[2].Width = 16;
            ScriptText.Lexing.LineCommentPrefix = "'";
//             native.SetKeywords(0, ReservedWords.ToLower());
//             native.SetKeywords(1,FunctionNames.ToLower() + " " + CustomFunctions.ToLower() );
//             native.SetKeywords(3, "altsyntax compile");
            //ScriptText.Lexing.Lexer = ScintillaNet.Lexer.BlitzBasic;
            ScriptText.Lexing.Colorize();
        }
        public void UseBVM()
        {
            ScriptText.ConfigurationManager.Language = "vbscript";
            ScriptText.AutoComplete.RegisterImages(imageList1, Color.Black);
            ScriptText.AutoComplete.ImageSeparator = '>';
            ScriptText.AutoComplete.List = AutoComplete;
            ScriptText.Margins[2].Width = 16;
            ScriptText.Lexing.LineCommentPrefix = ";";
            FunctionNames = FunctionNames.Replace("End", "");
//             native.SetKeywords(0, ReservedWords.ToLower());
// 
//             native.SetKeywords(1, FunctionNames.ToLower() + " " + CustomFunctions.ToLower());
//             native.SetKeywords(3, "altsyntax compile");
            ScriptText.Lexing.Lexer = ScintillaNet.Lexer.BlitzBasic;
            ScriptText.Lexing.Colorize();
        }

        public void UseRCScript()
        {
            ScriptText.ConfigurationManager.Language = "cs";
            ScriptText.AutoComplete.RegisterImages(imageList1, Color.Black);
            ScriptText.AutoComplete.ImageSeparator = '>';
            ScriptText.AutoComplete.List = AutoComplete;
            ScriptText.Margins[2].Width = 0;
            ScriptText.Lexing.LineCommentPrefix = "//";
            //native.SetKeywords(0, ReservedWords + " " + ReservedWords.ToLower() + " " + ReservedWords.ToUpper());
            //             native.SetKeywords(1,
            //                    FunctionNames + " " + FunctionNames.ToLower() + " " + FunctionNames.ToUpper() + " " +
            //                    CustomFunctions + " " + CustomFunctions.ToLower() + " " + CustomFunctions.ToUpper());
            native.SetKeywords(1,
                   FunctionNames + " " + 
                   CustomFunctions);
            ScriptText.Lexing.Colorize();

        }

        public String GetCustomCommands()
        {
            int restore = ScriptText.Caret.Position;
            string NewLine, Custom = " ";
            int Pos, Count = 0;
            ScriptText.Caret.Position = 0;
            NewLine = ScriptText.Lines.Current.Text;

            while (ScriptText.Caret.LineNumber != ScriptText.Lines.Count - 1)
            {
                Pos = NewLine.ToUpper().IndexOf("FUNCTION");
                if (Pos >= 0)
                {
                    Pos = NewLine.IndexOf("(", Pos);
                    if (Pos > 0)
                    {
                        int CommentPos = NewLine.ToUpper().IndexOf("//");
                        if (!(CommentPos < Pos))
                        {
                            Custom += NewLine.Substring(0, Pos).Substring(9).Trim() + " ";
                            ++Count;
                        }
                        else if (CommentPos == -1)
                        {
                            Custom += NewLine.Substring(0, Pos).Substring(9).Trim() + " ";
                            ++Count;
                        }
                    }
                }
                ScriptText.Caret.LineNumber += 1;
                NewLine = ScriptText.Lines.Current.Text;
            }

            ScriptText.Caret.Position = restore;
            ScriptText.Selection.Length = 0;
            return Custom;
        }
        private void ReadFunctionNames(string FileName)
        {
            List<string> TempFunctions = new List<string>();
            string line;
            if (File.Exists(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName))
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName);
                while ((line = file.ReadLine()) != null)
                {
                    TempFunctions.Add(line.Trim());
                }
                FunctionNames = "";
                for (int i = 0; i < TempFunctions.Count; i++)
                {
                    FunctionNames += TempFunctions[i] + " ";
                }
                native.SetKeywords(0, ReservedWords + " " + ReservedWords.ToLower() + " " + ReservedWords.ToUpper());
                native.SetKeywords(1, FunctionNames + " " + FunctionNames.ToLower() + " " + FunctionNames.ToUpper());
            }
        }

        private void ReadAutoComplete(string FileName)
        {
            List<string> TempFunctions = new List<string>();
            string line;
            if (File.Exists(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName))
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName);
                while ((line = file.ReadLine()) != null)
                {
                    TempFunctions.Add(line.Trim());
                }
                ACNames = "";
                for (int i = 0; i < TempFunctions.Count; i++)
                {
                    ACNames += TempFunctions[i] + " ";
                }
                ACNames = ACNames.Replace(" ", "> ");
                AutoComplete[0] = ACNames;
                ScriptText.AutoComplete.RegisterImages(imageList1, Color.Black);
                ScriptText.AutoComplete.ImageSeparator = '>';
                ScriptText.AutoComplete.List = AutoComplete;
            }
        }

        private void ReadKeyWords(string FileName)
        {
            List<string> TempFunctions = new List<string>();
            string line;
            if (File.Exists(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName))
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(Program.TestingVersion ? @".\GUE" + "\\" + FileName : @"..\..\Data\GUE" + "\\" + FileName);
                while ((line = file.ReadLine()) != null)
                {
                    TempFunctions.Add(line.Trim());
                }
                ReservedWords = "";
                for (int i = 0; i < TempFunctions.Count; i++)
                {
                    ReservedWords += TempFunctions[i] + " ";
                }
                native.SetKeywords(0, ReservedWords + " " + ReservedWords.ToLower() + " " + ReservedWords.ToUpper());
                native.SetKeywords(1, FunctionNames + " " + FunctionNames.ToLower() + " " + FunctionNames.ToUpper());
            }
        }
        
        private void ScriptForm_Load(object sender, EventArgs e)
        {
            ACNames = ACNames.Replace(" ", "> ");
            AutoComplete.Add(ACNames);
            ScriptText.AutoComplete.RegisterImages(imageList1, Color.Black);
            ScriptText.AutoComplete.ImageSeparator = '>';
            ScriptText.AutoComplete.List = AutoComplete;
            native.AutoCSetIgnoreCase(true);
            DefaultZoom = ScriptText.Zoom;
            ScriptText.Lexing.LineCommentPrefix = ";";
            ScriptText.Indentation.SmartIndentType = SmartIndent.CPP;
            ReadFunctionNames("RCFunctions.txt");
            ReadAutoComplete("RCAutoComplete.txt");
            ReadKeyWords("RCReserved.txt");
            ScriptText.Lexing.StreamCommentPrefix = ";";
            switch (Program.GE.m_ScriptView.SyntaxHighlighting)
            {
                case SimpleScriptEditor.SyntaxStyles.RCSCRIPT:
                    UseRCScript();
                    break;
                case SimpleScriptEditor.SyntaxStyles.BVM:
                    UseBVM();
                    break;
                case SimpleScriptEditor.SyntaxStyles.BVMALT:
                    UseBVMAlt();
                    break;
            }
        }

        public override void SetSaved(bool State)
        {
            if (Saved == State)
            {
                return;
            }
            if (State == false)
            {
                this.TabText = LoadedFileName + " *";
                Saved = false;
            }
            else
            {
                this.TabText = LoadedFileName;
                Saved = true;
            }
        }

        public bool LoadFile(string FileName)
        {
            string file = @"Data\Server Data\Scripts\" + FileName;
            if (File.Exists(file))
            {
                ScriptText.Text = File.ReadAllText(file, enc );
            }
            else
            {
                MessageBox.Show("Error loading: " + file);
                this.Dispose();
                return false;
            }
            ScriptText.UndoRedo.EmptyUndoBuffer();
            LoadedFileName = FileName;
            TabText = FileName;
            CanBeSaved = true;
            this.ToolTipText = @"Data\Server Data\Scripts\" + LoadedFileName;
            SetSaved(true);
            if (ScriptText.Lines.Count < 1000)
            {
                ColourCustomFunctions();
            }
            return true;
        }

        public override void SaveFile()
        {
            string file = @"Data\Server Data\Scripts\" + LoadedFileName;
            try
            {
                using (FileStream fs = File.Create(file))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(ScriptText.RawText, 0, ScriptText.RawText.Length - 1); // Omit trailing NULL
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error writing to:" + file);
            }
            if (ScriptText.Lines.Count < 1000)
            {
                ColourCustomFunctions();
            }
            SetSaved(true);
        }

        public void SaveFileName(string FileName)
        {
        }

        public void SaveFileAbsolute(string Directory, string FileName)
        {
        }

        private void ScriptText_TextInserted(object sender, TextModifiedEventArgs e)
        {
            SetSaved(false);
        }

        private void ScriptText_TextDeleted(object sender, TextModifiedEventArgs e)
        {
            SetSaved(false);
        }

        private void ScriptForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Saved == false)
            {
                DialogResult Result = MessageBox.Show("You will lose unsaved changes to this script, continue?",
                                                      "Continue without saving", MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);
                if (Result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ScriptText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Alt)
            {
                CustomFunctions = GetCustomCommands();
                native.SetKeywords(1,
                                   FunctionNames + " " + FunctionNames.ToLower() + " " + FunctionNames.ToUpper() + " " +
                                   CustomFunctions + " " + CustomFunctions.ToLower() + " " + CustomFunctions.ToUpper());
                ScriptText.Lexing.Colorize();
                //MessageBox.Show(GetCustomCommands());
            }
            if (e.KeyCode == Keys.S && e.Control)
            {
                SaveFile();
            }
            if (e.KeyCode == Keys.S && e.Control && e.Shift)
            {
                Program.GE.m_ScriptView.SaveAllDocuments();
            }
            if (e.KeyCode == Keys.Oemplus && e.Control)
            {
                ScriptText.Zoom = ScriptText.Zoom + 1;
            }
            if (e.KeyCode == Keys.OemMinus && e.Control)
            {
                ScriptText.Zoom = ScriptText.Zoom - 1;
            }
        }

        private void ColourCustomFunctions()
        {
            CustomFunctions = GetCustomCommands();
            native.SetKeywords(0, ReservedWords + " " + ReservedWords.ToLower() + " " + ReservedWords.ToUpper());
            native.SetKeywords(1,
                               FunctionNames + " " + FunctionNames.ToLower() + " " + FunctionNames.ToUpper() + " " +
                               CustomFunctions + " " + CustomFunctions.ToLower() + " " + CustomFunctions.ToUpper());
            ScriptText.Lexing.Colorize();
        }

        private void ScriptText_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void ScriptText_SelectionChanged(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.SetCursorPosition();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.SaveAllDocuments();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Close();
            Program.GE.m_ScriptView.CloseDocument(this);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.CloseAllDocuments();
        }

        private void closeAllButThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.CloseAllButThis(this);
        }

        private void openScriptsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"Data\Server Data\Scripts\");
        }

        public void Dispose()
        {

        }
    }
}