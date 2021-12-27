# bulk-tweet

## Twitter API を利用できるようにする

こういうサイトがとても参考になります。
https://www.itti.jp/web-direction/how-to-apply-for-twitter-api/

## Twitter API キーの情報を設定する

### ローカルの開発環境の場合

コマンドプロンプトから下記の命令を実行します。
????? には Twitter Apps で作った App のキーを入力します

```
dotnet user-secrets set "Twitter:ConsumerApiKey" "?????"
dotnet user-secrets set "Twitter:ConsumerApiSecret" "?????"
```

### Azure App Service の場合

設定 -> 構成 -> アプリケーション設定に下記のパラメータを追加します

```
名前：Twitter:ConsumerApiKey 値：?????
名前：Twitter:ConsumerApiSecret 値：?????
```

## Azure App Service の Windows を使う場合

IIS になるので環境を変える場合は、 Web.config を変更します。
また、 UseKestrel を使うとエラーになるで外すこと。
