Public Class Mapping
    Public mapBitmap As Bitmap
    Private DrawingSurface As Control

    Public Screen As ScreenArea
    Public MapViewPort As WorldViewPort
    Private Used_fixList As List(Of GPSFix)

    Public StartTime, StopTime As DateTime

    Public Sub New()

    End Sub

    Public Sub New(ByRef DrawingSurfaceCtrl As Control, ByVal fixList As List(Of GPSFix))
        DrawingSurface = DrawingSurfaceCtrl
        SetupMapBGraphics(DrawingSurface, fixList)
    End Sub

    Public Sub SetViewPort(ByVal vp As WorldViewPort)
        MapViewPort = vp.Clone()
    End Sub

    Public Sub SetupMapBGraphics(ByVal width As Integer, ByVal height As Integer, ByVal fixList As List(Of GPSFix), Optional ByVal viewPort As WorldViewPort = Nothing)
        mapBitmap = New Bitmap(width, height)
        Used_fixList = fixList
        Screen = New ScreenArea(width, height)
        If IsNothing(viewPort) Then viewPort = ZoomAll()
    End Sub

    Public Sub SetupMapBGraphics(ByVal target As Control, ByVal fixList As List(Of GPSFix), Optional ByVal viewPort As WorldViewPort = Nothing)
        mapBitmap = New Bitmap(target.Width, target.Height)
        Used_fixList = fixList
        Screen = New ScreenArea(target.Width, target.Height)
        If IsNothing(viewPort) Then
            MapViewPort = ZoomAll()
        Else
            MapViewPort = viewPort
        End If


    End Sub

    Public Function ZoomAll(Optional ByVal fixList As List(Of GPSFix) = Nothing) As WorldViewPort
        'search through all fixes, find top left fix and bottom right fix then do say 10% extra
        Dim Top As Double = -1000
        Dim Bottom As Double = 1000
        Dim Left As Double = 700
        Dim Right As Double = -100

        Dim topLeft, bottomRight As GlobePoint

        Dim lat, lon As Double

        Dim fix As GPSFix

        If IsNothing(fixList) Then fixList = Used_fixList

        For Each fix In fixList
            'If fix.Type = GPSFixType.Fix_Successful Or fix.Type = GPSFixType.Fix_Partial Then
            lat = fix.latitude
            lon = fix.longitude

            If lat > Top Then
                Top = lat
            ElseIf lat < Bottom Then
                Bottom = lat
            End If

            If lon < Left Then
                Left = lon
            ElseIf lon > Right Then
                Right = lon
            End If
            'End If
        Next

        Dim DeltaLat, DeltaLon As Double
        DeltaLat = Top - Bottom
        DeltaLon = Right - Left

        topLeft = New GlobePoint(Top + DeltaLat * 0.1, Left - DeltaLon * 0.1)
        bottomRight = New GlobePoint(Bottom - DeltaLat * 0.1, Right + DeltaLon * 0.1)





        Dim retVal As New WorldViewPort(topLeft, bottomRight)

        retVal.World_LengthX = retVal.World_HeightY * (Screen.Width / Screen.Height)

        Return retVal

    End Function

    Private Function ConvertWorldToScreenCoordinates(ByVal GP As GlobePoint, Optional ByVal viewPort As WorldViewPort = Nothing) As Point
        If IsNothing(viewPort) Then viewPort = MapViewPort
        Return GP.GetXYPositionOnScreen(viewPort, Screen)
    End Function

    Private Sub DrawMapBackground(ByRef m As Graphics, ByVal TopLeft As GlobePoint, ByVal BottomRight As GlobePoint, ByVal imgPath As String, Optional ByVal viewPort As WorldViewPort = Nothing)
        'If IsNothing(viewPort) Then viewPort = MapViewPort
        Dim scrTopLeft As Point = ConvertWorldToScreenCoordinates(TopLeft, viewPort)
        Dim scrBottomRight As Point = ConvertWorldToScreenCoordinates(BottomRight, viewPort)
        Dim width As Integer = scrBottomRight.X - scrTopLeft.X
        Dim height As Integer = scrBottomRight.Y - scrTopLeft.Y
        Dim destRect As New Rectangle(scrTopLeft, New Size(width, height))

        Dim tmpImg As New Bitmap(imgPath)
        m.DrawImage(tmpImg, destRect)

    End Sub

    Public Sub DrawMap(ByVal target As Bitmap, Optional ByVal viewPort As WorldViewPort = Nothing, Optional ByVal FixList As List(Of GPSFix) = Nothing)
        Dim m As Graphics = Graphics.FromImage(target)
        Dim h As Integer = target.Height
        Dim w As Integer = target.Width

        Dim fix As GPSFix
        Dim tmpGP As GlobePoint
        Dim tmpSP As Point

        Dim lastPnt As Point = Nothing

        If IsNothing(viewPort) Then viewPort = MapViewPort 'Else MapViewPort = viewPort
        If IsNothing(FixList) Then FixList = Used_fixList
        'Dim viewPort As WorldViewPort = ZoomAll(FixList)

        Dim scale As PointF = Screen.CalculateScale(viewPort)
        Dim col As Integer = 255
        Dim i As Integer = 0
        Dim tt As TimeSpan
        Dim moveDistPotential As Single
        m.Clear(Color.Black)
        'Prospero
        'DrawMapBackground(m, New GlobePoint(-27.578348, 153.277883), New GlobePoint(-27.584724, 153.290268), "E:\Wild Spy\Projects_New\WS007 - Bird GPS Logger\6 - Misc\Media For Koala Report\Sat BG + Scale + Site_3.png", viewPort)
        'Captain Bogard
        'DrawMapBackground(m, New GlobePoint(-27.230124, 152.992049), New GlobePoint(-27.251592, 153.029637), "E:\Wild Spy\Projects_New\WS007 - Bird GPS Logger\6 - Misc\Media for Lilia\Captain Bogard BG Sat.PNG", viewPort)

        'For Each fix In FixList.FixList
        '    If fix.GetFixTime >= StartTime AndAlso fix.GetFixTime <= StopTime Then
        '        If (fix.Type = GPSFixType.Fix_Partial Or fix.Type = GPSFixType.Fix_Successful) Then ' AndAlso fix.GetEHPE < 50 Then
        '            tt = StopTime - fix.GetFixTime
        '            tmpGP = New GlobePoint(fix.GetLatitude, fix.GetLongitude)
        '            tmpSP = tmpGP.GetXYPositionOnScreen(viewPort, Screen)

        '            moveDistPotential = tt.TotalSeconds * (0.01) 'koala max speed.. 0.1m/s made up number here... theoretical only

        '            'DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, fix.GetEHPE + moveDistPotential, scale)
        '            'DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, 0, scale)
        '            'DrawGradientAroundPoint(m, tmpSP.X, tmpSP.Y, fix.GetEHPE + moveDistPotential, scale.X)

        '            DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, fix.GetEHPE, scale)
        '            DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, 0, scale)

        '            If i > 0 Then
        '                m.DrawLine(Pens.Red, lastPnt, tmpSP)
        '            End If
        '            lastPnt = New Point(tmpSP.X, tmpSP.Y)

        '            'DrawGradientAroundPoint(m, tmpSP.X, tmpSP.Y, fix.GetEHPE, scale.X)
        '            'col = 255 * Math.Cos(i / (10 * 4)) '(255 / 10)

        '            'If col > 255 Then col = 255
        '            i += 1
        '        End If

        '    End If
        'Next

        'If i < 5 Then
        Dim ii As Integer
        For ii = FixList.Count - 1 To 0 Step -1
            fix = FixList(ii)
            'If fix.time > StartTime And fix.time < StopTime Then
            'If (fix.type = GPSFixType.Fix_Partial Or fix.type = GPSFixType.Fix_Successful) Then ' AndAlso fix.GetEHPE < 50 Then
            tt = StopTime - fix.time
            tmpGP = New GlobePoint(fix.latitude, fix.longitude)
            tmpSP = tmpGP.GetXYPositionOnScreen(viewPort, Screen)

            moveDistPotential = tt.TotalSeconds * (0.01) 'koala max speed.. 0.1m/s made up number here... theoretical only

            'DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, fix.GetEHPE + moveDistPotential, scale)
            'DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, 0, scale)
            'DrawGradientAroundPoint(m, tmpSP.X, tmpSP.Y, fix.GetEHPE + moveDistPotential, scale.X)

            'If i = 0 Then
            'DrawCircle(m, New Pen(Brushes.LightGreen, 3), tmpSP.X, tmpSP.Y, fix.GetEHPE, scale)
            'DrawCircle(m, New Pen(Brushes.LightGreen, 3), tmpSP.X, tmpSP.Y, fix.size, scale)
            'ElseIf i < 20 Then
            'DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, fix.GetEHPE, scale)
            DrawCircle(m, Pens.Gray, tmpSP.X, tmpSP.Y, fix.size, scale)
            'End If

            'DrawGradientAroundPoint(m, tmpSP.X, tmpSP.Y, fix.GetEHPE, scale.X)
            'col = 255 * Math.Cos(i / (10 * 4)) '(255 / 10)

            If i > 0 Then
                'm.DrawLine(New Pen(Brushes.Red, 3), lastPnt, tmpSP)
            End If
            lastPnt = New Point(tmpSP.X, tmpSP.Y)

            'If col > 255 Then col = 255
            i += 1
            'If i > 5 Then Exit For
            'End If
            'End If
        Next
        'End If
        'm.DrawRectangle(Pens.White, Screen.Width - 10, 10, 5, Screen.Height - 20)
        m.FillRectangle(Brushes.White, 7, Screen.Height - 21, 130, 16)
        m.DrawString(StopTime.ToString("dd/MM/yyyy HH:mm:ss"), New Font("Arial", 10), Brushes.Black, New PointF(10, Screen.Height - 20))

    End Sub

    Public Sub DrawMapGrad(ByVal target As Bitmap, Optional ByVal viewPort As WorldViewPort = Nothing, Optional ByVal FixList As List(Of GPSFix) = Nothing)
        Dim m As Graphics = Graphics.FromImage(target)
        Dim h As Integer = target.Height
        Dim w As Integer = target.Width

        Dim fix As GPSFix
        Dim tmpGP As GlobePoint
        Dim tmpSP As Point

        If IsNothing(viewPort) Then viewPort = MapViewPort
        If IsNothing(FixList) Then FixList = Used_fixList
        'Dim viewPort As WorldViewPort = ZoomAll(FixList)

        Dim scale As PointF = Screen.CalculateScale(viewPort)
        Dim col As Integer = 255
        Dim tt As TimeSpan
        Dim moveDistPotential As Single = 0
        Dim circList As New List(Of Circle)
        'm.Clear(Color.Black)

        Dim i, j As Integer
        'going based on stoptime and the fix before it.
        For i = 0 To FixList.Count - 1
            fix = FixList(i)
            If fix.time >= StopTime Then

                tt = fix.time - StopTime
                tmpGP = New GlobePoint(fix.latitude, fix.longitude)
                tmpSP = tmpGP.GetXYPositionOnScreen(viewPort, Screen)

                'moveDistPotential = tt.TotalSeconds * (0.02) 'koala max speed.. 0.1m/s made up number here... theoretical only

                'circList.Add(New Circle(tmpSP.X, tmpSP.Y, (fix.GetEHPE + moveDistPotential) * scale.X))
                circList.Add(New Circle(tmpSP.X, tmpSP.Y, (fix.size + moveDistPotential) * scale.X))

                ''find the last successful fix.
                'Do
                '    i -= 1
                '    fix = FixList.FixList(i)
                'Loop While fix.Type = GPSFixType.Fix_Failed

                'tt = StopTime - fix.GetFixTime
                'tmpGP = New GlobePoint(fix.GetLatitude, fix.GetLongitude)
                'tmpSP = tmpGP.GetXYPositionOnScreen(viewPort, Screen)

                ''moveDistPotential = tt.TotalSeconds * (0.02) 'koala max speed.. 0.1m/s made up number here... theoretical only

                'circList.Add(New Circle(tmpSP.X, tmpSP.Y, (fix.GetEHPE + moveDistPotential) * scale.X))
                Exit For
            End If
        Next

        'For Each fix In FixList.FixList
        '    If fix.GetFixTime >= StartTime AndAlso fix.GetFixTime <= StopTime Then
        '        If (fix.Type = GPSFixType.Fix_Partial Or fix.Type = GPSFixType.Fix_Successful) Then ' AndAlso fix.GetEHPE < 50 Then
        '            tt = StopTime - fix.GetFixTime
        '            tmpGP = New GlobePoint(fix.GetLatitude, fix.GetLongitude)
        '            tmpSP = tmpGP.GetXYPositionOnScreen(viewPort, Screen)

        '            moveDistPotential = tt.TotalSeconds * (0.01) 'koala max speed.. 0.1m/s made up number here... theoretical only

        '            circList.Add(New Circle(tmpSP.X, tmpSP.Y, (fix.GetEHPE + moveDistPotential) * scale.X))
        '        End If
        '    End If
        'Next



        Dim x, y As Integer
        Dim alpha As Integer
        Dim fp As New FastPixel(target)
        fp.Lock()
        'fp.Clear(Color.FromArgb(20, 0, 0, 255))
        For x = 0 To Screen.Width - 1
            For y = 0 To Screen.Height - 1
                col = CalculatePDatPoint(x, y, circList) * 256 / 2
                If col > 255 Then col = 255
                fp.SetPixel(x, y, Color.FromArgb((col) / 2, 0, col, 255))
            Next
        Next

        'draw legend
        For x = Screen.Width - 10 To Screen.Width - 5
            For y = 10 To Screen.Height - 10
                col = ((y - 10) / (Screen.Height - 20)) * 255
                'max (white) = PD=0.5/m^2
                'min (black) = PD=0/m^2
                fp.SetPixel(x, y, Color.FromArgb(0, col, 255))
            Next
        Next

        fp.Unlock(True)

        'm = Graphics.FromImage(target)
        m.DrawRectangle(Pens.White, Screen.Width - 10, 10, 5, Screen.Height - 20)
        m.FillRectangle(Brushes.White, 7, Screen.Height - 21, 110, 16)
        m.DrawString(StopTime.ToString("dd/MM/yyyy HH:mm"), New Font("Arial", 10), Brushes.Black, New PointF(10, Screen.Height - 20))

    End Sub

    Public Function DistBetweenPoints(ByVal p1X As Double, ByVal p1Y As Double, ByVal p2X As Double, ByVal p2Y As Double) As Double
        Return Math.Sqrt((p1X - p2X) ^ 2 + (p1Y - p2Y) ^ 2)
    End Function


    Private Function CalculatePDatPoint(ByVal x_scr As Double, ByVal y_scr As Double, ByVal pointlist As List(Of Circle)) As Single
        Dim sum As Single = 1
        Dim i As Integer

        For i = 0 To pointlist.Count - 1
            If pointlist(i).radius < 100 Then
                sum *= CalculateProbablityDensityArountPoint(DistBetweenPoints(x_scr, y_scr, pointlist(i).x, pointlist(i).y), pointlist(i).radius)
                If Single.IsInfinity(sum) Then Return 1000000
            End If

        Next

        Return sum / pointlist.Count

    End Function

    Private Function CalculateRadiusAroundPointWithProbabilityDensity(ByVal contourIndex As Integer, ByVal ehpe As Single) As PointF
        Dim rpList() As Single = {0.0564573, 0.0688814, 0.0837285, 0.101384, 0.122287, 0.14695, 0.175988, 0.210171, 0.250491, 0.298283, 0.355387, 0.424397, 0.509011, 0.614459, 0.747916, 0.918595, 1.1373}
        Dim PDList() As Single = {10, 8, 6.4, 5.12, 4.096, 3.2768, 2.62144, 2.097152, 1.6777216, 1.34217728, 1.073741824, 0.858993459, 0.687194767, 0.549755814, 0.439804651, 0.351843721, 0.281474977}

        'probably need a better solution to this then a lookup table.  Need to be able to calculate for different values of c..

        Return New PointF(PDList(contourIndex), rpList(contourIndex) * ehpe)

    End Function

    Private Function CalculateProbablityDensityArountPoint(ByVal r As Single, ByVal ehpe As Single) As Single
        Dim rp As Single = r / ehpe
        Dim a As Single
        Dim c As Single = 0.22
        Dim rpList() As Single = {0.0564573, 0.0688814, 0.0837285, 0.101384, 0.122287, 0.14695, 0.175988, 0.210171, 0.250491, 0.298283, 0.355387, 0.424397, 0.509011, 0.614459, 0.747916, 0.918595, 1.1373}
        Dim PDList() As Single = {10, 8, 6.4, 5.12, 4.096, 3.2768, 2.62144, 2.097152, 1.6777216, 1.34217728, 1.073741824, 0.858993459, 0.687194767, 0.549755814, 0.439804651, 0.351843721, 0.281474977}

        a = 1 / Math.PI * ((1 / rp) + (Math.Exp(-rp / c) / rp) + 0) '+ (alglib.exponentialintegralei(-rp/c)/c)) 'instead of +0)
        Return a
    End Function

    Private Sub DrawGradientAroundPoint(ByRef m As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal radius As Integer, ByVal scale As Single)
        'calculate a gradient
        'Dim gradpts As Integer = radius * scale
        'Dim grad As Single
        Dim i As Integer
        Dim col As Integer
        Dim DensityInfo As PointF

        For i = 0 To 15
            DensityInfo = CalculateRadiusAroundPointWithProbabilityDensity(i, radius)
            'col = ((10 - DensityInfo.X) / 10) * 255
            'col = ((DensityInfo.X) / 10) * 255
            col = (1 - (i / 15)) * 255
            If col > 255 Then col = 255
            DrawCircle(m, New Pen(Color.FromArgb(col, 0, 255, 0)), x, y, DensityInfo.Y * scale)
        Next

        'For i = 1 To gradpts / 5 Step 5 'Step gradpts / 20
        '    grad = CalculateProbablityDensityArountPoint(i / scale, radius)
        '    'col = (1 - (i / gradpts)) * 255
        '    col = grad
        '    If col > 255 Then col = 255
        '    DrawCircle(m, New Pen(Color.FromArgb(col, col, col)), x, y, i)
        'Next

    End Sub

    Private Sub DrawCircle(ByRef m As Graphics, ByVal pen As Drawing.Pen, ByVal x As Integer, ByVal y As Integer, ByVal radius As Integer)
        Dim top, left, length As Integer

        length = 2 * radius
        top = y - radius
        left = x - radius

        m.DrawEllipse(pen, left, top, length, length)
    End Sub


    Private Sub DrawCircle(ByRef m As Graphics, ByVal pen As Drawing.Pen, ByVal x As Integer, ByVal y As Integer, ByVal radius As Integer, ByVal scale As PointF)
        Dim top, left, width, height As Integer

        width = 2 * radius * scale.X
        height = 2 * radius * scale.Y

        If width < 1 Or height < 1 Then
            m.DrawEllipse(pen, x - 1, y - 1, 2, 2)
        Else
            top = y - height / 2
            left = x - width / 2
            m.DrawEllipse(pen, left, top, width, height)
        End If


    End Sub

    Private Sub copyGraphics(ByVal source As Bitmap, ByVal target As Control)
        'Dim sG As System.Drawing.Graphics = Graphics.FromImage(source)
        Dim tG As System.Drawing.Graphics = target.CreateGraphics()
        tG.DrawImage(source, 0, 0)
    End Sub

    Private Sub UpdateControl(Optional ByVal target As Control = Nothing)
        If IsNothing(target) Then
            target = DrawingSurface
        End If
        copyGraphics(mapBitmap, target)
    End Sub

    Public Sub DrawMapToControl(Optional ByVal viewport As WorldViewPort = Nothing)

        If IsNothing(viewport) Then viewport = MapViewPort

        DrawMap(mapBitmap, viewport, Used_fixList)
        copyGraphics(mapBitmap, DrawingSurface)
    End Sub

    Public Sub DrawGradMapToControl(Optional ByVal viewport As WorldViewPort = Nothing)

        If IsNothing(viewport) Then viewport = MapViewPort

        'DrawMap(mapBitmap, viewport, Used_fixList)
        DrawMapGrad(mapBitmap, viewport, Used_fixList)
        copyGraphics(mapBitmap, DrawingSurface)
    End Sub
