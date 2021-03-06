Imports Windows.Web.Http

Public Class tDekada
    Public Property sNazwa As String
    Public Property iCount As Integer
    Public Property iPlayTime As Integer
    Public Property sPlayTime As String
    Public Property iFreq As Integer = 2
    Public Property sFreq As String = "100 %"
End Class

Public Class tGranyUtwor
    Public Property ID As Integer
    Public Property artist As String
    Public Property album As String
    Public Property comment As String
    Public Property title As String
    Public Property track As String
    Public Property bitrate As Integer
    Public Property duration As Integer
    Public Property channels As String
    Public Property sample As Integer
    Public Property year As String
    Public Property dekada As String
    Public Property uri As String
    Public Property fsize As Integer
    Public Property countArtist As Integer = 0
    Public Property countAlbum As Integer = 0
    Public Property countTitle As Integer = 0
    Public Property countYear As Integer = 0
    Public Property countDekada As Integer = 0

End Class

''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
Partial NotInheritable Class App
    Inherits Application

#Region "Autogenerated"

    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Do not repeat app initialization when the Window already has content,
        ' just ensure that the window is active

        If rootFrame Is Nothing Then
            ' Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed
            ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
            AddHandler rootFrame.Navigated, AddressOf OnNavigatedAddBackButton
            AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf OnBackButtonPressed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Load state from previously suspended application
            End If
            ' Place the frame in the current Window
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' Ensure the current window is active
            Window.Current.Activate()
        End If

    End Sub

    ''' <summary>
    ''' Invoked when Navigation to a certain page fails
    ''' </summary>
    ''' <param name="sender">The Frame which failed navigation</param>
    ''' <param name="e">Details about the navigation failure</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Invoked when application execution is being suspended.  Application state is saved
    ''' without knowing whether the application will be terminated or resumed with the contents
    ''' of memory still intact.
    ''' </summary>
    ''' <param name="sender">The source of the suspend request.</param>
    ''' <param name="e">Details about the suspend request.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Save application state and stop any background activity
        deferral.Complete()
    End Sub
#End Region

    Public Shared moHttp As HttpClient = New HttpClient
    Public Shared Async Function HttpPageAsync(sUrl As String, sErrMsg As String, Optional sData As String = "") As Task(Of String)
#If CONFIG = "Debug" Then
        ' próba wylapywania errorów gdy nic innego tego nie złapie
        Dim sDebugCatch As String = ""
        Try
#End If
            If Not NetIsIPavailable(True) Then Return ""
            If sUrl = "" Then Return ""

            If sUrl.Substring(0, 4) <> "http" Then sUrl = BaseUri & "/p/dysk" & sUrl

            If App.moHttp Is Nothing Then
                App.moHttp = New HttpClient
                App.moHttp.DefaultRequestHeaders.UserAgent.TryParseAdd("GrajCyganie")
            End If

            Dim sError = ""
            Dim oResp As HttpResponseMessage = Nothing

            Try
                If sData <> "" Then
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await App.moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await App.moHttp.GetAsync(New Uri(sUrl))
                End If
            Catch ex As Exception
                sError = ex.Message
            End Try

            If sError <> "" Then
                Await DialogBoxAsync("error " & sError & " at " & sErrMsg & " page")
                Return ""
            End If

            If oResp.StatusCode = 303 Or oResp.StatusCode = 302 Or oResp.StatusCode = 301 Then
                ' redirect
                sUrl = oResp.Headers.Location.ToString
                'If sUrl.ToLower.Substring(0, 4) <> "http" Then
                '    sUrl = "https://sympatia.onet.pl/" & sUrl   ' potrzebne przy szukaniu
                'End If

                If sData <> "" Then
                    ' Dim oHttpCont = New HttpStringContent(sData, Text.Encoding.UTF8, "application/x-www-form-urlencoded")
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await App.moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await App.moHttp.GetAsync(New Uri(sUrl))
                End If
            End If

            If oResp.StatusCode > 290 Then
                Await DialogBoxAsync("ERROR " & oResp.StatusCode & " getting " & sErrMsg & " page")
                Return ""
            End If

            Dim sResp As String = ""
            Try
                sResp = Await oResp.Content.ReadAsStringAsync
            Catch ex As Exception
                sError = ex.Message
            End Try

            If sError <> "" Then
                Await DialogBoxAsync("error " & sError & " at ReadAsStringAsync " & sErrMsg & " page")
                Return ""
            End If

            Return sResp

#If CONFIG = "Debug" Then
        Catch ex As Exception
            sDebugCatch = ex.Message
        End Try

        If sDebugCatch <> "" Then
#Disable Warning BC42358
            DialogBox("DebugCatch in HttpPageAsync:" & vbCrLf & sDebugCatch)
