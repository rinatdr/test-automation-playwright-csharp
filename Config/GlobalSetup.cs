// No namespace: an NUnit SetUpFixture outside any namespace runs once before every test in the assembly.
[SetUpFixture]
public class GlobalSetup
{
    // Playwright sends API requests from a Node.js process, which by default trusts only its own
    // certificate list. A filter or proxy that re-signs HTTPS (such as NetSpark) then fails with
    // "self-signed certificate in certificate chain". This makes Node.js also trust the Windows
    // certificate store. It is set in code, not in .runsettings, because VS Code's Test Explorer
    // does not load .runsettings. The Node.js process inherits it when Playwright starts it.
    [OneTimeSetUp]
    public void TrustSystemCertificates()
    {
        if (Environment.GetEnvironmentVariable("NODE_USE_SYSTEM_CA") is null)
            Environment.SetEnvironmentVariable("NODE_USE_SYSTEM_CA", "1");
    }
}
