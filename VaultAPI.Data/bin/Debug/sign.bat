del .\Signed\VaultAPI.Data.* /F
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\ildasm.exe" VaultAPI.Data.dll /out:.\Signed\VaultAPI.Data.dll
"C:\Windows\Microsoft.NET\Framework\v2.0.50727\ilasm.exe" .\Signed\VaultAPI.Data.il /dll /key=.\vaultDataKey.snk /output=.\Signed\VaultAPI.Data.dll

pause