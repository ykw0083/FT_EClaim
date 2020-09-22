using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.StateMachine.NonPersistent;
using DevExpress.ExpressApp.StateMachine;

namespace FT_EClaim.Module.BusinessObjects
{
    public class ClaimTrxsStateMachine : StateMachine<ClaimTrxs>
    {
        private IState startState;
        public ClaimTrxsStateMachine(IObjectSpace objectSpace) : base(objectSpace)
        {
            startState = new DevExpress.ExpressApp.StateMachine.NonPersistent.State(this, "Not Applicable", ApprovalStatuses.Not_Applicable);

            IState Required_Approval = new DevExpress.ExpressApp.StateMachine.NonPersistent.State(this, "Required Approval", ApprovalStatuses.Required_Approval);
            IState Approved = new DevExpress.ExpressApp.StateMachine.NonPersistent.State(this, "Approved", ApprovalStatuses.Approved);
            IState Rejected = new DevExpress.ExpressApp.StateMachine.NonPersistent.State(this, "Rejected", ApprovalStatuses.Rejected);

            //startState.Transitions.Add(new Transition(Required_Approval));
            Required_Approval.Transitions.Add(new Transition(Required_Approval));
            Required_Approval.Transitions.Add(new Transition(Rejected));
            Required_Approval.Transitions.Add(new Transition(Approved));

            //States.Add(startState);
            States.Add(Required_Approval);
            States.Add(Approved);
            States.Add(Rejected);

            StateAppearance NAAppearance = new StateAppearance(startState);
            NAAppearance.TargetItems = "ApprovalStatus";
            //NAAppearance.Enabled = false;
            NAAppearance.FontColor = System.Drawing.Color.Red;
            StateAppearance RequiredAppearance = new StateAppearance(Required_Approval);
            RequiredAppearance.TargetItems = "ApprovalStatus";
            RequiredAppearance.FontColor = System.Drawing.Color.Red;
            StateAppearance completedAppearance = new StateAppearance(Approved);
            completedAppearance.TargetItems = "*";
            completedAppearance.FontColor = System.Drawing.Color.Green;
            StateAppearance RejectedAppearance = new StateAppearance(Rejected);
            RejectedAppearance.TargetItems = "*";
            RejectedAppearance.FontColor = System.Drawing.Color.Red;

        }
        public override IState StartState
        {
            get { return startState; }
        }
        public override string Name
        {
            get { return "Change status to"; }
        }
        public override string StatePropertyName
        {
            get { return "ApprovalStatus"; }
        }
    }
}