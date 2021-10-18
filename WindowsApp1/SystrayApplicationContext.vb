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

    ' uwaga: konieczne referencje do Windows.winmd and System.Runtime.WindowsRuntime.dll.

    Public Sub New()
        Dim openMenuItem As MenuItem = New MenuItem("Open UWP", New EventHandler(AddressOf OpenApp))
        Dim sendMenuItem As MenuItem = New MenuItem("Send message to UWP", New EventHandler(AddressOf SendToUWP))
        Dim legacyMenuItem As MenuItem = New MenuItem("Open legacy companion", New EventHandler(AddressOf OpenLegacy))
        Dim exitMenuItem As MenuItem = New MenuItem("Exit", New EventHandler(AddressOf ExitApp))
        openMenuItem.DefaultItem = True

        Dim oMenu As ContextMenu = New ContextMenu
        oMenu.MenuItems.Add(openMenuItem)
        oMenu.MenuItems.Add(sendMenuItem)
        oMenu.MenuItems.Add(legacyMenuItem)
        oMenu.MenuItems.Add(exitMenuItem)


        oNotifyIcon = New NotifyIcon
        AddHandler oNotifyIcon.DoubleClick, AddressOf OpenApp
        ' oNotifyIcon.Icon = Resources.Icon1
        oNotifyIcon.ContextMenu = oMenu
        oNotifyIcon.Visible = True

    End Sub


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
