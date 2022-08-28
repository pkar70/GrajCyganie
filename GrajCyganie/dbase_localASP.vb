Public Class dbase_localASP
    Inherits dbase_baseASP
    Public Overrides ReadOnly Property Nazwa As String = "LocalASP"

    Protected Overrides ReadOnly Property BaseUri As String = "http://127.0.0.1/"

End Class
