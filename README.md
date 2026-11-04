# 🧪 Swag Labs – UI Test Automation with Playwright & C#

End-to-end UI tests for [Swag Labs](https://www.saucedemo.com), Sauce Labs' demo e-commerce site, written in **C#** with **Playwright** and **NUnit** and following the **Page Object Model (POM)** design pattern.

The project covers the main user flows (logging in, adding products to the cart, managing the cart and checking out, from the customer information form to the order overview and completion) and shows how a maintainable test framework can be set up: Page Object Model, separate test data, configuration for several environments and test categories.

---

## 🌟 Key Features

* **Page Object Model (POM):** Each page (`LoginPage`, `InventoryPage`, `CartPage`, `CheckoutStepOnePage`, `CheckoutStepTwoPage`, `CheckoutCompletePage`) holds its locators and actions, so tests read like user steps and a UI change is fixed in one place.
* **Stable locators:** Elements are located by their `data-test` attributes instead of CSS classes or text, which keeps tests from breaking when the styling changes.
* **Multi-environment support:** Base URLs live in `Config/appsettings.json`, and the same tests run against any environment chosen at run time, with no code changes (see [Environments](#-environments)).
* **Separate test data:** Users and products are typed C# records in `TestData/`, not strings scattered through the tests.
* **Data-driven tests:** The valid-login test runs once for every user who is allowed to log in, using `[TestCaseSource]`, and the missing-field checkout tests use `[TestCase]`.
* **Test categories:** Every test is tagged by feature (`Login`, `Inventory`, `Cart`, `Checkout`), by type (`Positive` / `Negative`) and, for critical-path tests, `Smoke`, so any subset can be run on its own.
* **Known bugs as failing tests:** Tests for known site bugs assert the correct behavior and are tagged `KnownBug`, so they fail until the bug is fixed (see [Known Bugs](#-known-bugs)).
* **Shared base test:** `BaseTest` builds on Playwright's `PageTest`, which creates a fresh browser context and page for every test, and applies the selected environment's base URL and the default timeout.
* **Browser configured in one file:** The browser, headless mode and slow-motion delay are set in `.runsettings` and can be overridden from the command line.

---

## 🛠️ Tech Stack

| | |
|---|---|
| **Language** | C# (.NET 10) |
| **Browser automation** | [Microsoft Playwright for .NET](https://playwright.dev/dotnet/) |
| **Test framework** | NUnit 4 (via `Microsoft.Playwright.NUnit`) |
| **Browsers** | Chromium, Firefox, WebKit |
| **Configuration** | JSON (`appsettings.json`) read with `System.Text.Json` |

---

## 📁 Project Structure

```text
Swag Labs/
│
├── Base/
│   └── BaseTest.cs          # Base class for all tests: base URL and timeout
│
├── Config/
│   ├── appsettings.json     # Environments, default environment and timeout
│   └── TestSettings.cs      # Loads the settings once and resolves the selected environment
│
├── Pages/                   # Page Objects
│   ├── LoginPage.cs
│   ├── InventoryPage.cs
│   ├── CartPage.cs
│   ├── CheckoutStepOnePage.cs
│   ├── CheckoutStepTwoPage.cs
│   └── CheckoutCompletePage.cs
│
├── TestData/
│   ├── Users.cs             # Sauce Demo users (standard, locked out, ...)
│   └── Products.cs          # Products with names and prices
│
├── Tests/
│   ├── LoginTests.cs        # Login with valid and locked-out users
│   ├── InventoryTests.cs    # Adding and removing products, cart badge
│   ├── CartTests.cs         # Cart contents, removing items, checkout
│   ├── CheckoutStepOneTests.cs        # Checkout information form, errors, back to cart
│   ├── CheckoutStepOneAccessTests.cs  # Access without login, problem and error users
│   ├── CheckoutStepTwoTests.cs        # Order overview: items, totals, finish, cancel
│   └── CheckoutStepTwoAccessTests.cs  # Access without login or customer info, error user
│
└── .runsettings             # Browser, headless mode, slow motion
```

---

## 🏗️ Architecture Highlights

### 🌍 Environment Resolution (`TestSettings.cs`)

The base URL is resolved at run time, in this order:

1. The `env` runsettings parameter (passed on the command line or in `.runsettings`).
2. The `TEST_ENV` environment variable.
3. `DefaultEnvironment` from `appsettings.json`.

```csharp
public string BaseUrl
{
    get
    {
        var env = TestContext.Parameters.Get("env")
                  ?? Environment.GetEnvironmentVariable("TEST_ENV")
                  ?? DefaultEnvironment;

        return Environments.TryGetValue(env, out var url)
            ? url
            : throw new ArgumentException(
                $"Unknown environment '{env}'. Valid values: {string.Join(", ", Environments.Keys)}");
    }
}
```

Environment names are not case-sensitive, and an unknown name fails right away with a message that lists the valid ones.

### 🏷️ Test Categories

Feature categories are set once on the test class. Type, smoke and description are set on each test:

```csharp
[Category("Login")]
public class LoginTests : BaseTest
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
cd "Swag Labs"
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

The last command downloads the browsers Playwright needs. It only has to run once per machine.

---

## 🏃 Running the Tests

### ▶️ All tests (default environment: `prod`)

```bash
dotnet test
```

### 🏷️ By category

```bash
dotnet test --filter "TestCategory=Smoke"
dotnet test --filter "TestCategory=Login"
dotnet test --filter "TestCategory=Cart&TestCategory=Positive"
dotnet test --filter "TestCategory!=Negative"
```

### 🐞 Without known bugs

The `KnownBug` tests fail on purpose (see [Known Bugs](#-known-bugs)). To get a run that only fails on new problems, leave them out:

```bash
dotnet test --filter "TestCategory!=KnownBug"
```

### 🌍 Environments

The environments are defined in `Config/appsettings.json`:

```json
{
  "DefaultEnvironment": "prod",
  "DefaultTimeoutMs": 10000,
  "Environments": {
    "prod": "https://www.saucedemo.com",
    "stage": "https://stage.saucedemo.com",
    "demo": "https://demo.saucedemo.com"
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

> ⚠️ Only `prod` points to a live site. `stage` and `demo` are placeholder URLs that show how more environments are added.

### 🌐 Browser options

By default the tests run in Chromium with the browser window visible and a 1-second delay between actions, so the run is easy to follow. Change the defaults in `.runsettings`, or override them for one run:

```bash
# Another browser
dotnet test -- Playwright.BrowserName=firefox

# Headless, at full speed (e.g. in CI)
dotnet test -- Playwright.LaunchOptions.Headless=true Playwright.LaunchOptions.SlowMo=0
```

---

## 🐞 Known Bugs

Swag Labs has bugs on purpose. Each one has a test that asserts the **correct** behavior, so the test fails while the bug exists and starts passing once it is fixed. These tests are tagged `KnownBug` and marked with a `// KNOWN BUG:` comment.

| Bug | Test |
|---|---|
| Checkout can be started from an empty cart | `CartTests.CheckoutWithEmptyCart_StaysOnCartPage` |
| `problem_user`: text typed in Last Name goes into First Name | `CheckoutStepOneAccessTests.FillInformation_FieldsKeepEnteredValues` |
| `error_user`: the Last Name field does not accept input | `CheckoutStepOneAccessTests.FillInformation_FieldsKeepEnteredValues` |
| `error_user`: Continue without a last name reaches checkout step two | `CheckoutStepOneAccessTests.ContinueWithoutLastName_AsErrorUser_DoesNotNavigateToOverview` |
| The checkout overview opens by URL without filling in the customer information | `CheckoutStepTwoAccessTests.OpenDirectlyWithoutCustomerInformation_RedirectsToStepOne` |
| `error_user`: Finish does nothing | `CheckoutStepTwoAccessTests.Finish_AsErrorUser_CompletesOrder` |

---

## ➕ Adding a Test

1. Put new locators and actions in the matching Page Object under `Pages/`.
2. Write the test in the matching class under `Tests/`, inheriting from `BaseTest`.
3. Tag it with `Positive` or `Negative`, add `Smoke` if it covers a critical path, and give it a short `[Description]`.
4. For a known site bug, assert the correct behavior, add the `KnownBug` category and a `// KNOWN BUG:` comment, and list it under [Known Bugs](#-known-bugs).

> 💡 Category names start with a capital letter (`Smoke`, not `smoke`). A different spelling creates a separate category that the filters above won't find.
