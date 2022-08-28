Public Class dbase_domekASP
    Inherits dbase_baseASP

    Public Overrides ReadOnly Property Nazwa As String = "DomekASP"

    Protected Overrides ReadOnly Property BaseUri As String = "http://" & App.gDomekIP
End Class
