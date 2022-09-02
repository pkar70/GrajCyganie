'' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

'''' <summary>
'''' An empty page that can be used on its own or navigated to within a Frame.
'''' </summary>
'Public NotInheritable Class JamPilot
'    Inherits Page

'    Dim mDevList As Collection(Of Windows.System.RemoteSystems.RemoteSystem)
'    Dim mWatcher As Windows.System.RemoteSystems.RemoteSystemWatcher

'    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
'        Dim oAccStat As Windows.System.RemoteSystems.RemoteSystemAccessStatus =
'            Await Windows.System.RemoteSystems.RemoteSystem.RequestAccessAsync()

'        If oAccStat <> Windows.System.RemoteSystems.RemoteSystemAccessStatus.Allowed Then
'            Await DialogBoxAsync("Not permission")
'            Return
'        End If

'        mDevList = New Collection(Of Windows.System.RemoteSystems.RemoteSystem)
'        mWatcher = Windows.System.RemoteSystems.RemoteSystem.CreateWatcher()
'        AddHandler mWatcher.RemoteSystemAdded, AddressOf remsys_Added
'        AddHandler mWatcher.RemoteSystemRemoved, AddressOf remsys_Remove
'        AddHandler mWatcher.RemoteSystemUpdated, AddressOf remsys_Update
'        mWatcher.Start()
'    End Sub

'#Region "lista RemoteSystem"

'    Private Async Sub remsys_Update(sender As Windows.System.RemoteSystems.RemoteSystemWatcher,
'                              oArgs As Windows.System.RemoteSystems.RemoteSystemUpdatedEventArgs)

'        mDevList.Remove(mDevList.First(Function(aa) aa.Id = oArgs.RemoteSystem.Id))
'        Await remsys_SprawdzAdd(oArgs.RemoteSystem, True)

'    End Sub

'    Private Sub remsys_Remove(sender As Windows.System.RemoteSystems.RemoteSystemWatcher,
'                              oArgs As Windows.System.RemoteSystems.RemoteSystemRemovedEventArgs)

'        mDevList.Remove(mDevList.First(Function(aa) aa.Id = oArgs.RemoteSystemId))
'        UpdateList()
'    End Sub

'    Private Async Function remsys_SprawdzAdd(oRemSys As Windows.System.RemoteSystems.RemoteSystem, bAdd As Boolean) As Task(Of Boolean)
'        If oRemSys.Status <> Windows.System.RemoteSystems.RemoteSystemStatus.Available Then Return False

'        If Not Await oRemSys.GetCapabilitySupportedAsync(Windows.System.RemoteSystems.KnownRemoteSystemCapabilities.AppService) Then Return False

'        ' a teraz sprawdz cygan - czy zwroci OK
'        ' jesli nie - zwróc False
'        Dim sResp As String = Await SendMessageToRemoteSystemAsync(oRemSys, "cygan")
'        If sResp <> "OK" Then
'            Debug.WriteLine("ale nie ma OK z Cygan")
'            Return False
'        End If

'        If Not bAdd Then Return True    ' tylko sprawdz, bez dodawania

'        mDevList.Add(oRemSys)
'        UpdateList()

'        Return True
'    End Function

'    Private Async Sub remsys_Added(sender As Windows.System.RemoteSystems.RemoteSystemWatcher,
'                                   oargs As Windows.System.RemoteSystems.RemoteSystemAddedEventArgs)

'        Debug.WriteLine("  found: " & oargs.RemoteSystem.DisplayName)
'        Await remsys_SprawdzAdd(oargs.RemoteSystem, True)

'        ' oargs.RemoteSystem ' DisplayName Id IsAvailableByProximity  IsAvailableBySpatialProximity 
'    End Sub

'    Private Async Sub UpdateList()
'        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf UpdateListUI)
'    End Sub

'    Private Sub UpdateListUI()
'        uiListItems.ItemsSource = From c In mDevList
'        ' nie moze byc samo mDevList, bo wtedy tylko jedno pokazuje!
'    End Sub

'    Public Async Function SendCommandToRemoteSystemAsync(
'                     oRemSys As Windows.System.RemoteSystems.RemoteSystem, sMsg As String) As Task(Of AppService.AppServiceResponse)

'        Try
'            Dim mRemSysConn As AppService.AppServiceConnection

'            mRemSysConn = New AppService.AppServiceConnection()
'        ' bez tego pisze ze w zlym stanie, a z tym - ze unavailable
'        ' mRemSysConn.AppServiceName = "InProcessAppService" : RemoteSystemNotSupportedByApp
'        mRemSysConn.AppServiceName = "com.microsoft.pkar.cygan" ' AppServiceUnavailable, a przy takiej samej nazwie: RemoteSystemNotSupportedByApp

'        mRemSysConn.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName

'        Dim mRemSysReq As Windows.System.RemoteSystems.RemoteSystemConnectionRequest
'        mRemSysReq = New Windows.System.RemoteSystems.RemoteSystemConnectionRequest(oRemSys)

'        Dim mRemSysStat As AppService.AppServiceConnectionStatus
'        mRemSysStat = Await mRemSysConn.OpenRemoteAsync(mRemSysReq)

'        If mRemSysStat <> AppService.AppServiceConnectionStatus.Success Then Return Nothing

'        Dim oInputs = New ValueSet
'        oInputs.Add("command", sMsg)

'        Dim oRemSysResp As AppService.AppServiceResponse = Await mRemSysConn.SendMessageAsync(oInputs)

'        If Not oRemSysResp.Message.ContainsKey("result") Then Return Nothing

'        mRemSysConn.Dispose()

'        Return oRemSysResp

'        Catch ex As Exception
'            CrashMessageAdd("@SendCommandToRemoteSystemAsync", ex)
'        End Try

'        Return Nothing
'    End Function

'    Public Async Function SendMessageToRemoteSystemAsync(
'                     oRemSys As Windows.System.RemoteSystems.RemoteSystem, sMsg As String) As Task(Of String)

'        Dim oRemSysResp As AppService.AppServiceResponse = Await SendCommandToRemoteSystemAsync(oRemSys, sMsg)

'        If oRemSysResp Is Nothing Then Return ""

'        Return oRemSysResp.Message("result").ToString()

'    End Function

'#End Region


'    Private Sub Page_Unloaded(sender As Object, e As RoutedEventArgs)
'        ' uiSzukaj.Content = "LosingFocus"
'        Try
'            mWatcher.Stop()
'        Catch ex As Exception

'        End Try
'    End Sub

'    Private Async Function SendCommandCheckConfirm(sender As Object, sCmd As String) As Task(Of Boolean)
'        Dim oRemSys As Windows.System.RemoteSystems.RemoteSystem
'        oRemSys = TryCast(sender, MenuFlyoutItem).DataContext
'        If oRemSys Is Nothing Then Return False

'        Dim sResp As String = Await SendMessageToRemoteSystemAsync(oRemSys, sCmd)
'        If sResp <> "OK" Then
'            ' Debug.WriteLine("ret z komendy '" & sCmd & "': " & sResp)
'            DialogBox("ret z komendy '" & sCmd & "': " & sResp)
'            Return False
'        End If

'        Return True
'    End Function

'    Private Async Sub uiGoNext_Click(sender As Object, e As RoutedEventArgs)
'        Await SendCommandCheckConfirm(sender, "next")
'    End Sub

'    Private Async Sub uiPause_Click(sender As Object, e As RoutedEventArgs)
'        Await SendCommandCheckConfirm(sender, "pause")
'    End Sub

'    Private Async Sub uiGetDetails_Click(sender As Object, e As RoutedEventArgs)
'        Dim oRemSys As Windows.System.RemoteSystems.RemoteSystem
'        oRemSys = TryCast(sender, MenuFlyoutItem).DataContext
'        If oRemSys Is Nothing Then Return

'        Dim oRemSysResp As AppService.AppServiceResponse = Await SendCommandToRemoteSystemAsync(oRemSys, "details")
'        If oRemSysResp Is Nothing Then Return
'        If oRemSysResp.Message("result").ToString() <> "OK" Then
'            DialogBox("Error: " & oRemSysResp.Message("result").ToString())
'            Return
'        End If

'        Dim sXml As String = oRemSysResp.Message("granyUtwor").ToString()
'        If sXml.Length < 10 Then
'            DialogBox("Error: too short data: " & sXml)
'            Return
'        End If

'        Dim oSer As Xml.Serialization.XmlSerializer
'        oSer = New Xml.Serialization.XmlSerializer(GetType(tGranyUtwor))
'        Dim oStream As Stream = New MemoryStream
'        Dim oWrt As StreamWriter = New StreamWriter(oStream)
'        oWrt.Write(sXml)

'        App.mtGranyUtwor = oSer.Deserialize(oStream)
'        oStream.Flush()
'        Me.Frame.GoBack()
'    End Sub
'End Class
