# Pagination Guide

Bu dokümanda CryptoAlarmSystem API'sinde pagination (sayfalama) nasıl kullanılır açıklanmaktadır.

## Genel Bakış

API'deki liste dönen endpoint'ler otomatik pagination desteği sunar. Bu sayede büyük veri setleri performanslı bir şekilde parçalar halinde alınabilir.

## Pagination Parametreleri

Tüm pagination destekli endpoint'ler aşağıdaki query parametrelerini kabul eder:

| Parametre | Tip | Varsayılan | Açıklama |
|-----------|-----|------------|----------|
| `pageNumber` | integer | 1 | İstenen sayfa numarası (minimum: 1) |
| `pageSize` | integer | 10 | Sayfa başına kayıt sayısı (minimum: 1, maksimum: 100) |

## Pagination Response Formatı

Pagination destekli endpoint'ler aşağıdaki formatta response döner:

```json
{
  "data": [...],           // İstenen sayfa için veri listesi
  "pageNumber": 1,         // Mevcut sayfa numarası
  "pageSize": 10,          // Sayfa başına kayıt sayısı
  "totalRecords": 45,      // Toplam kayıt sayısı
  "totalPages": 5,         // Toplam sayfa sayısı
  "hasNextPage": true,     // Sonraki sayfa var mı?
  "hasPreviousPage": false // Önceki sayfa var mı?
}
```

## Pagination Destekli Endpoint'ler

### 1. Aktif Alarmları Listeleme

```http
GET /api/v1/alarms/active?pageNumber=1&pageSize=20
X-User-Id: user123
```

**Örnek Response:**
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
      "alarmType": {
        "id": 1,
        "code": "ABOVE",
        "name": "Fiyat Üzerine Çıkarsa"
      },
      "targetPrice": 45000.00,
      "status": "Active",
      "createdAt": "2026-02-16T10:30:00Z"
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

### 2. Tetiklenen Alarmları Listeleme

```http
GET /api/v1/alarms/triggered?pageNumber=2&pageSize=10
X-User-Id: user123
```

**Örnek Response:**
```json
{
  "data": [
    {
      "id": 15,
      "userId": "user123",
      "cryptoSymbol": {
        "id": 2,
        "code": "ETH",
        "name": "Ethereum"
      },
      "alarmType": {
        "id": 2,
        "code": "BELOW",
        "name": "Fiyat Altına Düşerse"
      },
      "targetPrice": 2500.00,
      "triggeredPrice": 2450.00,
      "triggeredAt": "2026-02-16T12:45:00Z",
      "status": "Triggered"
    }
  ],
  "pageNumber": 2,
  "pageSize": 10,
  "totalRecords": 23,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": true
}
```

### 3. Alarm Bildirim Logları

```http
GET /api/v1/alarms/1/logs?pageNumber=1&pageSize=50
X-User-Id: user123
```

**Örnek Response:**
```json
{
  "data": [
    {
      "id": 1,
      "alarmId": 1,
      "notificationChannel": {
        "id": 1,
        "code": "EMAIL",
        "name": "Email"
      },
      "sentAt": "2026-02-16T12:45:30Z",
      "status": "Sent"
    },
    {
      "id": 2,
      "alarmId": 1,
      "notificationChannel": {
        "id": 2,
        "code": "SMS",
        "name": "SMS"
      },
      "sentAt": "2026-02-16T12:45:31Z",
      "status": "Sent"
    }
  ],
  "pageNumber": 1,
  "pageSize": 50,
  "totalRecords": 2,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

## Kullanım Örnekleri

### cURL ile Kullanım

**İlk sayfa (varsayılan):**
```bash
curl -X GET "http://localhost:8080/api/v1/alarms/active" \
  -H "X-User-Id: user123"
```

**Özel sayfa ve boyut:**
```bash
curl -X GET "http://localhost:8080/api/v1/alarms/active?pageNumber=2&pageSize=25" \
  -H "X-User-Id: user123"
```

### JavaScript/TypeScript ile Kullanım

```typescript
interface PaginatedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

