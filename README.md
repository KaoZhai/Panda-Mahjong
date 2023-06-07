# README

## file organization

由於我們使用Unity開發專案，並使用github管理版本，因此會有一些自動生成的資料夾，於下方列出：

#### github相關

1. .git：包含了版本控制所需的元資料，如提交紀錄、分支資訊、遠端儲存庫的連結等。
2. hooks： Git 儲存庫中的一個特殊資料夾，用於存放 Git 鉤子（hooks）。鉤子是 Git 的一種功能，它允許您在特定的 Git 操作事件發生時執行自定義的腳本或命令。
3. lfs：Git LFS（Large File Storage）的相關資料夾。Git LFS 是 Git 的一個擴展，用於處理大型檔案的版本控制。
4. .gitatrributes：紀錄那些檔案需要使用LFS。
5. .gitignore：紀錄那些檔案不須加入版本管理。

#### Unity相關

Packages：裡面存放管理Unity package的json檔，package的內容則放在Assets中。

ProjectSettings：包含了Unity專案的設定和配置檔案。這些檔案包括專案的屬性、選項、編輯器的配置和設定、專案的導入設定、平台選項等

#### Aseets

Unity專案中用於存放所有專案資源（如模型、紋理、腳本、音效等）的主要目錄。

Prefab：存放Unity Prefab的資料夾，在Unity中，Prefab（預製）是一種可重複使用的物件或遊戲物件的範本。它可以被看作是一個物件的藍圖，包含了該物件的所有組件、屬性和設置。Prefab可以被實例化為具體的遊戲物件，並在場景中使用。

Resources：存放開發中用到的資源，其中圖片放在底下的Image，音樂放在底下的Music。

Scenes：存放Unity中的場景，在Unity中，Scene（場景）是遊戲或應用程序的虛擬世界或特定場景的可視化表示。它是Unity編輯器中用於設計和組織遊戲場景的主要概念。

Script：存放程式碼。

TextMesh Pro：Unity相關的package。

## installation setup

要使用我們的專案，可在Andriod手機上下載並安裝我們的apk，完成後即可使用，或是到https://majong.aokblast.me/使用WebGL版本。

如果想自行打包專案，需下載Unity 2021.3.21f1版本，並於Unity Hub中對此Unity版本安裝發布平台的套件，開啟專案後，點選File/Build Settings後，選擇想要的平台進行打包。