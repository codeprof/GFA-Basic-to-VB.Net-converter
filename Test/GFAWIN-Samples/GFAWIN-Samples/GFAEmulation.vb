'GFA Emualtionsmodul
'-------------------
'Zuletzt bearbeitet: 2010-11-01
'2010-09-15: Verbesserungen mit Try/Fianlly
'2010-09-15: Verbesserungen an _GET/_PUT
'2010-09-18: __X,__Y verbessert, Designerproblem behoben
'2010-09-18: _REGISTERWIN hinzugefügt
'2010-09-24: Umbau der kompletten Zeichenbefehle / _DEFFILL hinzugefügt 
Imports Microsoft.VisualBasic
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
#Region "Wichtige Hinweise"
'##########################################################################################
'1. Hinweis
'##########################################################################################
'"<DebuggerHidden()> _" vor Funktionen hinzufügen (nur wenn sicher Bugfrei)
'Bzw. "<DebuggerNonUserCode()> _" vor das Modul, verhindert, das der Debugger in den
'Code der Emulationsfunktionen springt.
'Um Emulationsfunktionen zu debuggen muss zunächst "<DebuggerNonUserCode()> _" vor dem
'Modulanfang entfernt werden! 
'##########################################################################################
'2. Hinweis
'##########################################################################################
' in den Kompileroptionen sollt immer die 
' Option "Überprüfungen auf Ganzzahlenüberlauf entfernen"
' angehakt sein, damit sich das Programm wie in GFA-Basic verhält
'##########################################################################################
'3. Hinweis
'##########################################################################################
'Ein Fenster mit Steuerelemente unterstützt keine Scancodes (wie z.B. Up,Down,Left,Right) im  KeyDown Event.
'In diesem Fall muss die Methode ProcessCmdKey mit der folgenden überschrieben werden:

'Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
    'Dim key As String = ""
    'Dim scancode As Integer = 0

    'Select Case keyData
    'Case Keys.F1
    'key = "p;"
    'Case Keys.F2
    'key = "q<"
    'Case Keys.F3
    'key = "r="
    'Case Keys.F4
    'key = "s>"
    'Case Keys.F5
    'key = "t?"
    'Case Keys.F6
    'key = "u@"
    'Case Keys.F7
    'key = "vA"
    'Case Keys.F8
    'key = "wB"
    'Case Keys.F9
    'key = "xC"

    'Case Keys.Home 'Pos1
    'key = "$G"
    'Case Keys.PageUp 'BildUp
    'key = "!I"
    'Case Keys.PageDown 'BildDown
    'key = Chr(34) + "Q"
    'Case Keys.Insert
    'key = "-R"
    'Case Keys.Delete
    'key = ".S"
    'Case Keys.End
    'key = "#O"

    '''Wird bereits von KeyPress gehandelt
    '''Case Keys.NumPad0
    '''    GFAWindows.INKEY = "0"
    '''Case Keys.NumPad1
    '''    GFAWindows.INKEY = "1"
    '''Case Keys.NumPad2
    '''    GFAWindows.INKEY = "2"
    '''Case Keys.NumPad3
    '''    GFAWindows.INKEY = "3"
    '''Case Keys.NumPad4
    '''    GFAWindows.INKEY = "4"
    '''Case Keys.NumPad5
    '''    GFAWindows.INKEY = "5"
    '''Case Keys.NumPad6
    '''    GFAWindows.INKEY = "6"
    '''Case Keys.NumPad7
    '''    GFAWindows.INKEY = "7"
    '''Case Keys.NumPad8
    '''    GFAWindows.INKEY = "8"
    '''Case Keys.NumPad9
    '''    GFAWindows.INKEY = "9"

    'Case Keys.Left
    'key = "%K"
    'Case Keys.Right
    'key = "'M"
    'Case Keys.Up
    'key = "&H"
    'Case Keys.Down
    'key = "(P"
    'End Select

    'If key <> "" Then
    'GFAWindows.INKEY = key
    'If Me.IsDisposed = False Then
    'scancode = DirectCast(keyData, Integer)
    'GFAWindows.AddEvent(Me.Handle, 1, WM_KEYDOWN, scancode, 1) 'Nicht gleich wie bei Windows API!
    'End If
    'End If
    'Return MyBase.ProcessCmdKey(msg, keyData) 'False
    'End Function
    '##########################################################################################
#End Region
'Die folgende Zeile auskommentieren, um Code zu Debuggen
'<DebuggerNonUserCode()> _
Module GFAEmulation
#Region "Globale Konstanten"
    Public Const E = 2.7182818284590451
    Public Const PI = 3.1415926535897931
#End Region
#Region "Win32-API"
    Public Const VK_LBUTTON = 1
    Public Const VK_RBUTTON = 2
    Public Const VK_MBUTTON = 4

    Public Const IDI_ERROR = 32513
    Public Const IDI_WARNING = 32515
    Public Const IDI_QUESTION = 32514

    Public Const SW_ERASE = 4
    Public Const SW_HIDE = 0
    Public Const SW_INVALIDATE = 2
    Public Const SW_MAX = 10
    Public Const SW_MAXIMIZE = 3
    Public Const SW_MINIMIZE = 6
    Public Const SW_NORMAL = 1
    Public Const SW_OTHERUNZOOM = 4
    Public Const SW_OTHERZOOM = 2
    Public Const SW_PARENTCLOSING = 1
    Public Const SW_PARENTOPENING = 3
    Public Const SW_RESTORE = 9
    Public Const SW_SCROLLCHILDREN = 1
    Public Const SW_SHOW = 5
    Public Const SW_SHOWDEFAULT = 10
    Public Const SW_SHOWMAXIMIZED = 3
    Public Const SW_SHOWMINIMIZED = 2
    Public Const SW_SHOWMINNOACTIVE = 7
    Public Const SW_SHOWNA = 8
    Public Const SW_SHOWNOACTIVATE = 4
    Public Const SW_SHOWNORMAL = 1

    Public Const MB_ICONASTERISK = 64
    Public Const MB_ICONHAND = 16
    Public Const MB_OK = 0
    Public Const MB_ICONQUESTION = 32
    Public Const MB_ICONEXCLAMATION = 48

    Public Const WM_CLOSE = 16
    Public Const WM_PAINT = 15
    Public Const WM_CHAR = 258
    Public Const WM_NULL = 0
    Public Const WM_LBUTTONDOWN = 513
    Public Const WM_LBUTTONUP = 514
    Public Const WM_LBUTTONDBLCLK = 515
    Public Const WM_RBUTTONDBLCLK = 518
    Public Const WM_RBUTTONDOWN = 516
    Public Const WM_RBUTTONUP = 517
    Public Const WM_COMMAND = 273
    Public Const WM_SIZE = 5
    Public Const WM_KEYDOWN = 256
    Public Const WM_KEYUP = 257

    Public Const SIZE_MAXHIDE = 4
    Public Const SIZE_MAXIMIZED = 2
    Public Const SIZE_MAXSHOW = 3
    Public Const SIZE_MINIMIZED = 1
    Public Const SIZE_RESTORED = 0

    Public Const SIZEICONIC = 1
    Public Const SIZEFULLSCREEN = 2

    'Fensterstyles
    Public Const WS_BORDER = 8388608
    Public Const WS_CAPTION = 12582912
    Public Const WS_CHILD = 1073741824
    Public Const WS_CHILDWINDOW = 1073741824
    Public Const WS_CLIPCHILDREN = 33554432
    Public Const WS_CLIPSIBLINGS = 67108864
    Public Const WS_DISABLED = 134217728
    Public Const WS_DLGFRAME = 4194304
    Public Const WS_GROUP = 131072
    Public Const WS_HSCROLL = 1048576
    Public Const WS_ICONIC = 536870912
    Public Const WS_MAXIMIZE = 16777216
    Public Const WS_MAXIMIZEBOX = 65536
    Public Const WS_MINIMIZE = 536870912
    Public Const WS_MINIMIZEBOX = 131072
    Public Const WS_OVERLAPPED = 0
    Public Const WS_OVERLAPPEDWINDOW = 13565952
    Public Const WS_POPUP = 2147483648
    Public Const WS_POPUPWINDOW = 2156396544
    Public Const WS_SIZEBOX = 262144
    Public Const WS_SYSMENU = 524288
    Public Const WS_TABSTOP = 65536
    Public Const WS_THICKFRAME = 262144
    Public Const WS_TILED = 0
    Public Const WS_TILEDWINDOW = 13565952
    Public Const WS_VISIBLE = 268435456
    Public Const WS_VSCROLL = 2097152

    'Konstanten für SetWindowLong
    Public Const GWLP_HINSTANCE = -6
    Public Const GWLP_HWNDPARENT = -8
    Public Const GWLP_ID = -12
    Public Const GWLP_USERDATA = -21
    Public Const GWLP_WNDPROC = -4
    Public Const GWL_STYLE = -16
    Public Const GWL_ID = -12
    Public Const GWL_HWNDPARENT = -8
    Public Const GWL_HINSTANCE = -6
    Public Const GWL_EXSTYLE = -20
    Public Const GWL_USERDATA = -21
    Public Const GWL_WNDPROC = -4

    'Konstanten für _DLG_COLOR
    Public Const CC_RGBINIT = 1
    Public Const CC_FULLOPEN = 2
    Public Const CC_ANYCOLOR = 256
    Public Const CC_PREVENTFULLOPEN = 4
    Public Const CC_SOLIDCOLOR = 128
    Public Const CC_SHOWHELP = 8

    Public Const SWP_ASYNCWINDOWPOS = 16384
    Public Const SWP_DEFERERASE = 8192
    Public Const SWP_DRAWFRAME = 32
    Public Const SWP_FRAMECHANGED = 32
    Public Const SWP_HIDEWINDOW = 128
    Public Const SWP_NOACTIVATE = 16
    Public Const SWP_NOCOPYBITS = 256
    Public Const SWP_NOMOVE = 2
    Public Const SWP_NOOWNERZORDER = 512
    Public Const SWP_NOREDRAW = 8
    Public Const SWP_NOREPOSITION = 512
    Public Const SWP_NOSENDCHANGING = 1024
    Public Const SWP_NOSIZE = 1
    Public Const SWP_NOZORDER = 4

    Public Const SB_BOTH = 3
    Public Const SB_BOTTOM = 7
    Public Const SB_CTL = 2
    Public Const SB_HORZ = 0
    Public Const SB_VERT = 1

    'Zwischenablageformate
    Public Const CF_BITMAP = 2
    Public Const CF_TEXT = 1

    'Menü
    Public Const MF_MENUBARBREAK = 32
    Public Const MF_CHECKED = 8

    ' =========================== comdlg32.dll ===========================

    'Konstanten für Druckerdialog
    Public Const PD_ALLPAGES = 0
    Public Const PD_COLLATE = 16
    Public Const PD_DISABLEPRINTTOFILE = 524288
    Public Const PD_ENABLEPRINTHOOK = 4096
    Public Const PD_ENABLEPRINTTEMPLATE = 16384
    Public Const PD_ENABLEPRINTTEMPLATEHANDLE = 65536
    Public Const PD_ENABLESETUPHOOK = 8192
    Public Const PD_ENABLESETUPTEMPLATE = 32768
    Public Const PD_ENABLESETUPTEMPLATEHANDLE = 131072
    Public Const PD_HIDEPRINTTOFILE = 1048576
    Public Const PD_NOPAGENUMS = 8
    Public Const PD_NOSELECTION = 4
    Public Const PD_NOWARNING = 128
    Public Const PD_PAGENUMS = 2
    Public Const PD_PRINTSETUP = 64
    Public Const PD_PRINTTOFILE = 32
    Public Const PD_RETURNDC = 256
    Public Const PD_RETURNDEFAULT = 1024
    Public Const PD_RETURNIC = 512
    Public Const PD_SELECTION = 1
    Public Const PD_SHOWHELP = 2048
    Public Const PD_USEDEVMODECOPIES = 262144
    'Konstanten für Fontdialog
    Public Const CF_SCALABLEONLY = 131072
    Public Const CF_EFFECTS = 256
    Public Const CF_TTONLY = 262144
    Public Const CF_SCREENFONTS = 1

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure _PRINTDLGX86
    Dim lStructSize As Int32
    Dim hwndOwner As IntPtr
    Dim hDevMode As IntPtr
    Dim hDevNames As IntPtr
    Dim hDC As IntPtr
    Dim Flags As Int32
    Dim FromPage As Int16
    Dim ToPage As Int16
    Dim MinPage As Int16
    Dim MaxPage As Int16
    Dim Copies As Int16
    Dim hInstance As IntPtr
    Dim lCustData As IntPtr
    Dim lpfnPrintHook As IntPtr
    Dim lpfnSetupHook As IntPtr
    Dim lpPrintTemplateName As IntPtr
    Dim lpSetupTemplateName As IntPtr
    Dim hPrintTemplate As IntPtr
    Dim hSetupTemplate As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Structure _PRINTDLGX64
        Dim lStructSize As Int32
        Dim hwndOwner As IntPtr
        Dim hDevMode As IntPtr
        Dim hDevNames As IntPtr
        Dim hDC As IntPtr
        Dim Flags As Int32
        Dim FromPage As Int16
        Dim ToPage As Int16
        Dim MinPage As Int16
        Dim MaxPage As Int16
        Dim Copies As Int16
        Dim hInstance As IntPtr
        Dim lCustData As IntPtr
        Dim lpfnPrintHook As IntPtr
        Dim lpfnSetupHook As IntPtr
        Dim lpPrintTemplateName As IntPtr
        Dim lpSetupTemplateName As IntPtr
        Dim hPrintTemplate As IntPtr
        Dim hSetupTemplate As IntPtr
    End Structure

    Public Declare Unicode Function PrintDlg Lib "comdlg32.dll" Alias "PrintDlgW" (ByRef lppd As _PRINTDLGX64) As Integer
    Public Declare Unicode Function PrintDlg Lib "comdlg32.dll" Alias "PrintDlgW" (ByRef lppd As _PRINTDLGX86) As Integer

    ' =========================== USER32.DLL ===========================
    'Von Alert verwendete Funktionen
    'Public Declare Function MessageBeep Lib "user32.dll" Alias "MessageBeep" (ByVal style As Integer) As Integer
    'Public Declare Function LoadIcon Lib "user32.dll" Alias "LoadIconA" (ByVal hInstance As IntPtr, ByVal name As Integer) As IntPtr

    Public Declare Function ShowWindow Lib "user32.dll" Alias "ShowWindow" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Integer
    Public Declare Function GetAsyncKeyState Lib "user32.dll" Alias "GetAsyncKeyState" (ByVal nVirtKey As Integer) As Integer
    Public Declare Function ScrollWindowEx Lib "user32.dll" Alias "ScrollWindowEx" (ByVal hWnd As IntPtr, ByVal dx As Integer, ByVal dy As Integer, ByVal prcScroll As Rectangle, ByVal prcClip As Rectangle, ByVal hrgnUpdate As IntPtr, ByRef prcUpdate As Rectangle, ByVal flags As Integer) As Integer
    Public Declare Function BringWindowToTop Lib "user32.dll" Alias "BringWindowToTop" (ByVal hWnd As IntPtr) As Integer
    Public Declare Function UpdateWindow Lib "user32.dll" Alias "UpdateWindow" (ByVal hWnd As IntPtr) As Integer
    Public Declare Function SetWindowPos Lib "user32.dll" Alias "SetWindowPos" (ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As Integer) As Integer
    Public Declare Function ShowScrollBar Lib "user32.dll" Alias "ShowScrollBar" (ByVal hWnd As IntPtr, ByVal wBar As Integer, ByVal bShow As Boolean) As Integer
    Public Declare Function SetCapture Lib "user32.dll" Alias "SetCapture" (ByVal hWnd As IntPtr) As Integer
    Public Declare Function ReleaseCapture Lib "user32.dll" Alias "ReleaseCapture" () As Integer
    ' es gibt zumindest unter 32-Bit keine SetWindowLongPtr funktion (evtl. Problematisch bei 64Bit!!!)
    Public Declare Unicode Function SetWindowLong Lib "user32.dll" Alias "SetWindowLongW" (ByVal hWnd As IntPtr, ByVal index As Integer, ByVal newValue As Integer) As Integer

    ' =========================== GDI32.DLL ===========================

    'Konstanten für GetDeviceCaps
    Public Const HORZRES = 8
    Public Const VERTRES = 10
    Public Const VERTSIZE = 6
    Public Const HORZSIZE = 4
    Public Const NUMCOLORS = 24

    'Konstanten für GetStockObject
    Public Const BLACK_BRUSH = 4
    Public Const BLACK_PEN = 7
    Public Const WHITE_BRUSH = 0
    Public Const WHITE_PEN = 6
    Public Const DKGRAY_BRUSH = 3
    Public Const GRAY_BRUSH = 2
    Public Const HOLLOW_BRUSH = 5
    Public Const NULL_PEN = 8
    Public Const LTGRAY_BRUSH = 1
    Public Const DEFAULT_PALETTE = 15
    Public Const NULL_BRUSH = 5
    Public Const SYSTEM_FONT = 13
    Public Const SYSTEM_FIXED_FONT = 16
    Public Const ANSI_FIXED_FONT = 11
    Public Const ANSI_VAR_FONT = 12
    Public Const DEVICE_DEFAULT_FONT = 14
    Public Const OEM_FIXED_FONT = 10
    Public Const DEFAULT_GUI_FONT = 17

    'Konstanten für GetObjectType
    Public Const OBJ_BITMAP = 7
    Public Const OBJ_BRUSH = 2
    Public Const OBJ_DC = 3
    Public Const OBJ_ENHMETADC = 12
    Public Const OBJ_ENHMETAFILE = 13
    Public Const OBJ_EXTPEN = 11
    Public Const OBJ_FONT = 6
    Public Const OBJ_MEMDC = 10
    Public Const OBJ_METADC = 4
    Public Const OBJ_METAFILE = 9
    Public Const OBJ_PAL = 5
    Public Const OBJ_PEN = 1
    Public Const OBJ_REGION = 8

    'Konstanten für SetBkMode
    Public Const TRANSPARENT = 1
    Public Const OPAQUE = 2

    'Konstanten für SetROP2
    Public Const R2_BLACK = 1
    Public Const R2_COPYPEN = 13
    Public Const R2_LAST = 16
    Public Const R2_MASKNOTPEN = 3
    Public Const R2_MASKPEN = 9
    Public Const R2_MASKPENNOT = 5
    Public Const R2_MERGENOTPEN = 12
    Public Const R2_MERGEPEN = 15
    Public Const R2_MERGEPENNOT = 14
    Public Const R2_NOP = 11
    Public Const R2_NOT = 6
    Public Const R2_NOTCOPYPEN = 4
    Public Const R2_NOTMASKPEN = 8
    Public Const R2_NOTMERGEPEN = 2
    Public Const R2_NOTXORPEN = 10
    Public Const R2_WHITE = 16
    Public Const R2_XORPEN = 7

    'Konstanten für LoadImage
    Public Const IMAGE_BITMAP = 0
    Public Const IMAGE_CURSOR = 2
    Public Const IMAGE_ICON = 1
    Public Const LR_COLOR = 2
    Public Const LR_COPYDELETEORG = 8
    Public Const LR_COPYFROMRESOURCE = 16384
    Public Const LR_COPYRETURNORG = 4
    Public Const LR_CREATEDIBSECTION = 8192
    Public Const LR_DEFAULTCOLOR = 0
    Public Const LR_DEFAULTSIZE = 64
    Public Const LR_LOADFROMFILE = 16
    Public Const LR_LOADMAP3DCOLORS = 4096
    Public Const LR_LOADTRANSPARENT = 32
    Public Const LR_MONOCHROME = 1
    Public Const LR_SHARED = 32768
    Public Const LR_VGACOLOR = 128

    'Public Const PS_ALTERNATE = 8
    'Public Const PS_COSMETIC = 0
    Public Const PS_DASH = 1
    Public Const PS_DASHDOT = 3
    Public Const PS_DASHDOTDOT = 4
    Public Const PS_DOT = 2
    Public Const PS_NULL = 5
    Public Const PS_SOLID = 0

    'Konstanten für BitBlt, StretchBlt
    Public Const BLACKNESS = 66
    Public Const WHITENESS = 16711778
    Public Const SRCAND = 8913094
    Public Const SRCCOPY = 13369376
    Public Const SRCERASE = 4457256
    Public Const SRCINVERT = 6684742
    Public Const SRCPAINT = 15597702
    Public Const DSTINVERT = 5570569
    Public Const MERGECOPY = 12583114
    Public Const MERGEPAINT = 12255782
    Public Const NOTSRCCOPY = 3342344
    Public Const NOTSRCERASE = 1114278
    Public Const PATCOPY = 15728673
    Public Const PATINVERT = 5898313
    Public Const PATPAINT = 16452105

    'Konstanten für SetStretchBltMode
    Public Const BLACKONWHITE = 1
    Public Const COLORONCOLOR = 3
    Public Const HALFTONE = 4

    'Konstanten für CreateFont
    Public Const FW_BLACK = 900
    Public Const FW_BOLD = 700
    Public Const FW_DEMIBOLD = 600
    Public Const FW_DONTCARE = 0
    Public Const FW_EXTRABOLD = 800
    Public Const FW_EXTRALIGHT = 200
    Public Const FW_HEAVY = 900
    Public Const FW_LIGHT = 300
    Public Const FW_MEDIUM = 500
    Public Const FW_NORMAL = 400
    Public Const FW_REGULAR = 400
    Public Const FW_SEMIBOLD = 600
    Public Const FW_THIN = 100
    Public Const FW_ULTRABOLD = 800
    Public Const FW_ULTRALIGHT = 200

    Public Const ANSI_CHARSET = 0
    Public Const BALTIC_CHARSET = 186
    Public Const CHINESEBIG5_CHARSET = 136
    Public Const DEFAULT_CHARSET = 1
    Public Const EASTEUROPE_CHARSET = 238
    Public Const GB2312_CHARSET = 134
    Public Const GREEK_CHARSET = 161
    Public Const MAC_CHARSET = 77
    Public Const OEM_CHARSET = 255
    Public Const RUSSIAN_CHARSET = 204
    Public Const SHIFTJIS_CHARSET = 128
    Public Const SYMBOL_CHARSET = 2
    Public Const TURKISH_CHARSET = 162
    Public Const VIETNAMESE_CHARSET = 163

    Public Const DEFAULT_PITCH = 0
    Public Const VARIABLE_PITCH = 2
    Public Const FIXED_PITCH = 1

    'Konstanten für DrawText
    Public Const DT_BOTTOM = 8
    Public Const DT_CALCRECT = 1024
    Public Const DT_CENTER = 1
    Public Const DT_EDITCONTROL = 8192
    Public Const DT_END_ELLIPSIS = 32768
    Public Const DT_EXPANDTABS = 64
    Public Const DT_EXTERNALLEADING = 512
    Public Const DT_HIDEPREFIX = 1048576
    Public Const DT_INTERNAL = 4096
    Public Const DT_LEFT = 0
    Public Const DT_MODIFYSTRING = 65536
    Public Const DT_NOCLIP = 256
    Public Const DT_NOFULLWIDTHCHARBREAK = 524288
    Public Const DT_NOPREFIX = 2048
    Public Const DT_PATH_ELLIPSIS = 16384
    Public Const DT_PREFIXONLY = 2097152
    Public Const DT_RIGHT = 2
    Public Const DT_RTLREADING = 131072
    Public Const DT_SINGLELINE = 32
    Public Const DT_TABSTOP = 128
    Public Const DT_TOP = 0
    Public Const DT_VCENTER = 4
    Public Const DT_WORD_ELLIPSIS = 262144
    Public Const DT_WORDBREAK = 16

    'Konstante für SetDIBitsToDevice
    Public Const DIB_RGB_COLORS = 0
    Public Const DIB_PAL_COLORS = 1

    Public Const BI_bitfields = 3
    Public Const BI_JPEG = 4
    Public Const BI_PNG = 5
    Public Const BI_RGB = 0
    Public Const BI_RLE4 = 2
    Public Const BI_RLE8 = 1

    Public Const NEWFRAME = 1
    Public Const ABORTDOC = 2
    Public Const STARTDOC = 10
    Public Const ENDDOC = 11

    'Konstantzen für GetDeviceCaps
    Public Const DT_PLOTTER = 0
    Public Const DT_RASCAMERA = 3
    Public Const DT_RASDISPLAY = 1
    Public Const DT_RASPRINTER = 2
    Public Const DT_CHARSTREAM = 4
    Public Const DT_METAFILE = 5

    Public Const TECHNOLOGY = 2

    'Konstanten für ExtFloodFill
    Public Const FLOODFILLBORDER = 0
    Public Const FLOODFILLSURFACE = 1

    'Größe ist für 32 und 64 bitz gleich
    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure LOGPEN
        Dim lopnStyle As Int32
        Dim lopnWidth As Point
        Dim lopnColor As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure LOGBRUSH
        Dim lbStyle As Int32
        Dim lbColor As Int32
        Dim lbHatch As IntPtr
    End Structure

    ' Pack darf hier nicht verwendet werden (größe muss 24 bytes für 32-Bit und 32 bytes für 64 bit sein! siehe System.Drawing...)
    <StructLayout(LayoutKind.Sequential)> _
    Structure GDIBITMAP 'BITMAP structure
        Dim bmType As Integer
        Dim bmWidth As Integer
        Dim bmHeight As Integer
        Dim bmWidthBytes As Integer
        Dim bmPlanes As Short
        Dim bmBitsPixel As Short
        Dim bmBits As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure RGBQUAD
        Dim rgbBlue As Byte
        Dim rgbGreen As Byte
        Dim rgbRed As Byte
        Dim rgbReserved As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure BITMAPFILEHEADER
        Dim bfType As Int16
        Dim bfSize As Int32
        Dim bfReserved1 As Int16
        Dim bfReserved2 As Int16
        Dim bfOffBits As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure BITMAPINFOHEADER
        Dim biSize As Int32
        Dim biWidth As Int32
        Dim biHeight As Int32
        Dim biPlanes As Int16
        Dim biBitCount As Int16
        Dim biCompression As Int32
        Dim biSizeImage As Int32
        Dim biXPelsPerMeter As Int32
        Dim biYPelsPerMeter As Int32
        Dim biClrUsed As Int32
        Dim biClrImportant As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure BITMAPINFO
        Dim bmiHeader As BITMAPINFOHEADER
        Dim bmiColors As RGBQUAD
    End Structure

    'Structure BITMAPINFO
    'Dim bmiHeader_biSize As Int32
    'Dim bmiHeader_biWidth As Int32
    'Dim bmiHeader_biHeight As Int32
    'Dim bmiHeader_biPlanes As Int16
    'Dim bmiHeader_biBitCount As Int16
    'Dim bmiHeader_biCompression As Int32
    'Dim bmiHeader_biSizeImage As Int32
    'Dim bmiHeader_biXPelsPerMeter As Int32
    'Dim bmiHeader_biYPelsPerMeter As Int32
    'Dim bmiHeader_biClrUsed As Int32
    'Dim bmiHeader_biClrImportant As Int32
    'Dim bmiColors_rgbBlue As Byte
    'Dim bmiColors_rgbGreen As Byte
    'Dim bmiColors_rgbRed As Byte
    'Dim bmiColors_rgbReserved As Byte
    'End Structure

    Public Const RGN_AND = 1
    Public Const RGN_COPY = 5
    Public Const RGN_DIFF = 4
    Public Const RGN_MAX = 5
    Public Const RGN_MIN = 1
    Public Const RGN_OR = 2
    Public Const RGN_XOR = 3

    Public Declare Ansi Function BeginPath Lib "gdi32.dll" Alias "BeginPath" (ByVal hdc As IntPtr) As Integer
    Public Declare Ansi Function EndPath Lib "gdi32.dll" Alias "EndPath" (ByVal hdc As IntPtr) As Integer
    Public Declare Ansi Function SelectClipPath Lib "gdi32.dll" Alias "SelectClipPath" (ByVal hdc As IntPtr, ByVal mode As Integer) As Integer
    Public Declare Ansi Function SelectClipRgn Lib "gdi32.dll" Alias "SelectClipRgn" (ByVal hdc As IntPtr, ByVal rgn As IntPtr) As Integer
    'Für Drucker Escapes:
    Public Declare Ansi Function Escape Lib "gdi32.dll" Alias "Escape" (ByVal hdc As IntPtr, ByVal nEscape As Integer, ByVal cbInput As Integer, ByVal lpvInData As IntPtr, ByVal lpvOutData As IntPtr) As Integer
    Public Declare Ansi Function EscapeString Lib "gdi32.dll" Alias "Escape" (ByVal hdc As IntPtr, ByVal nEscape As Integer, ByVal cbInput As Integer, ByVal lpvInData As String, ByVal lpvOutData As IntPtr) As Integer

    Public Declare Function GetClipBox Lib "gdi32.dll" Alias "GetClipBox" (ByVal hdc As IntPtr, ByRef lprc As GDIRECT) As Integer
    'Public Declare Function GetBoundsRect Lib "gdi32.dll" Alias "GetBoundsRect" (ByVal hdc As IntPtr, ByRef bounds As GDIRECT, ByVal flags As Integer) As Integer
    'Public Declare Function GetWindowExtEx Lib "gdi32.dll" Alias "GetWindowExtEx" (ByVal hdc As IntPtr, ByRef sz As Size) As Integer
    'Public Declare Function GetWindowOrgEx Lib "gdi32.dll" Alias "GetWindowOrgEx" (ByVal hdc As IntPtr, ByRef pt As Point) As Integer
    'Public Declare Function SetWindowOrgEx Lib "gdi32.dll" Alias "SetWindowOrgEx" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer, ByRef lpPoint As Point) As Integer
    'Public Declare Function SetWindowExtEx Lib "gdi32.dll" Alias "SetWindowExtEx" (ByVal hdc As IntPtr, ByVal nXExtent As Integer, ByVal nYExtent As Integer, ByRef lpSize As Size) As Integer
    'Public Declare Function OffsetWindowOrgEx Lib "gdi32.dll" Alias "OffsetWindowOrgEx" (ByVal hdc As IntPtr, ByVal nXOffset As Integer, ByVal nYOffset As Integer, ByRef lpPoint As Point) As Integer
    'Public Declare Function OffsetViewportOrgEx Lib "gdi32.dll" Alias "OffsetViewportOrgEx" (ByVal hdc As IntPtr, ByVal nXOffset As Integer, ByVal nYOffset As Integer, ByRef lpPoint As Point) As Integer
    'Public Declare Function SetViewportExtEx Lib "gdi32.dll" Alias "SetViewportExtEx" (ByVal hdc As IntPtr, ByVal nXExtent As Integer, ByVal nYExtent As Integer, ByRef lpSize As Size) As Integer
    Public Declare Function SetViewportOrgEx Lib "gdi32.dll" Alias "SetViewportOrgEx" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer, ByRef lpPoint As Point) As Integer
    'Public Declare Function SetGraphicsMode Lib "gdi32.dll" Alias "SetGraphicsMode" (ByVal hdc As IntPtr, ByVal mode As Integer) As Integer
    'Public Declare Function SetMapMode Lib "gdi32.dll" Alias "SetMapMode" (ByVal hdc As IntPtr, ByVal mode As Integer) As Integer
    Public Declare Function GetCurrentObject Lib "gdi32.dll" Alias "GetCurrentObject" (ByVal hdc As IntPtr, ByVal obType As Integer) As IntPtr
    Public Declare Function CreatePatternBrush Lib "gdi32.dll" Alias "CreatePatternBrush" (ByVal hBmp As IntPtr) As IntPtr
    Public Declare Function GetDIBits Lib "gdi32.dll" Alias "GetDIBits" (ByVal hDC As IntPtr, ByVal hBmp As IntPtr, ByVal uStartScan As Integer, ByVal cScanLines As Integer, ByVal lpvBits() As Byte, ByRef lpbi As BITMAPINFO, ByVal uUsage As Integer) As Integer
    Public Declare Function SetDIBitsToDevice Lib "gdi32.dll" Alias "SetDIBitsToDevice" (ByVal hDC As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal dwWidth As Integer, ByVal dwHeight As Integer, ByVal XSrc As Integer, ByVal YSrc As Integer, ByVal uStartScan As Integer, ByVal cScanLines As Integer, ByVal lpvBits() As Byte, ByRef lpbmi As BITMAPINFO, ByVal fuColorUse As Integer) As Integer
    Public Declare Unicode Function CreateDC Lib "gdi32.dll" Alias "CreateDCW" (ByVal lpszDriver As String, ByVal lpszDevice As String, ByVal lpszOutput As String, ByVal lpInitData As IntPtr) As IntPtr
    Public Declare Unicode Function CreateFont Lib "gdi32.dll" Alias "CreateFontW" (ByVal nHeight As Integer, ByVal nWidth As Integer, ByVal nEscapement As Integer, ByVal nOrientation As Integer, ByVal fnWeight As Integer, ByVal fdwItalic As Integer, ByVal fdwUnderline As Integer, ByVal fdwStrikeOut As Integer, ByVal fdwCharSet As Integer, ByVal fdwOutputPrecision As Integer, ByVal fdwClipPrecision As Integer, ByVal fdwQuality As Integer, ByVal fdwPitchAndFamily As Integer, ByVal lpszFace As String) As IntPtr
    Public Declare Unicode Function GetMetaFile Lib "gdi32.dll" Alias "GetMetaFileW" (ByVal Filename As String) As IntPtr
    Public Declare Unicode Function CreateMetaFile Lib "gdi32.dll" Alias "CreateMetaFileW" (ByVal Filename As String) As IntPtr
    Public Declare Function PlayMetaFile Lib "gdi32.dll" Alias "PlayMetaFile" (ByVal hdc As IntPtr, ByVal hmf As IntPtr) As Integer
    Public Declare Function CloseMetaFile Lib "gdi32.dll" Alias "CloseMetaFile" (ByVal hdc As IntPtr) As Integer
    Public Declare Function DeleteMetaFile Lib "gdi32.dll" Alias "DeleteMetaFile" (ByVal hmf As IntPtr) As Integer
    Public Declare Function SetStretchBltMode Lib "gdi32.dll" Alias "SetStretchBltMode" (ByVal hdcDest As IntPtr, ByVal mode As Integer) As Integer
    Public Declare Function StretchBlt Lib "gdi32.dll" Alias "StretchBlt" (ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidthDest As Integer, ByVal nHeightDest As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal nWidthSrc As Integer, ByVal nHeightSrc As Integer, ByVal ROP As Integer) As Integer
    Public Declare Function BitBlt Lib "gdi32.dll" Alias "BitBlt" (ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal ROP As Integer) As Integer
    Public Declare Function CreateCompatibleBitmap Lib "gdi32.dll" Alias "CreateCompatibleBitmap" (ByVal hdc As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr
    Public Declare Function CreateBitmap Lib "gdi32.dll" Alias "CreateBitmap" (ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal cPlanes As Integer, ByVal cBitsPerPel As Integer, ByVal lpvBits As IntPtr) As IntPtr
    Public Declare Function CreateCompatibleDC Lib "gdi32.dll" Alias "CreateCompatibleDC" (ByVal hdc As IntPtr) As IntPtr
    Public Declare Function DeleteDC Lib "gdi32.dll" Alias "DeleteDC" (ByVal hdc As IntPtr) As Integer
    Public Declare Function Polyline Lib "gdi32.dll" Alias "Polyline" (ByVal hdc As IntPtr, ByVal points() As Point, ByVal numPoints As Integer) As Integer
    Public Declare Function Polygon Lib "gdi32.dll" Alias "Polygon" (ByVal hdc As IntPtr, ByVal points() As Point, ByVal numPoints As Integer) As Integer
    Public Declare Function GetDeviceCaps Lib "gdi32.dll" Alias "GetDeviceCaps" (ByVal hdc As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Unicode Function GetTextExtentPoint32 Lib "gdi32.dll" Alias "GetTextExtentPoint32W" (ByVal hdc As IntPtr, ByVal lpString As String, ByVal chars As Integer, ByRef lpSize As Size) As Integer
    Public Declare Ansi Function GDIGetObject Lib "gdi32.dll" Alias "GetObjectA" (ByVal hgdiobj As IntPtr, ByVal cbBuffer As Integer, ByRef lpvObject As LOGPEN) As Integer
    Public Declare Ansi Function GDIGetObject Lib "gdi32.dll" Alias "GetObjectA" (ByVal hgdiobj As IntPtr, ByVal cbBuffer As Integer, ByRef lpvObject As LOGBRUSH) As Integer
    Public Declare Ansi Function GDIGetObject Lib "gdi32.dll" Alias "GetObjectA" (ByVal hgdiobj As IntPtr, ByVal cbBuffer As Integer, ByRef lpvObject As GDIBITMAP) As Integer
    Public Declare Function SetTextColor Lib "gdi32.dll" Alias "SetTextColor" (ByVal DC As IntPtr, ByVal crColor As Integer) As Integer
    Public Declare Function GetTextColor Lib "gdi32.dll" Alias "GetTextColor" (ByVal DC As IntPtr) As Integer
    Public Declare Function SetBkColor Lib "gdi32.dll" Alias "SetBkColor" (ByVal DC As IntPtr, ByVal crColor As Integer) As Integer
    Public Declare Function GetBkColor Lib "gdi32.dll" Alias "GetBkColor" (ByVal DC As IntPtr) As Integer
    Public Declare Function SetROP2 Lib "gdi32.dll" Alias "SetROP2" (ByVal DC As IntPtr, ByVal fnDrawMode As Integer) As Integer
    Public Declare Function SetBkMode Lib "gdi32.dll" Alias "SetBkMode" (ByVal DC As IntPtr, ByVal iBkMode As Integer) As Integer
    Public Declare Function SelectObject Lib "gdi32.dll" Alias "SelectObject" (ByVal DC As IntPtr, ByVal GDIObject As IntPtr) As IntPtr
    Public Declare Function DeleteObject Lib "gdi32.dll" Alias "DeleteObject" (ByVal GDIObject As IntPtr) As Integer
    Public Declare Function GetObjectType Lib "gdi32.dll" Alias "GetObjectType" (ByVal obj As IntPtr) As Integer
    Public Declare Function GetStockObject Lib "gdi32.dll" Alias "GetStockObject" (ByVal objTyte As Integer) As IntPtr
    Public Declare Function CreateSolidBrush Lib "gdi32.dll" Alias "CreateSolidBrush" (ByVal color As Integer) As Integer

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure GDIRECT
        Dim left As Integer
        Dim top As Integer
        Dim right As Integer
        Dim bottom As Integer
    End Structure
    Public Declare Unicode Function TextOut Lib "gdi32.dll" Alias "TextOutW" (ByVal hdc As IntPtr, ByVal nXStart As Integer, ByVal nYStart As Integer, ByVal lpString As String, ByVal cbString As Integer) As Integer
    Public Declare Function GetPixel Lib "gdi32.dll" Alias "GetPixel" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer) As Integer
    Public Declare Function SetPixelV Lib "gdi32.dll" Alias "SetPixelV" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal color As Integer) As Integer
    Public Declare Function Rectangle Lib "gdi32.dll" Alias "Rectangle" (ByVal hdc As IntPtr, ByVal nLeftRect As Integer, ByVal nTopRect As Integer, ByVal nRightRect As Integer, ByVal nBottomRect As Integer) As Integer
    Public Declare Function LineTo Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer) As Integer
    Public Declare Function MoveToEx Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal pos As Integer) As Integer
    Public Declare Function CreatePen Lib "gdi32.dll" (ByVal fnPenStyle As Integer, ByVal nWidth As Integer, ByVal crColor As Integer) As IntPtr
    Public Declare Function Ellipse Lib "gdi32.dll" (ByVal DC As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal x2 As Integer, ByVal y2 As Integer) As Integer
    Public Declare Function ExtFloodFill Lib "gdi32.dll" Alias "ExtFloodFill" (ByVal hdc As IntPtr, ByVal nXStart As Integer, ByVal nYStart As Integer, ByVal crColor As Integer, ByVal fuFillType As Integer) As Integer

    'Ist in user32.dll
    Public Declare Unicode Function DrawText Lib "user32.dll" Alias "DrawTextW" (ByVal hdc As IntPtr, ByVal lpString As String, ByVal count As Integer, ByRef rect As GDIRECT, ByVal uFormat As Integer) As Integer
    'Von LOADBMP verwendet
    Public Declare Ansi Function LoadImage Lib "user32.dll" Alias "LoadImageA" (ByVal hInst As IntPtr, ByVal lpsz As String, ByVal dwImageType As Integer, ByVal dwDesiredWidth As Integer, ByVal dwDesiredHeight As Integer, ByVal dwFlags As Integer) As IntPtr

    'Wurde für _MOUSEX/Y benötigt...
    'Public Declare Function ClientToScreen Lib "user32" Alias "ClientToScreen" (ByVal hWnd As Integer, ByRef pt As Point) As Boolean
    'Altes INKEY$
    'Public Declare Function GetKeyboardState Lib "user32" Alias "GetKeyboardState" (ByVal pbKeyState() As Byte) As Long
    'Public Declare Function ToAscii Lib "user32.dll" Alias "ToAscii" _
    '(ByVal vk As Integer, ByVal scancode As Integer, ByVal virtKey() As Byte, ByRef ch As Char, ByVal uFlags As Integer) As Integer
    'Public Declare Function MapVirtualKey Lib "user32.dll" Alias "MapVirtualKeyA" _
    '(ByVal uCode As Integer, ByVal uMapType As Integer) As Integer
    Declare Function GetKeyState Lib "user32.dll" Alias "GetKeyState" (ByVal nVirtKey As Integer) As Integer
    'Public Declare Function GetTickCount Lib "kernel32.dll" () As Integer
#End Region
#Region "Win16 Emulation"
    ''' <summary>
    ''' Räumt den Speicher auf (Garbage Collector).
    ''' </summary>
    ''' <param name="num">wird ignoriert</param>
    Function GlobalCompact(ByVal num As Integer) As Boolean
        GC.Collect()
        Return True
    End Function
#End Region
#Region "Interne Klassen"
    Class GFAMisc
        Private Const MAX_MENURESULT = 16
        Private Const MAX_REGISTERVARIABLE = 16
        Private Shared m_ClipboardFormat As Integer = -1
        Private Shared m_MenuResult(MAX_MENURESULT) As Integer
        Private Shared m_RegisterVariable(MAX_REGISTERVARIABLE) As Integer
        Private Shared m_SelectedFont As Drawing.Font

        Public Shared Property SelectedFont() As Drawing.Font
            Get
                Return m_SelectedFont
            End Get
            Set(ByVal value As Drawing.Font)
                m_SelectedFont = value
            End Set
        End Property

        Public Shared Function SwapRedAndBlue(ByVal color As Integer) As Integer
            Dim red, green, blue As Integer
            blue = color And 255
            green = (color / 256) And 255
            red = (color / (256 * 256)) And 255
            Return red + green * 256 + blue * 256 * 256
        End Function

        Public Shared Property RegisterVariable(ByVal index As Integer) As Integer
            Get
                If index >= 0 And index <= MAX_REGISTERVARIABLE Then
                    Return m_RegisterVariable(index)
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                If index >= 0 And index <= MAX_REGISTERVARIABLE Then
                    m_RegisterVariable(index) = value
                End If
            End Set
        End Property

        Public Shared Property MenuResult(ByVal index As Integer) As Integer
            Get
                If index >= 0 And index <= MAX_MENURESULT Then
                    Return m_MenuResult(index)
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                If index >= 0 And index <= MAX_MENURESULT Then
                    m_MenuResult(index) = value
                End If
            End Set
        End Property

        Public Shared Property ClipboardFormat() As Integer
            Get
                Return m_ClipboardFormat
            End Get
            Set(ByVal value As Integer)
                m_ClipboardFormat = value
            End Set
        End Property
    End Class
    Class GFAArray
        'Interne Arrayfunktionen
        Public Shared Sub ArrayFill1D(Of T)(ByRef arr() As T, ByVal newValue As T)
            Dim x As Integer
            For x = 0 To arr.GetLength(0) - 1
                arr(x) = newValue
            Next
        End Sub
        Public Shared Sub ArrayFill2D(Of T)(ByRef arr(,) As T, ByVal newValue As T)
            Dim x, y As Integer
            For y = 0 To arr.GetLength(1) - 1
                For x = 0 To arr.GetLength(0) - 1
                    arr(x, y) = newValue
                Next
            Next
        End Sub
        Public Shared Sub ArrayFill3D(Of T)(ByRef arr(,,) As T, ByVal newValue As T)
            Dim x, y, z As Integer
            For z = 0 To arr.GetLength(2) - 1
                For y = 0 To arr.GetLength(1) - 1
                    For x = 0 To arr.GetLength(0) - 1
                        arr(x, y, z) = newValue
                    Next
                Next
            Next
        End Sub
        Public Shared Sub ArrayFill4D(Of T)(ByRef arr(,,,) As T, ByVal newValue As T)
            Dim x, y, z, a As Integer
            For a = 0 To arr.GetLength(3) - 1
                For z = 0 To arr.GetLength(2) - 1
                    For y = 0 To arr.GetLength(1) - 1
                        For x = 0 To arr.GetLength(0) - 1
                            arr(x, y, z, a) = newValue
                        Next
                    Next
                Next
            Next
        End Sub
    End Class
    Class GFADataSection
        Private Shared m_ReadIndex As Integer = 0
        Private Shared m_Count As Integer = 0
        Private Shared m_Data(65535) As String
        Private Shared m_Labels As Hashtable = New Hashtable
        Private Shared m_LabelCount As Integer = 0

        Public Shared Sub ReadData(ByRef obj)
            If m_ReadIndex < 0 Or m_ReadIndex >= m_Count Then
                Throw New Exception("Out of DATA (" & m_ReadIndex & ")")
            End If
            obj = m_Data(m_ReadIndex)
            m_ReadIndex += 1
        End Sub

        Public Shared Sub Restore(ByVal Name As String)
            Dim newPos As Integer
            Name = Name.ToLower().Trim()
            If m_Labels.Contains(Name) Then
                newPos = m_Labels(Name) - 1
                If newPos < 0 Or newPos >= m_Count Then
                    Throw New ArgumentException("Illegal DATA position " & newPos & " " & Chr(34) & Name & Chr(34))
                End If
                m_ReadIndex = newPos
            Else
                Throw New ArgumentException("Cannot find DATA label " & Chr(34) & Name & Chr(34))
            End If
        End Sub

        Public Shared Sub AddLabel(ByVal Name As String)
            Name = Name.ToLower().Trim()
            If m_LabelCount > 65535 Then
                Throw New ArgumentException("Too many DATA labels (" & Name & ")")
            End If
            If m_Labels.Contains(Name) Then
                Throw New ArgumentException("Label already declared (" & Name & ")")
            End If
            m_LabelCount += 1
            m_Labels(Name) = m_Count + 1
        End Sub

        Public Shared Sub Define(ByVal value As String)
            If m_Count >= 65535 Then
                Throw New Exception("Too many DATAs (" & value & ")")
            End If
            If value IsNot Nothing Then
                m_Data(m_Count) = value
            Else
                m_Data(m_Count) = ""
            End If
            m_Count += 1
        End Sub
    End Class
    Class GFAConvert
        Public Shared Function ToStr(ByVal value As Double, ByVal numDigits As Integer) As String
            Dim strNum As String
            'Dim pos As Integer, n As Integer
            If numDigits < 0 Then numDigits = 0
            'n = numDigits
            'If n >= 0 Then
            '    If n > 15 Then n = 15 'Ansonsten Exception
            '    value = Math.Round(value, n)
            'End If

            'Format rundet die Zahl auch!
            strNum = Format(value, "#0." & New String("0"c, numDigits))
            strNum = strNum.Replace(Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, ".") ' Problem mit .->,

            'Alter Code hatte Problem mit Exponetialdarstellung bei extremen Zahlen!
            'pos = InStr(strNum, ".")
            'If pos > 0 Then
            'n = numDigits - (Len(strNum) - pos)
            'If n > 0 Then
            'strNum += New String("0"c, n)
            'End If
            'Else
            'If numDigits > 0 Then
            'strNum += "." + New String("0"c, numDigits)
            'End If
            'End If
            Return strNum
        End Function

        Public Shared Function StrToDouble(ByVal str As String, ByRef failed As Boolean) As Double
            'VB.NETs Val Befehl liefert nicht immer gleiche Ergebnisse
            '&H6 = 6
            '-&H6 -> 0
            Dim begin As Integer = 0
            Dim isHex As Boolean = False
            Dim isOct As Boolean = False
            Dim isBin As Boolean = False
            Dim isNegative As Boolean = False
            Dim numStr As String = ""
            Dim result As Double = 0.0R
            Dim findDec As String = ".0123456789"
            Dim i As Integer
            Dim expPos As Integer = 0
            Dim exponent As String = ""
            Dim innerFailed As Boolean = False ' Wird nicht ausgewertet (ist auch sinnvoll)

            failed = False
            str = UCase(Trim(str)) ' Vorhergehende Leerzeichen entfernen, Groß-Kleinschreibung nicht beachten
            'String abschneiden

            begin = 1
            If Left(str, 1) = "$" Then
                isHex = True
                begin = 2
            End If
            If Left(str, 2) = "&H" Then
                isHex = True
                begin = 3
            End If
            If Left(str, 2) = "&O" Or Left(str, 2) = "&Q" Then
                isOct = True
                begin = 3
            End If
            If Left(str, 1) = "%" Then
                isBin = True
                begin = 2
            End If
            If Left(str, 2) = "&X" Then
                isBin = True
                begin = 3
            End If

            If Left(str, 1) = "-" Then ' Wird in GFA nicht für OCT,BIN und HEX unterstüzt (Maximal eines)
                begin = 2
                isNegative = True
            End If
            If Left(str, 1) = "+" Then ' Wird in GFA nicht für OCT,BIN und HEX unterstüzt (Maximal eines)
                begin = 2
            End If

            If isBin = False And isOct = False And isHex = False Then
                expPos = _INSTR(str, "E", begin + 1)
                If expPos > 0 Then
                    exponent = _RIGHT(str, Len(str) - expPos)
                    str = _LEFT(str, expPos - 1)
                End If
            End If

            For i = begin To str.Length

                If isHex Then
                    If InStr("0123456789ABCDEF", _MID(str, i, 1)) > 0 Then
                        numStr += _MID(str, i, 1)
                    Else
                        Exit For
                    End If
                ElseIf isOct Then
                    If InStr("01234567", _MID(str, i, 1)) > 0 Then
                        numStr += _MID(str, i, 1)
                    Else
                        Exit For
                    End If
                ElseIf isBin Then
                    If InStr("01", _MID(str, i, 1)) > 0 Then
                        numStr += _MID(str, i, 1)
                    Else
                        Exit For
                    End If
                Else
                    If InStr(findDec, _MID(str, i, 1)) > 0 Then

                        If _MID(str, i, 1) = "." Then 'Nur den ersten Punkt zulassen!
                            findDec = findDec.Replace(".", "") 'Punkt entfernen '"0123456789"
                            If i = begin Then
                                numStr = "0"  ' Muss mit 0 anfangen..
                            End If
                        End If

                        numStr += _MID(str, i, 1)
                    Else
                        Exit For
                    End If
                End If
            Next

            If isHex Then
                Try
                    Return Convert.ToInt64(numStr, 16)
                Catch ex As FormatException
                    failed = True
                    Return 0.0R
                Catch ex As OverflowException
                    failed = True
                    Return 0.0R
                End Try

            ElseIf isOct Then
                Try
                    Return Convert.ToInt64(numStr, 8)
                Catch ex As FormatException
                    failed = True
                    Return 0.0R
                Catch ex As OverflowException
                    failed = True
                    Return 0.0R
                End Try
            ElseIf isBin Then
                Try
                    Return Convert.ToInt64(numStr, 2)
                Catch ex As FormatException
                    Return 0.0R
                Catch ex As OverflowException
                    Return 0.0R
                End Try
            Else
                numStr = numStr.Replace(".", Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) ' Problem mit .->,
                'Try
                '    Return Convert.ToDouble(numStr)
                'Catch ex As Exception
                '    Return 0D
                'End Try
                If Double.TryParse(numStr, result) Then

                    If exponent = "" Then
                        If isNegative Then
                            Return -result
                        Else
                            Return result
                        End If
                    Else
                        If isNegative Then
                            Return -result * Math.Pow(10.0, StrToDouble(exponent, innerFailed))
                        Else
                            Return result * Math.Pow(10.0, StrToDouble(exponent, innerFailed))
                        End If
                    End If

                Else
                    failed = True
                    Return 0.0R
                End If
            End If

            failed = True
            Return 0.0R
        End Function

    End Class
    Class GFAUsing
        Public Shared Function FormatString(ByVal format As String, ByVal str As String)
            Dim i As Integer = 0
            Dim length As Integer = 0
            If format = "&" Then
                Return str
            ElseIf format = "!" Then
                Return Microsoft.VisualBasic.Left(str, 1)
            Else

                For i = 1 To Len(format)
                    If Mid(format, i, 1) = "\" Or Mid(format, i, 1) = "." Then
                        length += 1
                    End If
                Next
                str = str + New String(" ", length)
                Return Microsoft.VisualBasic.Left(str, length)
            End If
        End Function

        Public Shared Function FormatObjects(ByVal str As String, ByVal ParamArray arr() As Object) As String
            Dim i As Integer = 0
            Dim inBlock As Boolean = False
            Dim isEscape As Boolean = False
            Dim param As String = ""
            Dim idx As Integer = 0
            Dim obj As Object = Nothing
            FormatObjects = ""

            For i = 1 To Len(str)
                Dim ch As String = Mid(str, i, 1)
                If idx < arr.Length Then
                    obj = arr(idx)
                Else
                    obj = Nothing
                End If

                If isEscape Then

                    isEscape = False
                    If inBlock Then
                        'params.Add(param)

                        If obj IsNot Nothing Then
                            If IsNumeric(obj) Then
                                FormatObjects += FormatNumber(param, arr(idx))
                            Else
                                FormatObjects += FormatString(param, obj.ToString())
                            End If
                        Else
                            FormatObjects += param
                        End If

                        idx += 1
                        param = ch
                        inBlock = False
                    Else
                        param += ch
                    End If

                Else
                    Select Case ch
                        Case ".", ",", "-", "+", "$", "#", "\", "!", "&", "^", Chr(34)
                            If inBlock Then
                                param += ch
                            Else
                                'params.Add(param)
                                FormatObjects += param
                                param = ch
                                If ch <> "^" Then '2010-11-01 Darf nicht am Anfang stehen
                                    inBlock = True
                                End If
                            End If
                        Case "_"
                                isEscape = True
                        Case Else
                                If inBlock Then

                                    If obj IsNot Nothing Then
                                        If IsNumeric(obj) Then
                                            FormatObjects += FormatNumber(param, arr(idx))
                                        Else
                                            FormatObjects += FormatString(param, obj.ToString())
                                        End If
                                    Else
                                        FormatObjects += param
                                    End If

                                    idx += 1
                                    param = ch
                                    inBlock = False
                                Else
                                    param += ch
                                End If
                    End Select
                End If
            Next
            If inBlock Then
                If obj IsNot Nothing Then
                    If IsNumeric(obj) Then
                        FormatObjects += FormatNumber(param, arr(idx))
                    Else
                        FormatObjects += FormatString(param, obj.ToString())
                    End If
                Else
                    FormatObjects += param
                End If
            Else
                FormatObjects += param
            End If

        End Function

        Public Shared Function FormatNumber(ByVal str As String, ByVal number As Double) As String
            Dim i As Integer = 0
            Dim beforeDecimalSeperator As Boolean = True
            Dim AddDollarChar As Boolean = False
            Dim numDigitsBefore As Integer = 0
            Dim numDigitsAfter As Integer = 0
            Dim digitsBefore As String = ""
            Dim digitsAfter As String = ""
            Dim idxBefore As Integer = 1
            Dim idxAfter As Integer = 1
            Dim strNum As String = ""
            Dim ignoreNext As Boolean = False
            'Erw. Exponentialschreibweise
            Dim exponent As Integer
            Dim exponentStr As String = ""
            Dim numExpDigits As Integer = 0

            FormatNumber = ""

            For i = 1 To Len(str)
                Dim ch As String = Mid(str, i, 1)

                If ignoreNext = False Then
                    Select Case ch
                        Case "#"
                            If beforeDecimalSeperator Then
                                numDigitsBefore += 1
                            Else
                                numDigitsAfter += 1
                            End If
                        Case "."
                            beforeDecimalSeperator = False
                        Case "$"
                            AddDollarChar = True
                        Case "_"
                            ignoreNext = True
                            '-- Erw. Exponentialschreibweise Anfang --
                        Case "^"
                            If numDigitsBefore > 0 Or numDigitsAfter > 0 Then 'Sicherheitsabfrage
                                numExpDigits += 1
                            End If
                            '-- Erw. Exponentialschreibweise Ende --
                    End Select
                Else
                    ignoreNext = False
                End If
            Next

            '-- Erw. Exponentialschreibweise Anfang --
            'ACHTUNG: Verhält sich zumindest bei 10^-x werten nicht wie in GFA!
            If numExpDigits > 0 Then
                exponent = 0
                If number <> 0.0R Then
                    exponent = Math.Floor(Math.Log10(number))
                End If

                If exponent > (numDigitsBefore - 1) Then
                    exponent -= (numDigitsBefore - 1)
                End If

                'If neededDigitsBefore < -(numDigitsBefore - 1) Then
                '    neededDigitsBefore -= (numDigitsBefore - 1)
                'End If

                number /= Math.Pow(10.0R, exponent)

                exponentStr = Math.Abs(exponent).ToString
                If numExpDigits > 2 Then
                    exponentStr = exponentStr.PadLeft(numExpDigits - 2, "0"c)
                End If
                If exponent >= 0 Then
                    exponentStr = "E+" & exponentStr
                Else
                    exponentStr = "E-" & exponentStr
                End If
            End If
            '-- Erw. Exponentialschreibweise Ende --

            If numDigitsAfter > 15 Then 'Ansonsten Exception
                number = Math.Round(number, 15)
            Else
                number = Math.Round(number, numDigitsAfter)
            End If

            strNum = number.ToString()
            strNum = strNum.Replace(Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, ".") ' Problem mit .->,


            'Vorne mit Leerzeichen und hinten mit 0en füllen
            If InStr(strNum, ".") Then
                If AddDollarChar Then
                    digitsBefore = New String(" ", numDigitsBefore) + "$" + Microsoft.VisualBasic.Left(strNum, InStr(strNum, ".") - 1)
                Else
                    digitsBefore = New String(" ", numDigitsBefore) + Microsoft.VisualBasic.Left(strNum, InStr(strNum, ".") - 1)
                End If

                digitsAfter = Microsoft.VisualBasic.Right(strNum, Len(strNum) - InStr(strNum, ".")) + New String("0", numDigitsAfter)
            Else
                digitsBefore = New String(" ", numDigitsBefore) + strNum
                digitsAfter = New String("0", numDigitsAfter)
            End If
            digitsBefore = Microsoft.VisualBasic.Right(digitsBefore, numDigitsBefore)
            digitsAfter = Microsoft.VisualBasic.Left(digitsAfter, numDigitsAfter)

            ignoreNext = False
            beforeDecimalSeperator = True
            For i = 1 To Len(str)
                Dim ch As String = Mid(str, i, 1)

                If ignoreNext = False Then
                    Select Case ch
                        Case "#"
                            If beforeDecimalSeperator Then
                                FormatNumber += Mid(digitsBefore, idxBefore, 1)
                                idxBefore += 1
                            Else
                                FormatNumber += Mid(digitsAfter, idxAfter, 1)
                                idxAfter += 1
                            End If
                        Case "."
                            beforeDecimalSeperator = False
                            FormatNumber += "."
                        Case "-"
                            If number < 0 Then
                                FormatNumber += "-"
                            Else
                                FormatNumber += " "
                            End If
                        Case "+"
                            If number >= 0 Then
                                FormatNumber += "+"
                            Else
                                FormatNumber += " "
                            End If
                        Case "$"
                        Case "_"
                            ignoreNext = True
                            '-- Erw. Exponentialschreibweise Anfang --
                        Case "^"
                            If Len(exponentStr) > 0 Then
                                If Left(exponentStr, 1) = "E" Then
                                    FormatNumber += Left(exponentStr, 2)
                                    exponentStr = Right(exponentStr, Len(exponentStr) - 2)
                                Else
                                    FormatNumber += Left(exponentStr, 1)
                                    exponentStr = Right(exponentStr, Len(exponentStr) - 1)
                                End If
                            Else
                                FormatNumber += " "
                            End If
                            '-- Erw. Exponentialschreibweise Ende --
                        Case Else
                            FormatNumber += ch
                    End Select
                Else
                    ignoreNext = False
                    FormatNumber += ch
                End If
            Next
        End Function
    End Class
    Class AlertWindow
        Protected m_Btns As List(Of Button)
        Protected m_Frm As Form
        Protected m_Result As Integer
        Protected m_Bmp As Bitmap
        Protected m_Pict As PictureBox
        Protected m_lblText As Label

        Public Shared Sub PlaySound(ByVal iconNr As Integer)
            Select Case iconNr
                Case 0
                    My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
                    'MessageBeep(MB_OK)
                Case 1
                    My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                    'MessageBeep(MB_ICONEXCLAMATION)
                Case 2
                    'MessageBeep(MB_ICONQUESTION)
                    My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Question)
                Case 3
                    'MessageBeep(MB_ICONHAND)
                    My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Hand)
            End Select
        End Sub
        Public ReadOnly Property Result() As Integer
            Get
                Result = m_Result
                Exit Property
            End Get
        End Property
        Private Function GetTextHeight(ByVal fnt As Font, ByVal text As String) As Integer
            Dim lbl As Label = New Label
            If fnt IsNot Nothing Then
                lbl.Font = fnt
            End If
            lbl.Text = text
            lbl.AutoSize = True
            Return lbl.PreferredHeight
        End Function
        Private Function GetTextWidth(ByVal fnt As Font, ByVal text As String) As Integer
            Dim lbl As Label = New Label
            If fnt IsNot Nothing Then
                lbl.Font = fnt
            End If
            lbl.Text = text
            lbl.AutoSize = True
            Return lbl.PreferredWidth
        End Function
        Private Function GetIcon(ByVal nr As Integer) As Bitmap
            Dim icon As Icon = Nothing
            Dim bmp As Bitmap = Nothing
            Select Case nr
                Case 1
                    icon = Drawing.SystemIcons.Warning
                Case 2
                    icon = Drawing.SystemIcons.Question
                Case 3
                    icon = Drawing.SystemIcons.Error
            End Select

            If icon IsNot Nothing Then
                bmp = icon.ToBitmap()
            End If

            Return bmp
        End Function
        Sub New(ByVal fnt As Font, ByVal iconNr As Integer, ByVal buttons As String, ByVal text As String, ByVal defaultButton As Integer)
            Dim height As Integer = 10
            Dim width As Integer = 0
            'Dim btnPos As Integer = 0
            Dim btnWidth As Integer = 0
            Dim btnHeight As Integer = 0
            Dim btnRowHeight As Integer = 0
            Dim btnRowWidth As Integer = 0
            Dim btnIdx As Integer = 0
            Dim widthButtons = 0
            Dim centerButtons As Integer = 0
            Dim btn As Button = Nothing
            Dim buttonList = buttons.Split("|")

            m_Frm = New Form

            If fnt Is Nothing Then
                fnt = m_Frm.Font
            End If
            m_Frm.Font = fnt
            m_Btns = New List(Of Button)
            m_lblText = New Label
            m_Pict = New PictureBox
            m_Bmp = GetIcon(iconNr)
            m_Result = 0

            If m_Bmp IsNot Nothing Then
                m_Pict.Image = DirectCast(m_Bmp, Image)
                m_Pict.SizeMode = PictureBoxSizeMode.StretchImage
                m_Pict.Location = New Point(10, 10)
                m_Pict.Size = New Size(32, 32)
                m_Frm.Controls.Add(m_Pict)
                width += 52
                widthButtons += 52
            Else
                width += 10
                widthButtons += 10
            End If

            text = text.Replace("|", vbCrLf) 'Chr(13) + Chr(10)

            m_lblText.Text = text
            m_lblText.AutoSize = True
            m_lblText.Font = fnt
            m_lblText.Location = New Point(widthButtons, height)
            m_Frm.Controls.Add(m_lblText)

            width += GetTextWidth(fnt, text)
            height += GetTextHeight(fnt, text)
            height += 10

            For Each btnText As String In buttonList
                btnRowWidth += Math.Max(GetTextWidth(m_Frm.Font, btnText) + 8, 75) + 2
            Next

            centerButtons = (GetTextWidth(m_Frm.Font, text) - btnRowWidth) / 2
            If centerButtons > 0 Then
                widthButtons += centerButtons
            End If

            For Each btnText As String In buttonList

                btnIdx += 1
                btn = New Button()
                btn.Font = fnt
                btn.Text = btnText
                btn.Location = New Point(widthButtons, height)
                btn.TextAlign = ContentAlignment.MiddleCenter

                btnWidth = Math.Max(GetTextWidth(m_Frm.Font, btnText) + 8, 75)
                btnHeight = Math.Max(GetTextHeight(m_Frm.Font, btnText) + 8, 21)
                btnRowHeight = Math.Max(btnRowHeight, btnHeight)

                btn.Size = New Size(btnWidth, btnHeight)
                btn.DialogResult = DialogResult.OK
                AddHandler btn.Click, AddressOf __Click

                widthButtons += btnWidth + 2
                m_Frm.Controls.Add(btn)
                m_Btns.Add(btn)
                If defaultButton = btnIdx Then
                    btn.Select()
                End If
            Next
            width += 10
            widthButtons += 10

            height += btnRowHeight
            height += 20

            m_Frm.StartPosition = FormStartPosition.CenterScreen
            m_Frm.ShowInTaskbar = False
            m_Frm.MaximizeBox = False
            m_Frm.MinimizeBox = False
            m_Frm.ControlBox = False
            m_Frm.FormBorderStyle = FormBorderStyle.FixedDialog
            m_Frm.ClientSize = New Size(Math.Max(width, widthButtons), height)
            m_Frm.ShowDialog()
        End Sub
        Private Sub __Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim idx = 0
            m_Result = 0
            For Each btn As Button In m_Btns
                idx += 1
                If btn.Equals(sender) Then
                    m_Result = idx
                End If
            Next
        End Sub
    End Class
    Class GFADrawing
        Implements IDisposable
        Private m_IsDisposed As Boolean = False  ' Ist True, wenn bereits freigegeben

        Protected m_PrintLocation As Point
        Protected m_DefFillMode As Integer = 0
        Protected m_OriginalBrush As IntPtr = IntPtr.Zero
        Protected m_OriginalPen As IntPtr = IntPtr.Zero
        Protected m_LastCreatedPen As IntPtr = IntPtr.Zero

        Protected m_form As Form
        Protected m_gr As Graphics
        Protected m_DC As IntPtr = IntPtr.Zero
        Protected m_ForeColor As Integer = 0
        Protected m_BackColor As Integer = &HFFFFFF

        Protected m_PenWidth As Long = 0
        Protected m_PenColor As Long = 0
        Protected m_PenStyle As Long = 0

        'Protected m_ClipOffset As Point

        Protected Shared m_PatternBrushes(54) As IntPtr
        Protected Shared m_PatternBitmaps(54) As IntPtr
        Protected Shared m_PatternBrushUseCounts(54) As Long

        Protected Shared m_ActiveDrawing As GFADrawing = Nothing

        Protected Shared m_MetaLastDrawing As GFADrawing = Nothing
        Protected Shared m_MetaDrawing As Boolean = False

        'Variablen für _FONT und _FONT_TO
        Private Shared m_Font_FontName As String = ""
        Private Shared m_Font_Weight As Integer = 0
        Private Shared m_Font_Width As Integer = 0
        Private Shared m_Font_Height As Integer = 0
        Private Shared m_Font_Italic As Integer = 0
        Private Shared m_Font_Strikeout As Integer = 0
        Private Shared m_Font_Underline As Integer = 0
        Private Shared m_Font_Escapement As Integer = 0
        Private Shared m_Font_Family As Integer = 0
        Private Shared m_Font_Charset As Integer = 0
        Private Shared m_Font_Pitch As Integer = 0
        Private Shared m_Font_OutPrecision As Integer = 0
        Private Shared m_Font_ClipPrecision As Integer = 0
        Private Shared m_Font_Orientation As Integer = 0
        Private Shared m_Font_Quality As Integer = 0

#Region "Private statische Funktionen/Prozeduren"

        Private Shared Function ReadDWORD(ByRef Buffer() As Byte, ByVal index As Integer) As Integer
            'Return Buffer(index) + (Buffer(index + 1) * 256) + (Buffer(index + 2) * 256 * 256) + (Buffer(index + 3) * 256 * 256 * 256)
            Return BitConverter.ToInt32(Buffer, index)
        End Function

        Private Shared Function ReadWORD(ByRef Buffer() As Byte, ByVal index As Integer) As Integer
            'Return Buffer(index) + (Buffer(index + 1) * 256)
            Return BitConverter.ToInt16(Buffer, index)
        End Function

        Private Shared Sub SaveWORD(ByRef Buffer() As Byte, ByVal index As Integer, ByVal value As Int16)
            'Buffer(index) = value And &HFF
            'Buffer(index + 1) = (value / 256) And &HFF
            Dim bytes() As Byte = BitConverter.GetBytes(value)
            If bytes.Length = 2 Then
                Buffer(index) = bytes(0)
                Buffer(index + 1) = bytes(1)
            Else
                Debug.WriteLine("incorrect number of bytes(" & bytes.Length & ") from BitConverter.GetBytes ")
            End If
        End Sub

        Private Shared Sub SaveDWORD(ByRef Buffer() As Byte, ByVal index As Integer, ByVal value As Int32)
            'Buffer(index) = value And &HFF
            'Buffer(index + 1) = (value / 256) And &HFF
            'Buffer(index + 2) = (value / (256 * 256)) And &HFF
            'Buffer(index + 3) = (value / (256 * 256)) And &HFF
            Dim bytes() As Byte = BitConverter.GetBytes(value)
            If bytes.Length = 4 Then
                Buffer(index) = bytes(0)
                Buffer(index + 1) = bytes(1)
                Buffer(index + 2) = bytes(2)
                Buffer(index + 3) = bytes(3)
            Else
                Debug.WriteLine("incorrect number of bytes(" & bytes.Length & ") from BitConverter.GetBytes ")
            End If
        End Sub

        Private Shared Sub SetBITMAPFILEHEADER(ByRef Buffer() As Byte, ByVal bih As BITMAPFILEHEADER)
            SaveWORD(Buffer, 0, bih.bfType)
            SaveWORD(Buffer, 2, bih.bfSize)
            SaveWORD(Buffer, 6, bih.bfReserved1)
            SaveWORD(Buffer, 8, bih.bfReserved1)
            SaveDWORD(Buffer, 10, bih.bfOffBits)
        End Sub

        Private Shared Sub SetBITMAPINFO(ByRef Buffer() As Byte, ByVal offset As Integer, ByVal bi As BITMAPINFO)
            SaveDWORD(Buffer, offset + 0, bi.bmiHeader.biSize)
            SaveDWORD(Buffer, offset + 4, bi.bmiHeader.biWidth)
            SaveDWORD(Buffer, offset + 8, bi.bmiHeader.biHeight)
            SaveWORD(Buffer, offset + 12, bi.bmiHeader.biPlanes)
            SaveWORD(Buffer, offset + 14, bi.bmiHeader.biBitCount)
            SaveDWORD(Buffer, offset + 16, bi.bmiHeader.biCompression)
            SaveDWORD(Buffer, offset + 20, bi.bmiHeader.biSizeImage)
            SaveDWORD(Buffer, offset + 24, bi.bmiHeader.biXPelsPerMeter)
            SaveDWORD(Buffer, offset + 28, bi.bmiHeader.biYPelsPerMeter)
            SaveDWORD(Buffer, offset + 32, bi.bmiHeader.biClrUsed)
            SaveDWORD(Buffer, offset + 36, bi.bmiHeader.biClrImportant)
            Buffer(offset + 40) = bi.bmiColors.rgbBlue
            Buffer(offset + 41) = bi.bmiColors.rgbGreen
            Buffer(offset + 42) = bi.bmiColors.rgbRed
            Buffer(offset + 43) = bi.bmiColors.rgbReserved
        End Sub

        Private Shared Function GetBITMAPFILEHEADER(ByRef Buffer() As Byte) As BITMAPFILEHEADER
            Dim result As BITMAPFILEHEADER
            result.bfType = ReadWORD(Buffer, 0)
            result.bfSize = ReadDWORD(Buffer, 2)
            result.bfReserved1 = 0
            result.bfReserved2 = 0
            result.bfOffBits = ReadDWORD(Buffer, 10)
            Return result
        End Function

        Private Shared Function GetBITMAPINFO(ByRef Buffer() As Byte, ByVal offset As Integer) As BITMAPINFO
            Dim result As BITMAPINFO
            result.bmiHeader.biSize = ReadDWORD(Buffer, offset + 0)
            result.bmiHeader.biWidth = ReadDWORD(Buffer, offset + 4)
            result.bmiHeader.biHeight = ReadDWORD(Buffer, offset + 8)
            result.bmiHeader.biPlanes = ReadWORD(Buffer, offset + 12)
            result.bmiHeader.biBitCount = ReadWORD(Buffer, offset + 14)
            result.bmiHeader.biCompression = ReadDWORD(Buffer, offset + 16)
            result.bmiHeader.biSizeImage = ReadDWORD(Buffer, offset + 20)
            result.bmiHeader.biXPelsPerMeter = ReadDWORD(Buffer, offset + 24)
            result.bmiHeader.biYPelsPerMeter = ReadDWORD(Buffer, offset + 28)
            result.bmiHeader.biClrUsed = ReadDWORD(Buffer, offset + 32)
            result.bmiHeader.biClrImportant = ReadDWORD(Buffer, offset + 36)
            result.bmiColors.rgbBlue = Buffer(offset + 40)
            result.bmiColors.rgbGreen = Buffer(offset + 41)
            result.bmiColors.rgbRed = Buffer(offset + 42)
            result.bmiColors.rgbReserved = Buffer(offset + 43)
            Return result
        End Function


        Private Shared Sub PutBitmap(ByVal DC As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal bmpData() As Byte)
            'Ähnlich wie, der _PUT Befehl, nur mit byte-Array als Parameter
            Dim bmpBits() As Byte ' Bitmap bits
            Dim BmpInf As BITMAPINFO
            Dim BmpHeader As BITMAPFILEHEADER
            Dim bmpHeaderData() As Byte 'Beinhaltet nur den Header

            If DC <> IntPtr.Zero Then
                If bmpData IsNot Nothing Then
                    If bmpData.Length > 0 Then
                        If DC <> IntPtr.Zero Then

                            ReDim bmpHeaderData(Marshal.SizeOf(BmpHeader) + Marshal.SizeOf(BmpInf))
                            Array.Copy(bmpData, bmpHeaderData, Marshal.SizeOf(BmpHeader) + Marshal.SizeOf(BmpInf))
                            BmpHeader = GetBITMAPFILEHEADER(bmpHeaderData)

                            If BmpHeader.bfType = 19778 Then 'BM
                                BmpInf = GetBITMAPINFO(bmpHeaderData, 14)

                                If BmpHeader.bfOffBits > 0 And BmpHeader.bfOffBits < bmpData.Length Then
                                    'Alte methode mit unmanaged memory war wohl schneller...
                                    ReDim bmpBits(bmpData.Length - BmpHeader.bfOffBits)
                                    Array.Copy(bmpData, BmpHeader.bfOffBits, bmpBits, 0, bmpData.Length - BmpHeader.bfOffBits)
                                    SetDIBitsToDevice(DC, x, y, BmpInf.bmiHeader.biWidth, BmpInf.bmiHeader.biHeight, 0, 0, 0, BmpInf.bmiHeader.biHeight, bmpBits, BmpInf, DIB_RGB_COLORS)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Shared Function LoadBrush(ByVal idx As Integer) As IntPtr
            Dim deffillBmp As String = ""
            Dim mem() As Byte = Nothing
            Dim memDC As IntPtr = IntPtr.Zero
            Dim hPatternBmp As IntPtr = IntPtr.Zero
            Dim hOldBmp As IntPtr = IntPtr.Zero

            If idx < 0 Then idx = 0
            If idx > 54 Then idx = 54
            If m_PatternBrushes(idx) = IntPtr.Zero Then
                m_PatternBitmaps(idx) = IntPtr.Zero
                Select Case idx
                    Case 37
                        m_PatternBrushes(idx) = CreateSolidBrush(_RGB(255, 255, 255))
                    Case 38
                        m_PatternBrushes(idx) = CreateSolidBrush(_RGB(198, 198, 198))
                    Case 39
                        m_PatternBrushes(idx) = CreateSolidBrush(_RGB(132, 132, 132))
                    Case 40
                        m_PatternBrushes(idx) = CreateSolidBrush(_RGB(66, 66, 66))
                    Case 41
                        m_PatternBrushes(idx) = CreateSolidBrush(_RGB(0, 0, 0))
                    Case Else
                        Try
                            deffillBmp = "Qk3+AQAAAAAAAD4AAAAoAAAAuAEAAAgAAAABAAEAAAAAAMABAAAlFgAAJRYAAAAAAAAAAAAAAAAAAP///wAAAAARVVVV3f//QAAEgMwA8VUQ7gED4RAiIswiEf//AsADAf/+AAAA/////+/+/e/8AAAAAAAAAAAiqqqq7v///xAgBQpAwADjAAAfxwEeOP8RZhURABEBYAMBAAAAAAD/////7/3+7/wAAAAAAAAAAAAARFVVVXf/EBACVQAYAcYRER8oAR58RIgziBEAEYAwhAEAAAAAAP/////v+3/vewAAAAAAAAAAiKqqqrv///8QKABABBsAbIIAExACHv6IRJlUEQARQBhIAQAAAAAA/////+/3v++3AAAAAAAAAAAAABFVVVXd//9EAEAIAwA+RBDuEAwe/xEizCIR//8gDDABAAAAAAD/////7+/f788AAAAAAAAAACKqqqru////AYJQoBAwAB8oAPF8MOH+/xFmUREAERAGMAEAAAAAAP///wDv3+8AzwAAAAAAAAAAAABEVVVVd/8BASBVALEIjRFV8YJI4XyIiDOIEQARCANIAQAAAAAA/////++/9++3AAAAAAAAAACIqqqqu////wGAAAQBjQDYAAAxAYThOEREmUURABEEgYQBAAAAAAD/////73/773sAAAAAAAAA"
                            hPatternBmp = CreateBitmap(8, 8, 1, 1, IntPtr.Zero) 'CreateBitmap(8, 8, 1, 1, IntPtr.Zero)
                            If hPatternBmp <> IntPtr.Zero Then
                                m_PatternBitmaps(idx) = hPatternBmp
                                memDC = CreateCompatibleDC(0)
                                hOldBmp = SelectObject(memDC, hPatternBmp)
                                PutBitmap(memDC, -8 * idx, 0, Convert.FromBase64String(deffillBmp))
                                m_PatternBrushes(idx) = CreatePatternBrush(hPatternBmp)
                            End If
                        Finally
                            If memDC <> IntPtr.Zero Then
                                SelectObject(memDC, hOldBmp)
                                DeleteDC(memDC)
                            End If
                            If hPatternBmp <> IntPtr.Zero And m_PatternBrushes(idx) = IntPtr.Zero Then
                                'CreatePatternBrush fehlgeschlagen -> bmp freigeben
                                DeleteObject(hPatternBmp)
                                m_PatternBitmaps(idx) = IntPtr.Zero
                            End If
                        End Try
                End Select
            End If
            Return m_PatternBrushes(idx)
        End Function
#End Region
#Region "Private Funktionen/Prozeduren"
        Private Sub IncRefPatternBrush(ByVal idx As Integer)
            If idx < 0 Then idx = 0
            If idx > 54 Then idx = 54
            m_PatternBrushUseCounts(idx) += 1
        End Sub

        Private Sub DecRefPatternBrush(ByVal idx As Integer)
            If idx < 0 Then idx = 0
            If idx > 54 Then idx = 54
            m_PatternBrushUseCounts(idx) -= 1
        End Sub

        Private Function GetBrush() As IntPtr
            Dim GDIBrush As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                GDIBrush = GetCurrentObject(m_DC, OBJ_BRUSH)
                'GDIBrush = SelectObject(m_DC, GetStockObject(NULL_BRUSH))
                'SelectObject(m_DC, GDIBrush) ' Wieder zurücksetzen
            End If
            Return GDIBrush
        End Function

        Private Function GetPen() As IntPtr
            Dim GDIPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                GDIPen = GetCurrentObject(m_DC, OBJ_PEN)
                'GDIPen = SelectObject(m_DC, GetStockObject(NULL_PEN))
                'SelectObject(m_DC, GDIPen) ' Wieder zurücksetzen
            End If
            Return GDIPen
        End Function

        Private Sub FreeUnusedBrushes()
            Dim idx As Integer = 0
            For idx = 0 To 54
                'ACHTUNG GEFÄHRLICH, ES MUSS SICHERGESTELLT WERDEN, DAS DIE OBJKETE NICHT MEHR VERWENDET WERDEN
                If m_PatternBrushUseCounts(idx) <= 0 Then
                    If m_PatternBrushes(idx) <> IntPtr.Zero Then
                        DeleteObject(m_PatternBrushes(idx))
                        m_PatternBrushes(idx) = IntPtr.Zero
                    End If
                    If m_PatternBitmaps(idx) <> IntPtr.Zero Then
                        DeleteObject(m_PatternBitmaps(idx))
                        m_PatternBitmaps(idx) = IntPtr.Zero
                    End If
                    m_PatternBrushUseCounts(idx) = 0
                End If
            Next
        End Sub

        Private Sub FreeDC()
            If m_DC <> IntPtr.Zero Then
                SelectObject(m_DC, m_OriginalPen)
                SelectObject(m_DC, m_OriginalBrush)
                If m_LastCreatedPen <> IntPtr.Zero Then
                    DeleteObject(m_LastCreatedPen)
                    m_LastCreatedPen = IntPtr.Zero
                End If
                DecRefPatternBrush(m_DefFillMode)
                FreeUnusedBrushes()
                If m_gr IsNot Nothing Then
                    m_gr.ReleaseHdc()
                    m_gr = Nothing
                End If
                m_DC = IntPtr.Zero
            End If
        End Sub

        Private Sub InitDC(ByVal DC As IntPtr)
            m_DC = DC
            m_BackColor = _RGB(255, 255, 255)
            m_ForeColor = _RGB(0, 0, 0)
            SetTextColor(DC, m_ForeColor)
            SetBkColor(DC, m_BackColor)
            m_OriginalBrush = GetBrush()
            m_OriginalPen = GetPen()
            m_PenColor = _RGB(0, 0, 0)
            m_PenStyle = PS_SOLID
            m_PenWidth = 1
            m_LastCreatedPen = CreatePen(m_PenStyle, m_PenWidth, m_PenColor)
            SelectObject(DC, m_LastCreatedPen)
            m_DefFillMode = 0
            SelectObject(DC, LoadBrush(0))
            m_PatternBrushUseCounts(0) += 1
        End Sub

        Private Sub SetPen(ByVal color As Long, ByVal width As Long, ByVal style As Long)
            Dim newPen As IntPtr = IntPtr.Zero
            Dim lastPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                If color <> m_PenColor Or width <> m_PenWidth Or m_PenStyle <> style Then
                    m_PenColor = color
                    m_PenStyle = style
                    m_PenWidth = width
                    newPen = CreatePen(style, width, color)
                    lastPen = SelectObject(m_DC, newPen)
                    If lastPen = m_LastCreatedPen And lastPen <> IntPtr.Zero Then
                        DeleteObject(lastPen)
                    End If
                    m_LastCreatedPen = newPen
                End If
            End If
        End Sub

        Private Sub SetPen(ByVal color As Long)
            SetPen(color, m_PenWidth, m_PenStyle)
        End Sub

#End Region
#Region "Öffentliche statische Prozeduren/Funktionen"
        Public Shared Property ActiveDrawing() As GFADrawing
            Get
                If m_ActiveDrawing Is Nothing Then
                    If GFAWindows.CurrentWindow IsNot Nothing Then
                        m_ActiveDrawing = GFAWindows.CurrentWindow.Drawing 'DC erst dann ermitteln, wenn benötigt
                        'GetDataFromActiveDC()
                    End If
                End If
                ActiveDrawing = m_ActiveDrawing
                Exit Property
            End Get
            Set(ByVal value As GFADrawing)

                If value IsNot Nothing Then
                    If m_ActiveDrawing IsNot Nothing Then
                        'Kein DC vom Fenster, also Ressourcen bereits hier freigeben
                        If m_ActiveDrawing.m_gr Is Nothing Then
                            m_ActiveDrawing.FreeDC()
                        End If
                    End If

                    m_ActiveDrawing = value
                End If
                Exit Property
            End Set
        End Property

        Public Shared ReadOnly Property ActiveDC()
            Get
                ActiveDC = IntPtr.Zero
                Dim drawing As GFADrawing = ActiveDrawing
                If drawing IsNot Nothing Then
                    ActiveDC = drawing.m_DC
                End If
                Exit Property
            End Get
        End Property

        Public Shared Sub ResetDC()
            m_ActiveDrawing = Nothing
        End Sub

        'Wird nur für CREATEMETA, CLOSEMETA verwendet
        Public Shared Property MetaDrawing() As Boolean
            Get
                MetaDrawing = m_MetaDrawing
                Exit Property
            End Get
            Set(ByVal value As Boolean)
                m_MetaDrawing = value
                Exit Property
            End Set
        End Property

        'Wird nur für CREATEMETA, CLOSEMETA verwendet
        Public Shared Property MetaLastDrawing() As GFADrawing
            Get
                MetaLastDrawing = m_MetaLastDrawing
                Exit Property
            End Get
            Set(ByVal value As GFADrawing)
                m_MetaLastDrawing = value
                Exit Property
            End Set
        End Property

        Public Shared Function QBColor(ByVal index As Integer) As Integer
            If index < 0 Then index = 0
            index = index Mod 16

            Select Case index
                Case 0
                    QBColor = &H0
                Case 1
                    QBColor = &H800000
                Case 2
                    QBColor = &H8000
                Case 3
                    QBColor = &H808000
                Case 4
                    QBColor = &H80
                Case 5
                    QBColor = &H800080
                Case 6
                    QBColor = &H8080
                Case 7
                    QBColor = &HC0C0C0
                Case 8
                    QBColor = &H808080
                Case 9
                    QBColor = &HFF0000
                Case 10
                    QBColor = &HFF00
                Case 11
                    QBColor = &HFFFF00
                Case 12
                    QBColor = &HFF
                Case 13
                    QBColor = &HFF00FF
                Case 14
                    QBColor = &HFFFF
                Case 15
                    QBColor = &HFFFFFF
                Case Else
                    QBColor = 0
            End Select
        End Function


#End Region
#Region "Öffentliche Funktionen/Prozeduren"

        Public Function CreateFontHandle() As IntPtr
            Return (CreateFont(m_Font_Height, m_Font_Width, m_Font_Escapement, m_Font_Orientation, m_Font_Weight, m_Font_Italic, m_Font_Underline, m_Font_Strikeout, m_Font_Charset, m_Font_OutPrecision, m_Font_ClipPrecision, m_Font_Quality, m_Font_Pitch + (m_Font_Family << 2), m_Font_FontName))
        End Function

        Public Sub SetFontCreationProperty(ByVal propertyName As String, ByVal value As Object)
            propertyName = UCase(Trim(propertyName))
            Select Case propertyName
                Case "NAME", "FONTNAME"
                    m_Font_FontName = value
                Case "WEIGHT"
                    m_Font_Weight = value
                Case "WIDTH"
                    m_Font_Width = value
                Case "HEIGHT"
                    m_Font_Height = value
                Case "ITALIC"
                    m_Font_Italic = value
                Case "STRIKEOUT"
                    m_Font_Strikeout = value
                Case "UNDERLINE"
                    m_Font_Underline = value
                Case "ESCAPEMENT"
                    m_Font_Escapement = value
                Case "FAMILY"
                    m_Font_Family = value
                Case "CHARSET"
                    m_Font_Charset = value
                Case "PITCH"
                    m_Font_Pitch = value
                Case "OUTPRECISION"
                    m_Font_OutPrecision = value
                Case "CLIPPRECISION"
                    m_Font_ClipPrecision = value
                Case "ORIENTATION"
                    m_Font_Orientation = value
                Case "QUALITY"
                    m_Font_Quality = value
                Case Else
                    Debug.WriteLine("FONT Property " & Chr(34) & propertyName & Chr(34) & " was not found!")
            End Select
        End Sub

        Public Sub DrawWMFFile(ByVal filename As String)
            Dim hmf As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                Try
                    hmf = GetMetaFile(filename)
                    If hmf <> IntPtr.Zero Then
                        PlayMetaFile(m_DC, hmf)
                        'In GFA wird hier auch nicht der orginal status wiederhergestellt (z.B. Textfarbe kann durch _PLAYMETA geändert werden)
                    End If
                Finally
                    If hmf <> IntPtr.Zero Then DeleteMetaFile(hmf)
                End Try
            End If
        End Sub

        Public Sub DrawBitmap(ByVal x As Integer, ByVal y As Integer, ByVal bitmap As IntPtr)
            Dim MemDC As IntPtr = IntPtr.Zero
            Dim oldBmp As IntPtr = IntPtr.Zero
            Dim bmpStruct As GDIBITMAP
            Try
                If m_DC <> IntPtr.Zero Then
                    MemDC = CreateCompatibleDC(0)
                    If MemDC <> IntPtr.Zero Then
                        oldBmp = SelectObject(MemDC, bitmap)
                        GDIGetObject(bitmap, Marshal.SizeOf(bmpStruct), bmpStruct)
                        BitBlt(m_DC, x, y, bmpStruct.bmWidth, bmpStruct.bmHeight, MemDC, 0, 0, SRCCOPY)
                        SelectObject(MemDC, oldBmp)
                    End If
                End If
            Finally
                If MemDC <> IntPtr.Zero Then
                    DeleteDC(MemDC)
                End If
            End Try
        End Sub

        Public Sub DrawBitmap(ByVal x As Integer, ByVal y As Integer, ByVal bmpStr As String)
            Dim bmpData() As Byte ' Bitmap bits
            Dim BmpInf As BITMAPINFO
            Dim BmpHeader As BITMAPFILEHEADER
            Dim bmpHeaderData() As Byte 'Beinhaltet nur den Header

            'Dim BmpPixels As IntPtr
            If bmpStr IsNot Nothing Then
                If bmpStr.Length > 0 Then
                    If m_DC <> IntPtr.Zero Then
                        bmpHeaderData = System.Text.ASCIIEncoding.Default.GetBytes(bmpStr, 0, Marshal.SizeOf(BmpHeader) + Marshal.SizeOf(BmpInf))
                        BmpHeader = GetBITMAPFILEHEADER(bmpHeaderData)

                        If BmpHeader.bfType = 19778 Then 'BM
                            BmpInf = GetBITMAPINFO(bmpHeaderData, 14)

                            If BmpHeader.bfOffBits > 0 And BmpHeader.bfOffBits < bmpStr.Length Then
                                'Alte methode mit unmanaged memeory war wohl schneller...
                                bmpData = System.Text.ASCIIEncoding.Default.GetBytes(bmpStr, BmpHeader.bfOffBits, bmpStr.Length - BmpHeader.bfOffBits)
                                SetDIBitsToDevice(m_DC, x, y, BmpInf.bmiHeader.biWidth, BmpInf.bmiHeader.biHeight, 0, 0, 0, BmpInf.bmiHeader.biHeight, bmpData, BmpInf, DIB_RGB_COLORS)
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Public Sub StretchBitmap(ByVal x As Integer, ByVal y As Integer, ByVal bitmap As IntPtr, ByVal width As Integer, ByVal height As Integer, ByVal ROP As Integer)
            Dim MemDC, oldBmp As IntPtr
            Dim bmpStruct As GDIBITMAP
            Dim lastStretchMode As Integer
            If m_DC <> IntPtr.Zero Then
                MemDC = CreateCompatibleDC(0)
                If MemDC <> IntPtr.Zero Then
                    oldBmp = SelectObject(MemDC, bitmap)
                    GDIGetObject(bitmap, Marshal.SizeOf(bmpStruct), bmpStruct)
                    SetStretchBltMode(MemDC, COLORONCOLOR)
                    lastStretchMode = SetStretchBltMode(DC, COLORONCOLOR)
                    StretchBlt(m_DC, x, y, width, height, MemDC, 0, 0, bmpStruct.bmWidth, bmpStruct.bmHeight, SRCCOPY)
                    SetStretchBltMode(DC, lastStretchMode)
                    SelectObject(MemDC, oldBmp)
                    DeleteDC(MemDC)
                End If
            End If
        End Sub

        Public Sub GrabBitmap(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef hBitmap As IntPtr)
            Dim MemDC As IntPtr = IntPtr.Zero
            Dim bmp As IntPtr = IntPtr.Zero
            Dim oldBmp As IntPtr = IntPtr.Zero

            hBitmap = IntPtr.Zero

            If m_DC <> IntPtr.Zero Then
                Try
                    bmp = CreateCompatibleBitmap(m_DC, width, height)
                    If bmp <> IntPtr.Zero Then
                        MemDC = CreateCompatibleDC(0)
                        If MemDC <> IntPtr.Zero Then
                            oldBmp = SelectObject(MemDC, bmp)
                            BitBlt(MemDC, 0, 0, width, height, m_DC, x, y, SRCCOPY)
                            SelectObject(MemDC, oldBmp)
                            hBitmap = bmp
                        End If
                    End If
                Finally
                    If MemDC <> IntPtr.Zero Then
                        DeleteDC(MemDC)
                    End If
                    If hBitmap <> IntPtr.Zero Then
                        'Alles Ok, Bitmaphandle wird zurückgegeben
                    Else
                        If bmp <> IntPtr.Zero Then
                            DeleteObject(bmp) ' Bitmap freigeben
                        End If
                    End If
                End Try
            End If
        End Sub

        Public Sub GrabBitmap(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef bmpStr As String)
            Dim MemDC As IntPtr = IntPtr.Zero
            Dim bmp As IntPtr = IntPtr.Zero
            Dim oldBmp As IntPtr = IntPtr.Zero
            Dim bmi As BITMAPINFO
            Dim bmfh As BITMAPFILEHEADER
            Dim bits() As Byte
            Dim bmpData() As Byte
            bmpStr = ""

            Try
                If m_DC <> IntPtr.Zero Then
                    bmp = CreateCompatibleBitmap(m_DC, width, height)
                    If bmp <> IntPtr.Zero Then
                        MemDC = CreateCompatibleDC(0)
                        If MemDC <> IntPtr.Zero Then
                            oldBmp = SelectObject(MemDC, bmp)
                            BitBlt(MemDC, 0, 0, width, height, m_DC, x, y, SRCCOPY)

                            bmfh.bfOffBits = Marshal.SizeOf(bmi) + Marshal.SizeOf(bmfh)
                            bmfh.bfSize = 14
                            bmfh.bfType = 19778 'BM
                            bmfh.bfReserved1 = 0
                            bmfh.bfReserved2 = 0

                            bmi.bmiHeader.biSize = Marshal.SizeOf(bmi.bmiHeader) ' muss 40 sein!
                            bmi.bmiHeader.biWidth = width
                            bmi.bmiHeader.biHeight = height ' -height wäre Top-Down
                            bmi.bmiHeader.biPlanes = 1
                            bmi.bmiHeader.biBitCount = 24
                            bmi.bmiHeader.biCompression = BI_RGB

                            'Benötigte Größe ermitteln
                            GetDIBits(MemDC, bmp, 0, height, Nothing, bmi, DIB_RGB_COLORS)
                            If bmi.bmiHeader.biSizeImage > 0 Then
                                ReDim bits(bmi.bmiHeader.biSizeImage)
                                GetDIBits(MemDC, bmp, 0, height, bits, bmi, DIB_RGB_COLORS)

                                ReDim bmpData(bmi.bmiHeader.biSizeImage + Marshal.SizeOf(bmi) + Marshal.SizeOf(bmfh))
                                Array.Copy(bits, 0, bmpData, Marshal.SizeOf(bmi) + Marshal.SizeOf(bmfh), bmi.bmiHeader.biSizeImage)
                                SetBITMAPFILEHEADER(bmpData, bmfh)
                                SetBITMAPINFO(bmpData, Marshal.SizeOf(bmfh), bmi)
                                bmpStr = System.Text.ASCIIEncoding.Default.GetString(bmpData)
                            End If
                            SelectObject(MemDC, oldBmp)
                        End If
                    End If
                End If
            Finally
                If MemDC <> IntPtr.Zero Then
                    DeleteDC(MemDC)
                End If
                If bmp <> IntPtr.Zero Then
                    DeleteObject(bmp)
                End If
            End Try
        End Sub

        Public Property BackColor() As Integer
            Get
                BackColor = m_BackColor
                Exit Property
            End Get
            Set(ByVal value As Integer)
                m_BackColor = value
                If m_DC <> IntPtr.Zero Then
                    SetBkColor(m_DC, m_BackColor)
                End If
                Exit Property
            End Set
        End Property

        Public Property ForeColor() As Integer
            Get
                ForeColor = m_ForeColor
                Exit Property
            End Get
            Set(ByVal value As Integer)
                m_ForeColor = value
                If m_DC <> IntPtr.Zero Then
                    SetTextColor(m_DC, m_ForeColor)
                End If
                Exit Property
            End Set
        End Property

        Public Sub New(ByVal DC As IntPtr)
            MyBase.New()
            'Me.m_ClipOffset = New Point(0, 0)
            If DC <> IntPtr.Zero Then
                m_gr = Nothing
                InitDC(DC)
            Else
                Throw New Exception("Invalid Device context " & DC.ToString)
            End If
        End Sub

        Public Sub New(ByVal gr As Graphics)
            MyBase.New()
            'Me.m_ClipOffset = New Point(0, 0)
            Dim DC As IntPtr = IntPtr.Zero
            If gr IsNot Nothing Then
                m_gr = gr
                DC = gr.GetHdc()
                If DC <> IntPtr.Zero Then
                    InitDC(DC)
                Else
                    Throw New Exception("Invalid Device context " & DC.ToString)
                End If
            Else
                Throw New Exception("Graphics object is nothing")
            End If
        End Sub

        Public Sub DefLine(ByVal style As Integer, ByVal width As Integer)
            SetPen(m_ForeColor, width, style)
        End Sub

        Public Sub DefLine(ByVal style As Integer)
            SetPen(m_ForeColor, m_PenWidth, style)
        End Sub

        Public Sub DefFill(ByVal mode As Integer)
            If mode < 0 Then mode = 0
            If mode > 54 Then mode = 54
            If m_DC <> IntPtr.Zero Then
                IncRefPatternBrush(mode)
                DecRefPatternBrush(m_DefFillMode)
                SelectObject(m_DC, LoadBrush(mode))
            End If
        End Sub

        Public Property PrintLocation() As Point
            Get
                PrintLocation = m_PrintLocation
                Exit Property
            End Get
            Set(ByVal value As Point)
                m_PrintLocation = value
                Exit Property
            End Set
        End Property

        Public Sub DrawTextString(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
            If m_DC <> IntPtr.Zero Then
                TextOut(m_DC, x, y, text, Len(text))
            End If
        End Sub




        Public Sub DrawTextString(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal text As String, ByVal style As Integer)
            If m_DC <> IntPtr.Zero Then
                Dim re As GDIRECT
                Dim length As Integer = Len(text)
                re.left = x1
                re.right = x2
                re.top = y1
                re.bottom = y2
                DrawText(m_DC, text, length, re, style)
                'DrawTextString(x1, y1, text)
            End If
        End Sub

        Public ReadOnly Property DC()
            Get
                DC = m_DC
                Exit Property
            End Get
        End Property

        Public Sub DrawCircle(ByVal x As Integer, ByVal y As Integer, ByVal r As Integer, ByVal filled As Boolean)
            'Circle ist immer nicht gefüllt (<>PCIRCLE)
            Dim oldBrush As IntPtr = IntPtr.Zero
            Dim oldPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                If filled Then

                    If m_DefFillMode = 0 Then
                        oldPen = SelectObject(m_DC, GetStockObject(NULL_PEN))
                        Ellipse(m_DC, x - r / 2, y - r / 2, x + r / 2, y + r / 2)
                        SelectObject(m_DC, oldPen)
                    Else
                        SetPen(m_ForeColor)
                        Ellipse(m_DC, x - r / 2, y - r / 2, x + r / 2, y + r / 2)
                    End If

                Else
                    SetPen(m_ForeColor)
                    oldBrush = SelectObject(m_DC, GetStockObject(NULL_BRUSH))
                    Ellipse(m_DC, x - r / 2, y - r / 2, x + r / 2, y + r / 2)
                    SelectObject(m_DC, oldBrush) ' Brush wiederherstellen
                End If
            End If
        End Sub

        Public Sub DrawEllipse(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal filled As Boolean)
            Dim oldBrush As IntPtr = IntPtr.Zero
            Dim oldPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then

                If filled Then

                    If m_DefFillMode = 0 Then
                        oldPen = SelectObject(DC, GetStockObject(NULL_PEN))
                        Ellipse(m_DC, x, y, x + w, y + h)
                        SelectObject(m_DC, oldPen)
                    Else
                        SetPen(m_ForeColor)
                        Ellipse(m_DC, x, y, x + w, y + h)
                    End If

                Else
                    SetPen(m_ForeColor)
                    oldBrush = SelectObject(m_DC, GetStockObject(NULL_BRUSH))
                    Ellipse(m_DC, x, y, x + w, y + h)
                    SelectObject(m_DC, oldBrush) ' Brush wiederherstellen
                End If
            End If
        End Sub

        Public Sub DrawLine(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
            If m_DC <> IntPtr.Zero Then
                SetPen(m_ForeColor)
                MoveToEx(m_DC, x1, y1, 0)
                LineTo(m_DC, x2, y2)
            End If
        End Sub

        Public Sub DrawPixel(ByVal x As Integer, ByVal y As Integer)
            If m_DC <> IntPtr.Zero Then
                SetPixelV(m_DC, x, y, m_ForeColor)
            End If
        End Sub

        Public Function GetPixelColor(ByVal x As Integer, ByVal y As Integer) As Integer
            If m_DC <> IntPtr.Zero Then
                GetPixelColor = GetPixel(m_DC, x, y)
            Else
                GetPixelColor = -1
            End If
        End Function

        Public Sub Clear()
            If m_DC <> IntPtr.Zero Then
                'BitBlt(m_DC, 0, 0, My.Computer.Screen.Bounds.Size.Width, My.Computer.Screen.Bounds.Size.Height, IntPtr.Zero, 0, 0, WHITENESS)
                BitBlt(m_DC, 0, 0, GetClientWidth, GetClientHeight, IntPtr.Zero, 0, 0, WHITENESS)
            End If
            m_PrintLocation = New Point(0, 0)
        End Sub

        Public Sub ClearRectangle(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            If m_DC <> IntPtr.Zero Then
                BitBlt(m_DC, x, y, width, height, IntPtr.Zero, 0, 0, WHITENESS)
            End If
        End Sub

        Public Sub Fill(ByVal x As Integer, ByVal y As Integer, ByVal color As Integer, ByVal fillBorder As Boolean)
            If m_DC <> IntPtr.Zero Then
                If fillBorder Then
                    ExtFloodFill(m_DC, x, y, color, FLOODFILLBORDER)
                Else
                    ExtFloodFill(m_DC, x, y, color, FLOODFILLSURFACE)
                End If
            End If
        End Sub

        Public Sub DrawRectangle(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal filled As Boolean)
            Dim oldBrush As IntPtr = IntPtr.Zero
            Dim oldPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                If filled Then

                    If m_DefFillMode = 0 Then
                        oldPen = SelectObject(DC, GetStockObject(NULL_PEN))
                        'If a PS_NULL pen is used, the dimensions of the rectangle are 1 pixel less in height and 1 pixel less in width.
                        Rectangle(m_DC, x1, y1, x2 + 1, y2 + 1) ' Da Rand nicht gezeichnet wird (Ansonsten zu klein)
                        SelectObject(m_DC, oldPen)
                    Else
                        SetPen(m_ForeColor)
                        Rectangle(m_DC, x1, y1, x2, y2)
                    End If

                Else
                    SetPen(m_ForeColor)
                    oldBrush = SelectObject(m_DC, GetStockObject(NULL_BRUSH))
                    Rectangle(m_DC, x1, y1, x2, y2)
                    SelectObject(m_DC, oldBrush) ' Brush wiederherstellen
                End If
            End If
        End Sub

        Public Sub DrawPolygon(ByVal n As Integer, ByRef x_array() As Double, ByRef y_array() As Double, ByVal filled As Boolean)
            'Mit GDIPlus...
            'Dim tmppen As New Drawing.Pen(System.Drawing.Color.FromArgb(ForeColor And &HFF, (ForeColor >> 8) And &HFF, ForeColor >> 16))
            'gr.DrawPolygon(tmppen, points)
            Dim i As Integer = 0
            Dim oldPen As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero And n > 0 Then
                Dim points(n) As Point

                If DC <> IntPtr.Zero Then
                    For i = 0 To n - 1
                        points(i).X = x_array(i)
                        points(i).Y = y_array(i)
                    Next
                End If

                If filled Then
                    If m_DefFillMode = 0 Then
                        oldPen = SelectObject(m_DC, GetStockObject(NULL_PEN))
                        Polygon(m_DC, points, n)
                        SelectObject(m_DC, oldPen)
                    Else
                        SetPen(m_ForeColor)
                        Polygon(m_DC, points, n)
                    End If

                Else
                    SetPen(m_ForeColor)
                    Polyline(m_DC, points, n)
                End If
            End If
        End Sub

        Public Sub GraphMode(ByVal Rop2 As Integer, ByVal BkMode As Integer)
            If m_DC <> IntPtr.Zero Then
                SetROP2(m_DC, Rop2)
                SetBkMode(m_DC, BkMode)
            End If
        End Sub

        Public Sub GraphMode(ByVal Rop2 As Integer)
            If m_DC <> IntPtr.Zero Then
                SetROP2(m_DC, Rop2)
            End If
        End Sub

        Public Sub FreeBitmap(ByVal hBitmap As IntPtr)
            If GetObjectType(hBitmap) = OBJ_BITMAP Then
                DeleteObject(hBitmap)
            Else
                Debug.WriteLine("Invalid call of FreeBitmap " & hBitmap.ToString)
            End If
        End Sub

        Public Sub FreeFont(ByVal hFont As IntPtr)
            If GetObjectType(hFont) = OBJ_FONT Then
                DeleteObject(hFont)
            Else
                Debug.WriteLine("Invalid call of FreeFont " & hFont.ToString)
            End If
        End Sub

        Public Sub ScrollHV(ByVal dx As Integer, ByVal dy As Integer)
            Dim bmp As IntPtr = IntPtr.Zero
            If m_DC <> IntPtr.Zero Then
                If dx <> 0 Or dy <> 0 Then 'Speedoptimierung
                    GrabBitmap(0, 0, GetClientWidth, GetClientHeight, bmp)
                    If bmp <> IntPtr.Zero Then
                        Try
                            DrawBitmap(dx, dy, bmp)
                        Finally
                            FreeBitmap(bmp)
                        End Try
                    End If
                End If
            End If
        End Sub

        Public Sub Clip(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            If m_DC Then
                'GFA verwendet auch eine ClipRgn!
                'm_ClipOffset = New Point(0, 0)
                SelectClipRgn(m_DC, Nothing)
                BeginPath(m_DC)
                Rectangle(m_DC, x, y, x + width, y + height)
                EndPath(m_DC)
                SelectClipPath(m_DC, RGN_AND)
                SetViewportOrgEx(m_DC, 0, 0, Nothing)

                'SetViewportExtEx(m_DC, width, height, Nothing)
                'SetViewportOrgEx(m_DC, x, y, Nothing)
                'OffsetViewportOrgEx(m_DC, 0, 0, Nothing)
            End If
        End Sub

        Public Sub Clip(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal offsetX As Integer, ByVal offsetY As Integer)
            If m_DC Then
                'GFA verwendet auch eine ClipRgn!
                SelectClipRgn(m_DC, Nothing)
                BeginPath(m_DC)
                Rectangle(m_DC, x, y, x + width, y + height)
                EndPath(m_DC)
                SelectClipPath(m_DC, RGN_AND)
                SetViewportOrgEx(m_DC, offsetX, offsetY, Nothing)

                'm_ClipOffset = New Point(offsetX, offsetY)
                'OffsetViewportOrgEx(m_DC, offsetX, offsetY, Nothing)
                'SetViewportExtEx(m_DC, width, height, Nothing)
                'SetViewportOrgEx(m_DC, x, y, Nothing)
            End If

        End Sub

        Public Sub ClipOff()
            If m_DC Then
                'm_ClipOffset = New Point(0, 0)
                'OffsetViewportOrgEx(m_DC, 0, 0, Nothing)
                'SetViewportExtEx(m_DC, ClientWidth, ClientHeight, Nothing)
                'OffsetViewportOrgEx(m_DC, 0, 0, Nothing)
                SetViewportOrgEx(m_DC, 0, 0, Nothing)
                SelectClipRgn(m_DC, Nothing)
            End If
        End Sub


        Public Sub PrintText(ByVal scroll As Boolean, ByVal lf As Boolean, ByVal ParamArray texts() As Object)
            Dim cWidth As Integer
            Dim cHeight As Integer
            Dim posX, posY, maxPosY, maxPosX As Integer
            Dim text, ch As String
            Dim resX, resY As Integer
            Dim j As Integer
            Dim i As Integer

            If m_DC <> IntPtr.Zero Then
                resX = GetClientWidth()
                resY = GetClientHeight()
                If m_MetaDrawing Then
                    'Auflösung ist unbeschränkt
                    resX = &HFFFF
                    resY = &HFFFF
                End If

                'TOTO? Leider gibt TextWidth/Height für MetaDC 0 zurück...
                cWidth = GetTextWidth("_")
                cHeight = GetTextHeight("|")
                If cHeight < 1 Then ' verhindert teilen durch 0
                    cHeight = 1
                End If
                If cWidth < 1 Then ' verhindert teilen durch 0
                    cWidth = 1
                End If

                posY = cHeight * m_PrintLocation.Y
                posX = cWidth * m_PrintLocation.X

                maxPosY = (resY) / cHeight
                maxPosY -= 2
                maxPosY *= cHeight
                If maxPosY < 0 Then
                    maxPosY = 0
                End If

                maxPosX = resX - cWidth

                For j = 0 To texts.Length - 1
                    text = texts(j)
                    If j = texts.Length - 1 Then 'Beim letzten Chr 10 und 13 anhängen
                        If lf Then
                            text += Chr(10) + Chr(13)
                        End If
                    End If
                    If j > 0 Then ' An 16er Blöcke anordnen
                        posX = posX / cWidth
                        posX = posX / 16
                        posX += 1
                        posX *= 16
                        posX *= cWidth
                    End If

                    For i = 1 To Len(text)
                        ch = Mid(text, i, 1)

                        If ch = Chr(10) Then 'LF
                            m_PrintLocation = New Point(0, m_PrintLocation.Y)
                            posX = 0
                        ElseIf ch = Chr(13) Then
                            m_PrintLocation = New Point(m_PrintLocation.X, m_PrintLocation.Y + 1)
                            posY += cHeight
                        Else

                            If posX > maxPosX Then
                                posX = 0
                                posY += cHeight
                                m_PrintLocation = New Point(0, m_PrintLocation.Y + 1)
                            End If

                            If posY > maxPosY Then
                                'Funktioniert unter Windows 7 nicht... nur unter Vista
                                'ScrollWindowEx(wnd.Form.Handle, 0, -cHeight, Nothing, Nothing, IntPtr.Zero, Nothing, 0)
                                'UpdateWindow(wnd.Form.Handle) 'Fenster muss neugezeichnet werden, UpdateWindow umgeht die Event-Queue sodass kein Application.DoEvents() nötig ist.

                                If GetDeviceCaps(m_DC, TECHNOLOGY) = DT_RASPRINTER Then
                                    'Wenn Ausdruck, dann neue Seite
                                    Escape(m_DC, NEWFRAME, 0, Nothing, Nothing)
                                    'Komplett bei 0,0 anfangen 
                                    posY = 0
                                    posX = 0
                                    m_PrintLocation = New Point(0, 0)

                                Else
                                    If m_MetaDrawing = False And scroll = True Then 'Kein Scrolling bei Meta drawing (zu groß)
                                        ScrollHV(0, -cHeight)
                                    End If
                                    posY = maxPosY
                                End If


                            End If

                            DrawTextString(posX, posY, ch)
                            posX += GetTextWidth(ch)
                        End If
                    Next
                Next
            End If
        End Sub

        Public Sub SetSystemFont(ByVal fontID As Integer)
            If m_DC <> IntPtr.Zero Then
                Dim handle As IntPtr = GetStockObject(fontID)
                If GetObjectType(handle) = OBJ_FONT Then 'Sicherstellen, dass es eine Schriftart ist
                    If handle <> IntPtr.Zero Then
                        SelectObject(m_DC, handle)
                    End If
                End If
            End If
        End Sub

        Public Sub SetFont(ByVal fnt As IntPtr)
            If m_DC <> IntPtr.Zero Then
                If GetObjectType(fnt) = OBJ_FONT Then
                    SelectObject(m_DC, fnt)
                End If
            End If
        End Sub

        Public Function GetClientWidth() As Integer
            Dim rect As GDIRECT
            rect.left = 0
            rect.right = 0
            rect.top = 0
            rect.bottom = 0
            If m_DC <> IntPtr.Zero Then
                GetClipBox(m_DC, rect)
            End If
            Return rect.right - rect.left
        End Function

        Public Function GetClientHeight() As Integer
            Dim rect As GDIRECT
            rect.left = 0
            rect.right = 0
            rect.top = 0
            rect.bottom = 0
            If m_DC <> IntPtr.Zero Then
                GetClipBox(m_DC, rect)
            End If
            Return rect.bottom - rect.top
        End Function

        Public Function GetTextWidth(ByVal text As String) As Integer
            'Gibt bei MetaDC 0 zurück...
            Dim sz As Size = New Size(0, 0)
            If m_DC <> IntPtr.Zero Then
                GetTextExtentPoint32(m_DC, text, Len(text), sz)
            End If
            Return sz.Width
        End Function

        Public Function GetTextHeight(ByVal text As String) As Integer
            'Gibt bei MetaDC 0 zurück...
            Dim sz As Size = New Size(0, 0)
            If m_DC <> IntPtr.Zero Then
                GetTextExtentPoint32(m_DC, text, Len(text), sz)
            End If
            Return sz.Height
        End Function
#End Region
#Region "IDisposable Support"
        Public Function IsDisposed() As Boolean
            Return m_IsDisposed
        End Function
        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.m_IsDisposed Then
                If disposing Then
                    ' TODO: Anderen Zustand freigeben (verwaltete Objekte).
                End If

                ' TODO: Eigenen Zustand freigeben (nicht verwaltete Objekte).
                ' TODO: Große Felder auf NULL festlegen.
                FreeDC()
            End If
            Me.m_IsDisposed = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        'Wird automatisch aufgerufen
        Protected Overrides Sub Finalize()
            ' Do not re-create Dispose clean-up code here.
            ' Calling Dispose(false) is optimal in terms of
            ' readability and maintainability.
            Dispose(False)
            MyBase.Finalize()
        End Sub
#End Region
    End Class
    Class GFAEvent
        Protected m_hWnd As IntPtr
        Protected m_Msg As Integer
        Protected m_ID As Integer
        Protected m_wParam As Integer
        Protected m_lParam As Integer

        Public Sub New(ByVal ID As Integer, ByVal hWnd As IntPtr, ByVal Msg As Integer)
            m_hWnd = hWnd
            m_ID = ID
            m_Msg = Msg
            m_wParam = 0
            m_lParam = 0
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
            m_hWnd = hWnd
            m_ID = ID
            m_Msg = Msg
            m_wParam = wParam
            m_lParam = lParam
        End Sub

        Public Property HWnd() As String
            Get
                HWnd = m_hWnd
                Exit Property
            End Get
            Set(ByVal value As String)
                m_hWnd = value
                Exit Property
            End Set
        End Property

        Public Property ID() As String
            Get
                ID = m_ID
                Exit Property
            End Get
            Set(ByVal value As String)
                m_ID = value
                Exit Property
            End Set
        End Property

        Public Property Msg() As String
            Get
                Msg = m_Msg
                Exit Property
            End Get
            Set(ByVal value As String)
                m_Msg = value
                Exit Property
            End Set
        End Property

        Public Property WParam() As String
            Get
                WParam = m_wParam
                Exit Property
            End Get
            Set(ByVal value As String)
                m_wParam = value
                Exit Property
            End Set
        End Property

        Public Property LParam() As String
            Get
                LParam = m_lParam
                Exit Property
            End Get
            Set(ByVal value As String)
                m_lParam = value
                Exit Property
            End Set
        End Property


    End Class
    Class GFAWindows
        Private Const MAX_GFAWINDOWS = 31
        Private Const MAX_GFA_EVENTS As Integer = 50
        Private Shared m_Wnds(MAX_GFAWINDOWS) As GFAWindow
        Private Shared m_Cursor As Cursor = Cursors.Default
        Private Shared m_Inkey As String = ""
        Private Shared m_CurrentWnd As GFAWindow = Nothing
        Private Shared m_Events As List(Of GFAEvent) = New List(Of GFAEvent)
        Private Shared m_CustomFormClasses(MAX_GFAWINDOWS) As System.Type

        Public Shared Sub CloseAll()
            Dim idx As Integer
            For idx = 0 To MAX_GFAWINDOWS
                If m_Wnds(idx) IsNot Nothing Then
                    m_Wnds(idx).Close()
                    m_Wnds(idx) = Nothing
                End If
            Next
        End Sub

        Public Shared Function FindWindowFromDC(ByVal DC As IntPtr)
            Dim idx As Integer
            For idx = 0 To MAX_GFAWINDOWS
                If m_Wnds(idx) IsNot Nothing Then
                    If m_Wnds(idx).IsForm Then 'Besser auch IsForm überprüfen
                        If m_Wnds(idx).Drawing IsNot Nothing Then
                            If m_Wnds(idx).Drawing.DC = DC Then
                                Return idx
                            End If
                        End If
                    End If
                End If
            Next
            Return -1
        End Function

        Public Shared Sub AddGFAWindow(ByVal ID As Integer, ByVal frm As Form)
            Dim wnd As GFAWindow

            If ID > MAX_GFAWINDOWS Or ID < 0 Then
                Throw New ArgumentException("Wrong window no " & ID)
            End If

            If frm Is Nothing Then
                If m_CustomFormClasses(ID) IsNot Nothing Then
                    frm = Activator.CreateInstance(m_CustomFormClasses(ID))
                    frm.Show()
                End If
            End If

            RemoveGFAWindow(ID) ' Sicherstellen das es kein Fenster mit der gleichen ID gibt 
            If frm IsNot Nothing Then
                If frm.IsDisposed = False Then 'Nur, wenn Form nicht disposed ist (Problem, wenn MAKEWIN 2x aufgerufen wird)
                    wnd = New GFAWindow(ID, frm)
                Else
                    wnd = Nothing
                End If
            Else
                wnd = New GFAWindow(ID, Nothing)
            End If

            'Nur wenn erfolgreich...
            If wnd IsNot Nothing Then
                m_Wnds(ID) = wnd
                CurrentWindow = wnd ' Fenster als current setzen
            End If
        End Sub

        Public Shared Sub RemoveGFAWindow(ByVal ID As Integer)
            Dim idx As Integer
            Dim idx_Count As Integer = -1
            Dim max_Count As Integer = -1
            If ID < MAX_GFAWINDOWS And ID >= 0 Then ' Keinen Exception auslösen
                If m_Wnds(ID) IsNot Nothing Then

                    If m_Wnds(ID) Is m_CurrentWnd Then 'Problem! das aktive Fenster soll entfernt werden
                        CurrentWindow = Nothing

                        For idx = 0 To MAX_GFAWINDOWS - 1
                            If m_Wnds(idx) IsNot Nothing Then
                                If m_Wnds(idx).IsForm Then '2010-10-01 hinz.
                                    If m_Wnds(idx).CreateCount > max_Count Then
                                        max_Count = m_Wnds(idx).CreateCount
                                        idx_Count = idx
                                    End If
                                End If
                            End If
                        Next

                        If idx_Count > -1 Then
                            'Neues aktives Fenster gefunden!
                            CurrentWindow = m_Wnds(idx_Count)
                        End If

                    End If
                    'Fenster entfernen
                    m_Wnds(ID).Close()
                    m_Wnds(ID) = Nothing
                End If
            End If
        End Sub

        Public Shared Sub SetCurrentWindow(ByVal ID As Integer)
            If ID < MAX_GFAWINDOWS And ID >= 0 Then ' Keinen Exception auslösen
                If m_Wnds(ID) IsNot Nothing Then
                    CurrentWindow = m_Wnds(ID) ' Fenster als current setzen
                End If
            End If
        End Sub

        Public Shared Function GetWindow(ByVal ID As Integer) As GFAWindow
            If ID > MAX_GFAWINDOWS Or ID < 0 Then
                GetWindow = Nothing
            Else
                GetWindow = Nothing
                If m_Wnds(ID) IsNot Nothing Then
                    GetWindow = m_Wnds(ID)
                End If
            End If
        End Function

        Public Shared Property CurrentCursor() As Cursor
            Get
                CurrentCursor = m_Cursor
                Exit Property
            End Get
            Set(ByVal value As Cursor)
                m_Cursor = value
                For Each wnd As GFAWindow In m_Wnds
                    If wnd IsNot Nothing Then
                        wnd.CurrentCursor = value
                    End If
                Next
                Exit Property
            End Set
        End Property

        Public Shared Property INKEY() As String
            Get
                INKEY = m_Inkey
                m_Inkey = "" ' Zeichen darf nur einmal zurückgegeben werden!
                Exit Property
            End Get
            Set(ByVal value As String) ' Sollte eingetlich nur lesbar sein(Muss in GFA Window aber gesetzt werden)
                m_Inkey = value
                Exit Property
            End Set
        End Property

        Public Shared ReadOnly Property Events() As List(Of GFAEvent)
            Get
                Events = m_Events
                Exit Property
            End Get
        End Property

        Public Shared Sub AddEvent(ByVal hWnd As IntPtr, ByVal ID As Integer, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
            If m_Events IsNot Nothing Then ' Sollte niemals der Fall sein

                m_Events.Add(New GFAEvent(ID, hWnd, Msg, wParam, lParam))
                If m_Events.Count > MAX_GFA_EVENTS Then ' Verhindert, das sich zuviele Events aufstauen(z.B. bei INKEY$)
                    m_Events.RemoveAt(0) ' Den ersten und ältesten Einträg entfernen 
                End If
            End If
        End Sub

        Public Shared Property CurrentWindow() As GFAWindow
            Get
                Dim result As GFAWindow = Nothing
                If m_CurrentWnd IsNot Nothing Then
                    result = m_CurrentWnd
                End If
                CurrentWindow = result
                Exit Property
            End Get
            Set(ByVal value As GFAWindow)
                GFADrawing.ResetDC() ' Muss immer resettet werden, wenn sich das Fenster ändert
                m_CurrentWnd = value
                Exit Property
            End Set
        End Property

        Public Shared Sub RegisterCustomForm(ByVal ID As Integer, ByVal frmType As System.Type)
            If ID >= 0 And ID <= MAX_GFAWINDOWS Then
                m_CustomFormClasses(ID) = frmType
            End If
        End Sub

    End Class
    'DesignerCategory("code") verhindert Anzeigefehler von Designer
    <System.ComponentModel.DesignerCategory("code")> _
    Class GFAToolStripMenuItem
        Inherits ToolStripMenuItem

        Dim m_hWnd As IntPtr
        Dim m_ID As Integer
        Public Sub New(ByVal hWnd As IntPtr, ByVal ID As Integer)
            m_hWnd = hWnd
            m_ID = ID
            AddHandler Me.Click, AddressOf __Click
        End Sub

        Private Sub __Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If TypeOf sender Is GFAToolStripMenuItem Then
                Dim item As GFAToolStripMenuItem = DirectCast(sender, GFAToolStripMenuItem)
                GFAWindows.AddEvent(m_hWnd, m_ID, WM_COMMAND, item.Tag And &HFFFF, 0)
            End If
        End Sub
    End Class
    'DesignerCategory("code") verhindert Anzeigefehler von Designer
    <System.ComponentModel.DesignerCategory("code")> _
    Class GFAMenuItem
        Inherits MenuItem
        Dim m_hWnd As IntPtr
        Dim m_ID As Integer

        Public Sub New(ByVal hWnd As IntPtr, ByVal ID As Integer)
            m_hWnd = hWnd
            m_ID = ID
            AddHandler Me.Click, AddressOf __Click
        End Sub

        Private Sub __Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If TypeOf sender Is GFAMenuItem Then
                Dim item As GFAMenuItem = DirectCast(sender, GFAMenuItem)
                GFAWindows.AddEvent(m_hWnd, m_ID, WM_COMMAND, item.Tag And &HFFFF, 0)
            End If
        End Sub
    End Class
    Class GFAWindow
        Protected m_Frm As Form
        Protected m_ID As Integer
        Protected m_CancelClose As Boolean
        Protected m_CreateCount As Integer
        Protected m_Graphics As Graphics
        'Protected m_DC As IntPtr
        Protected m_Drawing As GFADrawing
        'Protected m_Menu As GFAMenu
        'Protected m_PrintPos As Point

        'Für Backbuffering...
        Protected m_MemDC As IntPtr
        Protected m_MemBmp As IntPtr
        Protected m_PlusBmp As Bitmap
        Protected m_intGr As Graphics

        Shared m_CreateCounter As Integer ' Anzahl der erstellten Fenster

        Public Function IsForm() As Boolean ' Gibt zurück, ob ein gültiges Form vorhanden ist
            IsForm = False
            If m_Frm IsNot Nothing Then
                If m_Frm.IsDisposed = False Then 'Fenster darf nicht disposed sein! (Schließen des Hauptfensters, wenn ein GFA-Fenster offen ist)
                    IsForm = True
                End If
            End If
        End Function

        'Public ReadOnly Property DC() As IntPtr
        'Get
        'If m_DC = IntPtr.Zero Then
        'If m_Graphics Is Nothing Then
        'm_Graphics = m_Frm.CreateGraphics()
        'End If
        'If m_Graphics IsNot Nothing Then
        ''m_DC = m_Graphics.GetHdc()
        ''GFADrawing.InitDC(m_DC) '2010-09-23
        'End If
        'End If
        'DC = m_DC
        'Exit Property
        'End Get
        'End Property


        Public Sub TEST_DOUBLEBUFFERING()
            Dim dc As IntPtr = CreateDC("display", "", 0, 0)

            m_intGr = m_Frm.CreateGraphics
            m_PlusBmp = New Bitmap(1920, 1080, m_intGr)

            m_MemBmp = m_PlusBmp.GetHbitmap()

            m_MemDC = CreateCompatibleDC(dc)
            'm_MemBmp = CreateCompatibleBitmap(dc, 1920, 1080)
            SelectObject(m_MemDC, m_MemBmp)
            'm_Graphics = Graphics.FromImage(m_PlusBmp)
            m_Drawing = New GFADrawing(m_MemDC)
            GFADrawing.ResetDC()
            GFADrawing.ActiveDrawing = Nothing
        End Sub

        Public Sub PAINT_TEST()
            Dim gr As Graphics = m_Frm.CreateGraphics
            'If m_intGr IsNot Nothing Then

            '    m_intGr.DrawImage(m_PlusBmp, New PointF(0, 0))
            'End If
            Dim DC As IntPtr = gr.GetHdc
            Dim res As Integer = BitBlt(DC, 0, 0, 1920, 1080, m_MemDC, 0, 0, SRCCOPY)
            gr.ReleaseHdc()
        End Sub

        Private Sub CreateGraphics()
            m_Graphics = m_Frm.CreateGraphics()
        End Sub

        Public ReadOnly Property Drawing() As GFADrawing
            Get
                If m_Drawing Is Nothing Then
                    If m_Graphics Is Nothing Then
                        If m_Frm.IsDisposed = False Then
                            CreateGraphics()
                        End If
                    End If
                    If m_Graphics IsNot Nothing Then
                        m_Drawing = New GFADrawing(m_Graphics)
                    End If
                End If
                Drawing = m_Drawing
                Exit Property
            End Get
        End Property

        Public Sub New(ByVal ID As Integer, ByVal frm As Form)
            If frm Is Nothing Then
                frm = New Form
                frm.Width = My.Computer.Screen.Bounds.Size.Width / 2
                frm.Height = My.Computer.Screen.Bounds.Size.Height / 2
                frm.BackColor = Color.White
                frm.Text = "Window #" & ID
                frm.ShowIcon = False ' Kein .Net Icon
                frm.Show()
            End If

            m_Frm = frm
            m_ID = ID
            frm.KeyPreview = True
            frm.Cursor = GFAWindows.CurrentCursor
            m_CreateCounter += 1
            m_CreateCount = m_CreateCounter
            m_CancelClose = True
            m_Graphics = Nothing ' Is hier problematisch (Problem mit child windows..)
            'm_DC = IntPtr.Zero ' Erst ermitteln, wenn gebraucht
            'm_PrintPos = New Point(0, 0)
            m_Drawing = Nothing
            AddHandler frm.KeyPress, AddressOf __KeyPress
            AddHandler frm.FormClosing, AddressOf __FormClosing
            AddHandler frm.Paint, AddressOf __Paint
            AddHandler frm.SizeChanged, AddressOf __SizeChanged
            AddHandler frm.KeyDown, AddressOf __KeyDown
        End Sub

        Public ReadOnly Property ID() As Integer
            Get
                ID = m_ID
                Exit Property
            End Get
        End Property

        Public ReadOnly Property CreateCount() As Integer
            Get
                CreateCount = m_CreateCount
                Exit Property
            End Get
        End Property

        Public ReadOnly Property Form() As Form
            Get
                Form = Nothing
                If IsForm() Then
                    Form = m_Frm
                End If
                Exit Property
            End Get
        End Property

        Public ReadOnly Property Graph() As Graphics
            Get
                If m_Graphics Is Nothing Then
                    CreateGraphics()
                End If
                Graph = m_Graphics
                Exit Property
            End Get
        End Property

        Public Property CurrentCursor() As Cursor
            Get
                CurrentCursor = Nothing
                If IsForm() Then
                    CurrentCursor = m_Frm.Cursor
                End If
                Exit Property
            End Get
            Set(ByVal value As Cursor)
                If IsForm() Then
                    m_Frm.Cursor = value
                End If
                Exit Property
            End Set
        End Property

        '        Public Property PrintPos() As Point
        '            Get
        '                PrintPos = m_PrintPos
        '                Exit Property
        '            End Get
        '            Set(ByVal value As Point)
        '                m_PrintPos = value
        '                Exit Property
        '            End Set
        '        End Property

        'Public Property Menu() As GFAMenu
        '    Get
        '        Menu = m_Menu
        '        Exit Property
        '    End Get
        '    Set(ByVal value As GFAMenu)
        '        m_Menu = value
        '        Exit Property
        '    End Set
        'End Property

        Public Sub Close()
            'If m_DC <> IntPtr.Zero Then
            'If m_Graphics IsNot Nothing Then
            'DeleteObject(SelectObject(m_DC, GetStockObject(BLACK_PEN))) ' Free Pen Ressource
            'SelectObject(m_DC, GetStockObject(WHITE_BRUSH)) ' Sicherstellen, dass Pattern Brush nicht mehr gesetzt ist
            'm_Graphics.ReleaseHdc(m_DC)

            'End If
            'End If
            'm_DC = IntPtr.Zero
            If m_Drawing IsNot Nothing Then
                If m_Drawing.IsDisposed = False Then
                    m_Drawing.Dispose() 'TEST 29-10-2010
                End If
            End If

            m_Drawing = Nothing ' Dispose solle von selbst aufgerufen werden, aber nicht unbedingt rechtzeitig, deshalb selbst aufrufen (siehe oben)
            m_Graphics = Nothing
            m_CancelClose = False ' Muss schließbar sein
            If IsForm() Then
                m_Frm.Close()
            End If
        End Sub

        Private Sub __KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
            GFAWindows.INKEY = e.KeyChar
            If IsForm() Then
                GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_CHAR, AscW(e.KeyChar), 1) 'Nicht gleich wie bei Windows API!
            End If
        End Sub

        Private Sub __FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs)
            'Fenster darf nicht mit dem X schließbar sein...
            If IsForm() Then
                GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_CLOSE, 0, 0) ' WM_CLOSE hat kein wParam und lParam
            End If
            e.Cancel = m_CancelClose
        End Sub

        Private Sub __Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
            If IsForm() Then
                GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_PAINT, 0, 0)
            End If
        End Sub

        Private Sub __SizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If IsForm() Then
                Select Case Me.m_Frm.WindowState
                    Case FormWindowState.Normal
                        GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_SIZE, SIZE_RESTORED, Me.m_Frm.ClientSize.Width + (Me.m_Frm.ClientSize.Height << 16))
                    Case FormWindowState.Minimized
                        GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_SIZE, SIZE_MINIMIZED, Me.m_Frm.ClientSize.Width + (Me.m_Frm.ClientSize.Height << 16))
                    Case FormWindowState.Maximized
                        GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_SIZE, SIZE_MAXIMIZED, Me.m_Frm.ClientSize.Width + (Me.m_Frm.ClientSize.Height << 16))
                    Case Else
                        GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_SIZE, SIZE_RESTORED, Me.m_Frm.ClientSize.Width + (Me.m_Frm.ClientSize.Height << 16))
                End Select
            End If
        End Sub

        Private Sub __KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
            Dim scancode As Integer = 0
            Dim key As String = ""

            Select Case e.KeyCode
                Case Keys.F1
                    key = "p;"
                Case Keys.F2
                    key = "q<"
                Case Keys.F3
                    key = "r="
                Case Keys.F4
                    key = "s>"
                Case Keys.F5
                    key = "t?"
                Case Keys.F6
                    key = "u@"
                Case Keys.F7
                    key = "vA"
                Case Keys.F8
                    key = "wB"
                Case Keys.F9
                    key = "xC"

                Case Keys.Home 'Pos1
                    key = "$G"
                Case Keys.PageUp 'BildUp
                    key = "!I"
                Case Keys.PageDown 'BildDown
                    key = Chr(34) + "Q"
                Case Keys.Insert
                    key = "-R"
                Case Keys.Delete
                    key = ".S"
                Case Keys.End
                    key = "#O"

                    'Wird bereits von KeyPress gehandelt
                    'Case Keys.NumPad0
                    '    key = "0"
                    'Case Keys.NumPad1
                    '    key = "1"
                    'Case Keys.NumPad2
                    '    key = "2"
                    'Case Keys.NumPad3
                    '    key = "3"
                    'Case Keys.NumPad4
                    '    key = "4"
                    'Case Keys.NumPad5
                    '    key = "5"
                    'Case Keys.NumPad6
                    '    key = "6"
                    'Case Keys.NumPad7
                    '    key = "7"
                    'Case Keys.NumPad8
                    '    key = "8"
                    'Case Keys.NumPad9
                    '    key = "9"

                Case Keys.Left
                    key = "%K"
                Case Keys.Right
                    key = "'M"
                Case Keys.Up
                    key = "&H"
                Case Keys.Down
                    key = "(P"
            End Select
            If key <> "" Then
                GFAWindows.INKEY = key
                If IsForm() Then
                    scancode = DirectCast(e.KeyCode, Integer)
                    GFAWindows.AddEvent(Me.m_Frm.Handle, 1, WM_KEYDOWN, scancode, 1) 'Nicht gleich wie bei Windows API!
                    'GFAWindows.AddEvent(Me.m_Frm.Handle, Me.ID, WM_CHAR, _CVI(key), 1) 'Nicht gleich wie bei Windows API!
                End If
            End If
        End Sub
    End Class




    Class GFAFileOld
        Public Enum GFAFILEMODE
            Output
            Input
            Append
            Update
            Random
        End Enum

        Public Const MAX_FILES As Integer = 99
        Protected Const TMPBUFFER_SIZE = 512
        Protected Const TMPSTRING_SIZE = 512

        Protected m_strBuffer(TMPSTRING_SIZE - 1) As Byte
        Protected m_strm As IO.Stream
        Protected m_bufstrm As IO.BufferedStream
        Protected m_mode As GFAFILEMODE
        Protected m_filename As String
        Protected Shared m_openFiles(MAX_FILES) As GFAFile

        Public Shared Function CheckRange(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckRange = True
            If index < 0 Then
                CheckRange = False
                If throwExceptions Then Throw New ArgumentException("Wrong File-ID (cannot be negative)")
            End If
            If index > MAX_FILES Then
                CheckRange = False
                If throwExceptions Then Throw New ArgumentException("Wrong File-ID (cannot be bigger than" & MAX_FILES & ")")
            End If
        End Function

        Public Shared Function CheckOpen(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckOpen = False
            If index <= MAX_FILES And index >= 0 Then
                If m_openFiles(index) IsNot Nothing Then
                    CheckOpen = True
                Else
                    If throwExceptions Then Throw New IO.IOException("File-ID #" & index & +" is not open")
                End If
            End If
        End Function

        Public Shared Function CheckAlreadyOpen(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckAlreadyOpen = False
            If index <= MAX_FILES And index >= 0 Then
                If m_openFiles(index) IsNot Nothing Then
                    CheckAlreadyOpen = True
                    If throwExceptions Then Throw New IO.IOException("File-ID #" & index & " is already open (" + Chr(34) + m_openFiles(index).Filename + Chr(34) + ")")
                End If
            End If
        End Function

        Public Shared Property OpenFiles(ByVal index As Integer) As GFAFile
            Get
                CheckRange(index, True)
                OpenFiles = m_openFiles(index)
            End Get
            Set(ByVal value As GFAFile)
                CheckRange(index, True)
                'Nothing muss auch zugelassen werden, wegen Close...
                'If value Is Nothing Then
                '    Throw New ArgumentException("File object cannot be nothing")
                'End If
                m_openFiles(index) = value
            End Set
        End Property

        Public Sub New(ByVal filename As String, ByVal mode As GFAFILEMODE)
            m_mode = mode
            m_filename = filename
            Select Case mode
                Case GFAFILEMODE.Input
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
                Case GFAFILEMODE.Output
                    m_strm = New IO.FileStream(filename, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
                Case GFAFILEMODE.Append
                    m_strm = New IO.FileStream(filename, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.None)
                Case GFAFILEMODE.Update
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.ReadWrite, IO.FileShare.None)
                Case GFAFILEMODE.Random
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.ReadWrite, IO.FileShare.None)
                Case Else
                    Throw New ArgumentException("mode " & mode & " is not supported")
            End Select
            'm_bufstrm = New IO.BufferedStream(m_strm)
            'ReadBuffer()
        End Sub

        Public ReadOnly Property Length() As Long
            Get
                Return m_strm.Length
            End Get
        End Property

        Public Property Location() As Long
            Get
                Return m_bufstrm.Position
            End Get
            Set(ByVal value As Long)
                m_strm.Seek(value, IO.SeekOrigin.Begin)
            End Set
        End Property

        Public ReadOnly Property EndOfFile() As Boolean
            Get
                Return (m_strm.Position >= m_strm.Length)
            End Get
        End Property

        Public Function ReadByte() As Byte
            Dim result As Integer
            result = m_strm.ReadByte()
            If result < 0 Then
                Throw New IO.EndOfStreamException()
            End If
            Return result And 255  'And 255 ist eigentlich nicht nötig...
        End Function

        Public Sub WriteLine(ByVal line As String)
            Dim buffer() As Byte = System.Text.Encoding.Default.GetBytes(line + Chr(13) + Chr(10))
            m_strm.Write(buffer, 0, buffer.Length)
        End Sub

        Public Sub Write(ByVal line As String)
            Dim buffer() As Byte = System.Text.Encoding.Default.GetBytes(line)
            m_strm.Write(buffer, 0, buffer.Length)
        End Sub

        Public Sub Flush()
            m_strm.Flush()
        End Sub

        Public Sub Close()
            m_strm.Close()
            m_bufstrm = Nothing
            m_strm = Nothing
        End Sub

        Public ReadOnly Property Filename() As String
            Get
                Return m_filename
            End Get
        End Property

        Public ReadOnly Property Stream() As IO.Stream
            Get
                Return m_strm
            End Get
        End Property

        Public Function ReadLine() As String
            Return Read(False)
        End Function

        Public Function ReadBlock() As String
            Return Read(True)
        End Function

        Protected Function Read(ByVal readBlock As Boolean) As String
            Dim strBufPos As Integer = 0
            Dim resultStr = ""
            Dim abort As Boolean = False
            Dim ch As Integer = 0
            Dim lastch As Integer = 0
            Dim lastch2 As Integer = 0
            Dim position As Long = 0
            Dim firstChar As Boolean = True
            Dim inQuote As Boolean = False
            Do
                lastch2 = lastch
                lastch = ch
                ch = m_strm.ReadByte()

                If readBlock Then
                    If inQuote = False And ch = 34 Then
                        inQuote = True
                    ElseIf inQuote = True And ch = 44 And lastch = 34 Then
                        inQuote = False
                        abort = True
                    ElseIf inQuote = True And ch = 10 And lastch = 13 And lastch2 = 34 Then
                        inQuote = False
                        abort = True
                    End If
                    If inQuote = False And ch = 44 Then
                        abort = True
                    End If
                End If

                If inQuote = False And ch = 10 And lastch = 13 Then
                    abort = True
                End If

                If ch <> -1 Then
                    If strBufPos >= TMPSTRING_SIZE - 1 Then
                        resultStr += System.Text.Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
                        strBufPos = 0
                    End If
                    m_strBuffer(strBufPos) = ch And 255
                    strBufPos += 1
                Else
                    abort = True
                End If

            Loop Until abort = True

            If strBufPos > 0 Then
                resultStr += Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
            End If
            If Right(resultStr, 2) = Chr(13) + Chr(10) Then
                resultStr = Left(resultStr, Len(resultStr) - 2)
            End If

            If readBlock Then
                If Right(resultStr, 1) = "," Then
                    resultStr = Left(resultStr, Len(resultStr) - 1)
                End If
                If Left(resultStr, 1) = Chr(34) Then resultStr = Right(resultStr, Len(resultStr) - 1)
                If Right(resultStr, 1) = Chr(34) Then resultStr = Left(resultStr, Len(resultStr) - 1)
            End If

            Return resultStr
        End Function
    End Class

    Class GFAFile
        Public Enum GFAFILEMODE
            Output
            Input
            Append
            Update
            Random
        End Enum

        Public Const MAX_FILES As Integer = 99
        Protected Const TMPSTRING_SIZE = 2048
        Protected Const TMPBUFFER_SIZE = 2048

        Protected m_CurrentFilePos As Long = 0

        'Lese-Buffer:
        Protected m_tmpBufferPos As Integer = 0
        Protected m_AvailSize As Integer = 0
        Protected m_tmpBuffer(TMPBUFFER_SIZE - 1) As Byte

        Protected m_strBuffer(TMPSTRING_SIZE - 1) As Byte
        Protected m_strm As IO.Stream
        'Protected m_bufstrm As IO.BufferedStream
        Protected m_mode As GFAFILEMODE
        Protected m_filename As String
        Protected Shared m_openFiles(MAX_FILES) As GFAFile

        Public Shared Function CheckRange(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckRange = True
            If index < 0 Then
                CheckRange = False
                If throwExceptions Then Throw New ArgumentException("Wrong File-ID #" & index & " (cannot be negative)")
            End If
            If index > MAX_FILES Then
                CheckRange = False
                If throwExceptions Then Throw New ArgumentException("Wrong File-ID #" & index & " (cannot be bigger than" & MAX_FILES & ")")
            End If
        End Function

        Public Shared Function CheckOpen(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckOpen = False
            If index <= MAX_FILES And index >= 0 Then
                If m_openFiles(index) IsNot Nothing Then
                    CheckOpen = True
                Else
                    If throwExceptions Then Throw New IO.IOException("File-ID #" & index & " is not open")
                End If
            End If
        End Function

        Public Shared Function CheckAlreadyOpen(ByVal index As Integer, ByVal throwExceptions As Boolean) As Boolean
            CheckAlreadyOpen = False
            If index <= MAX_FILES And index >= 0 Then
                If m_openFiles(index) IsNot Nothing Then
                    CheckAlreadyOpen = True
                    If throwExceptions Then Throw New IO.IOException("File-ID #" & index & " is already open (" + Chr(34) + m_openFiles(index).Filename + Chr(34) + ")")
                End If
            End If
        End Function

        Public Shared Property OpenFiles(ByVal index As Integer) As GFAFile
            Get
                CheckRange(index, True)
                OpenFiles = m_openFiles(index)
            End Get
            Set(ByVal value As GFAFile)
                CheckRange(index, True)
                'Nothing muss auch zugelassen werden, wegen Close...
                'If value Is Nothing Then
                '    Throw New ArgumentException("File object cannot be nothing")
                'End If
                m_openFiles(index) = value
            End Set
        End Property

        Public Sub New(ByVal filename As String, ByVal mode As GFAFILEMODE)
            m_mode = mode
            m_filename = filename
            Select Case mode
                Case GFAFILEMODE.Input
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
                Case GFAFILEMODE.Output
                    m_strm = New IO.FileStream(filename, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
                Case GFAFILEMODE.Append
                    m_strm = New IO.FileStream(filename, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.None)
                Case GFAFILEMODE.Update
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.ReadWrite, IO.FileShare.None)
                Case GFAFILEMODE.Random
                    m_strm = New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.ReadWrite, IO.FileShare.None)
                Case Else
                    Throw New ArgumentException("mode " & mode & " is not supported")
            End Select
            m_CurrentFilePos = m_strm.Position
        End Sub

        Public ReadOnly Property Length() As Long
            Get
                Return m_strm.Length
            End Get
        End Property

        Public Property Location() As Long
            Get
                Return m_CurrentFilePos
            End Get
            Set(ByVal value As Long)
                m_CurrentFilePos = value
                m_strm.Seek(value, IO.SeekOrigin.Begin)
            End Set
        End Property

        Public ReadOnly Property EndOfFile() As Boolean
            Get
                Return (m_CurrentFilePos >= m_strm.Length)
                ' Return (m_bufstrm.Position >= m_bufstrm.Length)
            End Get
        End Property

        Public Function ReadByte() As Byte
            Dim isEof As Boolean = True
            If m_tmpBufferPos >= m_AvailSize Then
                If Update() Then
                    isEof = False
                    ReadByte = m_tmpBuffer(m_tmpBufferPos)
                    m_tmpBufferPos += 1
                    m_CurrentFilePos += 1 'Muss auch erhöht werden!
                End If
            Else
                isEof = False
                ReadByte = m_tmpBuffer(m_tmpBufferPos)
                m_tmpBufferPos += 1
                m_CurrentFilePos += 1 'Muss auch erhöht werden!
            End If
            If isEof Then
                Throw New IO.EndOfStreamException()
            End If
        End Function

        Public Sub WriteLine(ByVal line As String)
            Dim buffer() As Byte = Encoding.Default.GetBytes(line & vbCrLf) 'Chr(13) & Chr(10))
            FlushReadBuffer()
            m_strm.Write(buffer, 0, buffer.Length)
            m_CurrentFilePos = m_strm.Position 'Position muss aktualisiert werden!
        End Sub

        Public Sub Write(ByVal line As String)
            Dim buffer() As Byte = Encoding.Default.GetBytes(line)
            FlushReadBuffer()
            m_strm.Write(buffer, 0, buffer.Length)
            m_CurrentFilePos = m_strm.Position 'Position muss aktualisiert werden!
        End Sub

        Protected Function Update() As Boolean ' Gibt True bei EOF zurück
            Update = True
            If m_tmpBufferPos >= m_AvailSize Then 'Update noch nicht nötig
                m_tmpBufferPos = 0
                m_CurrentFilePos = m_strm.Position
                m_AvailSize = m_strm.Read(m_tmpBuffer, 0, TMPBUFFER_SIZE)
                If m_AvailSize <= 0 Then
                    Update = False ' End of File!
                End If
            End If
        End Function

        Protected Sub FlushReadBuffer()
            m_AvailSize = 0
            m_tmpBufferPos = 0
            m_strm.Seek(m_CurrentFilePos, IO.SeekOrigin.Begin)
        End Sub

        Public Sub Flush()
            FlushReadBuffer()
            m_strm.Flush()
        End Sub

        Public Sub Close()
            'm_bufstrm.Close()
            'm_bufstrm = Nothing
            Flush() ' Eigentlich nicht nötig
            m_strm.Close()
            m_strm = Nothing
        End Sub

        Public ReadOnly Property Filename() As String
            Get
                Return m_filename
            End Get
        End Property

        Public ReadOnly Property Stream() As IO.Stream
            Get
                Return m_strm
            End Get
        End Property

        Public Function ReadLine() As String
            Return Read(False)
        End Function

        Public Function ReadBlock() As String
            Return Read(True)
        End Function

        Protected Function Read(ByVal readBlock As Boolean) As String
            Dim strBufPos As Integer = 0
            Dim resultStr As String = ""
            Dim abort As Boolean = False
            Dim ch As Integer = 0
            Dim lastch As Integer = 0
            Dim lastch2 As Integer = 0
            Dim inQuote As Boolean = False
            Dim ok As Boolean = False
            Do
                lastch2 = lastch
                lastch = ch
                'ch = m_bufstrm.ReadByte()

                ok = False
                If m_tmpBufferPos >= m_AvailSize Then
                    If Update() Then
                        ok = True
                    End If
                Else
                    ok = True
                End If

                If ok Then
                    ch = m_tmpBuffer(m_tmpBufferPos)
                    m_tmpBufferPos += 1
                    m_CurrentFilePos += 1

                    'If ch < 45 Then 'Abfrage nur aus Geschwindigkeitsgründen!
                    If readBlock Then
                        If inQuote = False And ch = 34 Then
                            inQuote = True
                        ElseIf (inQuote = True) And ((ch = 44 And lastch = 34) Or (ch = 10 And lastch = 13 And lastch2 = 34)) Then
                            inQuote = False
                            abort = True
                            'ElseIf inQuote = True And ch = 44 And lastch = 34 Then
                            '    inQuote = False
                            '    abort = True
                            'ElseIf inQuote = True And ch = 10 And lastch = 13 And lastch2 = 34 Then
                            '    inQuote = False
                            '    abort = True
                        End If
                        If inQuote = False And ch = 44 Then
                            abort = True
                        End If
                    End If
                    If inQuote = False And ch = 10 And lastch = 13 Then
                        abort = True
                    End If
                    'End If

                    If strBufPos >= TMPSTRING_SIZE - 1 Then
                        resultStr += Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
                        strBufPos = 0
                    End If
                    m_strBuffer(strBufPos) = ch 'And 255
                    strBufPos += 1
                Else
                    abort = True
                End If
            Loop Until abort = True

            If strBufPos > 0 Then
                resultStr += Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
            End If
            If Right(resultStr, 2) = vbCrLf Then ' Chr(13) + Chr(10)
                resultStr = Left(resultStr, Len(resultStr) - 2)
            End If

            If readBlock Then

                'If resultStr.Length > 0 Then
                'If resultStr(resultStr.Length - 1) = ","c Then
                '         resultStr = resultStr.Substring(0, resultStr.Length - 1)
                '     End If
                ' End If
                ' If resultStr.Length > 0 Then
                '     If resultStr(resultStr.Length - 1) = """"c Then
                '         resultStr = resultStr.Substring(0, resultStr.Length - 1)
                '    End If
                ' End If
                'If resultStr.Length > 0 Then
                '    If resultStr(0) = """"c Then
                '         resultStr = resultStr.Substring(1)
                '     End If
                'End If
                If Right(resultStr, 1) = "," Then
                    resultStr = Left(resultStr, Len(resultStr) - 1)
                End If
                If Left(resultStr, 1) = Chr(34) Then resultStr = Right(resultStr, Len(resultStr) - 1)
                If Right(resultStr, 1) = Chr(34) Then resultStr = Left(resultStr, Len(resultStr) - 1)
            End If
            Return resultStr
        End Function



        Protected Function ReadOld(ByVal readBlock As Boolean) As String
            Dim strBufPos As Integer = 0
            Dim resultStr = ""
            Dim abort As Boolean = False
            Dim ch As Integer = 0
            Dim lastch As Integer = 0
            Dim lastch2 As Integer = 0
            Dim position As Long = 0
            Dim firstChar As Boolean = True
            Dim inQuote As Boolean = False
            Do
                lastch2 = lastch
                lastch = ch
                'ch = m_bufstrm.ReadByte()

                If readBlock Then
                    If inQuote = False And ch = 34 Then
                        inQuote = True
                    ElseIf inQuote = True And ch = 44 And lastch = 34 Then
                        inQuote = False
                        abort = True
                    ElseIf inQuote = True And ch = 10 And lastch = 13 And lastch2 = 34 Then
                        inQuote = False
                        abort = True
                    End If
                    If inQuote = False And ch = 44 Then
                        abort = True
                    End If
                End If

                If inQuote = False And ch = 10 And lastch = 13 Then
                    abort = True
                End If

                If ch <> -1 Then
                    If strBufPos >= TMPSTRING_SIZE - 1 Then
                        resultStr += System.Text.Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
                        strBufPos = 0
                    End If
                    m_strBuffer(strBufPos) = ch And 255
                    strBufPos += 1
                Else
                    abort = True
                End If

            Loop Until abort = True

            If strBufPos > 0 Then
                resultStr += System.Text.Encoding.Default.GetString(m_strBuffer, 0, strBufPos)
            End If
            If Right(resultStr, 2) = vbCrLf Then ' Chr(13) + Chr(10)
                resultStr = Left(resultStr, Len(resultStr) - 2)
            End If

            If readBlock Then
                If Right(resultStr, 1) = "," Then
                    resultStr = Left(resultStr, Len(resultStr) - 1)
                End If
                If Left(resultStr, 1) = Chr(34) Then resultStr = Right(resultStr, Len(resultStr) - 1)
                If Right(resultStr, 1) = Chr(34) Then resultStr = Left(resultStr, Len(resultStr) - 1)
            End If

            Return resultStr
        End Function
    End Class
#End Region
#Region "GFABasic - Dateisystem Funktionen"
    ''' <summary>
    ''' Löscht eine Datei
    ''' </summary>
    Public Sub _KILL(ByVal filename As String)
        Kill(filename)
    End Sub
    ''' <summary>
    ''' Erstellt ein Verzeichnis
    ''' </summary>
    Public Sub _MKDIR(ByVal path As String)
        MkDir(path)
    End Sub
    ''' <summary>
    ''' Ändert den Namen für eine Datei
    ''' </summary>
    Public Sub _RENAME(ByVal oldPath As String, ByVal newPath As String)
        Rename(oldPath, newPath)
    End Sub
    ''' <summary>
    ''' Überprüft, ob eine Datei existiert
    ''' </summary>
    ''' <param name="filename">Dateiname</param>
    ''' <returns>True, wenn die Datei existiert (Ansonsten False)</returns>
    Public Function _EXIST(ByVal filename As String) As Boolean
        Return System.IO.File.Exists(filename)
    End Function
#End Region
#Region "GFABasic - Dateioperationen Funktionen"
    'Private openFileHandles(99) As GFAFile
    ''' <summary>
    ''' Öffnet den angegebenen Dateikanal für die angegebene Datei mit dem angegebene Modus.
    ''' </summary>
    ''' <param name="modus">"o","u","i","a" oder "r"</param>
    ''' <param name="fileNb">Kanalnummer 0-99</param>
    ''' <param name="filename">Dateiname</param>
    Public Sub _OPEN(ByVal modus As String, ByVal fileNb As Integer, ByVal filename As String)
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckAlreadyOpen(fileNb, True)
        modus = LCase(Trim(modus))
        Select Case modus
            Case "o"
                GFAFile.OpenFiles(fileNb) = New GFAFile(filename, GFAFile.GFAFILEMODE.Output)
            Case "i"
                GFAFile.OpenFiles(fileNb) = New GFAFile(filename, GFAFile.GFAFILEMODE.Input)
            Case "a"
                GFAFile.OpenFiles(fileNb) = New GFAFile(filename, GFAFile.GFAFILEMODE.Append)
            Case "r"
                GFAFile.OpenFiles(fileNb) = New GFAFile(filename, GFAFile.GFAFILEMODE.Random)
            Case "u"
                GFAFile.OpenFiles(fileNb) = New GFAFile(filename, GFAFile.GFAFILEMODE.Update)
            Case Else
                Throw New ArgumentException("unknown mode " & Chr(34) & modus & Chr(34) & " for OPEN")
                'Exit Sub
        End Select
    End Sub
    ''' <summary>
    ''' Schließt den angegebenen Dateikanal
    ''' </summary>
    Public Sub _CLOSE(ByVal fileNb As Integer)
        'Veruhrsacht auch in GFA keinen Exception, wenn fileNb falsch ist...
        If fileNb < 100 And fileNb >= 0 Then
            If GFAFile.OpenFiles(fileNb) IsNot Nothing Then
                GFAFile.OpenFiles(fileNb).Close()
                GFAFile.OpenFiles(fileNb) = Nothing
            End If
        End If
    End Sub
    ''' <summary>
    ''' Schließt alle offenen Dateikanäle
    ''' </summary>
    Public Sub _CLOSE()
        Dim i As Integer
        For i = 0 To GFAFile.MAX_FILES
            If GFAFile.OpenFiles(i) IsNot Nothing Then
                GFAFile.OpenFiles(i).Close()
                GFAFile.OpenFiles(i) = Nothing
            End If
        Next
    End Sub
    ''' <summary>
    ''' Liest ein Byte von dem angegebenen Dateikanal
    ''' </summary>
    Public Function _INP(ByVal fileNb As Integer) As Byte
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        _INP = GFAFile.OpenFiles(fileNb).ReadByte() ' Löst, wie GFA bei EOF einen Exception aus!
    End Function
    ''' <summary>
    ''' Liest eine Zeile aus dem angegebenen Dateikanal aus
    ''' </summary>
    Public Sub _LINE_INPUT(ByVal fileNb As Integer, ByRef str As String)
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        str = GFAFile.OpenFiles(fileNb).ReadLine
    End Sub
    ''' <summary>
    ''' Liest zwei Zeilen aus dem angegebenen Dateikanal aus
    ''' </summary>
    Public Sub _LINE_INPUT(ByVal fileNb As Integer, ByRef str1 As String, ByRef str2 As String)
        _LINE_INPUT(fileNb, str1)
        _LINE_INPUT(fileNb, str2)
    End Sub
    ''' <summary>
    ''' Liest drei Zeilen aus dem angegebenen Dateikanal aus
    ''' </summary>
    Public Sub _LINE_INPUT(ByVal fileNb As Integer, ByRef str1 As String, ByRef str2 As String, ByRef str3 As String)
        _LINE_INPUT(fileNb, str1)
        _LINE_INPUT(fileNb, str2)
        _LINE_INPUT(fileNb, str3)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(Of T)(ByVal fileNb As Integer, ByRef obj As T)
        Dim block As String : Dim value As Double : Dim failed As Boolean
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        If GFAFile.OpenFiles(fileNb).EndOfFile Then
            Throw New IO.EndOfStreamException
        End If
        block = GFAFile.OpenFiles(fileNb).ReadBlock
        'Wenn es vom Typ Object bzw. Nothing ist, dann müssen wir hier auch davon ausgehen, das es ein String ist...
        If TypeOf (obj) Is String Or obj Is Nothing Then
            'If GetType(T).Equals(GetType(String)) Or GetType(T).Equals(GetType(Object)) Then
            obj = DirectCast(block, Object)
        Else
            'Bei Integer gehen keine Zahlen verloren, wenn diese zunächst in Double gespeichert werden (getestet mit PB)
            value = GFAConvert.StrToDouble(block, failed)
            'failed = False
            If failed Then
                Throw New InvalidCastException("cannot convert " & Chr(34) & block & Chr(34) & _
                " to type " & Chr(34) & GetType(T).ToString & Chr(34))
            End If
            obj = DirectCast(value, Object)
        End If
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As String)
        _INPUT(Of String)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Double)
        _INPUT(Of Double)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Single)
        _INPUT(Of Single)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Byte)
        _INPUT(Of Byte)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Short)
        _INPUT(Of Short)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Integer)
        _INPUT(Of Integer)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest einen Block von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj As Boolean)
        _INPUT(Of Boolean)(fileNb, obj)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
        _INPUT(fileNb, obj10)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
        _INPUT(fileNb, obj10)
        _INPUT(fileNb, obj11)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
        _INPUT(fileNb, obj10)
        _INPUT(fileNb, obj11)
        _INPUT(fileNb, obj12)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object, ByRef obj13 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
        _INPUT(fileNb, obj10)
        _INPUT(fileNb, obj11)
        _INPUT(fileNb, obj12)
        _INPUT(fileNb, obj13)
    End Sub
    ''' <summary>
    ''' Liest Blöcke von dem angegebenen Dateikanal
    ''' </summary>
    Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object, ByRef obj13 As Object, ByRef obj14 As Object)
        _INPUT(fileNb, obj1)
        _INPUT(fileNb, obj2)
        _INPUT(fileNb, obj3)
        _INPUT(fileNb, obj4)
        _INPUT(fileNb, obj5)
        _INPUT(fileNb, obj6)
        _INPUT(fileNb, obj7)
        _INPUT(fileNb, obj8)
        _INPUT(fileNb, obj9)
        _INPUT(fileNb, obj10)
        _INPUT(fileNb, obj11)
        _INPUT(fileNb, obj12)
        _INPUT(fileNb, obj13)
        _INPUT(fileNb, obj14)
    End Sub
    ''' <summary>
    ''' Schreibt den angegebenen Inhalt in die angegebenen Datei. Für den Textausgabebefehl PRINT siehe _PRINTTEXT 
    ''' </summary>
    Public Sub _PRINT(ByVal fileNb As Integer, ByVal ParamArray Output() As Object)
        Dim i As Integer = 0
        Dim data As String = ""
        Dim numSpaces As Integer = 0
        Dim value As String = ""
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        For i = 0 To Output.Length - 1

            numSpaces = (16 - data.Length Mod 16) Mod 16
            data += New String(" "c, numSpaces)
            value = ""
            If Output(i) IsNot Nothing Then
                If TypeOf Output(i) Is String Then
                    value = Output(i)
                ElseIf TypeOf Output(i) Is Boolean Then
                    'Nicht als True,False speichern!
                    If Output(i) Then
                        value = "-1" 'TRUE IST -1!
                    Else
                        value = "0"
                    End If
                Else
                    value = Trim(Str(Output(i)))
                End If
            End If
            data += value
        Next
        GFAFile.OpenFiles(fileNb).WriteLine(data)
        'PrintLine(fileNb, Output) 'Muss in die nächste Zeile wie in GFA, daher PrintLine und nicht Print!
    End Sub
    ''' <summary>
    ''' Schreibt den angegebenen Inhalt in die angegebenen Datei. Im gegensatz zu PRINT wird kein Zeilenumbruch angehängt. Entspricht PRINT #n,...; (mit Semikolon)
    ''' </summary>
    Public Sub _PRINTBLOCK(ByVal fileNb As Integer, ByVal ParamArray Output() As Object)
        Dim i As Integer = 0
        Dim data As String = ""
        Dim numSpaces As Integer = 0
        Dim value As String = ""
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        For i = 0 To Output.Length - 1

            numSpaces = (16 - data.Length Mod 16) Mod 16
            data += New String(" "c, numSpaces)
            value = ""
            If Output(i) IsNot Nothing Then
                If TypeOf Output(i) Is String Then
                    value = Output(i)
                ElseIf TypeOf Output(i) Is Boolean Then
                    'Nicht als True,False speichern!
                    If Output(i) Then
                        value = "-1" 'TRUE IST -1!
                    Else
                        value = "0"
                    End If
                Else
                    value = Trim(Str(Output(i)))
                End If
            End If
            data += value
        Next
        GFAFile.OpenFiles(fileNb).Write(data)
        'PrintLine(fileNb, Output) 'Muss in die nächste Zeile wie in GFA, daher PrintLine und nicht Print!
    End Sub
    ''' <summary>
    ''' Schreibt den angegebenen Inhalt in die angegebenen Datei.
    ''' </summary>
    Public Sub _WRITE(ByVal fileNb As Integer, ByVal ParamArray Output() As Object)
        Dim i As Integer = 0
        Dim outputStr As String = ""
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        For i = 0 To Output.Length - 1

            Dim value As String = Chr(34) & Chr(34)
            If Output(i) IsNot Nothing Then
                If TypeOf Output(i) Is String Then
                    value = Chr(34) & Output(i) & Chr(34)
                ElseIf TypeOf Output(i) Is Boolean Then
                    'Nicht als True,False speichern!
                    If Output(i) Then
                        value = "-1" 'TRUE IST -1!
                    Else
                        value = "0"
                    End If
                Else
                    value = Trim(Str(Output(i))) 'Zahlen dürfen nicht in Anführungszeichen stehen! 
                End If
            End If

            If i < Output.Length - 1 Then
                outputStr = outputStr & value & ","
                'GFAFile.OpenFiles(fileNb).Write(value & ",") 'SM 2010-11-05
            Else
                outputStr = outputStr & value
                'GFAFile.OpenFiles(fileNb).WriteLine(value) 'SM 2010-11-05
            End If
        Next
        If Output.Length > 0 Then 'SM 2010-11-05
            GFAFile.OpenFiles(fileNb).WriteLine(outputStr)
        End If
        'WriteLine(fileNb, Output) ' nächste Zeile wie in GFA. Im gegensatz zu Print ist der Text in Anführungzeichen
    End Sub
    ''' <summary>
    ''' Gibts zurück, ob das Ende der Datei erreicht ist
    ''' </summary>
    Public Function _EOF(ByVal fileNb As Integer) As Integer
        'Darf wie bei GFA keinen Exception auslösen
        If GFAFile.CheckRange(fileNb, False) Then
            If GFAFile.CheckOpen(fileNb, False) Then
                _EOF = GFAFile.OpenFiles(fileNb).EndOfFile
            Else
                _EOF = 0 'GFA gibt hier 0 zurück
            End If
        Else
            _EOF = -2 'GFA gibt hier -2 zurück
        End If
    End Function
    ''' <summary>
    ''' Gibts die Dateigröße in Bytes zurück
    ''' </summary>
    Public Function _LOF(ByVal fileNb As Integer) As Long
        'Darf wie bei GFA keinen Exception auslösen
        If GFAFile.CheckRange(fileNb, False) Then
            If GFAFile.CheckOpen(fileNb, False) Then
                _LOF = GFAFile.OpenFiles(fileNb).Length
            Else
                _LOF = -1 'GFA gibt hier -1 zurück
            End If
        Else
            _LOF = 0 'GFA gibt hier 0 zurück
        End If
    End Function
    ''' <summary>
    ''' Gibt die aktuelle Lese- bzw. Schreibposition für den angegebenen Dateikanal zurück
    ''' </summary>
    Public Function _LOC(ByVal fileNb As Integer) As Long
        'Darf wie bei GFA keinen Exception auslösen
        If GFAFile.CheckRange(fileNb, False) Then
            If GFAFile.CheckOpen(fileNb, False) Then
                _LOC = GFAFile.OpenFiles(fileNb).Location
            Else
                _LOC = -1 'GFA gibt hier -1 zurück
            End If
        Else
            _LOC = 0 'GFA gibt hier 0 zurück
        End If
    End Function
    ''' <summary>
    ''' Setzt die aktuelle Lese- bzw. Schreibposition für den angegebenen Dateikanal
    ''' </summary>
    Public Sub _SEEK(ByVal fileNb As Integer, ByVal position As Long)
        'Exceptions auslösen (wie in GFA)
        GFAFile.CheckRange(fileNb, True)
        GFAFile.CheckOpen(fileNb, True)
        GFAFile.OpenFiles(fileNb).Location = position
        'Seek(fileNb, position)
    End Sub
#End Region
#Region "GFABasic - Array-Funktionen"
    ''' <summary>
    ''' Füllt das Array mit dem angegeben Wert.
    ''' </summary>
    ''' <param name="arr">1,2,3 oder 4-dimensionales array vom typ Boolean, Integer, Byte, Short, Long, Double, Single oder String</param>
    ''' <param name="newValue">neuer Wert</param>
    Public Sub _ARRAYFILL(ByRef arr As Object, ByVal newValue As Object)
        If arr Is Nothing Then
            Return
        End If
        '================ 1D Arrays =================
        If (TypeOf arr Is Boolean()) Then
            GFAArray.ArrayFill1D(Of Boolean)(DirectCast(arr, Boolean()), newValue)
            Return
        End If
        If (TypeOf arr Is Integer()) Then
            GFAArray.ArrayFill1D(Of Integer)(DirectCast(arr, Integer()), newValue And &HFFFFFFFF)
            Return
        End If
        If (TypeOf arr Is Byte()) Then
            GFAArray.ArrayFill1D(Of Byte)(DirectCast(arr, Byte()), newValue And &HFF) 'Kein Exception in GFA bei -1 -> 255
            Return
        End If
        If (TypeOf arr Is Short()) Then
            GFAArray.ArrayFill1D(Of Short)(DirectCast(arr, Short()), newValue And &HFFFF)
            Return
        End If
        If (TypeOf arr Is Long()) Then
            GFAArray.ArrayFill1D(Of Long)(DirectCast(arr, Long()), newValue)
            Return
        End If
        If (TypeOf arr Is Double()) Then
            GFAArray.ArrayFill1D(Of Double)(DirectCast(arr, Double()), newValue)
            Return
        End If
        If (TypeOf arr Is Single()) Then
            GFAArray.ArrayFill1D(Of Single)(DirectCast(arr, Single()), newValue)
            Return
        End If
        If (TypeOf arr Is String()) Then
            GFAArray.ArrayFill1D(Of String)(DirectCast(arr, String()), newValue)
            Return
        End If
        '================ 2D Arrays =================
        If (TypeOf arr Is Boolean(,)) Then
            GFAArray.ArrayFill2D(Of Boolean)(DirectCast(arr, Boolean(,)), newValue)
            Return
        End If
        If (TypeOf arr Is Integer(,)) Then
            GFAArray.ArrayFill2D(Of Integer)(DirectCast(arr, Integer(,)), newValue And &HFFFFFFFF)
            Return
        End If
        If (TypeOf arr Is Byte(,)) Then
            GFAArray.ArrayFill2D(Of Byte)(DirectCast(arr, Byte(,)), newValue And &HFF)
            Return
        End If
        If (TypeOf arr Is Short(,)) Then
            GFAArray.ArrayFill2D(Of Short)(DirectCast(arr, Short(,)), newValue And &HFFFF)
            Return
        End If
        If (TypeOf arr Is Long(,)) Then
            GFAArray.ArrayFill2D(Of Long)(DirectCast(arr, Long(,)), newValue)
            Return
        End If
        If (TypeOf arr Is Double(,)) Then
            GFAArray.ArrayFill2D(Of Double)(DirectCast(arr, Double(,)), newValue)
            Return
        End If
        If (TypeOf arr Is Single(,)) Then
            GFAArray.ArrayFill2D(Of Single)(DirectCast(arr, Single(,)), newValue)
            Return
        End If
        If (TypeOf arr Is String(,)) Then
            GFAArray.ArrayFill2D(Of String)(DirectCast(arr, String(,)), newValue)
            Return
        End If
        '================ 3D Arrays =================
        If (TypeOf arr Is Boolean(,,)) Then
            GFAArray.ArrayFill3D(Of Boolean)(DirectCast(arr, Boolean(,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Integer(,,)) Then
            GFAArray.ArrayFill3D(Of Integer)(DirectCast(arr, Integer(,,)), newValue And &HFFFFFFFF)
            Return
        End If
        If (TypeOf arr Is Byte(,,)) Then
            GFAArray.ArrayFill3D(Of Byte)(DirectCast(arr, Byte(,,)), newValue And &HFF)
            Return
        End If
        If (TypeOf arr Is Short(,,)) Then
            GFAArray.ArrayFill3D(Of Short)(DirectCast(arr, Short(,,)), newValue And &HFFFF)
            Return
        End If
        If (TypeOf arr Is Long(,,)) Then
            GFAArray.ArrayFill3D(Of Long)(DirectCast(arr, Long(,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Double(,,)) Then
            GFAArray.ArrayFill3D(Of Double)(DirectCast(arr, Double(,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Single(,,)) Then
            GFAArray.ArrayFill3D(Of Single)(DirectCast(arr, Single(,,)), newValue)
            Return
        End If
        If (TypeOf arr Is String(,,)) Then
            GFAArray.ArrayFill3D(Of String)(DirectCast(arr, String(,,)), newValue)
            Return
        End If
        '================ 4D Arrays =================
        If (TypeOf arr Is Boolean(,,,)) Then
            GFAArray.ArrayFill4D(Of Boolean)(DirectCast(arr, Boolean(,,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Integer(,,,)) Then
            GFAArray.ArrayFill4D(Of Integer)(DirectCast(arr, Integer(,,,)), newValue And &HFFFFFFFF)
            Return
        End If
        If (TypeOf arr Is Byte(,,,)) Then
            GFAArray.ArrayFill4D(Of Byte)(DirectCast(arr, Byte(,,,)), newValue And &HFF)
            Return
        End If
        If (TypeOf arr Is Short(,,,)) Then
            GFAArray.ArrayFill4D(Of Short)(DirectCast(arr, Short(,,,)), newValue And &HFFFF)
            Return
        End If
        If (TypeOf arr Is Long(,,,)) Then
            GFAArray.ArrayFill4D(Of Long)(DirectCast(arr, Long(,,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Double(,,,)) Then
            GFAArray.ArrayFill4D(Of Double)(DirectCast(arr, Double(,,,)), newValue)
            Return
        End If
        If (TypeOf arr Is Single(,,,)) Then
            GFAArray.ArrayFill4D(Of Single)(DirectCast(arr, Single(,,,)), newValue)
            Return
        End If
        If (TypeOf arr Is String(,,,)) Then
            GFAArray.ArrayFill4D(Of String)(DirectCast(arr, String(,,,)), newValue)
            Return
        End If
        Throw New Exception("array " & arr.ToString() & " is not supported by ArrayFill")
    End Sub
    ''' <summary>
    ''' entspricht DELETE array(index) in GFA. Achtung: Funktion ist sehr langsam
    ''' </summary>
    ''' <param name="arr">eindimensionales Array</param>
    ''' <param name="idx">Index der enfernt werden soll</param>
    Public Sub _DELETE(ByRef arr As Object, ByVal idx As Integer)
        Dim tmpArray As ArrayList = New ArrayList()
        For Each tmpObj As Object In arr
            tmpArray.Add(tmpObj)
        Next
        tmpArray.RemoveAt(idx)
        'tmpArray.Remove(obj) würde nicht gleich wie in GFA funktionieren (manchmal wird falsches Objekt gelöscht)
        'If tmpArray.Count > 0 Then

        'arr(0).GetType funktioniert wohl zumindest bei Strings mit Nothing nicht...
        'arr = tmpArray.ToArray(arr(0).GetType()) ' Es gibt scheinbar auch keinen Crash, wenn die Arraygröße hier 0 wäre
        arr = tmpArray.ToArray(arr.GetType.GetElementType())
        'End If
    End Sub
    ''' <summary>
    ''' Fügt ein neues Element an der angegebenen Stelle in das Array ein. Achtung: Funktion ist sehr langsam
    ''' </summary>
    Public Sub _INSERT(ByRef arr As Object, ByVal idx As Integer, ByVal value As Object)
        Dim tmpArray As ArrayList = New ArrayList()
        For Each tmpObj As Object In arr
            tmpArray.Add(tmpObj)
        Next
        tmpArray.Insert(idx, value)

        'arr(0).GetType funktioniert wohl zumindest bei Strings mit Nothing nicht...
        arr = tmpArray.ToArray(arr.GetType.GetElementType())
    End Sub
#End Region
#Region "GFABasic - Mathematische Funktionen"
    ''' <summary>
    ''' Rundet eine Fließkommazahl wie in GFA-Basic
    ''' </summary>
    Public Function _ROUND(ByVal number As Double, ByVal numDigits As Integer) As Double
        Dim negDigits As Integer = 0
        'Es dürfen wie in GFA keine Exceptions ausgelöst werden
        If numDigits > 15 Then numDigits = 15
        If numDigits < 0 Then
            negDigits = -numDigits
            numDigits = 0
            number /= Math.Pow(10.0R, negDigits)
        End If
        'ACHTUNG: Bei negativen Zahlen ist das Verhalten von Math.Round nicht gleich wie in GFA!
        'Deswegen: in dem Fall zunächst Teilen und dann multiplizieren
        _ROUND = (Math.Round(number, numDigits))
        If negDigits > 0 Then
            _ROUND *= Math.Pow(10.0R, negDigits)
        End If
    End Function
    ''' <summary>
    ''' Rundet auf die nächste Ganzzahl
    ''' </summary>
    Public Function _ROUND(ByVal number As Double) As Double
        Return Math.Round(number)
    End Function
    ''' <summary>
    ''' Gibt das Maximum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MAX(ByVal a As Double) As Double
        Return a
    End Function
    ''' <summary>
    ''' Gibt das Maximum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MAX(ByVal a As Double, ByVal b As Double) As Double
        If a > b Then
            Return a
        End If
        Return b
    End Function
    ''' <summary>
    ''' Gibt das Maximum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MAX(ByVal a As Double, ByVal b As Double, ByVal c As Double) As Double
        Dim result = a
        If b > result Then
            result = b
        End If
        If c > result Then
            result = c
        End If
        Return result
    End Function
    ''' <summary>
    ''' Gibt das Maximum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MAX(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal d As Double) As Double
        Dim result = a
        If b > result Then
            result = b
        End If
        If c > result Then
            result = c
        End If
        If d > result Then
            result = d
        End If
        Return result
    End Function
    ''' <summary>
    ''' Gibt das Minimum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MIN(ByVal a As Double) As Double
        Return a
    End Function
    ''' <summary>
    ''' Gibt das Minimum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MIN(ByVal a As Double, ByVal b As Double) As Double
        If a < b Then
            Return a
        End If
        Return b
    End Function
    ''' <summary>
    ''' Gibt das Minimum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MIN(ByVal a As Double, ByVal b As Double, ByVal c As Double) As Double
        Dim result = a
        If b < result Then
            result = b
        End If
        If c < result Then
            result = c
        End If
        Return result
    End Function
    ''' <summary>
    ''' Gibt das Minimum der angegebenen Werte zurück
    ''' </summary>
    Public Function _MIN(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal d As Double) As Double
        Dim result = a
        If b < result Then
            result = b
        End If
        If c < result Then
            result = c
        End If
        If d < result Then
            result = d
        End If
        Return result
    End Function
    ''' <summary>
    ''' Gibt den Tangens für den im Bogenmaß angegebenen Winkels zurück
    ''' </summary>
    Public Function _TAN(ByVal d As Double) As Double
        Return Math.Tan(d)
    End Function
    ''' <summary>
    ''' Gibt den Arkuskosinus für den angegebenen Wert zurück
    ''' </summary>
    Public Function _ACOS(ByVal d As Double) As Double
        Return Math.Acos(d)
    End Function
    ''' <summary>
    ''' Gibt den Arkussinus für den angegebenen Wert zurück
    ''' </summary>
    Public Function _ASIN(ByVal d As Double) As Double
        Return Math.Asin(d)
    End Function
    ''' <summary>
    ''' Gibt den Cosinus für den im Bogenmaß angegebenen Winkels zurück
    ''' </summary>
    Public Function _COS(ByVal d As Double) As Double
        Return Math.Cos(d)
    End Function
    ''' <summary>
    ''' Gibt den Sinus für den im Bogenmaß angegebenen Winkels zurück
    ''' </summary>
    Public Function _SIN(ByVal d As Double) As Double
        Return Math.Sin(d)
    End Function
    ''' <summary>
    ''' Gibt den Arkustangens für den angegebenen Wert zurück
    ''' </summary>
    Public Function _ATN(ByVal d As Double) As Double
        Return Math.Atan(d)
    End Function
    ''' <summary>
    ''' Gibt den Arkustangens für den angegebenen Wert zurück
    ''' </summary>
    Public Function _ATAN(ByVal d As Double) As Double
        Return Math.Atan(d)
    End Function
    ''' <summary>
    ''' berechnet Logarithmus zur Basis 10
    ''' </summary>
    Public Function _LOG10(ByVal x As Double) As Double
        Return Math.Log(x) / Math.Log(10.0)
    End Function
    ''' <summary>
    ''' berechnet Logarithmus zur Basis 2
    ''' </summary>
    Public Function _LOG2(ByVal x As Double) As Double
        Return Math.Log(x) / Math.Log(2.0)
    End Function
    ''' <summary>
    ''' berechnet Logarithmus zur Basis der EULER'schen Zahl(wie in GFA)
    ''' </summary>
    Public Function _LOG(ByVal x As Double) As Double
        Return Math.Log(x)
    End Function
    ''' <summary>
    ''' Berechnet den Absolutbetrag von x.
    ''' </summary>
    Public Function _ABS(ByVal x As Double) As Double
        Return Math.Abs(x)
    End Function
    ''' <summary>
    ''' Achtung! Fix gibt bei z.B. -3.6 als Erg. -3 zurück (Wie bei TRUNC). FLOOR und INT geben hier -4 zurück
    ''' </summary>
    Public Function _FIX(ByVal x As Double) As Integer
        Return Math.Truncate(x)
    End Function
    ''' <summary>
    ''' Achtung! FLOOR gibt bei z.B. -3.6 als Erg. -4 zurück (Wie bei INT). TRUNC und FIX geben hier -3 zurück
    ''' </summary>
    Public Function _INT(ByVal x As Double) As Integer
        Return Math.Floor(x)
    End Function
    ''' <summary>
    ''' Achtung! INT gibt bei z.B. -3.6 als Erg. -4 zurück (Wie bei INT). TRUNC und FIX geben hier -3 zurück
    ''' </summary>
    Public Function _FLOOR(ByVal x As Double) As Integer
        Return Math.Floor(x)
    End Function
    ''' <summary>
    ''' Berechnet die Fakultät
    ''' </summary>
    Public Function _FACT(ByVal value As Integer) As Integer
        Dim result As Integer = 1
        Dim i As Integer
        If (value < 0) Then
            Throw New ArgumentException("Error with faculty " + value + "(cannot be negative)")
        End If
        For i = value To 1 Step -1
            result *= i
        Next
        Return result
    End Function
    ''' <summary>
    ''' Gibt den hyperbolischen Tangens zurück
    ''' </summary>
    Public Function _TANH(ByVal x As Double) As Double
        Return Math.Tanh(x)
    End Function
    ''' <summary>
    ''' Gibt den hyperbolischen Cosinus zurück
    ''' </summary>
    Public Function _COSH(ByVal x As Double) As Double
        Return Math.Cosh(x)
    End Function
    ''' <summary>
    ''' Gibt den hyperbolischen Sinus zurück
    ''' </summary>
    Public Function _SINH(ByVal x As Double) As Double
        Return Math.Sinh(x)
    End Function
    ''' <summary>
    ''' Gibt die Quadratwurzel von x zurück 
    ''' </summary>
    Public Function _SQR(ByVal x As Double) As Double
        Return Math.Sqrt(x)
    End Function
    ''' <summary>
    ''' Gibt das Ergebnis der Exponentialfunktion zurück
    ''' </summary>
    Public Function _EXP(ByVal x As Double) As Double
        Return Math.Pow(Math.E, x)
    End Function
    ''' <summary>
    ''' setzt diejenigen Bits, die in beiden Verknüpfungswerten i und j gleich sind. Entspricht NOT(XOR(i,j)) 
    ''' </summary>
    Public Function _EQV(ByVal i As Integer, ByVal j As Integer) As Integer
        Return Not (i Xor j)
    End Function
    ''' <summary>
    ''' Gibt die Zahl zurück, die um 1 kleiner als x ist
    ''' </summary>
    Public Function _PRED(ByVal x As Integer) As Integer
        Return x - 1
    End Function
    ''' <summary>
    ''' Gibt das Zeichen zurück, welches einen um 1 kleineren ASCII-Code besitzt als das erste Zeichen im String
    ''' </summary>
    Public Function _PRED(ByVal x As String) As String
        If x IsNot Nothing Then
            If Len(x) > 0 Then
                Return Chr(Asc(Left(x, 1)) - 1)
            End If
        End If
        Return ""
    End Function
    ''' <summary>
    ''' Gibt die Zahl zurück, die um 1 größer als x ist
    ''' </summary>
    Public Function _SUCC(ByVal x As Integer) As Integer
        Return x + 1
    End Function
    ''' <summary>
    ''' Gibt das Zeichen zurück, welches einen um 1 kleineren ASCII-Code besitzt als das erste Zeichen im String
    ''' </summary>
    Public Function _SUCC(ByVal x As String) As String
        If x IsNot Nothing Then
            If Len(x) > 0 Then
                Return Chr(Asc(Left(x, 1)) + 1)
            End If
        End If
        Return ""
    End Function
    ''' <summary>
    ''' Konvertiert einen Winkel vom Gradmaß in das Bogenmaß
    ''' </summary>
    Public Function _RAD(ByVal x As Double) As Double
        Return x * (Math.PI / 180.0R)
    End Function
    ''' <summary>
    ''' Konvertiert einen Winkel vom Bogenmaß in das Gradmaß
    ''' </summary>
    Public Function _DEG(ByVal x As Double) As Double
        Return x * (180.0R / Math.PI)
    End Function
    Public Function _TRUNC(ByVal x As Double) As Integer
        Return Math.Truncate(x)
    End Function
    Public Function _RAND(ByVal x As Integer) As Integer
        'CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound
        Return CInt(Math.Floor(x * Rnd()))
    End Function
    ''' <summary>
    ''' Gibt eine Zufallszahl zurück
    ''' </summary>
    Public Function _RANDOM(ByVal x As Double) As Integer
        'Return Math.Truncate(Rnd() * x)
        Return CInt(Math.Floor(x * Rnd()))
    End Function
    ''' <summary>
    ''' Initailisiert den Zufallszahlengenerator mit einem Startwert
    ''' </summary>
    Public Sub _RANDOMIZE(ByVal seed As Integer)
        Randomize(seed)
    End Sub
    ''' <summary>
    ''' Gibt -1 zurück, wenn die Zahl ungerade ist. Ansonsten wird 0 zurückgegeben
    ''' </summary>
    Public Function _ODD(ByVal x As Integer) As Integer
        If (x Mod 2) Then
            Return -1
        Else
            Return 0
        End If
    End Function
    ''' <summary>
    ''' Gibt -1 zurück, wenn die Zahl gerade ist. Ansonsten wird 0 zurückgegeben
    ''' </summary>
    Public Function _EVEN(ByVal x As Integer) As Integer
        If (x Mod 2) Then
            Return 0
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' Gibt bei positiven Zahlen 1, bei negativen Zahlen -1 und 0 bei 0 zurück
    ''' </summary>
    Public Function _SGN(ByVal val As Double) As Integer
        If val < 0.0R Then
            Return -1
        ElseIf val > 0.0R Then
            Return 1
        Else
            Return 0
        End If
    End Function
    ''' <summary>
    ''' Vertauscht den Inhalt der Variablen. Achtung! Möglicherweise nicht gleiches Verhalten bei unterscheidlichen Typen (z.B.: integer mit double austauschen!)
    ''' </summary>
    Public Sub _SWAP(ByRef a As Object, ByRef b As Object)
        Dim t As Object = b
        b = a
        a = t
    End Sub
    ''' <summary>
    ''' Erhöht die Variable um 1
    ''' </summary>
    Public Sub _INC(ByRef x As Byte)
        x += 1
    End Sub
    ''' <summary>
    ''' Erhöht die Variable um 1
    ''' </summary>
    Public Sub _INC(ByRef x As Short)
        x += 1
    End Sub
    ''' <summary>
    ''' Erhöht die Variable um 1
    ''' </summary>
    Public Sub _INC(ByRef x As Integer)
        x += 1
    End Sub
    ''' <summary>
    ''' Erhöht die Variable um 1
    ''' </summary>
    Public Sub _INC(ByRef x As Double)
        x += 1
    End Sub
    ''' <summary>
    ''' Verringert die Variable um 1
    ''' </summary>
    Public Sub __DEC(ByRef x As Byte) 'Problem mit DEC$
        x -= 1
    End Sub
    ''' <summary>
    ''' Verringert die Variable um 1
    ''' </summary>
    Public Sub __DEC(ByRef x As Short)
        x -= 1
    End Sub
    ''' <summary>
    ''' Verringert die Variable um 1
    ''' </summary>
    Public Sub __DEC(ByRef x As Integer)
        x -= 1
    End Sub
    ''' <summary>
    ''' Verringert die Variable um 1
    ''' </summary>
    Public Sub __DEC(ByRef x As Double)
        x -= 1
    End Sub
    ''' <summary>
    ''' Multipliziert die zwei Zahlen und speichert das Ergebnis in der ersten Variable
    ''' </summary>
    Public Sub _MUL(ByRef x As Double, ByVal y As Double)
        x *= y
    End Sub
    ''' <summary>
    ''' Dividiert die zwei Zahlen und speichert das Ergebnis in der ersten Variable
    ''' </summary>
    Public Sub _DIV(ByRef x As Double, ByVal y As Double)
        x /= y
    End Sub
    ''' <summary>
    ''' Addiert die zwei Zahlen und speichert das Ergebnis in der ersten Variable
    ''' </summary>
    Public Sub _ADD(ByRef x As Double, ByVal y As Double)
        x += y
    End Sub
    ''' <summary>
    ''' Subtrahiert die zwei Zahlen und speichert das Ergebnis in der ersten Variable
    ''' </summary>
    Public Sub _SUB(ByRef x As Double, ByVal y As Double)
        x -= y
    End Sub
    'Es ist nicht möglich beide Arten von MUL zu definieren
    'Function MUL(ByVal x As Double, ByVal y As Double)
    '    Return x * y
    'End Function
    ''' <summary>
    ''' Gibt den rest der Division value1 / value2 zurück
    ''' </summary>
    Public Function _MOD(ByVal value1 As Integer, ByVal value2 As Integer) As Integer
        Return value1 Mod value2
    End Function
#End Region
#Region "GFABasic - Binäroperationen"
    ''' <summary>
    ''' Verschiebt die Bits der Zahl um die bei shift angegebenen Stellen nach links
    ''' </summary>
    Public Function _SHL(ByVal value As Long, ByVal shift As Integer) As Long
        Return (value << shift) 'GFA unterstützt wie VB.Net auch keine Fließkommazahlen
    End Function
    ''' <summary>
    ''' Verschiebt die Bits der Zahl um die bei shift angegebenen Stellen nach rechts
    ''' </summary>
    Public Function _SHR(ByVal value As Long, ByVal shift As Integer) As Long
        Return (value >> shift) 'GFA unterstützt wie VB.Net auch keine Fließkommazahlen
    End Function
    ''' <summary>
    ''' Verschiebt die Bits der Zahl um die bei shift angegebenen Stellen nach links
    ''' </summary>
    Public Function _SHL(ByVal value As Integer, ByVal shift As Integer) As Integer
        Return (value << shift) 'GFA unterstützt wie VB.Net auch keine Fließkommazahlen
    End Function
    ''' <summary>
    ''' Verschiebt die Bits der Zahl um die bei shift angegebenen Stellen nach rechts
    ''' </summary>
    Public Function _SHR(ByVal value As Integer, ByVal shift As Integer) As Integer
        Return (value >> shift) 'GFA unterstützt wie VB.Net auch keine Fließkommazahlen
    End Function
    ''' <summary>
    ''' vertauscht Hi- und Low-Word
    ''' </summary>
    Public Function _SWAP(ByVal val As Integer)
        Return ((val And &HFFFF) << 16) + ((val >> 16) And &HFFFF)
    End Function
    ''' <summary>
    ''' gibt die das untere Word (16 Bit) als vorzeichenlose Zahl zurück
    ''' </summary>
    Public Function _UWORD(ByVal val As Integer) As Integer
        Return (val And &HFFFF)
    End Function
    ''' <summary>
    ''' Führt And 0xFFFF durch
    ''' </summary>
    Public Function _USHORT(ByVal val As Integer) As Integer
        Return (val And &HFFFF)
    End Function
    ''' <summary>
    ''' Führt And 0xFFFF durch, wenn Ergebnis größer 32767, dann wird 65536 abgezogen 
    ''' </summary>
    Public Function _SHORT(ByVal val As Integer) As Integer
        Dim res As Integer = (val And &HFFFF)
        If (val > 32767) Then
            res -= 65536
        End If
        Return res
    End Function
    ''' <summary>
    ''' Führt And 0xFFFF durch, wenn Ergebnis größer 32767, dann wird 65536 abgezogen 
    ''' </summary>
    Public Function _WORD(ByVal val As Integer) As Integer
        Dim res As Integer = (val And &HFFFF)
        If (val > 32767) Then
            res -= 65536
        End If
        Return res
    End Function
    ''' <summary>
    ''' Führt And 0xFF durch
    ''' </summary>
    Public Function _BYTE(ByVal val As Integer) As Integer
        Return (val And &HFF)
    End Function
    ''' <summary>
    ''' exklusive Bitweise OR-Operation
    ''' </summary>
    Public Function _XOR(ByVal value1 As Integer, ByVal value2 As Integer)
        Return value1 Xor value2
    End Function
#End Region
#Region "GFABasic - Stringfunktionen"
    ''' <summary>
    ''' Wie Left, löst jedoch keinen Exception aus, wenn die Länge kleiner als 0 ist (wie in GFA).
    ''' </summary>
    Public Function _LEFT(ByVal str As String, ByVal length As Integer) As String
        If length < 0 Then length = 0
        Return Microsoft.VisualBasic.Left(str, length)
    End Function
    ''' <summary>
    ''' Gibt das erste Zeichen des Strings zurück
    ''' </summary>
    Public Function _LEFT(ByVal str As String) As String
        Return Microsoft.VisualBasic.Left(str, 1)
    End Function
    ''' <summary>
    ''' Wie Right, löst jedoch keinen Exception aus, wenn die Länge kleiner als 0 ist (wie in GFA).
    ''' </summary>
    Public Function _RIGHT(ByVal str As String, ByVal length As Integer) As String
        If length < 0 Then length = 0
        Return Microsoft.VisualBasic.Right(str, length)
    End Function
    ''' <summary>
    ''' Gibt das letzte Zeichen des Strings zurück
    ''' </summary>
    Public Function _RIGHT(ByVal str As String) As String
        Return Microsoft.VisualBasic.Right(str, 1)
    End Function
    ''' <summary>
    ''' Hat gegenüber Mid eine zusätzliche Abfrage, ob start kleiner 1 ist, sodass in diesem Fall kein Exception auftritt (wie in GFA)
    ''' </summary>
    Public Function _MID(ByVal str As String, ByVal start As Integer, ByVal len As Integer) As String
        _MID = ""
        If start < 1 Then start = 1
        If len < 0 Then len = 0
        _MID = Mid(str, start, len)
    End Function
    ''' <summary>
    ''' Hat gegenüber Mid eine zusätzliche Abfrage, ob start kleiner 1 ist, sodass in diesem Fall kein Exception auftritt (wie in GFA)
    ''' </summary>
    Public Function _MID(ByVal str As String, ByVal start As Integer) As String
        _MID = ""
        If start < 1 Then start = 1
        _MID = Mid(str, start)
    End Function
    ''' <summary>
    ''' Löst im gegesatz zu Asc() keinen Exception bei einem Leerstring aus (wie in GFA)
    ''' </summary>
    Public Function _ASC(ByVal str As String) As Integer
        _ASC = 0
        If str IsNot Nothing Then
            If str <> "" Then
                _ASC = Asc(str)
            End If
        End If
    End Function
    ''' <summary>
    ''' Konvertiert einen String in eine 16-Bit Integer-Zahl 
    ''' </summary>
    Public Function _CVI(ByVal str As String) As Int16
        _CVI = 0
        If str IsNot Nothing Then
            If str.Length >= 2 Then
                _CVI = BitConverter.ToInt16(System.Text.ASCIIEncoding.Default.GetBytes(str), 0)
            End If
        End If
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Binär-String um.
    ''' </summary>
    Public Function _BIN$(ByVal val As Integer)
        Return Convert.ToString(val, 2)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Binär-String um.
    ''' </summary>
    Public Function _BIN$(ByVal val As Integer, ByVal len As Integer)
        Return Convert.ToString(val, 2).PadLeft(len, "0"c)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Hexadezimal-String um.
    ''' </summary>
    Public Function _HEX$(ByVal val As Integer)
        Return Convert.ToString(val, 16)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Hexadezimal-String um.
    ''' </summary>
    Public Function _HEX$(ByVal val As Integer, ByVal len As Integer)
        Return Convert.ToString(val, 16).PadLeft(len, "0"c)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Octal-String um.
    ''' </summary>
    Public Function _OCT$(ByVal val As Integer)
        Return Convert.ToString(val, 8)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Octal-String um.
    ''' </summary>
    Public Function _OCT$(ByVal val As Integer, ByVal len As Integer)
        Return Convert.ToString(val, 8).PadLeft(len, "0"c)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Dezimal-String um.
    ''' </summary>
    Public Function _DEC$(ByVal val As Integer)
        Return Convert.ToString(val, 10)
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen Dezimal-String um.
    ''' </summary>
    Public Function _DEC$(ByVal val As Integer, ByVal len As Integer)
        Return Convert.ToString(val, 10).PadLeft(len, "0"c)
    End Function
    ''' <summary>
    ''' Erstellt einen String, der die angegebene Anzahl des angegebenen Strings enthält
    ''' </summary>
    Public Function _STRING$(ByVal len As Integer, ByVal obj As Object)
        'VB.Net ist zu blöd zwei verschiedene Versionen String und Integer als Parameter zu unterstützen
        If TypeOf obj Is String Then
            Return (New String(" "c, len)).Replace(" ", obj) 'Löst in GFA auch einen Fehler aus, wenn len < 0...
        Else
            Return (New String(" "c, len)).Replace(" ", Chr(obj)) 'Löst in GFA auch einen Fehler aus, wenn len < 0...
        End If
    End Function
    ''' <summary>
    ''' Erstellt einen String mit der angegebenen Anzahl Leerzeichen
    ''' </summary>
    Public Function _SPACE$(ByVal len As Integer)
        Return (New String(" "c, len)) 'Löst in GFA auch einen Fehler aus, wenn len < 0...
    End Function
    ''' <summary>
    ''' Wie StrReverse
    ''' </summary>
    Public Function _MIRROR$(ByVal str As String)
        Return StrReverse(str)
    End Function
    ''' <summary>
    ''' Wie UCase
    ''' </summary>
    Public Function _UPPER$(ByVal str As String)
        Return UCase(str)
    End Function
    ''' <summary>
    ''' Wie LCase
    ''' </summary>
    Public Function _LOWER$(ByVal str As String)
        Return LCase(str)
    End Function
    ''' <summary>
    ''' Sucht den String strMatch in strCheck und gibt die Position zurück. Wenn der String nicht gefunden wird, wird 0 zurückgegeben
    ''' </summary>
    Function _INSTR(ByVal strCheck As String, ByVal strMatch As String, ByVal start As Integer) As String
        If start < 1 Then start = 1
        Return InStr(start, strCheck, strMatch)
        'Eigene methode würde auch funktionieren, aber besser direkt InStr verwenden...
        'Dim newStrCheck As String
        'Dim result As Integer
        'Dim length = Len(strCheck) - start + 1
        'If start < 1 Then start = 1
        'If length < 0 Then length = 0
        'newStrCheck = Right(strCheck, length)
        'result = InStr(newStrCheck, strMatch)
        'If result > 0 Then
        'Return result + start - 1
        'Else
        'Return 0
        'End If
    End Function
    ''' <summary>
    ''' Sucht den String strMatch in strCheck und gibt die Position zurück. Wenn der String nicht gefunden wird, wird 0 zurückgegeben
    ''' </summary>
    Function _INSTR(ByVal strCheck As String, ByVal strMatch As String) As String
        Return InStr(strCheck, strMatch)
    End Function
    ''' <summary>
    ''' Wie InStrRev
    ''' </summary>
    Function _RINSTR(ByVal strCheck As String, ByVal strMatch As String, ByVal start As Integer) As String
        If start < 1 Then start = 1
        Return InStrRev(strCheck, strMatch, start)
    End Function
    ''' <summary>
    ''' Wie InStrRev
    ''' </summary>
    Function _RINSTR(ByVal strCheck As String, ByVal strMatch As String) As String
        Return InStrRev(strCheck, strMatch)
    End Function
    ''' <summary>
    ''' Wandelt eine String in eine Zahl um und unterstützt dabei auch Hex,Oct,Bin und die Exponentialschreibweise wie z.B.: 1.34E-09.
    ''' </summary>
    ''' <param name="str">String der konvertiert werden soll</param>
    ''' <returns>Zahl als Double</returns>
    Function _VAL(ByVal str As String) As Double
        Dim failed As Boolean = False 'Wird ignoriert
        Return GFAConvert.StrToDouble(str, failed)
    End Function

    ''' <summary>
    ''' Wandelt eine Zahl in einen String um. ACHTUNG! verhält sich nicht wie in GFA-Basic 16 Bit, sondern wie in GFA-Basic 32 Bit
    ''' </summary>
    Public Function _STR(ByVal value As Double, ByVal m As Integer, ByVal n As Integer) As String
        Dim strNum As String = GFAConvert.ToStr(value, n)
        If strNum.Length < m Then
            strNum = New String(" "c, m - strNum.Length) + strNum
        End If
        'strNum = Right(New String(" "c, m) + strNum, m)
        Return strNum
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen String um. ACHTUNG! verhält sich nicht wie in GFA-Basic 16 Bit, sondern wie in GFA-Basic 32 Bit
    ''' </summary>
    Public Function _STR(ByVal value As Double, ByVal m As Integer) As String
        Dim strNum As String = Str(value)
        If strNum.Length < m Then
            strNum = New String(" "c, m - strNum.Length) + strNum
        End If
        'strNum = Right(New String(" "c, m) + strNum, m)
        Return strNum
    End Function
    ''' <summary>
    ''' Wandelt eine Zahl in einen String um. ACHTUNG! verhält sich nicht wie in GFA-Basic 16 Bit, sondern wie in GFA-Basic 32 Bit
    ''' </summary>
    Public Function _STR(ByVal value As Double) As String
        Return Trim(Str(value)) 'GFA 16 bit hat hier kein Leerzeichen vor der Zahl
    End Function
#End Region
#Region "GFABasic - Fensterbefehle"
    ''' <summary>
    ''' Öffnet ein neues Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _OPENW(ByVal ID As Integer)
        GFAWindows.AddGFAWindow(ID, Nothing)
    End Sub
    ''' <summary>
    ''' Öffnet ein neues Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _OPENW(ByVal ID As Integer, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal style As Integer)
        GFAWindows.AddGFAWindow(ID, Nothing)
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.Location = New Point(x, y)
                wnd.Form.Size = New Size(width, height) ' Ist in GFA nicht die Client Weite und Höhe
                'TODO: Vertical/Horizontal Scrollbar

                If style = 0 Then
                    wnd.Form.FormBorderStyle = FormBorderStyle.None
                ElseIf style = -1 Then
                    wnd.Form.FormBorderStyle = FormBorderStyle.Sizable
                    wnd.Form.MaximizeBox = True
                    wnd.Form.MinimizeBox = True
                    wnd.Form.ControlBox = True
                Else

                    If style And 512 Then 'size box
                        wnd.Form.FormBorderStyle = FormBorderStyle.Sizable
                    Else
                        wnd.Form.FormBorderStyle = FormBorderStyle.FixedSingle
                    End If

                    If style And 16 Then 'title line
                        'Nichts tun
                    Else
                        wnd.Form.FormBorderStyle = FormBorderStyle.None
                    End If

                    If style And 64 Then 'minimize box
                        wnd.Form.MinimizeBox = True
                    Else
                        wnd.Form.MinimizeBox = False
                    End If
                    If style And 128 Then 'maximize box
                        wnd.Form.MaximizeBox = True
                    Else
                        wnd.Form.MaximizeBox = False
                    End If
                    If (style And 32) Or (style And 128) Or (style And 16) Then
                        wnd.Form.ControlBox = True
                    Else
                        wnd.Form.ControlBox = False
                    End If
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Öffnet ein neues Parent Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _PARENTW(ByVal ID As Integer)
        GFAWindows.AddGFAWindow(ID, Nothing)
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.IsMdiContainer = True
            End If
        End If
    End Sub
    ''' <summary>
    ''' Öffnet ein neues Child Fenster
    ''' </summary>
    Public Sub _CHILDW(ByVal ChildID As Integer, ByVal ParentID As Integer)
        GFAWindows.AddGFAWindow(ChildID, Nothing)
        Dim wndParent As GFAWindow = GFAWindows.GetWindow(ParentID)
        Dim wndChild As GFAWindow = GFAWindows.GetWindow(ChildID)
        If wndParent IsNot Nothing And wndChild IsNot Nothing Then
            If wndParent.Form IsNot Nothing And wndChild.Form IsNot Nothing Then
                wndChild.Form.MdiParent = wndParent.Form
            End If
        End If
    End Sub
    ''' <summary>
    ''' Öffnet ein neues Child Fenster
    ''' </summary>
    Public Sub _CHILDW(ByVal ChildID As Integer, ByVal ParentID As Integer, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal style As Integer)
        GFAWindows.AddGFAWindow(ChildID, Nothing)
        Dim wndParent As GFAWindow = GFAWindows.GetWindow(ParentID)
        Dim wndChild As GFAWindow = GFAWindows.GetWindow(ChildID)
        If wndParent IsNot Nothing And wndChild IsNot Nothing Then
            If wndParent.Form IsNot Nothing And wndChild.Form IsNot Nothing Then

                wndChild.Form.Location = New Point(x, y)
                wndChild.Form.Size = New Size(width, height) ' Ist in GFA nicht die Client Weite und Höhe
                'TODO: Vertical/Horizontal Scrollbar

                If style = 0 Then
                    wndChild.Form.FormBorderStyle = FormBorderStyle.None
                ElseIf style = -1 Then
                    wndChild.Form.FormBorderStyle = FormBorderStyle.Sizable
                    wndChild.Form.MaximizeBox = True
                    wndChild.Form.MinimizeBox = True
                    wndChild.Form.ControlBox = True
                Else

                    If style And 512 Then 'size box
                        wndChild.Form.FormBorderStyle = FormBorderStyle.Sizable
                    Else
                        wndChild.Form.FormBorderStyle = FormBorderStyle.FixedSingle
                    End If

                    If style And 16 Then 'title line
                        'Nichts tun
                    Else
                        wndChild.Form.FormBorderStyle = FormBorderStyle.None
                    End If

                    If style And 64 Then 'minimize box
                        wndChild.Form.MinimizeBox = True
                    Else
                        wndChild.Form.MinimizeBox = False
                    End If
                    If style And 128 Then 'maximize box
                        wndChild.Form.MaximizeBox = True
                    Else
                        wndChild.Form.MaximizeBox = False
                    End If
                    If (style And 32) Or (style And 128) Or (style And 16) Then
                        wndChild.Form.ControlBox = True
                    Else
                        wndChild.Form.ControlBox = False
                    End If
                End If
                wndChild.Form.MdiParent = wndParent.Form
            End If
        End If
    End Sub
    ''' <summary>
    ''' Setzt das Fenster in Vordergrund
    ''' </summary>
    Public Sub _TOPW(ByVal ID As Integer)
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.BringToFront()
            End If
        End If
    End Sub
    ''' <summary>
    ''' Schliesst das Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _CLOSEW(ByVal ID As Integer)
        'Darf keine Exception auslösen bei falscher ID
        GFAWindows.RemoveGFAWindow(ID)
    End Sub
    ''' <summary>
    ''' Aktuelles Fenster setzen
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _WIN(ByVal ID As Integer)
        'Darf keine Exception auslösen bei falscher ID
        GFAWindows.SetCurrentWindow(ID)
        GFADrawing.ResetDC() ' Nach WIN #X, wird wieder der DC vom Fenster zurückgegeben (auch wenn zuvor SETDC verwendet wurde)
    End Sub
    ''' <summary>
    ''' Gibt die Fensternummer des aktiven Fenster zurück (wie in GFa)
    ''' </summary>
    ''' <returns>Fensternummer (Kein Handle!)</returns>
    Public Function _WIN() As Integer
        _WIN = 0 ' Wie bei GFA 0 im Fehlerfall zurückgeben
        If GFAWindows.CurrentWindow IsNot Nothing Then
            _WIN = GFAWindows.CurrentWindow.ID
        End If
    End Function
    ''' <summary>
    ''' Gibt das Handle des aktiven Fenster zurück (entspricht WIN(Fenster) in GFA)
    ''' </summary>
    ''' <returns>Fensterhandle (HWND)</returns>
    Public Function _WIN_HANDLE(ByVal ID As Integer) As IntPtr
        _WIN_HANDLE = IntPtr.Zero ' Wie bei GFA 0 im Fehlerfall zurückgeben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                _WIN_HANDLE = wnd.Form.Handle
            End If
        End If
    End Function
    ''' <summary>
    ''' Gibt den Device Context zurück
    ''' </summary>
    ''' <returns>Gibt den Device Context zurück</returns>
    Public Function __DC() As IntPtr
        '__DC = IntPtr.Zero ' Wie bei GFA 0 im Fehlerfall zurückgeben
        'If GFAWindows.CurrentWindow IsNot Nothing Then
        '    If GFAWindows.CurrentWindow.Form IsNot Nothing Then
        '        __DC = GFAWindows.CurrentWindow.DC
        '    End If
        'End If
        __DC = GFADrawing.ActiveDC
    End Function
    ''' <summary>
    ''' Gibt den Device Context des Fensters zurück
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <returns>Gibt den Device Context zurück</returns>
    Public Function __DC(ByVal ID As Integer) As IntPtr
        __DC = IntPtr.Zero ' Wie bei GFA 0 im Fehlerfall zurückgeben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        Dim drawing As GFADrawing = Nothing
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                drawing = wnd.Drawing
                If drawing IsNot Nothing Then
                    __DC = drawing.DC
                End If
            End If
        End If
    End Function
    ''' <summary>
    ''' Ändert den Titel für das Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="Title">Neuer Titel</param>
    Public Sub _TITLEW(ByVal ID As Integer, ByVal Title As String)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.Text = Title
            End If
        End If
    End Sub
    ''' <summary>
    ''' Verschiebt das Fenster
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="x">neue X-Position</param>
    ''' <param name="y">neue Y-Position</param>
    ''' <remarks></remarks>
    Public Sub _MOVEW(ByVal ID As Integer, ByVal x As Integer, ByVal y As Integer)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.Location = New Point(x, y)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Ändert die Größe des Fensters
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="sx">neue Weite</param>
    ''' <param name="sy">neue Höhe</param>
    ''' <remarks></remarks>
    Public Sub _SIZEW(ByVal ID As Integer, ByVal sx As Integer, ByVal sy As Integer)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.Size = New Point(sx, sy)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Ermittelt die Position und Größe eines Fensters
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="x">zu füllende X-Position</param>
    ''' <param name="y">zu füllende Y-Position</param>
    ''' <param name="sx">zu füllende Weite</param>
    ''' <param name="sy">zu füllende Höhe</param>
    Public Sub _GETWINRECT(ByVal ID As Integer, ByRef x As Integer, ByRef y As Integer, ByRef sx As Integer, ByRef sy As Integer)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        sx = 0
        sy = 0
        x = 0
        y = 0
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                sx = wnd.Form.Size.Width
                sy = wnd.Form.Size.Height
                x = wnd.Form.Location.X
                y = wnd.Form.Location.Y
            End If
        End If
    End Sub
    ''' <summary>
    ''' Ändert den Anzeigestatus des Fensters
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="showCmd">Statusbefhel</param>
    ''' <remarks></remarks>
    Public Sub _SHOWW(ByVal ID As Integer, ByVal showCmd As Integer)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                ShowWindow(wnd.Form.Handle, showCmd)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Öffnet ein neues Fenster oder maximiert das bestehende
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    Public Sub _FULLW(ByVal ID As Integer)
        'Falsche ID darf keine Exception geben
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                wnd.Form.WindowState = FormWindowState.Maximized
            End If
        Else
            GFAWindows.AddGFAWindow(ID, Nothing)
            wnd = GFAWindows.GetWindow(ID)
            If wnd IsNot Nothing Then
                If wnd.Form IsNot Nothing Then
                    wnd.Form.WindowState = FormWindowState.Maximized
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Gibt zurück, ob das Fenster maximiert ist.
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <returns>Bool-Wert</returns>
    Public Function _ZOOMED(ByVal ID As Integer) As Boolean
        _ZOOMED = False
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                If wnd.Form.WindowState = FormWindowState.Maximized Then
                    _ZOOMED = True
                End If
            End If
        End If
    End Function
    ''' <summary>
    ''' Gibt zurück, ob das Fenster minimiert ist.
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <returns>Bool-Wert</returns>
    Public Function _ICONIC(ByVal ID As Integer) As Boolean
        _ICONIC = False
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                If wnd.Form.WindowState = FormWindowState.Minimized Then
                    _ICONIC = True
                End If
            End If
        End If
    End Function
    ''' <summary>
    ''' Leitet alle Mausaktivitäten auf ein Fenster um
    ''' </summary>
    Public Sub _SETCAPTURE(ByVal ID As Integer)
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                SetCapture(wnd.Form.Handle)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Entfernt die Umleitung für die Eingabe mit der Maus
    ''' </summary>
    Public Sub _RELEASECAPTURE()
        ReleaseCapture()
    End Sub
    ''' <summary>
    ''' Liest Fensterparameter aus. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDGET(ByVal index As Integer, ByRef value As Integer)
        '        i(Parameter)
        '0:      outer(X - coordinate)
        '1:      outer(Y - coordinate)
        '2:      outer(width)
        '3:      outer(height)
        '4:      inner(X - coordinate)
        '5:      inner(Y - coordinate)
        '6:      inner(width)
        '7:      inner(height)
        ' 8	* position of vertical slider (0..1000)
        ' 9	* size of slider area
        '10	* position of horizontal slider (0..1000)
        '11	* size of slider area
        '12	reads the window attributes (as set with OPENW)
        '13	* reads the attributes of the pressed window 
        '	button (from WINDSET)
        '14	character height (for example 8, 14, 16)

        '15	character set address
        '16	number of top window
        '17	number of second to top window
        '18	number of second to bottom window
        '19	number of bottom window
        Dim wnd As GFAWindow = GFAWindows.CurrentWindow
        value = 0
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                Select Case index
                    Case 0
                        value = wnd.Form.Location.X
                    Case 1
                        value = wnd.Form.Location.Y
                    Case 2
                        value = wnd.Form.Size.Width
                    Case 3
                        value = wnd.Form.Size.Height
                    Case 4
                        value = wnd.Form.ClientRectangle.X
                    Case 5
                        value = wnd.Form.ClientRectangle.Y
                    Case 6
                        value = wnd.Form.ClientRectangle.Width
                    Case 7
                        value = wnd.Form.ClientRectangle.Height
                    Case 8
                        value = 0 'TODO: Nicht unterstüzt
                    Case 9
                        value = 0 'TODO: Nicht unterstüzt
                    Case 10
                        value = 0 'TODO: Nicht unterstüzt
                    Case 11
                        value = 0 'TODO: Nicht unterstüzt
                    Case 12
                        value = -1 'TODO: Nicht unterstüzt
                    Case Else
                        value = 0
                        'TODO: Ret wird noch nicht unterstüzt...
                End Select
            End If
        End If
    End Sub
    ''' <summary>
    ''' Liest Fensterparameter aus. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDGET(ByVal index As Integer, ByRef value1 As Integer, ByRef value2 As Integer)
        _WINDGET(index, value1)
        _WINDGET(index + 1, value2)
    End Sub
    ''' <summary>
    ''' Liest Fensterparameter aus. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDGET(ByVal index As Integer, ByRef value1 As Integer, ByRef value2 As Integer, ByRef value3 As Integer)
        _WINDGET(index, value1)
        _WINDGET(index + 1, value2)
        _WINDGET(index + 2, value3)
    End Sub
    ''' <summary>
    ''' Liest Fensterparameter aus. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDGET(ByVal index As Integer, ByRef value1 As Integer, ByRef value2 As Integer, ByRef value3 As Integer, ByRef value4 As Integer)
        _WINDGET(index, value1)
        _WINDGET(index + 1, value2)
        _WINDGET(index + 2, value3)
        _WINDGET(index + 3, value4)
    End Sub
    ''' <summary>
    ''' Setzt Fensterparameter. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDSET(ByVal index As Integer, ByVal value As Integer)
        '        i(Parameter)
        '0:      outer(X - coordinate)
        '1:      outer(Y - coordinate)
        '2:      outer(width)
        '3:      outer(height)
        '4:      inner(X - coordinate)
        '5:      inner(Y - coordinate)
        '6:      inner(width)
        '7:      inner(height)
        ' 8	* position of vertical slider (0..1000)
        ' 9	* size of slider area
        '10	* position of horizontal slider (0..1000)
        '11	* size of slider area
        '12	reads the window attributes (as set with OPENW)
        '13	* reads the attributes of the pressed window 
        '	button (from WINDSET)
        '14	character height (for example 8, 14, 16)

        '15	character set address
        '16	number of top window
        '17	number of second to top window
        '18	number of second to bottom window
        '19	number of bottom window
        Dim wnd As GFAWindow = GFAWindows.CurrentWindow
        value = 0
        If wnd IsNot Nothing Then
            If wnd.Form IsNot Nothing Then
                Select Case index
                    Case 0
                        wnd.Form.Location = New Point(value, wnd.Form.Location.Y)
                    Case 1
                        wnd.Form.Location = New Point(wnd.Form.Location.X, value)
                    Case 2
                        wnd.Form.Size = New Size(value, wnd.Form.Size.Height)
                    Case 3
                        wnd.Form.Size = New Size(wnd.Form.Size.Width, value)
                    Case 4
                        'Tut nichts... Überprüfen, ob in Ordnung
                    Case 5
                        'Tut nichts... Überprüfen, ob in Ordnung
                    Case 6
                        wnd.Form.ClientSize = New Size(value, wnd.Form.ClientSize.Height)
                    Case 7
                        wnd.Form.ClientSize = New Size(wnd.Form.ClientSize.Width, value)
                    Case 8
                        'TODO: Nicht unterstüzt
                    Case 9
                        'TODO: Nicht unterstüzt
                    Case 10
                        'TODO: Nicht unterstüzt
                    Case 11
                        'TODO: Nicht unterstüzt
                    Case 12
                        'TODO: Nicht unterstüzt
                    Case Else
                        'TODO: Ret wird noch nicht unterstüzt...
                End Select
            End If
        End If
    End Sub
    ''' <summary>
    ''' Setzt Fensterparameter. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDSET(ByVal index As Integer, ByVal value1 As Integer, ByVal value2 As Integer)
        _WINDSET(index, value1)
        _WINDSET(index + 1, value2)
    End Sub
    ''' <summary>
    ''' Setzt Fensterparameter. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDSET(ByVal index As Integer, ByVal value1 As Integer, ByVal value2 As Integer, ByVal value3 As Integer)
        _WINDSET(index, value1)
        _WINDSET(index + 1, value2)
        _WINDSET(index + 2, value3)
    End Sub
    ''' <summary>
    ''' Setzt Fensterparameter. Achtung! Unterstüzt bisher nicht alle Werte!
    ''' </summary>
    Public Sub _WINDSET(ByVal index As Integer, ByVal value1 As Integer, ByVal value2 As Integer, ByVal value3 As Integer, ByVal value4 As Integer)
        _WINDSET(index, value1)
        _WINDSET(index + 1, value2)
        _WINDSET(index + 2, value3)
        _WINDSET(index + 3, value4)
    End Sub
    ''' <summary>
    ''' Ändert den Windows style.
    ''' </summary>
    Public Sub _SETWINDOWSTYLE(ByVal ID As Integer, ByVal style As Integer)
        Dim wnd As GFAWindow = GFAWindows.GetWindow(ID)
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                SetWindowLong(wnd.Form.Handle, GWL_STYLE, style)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Weite des aktiven Fensters in Pixel. Ansonsten Weite des Bildschirms in Pixel
    ''' </summary>
    Public Function __X() As Integer
        Dim wnd As GFAWindow = GFAWindows.CurrentWindow
        Dim result As Integer = My.Computer.Screen.Bounds.Size.Width
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                result = wnd.Form.ClientSize.Width
            End If
        End If
        Return result
    End Function
    ''' <summary>
    ''' Höhe des aktiven Fensters in Pixel. Ansonsten Höhe des Bildschirms in Pixel
    ''' </summary>
    Public Function __Y() As Integer
        Dim wnd As GFAWindow = GFAWindows.CurrentWindow
        Dim result As Integer = My.Computer.Screen.Bounds.Size.Height
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                result = wnd.Form.ClientSize.Height
            End If
        End If
        Return result
    End Function
#End Region
#Region "GFABasic - Grafik- und Zeichenbefehle"
    ''' <summary>
    '''  Setzt die Schriftart für den aktiven DC.
    ''' </summary>
    ''' <param name="fnt">Schriftart die verwendet werden soll</param>
    Sub _SETFONT(ByVal fnt As IntPtr)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            'If fntNr > 32 Then 'Evtl. nicht so sicher
            If GetObjectType(fnt) = OBJ_FONT Then
                drawing.SetFont(fnt)
            Else
                Dim fntNr As Integer = (fnt.ToInt64 And &HFFFFFFFF)
                If (fntNr >= 10 And fntNr <= 14) Or (fntNr = 16 Or fntNr = 17) Then
                    drawing.SetSystemFont(fntNr)
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Setzt einen DC(Device Context)
    ''' </summary>
    Public Sub _SETDC(ByVal DC As IntPtr)
        If DC <> IntPtr.Zero Then
            Dim wndIdx As Integer = GFAWindows.FindWindowFromDC(DC)
            If wndIdx > -1 Then
                'Passendes Fenster gefunden
                Dim wnd As GFAWindow = GFAWindows.GetWindow(wndIdx)
                If wnd IsNot Nothing Then
                    GFADrawing.ActiveDrawing = wnd.Drawing
                End If
            Else
                'Zunächst überprüfen, ob der DC bereits gesetzt ist
                If DC <> GFADrawing.ActiveDC Then
                    GFADrawing.ActiveDrawing = New GFADrawing(DC)
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Überprüft Fähigkeiten des aktiven Device Context(DC)
    ''' </summary>
    Public Function _GETDEVCAPS(ByVal capability As Integer) As Integer
        Dim DC As IntPtr = GFADrawing.ActiveDC
        If DC <> IntPtr.Zero Then
            Return GetDeviceCaps(DC, capability)
        End If
        Return 0
    End Function
    ''' <summary>
    ''' Setzt die Vordergrundfarbe und Hintergrundfarbe im RGB-Format
    ''' </summary>
    ''' <param name="vordergrund_rgb">Vordergrundfarbe im RGB-Format</param>
    ''' <param name="hintergrund_rgb">Hintergrundfarbe im RGB-Format</param>
    Public Sub _RGBCOLOR(ByVal vordergrund_rgb As Integer, ByVal hintergrund_rgb As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.ForeColor = vordergrund_rgb
            drawing.BackColor = hintergrund_rgb
        End If
    End Sub
    ''' <summary>
    ''' Setzt die Vordergroundfarbe im RGB-Format
    ''' </summary>
    ''' <param name="vordergrund_rgb">Vordergroundfarbe im RGB-Format</param>
    Public Sub _RGBCOLOR(ByVal vordergrund_rgb As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.ForeColor = vordergrund_rgb
        End If
    End Sub
    ''' <summary>
    ''' Setzt die Vordergrundfarbe aus der Standardfarbpalette(0-15)
    ''' </summary>
    Public Sub _COLOR(ByVal vordergrund_index As Integer)
        _RGBCOLOR(GFADrawing.QBColor(vordergrund_index))
    End Sub
    ''' <summary>
    ''' Setzt die Vordergroundfarbe und Hintergrundfarbe aus der Standardfarbpalette(0-15)
    ''' </summary>
    Public Sub _COLOR(ByVal vordergrund_index As Integer, ByVal hintergrund_index As Integer)
        _RGBCOLOR(GFADrawing.QBColor(vordergrund_index), GFADrawing.QBColor(hintergrund_index))
    End Sub
    ''' <summary>
    ''' Zeichnet einen nicht gefüllten Kreis
    ''' </summary>
    ''' <param name="x">X-Position</param>
    ''' <param name="y">Y-Position</param>
    ''' <param name="r">Radius</param>
    Public Sub _CIRCLE(ByVal x As Integer, ByVal y As Integer, ByVal r As Integer)
        'Circle ist immer nicht gefüllt (<>PCIRCLE)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawCircle(x, y, r, False)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet eine nicht gefüllte Ellipse
    ''' </summary>
    ''' <param name="x">X-Position</param>
    ''' <param name="y">Y-Position</param>
    ''' <param name="w">Weite</param>
    ''' <param name="h">Höhe</param>
    ''' <remarks></remarks>
    Public Sub _ELLIPSE(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawEllipse(x, y, w, h, False)
        End If
    End Sub
    ''' <summary>
    ''' Ändert Dicke und Style des Zeichenstifts
    ''' </summary>
    Public Sub _DEFLINE(ByVal style As Integer, ByVal thinkness As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DefLine(style, thinkness)
        End If
    End Sub
    ''' <summary>
    ''' Ändert Style des Zeichenstifts
    ''' </summary>
    Public Sub _DEFLINE(ByVal style As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DefLine(style)
        End If
    End Sub
    ''' <summary>
    ''' Setzt den Füllmodus. Untersützt Werte von 0 - 48.
    ''' </summary>
    Public Sub _DEFFILL(ByVal pattern As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DefFill(pattern)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet eine Linie
    ''' </summary>
    ''' <param name="x1">X1-Position</param>
    ''' <param name="y1">Y1-Position</param>
    ''' <param name="x2">X2-Position</param>
    ''' <param name="y2">Y2-Position</param>
    Public Sub _LINE(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawLine(x1, y1, x2, y2)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet einen Pixel
    ''' </summary>
    ''' <param name="x">X-Position</param>
    ''' <param name="y">Y-Position</param>
    Public Sub _PLOT(ByVal x As Integer, ByVal y As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawPixel(x, y)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet ein nicht gefülltes Rechteck
    ''' </summary>
    ''' <param name="x1">X1-Position</param>
    ''' <param name="y1">Y1-Position</param>
    ''' <param name="x2">X2-Position</param>
    ''' <param name="y2">Y2-Position</param>
    Public Sub _BOX(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawRectangle(x1, y1, x2, y2, False)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet ein gefülltes Rechteck
    ''' </summary>
    ''' <param name="x1">X1-Position</param>
    ''' <param name="y1">Y1-Position</param>
    ''' <param name="x2">X2-Position</param>
    ''' <param name="y2">Y2-Position</param>
    Public Sub _PBOX(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawRectangle(x1, y1, x2, y2, True)
        End If
    End Sub
    ''' <summary>
    ''' Füll den Hintergrund mit weiss
    ''' </summary>
    Public Sub _CLS()
        'CLS setzt auch die Position von Print zurück
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.Clear() ' Fenster mit WHITENESS füllen
        End If
    End Sub
    ''' <summary>
    ''' Füllt den Zeichenbereich
    ''' </summary>
    Public Sub _FILL(ByVal x As Integer, ByVal y As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.Fill(x, y, drawing.GetPixelColor(x, y), False)
        End If
    End Sub
    ''' <summary>
    ''' Füllt den Zeichenbereich
    ''' </summary>
    Public Sub _FILL(ByVal x As Integer, ByVal y As Integer, ByVal borderColor As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.Fill(x, y, borderColor, True)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet den angegbenen Text
    ''' </summary>
    Public Sub _TEXT(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawTextString(x, y, text)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet den Text in dem angegbenen Bereich mit dem angegbenen Alignment.
    ''' </summary>
    Public Sub _DRAWTEXT(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal text As String, ByVal style As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawTextString(x1, y1, x2, y2, text, style)
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet einen gefüllten Kreis
    ''' </summary>
    ''' <param name="x">X-Position</param>
    ''' <param name="y">Y-Position</param>
    ''' <param name="r">Radius</param>
    Public Sub _PCIRCLE(ByVal x As Integer, ByVal y As Integer, ByVal r As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawCircle(x, y, r, True)
        End If
    End Sub
    ''' <summary>
    ''' Setzt den Grafikmodus
    ''' </summary>
    Public Sub _GRAPHMODE(ByVal Rop2 As Integer, ByVal BkMode As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.GraphMode(Rop2, BkMode)
        End If
    End Sub
    ''' <summary>
    ''' Setzt den Grafikmodus
    ''' </summary>
    Public Sub _GRAPHMODE(ByVal Rop2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.GraphMode(Rop2)
        End If
    End Sub
    ''' <summary>
    ''' Lädt eine BMP-Datei
    ''' </summary>
    ''' <param name="filename">Dateinamen</param>
    ''' <returns>Bitmap handle</returns>
    Public Function _LOADBMP(ByVal filename As String) As IntPtr
        Return LoadImage(IntPtr.Zero, filename, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE)
    End Function
    ''' <summary>
    ''' Gibt ein Bitmap Handle frei
    ''' </summary>
    ''' <param name="bmp">Bitmap handle</param>
    Public Sub _FREEBMP(ByVal bmp As IntPtr)
        If bmp <> IntPtr.Zero Then
            DeleteObject(bmp)
        End If
    End Sub
    ''' <summary>
    ''' Gibt den angegebenen DC (Device Context) frei
    ''' </summary>
    ''' <param name="hDC">Device Context</param>
    Public Sub _FREEDC(ByVal hDC As IntPtr)
        'Dim drawing As GFADrawing
        If hDC <> IntPtr.Zero Then
            'Wenn dies der aktuell gesetzte DC ist
            'If GFADrawing.ActiveDC = hDC Then
            'drawing = GFADrawing.ActiveDrawing
            'If drawing IsNot Nothing Then
            'drawing.Dispose() 'Sicherstellen, das die Ressourcen jetzt freigegeben werden
            'GFADrawing.ActiveDrawing = Nothing
            'End If
            'End If
            'TODO: Fraglich, ob das so gut ist...
            'DeleteObject(SelectObject(hDC, GetStockObject(BLACK_PEN))) ' Free Pen Ressource
            DeleteDC(hDC)
        End If
    End Sub
    ''' <summary>
    ''' Gibt die Länge in Pixel für den angegebenen String zurück.
    ''' </summary>
    ''' <param name="text">String</param>
    ''' <returns>Weite in Pixel</returns>
    Public Function _TXTLEN(ByVal text As String) As Integer
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        _TXTLEN = 0
        If drawing IsNot Nothing Then
            _TXTLEN = drawing.GetTextWidth(text)
        End If
    End Function
    ''' <summary>
    ''' Stellt ein nicht gefülltes Polygon
    ''' </summary>
    Public Sub _POLYLINE(ByVal n As Integer, ByRef x_array() As Double, ByRef y_array() As Double)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawPolygon(n, x_array, y_array, False)
        End If
    End Sub
    ''' <summary>
    ''' Stellt ein gefülltes Polygon dar
    ''' </summary>
    Public Sub _POLYFILL(ByVal n As Integer, ByRef x_array() As Double, ByRef y_array() As Double)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawPolygon(n, x_array, y_array, True)
        End If
    End Sub
    ''' <summary>
    ''' Erstellt ein Bitmap aus dem angegebenen Rechteck. Achtung! Das Bitmaphandle muss mit FREEBMP freigegeben werden
    ''' </summary>
    Public Sub _GET(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef hBitmap As IntPtr)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.GrabBitmap(x, y, width, height, hBitmap)
        End If
    End Sub
    ''' <summary>
    ''' Erstellt ein Bitmap aus dem angegebenen Rechteck. 
    ''' </summary>
    Public Sub _GET2(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef bmpStr As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.GrabBitmap(x, y, width, height, bmpStr)
        End If
    End Sub
    ''' <summary>
    ''' Stellt das Bitmap an der angegebenen Stelle dar
    ''' </summary>
    Public Sub _PUT(ByVal x As Integer, ByVal y As Integer, ByVal hBitmap As IntPtr)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawBitmap(x, y, hBitmap)
        End If
    End Sub
    ''' <summary>
    ''' Stellt ein Bitmap aus einem String dar. PUT mit einem Bitmap-Handle ist jedoch schneller 
    ''' </summary>
    Public Sub _PUT2(ByVal x As Integer, ByVal y As Integer, ByVal bmpStr As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawBitmap(x, y, bmpStr)
        End If
    End Sub
    ''' <summary>
    ''' Stellt ein Bitmap gestreckt oder gestaucht dar
    ''' </summary>
    Public Sub _STRETCH(ByVal x As Integer, ByVal y As Integer, ByVal bitmap As IntPtr, ByVal width As Integer, ByVal height As Integer, ByVal ROP As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.StretchBitmap(x, y, bitmap, width, height, ROP)
        End If
    End Sub
    ''' <summary>
    ''' Stellt ein Bitmap gestreckt oder gestaucht dar
    ''' </summary>
    Public Sub _STRETCH(ByVal x As Integer, ByVal y As Integer, ByVal bitmap As IntPtr, ByVal width As Integer, ByVal height As Integer)
        _STRETCH(x, y, bitmap, width, height, SRCCOPY)
    End Sub
    ''' <summary>
    ''' Entspricht dem GFA Befehl PRINT. Wenn mehrere Strings angegeben werden, werden diese wie in GFA an die nächste durch 16 Teilbare Stelle gesetzt. Wenn die letzte Zeile erreicht ist, wird der Bildschirminhalt um eine Zeile nach oben gescrollt.
    ''' </summary>
    Public Sub _PRINTTEXT(ByVal ParamArray texts() As Object)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.PrintText(True, True, texts)
        End If
    End Sub
    ''' <summary>
    ''' Gibt den Text an der angegebenen Position aus.
    ''' </summary>
    Public Sub _PRINT_AT(ByVal column As Integer, ByVal row As Integer, ByVal text As String)
        'BEI GFA16 ist PRINT AT(0,0) der Uhrsprung, bei GFA32 PRINT AT(0,1)
        'Achtung: Ursprüngliche Position darf nicht wieder zurückgesetzt werden
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If text IsNot Nothing Then
            If drawing IsNot Nothing Then
                drawing.PrintLocation = New Point(column, row)
                If text = "" Then
                    drawing.PrintText(True, False, text)
                Else
                    drawing.PrintText(True, True, text)
                End If

            End If
        End If
    End Sub
    ''' <summary>
    ''' Gibt den Text an der angegebenen Position aus.
    ''' </summary>
    Public Sub _PRINT_AT(ByVal column As Integer, ByVal row As Integer)
        _PRINT_AT(column, row, "")
    End Sub
    ''' <summary>
    ''' Unterstüzt zwar Exponentialschreibweise mit ^, die Darstellung ist jedoch nicht gleich wie in GFA!
    ''' </summary>
    Public Sub _PRINT_USING(ByVal format As String, ByVal ParamArray objs() As Object)
        _PRINTTEXT(GFAUsing.FormatObjects(format, objs))
    End Sub
    ''' <summary>
    ''' Setzt die Position für grafische PRINT Befehle
    ''' </summary>
    Public Sub _LOCATE(ByVal column As Integer, ByVal row As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.PrintLocation = New Point(column, row)
        End If
    End Sub
    ''' <summary>
    ''' Entfernt den Clippingbereich
    ''' </summary>
    Public Sub _CLIPOFF()
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.ClipOff()
        End If
    End Sub
    ''' <summary>
    ''' Fügt einen Clippingbereich ein
    ''' </summary>
    Public Sub _CLIP(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.Clip(x, y, width, height)
        End If
    End Sub
    ''' <summary>
    ''' Fügt einen Clippingbereich ein
    ''' </summary>
    Public Sub _CLIP(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal offsetX As Integer, ByVal offsetY As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.Clip(x, y, width, height, offsetX, offsetY)
        End If
    End Sub
    ''' <summary>
    ''' Zeicheneingabe
    ''' </summary>
    Public Sub _FORM_INPUT(ByVal length As Integer, ByRef str As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        Dim col As Integer = _CRSCOL()
        Dim row As Integer = _CRSLIN()
        Dim oldForeColor As Integer
        Dim done As Boolean = False
        Dim lastRedraw As Integer
        'Dim orgMinimumSize As Size
        'Dim orgMaximumSize As Size
        'Dim orgMaximizeBox As Boolean

        'Sicherstellen das das Fenster den Fokus hat (ist wichtig!)
        Dim wnd As GFAWindow = GFAWindows.CurrentWindow
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                '_WIN(wnd.ID) 'Sicherstellen, das überhaupt ein Fenster aktiv ist
                wnd.Form.Focus()

                'Nicht verwendet, da Fensterinhalt dann gelöscht wird
                'orgMinimumSize = wnd.Form.MinimumSize
                'orgMaximumSize = wnd.Form.MinimumSize
                'orgMaximizeBox = wnd.Form.MaximizeBox

                Try
                    'wnd.Form.MinimumSize = wnd.Form.Size
                    'wnd.Form.MaximumSize = wnd.Form.Size
                    'wnd.Form.MaximizeBox = False

                    If length < 1 Then length = 1

                    'Es kann bereits etwas im String stehen
                    If Len(str) > length Then
                        str = Microsoft.VisualBasic.Left(str, length)
                    End If
                    lastRedraw = _TIMER()
                    If drawing IsNot Nothing Then
                        Dim ch$
                        drawing.PrintText(True, True, str + "_") 'Nur hier scrollen

                        Do
                            ch$ = _INKEY()
                            Select Case ch$
                                Case Chr(27)
                                Case Chr(9)
                                Case Chr(8)
                                    If Len(str) > 0 Then
                                        Try
                                            oldForeColor = drawing.ForeColor
                                            drawing.ForeColor = drawing.BackColor
                                            _LOCATE(col, row)
                                            drawing.PrintText(False, True, str + "_")
                                        Finally
                                            drawing.ForeColor = oldForeColor
                                        End Try

                                        str = Microsoft.VisualBasic.Left(str, Len(str) - 1)

                                        _LOCATE(col, row)
                                        drawing.PrintText(False, True, str + "_")
                                    End If
                                Case Chr(10), Chr(13), Chr(13) + Chr(10), Chr(10) + Chr(13)
                                    done = True
                                Case Else

                                    If Len(ch$) = 1 And Len(str) < length Then
                                        str += ch$
                                        _LOCATE(col, row)
                                        drawing.PrintText(False, True, str + "_")
                                    End If
                            End Select

                            If _TIMER() - lastRedraw > 250 Then
                                lastRedraw = _TIMER()
                                _LOCATE(col, row)
                                drawing.PrintText(False, True, str + "_")
                            End If

                            If ch$ = "" Then
                                System.Threading.Thread.Sleep(1)
                            End If

                        Loop Until done

                        Try
                            oldForeColor = drawing.ForeColor
                            drawing.ForeColor = drawing.BackColor
                            _LOCATE(col, row)
                            drawing.PrintText(False, True, str + "_")
                        Finally
                            drawing.ForeColor = oldForeColor
                        End Try
                        _LOCATE(col, row)
                        drawing.PrintText(False, True, str)
                    End If

                Finally
                    'Schierstellen, das die Einstellungen wiederhergestellt werden
                    'wnd.Form.MinimumSize = orgMinimumSize
                    'wnd.Form.MaximumSize = orgMaximumSize
                    'wnd.Form.MaximizeBox = orgMaximizeBox
                End Try

            End If
        End If
    End Sub
    ''' <summary>
    ''' Zeicheneingabe
    ''' </summary>
    Public Sub _INPUTTEXT(ByRef str As String)
        _FORM_INPUT(256, str)
    End Sub
    ''' <summary>
    ''' Beginnt das Erstellen einer WMF-Datei
    ''' </summary>
    ''' <param name="filename">Dateiname</param>
    Public Sub _CREATEMETA(ByVal filename As String)
        Dim newDC As IntPtr = CreateMetaFile(filename)
        If newDC <> IntPtr.Zero Then
            GFADrawing.MetaLastDrawing = GFADrawing.ActiveDrawing
            GFADrawing.ActiveDrawing = New GFADrawing(newDC)
            GFADrawing.MetaDrawing = True
        End If
    End Sub
    ''' <summary>
    ''' Beendet das Erstellen einer WMF-Datei
    ''' </summary>
    Public Sub _CLOSEMETA()
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        Dim metaDC As IntPtr = IntPtr.Zero
        If drawing IsNot Nothing Then

            If GFADrawing.MetaDrawing Then
                'Ressourcen freigben

                metaDC = GFADrawing.ActiveDC
                drawing.Dispose()
                CloseMetaFile(metaDC) 'DC schließen
                'Altes Zeichenobjekt wiederherstellen
                GFADrawing.MetaDrawing = False
                GFADrawing.ActiveDrawing = GFADrawing.MetaLastDrawing
            End If
        End If
    End Sub
    ''' <summary>
    ''' Zeichnet die angegebene WMF-Datei
    ''' </summary>
    ''' <param name="filename">Dateiname</param>
    Public Sub _PLAYMETA(ByVal filename As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.DrawWMFFile(filename)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName As String, ByVal value As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName, value)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer, ByVal propertyName6 As String, ByVal value6 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
            drawing.SetFontCreationProperty(propertyName6, value6)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer, ByVal propertyName6 As String, ByVal value6 As Integer, ByVal propertyName7 As String, ByVal value7 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
            drawing.SetFontCreationProperty(propertyName6, value6)
            drawing.SetFontCreationProperty(propertyName7, value7)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer, ByVal propertyName6 As String, ByVal value6 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
            drawing.SetFontCreationProperty(propertyName6, value6)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT(ByVal fontName As String, ByVal propertyName1 As String, ByVal value1 As Integer, ByVal propertyName2 As String, ByVal value2 As Integer, ByVal propertyName3 As String, ByVal value3 As Integer, ByVal propertyName4 As String, ByVal value4 As Integer, ByVal propertyName5 As String, ByVal value5 As Integer, ByVal propertyName6 As String, ByVal value6 As Integer, ByVal propertyName7 As String, ByVal value7 As Integer)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            drawing.SetFontCreationProperty("FONTNAME", fontName)
            drawing.SetFontCreationProperty(propertyName1, value1)
            drawing.SetFontCreationProperty(propertyName2, value2)
            drawing.SetFontCreationProperty(propertyName3, value3)
            drawing.SetFontCreationProperty(propertyName4, value4)
            drawing.SetFontCreationProperty(propertyName5, value5)
            drawing.SetFontCreationProperty(propertyName6, value6)
            drawing.SetFontCreationProperty(propertyName7, value7)
        End If
    End Sub
    ''' <summary>
    ''' Definiert eine Schriftart
    ''' </summary>
    Public Sub _FONT_TO(ByRef hFont As IntPtr)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            hFont = drawing.CreateFontHandle()
        End If
    End Sub
    ''' <summary>
    ''' Gibt ein Fonthandle frei
    ''' </summary>
    Public Sub _FREEFONT(ByVal hFont As IntPtr)
        If hFont <> IntPtr.Zero Then
            DeleteObject(hFont)
        End If
    End Sub
    ''' <summary>
    ''' ACHTUNG! Verhält sich nicht gleich wie in GFA. Hier wird nur ein Leerstring mit der angegebenen Länge erzeugt.
    ''' </summary>
    ''' <param name="num">Anzahl Leerzeichen</param>
    Public Function _TAB(ByVal num As Integer) As String
        Return New String(" ", num)
        'If num > 1 And num < 256 Then
        '    Return "@??_TAB" + _DEC(num, 3) 'New String(" ", num)
        'Else
        '    Return ""
        'End If
    End Function
    ''' <summary>
    ''' Y-Cursoposition für grafische PRINT-Befehle
    ''' </summary>
    Public Function _CRSLIN()
        Dim result As Integer = 0
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            result = drawing.PrintLocation.Y
        End If
        Return result
    End Function
    ''' <summary>
    ''' X-Cursoposition für grafische PRINT-Befehle
    ''' </summary>
    Public Function _CRSCOL()
        Dim result As Integer = 0
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            result = drawing.PrintLocation.X
        End If
        Return result
    End Function
    ''' <summary>
    ''' Neue Druckerseite
    ''' </summary>
    Public Sub _NEWFRAME()
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            _LOCATE(0, 0) 'Auf Ursprung setzen
            Escape(drawing.DC, NEWFRAME, 0, Nothing, Nothing)
        End If
    End Sub
    ''' <summary>
    ''' Beginnt eine Druckerseite
    ''' </summary>
    Public Sub _STARTDOC(ByVal title As String)
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            _LOCATE(0, 0) ' Auch auf Ursprung setzen
            EscapeString(drawing.DC, STARTDOC, Len(title), title, Nothing)
        End If
    End Sub
    ''' <summary>
    ''' Ende der Druckerseite
    ''' </summary>
    Public Sub _ENDDOC()
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            Escape(drawing.DC, ENDDOC, 0, Nothing, Nothing)
        End If
    End Sub
    ''' <summary>
    ''' Gibt den DC des Druckers zurück
    ''' </summary>
    Public Function _PRINTERDC() As IntPtr
        'TODO: Funktioniert wohl nicht korrekt
        Dim DC As IntPtr = CreateDC("WINSPOOL", Nothing, Nothing, Nothing)
        Return DC
    End Function
    
    ''' <summary>
    ''' Gibt Integer-Wert aus den RGB-Farbwerten r,g und b zurück
    ''' </summary>
    Public Function _RGB(byval r As Integer, byval g As Integer, byval b As Integer) As Integer
        Return r + (g << 8) + (b << 16)
    End Function    
#End Region
#Region "GFABasic - Uhrzeit und Datum"
    ''' <summary>
    ''' Unterbricht für n/18.2 Sekunden
    ''' </summary>
    Public Sub _PAUSE(ByVal n As Integer)
        Threading.Thread.Sleep((n * 1000.0F) / 18.2F)
    End Sub
    ''' <summary>
    ''' Unterbricht für n Sekunden
    ''' </summary>
    Public Sub _DELAY(ByVal n As Integer)
        Threading.Thread.Sleep(n * 1000)
    End Sub
    ''' <summary>
    ''' Datum ist immer im Format TT.MM.JJJJ und kann nicht durch MODE geändert werden
    ''' </summary>
    Public Function _DATE$()
        Try
            Return Format(DateTime.Now, "dd.MM.yyyy")
        Catch ex As Exception
            Return "01.01.0001"
        End Try
    End Function
    ''' <summary>
    ''' Zeit im Format HH:MM:SS
    ''' </summary>
    Public Function _TIME$()
        Try
            Return Format(DateTime.Now, "HH:mm:ss")
            'Return String.Format(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, "{0:T}", DateTime.Now)
        Catch ex As Exception
            Return "00:00:00"
        End Try
    End Function
    ''' <summary>
    ''' Millisekunden seit dem Systemstart. Achtung! gleicher Name wie "Timer" Property. 
    ''' </summary>
    Public Function _TIMER() As Integer
        Return Environment.TickCount
    End Function
#End Region
#Region "GFABasic - Mausfunktionen"
    ''' <summary>
    ''' Zeigt den Mauscursor an
    ''' </summary>	
    Public Sub _SHOWM()
        Cursor.Show()
    End Sub
    ''' <summary>
    ''' Versteckt den Mauscursor
    ''' </summary>	
    Public Sub _HIDEM()
        Cursor.Hide()
    End Sub
    ''' <summary>
    ''' Setzt das Symbol des Mauscursors. Mögliche Werte 0 - 15
    ''' </summary>	
    Public Sub _DEFMOUSE(ByVal x As Integer)
        Select Case x
            Case 0
                GFAWindows.CurrentCursor = Cursors.Default
            Case 1
                GFAWindows.CurrentCursor = Cursors.Default
            Case 2
                GFAWindows.CurrentCursor = Cursors.Cross
            Case 3
                GFAWindows.CurrentCursor = Cursors.IBeam
            Case 4
                GFAWindows.CurrentCursor = Cursors.Default
            Case 5
                GFAWindows.CurrentCursor = Cursors.SizeAll
            Case 6
                GFAWindows.CurrentCursor = Cursors.SizeNESW
            Case 7
                GFAWindows.CurrentCursor = Cursors.SizeNS
            Case 8
                GFAWindows.CurrentCursor = Cursors.SizeNWSE
            Case 9
                GFAWindows.CurrentCursor = Cursors.SizeWE
            Case 10
                GFAWindows.CurrentCursor = Cursors.UpArrow
            Case 11
                GFAWindows.CurrentCursor = Cursors.WaitCursor
            Case 12
                GFAWindows.CurrentCursor = Cursors.No
            Case 13
                GFAWindows.CurrentCursor = Cursors.AppStarting
            Case 14
                GFAWindows.CurrentCursor = Cursors.Help
            Case 15
                GFAWindows.CurrentCursor = Cursors.SizeAll ' wie 5
            Case Else
                GFAWindows.CurrentCursor = Cursors.Default
        End Select
    End Sub
    ''' <summary>
    ''' Setzt die absolute Mausposition wie in GFA.
    ''' </summary>
    ''' <param name="x">x-Position</param>
    ''' <param name="y">y-Position</param>
    ''' <remarks></remarks>
    Public Sub _SETMOUSE(ByVal x As Integer, ByVal y As Integer)
        Cursor.Position = New Point(x, y)
    End Sub
    ''' <summary>
    ''' Gibt die X-Koordinate der absoluten Mausposition zurück
    ''' </summary>	
    Public Function _MOUSESX() As Integer
        Return Cursor.Position.X
    End Function
    ''' <summary>
    ''' Gibt die Y-Koordinate der absoluten Mausposition zurück
    ''' </summary>	
    Public Function _MOUSESY() As Integer
        Return Cursor.Position.Y
    End Function
    ''' <summary>
    ''' Gibt die gedrückten Maustasten zurück (1=linke Maustaste, 2=rechte Maustaste, 3=beide)
    ''' </summary>		
    Public Function _MOUSEK() As Integer
        'MouseK gibt in GFA auch unabhängig vom aktiven Fenster(Fenster muss nicht aktiv sein) immer den Mausstatus zurück
        Dim result As Integer = 0
        If ((GetAsyncKeyState(VK_LBUTTON) And 32768) <> 0) Then
            result = 1
        End If
        If ((GetAsyncKeyState(VK_RBUTTON) And 32768) <> 0) Then
            result = result + 2
        End If
        Return result
    End Function
    ''' <summary>
    ''' X-Koordinate der Maus relativ zum Clientbereich des aktuellen Fenseters
    ''' </summary>
    ''' <remarks></remarks>
    Public Function _MOUSEX() As Integer
        'Die Koordinate ist relativ zum Clientbereich (Kann auch negative Werte zurückgeben)
        Dim pt As Point = New Point(0, 0)
        Dim frm As GFAWindow = GFAWindows.CurrentWindow
        If frm IsNot Nothing Then
            If frm.Form IsNot Nothing Then
                pt = frm.Form.PointToScreen(pt)
            End If
        End If
        Return Cursor.Position.X - pt.X
    End Function
    ''' <summary>
    ''' Y-Koordinate der Maus relativ zum Clientbereich des aktuellen Fenseters
    ''' </summary>
    ''' <remarks></remarks>
    Public Function _MOUSEY() As Integer
        'Die Koordinate ist relativ zum Clientbereich (Kann auch negative Werte zurückgeben)
        Dim pt As Point = New Point(0, 0)
        Dim frm As GFAWindow = GFAWindows.CurrentWindow
        If frm IsNot Nothing Then
            If frm.Form IsNot Nothing Then
                pt = frm.Form.PointToScreen(pt)
            End If
        End If
        Return Cursor.Position.Y - pt.Y
    End Function
    ''' <summary>
    ''' Ermittelt den Mausstatus
    ''' </summary>
    Public Sub _MOUSE(ByRef x As Integer, ByRef y As Integer, ByRef mk As Integer)
        x = _MOUSEX()
        y = _MOUSEY()
        mk = _MOUSEK()
    End Sub
#End Region
#Region "GFABasic - Betriebsystemfunktionen"
    'Private Menu_LastClickWinID As Integer 'TODO: Bisher nicht implemntiert!
    Public Delegate Sub _ON_MENU_GOSUB_DELEGATE()
    Public Delegate Sub _ON_KEY_GOSUB_DELEGATE()
    Public Delegate Sub _ON_MESSAGE_GOSUB_DELEGATE()
    Public _ON_MENU_GOSUB As _ON_MENU_GOSUB_DELEGATE = Nothing
    Public _ON_KEY_GOSUB As _ON_KEY_GOSUB_DELEGATE = Nothing
    Public _ON_MESSAGE_GOSUB As _ON_MESSAGE_GOSUB_DELEGATE = Nothing
    'Definieren mit:
    '_ON_MENU_GOSUB = New _ON_MENU_GOSUB_DELEGATE(AddressOf test)
    ''' <summary>
    ''' Führt das angegebene Programm aus. Ein Rückgabewert > 32 bedeutet erfolgreich 
    ''' </summary>		
    Public Function _EXEC(ByVal name As String, ByVal cmdline As String) As Integer
        Dim result As Integer = 0
        Try
            Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
            p.StartInfo.FileName = name
            p.StartInfo.WorkingDirectory = ""
            p.StartInfo.Arguments = cmdline
            p.Start()
            p.WaitForExit()
            Return 33 ' > 32 bedeutet erfolgreich
        Catch ex As System.ComponentModel.Win32Exception
            result = ex.NativeErrorCode
            If result > 32 Then 'Sicherstellen, das keine Werte > 32 zurückgegeben werden!
                result = 0
            End If
        Catch ex As Exception
            result = 0 ' Bedeutet eigentlich "The operating system is out of memory or resources."
        End Try
        Return result
    End Function
    ''' <summary>
    ''' Führt die Angegebene EXE-Datei aus
    ''' </summary>
    Public Sub _CHAIN(ByVal Filename As String)
        'Try
        If IO.File.Exists(Filename) Then
            Shell(Filename)
        End If
        'Catch ex As Exception
        'Keine Exceptions veruhrsachen, wie in GFA (veruhrsacht exception, wenn Datei nicht gefunden wird)
        'End Try
    End Sub
    ''' <summary>
    ''' Gibt wie in GFA 16-Bit die gedrückte Taste zurück.
    ''' </summary>		
    Public Function _INKEY() As String
        Application.DoEvents() ' Funktioniert nur, wenn Events verarbeitet werden
        Return GFAWindows.INKEY
    End Function
    ''' <summary>
    ''' Wartet auf Tastatureingabe
    ''' </summary>
    Public Sub _KEYGET(ByRef key As Integer)
        key = 0
        Dim ch As String = ""
        Do
            ch = _INKEY()
            If ch = "" Then
                System.Threading.Thread.Sleep(1) ' CPU schonen
            End If
        Loop Until ch <> ""
        If Len(ch) = 1 Then
            key = Asc(ch)
        Else
            key = _CVI(ch)
        End If
    End Sub
    '''' <summary>
    '''' Gibt die gedrückte Taste zurück. Nicht gleich, wie INKEY$ in GFA, da das Zeichen dauerhaft zurückgegeben wird und das Fenster nicht im Vordergrund sein muss. 
    '''' </summary>		
    'Public Function _INKEY_RAW$()
    'Dim result As String = ""
    'Dim ch As Char
    'Dim len As Integer
    'Dim keyStates = New Byte(256) {}
    'Dim vk As Integer
    '
    '    Application.DoEvents() ' Muss aufgerufen werden!
    '    GetKeyboardState(keyStates)
    '    For vk = 0 To 255
    'Dim value As Integer = keyStates(vk)
    '        If (value And (1 << 7)) Then
    '            ch = ""
    '            len = ToAscii(vk, MapVirtualKey(vk, 0), keyStates, ch, 0)
    '            If len > 0 Then
    '                Return ch
    '            End If
    '        End If
    '    Next
    '    Return ""
    'End Function
    ''' <summary>
    ''' Gibt Kommandozeile zurück
    ''' </summary>
    Public Function __dosCmd$()
        Return Microsoft.VisualBasic.Command()
    End Function
    ''' <summary>
    ''' Erstellt ein Menü
    ''' </summary>
    ''' <param name="entries">Array mit Menüpunkten</param>
    Public Sub _MENU(ByVal entries() As String)
        Dim wndMenu As MainMenu
        Dim i As Integer = 0
        Dim newMenu As Boolean = True
        'Dim menuStrp As MenuItem = New MenuItem
        Dim items As List(Of MenuItem) = New List(Of MenuItem)
        Dim menues As List(Of MenuItem) = New List(Of MenuItem)
        Dim title As String = ""

        If GFAWindows.CurrentWindow IsNot Nothing Then
            If GFAWindows.CurrentWindow.Form IsNot Nothing Then
                'If GFAWindows.CurrentWindow.Menu Is Nothing Then ' Is Nothing!
                'wndMenu.Parent = GFAWindows.CurrentWindow.Form
                For i = 0 To entries.Length - 1

                    If entries(i) = "" Then
                        If title <> "" Then
                            menues.Add(New MenuItem(title, items.ToArray()))
                            items = New List(Of MenuItem)
                            title = ""
                        End If
                    Else
                        If title <> "" Then
                            Dim item As GFAMenuItem = New GFAMenuItem(GFAWindows.CurrentWindow.Form.Handle, GFAWindows.CurrentWindow.ID)
                            item.Tag = i
                            item.Text = entries(i)
                            If entries(i) = "-" Then
                                items.Add(item)
                            Else
                                items.Add(item)
                            End If
                        Else
                            title = entries(i)
                        End If

                    End If
                Next
                wndMenu = New MainMenu(menues.ToArray)
                GFAWindows.CurrentWindow.Form.Menu = wndMenu '2010-09-15 hinzugefügt
            End If
        End If
    End Sub
    ''' <summary>
    ''' Abfrage von Eventereignissen
    ''' </summary>
    Public Function _MENU(ByVal index As Integer) As Integer
        Return GFAMisc.MenuResult(index)
    End Function
    ''' <summary>
    ''' Interner Eventbefehl
    ''' </summary>
    Private Sub ___ProcessGFAEvents(ByVal ProcessGoSubs As Boolean, ByVal Wait As Boolean)
        Dim CallOnMenuCallback As Boolean = False
        Dim CallOnKeyCallback As Boolean = False
        Dim CallOnMessageCallback As Boolean = False

        If GFAWindows.Events.Count = 0 Then ' Nur neue Events verarbeiten, wenn alle abgearbeitet sind (das sich keine Events aufstauen)
            Application.DoEvents()
            If Wait Then
                Threading.Thread.Sleep(1) ' Warten um CPU zu schonen
            End If
        End If
        'Daten die immer aktualisiert werden...

        'Menu(4) the status of the mouse keys:
        'MENU(4)=0	no mouse key was pressed
        'MENU(4)=1	the left mouse button was pressed
        'MENU(4)=2	the right mouse button was pressed
        GFAMisc.MenuResult(2) = _MOUSESX() 'mouse x-position (corresponds to the MOUSESX function)
        GFAMisc.MenuResult(3) = _MOUSESY() 'mouse y-position (corresponds to the MOUSESY function)
        GFAMisc.MenuResult(4) = _MOUSEK() ' Evtl. nicht genau gleich

        'MENU(7)	returns the number of the GFA-BASIC window above which the mouse was located when the mouse button was 
        'pressed.
        GFAMisc.MenuResult(7) = 0 'Menu_LastClickWinID 'TODO: Bisher nicht implemntiert!

        'MENU(16)	time in ms since booting
        GFAMisc.MenuResult(16) = Environment.TickCount
        GFAMisc.MenuResult(1) = 0 ' Kein GFA event
        GFAMisc.MenuResult(0) = 0 'Menupunkt zunächst auf 0 setzen

        'Evtl. Problematisch...
        GFAMisc.MenuResult(11) = 0
        GFAMisc.MenuResult(12) = 0
        GFAMisc.MenuResult(13) = 0
        GFAMisc.MenuResult(14) = 0
        GFAMisc.MenuResult(15) = 0

        'Keyboardstatus
        GFAMisc.MenuResult(5) = 0

        'Maimize, Minimize Event
        GFAMisc.MenuResult(7) = 0
        GFAMisc.MenuResult(8) = 0
        GFAMisc.MenuResult(9) = 0

        If GFAWindows.Events.Count > 0 Then
            Dim msg As GFAEvent = GFAWindows.Events(0)
            'MENU(11)	WINDOWS-ID message
            'MENU(12)	WParameter
            'MENU(13)	LParamter
            GFAMisc.MenuResult(11) = msg.Msg
            GFAMisc.MenuResult(12) = msg.WParam
            GFAMisc.MenuResult(13) = msg.LParam

            'MENU(14)	GFA-BASIC window handle(0-31)
            'MENU(15)	Windows window handle
            GFAMisc.MenuResult(14) = msg.ID
            GFAMisc.MenuResult(15) = msg.HWnd
            'Menuitem wurde angeklick!
            If msg.Msg = WM_COMMAND Then
                GFAMisc.MenuResult(1) = 20 ' Menu oder Popupeintrag ausgewählt
                GFAMisc.MenuResult(0) = msg.WParam
                CallOnMenuCallback = True
            End If

            If msg.Msg = WM_CHAR Then
                GFAMisc.MenuResult(1) = 1
                GFAMisc.MenuResult(5) = msg.WParam '& 255 'TODO: ACHTUNG UNTERSTÜTZT SCANCODE NICHT!
                CallOnKeyCallback = True
            End If

            If msg.Msg = WM_KEYDOWN Then
                GFAMisc.MenuResult(1) = 1
                GFAMisc.MenuResult(5) = msg.WParam * 256 '& 255 'TODO: ACHTUNG UNTERSTÜTZT SCANCODE NICHT!
                CallOnKeyCallback = True
            End If

            If msg.Msg = WM_PAINT Then
                GFAMisc.MenuResult(1) = 21
            End If

            If msg.Msg = WM_CLOSE Then
                GFAMisc.MenuResult(1) = 4
            End If

            If msg.Msg = WM_SIZE Then
                GFAMisc.MenuResult(7) = msg.LParam And &HFFFF
                GFAMisc.MenuResult(8) = (msg.LParam >> 16) & &HFFFF

                If msg.WParam = SIZE_MINIMIZED Then
                    GFAMisc.MenuResult(1) = 5
                    GFAMisc.MenuResult(9) = SIZEICONIC
                End If
                If msg.WParam = SIZE_MAXIMIZED Then
                    GFAMisc.MenuResult(1) = 6
                    GFAMisc.MenuResult(9) = SIZEFULLSCREEN
                End If
            End If

            GFAWindows.Events.RemoveAt(0) ' Message entfernen
            CallOnMessageCallback = True
        End If
        'Achtung, darf nicht zu oft aufgerufen werden!!!
        'CallOnMessageCallback = True 'Message Callback immer aufrufen(Mausklick...)
        If GFAMisc.MenuResult(4) Then 'Auf Mausklicks reagieren
            'TODO: Nur wenn Fenster wirklich aktiv...
            CallOnMessageCallback = True
        End If

        If ProcessGoSubs Then
            If CallOnMenuCallback Then
                If _ON_MENU_GOSUB IsNot Nothing Then
                    _ON_MENU_GOSUB.Invoke()
                End If
            End If

            If CallOnKeyCallback Then
                If _ON_KEY_GOSUB IsNot Nothing Then
                    _ON_KEY_GOSUB.Invoke()
                End If
            End If

            If CallOnMessageCallback Then 'Evtl. nicht genau gleich wie in GFA...
                If _ON_MESSAGE_GOSUB IsNot Nothing Then
                    _ON_MESSAGE_GOSUB.Invoke()
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Überwacht Menü-Events
    ''' </summary>
    Public Sub _ON_MENU()
        ___ProcessGFAEvents(True, False)
    End Sub
    ''' <summary>
    ''' Verabeitet Events
    ''' </summary>
    Public Sub _PEEKEVENT()
        'PEEKEVENT monitors the occurrence of events in menu bars, pop-up menus and windows. In contrast to ON MENU, no PROCEDURE is invoked by PEEKEVENT. 
        ___ProcessGFAEvents(False, False)
    End Sub
    ''' <summary>
    ''' Verabeitet Events und wartet. Verhält sich allerdings nicht gleich wie in GFA, da immer gewartet wird.
    ''' </summary>
    Public Sub _GETEVENT()
        ___ProcessGFAEvents(False, True)
    End Sub
    ''' <summary>
    ''' Verabeitet Events und wartet. Verhält sich wohl allerdings nicht gleich wie in GFA.
    ''' </summary>
    Public Sub _SLEEP()
        ___ProcessGFAEvents(False, True)
    End Sub
    ''' <summary>
    ''' Löscht die Zwischenablage
    ''' </summary>
    Public Sub _CLRCLIP()
        Clipboard.Clear()
    End Sub
    ''' <summary>
    ''' Setzt das Format für die Zwischenablage.Unterstützt derzeit nur CF_TEXT und CF_BITMAP
    ''' </summary>
    Public Sub _CLIPFORMAT(ByVal format As Integer)
        GFAMisc.ClipboardFormat = format
    End Sub
    ''' <summary>
    ''' Kopiert den Text in die Zwischenablage. Zuvor muss CLIPFORMAT(CF_TEXT) aufgerufen werden
    ''' </summary>
    Public Sub _CLIPCOPY(ByVal text As String, ByVal len As Integer)
        If GFAMisc.ClipboardFormat = CF_TEXT Then
            Clipboard.SetText(Mid(text, 1, len))
        End If
    End Sub
    ''' <summary>
    ''' Kopiert das Bild in die Zwischenablage. Zuvor muss _CLIPFORMAT(CF_BITMAP) aufgerufen werden
    ''' </summary>
    Public Sub _CLIPCOPY(ByVal hBmp As IntPtr, ByVal dummy As Integer)
        If GFAMisc.ClipboardFormat = CF_BITMAP Then
            Clipboard.SetImage(Bitmap.FromHbitmap(hBmp))
        End If
    End Sub
    ''' <summary>
    ''' Dummyfunktion. erzwingt Garbage Colletion, wenn Parameter dummy 0 ist.
    ''' </summary>
    Public Function _FRE(ByVal dummy As Integer) As Integer
        If dummy = 0 Then
            GC.Collect()
        End If
        Return 32768 'dummy Wert
    End Function
    ''' <summary>
    ''' Allokiert nicht verwalteten Speicher (Sollte nicht verwendet werden!)
    ''' </summary>
    Public Function _MALLOC(ByVal size As Integer) As IntPtr
        If size = -1 Then
            'Verfügbarer Speicher zurückgeben
            Return (My.Computer.Info.AvailablePhysicalMemory And &H7FFFFFFF)
        Else
            Return Marshal.AllocHGlobal(size)
        End If
    End Function
    ''' <summary>
    ''' Gibt nicht verwalteten Speicher frei (Sollte nicht verwendet werden!)
    ''' </summary>
    Public Sub _MFREE(ByVal mem As IntPtr)
        Marshal.FreeHGlobal(mem)
    End Sub
    ''' <summary>
    ''' Passt das Menu an. Für Flags wird jedoch nur MF_CHECKED unterstützt.
    ''' </summary>
    Public Sub _MENU(ByVal id As Integer, ByVal flags As Integer, ByVal text As String)
        Dim done As Boolean = False
        Dim wnd As GFAWindow
        Dim main As MainMenu
        Dim gfaItem As GFAMenuItem
        wnd = GFAWindows.CurrentWindow
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                main = wnd.Form.Menu
                If main IsNot Nothing Then
                    For Each menuItem As MenuItem In main.MenuItems
                        If menuItem IsNot Nothing Then
                            For Each item As Object In menuItem.MenuItems
                                If done = False Then
                                    If item IsNot Nothing Then
                                        'Type muss GFAToolStripMenuItem sein!
                                        If TypeOf item Is GFAMenuItem Then
                                            gfaItem = DirectCast(item, GFAMenuItem)
                                            If gfaItem.Tag = id Then
                                                gfaItem.Text = text

                                                If flags And MF_CHECKED Then
                                                    gfaItem.Checked = True
                                                Else
                                                    gfaItem.Checked = False
                                                End If
                                                'Achtung: nicht gleiches Verhalten. Sollte vertikalen Balken erzeugen...
                                                'Menüitem wird in GFA auch mit dem Flag angezeigt
                                                'If flags And MF_MENUBARBREAK Then
                                                '    item.Visible = False
                                                'Else
                                                '    item.Visible = True
                                                'End If
                                                done = True
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Beendet die Anwendung und gibt Resssourcen frei
    ''' </summary>
    Public Sub _END()
        GFAWindows.CloseAll()
        'GFADrawing.ResetDC()
        GFADrawing.ActiveDrawing = Nothing ' Ressourcen freigeben... 
        Application.Exit()
    End Sub
    ''' <summary>
    ''' Spielt einen Signalton ab
    ''' </summary>
    Public Sub _BEEP()
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
    End Sub
#End Region
#Region "GFABasic - DATA-Befehle"
    ''' <summary>
    ''' Liest Daten aus dem DATA-Bereich.
    ''' </summary>
    ''' <param name="obj">Variable, in der der Wert gespeichert werden soll</param>
    Public Sub _READ(ByRef obj)
        GFADataSection.ReadData(obj)
    End Sub
    ''' <summary>
    ''' Liest Daten aus dem DATA-Bereich.
    ''' </summary>
    Public Sub _READ(ByRef obj1, ByRef obj2)
        _READ(obj1)
        _READ(obj2)
    End Sub
    ''' <summary>
    ''' Liest Daten aus dem DATA-Bereich.
    ''' </summary>
    Public Sub _READ(ByRef obj1, ByRef obj2, ByRef obj3)
        _READ(obj1)
        _READ(obj2)
        _READ(obj3)
    End Sub
    ''' <summary>
    ''' Liest Daten aus dem DATA-Bereich.
    ''' </summary>
    Public Sub _READ(ByRef obj1, ByRef obj2, ByRef obj3, ByRef obj4)
        _READ(obj1)
        _READ(obj2)
        _READ(obj3)
        _READ(obj4)
    End Sub
    ''' <summary>
    ''' Ändert den Lesepointer für READ
    ''' </summary>
    Public Sub _RESTORE(ByVal labelName As String)
        GFADataSection.Restore(labelName)
    End Sub
    ''' <summary>
    ''' Definiert ein Label, um die Position mit Restore setzten zu können. ACHTUNG! Verhält sich nicht gleich wie in GFA, da der Befehl mehrfach aufgerufen werden kann. 
    ''' </summary>
    ''' <param name="labelName">Name des Labels</param>
    Public Sub _DATA_LABEL(ByVal labelName As String)
        GFADataSection.AddLabel(labelName)
    End Sub
    ''' <summary>
    ''' Speichert Daten im DATA-Bereich.  ACHTUNG! Verhält sich nicht gleich wie in GFA, da der Befehl mehrfach aufgerufen werden kann. 
    ''' </summary>
    ''' <param name="Value">Inhalt des DATA-Bereichs</param>
    Public Sub _DATA(ByVal value As String)
        GFADataSection.Define(value)
    End Sub
    ''' <summary>
    ''' Speichert Daten im DATA-Bereich. ACHTUNG! Verhält sich nicht gleich wie in GFA, da der Befehl mehrfach aufgerufen werden kann. 
    ''' </summary>
    ''' <param name="values">Inhalt des DATA-Bereichs</param>
    Public Sub _DATA(ByVal ParamArray values() As String)
        Dim i As Integer
        If values IsNot Nothing Then 'Kann auch keine Parameter haben?
            For i = 0 To values.Length - 1
                _DATA(values(i))
            Next
        End If
    End Sub
#End Region
#Region "GFABasic - Dialoge"
    ''' <summary>
    ''' Öffnet einen Eingabedialog
    ''' </summary>
    Public Sub _PROMPT(ByVal title As String, ByVal text As String, ByRef input As String)
        input = InputBox(text, title, input)
    End Sub
    ''' <summary>
    ''' Öffnet einen Dialog zum Auswählen einer Datei
    ''' </summary>
    Public Sub _FILESELECT(ByVal Title As String, ByVal PathExt As String, ByVal defaultFile As String, ByRef result As String)
        Dim open As Windows.Forms.OpenFileDialog = New Windows.Forms.OpenFileDialog()
        Dim Path As String = ""
        Dim Ext As String = ""
        Dim pos As Integer = InStrRev(PathExt, "\")
        If pos > 0 Then
            Ext = Mid(PathExt, pos + 1)
            Path = Mid(PathExt, 1, pos)
        Else
            Ext = PathExt
            Path = ""
        End If
        Ext = Ext.Replace("|", " ") 'Nur eine Endung unterstützen
        open.InitialDirectory = Path
        open.FileName = defaultFile
        Try
            open.Filter = Ext + "|" + Ext
        Catch ex As ArgumentException
            open.Filter = "*.*|*.*"
        End Try
        open.Title = Title
        open.FilterIndex = 0
        open.RestoreDirectory = True
        open.Multiselect = False
        open.CheckFileExists = False 'Damit auch eine Datei angegeben werden kann, die es nicht gibt (wie GFA). Konnte für Öffnen und Speichern verwendet werden...
        If (open.ShowDialog() = DialogResult.OK) Then
            result = open.FileName
        Else
            result = ""
        End If
    End Sub
    ''' <summary>
    ''' Öffnet einen Dialog zum Auswählen einer Datei
    ''' </summary>
    Public Sub _FILESELECT(ByVal Path As String, ByVal Ext As String, ByRef result As String)
        _FILESELECT("", Path, Ext, result)
    End Sub

    ''' <summary>
    ''' Öffnet einen Dialog zum Auswählen einer Farbe 
    ''' </summary>
    ''' <param name="wnd">Wird ignoriert!</param>
    ''' <param name="flags">Unterstüzt Kombination aus CC_RGBINIT, CC_FULLOPEN, CC_ANYCOLOR, CC_PREVENTFULLOPEN, CC_SOLIDCOLOR und CC_SHOWHELP</param>
    ''' <param name="colors">Array mit RGB-Farben</param>
    ''' <param name="col">RGB-Farbe</param>
    Public Sub _DLG_COLOR(ByVal wnd As Integer, ByVal flags As Integer, ByVal colors() As Integer, ByRef col As Integer)
        Dim cd As System.Windows.Forms.ColorDialog = New System.Windows.Forms.ColorDialog()
        Dim result As Integer

        'Rot und Blau müssen vertauscht werden
        result = GFAMisc.SwapRedAndBlue(col)

        cd.Color = Color.FromArgb(result Or &HFF000000)
        If flags And CC_FULLOPEN Then
            cd.FullOpen = True
        Else
            cd.FullOpen = False
        End If
        If flags And CC_RGBINIT Then
            cd.CustomColors = colors
        End If
        If flags And CC_SHOWHELP Then
            cd.ShowHelp = True
        Else
            cd.ShowHelp = False
        End If
        If flags And CC_SOLIDCOLOR Then
            cd.SolidColorOnly = True
        Else
            cd.SolidColorOnly = False
        End If
        If flags And CC_PREVENTFULLOPEN Then
            cd.AllowFullOpen = False
        Else
            cd.AllowFullOpen = True
        End If
        If flags And CC_ANYCOLOR Then
            cd.AnyColor = True
        Else
            cd.AnyColor = False
        End If
        cd.ShowDialog()
        result = cd.Color.ToArgb() And &HFFFFFF
        'Rot und Blau müssen vertauscht werden
        col = GFAMisc.SwapRedAndBlue(result)
    End Sub
    ''' <summary>
    ''' Öffnet einen Druckerdialog
    ''' </summary>
    ''' <param name="wnd">Wird ignoriert!</param>
    ''' <param name="flags">Wird ignoriert!</param>
    ''' <param name="hDC">Gibt den DC zurück</param>
    ''' <remarks></remarks>
    Public Sub _DLG_PRINT(ByVal wnd As Integer, ByVal flags As Integer, ByRef hDC As IntPtr)
        'Dim printDlg As System.Windows.Forms.PrintDialog = New System.Windows.Forms.PrintDialog()
        'printDlg.ShowNetwork = True
        'printDlg.
        'hDC = IntPtr.Zero
        'If printDlg.ShowDialog() = DialogResult.OK Then  'TODO: öffnet scheinbar (manchmal?) keinen Dialog
        '    hDC = CreateDC("", printDlg.PrinterSettings.PrinterName, Nothing, printDlg.PrinterSettings.GetHdevmode())
        '    MsgBox(hDC)
        'End If
        hDC = IntPtr.Zero
        If _Is64BitApplication() Then
            Dim pdlg As _PRINTDLGX64
            pdlg.lStructSize = Marshal.SizeOf(pdlg)
            pdlg.hDC = IntPtr.Zero
            pdlg.Flags = flags Or PD_RETURNDC

            If PrintDlg(pdlg) <> 0 Then
                hDC = pdlg.hDC
            End If
        Else
            Dim pdlg As _PRINTDLGX86
            pdlg.lStructSize = Marshal.SizeOf(pdlg)
            pdlg.hDC = IntPtr.Zero
            pdlg.Flags = flags Or PD_RETURNDC

            If PrintDlg(pdlg) <> 0 Then
                hDC = pdlg.hDC
            End If
        End If
    End Sub
    ''' <summary>
    ''' Alert Box wie in GFA
    ''' </summary>
    ''' <param name="iconNr">Icon Nummer (0 = Kein Symbol, 1 = Warnung, 2 = Fragezeichen, 3= Error)</param>
    ''' <param name="text">Text (mehrzeilig durch "|" getrennt)</param>
    ''' <param name="defButton">Ausgewählter Button (beginnend bei 1)</param>
    ''' <param name="buttons">Buttontexte durch "|" getrennt</param>
    ''' <param name="result">Nummer des gedrückten Buttons</param>
    Sub _ALERT(ByVal iconNr As Integer, ByVal text As String, ByVal defButton As Integer, ByVal buttons As String, ByRef result As Integer)
        AlertWindow.PlaySound(iconNr)
        Dim alert As AlertWindow = New AlertWindow(Nothing, iconNr, buttons, text, defButton)
        result = alert.Result
    End Sub
#End Region
#Region "GFABasic - Andere Befehle"
    ''' <summary>
    ''' Löscht den Inhalt der Variable
    ''' </summary>
    Public Sub _CLR(ByRef variable As Byte)
        variable = 0
    End Sub
    ''' <summary>
    ''' Löscht den Inhalt der Variable
    ''' </summary>
    Public Sub _CLR(ByRef variable As Short)
        variable = 0
    End Sub
    ''' <summary>
    ''' Löscht den Inhalt der Variable
    ''' </summary>
    Public Sub _CLR(ByRef variable As Integer)
        variable = 0
    End Sub
    ''' <summary>
    ''' Löscht den Inhalt der Variable
    ''' </summary>
    Public Sub _CLR(ByRef variable As Double)
        variable = 0.0R
    End Sub
    ''' <summary>
    ''' Löscht den Inhalt der Variable
    ''' </summary>
    Public Sub _CLR(ByRef variable As String)
        variable = ""
    End Sub
#End Region
#Region "GFABasic - Registervariablen"
    ''' <summary>
    ''' Pseudo Registervariable EAX
    ''' </summary>
    Public Property __EAX() As Integer
        Get
            Return GFAMisc.RegisterVariable(0)
        End Get
        Set(ByVal value As Integer)
            GFAMisc.RegisterVariable(0) = value
        End Set
    End Property
    ''' <summary>
    ''' Pseudo Registervariable EBX
    ''' </summary>
    Public Property __EBX() As Integer
        Get
            Return GFAMisc.RegisterVariable(1)
        End Get
        Set(ByVal value As Integer)
            GFAMisc.RegisterVariable(1) = value
        End Set
    End Property
    ''' <summary>
    ''' Pseudo Registervariable ECX
    ''' </summary>
    Public Property __ECX() As Integer
        Get
            Return GFAMisc.RegisterVariable(2)
        End Get
        Set(ByVal value As Integer)
            GFAMisc.RegisterVariable(2) = value
        End Set
    End Property
    ''' <summary>
    ''' Pseudo Registervariable EDX
    ''' </summary>
    Public Property __EDX() As Integer
        Get
            Return GFAMisc.RegisterVariable(3)
        End Get
        Set(ByVal value As Integer)
            GFAMisc.RegisterVariable(3) = value
        End Set
    End Property
#End Region
#Region "Zusatzbefehle, die es in GFA nicht gab"
    ''' <summary>
    ''' registriert eine eigene Windows Form Klasse für eine GFA Fensternummer. Das Fenster kann dadurch im Gegensatz zu _MAKEWIN beliebig oft geöffnet und geschlossen werden (Ist keine GFA-Funktion)
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="baseForm">Windows Form dessen Type verwendet wird</param>
    Public Sub _REGISTERWIN(ByVal ID As Integer, ByVal baseForm As Form) 'Neuer Befehl für VB.Net, gibt es im GFA16 nicht
        If baseForm IsNot Nothing Then
            GFAWindows.RegisterCustomForm(ID, baseForm.GetType)
        End If
    End Sub
    ''' <summary>
    ''' erstellt aus einem Windows Form ein GFA Fenster (Ist keine original GFA-Funktion)
    ''' </summary>
    ''' <param name="ID">Fensternummer</param>
    ''' <param name="frm">Windows Form</param>
    Public Sub _MAKEWIN(ByVal ID As Integer, ByVal frm As Form) 'Neuer Befehl für VB.Net, gibt es im GFA16 nicht
        If frm IsNot Nothing Then
            If frm.IsDisposed = False Then 'Nur, wenn nicht disposed!
                GFAWindows.AddGFAWindow(ID, frm)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Gibt die Höhe in Pixel für den angegebenen String zurück. (Kein original GFA-Basic Befehl)
    ''' </summary>
    ''' <param name="text">String</param>
    ''' <returns>Höhe in Pxiel</returns>
    Public Function _TXTHEIGHT(ByVal text As String) As Integer
        _TXTHEIGHT = 0
        Dim drawing As GFADrawing = GFADrawing.ActiveDrawing
        If drawing IsNot Nothing Then
            _TXTHEIGHT = drawing.GetTextHeight(text)
        End If
    End Function
    ''' <summary>
    ''' Gibt Windows-Verzeichnis zurück
    ''' </summary>
    Public Function _GetWindowsDirectory() As String
        Return Environment.GetEnvironmentVariable("windir")
    End Function
    ''' <summary>
    ''' Gibt true zurück, wenn die Anwendung als 64-Bit Anwendung läuft
    ''' </summary>
    Public Function _Is64BitApplication() As Boolean
        Dim ptr As IntPtr = IntPtr.Zero
        If Marshal.SizeOf(ptr) = 8 Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Ersatzfunktion für DIM?
    ''' </summary>
    Public Function _ARRAYSIZE(ByVal arr As Object)
        _ARRAYSIZE = 0
        If arr IsNot Nothing Then
            If arr.GetType().IsArray Then
                _ARRAYSIZE = arr.Length
            End If
        End If
    End Function
    ''' <summary>
    ''' Entfernt das angegebene Menü
    ''' </summary>
    Public Sub DestroyGFAMenu(ByVal hMenu As Integer)
        Dim wnd As GFAWindow
        hMenu -= 1024

        If hMenu >= 0 And hMenu <= 31 Then
            wnd = GFAWindows.GetWindow(hMenu)
            If wnd IsNot Nothing Then
                If wnd.IsForm = True Then
                    Dim ms As MainMenu = wnd.Form.Menu

                    If ms IsNot Nothing Then
                        'If ms.Disposing = False And ms.IsDisposed = False Then

                        'ZU TESTZWECKEN AUSKOMMENTIERT
                        'wnd.Form.Menu = Nothing

                        'ms.Visible = False
                        'ms.Dispose()
                        'End If
                    End If
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Gibt ein Pseudohandle für das Menü zurück  
    ''' </summary>
    Public Function GetGFAMenu(ByVal hWnd As IntPtr) As Integer
        Dim i As Integer = 0
        For i = 0 To 31
            Dim wnd As GFAWindow
            wnd = GFAWindows.GetWindow(i)
            If wnd IsNot Nothing Then
                If wnd.IsForm Then
                    If wnd.Form.Handle = hWnd Then
                        Return 1024 + i
                    End If
                End If
            End If
        Next
        Return 0
    End Function
    ''' <summary>
    ''' Erstellt ein Menü
    ''' </summary>
    ''' <param name="entries">Array mit Menüpunkten</param>
    Public Sub _MENU_MODERN(ByVal entries() As String)
        Dim wndMenu As MenuStrip
        Dim item As GFAToolStripMenuItem
        Dim i As Integer = 0
        Dim newMenu As Boolean = True
        Dim menuStrp As ToolStripMenuItem = New ToolStripMenuItem

        If GFAWindows.CurrentWindow IsNot Nothing Then
            If GFAWindows.CurrentWindow.Form IsNot Nothing Then
                'If GFAWindows.CurrentWindow.Menu Is Nothing Then ' Is Nothing!
                wndMenu = New MenuStrip
                wndMenu.Parent = GFAWindows.CurrentWindow.Form
                GFAWindows.CurrentWindow.Form.MainMenuStrip = wndMenu '2010-09-15 hinzugefügt
                For i = 0 To entries.Length - 1

                    If newMenu Then
                        If entries(i) <> "" Then
                            newMenu = False
                            menuStrp = New ToolStripMenuItem
                            menuStrp.Text = entries(i)
                        End If
                    Else
                        If entries(i) <> "" Then
                            If entries(i) = "-" Then
                                menuStrp.DropDownItems.Add(New ToolStripSeparator())
                            Else
                                item = New GFAToolStripMenuItem(GFAWindows.CurrentWindow.Form.Handle, GFAWindows.CurrentWindow.ID)
                                item.Text = entries(i)
                                item.Tag = i  'Index ist Position im array (Eintrag beginnt bei 1, Seperatoren zählen auch)
                                'items.Add(item)
                                menuStrp.DropDownItems.Add(item)
                            End If

                        Else
                            wndMenu.Items.Add(menuStrp)
                            newMenu = True
                        End If
                    End If
                Next
                wndMenu.Show()
                'End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Passt das Menu an. Für Flags wird jedoch nur MF_CHECKED unterstützt.
    ''' </summary>
    Public Sub _MENU_MODERN(ByVal id As Integer, ByVal flags As Integer, ByVal text As String)
        Dim done As Boolean = False
        Dim wnd As GFAWindow
        Dim menuStrip As MenuStrip
        Dim gfaItem As GFAToolStripMenuItem
        wnd = GFAWindows.CurrentWindow
        If wnd IsNot Nothing Then
            If wnd.IsForm Then
                menuStrip = wnd.Form.MainMenuStrip
                If menuStrip IsNot Nothing Then
                    For Each menuItem As ToolStripMenuItem In menuStrip.Items
                        If menuItem IsNot Nothing Then
                            For Each item As Object In menuItem.DropDownItems
                                If done = False Then
                                    If item IsNot Nothing Then
                                        'Type muss GFAToolStripMenuItem sein!
                                        If TypeOf item Is GFAToolStripMenuItem Then
                                            gfaItem = DirectCast(item, GFAToolStripMenuItem)
                                            If gfaItem.Tag = id Then
                                                gfaItem.Text = text
                                                '
                                                If flags And MF_CHECKED Then
                                                    item.Checked = True
                                                Else
                                                    item.Checked = False
                                                End If
                                                'Achtung: nicht gleiches Verhalten. Sollte vertikalen Balken erzeugen...
                                                'Menüitem wird in GFA auch mit dem Flag angezeigt
                                                'If flags And MF_MENUBARBREAK Then
                                                '    item.Visible = False
                                                'Else
                                                '    item.Visible = True
                                                'End If
                                                done = True
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If
    End Sub
#End Region
#Region "Alter auskommentierter Code"
    '''' <summary>
    '''' Erstellt ein Bitmap aus dem angegebenen Rechteck. Achtung! Die Prozedure ist sehr langsam
    '''' </summary>
    'Public Sub _GET(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef bmpString As String)
    'Dim hBmp As IntPtr = IntPtr.Zero
    'Dim bmpObj As Bitmap = Nothing
    'Dim bmpStream As IO.MemoryStream = Nothing
    'bmpString = ""
    '_GET(x, y, width, height, hBmp)
    'If hBmp <> IntPtr.Zero Then
    'Try
    'bmpStream = New IO.MemoryStream
    'bmpObj = Bitmap.FromHbitmap(hBmp)
    '
    '           bmpObj.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Png)
    '            bmpString = Convert.ToBase64String(bmpStream.GetBuffer())
    '         Catch
    '              'Im Fehlerfall nichts tun...
    '           End Try
    '
    '        DeleteObject(hBmp)
    '    End If
    'End Sub

    '''' <summary>
    '''' Erstellt ein Bitmap aus dem angegebenen Rechteck. Achtung! Die Prozedure ist sehr langsam
    '''' </summary>
    'Public Sub _GET2(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByRef bmpString As String)
    'Dim hBmp As IntPtr = IntPtr.Zero
    'Dim bmpObj As Bitmap = Nothing
    'Dim bmpStream As IO.MemoryStream = Nothing
    '    bmpString = ""
    '    _GET(x, y, width, height, hBmp)
    '    If hBmp <> IntPtr.Zero Then
    'Try
    '        bmpStream = New IO.MemoryStream
    '        bmpObj = Bitmap.FromHbitmap(hBmp)

    'Dim bmpData As System.Drawing.Imaging.BitmapData = bmpObj.LockBits(New Rectangle(0, 0, width, height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format24bppRgb)
    'Dim dest(bmpData.Stride * bmpData.Height + 8) As Byte '= New Byte()
    '        dest(0) = width
    '        dest(1) = height
    '        dest(2) = bmpData.Stride And 255
    '        dest(3) = (bmpData.Stride / 256) And 255
    '        Marshal.Copy(bmpData.Scan0, dest, 8, bmpData.Stride * bmpData.Height)

    'bmpObj.Save(bmpStream, System.Drawing.Imaging.ImageFormat.MemoryBmp)
    '        bmpString = Convert.ToBase64String(dest)
    'Catch
    'Im Fehlerfall nichts tun...
    'End Try

    '       DeleteObject(hBmp)
    '   End If
    'End Sub

    '''' <summary>
    '''' Stellt das Bitmap an der angegebenen Stelle dar. Achtung! Die Prozedure ist sehr langsam
    '''' </summary>
    'Public Sub _PUT(ByVal x As Integer, ByVal y As Integer, ByVal bmp As String)
    'Dim bmpStream As IO.MemoryStream = Nothing
    'Dim bmpObj As Bitmap = Nothing
    'Dim hBitmap As IntPtr = IntPtr.Zero
    '    Try
    '        bmpStream = New IO.MemoryStream(Convert.FromBase64String(bmp), False)
    '        bmpObj = Bitmap.FromStream(bmpStream)
    '        hBitmap = bmpObj.GetHbitmap()
    '    Catch
    'Im Fehlerfall nichts tun...
    '    End Try
    '    If hBitmap <> IntPtr.Zero Then
    '        _PUT(x, y, hBitmap)
    '        DeleteObject(hBitmap)
    '    End If
    'End Sub

    '''' <summary>
    '''' Stellt das Bitmap an der angegebenen Stelle dar. Achtung! Die Prozedure ist sehr langsam
    '''' </summary>
    'Public Sub _PUT2(ByVal x As Integer, ByVal y As Integer, ByVal bmp As String)
    'Dim bmpStream As IO.MemoryStream = Nothing
    'Dim bmpObj As Bitmap = Nothing
    'Dim hBitmap As IntPtr = IntPtr.Zero
    'Try
    'Dim bytes() As Byte = Convert.FromBase64String(bmp)
    'Dim mem As IntPtr = Marshal.AllocHGlobal(bytes.Length - 8)
    '    Marshal.Copy(bytes, 8, mem, bytes.Length - 8)
    '
    '
    '    bmpObj = New Bitmap(bytes(0), bytes(1), bytes(2) + (bytes(3) * 256), Imaging.PixelFormat.Format24bppRgb, mem)

    ''bmpObj = Bitmap.FromStream(bmpStream)
    '    hBitmap = bmpObj.GetHbitmap()
    ''Catch
    ''Im Fehlerfall nichts tun...
    ''End Try
    '    If hBitmap <> IntPtr.Zero Then
    '        _PUT(x, y, hBitmap)
    '        DeleteObject(hBitmap)
    '    End If
    'End Sub


    '''' <summary>
    '''' Stellt das Bitmap an der angegebenen Stelle dar. Achtung! Die Prozedure ist sehr langsam
    '''' </summary>
    'Public Sub _PUT2X(ByVal x As Integer, ByVal y As Integer, ByRef bytes() As Byte)
    'Dim bmpStream As IO.MemoryStream = Nothing
    'Dim bmpObj As Bitmap = Nothing
    'Dim hBitmap As IntPtr = IntPtr.Zero
    ''Try
    ''Dim bytes() As Byte = Convert.FromBase64String(bmp)
    'Dim mem As IntPtr = Marshal.AllocHGlobal(bytes.Length - 8)
    '    Marshal.Copy(bytes, 8, mem, bytes.Length - 8)


    '   bmpObj = New Bitmap(bytes(0), bytes(1), bytes(2) + (bytes(3) * 256), Imaging.PixelFormat.Format24bppRgb, mem)

    ''bmpObj = Bitmap.FromStream(bmpStream)
    '   hBitmap = bmpObj.GetHbitmap()
    ''Catch
    ''Im Fehlerfall nichts tun...
    ''End Try
    '  If hBitmap <> IntPtr.Zero Then
    '     _PUT(x, y, hBitmap)
    '    DeleteObject(hBitmap)
    ' End If
    'End Sub

    'Private Function GetStructureFromByteArray(ByRef Buffer() As Byte, ByVal offset As Integer, ByVal size As Integer, ByVal dataType As System.Type) As Object
    '    Dim tmpBuffer(size) As Byte
    '    Array.Copy(Buffer, offset, tmpBuffer, 0, size)
    '    Dim gcHandle As GCHandle = gcHandle.Alloc(Buffer, GCHandleType.Pinned)
    '    Dim Obj As Object = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject, dataType)
    'gcHandle.Free()
    '    Return Obj
    'End Function

    '''' <summary>
    '''' Gibt Text aus.
    '''' </summary>
    'Public Sub _PRINT(ByVal text As String)
    'Dim cWidth As Integer
    'Dim cHeight As Integer
    'Dim lastDC As IntPtr
    'Dim wnd As GFAWindow
    'Dim posX, posY, maxPosY As Integer
    '
    '    lastDC = GFADrawing.ActiveDC
    '    If lastDC <> IntPtr.Zero Then
    '        If GFAWindows.CurrentWindow IsNot Nothing Then
    '            If GFAWindows.CurrentWindow.IsForm Then
    '                wnd = GFAWindows.CurrentWindow
    '
    '                   GFADrawing.ActiveDC = wnd.DC ' Sicherstellen, das nur auf ein Fenster ausgegeben wird
    '                  cWidth = _TXTLEN("W")
    '                 cHeight = _TXTHEIGHT("|")
    '                If cHeight < 1 Then ' verhindert teilen durch 0
    '                   cHeight = 1
    '              End If
    '
    '               posY = cHeight * wnd.PrintPos.Y
    '              posX = cWidth * wnd.PrintPos.X
    '
    '               maxPosY = (wnd.Form.ClientSize.Height) / cHeight
    '              maxPosY -= 2
    '             maxPosY *= cHeight
    '            If maxPosY < 0 Then
    '               maxPosY = 0
    '          End If
    '
    '               wnd.PrintPos = New Point(wnd.PrintPos.X, wnd.PrintPos.Y + 1)
    '              If posY > maxPosY Then
    '                 ScrollWindowEx(wnd.Form.Handle, 0, -cHeight, Nothing, Nothing, IntPtr.Zero, Nothing, 0)
    '                posY = maxPosY
    '           End If
    '          _TEXT(posX, posY, text)
    '
    '               GFADrawing.ActiveDC = lastDC 'DC wieder zurücksetzen
    '          End If
    '     End If
    'End If
    'End Sub

    'Class GFADrawingOld

    '    Protected Shared m_DefFillMode As Integer = 0
    '    'Protected Shared m_OriginalBrush As IntPtr = IntPtr.Zero
    '    'Protected Shared m_OriginalPen As IntPtr = IntPtr.Zero
    '    'Protected Shared m_LastCreatedPen As IntPtr = IntPtr.Zero

    '    Protected Shared m_DC As IntPtr = IntPtr.Zero
    '    Protected Shared m_ForeColor As Integer = 0
    '    Protected Shared m_BackColor As Integer = &HFFFFFF

    '    Protected Shared m_PenWidth As Long = 0
    '    Protected Shared m_PenColor As Long = 0
    '    Protected Shared m_PenStyle As Long = 0
    '    Protected Shared m_MetaLastDC As IntPtr = IntPtr.Zero
    '    Protected Shared m_MetaDrawing As Boolean = False
    '    Protected Shared m_PatternBrushes(54) As IntPtr
    '    Protected Shared m_PatternBitmaps(54) As IntPtr

    '    Private Shared Sub PutBitmap(ByVal DC As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal bmpData() As Byte)
    '        'Ähnlich wie, der _PUT Befehl, nur mit byte-Array als Parameter
    '        Dim bmpBits() As Byte ' Bitmap bits
    '        Dim BmpInf As BITMAPINFO
    '        Dim BmpHeader As BITMAPFILEHEADER
    '        Dim bmpHeaderData() As Byte 'Beinhaltet nur den Header

    '        If DC <> IntPtr.Zero Then
    '            If bmpData IsNot Nothing Then
    '                If bmpData.Length > 0 Then
    '                    If DC <> IntPtr.Zero Then

    '                        ReDim bmpHeaderData(Marshal.SizeOf(BmpHeader) + Marshal.SizeOf(BmpInf))
    '                        Array.Copy(bmpData, bmpHeaderData, Marshal.SizeOf(BmpHeader) + Marshal.SizeOf(BmpInf))
    '                        BmpHeader = ___GetBITMAPFILEHEADER(bmpHeaderData)

    '                        If BmpHeader.bfType = 19778 Then 'BM
    '                            BmpInf = ___GetBITMAPINFO(bmpHeaderData, 14)

    '                            If BmpHeader.bfOffBits > 0 And BmpHeader.bfOffBits < bmpData.Length Then
    '                                'Alte methode mit unmanaged memory war wohl schneller...
    '                                ReDim bmpBits(bmpData.Length - BmpHeader.bfOffBits)
    '                                Array.Copy(bmpData, BmpHeader.bfOffBits, bmpBits, 0, bmpData.Length - BmpHeader.bfOffBits)
    '                                SetDIBitsToDevice(DC, x, y, BmpInf.bmiHeader.biWidth, BmpInf.bmiHeader.biHeight, 0, 0, 0, BmpInf.bmiHeader.biHeight, bmpBits, BmpInf, DIB_RGB_COLORS)
    '                            End If
    '                        End If
    '                    End If
    '                End If
    '            End If
    '        End If
    '    End Sub

    '    Public Shared Sub InitDC(ByVal DC As IntPtr)
    '        If DC <> IntPtr.Zero Then
    '            'SetTextColor(DC, RGB(255, 255, 255))
    '            'SetBkColor(DC, RGB(0, 0, 0))
    '            If GetDeffillMode(DC) = -1 Then
    '                SelectObject(DC, LoadBrush(0))
    '            End If
    '        End If
    '    End Sub

    '    Public Shared Function GetDeffillMode(ByVal DC As IntPtr)
    '        Dim gdiObject As IntPtr = GetCurrentObject(DC, OBJ_BRUSH)
    '        For idx = 0 To 54
    '            If m_PatternBrushes(idx) = gdiObject Then
    '                Return idx
    '            End If
    '        Next
    '        Return -1
    '    End Function

    '    Shared Function LoadBrush(ByVal idx As Integer) As IntPtr
    '        Dim deffillBmp As String = ""
    '        Dim mem() As Byte = Nothing
    '        Dim memDC As IntPtr = IntPtr.Zero
    '        Dim hPatternBmp As IntPtr = IntPtr.Zero
    '        Dim hOldBmp As IntPtr = IntPtr.Zero

    '        If idx < 0 Then idx = 0
    '        If idx > 54 Then idx = 54

    '        If m_PatternBrushes(idx) = IntPtr.Zero Then
    '            m_PatternBitmaps(idx) = IntPtr.Zero
    '            Select Case idx
    '                Case 37
    '                    m_PatternBrushes(idx) = CreateSolidBrush(RGB(255, 255, 255))
    '                Case 38
    '                    m_PatternBrushes(idx) = CreateSolidBrush(RGB(198, 198, 198))
    '                Case 39
    '                    m_PatternBrushes(idx) = CreateSolidBrush(RGB(132, 132, 132))
    '                Case 40
    '                    m_PatternBrushes(idx) = CreateSolidBrush(RGB(66, 66, 66))
    '                Case 41
    '                    m_PatternBrushes(idx) = CreateSolidBrush(RGB(0, 0, 0))
    '                Case Else
    '                    Try
    '                        deffillBmp = "Qk3+AQAAAAAAAD4AAAAoAAAAuAEAAAgAAAABAAEAAAAAAMABAAAlFgAAJRYAAAAAAAAAAAAAAAAAAP///wAAAAARVVVV3f//QAAEgMwA8VUQ7gED4RAiIswiEf//AsADAf/+AAAA/////+/+/e/8AAAAAAAAAAAiqqqq7v///xAgBQpAwADjAAAfxwEeOP8RZhURABEBYAMBAAAAAAD/////7/3+7/wAAAAAAAAAAAAARFVVVXf/EBACVQAYAcYRER8oAR58RIgziBEAEYAwhAEAAAAAAP/////v+3/vewAAAAAAAAAAiKqqqrv///8QKABABBsAbIIAExACHv6IRJlUEQARQBhIAQAAAAAA/////+/3v++3AAAAAAAAAAAAABFVVVXd//9EAEAIAwA+RBDuEAwe/xEizCIR//8gDDABAAAAAAD/////7+/f788AAAAAAAAAACKqqqru////AYJQoBAwAB8oAPF8MOH+/xFmUREAERAGMAEAAAAAAP///wDv3+8AzwAAAAAAAAAAAABEVVVVd/8BASBVALEIjRFV8YJI4XyIiDOIEQARCANIAQAAAAAA/////++/9++3AAAAAAAAAACIqqqqu////wGAAAQBjQDYAAAxAYThOEREmUURABEEgYQBAAAAAAD/////73/773sAAAAAAAAA"
    '                        hPatternBmp = CreateBitmap(8, 8, 1, 1, IntPtr.Zero) 'CreateBitmap(8, 8, 1, 1, IntPtr.Zero)
    '                        If hPatternBmp <> IntPtr.Zero Then
    '                            m_PatternBitmaps(idx) = hPatternBmp
    '                            memDC = CreateCompatibleDC(0)
    '                            hOldBmp = SelectObject(memDC, hPatternBmp)
    '                            PutBitmap(memDC, -8 * idx, 0, Convert.FromBase64String(deffillBmp))
    '                            m_PatternBrushes(idx) = CreatePatternBrush(hPatternBmp)
    '                        End If
    '                    Finally
    '                        If memDC <> IntPtr.Zero Then
    '                            SelectObject(memDC, hOldBmp)
    '                            DeleteDC(memDC)
    '                        End If
    '                        If hPatternBmp <> IntPtr.Zero And m_PatternBrushes(idx) = IntPtr.Zero Then
    '                            'CreatePatternBrush fehlgeschlagen -> bmp freigeben
    '                            DeleteObject(hPatternBmp)
    '                            m_PatternBitmaps(idx) = IntPtr.Zero
    '                        End If
    '                    End Try
    '            End Select
    '        End If
    '        Return m_PatternBrushes(idx)
    '    End Function


    '    Public Shared Sub FreeBrushes()
    '        For idx = 0 To 54
    '            'ACHTUNG GEFÄHRLICH, ES MUSS SICHERGESTELLT WERDEN, DAS DIE OBJKETE NICHT MEHR VERWENDET WERDEN
    '            If m_PatternBrushes(idx) <> IntPtr.Zero Then
    '                DeleteObject(m_PatternBrushes(idx))
    '                m_PatternBrushes(idx) = IntPtr.Zero
    '            End If
    '            If m_PatternBitmaps(idx) <> IntPtr.Zero Then
    '                DeleteObject(m_PatternBitmaps(idx))
    '                m_PatternBitmaps(idx) = IntPtr.Zero
    '            End If
    '        Next
    '    End Sub

    '    Public Shared Sub Deffill(ByVal mode As Integer)
    '        If mode < 0 Then mode = 0
    '        If mode > 54 Then mode = 54
    '        If m_DC <> IntPtr.Zero Then
    '            SelectObject(m_DC, LoadBrush(mode))
    '            'Select Case mode
    '            ' Case 37
    '            '     SelectObject(m_DC, GetStockObject(WHITE_BRUSH))
    '            ' Case 38
    '            '     SelectObject(m_DC, GetStockObject(LTGRAY_BRUSH))
    '            ' Case 39
    '            '     SelectObject(m_DC, m_PatternBrushes(GRAY_BRUSH))
    '            ' Case 40
    '            '     SelectObject(m_DC, m_PatternBrushes(DKGRAY_BRUSH))
    '            ' Case 41
    '            '     SelectObject(m_DC, m_PatternBrushes(BLACK_BRUSH))
    '            ' Case Else
    '            'SelectObject(m_DC, LoadBrush(mode))
    '            'End Select
    '        End If
    '    End Sub

    '    Private Shared Function GetBrush() As IntPtr
    '        Dim GDIBrush As IntPtr = IntPtr.Zero
    '        If m_DC <> IntPtr.Zero Then
    '            GDIBrush = GetCurrentObject(m_DC, OBJ_BRUSH)
    '            'GDIBrush = SelectObject(m_DC, GetStockObject(NULL_BRUSH))
    '            'SelectObject(m_DC, GDIBrush) ' Wieder zurücksetzen
    '        End If
    '        Return GDIBrush
    '    End Function

    '    Private Shared Function GetPen() As IntPtr
    '        Dim GDIPen As IntPtr = IntPtr.Zero
    '        If m_DC <> IntPtr.Zero Then
    '            GDIPen = GetCurrentObject(m_DC, OBJ_PEN)
    '            'GDIPen = SelectObject(m_DC, GetStockObject(NULL_PEN))
    '            'SelectObject(m_DC, GDIPen) ' Wieder zurücksetzen
    '        End If
    '        Return GDIPen
    '    End Function

    '    Public Shared Sub SetPen(ByVal color As Long, ByVal width As Long, ByVal style As Long)
    '        Dim newPen As IntPtr = IntPtr.Zero
    '        Dim lastPen As IntPtr = IntPtr.Zero
    '        If m_DC <> IntPtr.Zero Then
    '            If color <> m_PenColor Or width <> m_PenWidth Or m_PenStyle <> style Then
    '                m_PenColor = color
    '                m_PenStyle = style
    '                m_PenWidth = width
    '                DeleteObject(SelectObject(m_DC, CreatePen(style, width, color)))
    '                'newPen = CreatePen(style, width, color)
    '                'lastPen = SelectObject(m_DC, newPen)
    '                'If lastPen = m_LastCreatedPen And lastPen <> IntPtr.Zero Then
    '                'DeleteObject(lastPen)
    '                'End If
    '                'm_LastCreatedPen = newPen
    '            End If
    '        End If
    '    End Sub

    '    Public Shared Sub SetPen(ByVal color As Long)
    '        SetPen(color, m_PenWidth, m_PenStyle)
    '    End Sub

    '    'Wird nur für CREATEMETA, CLOSEMETA verwendet
    '    Public Shared Property MetaDrawing() As Boolean
    '        Get
    '            MetaDrawing = m_MetaDrawing
    '            Exit Property
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_MetaDrawing = value
    '            Exit Property
    '        End Set
    '    End Property

    '    'Wird nur für CREATEMETA, CLOSEMETA verwendet
    '    Public Shared Property MetaLastDC() As IntPtr
    '        Get
    '            MetaLastDC = m_MetaLastDC
    '            Exit Property
    '        End Get
    '        Set(ByVal value As IntPtr)
    '            m_MetaLastDC = value
    '            Exit Property
    '        End Set
    '    End Property

    '    Public Shared Property BackColor() As Integer
    '        Get
    '            BackColor = m_BackColor
    '            Exit Property
    '        End Get
    '        Set(ByVal value As Integer)
    '            m_BackColor = value
    '            Exit Property
    '        End Set
    '    End Property

    '    Public Shared Property ForeColor() As Integer
    '        Get
    '            ForeColor = m_ForeColor
    '            Exit Property
    '        End Get
    '        Set(ByVal value As Integer)
    '            m_ForeColor = value
    '            Exit Property
    '        End Set
    '    End Property

    '    Private Shared Sub GetDataFromActiveDC()
    '        Dim penData As LOGPEN
    '        'Dim brushData As LOGBRUSH

    '        If m_DC <> IntPtr.Zero Then
    '            'Farben den Device Context ermitteln
    '            m_ForeColor = GetTextColor(m_DC)
    '            m_BackColor = GetBkColor(m_DC)
    '            GDIGetObject(GetPen(), Marshal.SizeOf(penData), penData)
    '            'GDIGetObject(GetBrush(), Marshal.SizeOf(brushData), brushData)
    '            'm_OriginalBrush = GetPen()
    '            'm_OriginalPen = GetBrush()

    '            m_PenColor = penData.lopnColor
    '            m_PenStyle = penData.lopnStyle
    '            m_PenWidth = penData.lopnWidth.X
    '            m_DefFillMode = GetDeffillMode(m_DC)
    '        End If
    '    End Sub

    '    Public Shared Property ActiveDC()
    '        Get
    '            If m_DC = IntPtr.Zero Then
    '                If GFAWindows.CurrentWindow IsNot Nothing Then
    '                    m_DC = GFAWindows.CurrentWindow.DC 'DC erst dann ermitteln, wenn benötigt
    '                    GetDataFromActiveDC()
    '                End If
    '            End If
    '            ActiveDC = m_DC
    '            Exit Property
    '        End Get
    '        Set(ByVal value)
    '            If value <> IntPtr.Zero Then
    '                m_DC = value
    '                'Farben des Device Context ermitteln
    '                GetDataFromActiveDC()
    '            End If
    '            Exit Property
    '        End Set
    '    End Property

    '    Public Shared Sub ResetDC()
    '        m_DC = IntPtr.Zero
    '    End Sub

    'End Class


    '     ''' <summary>
    '     ''' Öffnet den angegebenen Dateikanal für die angegebene Datei mit dem angegebene Modus.
    '     ''' </summary>
    '     ''' <param name="modus">"o","i","a" oder "r"</param>
    '     ''' <param name="fileNb">Kanalnummer 0-99</param>
    '     ''' <param name="name">Dateiname</param>
    '     Public Sub _OPEN(ByVal modus As String, ByVal fileNb As Integer, ByVal name As String)
    '         modus = LCase(Trim(modus))
    '         Select Case modus
    '             Case "o"
    '                 FileOpen(fileNb, name, OpenMode.Output)
    '             Case "i"
    '                 FileOpen(fileNb, name, OpenMode.Input)
    '             Case "a"
    '                 FileOpen(fileNb, name, OpenMode.Append)
    '             Case "r"
    '                 FileOpen(fileNb, name, OpenMode.Random)
    '             Case Else
    '                 Throw New ArgumentException("unknown mode " + Chr(34) + modus + Chr(34) + " for OPEN")
    '                 'Exit Sub
    '         End Select
    '         openFileHandles(fileNb) = True
    '     End Sub
    ' 
    '     ''' <summary>
    '     ''' Schließt den angegebenen Dateikanal
    '     ''' </summary>
    '     Public Sub _CLOSE(ByVal fileNb As Integer)
    '         FileClose(fileNb)
    '         If fileNb < 100 Then
    '             openFileHandles(fileNb) = False
    '         End If
    '     End Sub
    ' 
    '     ''' <summary>
    '     ''' Schließt alle offenen Dateikanäle
    '     ''' </summary>
    '     Public Sub _CLOSE()
    '         Dim i As Integer
    '         For i = 0 To 99
    '             If openFileHandles(i) Then
    '                 openFileHandles(i) = False
    '                 FileClose(i)
    '             End If
    '         Next
    '     End Sub
    ' 
    '     Public Sub _LINE_INPUT(ByVal fileNb As Integer, ByRef str1 As String)
    '         str1 = LINEINPUT(fileNb)
    '     End Sub
    ' 
    '     Public Sub _LINE_INPUT(ByVal fileNb As Integer, ByRef str1 As String, ByRef str2 As String)
    '         str1 = LineInput(fileNb)
    '         str2 = LineInput(fileNb)
    '     End Sub
    ' 
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object)
    '         'Try
    ' 
    '         'MsgBox(5)
    '         'Try
    '         Input(fileNb, obj1)
    '         'If obj1 Is Nothing Then
    '         'Input(fileNb, obj1)
    '         'End If
    '         'Catch ex As Exception
    '         'obj1 = Nothing
    '         'End Try
    '         'Bei string Leerstring  anstelle von Nothing
    '         'If TypeOf obj1 Is String Then 'And obj1 Is Nothing Then
    ' 
    '         'End If
    '         'Catch ex As Exception
    '         'End Try
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '         _INPUT(fileNb, obj10)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '         _INPUT(fileNb, obj10)
    '         _INPUT(fileNb, obj11)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '         _INPUT(fileNb, obj10)
    '         _INPUT(fileNb, obj11)
    '         _INPUT(fileNb, obj12)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object, ByRef obj13 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '         _INPUT(fileNb, obj10)
    '         _INPUT(fileNb, obj11)
    '         _INPUT(fileNb, obj12)
    '         _INPUT(fileNb, obj13)
    '     End Sub
    ' 
    '     Public Sub _INPUT(ByVal fileNb As Integer, ByRef obj1 As Object, ByRef obj2 As Object, ByRef obj3 As Object, ByRef obj4 As Object, ByRef obj5 As Object, ByRef obj6 As Object, ByRef obj7 As Object, ByRef obj8 As Object, ByRef obj9 As Object, ByRef obj10 As Object, ByRef obj11 As Object, ByRef obj12 As Object, ByRef obj13 As Object, ByRef obj14 As Object)
    '         _INPUT(fileNb, obj1)
    '         _INPUT(fileNb, obj2)
    '         _INPUT(fileNb, obj3)
    '         _INPUT(fileNb, obj4)
    '         _INPUT(fileNb, obj5)
    '         _INPUT(fileNb, obj6)
    '         _INPUT(fileNb, obj7)
    '         _INPUT(fileNb, obj8)
    '         _INPUT(fileNb, obj9)
    '         _INPUT(fileNb, obj10)
    '         _INPUT(fileNb, obj11)
    '         _INPUT(fileNb, obj12)
    '         _INPUT(fileNb, obj13)
    '         _INPUT(fileNb, obj14)
    '     End Sub

    'Me.AutoScroll = True
    'Me.AutoScrollMinSize = New Size(1000, 1000)
    'Me.AutoScroll = False
    'Me.HorizontalScroll.Visible = True
    'Me.HorizontalScroll.Enabled = True
    'Me.HorizontalScroll.Minimum = 0
    'Me.HorizontalScroll.Maximum = 1000

    'Me.VerticalScroll.Visible = True
    'Me.VerticalScroll.Enabled = True
    'Me.VerticalScroll.Minimum = 0
    'Me.VerticalScroll.Maximum = 1000

#End Region
#Region "Unfertige Befehle"
    ''' <summary>
    ''' Öffnet einen Dialog zum auswählen einer Schriftart
    ''' </summary>
    ''' <param name="hwnd">Wird irnoriert</param>
    ''' <param name="DC">Wird irnoriert</param>
    ''' <param name="flags">Alles bis auf CF_EFFECTS wird ignoriert</param>
    Public Sub _DLG_FONT(ByVal hwnd As IntPtr, ByVal DC As IntPtr, ByVal flags As Integer)
        Dim dlg As Windows.Forms.FontDialog = New Windows.Forms.FontDialog
        dlg.Color = Color.Black 'Color.FromArgb(GFAMisc.SwapRedAndBlue(__ECX And &HFFFFFF)) ' Wahrscheinlich nicht gleiches verhalten wie in GFA...
        If flags And CF_EFFECTS Then
            dlg.ShowEffects = True
            dlg.ShowColor = True
        Else
            dlg.ShowEffects = False
            dlg.ShowColor = False
            'dlg.Font = System.Drawing.Font.FromHfont(GFADrawing.ActiveDrawing)
        End If
        dlg.ShowDialog()
        __ECX = GFAMisc.SwapRedAndBlue(dlg.Color.ToArgb() And &HFFFFFF)
        GFAMisc.SelectedFont = dlg.Font
    End Sub
    Public Sub _RFONT_NAME(ByRef name As Object)
        name = ""
        If GFAMisc.SelectedFont IsNot Nothing Then
            name = GFAMisc.SelectedFont.FontFamily.Name 'OriginalFontName ist leerstring
        End If
    End Sub
    Public Sub _RFONT_WEIGHT(ByRef weight As Object)
        'Wird nicht unterstützt
        weight = 0
    End Sub
    Public Sub _RFONT_WIDTH(ByRef width As Object)
        'Wird nicht unterstützt
        width = 0
    End Sub
    Public Sub _RFONT_HEIGHT(ByRef height As Object)
        If GFAMisc.SelectedFont IsNot Nothing Then
            height = GFAMisc.SelectedFont.Height
        End If
    End Sub
    Public Sub _RFONT_ITALIC(ByRef italic As Object)
        italic = False
        If GFAMisc.SelectedFont IsNot Nothing Then
            italic = GFAMisc.SelectedFont.Italic
        End If
    End Sub
    Public Sub _RFONT_UNDERLINE(ByRef underline As Object)
        underline = False
        If GFAMisc.SelectedFont IsNot Nothing Then
            underline = GFAMisc.SelectedFont.Underline
        End If
    End Sub
    Public Sub _RFONT_ESCPAEMENT(ByRef escapement As Object)
        'Wird nicht unterstützt
        escapement = 0
    End Sub
    Public Sub _RFONT_FAMILY(ByRef family As Object)
        'Wird nicht unterstützt
        family = 0
    End Sub
    Public Sub _RFONT_CHARSET(ByRef charset As Object)
        charset = 0
        If GFAMisc.SelectedFont IsNot Nothing Then
            charset = GFAMisc.SelectedFont.GdiCharSet
        End If
    End Sub
    Public Sub _RFONT_PITCH(ByRef pitch As Object)
        'Wird nicht unterstützt
        pitch = 0
    End Sub
#End Region
End Module