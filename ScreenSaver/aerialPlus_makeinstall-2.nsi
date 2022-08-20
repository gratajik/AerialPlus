; NSIS Script file to Install AerialPlus
; https://en.wikipedia.org/wiki/Nullsoft_Scriptable_Install_System
;

; Script generated by the HM NIS Edit Script Wizard.
SetCompressor /FINAL /SOLID lzma

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "AerialPlus"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "Ratajik Software"
!define PRODUCT_WEB_SITE "https://www.ratajik.com/AerialPlus"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\AerialPlus.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"

; MUI 1.67 compatible ------
!include "..\..\nsis\include\mui.nsh"
!include WinVer.nsh

; MUI Settings
!define MUI_ABORTWARNING
;!define MUI_WELCOMEFINISHPAGE_BITMAP welcome.bmp
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page

; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN_PARAMETERS "/c"
!define MUI_FINISHPAGE_RUN "$INSTDIR\AerialPlus.exe"
!Define MUI_FINISHPAGE_RUN_TEXT 'Open AerialPlus settings'
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "AerialPlusInst.exe"

InstallDir "$PROGRAMFILES\Ratajik Software\AerialPlus"
InstallDirRegKey HKCU "${PRODUCT_DIR_REGKEY}" ""

ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01  
  SetShellVarContext all
  ;StrCpy $INSTDIR "$PROGRAMFILES\Ratajik Software\AerialPlus"
    
  SetOutPath "$INSTDIR"
  ; Let the user run as a normal app     
  
  ; Files for normal install
  File  bin\Release\AerialPlus.scr
  File  bin\Release\AerialPlus.exe
  File  bin\Release\AerialPlus_Version.json
  File  bin\Release\Newtonsoft.Json.dll
  File  bin\Release\Newtonsoft.Json.xml
  
  CreateShortCut "$SMPROGRAMS\StationRipper\AerialPlus.lnk" "$INSTDIR\AerialPlus.exe"
  CreateShortCut "$DESKTOP\AerialPlus.lnk" "$INSTDIR\AerialPlus.exe"

  SetOutPath "$WINDIR"

  File  bin\Release\AerialPlus.scr  
  File  bin\Release\AerialPlus_Version.json
  File  bin\Release\Newtonsoft.Json.dll
  File  bin\Release\Newtonsoft.Json.xml

  ; Set screensaver and make it active
  ${If} ${AtMostWinXP}
    WriteINIStr "$WINDIR\system.ini" "boot" "SCRNSAVE.EXE" "$WINDIR\AerialPlus.scr"
  ${Else}
    WriteRegStr HKCU "Control Panel\desktop" "SCRNSAVE.EXE" "$WINDIR\AerialPlus.scr"
    WriteRegStr HKCU "Control Panel\desktop" "ScreenSaveActive" "1"
  ${EndIf}

  ; Notify system of the change
  System::Call 'user32.dll::SystemParametersInfo(17, 1, 0, 2)'             

  ;MessageBox MB_OK "$INSTDIR"
SectionEnd

Section -Post
SetShellVarContext all
  WriteUninstaller "$INSTDIR\uninstall-AerialPlus.exe"  

  WriteRegStr HKCU "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\AerialPlus.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninstall-AerialPlus.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\AerialPlus.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"  
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components? " IDYES +2
  Abort
FunctionEnd

Section Uninstall  
  SetShellVarContext all

  ; Clean up the normal app
  Delete "$INSTDIR\uninstall-AerialPlus.exe"
  Delete "$INSTDIR\AerialPlus.exe"
  Delete "$INSTDIR\AerialPlus.scr"
  Delete "$INSTDIR\AerialPlus_Version.json"
 
  ; Clean up the special screen saver bits  
  Delete "$WINDIR\AerialPlus.scr"    
  Delete "$WINDIR\AerialPlus_Version.json"    

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKCU "${PRODUCT_DIR_REGKEY}"
    
  DeleteRegKey HKCU 'SOFTWARE\Ratajik Software\AerialPlus'  

  Delete "$DESKTOP\AerialPlus.lnk"  
  Delete "$SMPROGRAMS\AerialPlus\AerialPlus.lnk"

  RMDir "$SMPROGRAMS\AerialPlus"

  SetAutoClose true
SectionEnd