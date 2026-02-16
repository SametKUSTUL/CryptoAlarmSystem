# 🚀 CryptoAlarmSystem

Kripto para fiyat hareketlerini takip eden ve kullanıcıları çoklu kanallardan bilgilendiren gerçek zamanlı alarm sistemi.

## 📋 İçindekiler

- [Genel Bakış](#-genel-bakış)
- [Mimari](#-mimari)
- [Teknolojiler](#-teknolojiler)
- [Özellikler](#-özellikler)
- [Kurulum](#-kurulum)
- [API Kullanımı](#-api-kullanımı)
- [Sistem Akışı](#-sistem-akışı)
- [Proje Yapısı](#-proje-yapısı)

## 🎯 Genel Bakış

CryptoAlarmSystem, kullanıcıların kripto para fiyatları için alarm oluşturmasına ve belirlenen fiyat seviyelerine ulaşıldığında çoklu kanallardan (Email, SMS, Push) bildirim almasına olanak tanıyan mikroservis tabanlı bir sistemdir.

### Temel Özellikler

- ✅ 9 farklı kripto para desteği (BTC, ETH, SOL, DOGE, LTC, XRP, BNB, USDT, ADA)
- ✅ İki yönlü alarm tipi (Fiyat üzerine çıkarsa / Fiyat altına düşerse)
- ✅ Çoklu bildirim kanalı (Email, SMS, Push Notification)
- ✅ Gerçek zamanlı fiyat takibi
- ✅ Asenkron bildirim işleme
- ✅ Kapsamlı loglama ve izleme
- ✅ Docker ile kolay deployment

## 🏗️ Mimari

Proje **Clean Architecture** prensiplerine göre tasarlanmıştır ve 6 ana katmandan oluşur:

```
┌─────────────────────────────────────────────────────────────────┐
│                    CryptoAlarmSystem                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────┐         ┌──────────────────┐              │
│  │  PriceWorker     │         │   API Server     │              │
│  │  (Container)     │────────▶│  (Container)     │              │
│  │                  │ HTTP    │                  │              │
│  │ Generates prices │ POST    │ ┌──────────────┐ │              │
│  │ every 10s        │ /prices │ │ Controllers  │ │              │
│  └──────────────────┘         │ │ - Alarms     │ │              │
│                               │ │ - Prices     │ │              │
│                               │ └──────────────┘ │              │
│                               │ ┌──────────────┐ │              │
│                               │ │ Services     │ │              │
│                               │ │ - Alarm      │ │              │
│                               │ │ - AlarmCheck │ │              │
│                               │ │ - Notif      │ │              │
│                               │ └──────────────┘ │              │
│                               │        │         │              │
│                               │        │ Publish │              │
│                               │        ▼         │              │
│                               │ ┌──────────────┐ │              │
│                               │ │ RabbitMQ     │ │              │
│                               │ │ Publisher    │ │              │
│                               │ └──────────────┘ │              │
│                               └────────┬─────────┘              │
│                                        │                        │
│                                        │ Queue: notifications   │
│                                        ▼                        │
│  ┌──────────────────┐         ┌──────────────────┐              │
│  │ NotificationWorker│         │   RabbitMQ       │              │
│  │  (Container)     │◀────────│   (Container)    │              │
│  │                  │ Consume │                  │              │
│  │ Processes        │         └──────────────────┘              │
│  │ notifications    │                                           │
│  │ - Email          │                                           │
│  │ - SMS            │                                           │
│  │ - Push           │                                           │
│  └────────┬─────────┘                                           │
│           │                                                     │
│           │ Logs notifications                                  │
│           ▼                                                     │
│  ┌──────────────────┐         ┌──────────────────┐              │
│  │   PostgreSQL     │         │  Elasticsearch   │              │
│  │   (Container)    │         │  (Container)     │              │
│  │                  │         │                  │              │
│  │ - Alarms         │         │ - Logs           │              │
│  │ - Symbols        │         │ - Traces         │              │
│  │ - Channels       │         │                  │              │
│  │ - NotifLogs      │         └──────────────────┘              │
│  └──────────────────┘                                           │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

### Katmanlar

1. **Domain Layer** (`CryptoAlarmSystem.Domain`)
   - Core business entities (Alarm, CryptoSymbol, NotificationChannel)
   - Domain enums (AlarmStatus, AlarmType)
   - Business logic

2. **Application Layer** (`CryptoAlarmSystem.Application`)
   - Business rules ve workflows
   - Service interfaces ve implementations
   - DTOs ve mapping
   - RabbitMQ messaging

3. **Infrastructure Layer** (`CryptoAlarmSystem.Infrastructure`)
   - Entity Framework Core DbContext
   - Database configurations
   - Data access implementations

4. **API Layer** (`CryptoAlarmSystem.Api`)
   - REST API endpoints
   - Controllers (Alarms, Prices)
   - Validators (FluentValidation)
   - Middleware (Logging, Error Handling)

5. **PriceWorker** (`CryptoAlarmSystem.PriceWorker`)
   - Background service
   - Her 10 saniyede bir rastgele fiyat üretir
   - API'ye fiyat güncellemesi gönderir

6. **NotificationWorker** (`CryptoAlarmSystem.NotificationWorker`)
   - Background service
   - RabbitMQ'dan bildirimleri consume eder
   - Strategy pattern ile çoklu kanal desteği
   - Bildirim loglarını veritabanına kaydeder

## 🛠️ Teknolojiler

### Backend
- **.NET 8** - Modern C# framework
- **Entity Framework Core** - ORM
- **PostgreSQL 16** - İlişkisel veritabanı
- **RabbitMQ 3** - Message broker
- **FluentValidation** - Request validation
- **NLog** - Structured logging
- **OpenTelemetry** - Distributed tracing

### Infrastructure
- **Docker & Docker Compose** - Containerization
- **Elasticsearch 7.17** - Log aggregation
- **Kibana 7.17** - Log visualization

### Design Patterns
- Clean Architecture
- Strategy Pattern (Notification strategies)
- Factory Pattern (NotificationStrategyFactory)
- Workflow Pattern (Business rules orchestration)
- Result Pattern (Error handling)
- Repository Pattern (EF Core DbContext)

## ✨ Özellikler

### Alarm Yönetimi
- Kripto para fiyatları için alarm oluşturma
- İki alarm tipi:
  - **ABOVE**: Fiyat hedef değerin üzerine çıkarsa
  - **BELOW**: Fiyat hedef değerin altına düşerse
- Çoklu bildirim kanalı seçimi
- Aktif alarmları listeleme
- Tetiklenen alarmları görüntüleme
- Alarm silme (soft delete)
- Bildirim kanallarını güncelleme

### Fiyat Takibi
- 9 kripto para desteği:
  - Bitcoin (BTC): $40,000 - $50,000
  - Ethereum (ETH): $2,000 - $3,000
  - Solana (SOL): $80 - $120
  - Dogecoin (DOGE): $0.05 - $0.15
  - Litecoin (LTC): $60 - $100
  - Ripple (XRP): $0.40 - $0.60
  - Binance Coin (BNB): $200 - $350
  - Tether (USDT): $0.99 - $1.01
  - Cardano (ADA): $0.30 - $0.50
- Her 10 saniyede bir otomatik fiyat güncellemesi
- Gerçek zamanlı alarm kontrolü

### Bildirim Sistemi
- Çoklu kanal desteği:
  - 📧 Email
  - 📱 SMS
  - 🔔 Push Notification
- Asenkron işleme (RabbitMQ)
- Bildirim geçmişi ve audit trail
- Extensible strategy pattern

### Business Rules
- Kripto sembol varlık kontrolü
- Duplicate alarm önleme (aynı kullanıcı, sembol, tip ve fiyat)
- Kullanıcı bazlı yetkilendirme (X-User-Id header)

## 🚀 Kurulum

### Gereksinimler
- Docker Desktop
- .NET 8 SDK (local development için)
- Git

### Docker ile Çalıştırma

1. Repository'yi klonlayın:
```bash
git clone <repository-url>
cd CryptoAlarmSystem
```

2. Docker Compose ile tüm servisleri başlatın:
```bash
docker-compose up -d
```

3. Servisler hazır olduğunda:
- API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- Kibana: http://localhost:5601
- PostgreSQL: localhost:5432

### Container'lar

Sistem 7 container'dan oluşur:
- `cryptoalarm-api` - REST API
- `cryptoalarm-priceworker` - Fiyat üretici worker
- `cryptoalarm-notificationworker` - Bildirim işleyici worker
- `cryptoalarm-db` - PostgreSQL database
- `cryptoalarm-rabbitmq` - Message broker
- `cryptoalarm-elasticsearch` - Log storage
- `cryptoalarm-kibana` - Log visualization

### Health Checks

API health check endpoint'i:
```bash
curl http://localhost:8080/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2026-02-16T10:30:00Z"
}
```

## 📡 API Kullanımı

### Base URL
```
http://localhost:8080/api
```

### Authentication
Tüm kullanıcı bazlı endpoint'ler `X-User-Id` header'ı gerektirir:
```
X-User-Id: user123
```

### Pagination
Liste dönen endpoint'ler otomatik pagination desteği sunar. Query parametreleri:
- `pageNumber`: Sayfa numarası (varsayılan: 1)
- `pageSize`: Sayfa başına kayıt sayısı (varsayılan: 10, maksimum: 100)

Örnek:
```http
GET /api/alarms/active?pageNumber=1&pageSize=20
```

Pagination response formatı:
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalRecords": 45,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

Detaylı bilgi için: [PAGINATION.md](src/CryptoAlarmSystem.Api/PAGINATION.md)

### Endpoints

#### 1. Alarm Oluşturma
```http
POST /api/alarms
X-User-Id: user123
Content-Type: application/json

{
  "cryptoSymbolId": 1,
  "alarmTypeId": 1,
  "targetPrice": 45000.00,
  "notificationChannelIds": [1, 2]
}
```

Response (201 Created):
```json
{
  "id": 1,
  "userId": "user123",
  "cryptoSymbol": {
    "id": 1,
    "code": "BTC",
    "name": "Bitcoin"
  },
  "alarmType": {
    "id": 1,
    "code": "ABOVE",
    "name": "Fiyat Üzerine Çıkarsa"
  },
  "targetPrice": 45000.00,
  "status": "Active",
  "createdAt": "2026-02-16T10:30:00Z",
  "notificationChannels": [
    {
      "id": 1,
      "code": "EMAIL",
      "name": "Email"
    },
    {
      "id": 2,
      "code": "SMS",
      "name": "SMS"
    }
  ]
}
```

#### 2. Aktif Alarmları Listeleme (Pagination Destekli)
```http
GET /api/alarms/active?pageNumber=1&pageSize=20
X-User-Id: user123
```

Response:
```json
{
  "data": [
    {
      "id": 1,
      "userId": "user123",
      "cryptoSymbol": {
        "id": 1,
        "code": "BTC",
        "name": "Bitcoin"
      },
      "targetPrice": 45000.00,
      "status": "Active"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalRecords": 45,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

#### 3. Tetiklenen Alarmları Listeleme (Pagination Destekli)
```http
GET /api/alarms/triggered?pageNumber=1&pageSize=10
X-User-Id: user123
```

#### 4. Alarm Silme
```http
DELETE /api/alarms/{id}
X-User-Id: user123
```

#### 5. Bildirim Kanallarını Güncelleme
```http
PATCH /api/alarms/{id}/channels
X-User-Id: user123
Content-Type: application/json

{
  "notificationChannelIds": [1, 3]
}
```

#### 6. Alarm Bildirim Logları (Pagination Destekli)
```http
GET /api/alarms/{id}/logs?pageNumber=1&pageSize=50
X-User-Id: user123
```

#### 7. Kripto Sembolleri Listeleme
```http
GET /api/alarms/crypto-symbols
```

Response:
```json
[
  {
    "id": 1,
    "code": "BTC",
    "name": "Bitcoin"
  },
  {
    "id": 2,
    "code": "ETH",
    "name": "Ethereum"
  }
]
```

#### 8. Bildirim Kanalları Listeleme
```http
GET /api/alarms/notification-channels
```

#### 9. Alarm Tipleri Listeleme
```http
GET /api/alarms/alarm-types
```

### Validation Rules

#### CreateAlarmRequest
- `cryptoSymbolId`: Required, must exist
- `alarmTypeId`: Required, must exist
- `targetPrice`: Required, must be > 0
- `notificationChannelIds`: Required, at least 1 channel

#### UpdateAlarmChannelsRequest
- `notificationChannelIds`: Required, at least 1 channel

## 🔄 Sistem Akışı

### 1. Fiyat Güncelleme ve Alarm Kontrolü

```
┌─────────────┐
│ PriceWorker │
└──────┬──────┘
       │ Her 10 saniyede bir
       │ 9 kripto için fiyat üret
       ▼
┌─────────────────────────────┐
│ POST /api/prices/update     │
│ { symbolId: 1, price: 45500 }│
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ AlarmCheckService           │
│ - Aktif alarmları getir     │
│ - Fiyat koşulunu kontrol et │
│ - Alarm tetikle             │
└──────────┬──────────────────┘
           │ Koşul sağlanırsa
           ▼
┌─────────────────────────────┐
│ Alarm Tetikleme             │
│ - Status = Triggered        │
│ - TriggeredPrice kaydet     │
│ - TriggeredAt kaydet        │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ NotificationService         │
│ - Bildirim mesajı oluştur   │
│ - RabbitMQ'ya publish et    │
└─────────────────────────────┘
```

### 2. Bildirim İşleme

```
┌─────────────────────────────┐
│ RabbitMQ Queue              │
│ "notifications"             │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ NotificationWorker          │
│ - Mesajı consume et         │
│ - Her kanal için işle       │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ NotificationStrategyFactory │
│ - Kanal koduna göre         │
│   strategy seç              │
└──────────┬──────────────────┘
           │
           ├──────────────────────┬──────────────────────┐
           ▼                      ▼                      ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ EmailStrategy    │  │ SmsStrategy      │  │ PushStrategy     │
│ - Email gönder   │  │ - SMS gönder     │  │ - Push gönder    │
└──────────────────┘  └──────────────────┘  └──────────────────┘
           │                      │                      │
           └──────────────────────┴──────────────────────┘
                                  │
                                  ▼
                    ┌─────────────────────────────┐
                    │ NotificationLog             │
                    │ - Bildirim kaydı oluştur    │
                    │ - Database'e kaydet         │
                    └─────────────────────────────┘
```

### 3. Alarm Oluşturma Workflow

```
┌─────────────────────────────┐
│ POST /api/alarms            │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ FluentValidation            │
│ - Request validation        │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ CreateAlarmWorkflow         │
│ - Business rules çalıştır   │
└──────────┬──────────────────┘
           │
           ├─────────────────────────────┐
           ▼                             ▼
┌──────────────────────┐  ┌──────────────────────────┐
│ CryptoSymbolExists   │  │ NoDuplicateActiveAlarm   │
│ Rule                 │  │ Rule                     │
└──────────┬───────────┘  └──────────┬───────────────┘
           │                         │
           └────────────┬────────────┘
                        │ Tüm kurallar OK
                        ▼
           ┌─────────────────────────────┐
           │ AlarmService                │
           │ - Alarm entity oluştur      │
           │ - Channels ilişkilendir     │
           │ - Database'e kaydet         │
           └─────────────────────────────┘
```

## 📁 Proje Yapısı

```
CryptoAlarmSystem/
├── src/
│   ├── CryptoAlarmSystem.Domain/
│   │   ├── Entities/
│   │   │   ├── Alarm.cs
│   │   │   ├── CryptoSymbol.cs
│   │   │   ├── AlarmType.cs
│   │   │   ├── NotificationChannel.cs
│   │   │   ├── AlarmNotificationChannel.cs
│   │   │   └── NotificationLog.cs
│   │   ├── Enums/
│   │   │   └── AlarmStatus.cs
│   │   └── Common/
│   │       ├── Result.cs
│   │       └── ErrorCode.cs
│   │
│   ├── CryptoAlarmSystem.Application/
│   │   ├── Services/
│   │   │   ├── AlarmService.cs
│   │   │   ├── AlarmCheckService.cs
│   │   │   └── NotificationService.cs
│   │   ├── Interfaces/
│   │   │   ├── IAlarmService.cs
│   │   │   ├── IAlarmCheckService.cs
│   │   │   └── INotificationService.cs
│   │   ├── DTOs/
│   │   │   └── AlarmDtos.cs
│   │   ├── BusinessRules/
│   │   │   ├── IBusinessRule.cs
│   │   │   └── CreateAlarm/
│   │   │       ├── CryptoSymbolExistsRule.cs
│   │   │       └── NoDuplicateActiveAlarmRule.cs
│   │   ├── Workflows/
│   │   │   ├── IWorkflow.cs
│   │   │   └── CreateAlarmWorkflow.cs
│   │   ├── Strategies/
│   │   │   ├── INotificationStrategy.cs
│   │   │   ├── NotificationStrategyFactory.cs
│   │   │   ├── EmailNotificationStrategy.cs
│   │   │   ├── SmsNotificationStrategy.cs
│   │   │   └── PushNotificationStrategy.cs
│   │   ├── Messaging/
│   │   │   └── NotificationPublisher.cs
│   │   ├── Models/
│   │   │   └── NotificationMessage.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── CryptoAlarmSystem.Infrastructure/
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── DbInitializer.cs
│   │   │   └── Configurations/
│   │   │       ├── AlarmConfiguration.cs
│   │   │       ├── CryptoSymbolConfiguration.cs
│   │   │       └── ...
│   │   └── DependencyInjection.cs
│   │
│   ├── CryptoAlarmSystem.Api/
│   │   ├── Controllers/
│   │   │   ├── AlarmsController.cs
│   │   │   └── PricesController.cs
│   │   ├── Filters/
│   │   │   └── ValidateUserIdAttribute.cs
│   │   ├── Middlewares/
│   │   │   └── RequestResponseLoggingMiddleware.cs
│   │   ├── Validators/
│   │   │   ├── CreateAlarmRequestValidator.cs
│   │   │   ├── PriceUpdateRequestValidator.cs
│   │   │   └── UpdateAlarmChannelsRequestValidator.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── nlog.config
│   │
│   ├── CryptoAlarmSystem.PriceWorker/
│   │   ├── PriceGeneratorWorker.cs
│   │   ├── Models/
│   │   │   └── PriceUpdateRequest.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── CryptoAlarmSystem.NotificationWorker/
│       ├── NotificationConsumer.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── Dockerfile
├── Dockerfile.priceworker
├── Dockerfile.notificationworker
├── docker-compose.yml
├── .dockerignore
├── .gitignore
├── CryptoAlarmSystem.sln
└── README.md
```

## 🔧 Konfigürasyon

### Environment Variables

#### API Container
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=db;Database=cryptoalarm;Username=postgres;Password=postgres
RabbitMQ__Host=rabbitmq
TZ=Europe/Istanbul
```

#### PriceWorker Container
```env
ApiSettings__BaseUrl=http://api:8080
TZ=Europe/Istanbul
```

#### NotificationWorker Container
```env
ConnectionStrings__DefaultConnection=Host=db;Database=cryptoalarm;Username=postgres;Password=postgres
RabbitMQ__Host=rabbitmq
TZ=Europe/Istanbul
```

### Database Connection String
```
Host=db;Database=cryptoalarm;Username=postgres;Password=postgres
```

### RabbitMQ Configuration
- Host: rabbitmq
- Queue: notifications
- Durable: true
- Auto-delete: false

## 📊 Monitoring & Logging

### NLog Configuration
- Console logging
- Elasticsearch sink
- Structured logging with JSON format
- Log levels: Trace, Debug, Info, Warn, Error, Fatal

### OpenTelemetry Tracing
- ASP.NET Core instrumentation
- HTTP client instrumentation
- Console exporter

### Kibana Dashboards
- Access: http://localhost:5601
- Index pattern: `logstash-*`
- Visualize logs, traces, and metrics

### RabbitMQ Management
- Access: http://localhost:15672
- Username: guest
- Password: guest
- Monitor queues, exchanges, and message rates

## 🧪 Testing

### Manual Testing with Swagger
1. Navigate to http://localhost:8080/swagger
2. Use "Try it out" for each endpoint
3. Add `X-User-Id` header for user-specific operations

### Example Test Scenario

1. **Kripto sembolleri listele**:
```bash
curl http://localhost:8080/api/alarms/crypto-symbols
```

2. **Alarm oluştur** (BTC $45,000 üzerine çıkarsa):
```bash
curl -X POST http://localhost:8080/api/alarms \
  -H "Content-Type: application/json" \
  -H "X-User-Id: test-user" \
  -d '{
    "cryptoSymbolId": 1,
    "alarmTypeId": 1,
    "targetPrice": 45000,
    "notificationChannelIds": [1, 2]
  }'
```

3. **Aktif alarmları kontrol et**:
```bash
curl http://localhost:8080/api/alarms/active \
  -H "X-User-Id: test-user"
```

4. **PriceWorker loglarını izle**:
```bash
docker logs -f cryptoalarm-priceworker
```

5. **Alarm tetiklendiğinde bildirim loglarını kontrol et**:
```bash
curl http://localhost:8080/api/alarms/1/logs \
  -H "X-User-Id: test-user"
```

## 🐛 Troubleshooting

### Container başlamıyor
```bash
# Tüm container'ları durdur ve temizle
docker-compose down -v

# Yeniden başlat
docker-compose up -d

# Logları kontrol et
docker-compose logs -f
```

### Database bağlantı hatası
```bash
# PostgreSQL container'ının sağlıklı olduğunu kontrol et
docker ps
docker logs cryptoalarm-db

# Database'e manuel bağlan
docker exec -it cryptoalarm-db psql -U postgres -d cryptoalarm
```

### RabbitMQ bağlantı hatası
```bash
# RabbitMQ container'ını kontrol et
docker logs cryptoalarm-rabbitmq

# Management UI'dan kontrol et
# http://localhost:15672 (guest/guest)
```

### API health check başarısız
```bash
# API container loglarını kontrol et
docker logs cryptoalarm-api

# Health endpoint'i test et
curl http://localhost:8080/health
```

## 🚀 Production Deployment

### Öneriler
1. Environment variables'ı güvenli şekilde yönetin (secrets)
2. PostgreSQL için güçlü şifre kullanın
3. RabbitMQ için authentication ekleyin
4. HTTPS kullanın (reverse proxy ile)
5. Rate limiting ekleyin
6. Database backup stratejisi oluşturun
7. Monitoring ve alerting ekleyin (Prometheus, Grafana)
8. Log retention policy belirleyin

### Docker Compose Production
```yaml
# Production için örnek değişiklikler
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
  - RabbitMQ__Host=${RABBITMQ_HOST}
  - RabbitMQ__Username=${RABBITMQ_USER}
  - RabbitMQ__Password=${RABBITMQ_PASS}
```

## 📝 License

Bu proje eğitim amaçlı geliştirilmiştir.

## 👥 Contributors

- Development Team

## 📞 İletişim

Sorularınız için issue açabilirsiniz.

---

**Not**: Bu sistem demo amaçlıdır. PriceWorker rastgele fiyatlar üretir. Production kullanımı için gerçek kripto fiyat API'si entegre edilmelidir (örn: CoinGecko, Binance API).
