using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class AddItemControllers : ViewController
    {
        public AddItemControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is ListView && !View.IsRoot)
            {
                NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
                if (controller != null)
                {
                    //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                    controller.ObjectCreated += Controller_ObjectCreated;
                }
            }
        }

        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            ListView lv = ((ListView)View);
            if (lv.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                if (collectionSource.MasterObject != null)
                {
                    int minvalue = -1;
                    int comparevalue = 0;
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxDetails))
                    {
                        if (collectionSource.MasterObjectType == typeof(ClaimTrxs))
                        {
                            ClaimTrxs masterobject = (ClaimTrxs)collectionSource.MasterObject;
                            ClaimTrxDetails currentobject = (ClaimTrxDetails)e.CreatedObject;
                            if (masterobject.Currency != null)
                                currentobject.Currency = currentobject.Session.GetObjectByKey<Currencies>(masterobject.Currency.Oid);
                            currentobject.FCRate = masterobject.FCRate;

                            #region assign oid
                            if (masterobject.ClaimTrxDetail.Count > 0)
                            {
                                comparevalue = masterobject.ClaimTrxDetail.Min(pp => pp.Oid);
                            }
                            if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                            currentobject.Oid = minvalue;
                            #endregion
                        }
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxDetailNotes))
                    {
                        if (collectionSource.MasterObjectType == typeof(ClaimTrxDetails))
                        {
                            ClaimTrxDetails masterobject = (ClaimTrxDetails)collectionSource.MasterObject;
                            ClaimTrxDetailNotes currentobject = (ClaimTrxDetailNotes)e.CreatedObject;
                            if (masterobject.Currency != null)
                            {
                                currentobject.Currency = currentobject.Session.GetObjectByKey<Currencies>(masterobject.Currency.Oid);
                                currentobject.FCRate = masterobject.FCRate;
                            }
                            else if (masterobject.ClaimTrx != null && masterobject.ClaimTrx.Currency != null)
                            {
                                currentobject.Currency = currentobject.Session.GetObjectByKey<Currencies>(masterobject.ClaimTrx.Currency.Oid);
                                currentobject.FCRate = masterobject.ClaimTrx.FCRate;
                            }
                            else
                                currentobject.FCRate = masterobject.FCRate;

                            #region assign oid
                            if (masterobject.ClaimTrxDetailNote.Count > 0)
                            {
                                comparevalue = masterobject.ClaimTrxDetailNote.Min(pp => pp.Oid);
                            }
                            if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                            currentobject.Oid = (masterobject.Oid < 0? masterobject.Oid * 100: masterobject.Oid * -100) + minvalue;
                            #endregion
                        }
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxItems))
                    {
                        if (collectionSource.MasterObjectType == typeof(ClaimTrxs))
                        {
                            ClaimTrxs masterobject = (ClaimTrxs)collectionSource.MasterObject;
                            ClaimTrxItems currentobject = (ClaimTrxItems)e.CreatedObject;
                            if (masterobject.Currency != null)
                                currentobject.Currency = currentobject.Session.GetObjectByKey<Currencies>(masterobject.Currency.Oid);
                            currentobject.FCRate = masterobject.FCRate;

                            #region assign oid
                            if (masterobject.ClaimTrxItem.Count > 0)
                            {
                                comparevalue = masterobject.ClaimTrxItem.Min(pp => pp.Oid);
                            }
                            if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                            currentobject.Oid = minvalue;
                            #endregion
                        }
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxMileages))
                    {
                        if (collectionSource.MasterObjectType == typeof(ClaimTrxs))
                        {
                            ClaimTrxs masterobject = (ClaimTrxs)collectionSource.MasterObject;
                            ClaimTrxMileages currentobject = (ClaimTrxMileages)e.CreatedObject;

                            #region assign oid
                            if (masterobject.ClaimTrxMileage.Count > 0)
                            {
                                comparevalue = masterobject.ClaimTrxMileage.Min(pp => pp.Oid);
                            }
                            if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                            currentobject.Oid = minvalue;
                            #endregion
                        }
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxPostDetails))
                    {
                        if (collectionSource.MasterObjectType == typeof(ClaimTrxs))
                        {
                            ClaimTrxs masterobject = (ClaimTrxs)collectionSource.MasterObject;
                            ClaimTrxPostDetails currentobject = (ClaimTrxPostDetails)e.CreatedObject;

                            #region assign oid
                            if (masterobject.ClaimTrxPostDetail.Count > 0)
                            {
                                comparevalue = masterobject.ClaimTrxPostDetail.Min(pp => pp.Oid);
                            }
                            if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                            currentobject.Oid = minvalue;
                            #endregion
                        }
                    }

                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            if (View is ListView && !View.IsRoot)
            {
                NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
                if (controller != null)
                {
                    controller.ObjectCreated -= Controller_ObjectCreated;
                }
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
