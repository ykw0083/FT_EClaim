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

namespace FT_EClaim.Module.BusinessObjects
{

    //[DefaultClassOptions]
    //public class Person : XPObject
    //{
    //    public string Name
    //    {
    //        get { return fName; }
    //        set { SetPropertyValue(nameof(Name), ref fName, value); }
    //    }
    //    string fName = "";

    //}

    //[MapInheritance(MapInheritanceType.ParentTable)]
    //class Customer : Person
    //{
    //    public string Preferences
    //    {
    //        get { return fPreferences; }
    //        set { SetPropertyValue(nameof(Preferences), ref fPreferences, value); }
    //    }
    //    string fPreferences = "";

    //}

    //[MapInheritance(MapInheritanceType.ParentTable)]
    //public class Employee : Person
    //{
    //    public int Salary
    //    {
    //        get { return fSalary; }
    //        set { SetPropertyValue(nameof(Salary), ref fSalary, value); }
    //    }
    //    int fSalary = 1000;

    //}

    //[MapInheritance(MapInheritanceType.ParentTable)]
    //public class Executive : Employee
    //{
    //    public int Bonus
    //    {
    //        get { return fBonus; }
    //        set { SetPropertyValue(nameof(Bonus), ref fBonus, value); }
    //    }
    //    int fBonus = 100;

    //}
}