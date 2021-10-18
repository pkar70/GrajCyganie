Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Core
Imports Windows.ApplicationModel.AppService
Imports Windows.Foundation.Collections

Public Class SystrayApplicationContext
    Inherits ApplicationContext

    Private oConn As AppServiceConnection = Nothing
    Private oNotifyIcon As NotifyIcon = Nothing
    Private oConfWindow As Form1 = New Form1



    '29          public SystrayApplicationContext() 
    '30          { 
    '31              MenuItem openMenuItem = New MenuItem("Open UWP", New EventHandler(OpenApp)); 
    '32              MenuItem sendMenuItem = New MenuItem("Send message to UWP", New EventHandler(SendToUWP)); 
    '33              MenuItem legacyMenuItem = New MenuItem("Open legacy companion", New EventHandler(OpenLegacy)); 
    '34              MenuItem exitMenuItem = New MenuItem("Exit", New EventHandler(Exit)); 
    '35              openMenuItem.DefaultItem = true; 
    '36  

    '37              notifyIcon = New NotifyIcon(); 
    '38              notifyIcon.DoubleClick += New EventHandler(OpenApp); 
    '39              notifyIcon.Icon = SystrayComponent.Properties.Resources.Icon1; 
    '40              notifyIcon.ContextMenu = New ContextMenu(New MenuItem[]{ openMenuItem, sendMenuItem, legacyMenuItem, exitMenuItem }); 
    '41              notifyIcon.Visible = true; 
    '42          } 


    Private Async Sub OpenApp(sender As Object, e As EventArgs)
        '46              IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync(); 
        '47              await appListEntries.First().LaunchAsync(); 
        Dim appListEntries As IEnumerable(Of AppListEntry) = Await Package.Current.GetAppListEntriesAsync()
        Await appListEntries.First().LaunchAsync()
    End Sub


    Private Async Sub SendToUWP(sender As Object, e As EventArgs)
        Dim oMsg As ValueSet = New ValueSet
        oMsg.add("content", "Message from Systray Extension")
        Await SendToUWP(oMsg)

    End Sub

    Private Sub OpenLegacy(sender As Object, e As EventArgs)
        Dim oForm As Form1 = New Form1
        oForm.Show()
    End Sub

    Private Async Sub ExitApp(sender As Object, e As EventArgs)
        Dim oMsg As ValueSet = New ValueSet
        oMsg.add("exit", "")
        Await SendToUWP(oMsg)
        Application.Exit()
    End Sub

    Private Async Function SendToUWP(oMsg As ValueSet) As Task
        If oConn Is Nothing Then
            oConn = New AppServiceConnection
            oConn.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            oConn.AppServiceName = "SystrayExtensionService"

            AddHandler oConn.ServiceClosed, AddressOf Connection_ServiceClosed

            Dim oConnStatus As AppServiceConnectionStatus = Await oConn.OpenAsync

            If oConnStatus <> AppServiceConnectionStatus.Success Then
                MessageBox.Show("Status: " & oConnStatus.ToString())
                Return
            End If

        End If

        Await oConn.SendMessageAsync(oMsg)
    End Function


    Private Sub Connection_ServiceClosed(sender As AppServiceConnection, args As AppServiceClosedEventArgs)
        RemoveHandler oConn.ServiceClosed, AddressOf Connection_ServiceClosed
        oConn = Nothing
    End Sub

End Class
