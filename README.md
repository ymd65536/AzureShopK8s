# Azure Template Repo README

## Kubernetesでショッピングサイトを構築する

このプロジェクトでは、Kubernetes上で動作するショッピングサイトをAzure環境に構築します。フロントエンドはBlazor WebAssembly、バックエンドはASP.NET Core Web APIで実装し、Azure Kubernetes Service (AKS) 上でホストします。コンテナイメージはAzure Container Registry (ACR) で管理し、注文データはAzure SQL Database、商品データはAzure Cosmos DB で保持します。運用監視にはAzure Monitor、Application Insights、Azure Automation を利用します。

## 技術スタック

- Azure Developer CLI (azd)
  - Azureリソースの作成・管理・デプロイに使用
- Azure CLI (az)
  - BicepのビルドやAzureリソースの管理に使用
- .NET / C#
  - Blazor WebAssembly（フロントエンド）
  - ASP.NET Core Web API（バックエンド）
- Docker
  - コンテナイメージ作成と配布に使用
- Aspire(.NET Aspire)
  - .NETアプリケーションの構築とデプロイに使用

## 利用するAzureサービス

- Azure Kubernetes Service (AKS)
  - フロントエンドとバックエンドをコンテナとして実行
- Azure Container Registry (ACR)
  - コンテナイメージの保存と配布
- Azure SQL Database
  - 注文データの永続化
- Azure Cosmos DB
  - 商品データの管理
- Azure Monitor / Application Insights
  - アプリケーションの性能とエラーの監視
- Azure Automation
  - 定期タスクの自動化

## 機能要件

- ユーザーは商品を閲覧し、カートに追加して購入できる
- 管理者は商品を管理し、注文を処理できる
- 高可用性とスケーラビリティを考慮した構成とする
- 監視・運用のためのログとメトリクスを収集する
- 注文時はAzure SQL Databaseに注文データを保存し、商品データはAzure Cosmos DBに保存する
- 注文確定前に確認モーダル画面を表示する

## システム構成のイメージ

- フロントエンド: Blazor WebAssembly
- バックエンド: ASP.NET Core Web API
- データストア: Azure SQL Database / Azure Cosmos DB
- オーケストレーション: AKS
- イメージ管理: ACR
- 監視: Azure Monitor / Application Insights

## Azure Developer CLIのセットアップ

以下のコマンドを実行して、Azure Developer CLI (azd) をインストールします。

```bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

インストール方法は[公式ドキュメント](https://learn.microsoft.com/ja-jp/azure/developer/azure-developer-cli/install-azd)を参照してください。

### Azure Developer CLIの動作確認

以下のコマンドでAzure Developer CLIのバージョンを確認します。

```bash
azd version
```

### Azure Developer CLIでログインする

環境変数 `AZURE_TENANT_ID`が設定されている場合は、以下のコマンドでログインします。

```bash
azd auth login --tenant-id $AZURE_TENANT_ID
```

環境変数が設定されていない場合は、以下のコマンドでログインします。

```bash
azd auth login
```

## Azure CLIをセットアップする

以下のコマンドを実行して、Azure CLIをインストールします。

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

インストール方法は[公式ドキュメント](https://learn.microsoft.com/ja-jp/cli/azure/install-azure-cli-linux?pivots=apt)を参照してください。

## Azure CLIでログインする

環境変数 `AZURE_TENANT_ID`が設定されている場合は、以下のコマンドでログインします。

```bash
az login --tenant $AZURE_TENANT_ID
```

### Azure CLIの動作確認

以下のコマンドでAzure CLIのバージョンとアカウント情報を確認します。

```bash
az version
az account list
```

## GitHub Codespacesの設定

`.env`でシークレットを管理する場合、以下のコマンドでCodespacesにシークレットを設定します。

```bash
gh secret set --app codespaces -f .env
```

シークレットの一覧を確認するには、以下のコマンドを実行します。

```bash
gh secret list --app codespaces
```

単一のシークレットを設定するには、以下のコマンドを使用します。

```bash
gh secret set --app codespaces SECRET_NAME
```

シークレットの削除は以下のコマンドで行います。

```bash
gh secret delete --app codespaces SECRET_NAME
```