#Enable Warning BC42358
        End If
        Return ""
#End If
    End Function




    Public Shared mlDekady As ObservableCollection(Of tDekada) = Nothing
    Public Shared mbGranted As Boolean = False
    Public Shared gsLog As String = ""

    Public Shared Async Function BeskidGetPermission(sUser As String) As Task(Of String)
        mbGranted = False
        If sUser = "" Then Return "Zaloguj się"

        Dim sRes As String = Await App.HttpPageAsync("/cygan-login.asp?user=" & sUser, "login page")
        If sRes.IndexOf("Masz prawo") > -1 Then mbGranted = 1

        Return sRes
    End Function

    Public Shared Async Function BeskidLogin(bForce As Boolean) As Task(Of Boolean)
        If Not bForce AndAlso mbGranted Then Return True
        Await BeskidGetPermission(GetSettingsString("userName"))
        If mbGranted Then Return True
        Return False
    End Function

    Private Shared Async Function BeskidLoadDekadyFile() As Task(Of Boolean)
        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder
        Dim oFile As Windows.Storage.StorageFile = Await oFold.TryGetItemAsync("dekady.xml")
        If oFile Is Nothing Then Return False

        Dim oSer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(ObservableCollection(Of tDekada)))
        Dim oStream As Stream = Await oFile.OpenStreamForReadAsync
        Dim bError As Boolean = False
        Try
            mlDekady = TryCast(oSer.Deserialize(oStream), ObservableCollection(Of tDekada))
        Catch ex As Exception
            bError = True
        End Try
        oStream.Dispose()   ' == fclose
        If bError Then Return False

        If mlDekady.Count > 0 Then Return True
        Return False
    End Function

    Public Shared Async Function BeskidSaveDekadyFile() As Task(Of Boolean)
        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder
        Dim oFile As Windows.Storage.StorageFile = Await oFold.CreateFileAsync("dekady.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting)
        If oFile Is Nothing Then Return False

        Dim oSer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(ObservableCollection(Of tDekada)))
        Dim oStream As Stream = Await oFile.OpenStreamForWriteAsync
        oSer.Serialize(oStream, mlDekady)
        oStream.Dispose()   ' == fclose

        Return True
    End Function

    Public Shared Function TimeSecToString(iSec As String) As String
        Dim oTS As TimeSpan = TimeSpan.FromSeconds(iSec)
        Dim sTxt As String = oTS.ToString("h\:mm\:ss")
        If oTS.Days > 0 Then
            If oTS.Hours < 10 Then sTxt = "0" & sTxt    ' jesli dajemy dni, to godziny z zerem
            sTxt = oTS.Days & "d " & sTxt
        End If
        Return sTxt
    End Function
    Private Shared Async Function BeskidDownloadDekady() As Task(Of Boolean)
        Dim sRes As String = Await App.HttpPageAsync("/cygan-dekady.asp", "statystyka dekad")

        mlDekady.Clear()

        Dim iTotalCnt As Integer = 0
        Dim iTotalTime As Integer = 0

        Dim aDekady As String() = sRes.Split(vbCrLf)
        Dim oNew As tDekada

        For Each sDekada In aDekady
            Dim aFields As String() = sDekada.Split("|")
            If aFields.GetUpperBound(0) = 2 Then
                oNew = New tDekada
                oNew.sNazwa = aFields(0).Trim
                oNew.iCount = aFields(1)
                iTotalCnt = iTotalCnt + aFields(1)
                oNew.iPlayTime = aFields(2)
                iTotalTime = iTotalTime + aFields(2)
                oNew.sPlayTime = TimeSecToString(oNew.iPlayTime)
                mlDekady.Add(oNew)
            End If
        Next

        If mlDekady.Count < 1 Then Return False

        ' total
        oNew = New tDekada
        oNew.sNazwa = "Total"
        oNew.iCount = iTotalCnt
        oNew.iPlayTime = iTotalTime
        oNew.sPlayTime = TimeSecToString(iTotalTime)
        mlDekady.Add(oNew)

        sRes = Await App.HttpPageAsync("/cygan-maxId.asp", "maxno")
        If sRes = "" Then Return False
        SetSettingsInt("maxSoundId", sRes.Trim)

        Return True
    End Function
    Public Shared Async Function BeskidGetDekady(bForce As Boolean) As Task(Of Boolean)
        If Not Await BeskidLogin(False) Then
            Await DialogBoxAsync("nie jesteś zalogowany")
            Return False
        End If

        If Not bForce AndAlso mlDekady IsNot Nothing Then Return True

        mlDekady = New ObservableCollection(Of tDekada)

        ' jesli ponad 30 dni minelo, to wymuś wczytanie dekad z sieci 
        If Math.Abs(GetSettingsInt("lastDekadyStat") - Date.Now.DayOfYear) > 30 Then bForce = True

        If Not bForce Then
            If Await BeskidLoadDekadyFile() Then Return True
        End If

        ' jesli nie z cache, to z serwera
        If Not Await BeskidDownloadDekady() Then Return False

        ' przetworz int na procenty
        For Each oItem As tDekada In App.mlDekady
            oItem.sFreq = App.FreqSlider2Text(oItem.iFreq)
        Next

        Await BeskidSaveDekadyFile()
        SetSettingsInt("lastDekadyStat", Date.Now.DayOfYear)

        Return True
    End Function

    Public Shared Function FreqSlider2Counter(iValue As Integer) As Integer
        Select Case iValue
            Case 0
                Return 1000
            Case 1
                Return 10
            Case 2
                Return 5
            Case 3
                Return 3
            Case 4
                Return 2
            Case Else
                Return 1
        End Select
    End Function

    Public Shared Function FreqSlider2Text(iValue As Integer) As String
        Select Case iValue
            Case 0
                Return "  0 %"
            Case 1
                Return " 10 %"
            Case 2
                Return " 20 %"
            Case 3
                Return " 33 %"
            Case 4
                Return " 50 %"
            Case Else
                Return "100 %"
        End Select
    End Function

    Public Shared miSessionFiles As Integer
    Public Shared miSessionMiB As Integer

    Public Shared mtGranyUtwor As tGranyUtwor
    Public Shared moMediaPlayer As Windows.Media.Playback.MediaPlayer = Nothing

    Private Shared moRandom As Random = New Random
    Public Shared Function MakeRandom(iMax As Integer) As Integer
        ' Random = Windows.Security.Cryptography.CryptographicBuffer.GenerateRandomNumber()
        Return moRandom.Next(iMax + 1)
    End Function

    Public Shared Sub OpenBrowser(oUri As Uri, bForceEdge As Boolean)
        If bForceEdge Then
            Dim options As Windows.System.LauncherOptions = New Windows.System.LauncherOptions()
            options.TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            Windows.System.Launcher.LaunchUriAsync(oUri, options)
        Else
            Windows.System.Launcher.LaunchUriAsync(oUri)
        End If

    End Sub

    Public Shared Sub OpenBrowser(sUri As String, bForceEdge As Boolean)
        Dim oUri As Uri = New Uri(sUri)
        OpenBrowser(oUri, bForceEdge)
    End Sub

