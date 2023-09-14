# VisualMOT

![Blue and yellow Visual MOT Icon](VisualMOT.Android/Resources/drawable/icon.png)

This is a Xamarin Forms (iOS and Android) app that uses the DVSA MOT History API. It retrives an MOT test and presents the user with options to add photos and comments which will then create a report incorporating these. It then allows the user to send the report via email or upload via FTP to a website and send a link in an SMS message.

All the configuration e.g. API keys is under `VisualMOT/Constants.cs`

To get the basic functionality in the app working, sign up for a [DVSA API Key](https://dvsa.github.io/mot-history-api-documentation/) and enter that into the `Constants.cs` file. To send a report you will need to also enter the credentials for an SMTP email account (see all constants starting 'Mail...').  

For more information please see:
https://ydrive.substack.com/p/visual-mot-an-app-for-mot-testers

Check `LICENCE.MD` for licencing info.