End Class

Public Class GlobePoint
    Implements ICloneable
    Public lat As Double
    Public lon As Double

    Public MercatorX As Double
    Public MercatorY As Double

    Public Sub New()

    End Sub

    Public Sub New(ByVal Mercator As Boolean, ByVal LatitudeOrX As Double, ByVal LongitudeOrY As Double)
        If Mercator = True Then
            MercatorX = LatitudeOrX
            MercatorY = LongitudeOrY
            MercatorToLatLon()
        Else
            lat = LatitudeOrX
            lon = LongitudeOrY
            LatLonToMercator()
        End If

    End Sub

    Public Sub New(ByVal latitude As Double, ByVal longitude As Double)
        lat = latitude
        lon = longitude
        LatLonToMercator()
    End Sub

    Public Sub SetMercatorXY(ByVal Mercator_X As Double, ByVal MerCator_Y As Double)
        MercatorX = Mercator_X
        MercatorY = MerCator_Y
        MercatorToLatLon()
    End Sub

    Public Sub LatLonToMercator()
        ' Ellipsoid model constants (actual values here are for WGS84) 
        Const sm_a As Double = 6378137.0
        'Const sm_b As Single = 6356752.314
        Const lon0Rad As Double = 0

        Dim latRad As Double = lat * Math.PI / 180
        Dim lonRad As Double = lon * Math.PI / 180
        MercatorX = sm_a * (lonRad - lon0Rad)
        'MercatorY = sm_a * Math.Log((Math.Sin(latRad) + 1) / Math.Cos(latRad))
        MercatorY = sm_a / 2 * Math.Log((1 + Math.Sin(latRad)) / (1 - Math.Sin(latRad)))
    End Sub

    Public Sub MercatorToLatLon()
        ' Ellipsoid model constants (actual values here are for WGS84) 
        Const sm_a As Double = 6378137.0
        'Const sm_b As Single = 6356752.314
        Const lon0 As Double = 0

        lon = (MercatorX + lon0) * 180 / (sm_a * Math.PI)
        'lon = Math.Atan((Math.Exp(2 * MercatorY / sm_a) - 1) / 2) * 180 / (Math.PI)
        'lon = Math.Asin(1 - 2 / (Math.Exp(sm_a / 2 * MercatorY) + 1)) * 180 / Math.PI
        lat = Math.Atan(Math.Sinh(MercatorY / sm_a)) * 180 / Math.PI
    End Sub

    'All X/Y values in meters
    Public Function GetXYPositionOnScreen(ByVal vp As WorldViewPort, ByVal Scr As ScreenArea) As Point
        Dim viewPortX, viewPortY As Double
        Dim ScrX, ScrY As Integer
        Dim scale As PointF = Scr.CalculateScale(vp)


        viewPortX = MercatorX - vp.World_OriginX
        viewPortY = -(MercatorY - vp.World_OriginY)

        ScrX = viewPortX * scale.X
        ScrY = viewPortY * scale.Y

        Return New Point(ScrX, ScrY)

    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim cl As New GlobePoint()
        cl.lat = lat
        cl.lon = lon
        cl.MercatorX = MercatorX
        cl.MercatorY = MercatorY
        Return cl
    End Function