#Region "Speech control"
    Public Shared moReco As Windows.Media.SpeechRecognition.SpeechRecognizer = Nothing

    Private Shared Function MakeRule(sTag As String, Optional sStr1 As String = "", Optional sStr2 As String = "", Optional sStr3 As String = "", Optional sStr4 As String = "", Optional sStr5 As String = "") As Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint
        Dim oList As List(Of String)
        oList = New List(Of String)
        oList.Clear()
        If sStr1 <> "" Then
            oList.Add(sStr1)
        Else
            oList.Add(sTag)
        End If
        If sStr2 <> "" Then oList.Add(sStr2)
        If sStr3 <> "" Then oList.Add(sStr3)
        If sStr4 <> "" Then oList.Add(sStr4)
        If sStr5 <> "" Then oList.Add(sStr5)

        Return New Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint(oList, sTag)

    End Function
    Public Shared Sub SpeechCommandCreateRules()

        moReco.Constraints.Clear()
        moReco.Constraints.Add(MakeRule("play", "play", "start"))
        moReco.Constraints.Add(MakeRule("stop"))
        moReco.Constraints.Add(MakeRule("pause", "pause", "silence"))
        moReco.Constraints.Add(MakeRule("cont", "continue", "resume"))
        moReco.Constraints.Add(MakeRule("next"))
        moReco.Constraints.Add(MakeRule("prev", "previous", "back"))
        moReco.Constraints.Add(MakeRule("info", "info", "describe"))
        moReco.Constraints.Add(MakeRule("loopArt", "loop artist"))
        moReco.Constraints.Add(MakeRule("loopTitle", "loop title"))
        moReco.Constraints.Add(MakeRule("loopAlbum", "loop album"))
        moReco.Constraints.Add(MakeRule("loopYear", "loop year"))
        moReco.Constraints.Add(MakeRule("loopDecade", "loop decade"))
        moReco.Constraints.Add(MakeRule("loopSong", "loop song"))
        moReco.Constraints.Add(MakeRule("loopNone", "loop none", "no loop"))
        moReco.Constraints.Add(MakeRule("stat", "stats", "how many", "statistic"))
        moReco.Constraints.Add(MakeRule("before"))
        moReco.Constraints.Add(MakeRule("after"))

    End Sub

    Public Shared Sub SpeechCommandSetTimeouts()
        moReco.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.InitialSilenceTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.BabbleTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.EndSilenceTimeout = TimeSpan.FromDays(60)
    End Sub

    Public Shared Function SpeechCommandText2Tag(sTxt As String) As String
        For Each oRule As Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint In moReco.Constraints
            For Each sCmd As String In oRule.Commands
                If sCmd = sTxt Then Return oRule.Tag
            Next
        Next

        Return ""
    End Function

