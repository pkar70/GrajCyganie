' Imports Windows.Web.Http


Imports Vblib
''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
Partial NotInheritable Class App
    Inherits Application

#Region "wizard"


    Protected Function OnLaunchFragment(aes As ApplicationExecutionState) As Frame
        Dim mRootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Do not repeat app initialization when the Window already has content,
        ' just ensure that the window is active

        If mRootFrame Is Nothing Then
            ' Create a Frame to act as the navigation context and navigate to the first page
            mRootFrame = New Frame()

            AddHandler mRootFrame.NavigationFailed, AddressOf OnNavigationFailed

            ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
            AddHandler mRootFrame.Navigated, AddressOf OnNavigatedAddBackButton
            AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf OnBackButtonPressed

            ' Place the frame in the current Window
            Window.Current.Content = mRootFrame

            InitLib(Nothing, True)
        End If

        Return mRootFrame
    End Function

    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)

        Dim mRootFrame As Frame = OnLaunchFragment(e.PreviousExecutionState)

        If e.PrelaunchActivated = False Then
            If mRootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                mRootFrame.Navigate(GetType(MainPage), e.Arguments)
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


    ' RemoteSystems
    Protected Overrides Async Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)
        ' tile update / warnings
        moTaskDeferal = args.TaskInstance.GetDeferral() ' w pkarmodule.App

        Dim bNoComplete As Boolean = False
        Dim bObsluzone As Boolean = False
        'Select Case args.TaskInstance.Task.Name
        '    Case "Wycofania_Timer"
        '        SetSettingsString("lastTimerRun", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        '        If NetIsIPavailable(False) Then Await SciagnijDane(False)
        '        bObsluzone = True
        'End Select

        Dim sLocalCmds As String = ""

        If Not bObsluzone Then bNoComplete = RemSysInit(args, sLocalCmds)

        If Not bNoComplete Then moTaskDeferal.Complete()

    End Sub

    ' CommandLine, Toasts
    Protected Overrides Async Sub OnActivated(args As IActivatedEventArgs)
        ' to jest m.in. dla Toast i tak dalej?

        ' próba czy to commandline
        If args.Kind = ActivationKind.CommandLineLaunch Then

            Dim commandLine As CommandLineActivatedEventArgs = TryCast(args, CommandLineActivatedEventArgs)
            Dim operation As CommandLineActivationOperation = commandLine?.Operation
            Dim strArgs As String = operation?.Arguments

            If Not String.IsNullOrEmpty(strArgs) Then
                InitLib(strArgs.Split(" ").ToList, True)
                Await ObsluzCommandLine(strArgs)
                Window.Current.Close()
                Return
            End If
        End If

        ' jesli nie cmdline (a np. toast), albo cmdline bez parametrow, to pokazujemy okno
        Dim rootFrame As Frame = OnLaunchFragment(args.PreviousExecutionState)

        If args.Kind = ActivationKind.ToastNotification Then
            rootFrame.Navigate(GetType(MainPage))
        End If

        Window.Current.Activate()

    End Sub


    'Public Shared Function TimeSecToString(iSec As String) As String
    '    Dim oTS As TimeSpan = TimeSpan.FromSeconds(iSec)
    '    Dim sTxt As String = oTS.ToString("h\:mm\:ss")
    '    If oTS.Days > 0 Then
    '        If oTS.Hours < 10 Then sTxt = "0" & sTxt    ' jesli dajemy dni, to godziny z zerem
    '        sTxt = oTS.Days & "d " & sTxt
    '    End If
    '    Return sTxt
    'End Function

    'Public Shared Function FreqSlider2Text(iValue As Integer) As String
    '    Select Case iValue
    '        Case 0
    '            Return "  0 %"
    '        Case 1
    '            Return " 10 %"
    '        Case 2
    '            Return " 20 %"
    '        Case 3
    '            Return " 33 %"
    '        Case 4
    '            Return " 50 %"
    '        Case Else
    '            Return "100 %"
    '    End Select
    'End Function

    Public Shared miSessionFiles As Integer
    Public Shared miSessionMiB As Integer

    Public Shared mtGranyUtwor As Vblib.tGranyUtwor
    Public Shared moMediaPlayer As Windows.Media.Playback.MediaPlayer = Nothing

    Private Shared moRandom As Random = New Random
    Public Shared Function MakeRandom(iMax As Integer) As Integer
        ' Random = Windows.Security.Cryptography.CryptographicBuffer.GenerateRandomNumber()
        Return moRandom.Next(iMax + 1)
    End Function


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

    Public Shared Event PilotChce(sCmd As String)

    Private Async Function AppServiceLocalCommand(sCommand As String) As Task(Of String)
        Dim sResult As String = "ERROR while processing command " & sCommand

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
                    sOpis = mtGranyUtwor.oAudioParam.artist & " - " & mtGranyUtwor.oAudioParam.title
                    'oResultMsg.Add("name", CType(sOpis, String))

                    'Dim oSer As Xml.Serialization.XmlSerializer
                    'oSer = New Xml.Serialization.XmlSerializer(GetType(tGranyUtwor))
                    'Dim oStream As Stream = New MemoryStream
                    'oSer.Serialize(oStream, mtGranyUtwor)
                    'oStream.Flush()
                    'Dim oRdr As StreamReader = New StreamReader(oStream)
                    'oResultMsg.Add("granyUtwor", CType(oRdr.ReadToEnd, String))
                End If
            Case "pause", "next"
                RaiseEvent PilotChce(sCommand.ToLower)
                sResult = "OK"
            Case "stop"
            Case "start"
            Case Else
                sResult = "ERROR unknown command"

        End Select

        Return sResult
    End Function

    Public Shared gsLog As String = ""

    ' Public Shared goStorage As Storage_Base = Nothing
    'Public Shared goDbase As Vblib.dbase_base = Nothing

    Public Shared inVb As New Vblib.App(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path)

End Class



#Region "konwertery"
Public Class KonwersjaFrequency
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.Convert

        ' value is the data from the source object.

        Dim temp As Integer = CType(value, Integer)
        Select Case temp
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

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaPlayTime
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.Convert

        ' value is the data from the source object.

        Dim temp As Integer = CType(value, Integer)
        Return CLng(temp).ToStringDHMS()
    End Function

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

#End Region