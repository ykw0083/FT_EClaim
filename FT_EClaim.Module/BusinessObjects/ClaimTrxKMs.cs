using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace FT_EClaim.Module.BusinessObjects
{
    [DomainComponent]
    [NonPersistent]
    [Appearance("ClaimTrxKMs1", AppearanceItemType = "Action", TargetItems = "New", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs2", AppearanceItemType = "Action", TargetItems = "Edit", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs3", AppearanceItemType = "Action", TargetItems = "New", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs4", AppearanceItemType = "Action", TargetItems = "Edit", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs5", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs6", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs7", AppearanceItemType = "Action", TargetItems = "Delete", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxKMs8", AppearanceItemType = "Action", TargetItems = "Delete", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("Claim KM")]
    public class ClaimTrxKMs : XPObject
    {
        public ClaimTrxKMs(Session session)
        : base(session)
        {
            //MyNotifications = new List<MyNotifications>();
        }
        public override void AfterConstruction()
        {
            KM = 0;
            Amount = 0;
        }
        
        public Mileages Mileage { get; set; }
        public int KM { get; set; }
        public decimal Amount { get; set; }
        //[Aggregated]
        //public virtual IList<MyNotifications> MyNotifications { get; set; }

        public decimal GetKMAmount()
        {
            if (KM <= 0) return 0;
            decimal rtn = 0;
            int km = KM;
            int temp = 0;


            if (Mileage != null)
            {
                MileageDetails obj = null;
                while (km > 0)
                {
                    if (Mileage.MileageDetail.Where(p => p.EndKM >= km).Count() > 0)
                    {
                        obj = Mileage.MileageDetail.Where(p => p.EndKM >= km).OrderBy(p => p.EndKM).First();
                        temp = km - obj.StartKM + 1;
                        rtn += (decimal)temp * obj.KMRate;
                        km = obj.StartKM - 1;
                    }
                    else
                        km = 0;
                }
            }
            Amount = rtn;
            return rtn;
        }
    }

    [DomainComponent]
    [NonPersistent]
    public class ClaimKMs : XPObject
    {
        public ClaimKMs(Session session)
        : base(session)
        {
            //MyNotifications = new List<MyNotifications>();
        }

        public IList<ClaimTrxKMs> AddClaimTrxKM(XPCollection<ClaimTrxMileages> ClaimTrxMileage, ref decimal amount)
        {
            decimal rtn = 0;
            bool found = false;

            IList<ClaimTrxKMs> _ClaimTrxKM = new List<ClaimTrxKMs>();

            foreach (ClaimTrxMileages dtl in ClaimTrxMileage)
            {
                found = false;
                foreach (ClaimTrxKMs dtlkm in _ClaimTrxKM)
                {
                    if (dtl.Mileage.Oid == dtlkm.Mileage.Oid)
                    {
                        found = true;
                        dtlkm.KM = dtlkm.KM + dtl.KM;
                    }
                }
                if (!found)
                {
                    ClaimTrxKMs obj = new ClaimTrxKMs(Session);
                    obj.Mileage = Session.FindObject<Mileages>(new BinaryOperator("Oid", dtl.Mileage.Oid, BinaryOperatorType.Equal));
                    obj.KM = dtl.KM;
                    _ClaimTrxKM.Add(obj);
                }

            }
            if (_ClaimTrxKM.Count > 0)
            {
                foreach (ClaimTrxKMs dtlkm in _ClaimTrxKM)
                {
                    rtn += dtlkm.GetKMAmount();
                }
            }
            amount = rtn;

            return _ClaimTrxKM;
        }
    }

}