async function getActiveAlarms(pageNumber: number = 1, pageSize: number = 10) {
  const response = await fetch(
    `http://localhost:8080/api/v1/alarms/active?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    {
      headers: {
        'X-User-Id': 'user123'
      }
    }
  );
  
  const result: PaginatedResponse<Alarm> = await response.json();
  return result;
}

// Kullanım
const firstPage = await getActiveAlarms(1, 20);
console.log(`Toplam ${firstPage.totalRecords} alarm, ${firstPage.totalPages} sayfa`);

if (firstPage.hasNextPage) {
  const secondPage = await getActiveAlarms(2, 20);
  console.log('İkinci sayfa yüklendi');
}
```

### C# ile Kullanım

```csharp
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public async Task<PaginatedResponse<AlarmResponse>> GetActiveAlarmsAsync(
    int pageNumber = 1, 
    int pageSize = 10)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-User-Id", "user123");
    
    var response = await client.GetAsync(
        $"http://localhost:8080/api/v1/alarms/active?pageNumber={pageNumber}&pageSize={pageSize}"
    );
    
    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<AlarmResponse>>();
    return result;
}

// Kullanım
var firstPage = await GetActiveAlarmsAsync(1, 20);
Console.WriteLine($"Toplam {firstPage.TotalRecords} alarm, {firstPage.TotalPages} sayfa");

if (firstPage.HasNextPage)
{
    var secondPage = await GetActiveAlarmsAsync(2, 20);
    Console.WriteLine("İkinci sayfa yüklendi");
}
```

## Validation Kuralları

### PageNumber
- Minimum değer: 1
- Maksimum değer: Yok (ancak totalPages'i geçemez)
- Varsayılan: 1

### PageSize
- Minimum değer: 1
- Maksimum değer: 100
- Varsayılan: 10

**Geçersiz değerler için örnek response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "PageSize": [
      "Page size must be between 1 and 100"
    ]
  }
}
```

## Best Practices

### 1. Uygun Sayfa Boyutu Seçimi
- Mobil uygulamalar için: 10-20 kayıt
- Web uygulamaları için: 20-50 kayıt
- Raporlama için: 50-100 kayıt

### 2. Infinite Scroll Implementasyonu
```typescript
class AlarmList {
  private currentPage = 1;
  private pageSize = 20;
  private allAlarms: Alarm[] = [];
  
  async loadMore() {
    const response = await getActiveAlarms(this.currentPage, this.pageSize);
    this.allAlarms.push(...response.data);
    
    if (response.hasNextPage) {
      this.currentPage++;
    }
    
    return response;
  }
  
  async refresh() {
    this.currentPage = 1;
    this.allAlarms = [];
    return await this.loadMore();
  }
}
```

### 3. Sayfa Navigasyonu
```typescript
class PaginationController {
  private currentPage = 1;
  private totalPages = 1;
  
  async goToPage(pageNumber: number) {
    if (pageNumber < 1 || pageNumber > this.totalPages) {
      throw new Error('Invalid page number');
    }
    
    const response = await getActiveAlarms(pageNumber, 20);
    this.currentPage = response.pageNumber;
    this.totalPages = response.totalPages;
    
    return response;
  }
  
  async nextPage() {
    return await this.goToPage(this.currentPage + 1);
  }
  
  async previousPage() {
    return await this.goToPage(this.currentPage - 1);
  }
  
  async firstPage() {
    return await this.goToPage(1);
  }
  
  async lastPage() {
    return await this.goToPage(this.totalPages);
  }
}
```

### 4. Caching Stratejisi
```typescript
class CachedAlarmService {
  private cache = new Map<string, PaginatedResponse<Alarm>>();
  private cacheTimeout = 60000; // 1 dakika
  
  private getCacheKey(pageNumber: number, pageSize: number): string {
    return `alarms_${pageNumber}_${pageSize}`;
  }
  
  async getActiveAlarms(pageNumber: number, pageSize: number) {
    const cacheKey = this.getCacheKey(pageNumber, pageSize);
    const cached = this.cache.get(cacheKey);
    
    if (cached && Date.now() - cached.timestamp < this.cacheTimeout) {
      return cached.data;
    }
    
    const response = await getActiveAlarms(pageNumber, pageSize);
    this.cache.set(cacheKey, {
      data: response,
      timestamp: Date.now()
    });
    
    return response;
  }
  
  clearCache() {
    this.cache.clear();
  }
}
```

## Performance Considerations

### Database Query Optimization
API backend'de pagination Entity Framework Core ile optimize edilmiştir:

```csharp
public async Task<PagedResponse<AlarmResponse>> GetActiveAlarmsAsync(
    string userId, 
    PaginationRequest pagination)
{
    var query = _context.Alarms
        .Where(a => a.UserId == userId && a.Status == AlarmStatus.Active)
        .OrderByDescending(a => a.CreatedAt);
    
    var totalRecords = await query.CountAsync();
    
    var alarms = await query
        .Skip((pagination.PageNumber - 1) * pagination.PageSize)
        .Take(pagination.PageSize)
        .Include(a => a.CryptoSymbol)
        .Include(a => a.AlarmType)
        .ToListAsync();
    
    return new PagedResponse<AlarmResponse>(
        alarms.Select(MapToDto).ToList(),
        pagination.PageNumber,
        pagination.PageSize,
        totalRecords
    );
}
```

### Index Recommendations
Optimal performans için aşağıdaki database index'leri önerilir:

```sql
-- Aktif alarmlar için
CREATE INDEX IX_Alarms_UserId_Status_CreatedAt 
ON Alarms(UserId, Status, CreatedAt DESC);

-- Tetiklenen alarmlar için
CREATE INDEX IX_Alarms_UserId_Status_TriggeredAt 
ON Alarms(UserId, Status, TriggeredAt DESC);

-- Bildirim logları için
CREATE INDEX IX_NotificationLogs_AlarmId_SentAt 
ON NotificationLogs(AlarmId, SentAt DESC);
```

## Troubleshooting

### Problem: Boş sayfa dönüyor
**Çözüm:** PageNumber değerini kontrol edin. TotalPages'i aşan bir sayfa numarası boş data array döner.

```json
{
  "data": [],
  "pageNumber": 10,
  "pageSize": 10,
  "totalRecords": 45,
  "totalPages": 5,
  "hasNextPage": false,
  "hasPreviousPage": true
}
```

### Problem: 400 Bad Request
**Çözüm:** PageSize değerinin 1-100 arasında olduğundan emin olun.

### Problem: Yavaş response
**Çözüm:** 
- PageSize değerini azaltın
- Caching kullanın
- Database index'lerini kontrol edin

## İlgili Dosyalar

- `src/CryptoAlarmSystem.Api/Models/PaginationRequest.cs` - Request model
- `src/CryptoAlarmSystem.Api/Models/PagedResponse.cs` - Response model
- `src/CryptoAlarmSystem.Api/Extensions/PaginationExtensions.cs` - Extension methods
- `src/CryptoAlarmSystem.Api/Controllers/V1/AlarmsController.cs` - Controller implementation

## Daha Fazla Bilgi

Pagination implementasyonu hakkında daha fazla bilgi için:
- [Microsoft Docs - Pagination in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page)
- [REST API Best Practices - Pagination](https://www.moesif.com/blog/technical/api-design/REST-API-Design-Filtering-Sorting-and-Pagination/)
