# CryptoAlarmSystem - Test Projeleri

Bu klasör, CryptoAlarmSystem projesi için yazılmış test projelerini içerir.

## 📦 Test Projeleri

### 1. CryptoAlarmSystem.UnitTests
Unit testler - İzole bileşen testleri

**Test Edilen Katmanlar:**
- ✅ Validators (FluentValidation)
- ✅ Business Rules
- ✅ Domain Entities
- ✅ Services (Mock kullanarak)

**Kullanılan Teknolojiler:**
- xUnit - Test framework
- FluentAssertions - Assertion library
- Moq - Mocking framework
- InMemoryDatabase - EF Core test database

### 2. CryptoAlarmSystem.IntegrationTests
Integration testler - API endpoint testleri

**Test Edilen Katmanlar:**
- ✅ API Controllers (AlarmsController, PricesController)
- ✅ End-to-end request/response flow
- ✅ Validation pipeline
- ✅ Database operations

**Kullanılan Teknolojiler:**
- xUnit - Test framework
- FluentAssertions - Assertion library
- WebApplicationFactory - API test host
- InMemoryDatabase - Test database

## 🚀 Testleri Çalıştırma

### Tüm Testleri Çalıştır
```bash
dotnet test
```

### Sadece Unit Testleri Çalıştır
```bash
dotnet test tests/CryptoAlarmSystem.UnitTests/CryptoAlarmSystem.UnitTests.csproj
```

### Sadece Integration Testleri Çalıştır
```bash
dotnet test tests/CryptoAlarmSystem.IntegrationTests/CryptoAlarmSystem.IntegrationTests.csproj
```

### Verbose Output ile Çalıştır
```bash
dotnet test --verbosity detailed
```

### Code Coverage ile Çalıştır
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Test Kapsamı

### Unit Tests (CryptoAlarmSystem.UnitTests)

#### Validators (3 test sınıfı)
- **CreateAlarmRequestValidatorTests** - 5 test
  - Valid request validation
  - Invalid CryptoSymbolId validation
  - Invalid TargetPrice validation
  - Empty notification channels validation
  - Multiple notification channels validation

- **PriceUpdateRequestValidatorTests** - 3 test
  - Valid request validation
  - Invalid CryptoSymbolId validation
  - Invalid Price validation

- **UpdateAlarmChannelsRequestValidatorTests** - 3 test
  - Valid request validation
  - Empty channels validation
  - Multiple channels validation

#### Business Rules (2 test sınıfı)
- **CryptoSymbolExistsRuleTests** - 3 test
  - Existing symbol validation
  - Non-existing symbol validation
  - Multiple existing symbols validation

- **NoDuplicateActiveAlarmRuleTests** - 5 test
  - No duplicate alarm validation
  - Duplicate active alarm detection
  - Null userId validation
  - Different user validation
  - Different alarm type validation

#### Domain (2 test sınıfı)
- **ResultTests** - 5 test
  - Success result creation
  - Failure result creation
  - Generic success with data
  - Generic failure without data
  - Complex object storage

- **AlarmEntityTests** - 5 test
  - Default values initialization
  - Properties setting
  - Triggered properties
  - Status types support
  - Navigation properties

#### Services (1 test sınıfı)
- **AlarmCheckServiceTests** - 5 test
  - No active alarms scenario
  - Price above target trigger
  - Price below target trigger
  - Price not meeting condition
  - Multiple alarms filtering

**Toplam Unit Tests: ~29 test**

### Integration Tests (CryptoAlarmSystem.IntegrationTests)

#### Controllers (2 test sınıfı)
- **AlarmsControllerTests** - 11 test
  - Get crypto symbols
  - Get notification channels
  - Get alarm types
  - Create alarm with valid data
  - Create alarm with invalid symbol
  - Create alarm with negative price
  - Create alarm without user ID
  - Get active alarms (pagination)
  - Delete existing alarm
  - Delete non-existing alarm
  - Update alarm channels

- **PricesControllerTests** - 5 test
  - Update price with valid data
  - Update price with invalid symbol
  - Update price with negative price
  - Update price with zero price
  - Update price for different symbols

**Toplam Integration Tests: ~16 test**

**GENEL TOPLAM: ~45 test**

## 🎯 Test Stratejisi

### Unit Tests
- **Amaç**: İzole bileşenlerin doğru çalıştığını doğrulama
- **Kapsam**: Validators, Business Rules, Domain Logic, Services
- **Yaklaşım**: Mock/Stub kullanarak bağımlılıkları izole etme
- **Hız**: Çok hızlı (milisaniyeler)

### Integration Tests
- **Amaç**: Bileşenlerin birlikte çalıştığını doğrulama
- **Kapsam**: API endpoints, Request/Response flow, Database operations
- **Yaklaşım**: WebApplicationFactory ile gerçek HTTP istekleri
- **Hız**: Orta (saniyeler)

## 📝 Test Yazma Kuralları

### Naming Convention
```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    // Act
    // Assert
}
```

### AAA Pattern
Her test 3 bölümden oluşmalı:
1. **Arrange**: Test verilerini hazırla
2. **Act**: Test edilecek metodu çalıştır
3. **Assert**: Sonuçları doğrula

### FluentAssertions Kullanımı
```csharp
// ✅ İyi
result.IsSuccess.Should().BeTrue();
result.ErrorMessage.Should().Contain("not found");

// ❌ Kötü
Assert.True(result.IsSuccess);
Assert.Contains("not found", result.ErrorMessage);
```

## 🔧 Troubleshooting

### Test Başarısız Oluyor
```bash
# Detaylı log ile çalıştır
dotnet test --verbosity detailed

# Sadece başarısız testleri göster
dotnet test --logger "console;verbosity=detailed"
```

### InMemoryDatabase Sorunları
Her test için yeni bir database instance oluşturulur:
```csharp
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### Integration Test Port Çakışması
WebApplicationFactory otomatik olarak rastgele port kullanır, manuel port ayarı gerekmez.

## 📚 Kaynaklar

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

## 🎓 Öğrenme Notları

### Test Piramidi
```
        /\
       /  \      E2E Tests (Az sayıda)
      /____\
     /      \    Integration Tests (Orta sayıda)
    /________\
   /          \  Unit Tests (Çok sayıda)
  /____________\
```

### Test Coverage Hedefi
- Unit Tests: %80+ coverage
- Integration Tests: Kritik API endpoints
- E2E Tests: Ana kullanıcı senaryoları

### Best Practices
1. Testler birbirinden bağımsız olmalı
2. Testler hızlı çalışmalı
3. Testler tekrarlanabilir olmalı
4. Test isimleri açıklayıcı olmalı
5. Her test tek bir şeyi test etmeli

## 🚀 Gelecek İyileştirmeler

- [ ] Test coverage raporlama (Coverlet)
- [ ] Mutation testing (Stryker.NET)
- [ ] Performance tests (BenchmarkDotNet)
- [ ] E2E tests (Playwright/Selenium)
- [ ] Contract tests (Pact)
- [ ] Load tests (k6/JMeter)
