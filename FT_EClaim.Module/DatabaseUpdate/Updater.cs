using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}

            string temp = "";
            string tempname = "";
            temp = GeneralSettings.hq;
            tempname = GeneralSettings.hq;
            Companies company = ObjectSpace.FindObject<Companies>(new BinaryOperator("BoCode", temp));
            if (company == null)
            {
                company = ObjectSpace.CreateObject<Companies>();
                company.BoCode = temp;
                company.BoName = tempname;
            }

            SystemUsers sampleUser = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "User"));
            if(sampleUser == null) {
                sampleUser = ObjectSpace.CreateObject<SystemUsers>();
                sampleUser.UserName = "User";
                sampleUser.Company = sampleUser.Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq));
                sampleUser.SetPassword("");
            }
            PermissionPolicyRole defaultRole = CreateDefaultRole();
            sampleUser.Roles.Add(defaultRole);

            SystemUsers userAdmin = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "Admin"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<SystemUsers>();
                userAdmin.UserName = "Admin";
                userAdmin.Company = userAdmin.Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq));
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
			// If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "SA"));
            if(adminRole == null) {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "SA";
            }
            adminRole.IsAdministrative = true;
			userAdmin.Roles.Add(adminRole);

            temp = GeneralSettings.claimrole;
            PermissionPolicyRole claimRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (claimRole == null)
            {
                claimRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                claimRole.Name = temp;
            }
            temp = GeneralSettings.claimsuperrole;
            PermissionPolicyRole claimsuperRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (claimsuperRole == null)
            {
                claimsuperRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                claimsuperRole.Name = temp;
            }
            temp = GeneralSettings.verifyrole;
            PermissionPolicyRole verifyRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (verifyRole == null)
            {
                verifyRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                verifyRole.Name = temp;
            }
            temp = GeneralSettings.Acceptancerole;
            PermissionPolicyRole AcceptanceRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (AcceptanceRole == null)
            {
                AcceptanceRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                AcceptanceRole.Name = temp;
            }
            temp = GeneralSettings.postrole;
            PermissionPolicyRole postRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (postRole == null)
            {
                postRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                postRole.Name = temp;
            }
            temp = GeneralSettings.ApprovalRole;
            PermissionPolicyRole approveRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (approveRole == null)
            {
                approveRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                approveRole.Name = temp;
            }
            temp = GeneralSettings.RejectApproveRole;
            PermissionPolicyRole RejectApproveRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (RejectApproveRole == null)
            {
                RejectApproveRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                RejectApproveRole.Name = temp;
            }

            temp = GeneralSettings.defclaimdoc;
            tempname = "Normal Claim";
            DocTypes doctype = ObjectSpace.FindObject<DocTypes>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocTypes>();
                doctype.BoCode = temp;
                doctype.BoName = tempname;
            }

            ObjectSpace.CommitChanges(); //This line persists created object(s).


            temp = GeneralSettings.defaulttax;
            tempname = GeneralSettings.defaulttax;
            Taxes tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", temp));
            if (tax == null)
            {
                tax = ObjectSpace.CreateObject<Taxes>();
                tax.BoCode = temp;
                tax.BoName = tempname;
            }
            if (GeneralSettings.defaultmileagetax != GeneralSettings.defaulttax)
            {
                temp = GeneralSettings.defaultmileagetax;
                tempname = GeneralSettings.defaultmileagetax;
                Taxes mtax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", temp));
                if (mtax == null)
                {
                    mtax = ObjectSpace.CreateObject<Taxes>();
                    mtax.BoCode = temp;
                    mtax.BoName = tempname;
                }
            }
            temp = GeneralSettings.defmileage;
            tempname = GeneralSettings.defmileage;
            Mileages mileage = ObjectSpace.FindObject<Mileages>(new BinaryOperator("BoCode", temp));
            if (mileage == null)
            {
                mileage = ObjectSpace.CreateObject<Mileages>();
                mileage.BoCode = temp;
                mileage.BoName = tempname;
            }

            temp = GeneralSettings.defaultregion;
            tempname = "Local";
            Regions region = ObjectSpace.FindObject<Regions>(new BinaryOperator("BoCode", temp));
            if (region == null)
            {
                region = ObjectSpace.CreateObject<Regions>();
                region.BoCode = temp;
                region.BoName = tempname;
            }

            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private PermissionPolicyRole CreateDefaultRole() {
            PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                defaultRole.Name = "Default";

				defaultRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
				defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);                
            }
            return defaultRole;
        }
    }
}
