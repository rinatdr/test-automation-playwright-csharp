# 🧪 Test Automation – UI & API Tests with Playwright & C#

Automated tests for two public demo applications, written in **C#** with **Playwright** and **NUnit**:

* **UI tests** for [Swag Labs](https://www.saucedemo.com), Sauce Labs' demo e-commerce site, following the **Page Object Model (POM)** design pattern. They cover the main user flows: logging in, adding products to the cart, managing the cart and checking out, from the customer information form to the order overview and completion.
* **API tests** for [Restful-Booker](https://restful-booker.herokuapp.com), a hotel booking REST API. They cover authentication and the full booking lifecycle: listing, reading, creating, updating and deleting bookings.

The two kinds of tests live in separate folders (`UI/` and `Api/`) and share only the configuration, so each can be run on its own. The project shows how a maintainable test framework can be set up: page objects and API clients, separate test data, configuration for several environments and test categories.

---

## 🌟 Key Features

* **UI and API kept apart:** `UI/` and `Api/` each have their own base class, test data and tests. Page objects exist only under `UI/Pages`, and their API counterparts, the clients, only under `Api/Clients`.
* **Page Object Model (POM):** Each page (`LoginPage`, `InventoryPage`, `CartPage`, `CheckoutStepOnePage`, `CheckoutStepTwoPage`, `CheckoutCompletePage`) holds its locators and actions, so tests read like user steps and a UI change is fixed in one place.
* **API clients:** `AuthClient` and `BookingClient` wrap the HTTP calls, so a test calls `BookingClient.CreateBookingAsync(booking)` instead of building a request. Requests and responses are typed C# records under `Api/Models`.
* **API tests clean up after themselves:** Restful-Booker is public and shared, so every test creates the bookings it needs and `ApiTest` deletes them after the test, even when the test fails.
* **Stable locators:** UI elements are located by their `data-test` attributes instead of CSS classes or text, which keeps tests from breaking when the styling changes.
* **Multi-environment support:** The UI and API base URLs of each environment live in `Config/appsettings.json`, and the same tests run against any environment chosen at run time, with no code changes (see [Environments](#-environments)).
* **Separate test data:** Users, products, credentials and bookings are typed C# records in each side's `TestData/` folder, not strings scattered through the tests.
* **Data-driven tests:** The valid-login test runs once for every user who is allowed to log in, using `[TestCaseSource]`, and the missing-field checkout tests use `[TestCase]`.
* **Test categories:** Every test is tagged by layer (`UI` / `Api`), by feature (`Login`, `Inventory`, `Cart`, `Checkout`, `Auth`, `Booking`), by type (`Positive` / `Negative`) and, for critical-path tests, `Smoke`, so any subset can be run on its own.
* **Known bugs as failing tests:** Tests for known bugs assert the correct behavior and are tagged `KnownBug`, so they fail until the bug is fixed (see [Known Bugs](#-known-bugs)).
* **Shared base tests:** `BaseTest` builds on Playwright's `PageTest`, which creates a fresh browser context and page for every UI test. `ApiTest` builds on `PlaywrightTest` and creates a fresh `APIRequestContext` for every API test, without opening a browser.
* **Browser configured in one file:** The browser, headless mode and slow-motion delay are set in `.runsettings` and can be overridden from the command line.

---

## 🛠️ Tech Stack

| | |
|---|---|
| **Language** | C# (.NET 10) |
| **Browser automation** | [Microsoft Playwright for .NET](https://playwright.dev/dotnet/) |
| **API testing** | Playwright's `APIRequestContext` |
| **Test framework** | NUnit 4 (via `Microsoft.Playwright.NUnit`) |
| **Browsers** | Chromium, Firefox, WebKit |
| **JSON** | `System.Text.Json` (configuration and API bodies) |

---

## 📁 Project Structure

```text
TestAutomation/
│
├── Config/                        # Shared by UI and API tests
│   ├── appsettings.json           # Environments (UI and API URLs), default environment and timeout
│   └── TestSettings.cs            # Loads the settings once and resolves the selected environment
│
├── UI/                            # Swag Labs UI tests
│   ├── Base/
│   │   └── BaseTest.cs            # Base class for UI tests: base URL, timeout, "UI" category
│   ├── Pages/                     # Page Objects
│   │   ├── LoginPage.cs
│   │   ├── InventoryPage.cs
│   │   ├── CartPage.cs
│   │   ├── CheckoutStepOnePage.cs
│   │   ├── CheckoutStepTwoPage.cs
│   │   └── CheckoutCompletePage.cs
│   ├── TestData/
│   │   ├── Users.cs               # Sauce Demo users (standard, locked out, ...)
│   │   └── Products.cs            # Products with names and prices
│   └── Tests/
│       ├── LoginTests.cs                  # Login with valid and locked-out users
│       ├── InventoryTests.cs              # Adding and removing products, cart badge
│       ├── CartTests.cs                   # Cart contents, removing items, checkout
│       ├── CheckoutStepOneTests.cs        # Checkout information form, errors, back to cart
│       ├── CheckoutStepOneAccessTests.cs  # Access without login, problem and error users
│       ├── CheckoutStepTwoTests.cs        # Order overview: items, totals, finish, cancel
│       └── CheckoutStepTwoAccessTests.cs  # Access without login or customer info, error user
│
├── Api/                           # Restful-Booker API tests
│   ├── Base/
│   │   ├── ApiTest.cs             # Base class for API tests: request context, cleanup, "Api" category
│   │   └── ApiJson.cs             # JSON serialization and reading responses
│   ├── Clients/                   # One client per resource (the API counterpart of Pages)
│   │   ├── AuthClient.cs          # POST /auth
│   │   └── BookingClient.cs       # GET, POST, PUT, DELETE /booking
│   ├── Models/
│   │   ├── Auth.cs                # Credentials, AuthResponse
│   │   └── Booking.cs             # Booking, BookingDates, CreatedBooking, BookingId
│   ├── TestData/
│   │   ├── AdminCredentials.cs    # Valid and wrong admin credentials
│   │   └── BookingData.cs         # Valid bookings with unique names
│   └── Tests/
│       ├── AuthTests.cs           # Token with valid and wrong credentials
│       ├── GetBookingTests.cs     # Listing IDs, filtering by name, reading one booking
│       ├── CreateBookingTests.cs  # Creating bookings, invalid bodies
│       ├── UpdateBookingTests.cs  # Full update, with and without a token
│       └── DeleteBookingTests.cs  # Deleting, with and without a token
│
└── .runsettings                   # Browser, headless mode, slow motion, environment variables
```

---

## 🏗️ Architecture Highlights

### 🌍 Environment Resolution (`TestSettings.cs`)

Each environment has a UI URL and an API URL. The environment is chosen at run time, in this order:

1. The `env` runsettings parameter (passed on the command line or in `.runsettings`).
2. The `TEST_ENV` environment variable.
3. `DefaultEnvironment` from `appsettings.json`.

```csharp
public string UiBaseUrl => CurrentEnvironment.Ui;

public string ApiBaseUrl => CurrentEnvironment.Api;

private EnvironmentUrls CurrentEnvironment
{
    get
    {
        var env = TestContext.Parameters.Get("env")
                  ?? Environment.GetEnvironmentVariable("TEST_ENV")
                  ?? DefaultEnvironment;

        return Environments.TryGetValue(env, out var urls)
            ? urls
            : throw new ArgumentException(
                $"Unknown environment '{env}'. Valid values: {string.Join(", ", Environments.Keys)}");
    }
}
```

Environment names are not case-sensitive, and an unknown name fails right away with a message that lists the valid ones.

### 🔌 API Clients and Cleanup

A test talks to the API only through the clients and gets back the raw response, so it can check the status code and read the body as a typed record:

```csharp
[Test]
[Category("Positive"), Category("Smoke")]
[Description("GET /booking/{id} returns every detail of the booking")]
public async Task GetBooking_ReturnsAllDetails()
{
    var booking = BookingData.NewBooking();
    var created = await CreateBookingAsync(booking);

    var response = await BookingClient.GetBookingAsync(created.BookingId);

    Assert.That(response.Status, Is.EqualTo(200));
    Assert.That(await response.ReadJsonAsync<Booking>(), Is.EqualTo(booking));
}
```

`CreateBookingAsync` and `PostBookingAsync` in `ApiTest` remember every booking they create, and a `[TearDown]` deletes them with an admin token. Restful-Booker reads the token from a `Cookie: token=...` header, not from `Authorization`, and `BookingClient` adds it for update and delete calls.

### 🏷️ Test Categories

The layer category (`UI` or `Api`) is set once on the base class and inherited by every test. Feature categories are set once on the test class. Type, smoke and description are set on each test:

```csharp
[Category("Login")]
public class LoginTests : BaseTest   // BaseTest adds [Category("UI")]
{
    [Test]
    [Category("Negative")]
    [Description("Locked-out user sees an error and stays on the login page")]
    public async Task Login_WithLockedOutUser_ShowsErrorAndStaysOnLoginPage()
```

---

## 🚀 Getting Started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* PowerShell (to run the Playwright browser install script)

### Setup

```bash
git clone <repository-url>
cd <repository-folder>
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

The last command downloads the browsers Playwright needs. It only has to run once per machine. The API tests don't need a browser.

---

## 🏃 Running the Tests

### ▶️ All tests (default environment: `prod`)

```bash
dotnet test
```

### 🧩 UI or API only

```bash
dotnet test --filter "TestCategory=UI"
dotnet test --filter "TestCategory=Api"
```

The API tests take seconds, while the UI tests open a browser and take a few minutes.

### 🏷️ By category

```bash
dotnet test --filter "TestCategory=Smoke"
dotnet test --filter "TestCategory=Login"
dotnet test --filter "TestCategory=Api&TestCategory=Smoke"
dotnet test --filter "TestCategory=Cart&TestCategory=Positive"
dotnet test --filter "TestCategory!=Negative"
```

### 🐞 Without known bugs

The `KnownBug` tests fail on purpose (see [Known Bugs](#-known-bugs)). To get a run that only fails on new problems, leave them out:

```bash
dotnet test --filter "TestCategory!=KnownBug"
```

### 🌍 Environments

The environments are defined in `Config/appsettings.json`, each with a UI URL and an API URL:

```json
{
  "DefaultEnvironment": "prod",
  "DefaultTimeoutMs": 10000,
  "Environments": {
    "prod": {
      "Ui": "https://www.saucedemo.com",
      "Api": "https://restful-booker.herokuapp.com"
    },
    "stage": {
      "Ui": "https://stage.saucedemo.com",
      "Api": "https://stage.restful-booker.herokuapp.com"
    }
  }
}
```

Choose one with the `env` runsettings parameter or the `TEST_ENV` environment variable:

```bash
# runsettings parameter
dotnet test -- TestRunParameters.Parameter(name=\"env\", value=\"stage\")

# environment variable (PowerShell)
$env:TEST_ENV = "stage"; dotnet test
```

> ⚠️ Only `prod` points to live sites. `stage` and `demo` are placeholder URLs that show how more environments are added.

### 🌐 Browser options

By default the UI tests run in Chromium with the browser window visible and a 1-second delay between actions, so the run is easy to follow. Change the defaults in `.runsettings`, or override them for one run:

```bash
# Another browser
dotnet test -- Playwright.BrowserName=firefox

# Headless, at full speed (e.g. in CI)
dotnet test -- Playwright.LaunchOptions.Headless=true Playwright.LaunchOptions.SlowMo=0
```

### 🔒 HTTPS filters and proxies

Playwright sends API requests through Node.js, which by default trusts only its own list of certificate authorities. On a machine where HTTPS goes through a filter or proxy (such as NetSpark or a corporate proxy), every API test would fail with `self-signed certificate in certificate chain`. `.runsettings` sets `NODE_USE_SYSTEM_CA=1`, so Node also trusts the certificates installed in the operating system. It has no effect on machines without such a filter.

---

## 🐞 Known Bugs

Both applications have bugs: Swag Labs on purpose, and Restful-Booker returns several status codes that don't follow HTTP conventions. Each bug has a test that asserts the **correct** behavior, so the test fails while the bug exists and starts passing once it is fixed. These tests are tagged `KnownBug` and marked with a `// KNOWN BUG:` comment.

### Swag Labs (UI)

| Bug | Test |
|---|---|
| Checkout can be started from an empty cart | `CartTests.CheckoutWithEmptyCart_StaysOnCartPage` |
| `problem_user`: text typed in Last Name goes into First Name | `CheckoutStepOneAccessTests.FillInformation_FieldsKeepEnteredValues` |
| `error_user`: the Last Name field does not accept input | `CheckoutStepOneAccessTests.FillInformation_FieldsKeepEnteredValues` |
| `error_user`: Continue without a last name reaches checkout step two | `CheckoutStepOneAccessTests.ContinueWithoutLastName_AsErrorUser_DoesNotNavigateToOverview` |
| The checkout overview opens by URL without filling in the customer information | `CheckoutStepTwoAccessTests.OpenDirectlyWithoutCustomerInformation_RedirectsToStepOne` |
| `error_user`: Finish does nothing | `CheckoutStepTwoAccessTests.Finish_AsErrorUser_CompletesOrder` |

### Restful-Booker (API)

| Bug | Expected | Actual | Test |
|---|---|---|---|
| Login with a wrong password | 401 | 200 | `AuthTests.CreateToken_WithWrongPassword_Returns401` |
| Create with missing required fields | 400 | 500 | `CreateBookingTests.CreateBooking_WithMissingRequiredFields_Returns400` |
| Create with checkout before checkin | 400 | 200 | `CreateBookingTests.CreateBooking_WithCheckoutBeforeCheckin_Returns400` |
| Create with a negative total price | 400 | 200 | `CreateBookingTests.CreateBooking_WithNegativePrice_Returns400` |
| Update without a token | 401 | 403 | `UpdateBookingTests.UpdateBooking_WithoutToken_Returns401` |
| Update a booking that does not exist | 404 | 405 | `UpdateBookingTests.UpdateBooking_NonExistentId_Returns404` |
| Successful delete | 200 / 204 | 201 | `DeleteBookingTests.DeleteBooking_ReturnsOkOrNoContent` |
| Delete a booking that does not exist | 404 | 405 | `DeleteBookingTests.DeleteBooking_NonExistentId_Returns404` |

---

## ➕ Adding a Test

### UI

1. Put new locators and actions in the matching Page Object under `UI/Pages/`.
2. Write the test in the matching class under `UI/Tests/`, inheriting from `BaseTest`.

### API

1. Add new endpoints to the matching client under `Api/Clients/`, and new request or response shapes under `Api/Models/`.
2. Write the test in the matching class under `Api/Tests/`, inheriting from `ApiTest`.
3. Create bookings through `CreateBookingAsync` or `PostBookingAsync`, never by calling the client directly, so they are deleted after the test.

### Both

* Tag the test with `Positive` or `Negative`, add `Smoke` if it covers a critical path, and give it a short `[Description]`.
* For a known bug, assert the correct behavior, add the `KnownBug` category and a `// KNOWN BUG:` comment, and list it under [Known Bugs](#-known-bugs).

> 💡 Category names start with a capital letter (`Smoke`, not `smoke`). A different spelling creates a separate category that the filters above won't find.
