<b>任務描述：</b>ASP .NET 8.0 建置一個 Web API 專案。<br><br>
<b>資料庫:</b> SQL Server Express LocalDB（Entity Framework Core）<br><br>
<b>實作功能簡述:</b> <br>
      1.呼叫 coindesk API，解析其內容並進行資料轉換，並實作新的 API。<br>
      2.coindesk API：https://api.coindesk.com/v1/bpi/currentprice.json<br>
      3.建立一張幣別與其對應中文名稱的資料表<br>
      4.幣別 DB 維護功能, 提供查詢/新增/修改/刪除功能的 API End Point<br>
      5.幣別相關資訊（幣別，幣別中文名稱，以及匯率）及依幣別代碼排序<br>
      6.實作多語系設計<br>
      7.實作加解密技術應用(AES256)<br>
      8.API End Point 單元測試<br>
<br><br>
<b>實作內容：</b></b><br>
<b>1.資料庫設計:</b></b><br>
<img width="296" alt="image" src="https://github.com/user-attachments/assets/82dacbae-6b5d-4f86-8270-cfb275bfa4ff">
<img width="840" alt="image" src="https://github.com/user-attachments/assets/3a605f88-c9d4-4f54-ae4d-91abecabdd9e">
<img width="634" alt="image" src="https://github.com/user-attachments/assets/3dd6227a-cb9e-4d1a-b5c0-84e024e9c609">
<br><br>
(1).Add TimeInfo<br>
CREATE TABLE [dbo].[TimeInfo](
[Id] [Uniqueidentifier] NOT NULL,
[Updated] [nvarchar](30) NOT NULL,
[UpdatedISO] [nvarchar](30) NOT NULL,
[UpdatedUK] [nvarchar](30) NOT NULL,
[UpdatedAt] [datetime2] NOT NULL,
PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
<br><br>
(2).Add Bpi<br>
CREATE TABLE [dbo].[Bpi](
[Id] [int] IDENTITY(1,1) NOT NULL,
[TimeInfoId] [Uniqueidentifier] NOT NULL,
[CurrencyCode] [varchar](10) NOT NULL,
[Symbol] [nvarchar](10) NOT NULL,
[Rate] [nvarchar](20) NOT NULL,
[RateFloat] [decimal](18, 4) NOT NULL,
PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
<br><br>
(3).Add BpiDetail<br>
CREATE TABLE [dbo].[BpiDetail](
[Id] [int] IDENTITY(1,1) NOT NULL,
[BpiId] [int] NOT NULL,
[Language] [nvarchar](10) NOT NULL,
[Description] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
<br><br>
(4).Add Content<br>
CREATE TABLE [dbo].[Content](
[Id] [int] IDENTITY(1,1) NOT NULL,
[TimeInfoId] [Uniqueidentifier] NOT NULL,
[ContentKey] [nvarchar](30) NOT NULL,
[Language] [nvarchar](10) NOT NULL,
[Content] [nvarchar](2048) NOT NULL,
PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
<br><br>
(5).Add Application<br>
CREATE TABLE [dbo].[Application](
[Id] [int] IDENTITY(1,1) NOT NULL,
[Name] [nvarchar](30) NOT NULL,
[SecretKey] [nvarchar](256) NOT NULL,
[Enabled] [bit] NOT NULL,
[CreatedAt] [datetime2] NOT NULL,
PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

<br><br>
<b>2.幣別 DB 維護功能 API Endpoint</b><br>
(1)呼叫 coindesk 的 API, Coindesk的GET進行coindesk的API內容取得，並且分表寫入資料庫<br>
(2)Currency的GET/POST/PUT/Delete API 進行幣別的多語系維護功能<br>
(3)Content的POST/PUT/Delete API 進行主檔文案的多語系維護功能<br>
(4)更新時間（時間格式範例：1990/01/01 00:00:00）<br>
(5)幣別相關資訊（幣別，幣別中文名稱，以及匯率）<br>
(6)swagger-ui<br>
<img width="1318" alt="image" src="https://github.com/user-attachments/assets/3c8a438e-d21f-4332-941a-d972c83600d5"><br><br>
<img width="1313" alt="image" src="https://github.com/user-attachments/assets/e4f42725-fcbb-4d7d-97c9-7d1103eb8ad3">

<b>3.Design Pattern 實作</b><br>
整個專案都是使用Mediator(中介者模式)的design pattern進行開發，中介者模式最主要的好處為<br>
    (1)乾淨的架構和關注點分離<br>
    (2)減少耦合<br><br>

<b>4.實作多語系設計<b><br>
(1)Currency的GET API 取得多國語系幣別資訊<br>
(2)當有填寫其他Language文案時，可透過Language可以切換至其他語系<br>
(3)設為en-us語系<br>
<img width="1224" alt="image" src="https://github.com/user-attachments/assets/1c42a8cc-2fa9-476e-a56e-526b013f792b">   
<br>
<b>5.實作加解密技術應用<b><br>
(1)API的安全機制部份，在Header實作了Client-Id及Client-Secret的驗證機制，並且Client-Secret使用AES256進行加驗證，資料庫內儲存的是已Encrypt的AES256碼，可以避免人員讀取資料直接查詢到名碼的Secret KEY<br>
(2)Name：DemoApp<br>
(3)明碼SecretKey：9de27add-6415-49a0-b39f-b17a6a93cf77<br>
(4)AES256 SecretKey：bZPN8bXRkb8V2R3R0Yvr186wfOwYp9c03kctEIiR4UXT/m0D4faS9Ek4fCO6uAuu<br><br>
     <img width="491" alt="image" src="https://github.com/user-attachments/assets/81f34ee2-4a7b-4091-9d73-ac5ded6337ce">
     <img width="529" alt="image" src="https://github.com/user-attachments/assets/6596ea99-f115-44d8-862b-8e891ae41566">
<br><br>
<b>6.Error handling 處理 API response,設定自定義Reponse model<b><br><br>
  {
    "isSuccess": false,
    "returnCode": "9997",
    "message": "en-us is default Language can't delete."
   }
<br><br><br>
<b>7.實作單元測試<b><br><br>
<img width="989" alt="image" src="https://github.com/user-attachments/assets/3defdf2a-5584-4fd7-9dae-c25968088d5b">
<br><br>
<b>8.印出所有 API 被呼叫以及呼叫外部 API 的 request and response body log<b><br><br>
(1)
*``` 
curl --location 'https://localhost:44365/api/Coindesk' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77' \
--data ''

Reponse:
{
    "data": null,
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
*

(2)
‵‵‵
curl --location 'https://localhost:44365/api/Currency?Language=en-us' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77'

{
    "data": {
        "updated": "2024/12/10 02:42:02",
        "disclaimer": "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org",
        "chartName": "Bitcoin",
        "currencys": [
            {
                "code": "EUR",
                "symbol": "€",
                "rate": "92,674.207",
                "rateFloat": 92674.2065,
                "description": "Euro"
            },
            {
                "code": "GBP",
                "symbol": "£",
                "rate": "76,683.269",
                "rateFloat": 76683.2690,
                "description": "British Pound Sterling"
            },
            {
                "code": "USD",
                "symbol": "$",
                "rate": "97,929.081",
                "rateFloat": 97929.0810,
                "description": "United States Dollar"
            }
        ]
    },
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵

(3)
‵‵‵
curl --location 'https://localhost:44365/api/Currency' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77' \
--header 'Content-Type: application/json' \
--data '{
  "TimeInfoId": "375A82F8-9811-40B7-B323-793C61F30AF9",
  "currencyCode": "USD",
  "language": "zh-tw",
  "description": "美元"
}'

Reponse:
{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵

(4)
‵‵‵
curl --location --request PUT 'https://localhost:44365/api/Currency' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77' \
--header 'Content-Type: application/json' \
--data-raw '{
  "TimeInfoId": "375A82F8-9811-40B7-B323-793C61F30AF9",
  "currencyCode": "USD",
  "language": "zh-tw",
  "symbol": "@",
  "rate": "8888,888",
  "rateFloat": 777777,
  "description": "美元(TEST)"
}'

Reponse:
{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵
(5)
‵‵‵
curl --location 'https://localhost:44365/api/Currency/CreateContent' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77' \
--header 'Content-Type: application/json' \
--data '{
  "timeInfoId": "375A82F8-9811-40B7-B323-793C61F30AF9",
  "contentKey": "ChartName",
  "language": "zh-tw",
  "content": "比特幣"
}'

Reponse:
{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵
(6)
‵‵‵
curl --location --request PUT 'https://localhost:44365/api/Currency/UpdateContent' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77' \
--header 'Content-Type: application/json' \
--data '{
  "timeInfoId": "375A82F8-9811-40B7-B323-793C61F30AF9",
  "contentKey": "ChartName",
  "language": "zh-tw",
  "content": "比特幣(更新)"
}'
Reponse:
{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵

(7)
‵‵‵
curl --location --request DELETE 'https://localhost:44365/api/Currency?TimeInfoId=375A82F8-9811-40B7-B323-793C61F30AF9&CurrencyCode=USD&Language=en-us' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77'

{
    "isSuccess": false,
    "returnCode": "9997",
    "message": "en-us is default Language can't delete."
}

{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵
(8)
‵‵‵
curl --location --request DELETE 'https://localhost:44365/api/Currency/DeleteContent?TimeInfoId=375A82F8-9811-40B7-B323-793C61F30AF9&ContentKey=ChartName&Language=en-us' \
--header 'Client-Id: DemoApp' \
--header 'Client-Secret: 9de27add-6415-49a0-b39f-b17a6a93cf77'

Reponse:
{
    "isSuccess": false,
    "returnCode": "9997",
    "message": "en-us is default Language can't delete."
}

{
    "isSuccess": true,
    "returnCode": "0000",
    "message": "成功"
}
‵‵‵


