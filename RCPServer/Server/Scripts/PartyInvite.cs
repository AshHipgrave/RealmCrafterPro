using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Enter description here
    /// </summary>
    public partial class PartyInvite : Form
    {
        Actor Invitee;

        public PartyInvite(Actor invitee)
        {
            InitializeComponent();
            Invitee = invitee;

            AcceptButton.Click += new Scripting.Forms.EventHandler(Accept);
            PlayerName.Name = invitee.Name;
        }

        private void Accept(object o, FormEventArgs e)
        {
            PartyInstance party = Invitee.GetCurrentParty();
            if (party == null)
            {
                actor.Output("Party no longer exists.");
                return;
            }

            actor.JoinParty(party);

            List<Actor> members = party.GetCurrentMembers();
            foreach (Actor a in members)
            {
                a.Output(actor.Name + " has joined the party.");
            }

            actor.CloseDialog(this);
        }

        private void Decline()
        {
            actor.CloseDialog(this);

            Invitee.Output(actor.Name + " declined invite.");
        }
        
    }
}
