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

## デプロイされる構成

このプロジェクトは、以下の流れでAzureへデプロイされます。

1. Azure上に Resource Group、Azure Container Registry (ACR)、Azure Kubernetes Service (AKS) を作成する
2. .NET アプリケーションを Docker コンテナ化する
3. コンテナイメージを ACR に push する
4. AKS に Kubernetes マニフェストを適用し、フロントエンドとバックエンドを展開する
5. Azure Load Balancer 経由で Web アプリへアクセスできるようにする

実際の構成イメージは次のとおりです。

- ユーザー: Web ブラウザからアプリへアクセス
- Frontend: Blazor WebAssembly
- Backend: ASP.NET Core Web API
- Container Registry: ACR
- Orchestrator: AKS
- Data Stores: Azure SQL Database / Azure Cosmos DB
- Monitoring: Azure Monitor / Application Insights

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

## このリポジトリのデプロイ手順

このリポジトリは、ローカル環境から Azure 上の AKS にアプリをデプロイする流れで構成されています。初回デプロイと、既存デプロイの更新では手順が少し異なります。

### 1. 前提条件

以下のツールが利用可能であることを確認します。

```bash
az version
azd version
docker --version
kubectl version --client
```

### 2. 変数を設定する

```bash
RESOURCE_GROUP=rg-azure-shop-k8s
LOCATION=japaneast
ACR_NAME=acrazureshopk8s2833
AKS_NAME=aks-azure-shop-k8s
```

### 3. Azure にログインする

```bash
az login
az account set --subscription <SUBSCRIPTION_ID>
```

### 4. 初回デプロイ: Azure リソースを作成する

```bash
az group create --name $RESOURCE_GROUP --location $LOCATION
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $AKS_NAME \
  --node-count 1 \
  --enable-managed-identity \
  --attach-acr $ACR_NAME \
  --generate-ssh-keys
```

### 5. AKS に接続する

```bash
az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_NAME
kubectl get nodes
```

### 6. コンテナイメージを作成して ACR に push する

```bash
az acr login --name $ACR_NAME

docker build -t $ACR_NAME.azurecr.io/azure-shop-api:latest -f Dockerfile.api .
docker push $ACR_NAME.azurecr.io/azure-shop-api:latest

docker build -t $ACR_NAME.azurecr.io/azure-shop-web:latest -f Dockerfile.web .
docker push $ACR_NAME.azurecr.io/azure-shop-web:latest
```

### 7. Kubernetes にデプロイする

```bash
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/web-deployment.yaml
```

### 8. デプロイ結果を確認する

```bash
kubectl get pods
kubectl get svc
```

Web アプリの外部 IP が割り当てられたら、以下でアクセス確認できます。

```bash
kubectl get svc azure-shop-web
curl http://<EXTERNAL_IP>
```

API も確認する場合は、次を実行します。

```bash
kubectl port-forward svc/azure-shop-api 8081:80
curl http://127.0.0.1:8081/health
curl http://127.0.0.1:8081/api/products
```

### 9. 既存デプロイの更新時

コードを変更したあとに再デプロイする場合は、イメージを再ビルドして Kubernetes に反映します。

```bash
az acr login --name $ACR_NAME

docker build -t $ACR_NAME.azurecr.io/azure-shop-api:latest -f Dockerfile.api .
docker push $ACR_NAME.azurecr.io/azure-shop-api:latest

docker build -t $ACR_NAME.azurecr.io/azure-shop-web:latest -f Dockerfile.web .
docker push $ACR_NAME.azurecr.io/azure-shop-web:latest

kubectl set image deployment/azure-shop-api azure-shop-api=$ACR_NAME.azurecr.io/azure-shop-api:latest
kubectl set image deployment/azure-shop-web web=$ACR_NAME.azurecr.io/azure-shop-web:latest
kubectl rollout status deployment/azure-shop-api
kubectl rollout status deployment/azure-shop-web
```

## ローカル開発で Aspire から起動する

Aspire を使うと、API と Web を1つの起動コマンドでまとめて起動できます。

```bash
dotnet run --project src/AzureShop.AppHost/AzureShop.AppHost.csproj
```

起動後、Aspire ダッシュボードで各サービスの状態を確認できます。Web アプリは通常、AppHost から表示される URL から開けます。

## wwwroot の復元手順

Blazor WebAssembly の静的ファイルは、ビルド時に再生成されるため、必要に応じて以下のコマンドで復元できます。

```bash
dotnet publish src/AzureShop.Web/AzureShop.Web.csproj -c Release
```

公開用の静的ファイルは `src/AzureShop.Web/wwwroot` 配下に再生成されるため、アプリの画面表示に必要な資産が揃います。

ローカルで実行する場合は、次のコマンドでも確認できます。

```bash
dotnet run --project src/AzureShop.Web/AzureShop.Web.csproj
```

## kubectl のコンテキストを戻す方法

AKS に接続した状態で `kubectl get pods` を実行すると、Azure の AKS クラスターを見に行きます。ローカルの Kubernetes に戻したい場合は、コンテキストを切り替えます。

```bash
kubectl config get-contexts
kubectl config use-context rancher-desktop
```

`rancher-desktop` ではなく別名のコンテキストを使っている場合は、その名前に置き換えて実行してください。

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
