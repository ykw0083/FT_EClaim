;#DropDB FT_EClaimEasyTest

#Application FT_EClaim.Win
#Application FT_EClaim.Web

*FillForm
 User Name = Admin
*Action Log In

#IfDef FT_EClaim.Win

*Action About...
 
*Action OK

#EndIf

#IfDef FT_EClaim.Web
*Action Navigation(My Details)
#EndIf