End Class

Public Class WorldViewPort
    Implements ICloneable
    Public World_OriginX As Double
    Public World_OriginY As Double
    Public World_LengthX As Single
    Public World_HeightY As Single

    Public Sub New()

    End Sub

    Public Sub New(ByVal W_Origin_X As Double, ByVal W_Origin_Y As Double, ByVal W_Length_X As Double, ByVal W_Height_Y As Double)
        World_OriginX = W_Origin_X
        World_OriginY = W_Origin_Y
        World_LengthX = W_Length_X
        World_HeightY = W_Height_Y
    End Sub

    Public Sub New(ByVal topLeft As GlobePoint, ByVal bottomRight As GlobePoint)
        World_OriginX = topLeft.MercatorX
        World_OriginY = topLeft.MercatorY
        World_LengthX = bottomRight.MercatorX - topLeft.MercatorX
        World_HeightY = topLeft.MercatorY - bottomRight.MercatorY
    End Sub

    Public Sub ChangeWorldViewPort(ByVal W_Origin_X As Double, ByVal W_Origin_Y As Double, ByVal W_Length_X As Double, ByVal W_Height_Y As Double)
        World_OriginX = W_Origin_X
        World_OriginY = W_Origin_Y
        World_LengthX = W_Length_X
        World_HeightY = W_Height_Y
    End Sub

    Public Sub ChangeWorldViewPortSize(ByVal W_Length_X As Double, ByVal W_Height_Y As Double)
        World_LengthX = W_Length_X
        World_HeightY = W_Height_Y
    End Sub

    Public Sub ChangeWorldViewPortPosition(ByVal W_Origin_X As Double, ByVal W_Origin_Y As Double)
        World_OriginX = W_Origin_X
        World_OriginY = W_Origin_Y
    End Sub

    Public Sub Zoom(ByVal ratio As Single)
        Dim offsetX, offsetY As Double
        Dim NewLen, NewHeight As Double

        NewLen = World_LengthX * ratio
        NewHeight = World_HeightY * ratio

        offsetX = (NewLen - World_LengthX) / 2
        offsetY = (NewHeight - World_HeightY) / 2

        World_LengthX = NewLen
        World_HeightY = NewHeight

        World_OriginX -= offsetX
        World_OriginY += offsetY

    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New WorldViewPort(World_OriginX, World_OriginY, World_LengthX, World_HeightY)
    End Function