#End Region

    Private moTaskDeferal As Background.BackgroundTaskDeferral = Nothing
    Private moAppConn As AppService.AppServiceConnection

    Protected Overrides Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)

        Select Case args.TaskInstance.Task.Name
            Case "MyCamerasCalUpdate"
            Case Else
                Dim oDetails As AppService.AppServiceTriggerDetails =
            TryCast(args.TaskInstance.TriggerDetails, AppService.AppServiceTriggerDetails)
                If oDetails IsNot Nothing Then
                    ' zrob co trzeba
                    moTaskDeferal = args.TaskInstance.GetDeferral()
                    AddHandler args.TaskInstance.Canceled, AddressOf OnTaskCanceled
                    moAppConn = oDetails.AppServiceConnection
                    AddHandler moAppConn.RequestReceived, AddressOf OnRequestReceived
                    ' AddHandler moAppConn.ServiceClosed, AddressOf OnServiceClosed
                End If
        End Select

    End Sub

    Private Sub OnTaskCanceled(sender As Background.IBackgroundTaskInstance, reason As Background.BackgroundTaskCancellationReason)
        If moTaskDeferal IsNot Nothing Then
            moTaskDeferal.Complete()
            moTaskDeferal = Nothing
        End If
        'If oAppConn IsNot Nothing Then
        '    oAppConn.Dispose()
        '    oAppConn = Nothing
        'End If
    End Sub

    Public Shared Event PilotChce(sCmd As String)

    Private Async Sub OnRequestReceived(sender As AppService.AppServiceConnection, args As AppService.AppServiceRequestReceivedEventArgs)
        'Get a deferral so we can use an awaitable API to respond to the message 
        Dim messageDeferral As AppService.AppServiceDeferral = args.GetDeferral()
        Dim oInputMsg As ValueSet = args.Request.Message
        Dim oResultMsg As ValueSet = New ValueSet()
        Dim sResult As String = "ERROR while processing command"
        Try
            Dim sCommand As String = CType(oInputMsg("command"), String)

            Select Case sCommand.ToLower
                Case "cygan"
                    sResult = "OK"
                Case "ping"
                    sResult = "pong" & vbCrLf &
                        Package.Current.Id.Version.Major & "." &
                            Package.Current.Id.Version.Minor & "." & Package.Current.Id.Version.Build
                    If Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile().IsWlanConnectionProfile Then
                        sResult = sResult & vbCrLf & "WIFI"
                    Else
                        sResult = sResult & vbCrLf & "OTHER"
                    End If
                Case "net"
                    If Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile().IsWlanConnectionProfile Then
                        sResult = "WIFI"
                    Else
                        sResult = "OTHER"
                    End If
                Case "ver"
                    sResult = Package.Current.Id.Version.Major & "." &
                        Package.Current.Id.Version.Minor & "." & Package.Current.Id.Version.Build
                Case "details"
                    If mtGranyUtwor Is Nothing Then
                        sResult = "No current file"
                    Else
                        sResult = "OK"
                        Dim sOpis As String
                        sOpis = mtGranyUtwor.artist & " - " & mtGranyUtwor.title
                        oResultMsg.Add("name", CType(sOpis, String))

                        Dim oSer As Xml.Serialization.XmlSerializer
                        oSer = New Xml.Serialization.XmlSerializer(GetType(tGranyUtwor))
                        Dim oStream As Stream = New MemoryStream
                        oSer.Serialize(oStream, mtGranyUtwor)
                        oStream.Flush()
                        Dim oRdr As StreamReader = New StreamReader(oStream)
                        oResultMsg.Add("granyUtwor", CType(oRdr.ReadToEnd, String))
                    End If
                Case "pause", "next"
                    RaiseEvent PilotChce(sCommand.ToLower)
                    sResult = "OK"
                Case "stop"
                Case "start"
                Case Else
                    sResult = "ERROR unknown command"

            End Select
        Catch ex As Exception

        End Try

        ' odsylamy cokolwiek - zeby "tamta strona" cos zobaczyla
        oResultMsg.Add("result", CType(sResult, String))
        Await args.Request.SendResponseAsync(oResultMsg)

        messageDeferral.Complete()
    End Sub

End Class
