# PopWithOAuth2Wpf

## 概要

本リポジトリは Exchange (Office365/outlook・Hotmail 系)・Gmail(Google Workspace/gmail) の 先進認証(OAuth2) によるメールサーバの認証設定手順・WPF アプリケーションでの実装例を示します。

なお、それぞれの設定方法は 2022-05-26 時点のものです。  
AzureAD・Google Developer Console の仕様・UI が執筆時と変わっている場合はご注意ください。

## プラットフォーム・使用ライブラリ

- .NET Core 3.1 (WPF)
- MailKit/MimeKit 3.2.0
- Google.Apis.Auth 1.57.0
- Microsoft.Identity.Client 4.44.0

## 設定手順・解説

- [Exchange](./Exchange.md)
- [Gmail](./Gmail.md)
