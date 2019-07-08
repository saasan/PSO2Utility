# PSO2Utility

ファンタシースターオンライン2用ユーティリティ。
主な機能は以下の通り。

1. UACを経由せずにPSO2を起動
1. PSO2のウィンドウに最小化ボタンを表示
1. PSO2のウィンドウ位置を保存、自動復元

## ダウンロード

[PSO2Utility-v1.0.zip](https://github.com/saasan/PSO2Utility/releases/latest/download/PSO2Utility-v1.0.zip)

## 動作環境

Windows 10 Pro (64bit) で動作を確認してます。
起動しない場合は
[.NET Framework 4 Client Profile](http://www.microsoft.com/ja-jp/net/netfx4/download.aspx)
を入れてみて下さい。

## 使い方

PSO2Utility.exeを起動するとアイコンがタスクトレイに表示されます。
アイコンを右クリックするとメニューが表示されて設定等ができます。
オプションでPSO2のフォルダを設定した後は、
アイコンのダブルクリックでPSO2が起動できます。

## 設定ファイル

設定ファイルは以下の場所に保存されます。

- Windows XP : %UserProfile%\Application Data\s2works\PSO2Utility
- Windows Vista/7/8/10 : %UserProfile%\AppData\Roaming\s2works\PSO2Utility

## 既知の不具合と対策

### PSO2 のウィンドウを最小化後、元に戻らなくなる場合がある

タスクトレイにある PSO2Utility のアイコンを右クリックし、「ウィンドウの位置を復元」を選択して下さい。
