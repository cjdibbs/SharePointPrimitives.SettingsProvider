Wspbuilder.exe

stsadm -o retractsolution -name SharePointPrimitives.SettingsProvider.wsp -local
stsadm -o execadmsvcjobs
stsadm -o deletesolution -name  SharePointPrimitives.SettingsProvider.wsp

stsadm -o addsolution -filename SharePointPrimitives.SettingsProvider.wsp
stsadm -o deploysolution -name sharepointprimitives.settingsprovider.wsp -local -allowgacdeployment
stsadm -o execadmsvcjobs