Imports GFAWIN_Samples.GFAEmulation
Module Sample1
	
	'================================================
'GLOBAL VARIABLES
'================================================
Public x As Integer                               ' Org: x%
Public y As Integer                               ' Org: y%
Public a_ As Integer                              ' Org: a%
Public area As Double                             ' Org: area
Public ch As Double                               ' Org: ch
Public INPUT As Double                            ' Org: INPUT
Public A$                                         ' Org: A$
'================================================
'GLOBAL ARRAYS
'================================================
'================================================
'ENDE DER DEKLARATION
'================================================


Public Sub Main()




'>>=  OPENW #1
_OPENW( 1  )
'>>=  TITLEW #1, "test"
_TITLEW( 1 , "test"  )
'>>=  For x%=0 to 20
For x = 0 To 20 
'>>=    For y%=0 to 20
For y = 0 To 20 
'>>=      DEFFILL x%+y%
_DEFFILL( x + y  )
'>>=  	COLOR x%
	_COLOR (x) 
'>>=  	PBOX x%*20, y%*20, (x%+1)*20, (y%+1)*20
	_PBOX (x * 20 , y * 20 , ( x + 1 ) * 20 , ( y + 1 ) * 20)
'>>=    Next
Next 
'>>=  Next 
Next 
'>>=  

'>>=  Print "Press A key"
_PRINTTEXT( "Press A key"  )
'>>=  KEYGET a%
_KEYGET( a_  )
'>>=  

'>>=  area = 40.5 * 35.5
area = 40.5 * 35.5 
'>>=  ALERT 1,"Area A=" + STR$(area,7,2),2,"Yes|No|Abort",ch
_ALERT( 1 , "Area A=" + _STR ( area , 7 , 2 ) , 2 , "Yes|No|Abort" , ch  )
'>>=  PRINT ch
_PRINTTEXT( ch  )
'>>=  Print "--------------------------------------------------------------------------------"
_PRINTTEXT( "--------------------------------------------------------------------------------"  )
'>>=  PRINT "Enter your name:"
_PRINTTEXT( "Enter your name:"  )
'>>=  INPUT A$
_INPUTTEXT (A$) 
'>>=  PRINT "Your name:" + A$
_PRINTTEXT( "Your name:" + A$  )
'>>=  Print "Press A key"
_PRINTTEXT( "Press A key"  )
'>>=  KEYGET a%
_KEYGET( a_  )
End Sub


End Module