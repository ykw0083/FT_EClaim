using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_EClaim.Module.BusinessObjects
{
    //public enum DocumentStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    CLOSE = 3
    //}

    //public enum MaintenanceStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    WIP = 2,
    //    CLOSE = 3
    //}

    //public enum Region
    //{
    //    Local = 0,
    //    Oversea = 1
    //}
    public enum CreditLimitTypes
    {
        Monthly = 0,
        PerDoc = 1
    }
    public enum CreditLimitBys
    {
        Employee = 0,
        Position = 1
    }
    public enum ApprovalBys
    {
        [XafDisplayName("Person")]
        User = 0,
        Position = 1
    }
    public enum PostToDocuments
    {
        JE = 0,
        APINV = 1
    }
    public enum BudgetTypes
    {
        Period = 0,
        Document = 1
    }
    public enum DocumentStatus
    {
        Create = 0,
        Submit = 1,
        DocPassed = 2,
        Accepted = 3,
        Cancelled = 4,
        Rejected = 5,
        Closed = 6,
        Reopen = 7,
        Posted = 8

    }

    public enum EClaimSAPDocs
    {
        Draft = 0,
        Document = 1
    }

    public enum ApprovalStatuses
    {
        Not_Applicable = 0,
        Approved = 1,
        Required_Approval = 2,
        Rejected = 3
        //Draft, NotStarted, InProgress, Paused, Completed, Dropped
    }

    public enum ApprovalActions
    {
        NA = 0,
        Yes = 1,
        No = 2
        //Draft, NotStarted, InProgress, Paused, Completed, Dropped
    }
    public enum ApprovalTypes
    {
        Budget = 0,
        Document = 1,
        SQL = 2
    }
}