End Class

Public Class Circle
    Public x As Single
    Public y As Single
    Public radius As Single

    Public Sub New()

    End Sub

    Public Sub New(ByVal _x As Single, ByVal _y As Single, ByVal _radius As Single)
        x = _x
        y = _y
        radius = _radius
    End Sub

End Class

Public Class ScreenArea
    Implements ICloneable
    Public Width As Integer
    Public Height As Integer

    Public Sub New()

    End Sub

    Public Sub New(ByVal Scr_Length As Integer, ByVal Scr_Height As Integer)
        Width = Scr_Length
        Height = Scr_Height
    End Sub

    Public Function CalculateScale(ByVal vp As WorldViewPort) As PointF
        Dim scaleX, scaleY As Single
        scaleX = Width / vp.World_LengthX
        scaleY = Height / vp.World_HeightY

        Return New PointF(scaleX, scaleY)

    End Function

    Public Sub ChangeScreenSize(ByVal Scr_Length As Integer, ByVal Scr_Height As Integer)
        Width = Scr_Length
        Height = Scr_Height
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New ScreenArea(Width, Height)
    End Function
End Class

Public Enum GPSFixType
    GPSFix
    Location
    Other
    Unknown
End Enum

Public Class GPSFix
    Public LoggerID As Integer
    Public name As String
    Public time As DateTime
    Public longitude As Double
    Public latitude As Double
    Public altitude As Double
    Public type As GPSFixType = GPSFixType.Unknown
    Public size As Integer = 1
End Class