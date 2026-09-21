# Learning Roadmap

## Confirmed Starting Point

- **Application security:** advanced hands-on experience with Invicti/Netsparker, Qualys, WebInspect, Postman, SonarQube, JFrog Xray, and Burp Suite validation.
- **New technology areas:** C#, ASP.NET Core, Azure, Azure DevOps YAML, and OAuth/OIDC.
- **Access:** no Azure subscription or Azure DevOps organization yet.
- **Plan:** free and local first, completed over 8-12 weeks.
- **Environment:** .NET 10, Git, Docker tooling, Node.js, and npm are available; Azure CLI is not installed.

We will anchor new platform concepts to the familiar vulnerability lifecycle: identify the trust boundary, reproduce the weakness, understand the owning code, implement the control, and retain regression evidence. All data and findings remain synthetic.

## Week 1 - Local Secure Baseline (Complete)

**Learn:** ASP.NET request flow, secure defaults, compiler analysis, SCA, and CI evidence. **Deliver:** health endpoint, integration test, NuGet audit, and Azure Pipeline definition. **Depends on:** .NET only. **Accept:** clean restore, build, and test; health output contains no component details.

## Weeks 2-3 - C# PIM API and STRIDE (Complete)

**Learn:** essential C# syntax, dependency injection, HTTP endpoints, validation, data flow diagrams, and STRIDE. **Deliver:** an in-memory synthetic product API, its threat model, abuse cases, and security tests. **Depends on:** Phase 1. **Accept:** invalid input is rejected consistently and each high-risk threat has a testable requirement, mitigation, and owner. Product routes remain development-only until Phase 3 supplies authentication.

## Weeks 4-5 - OAuth/OIDC and Tenant Isolation (Local Implementation Complete)

**Learn:** authorization code flow, access tokens, issuer, audience, scopes, claims, and policy-based authorization. **Deliver:** JWT-protected product endpoints, claim-derived tenant filtering, and subject-aware audit events. **Depends on:** a free Auth0 test tenant only for live-provider activation. **Accept:** missing or invalid issuer, signature, audience, expiry, scope, or tenant claims cannot access product data; local negative tests pass. Live Auth0 metadata retrieval remains pending account setup.

## Weeks 6-7 - CI/CD Security and Vulnerability Workflow (SonarQube Workflow Added)

**Learn:** GitHub Actions and Azure DevOps YAML, pull-request validation, pipeline permissions, SAST/SCA tuning, triage, exceptions, and artifact integrity. **Deliver:** SonarQube analysis on GitHub, local equivalents, then a running Azure Pipeline after a free organization is available. **Accept:** the SonarQube quality gate blocks failing changes, pull requests cannot deploy, and time-limited exceptions require human approval. The GitHub workflow is implemented; its first hosted run awaits repository and SonarQube Cloud connection.

## Weeks 8-9 - Azure Security Foundation

**Learn:** infrastructure as code, workload identity, Key Vault, RBAC, logging, and Defender for Cloud. **Deliver:** locally validated infrastructure definitions followed by an isolated Azure deployment when access is available. **Depends on:** an Azure subscription and explicit cost review. **Accept:** source and pipelines contain no secrets; workload identity has only required access; Defender findings are triaged.

No Azure resource will be provisioned until its region, tier, free allowance, and maximum expected monthly cost are documented. App hosting, Key Vault operations, logging, databases, and Defender plans can all create charges.

## Week 10 - AI/LLM Security

**Learn:** prompt injection, sensitive-data exposure, unsafe tool use, and output validation. **Deliver:** a constrained review assistant using sanitized inputs and read-only tools. **Accept:** hostile repository text cannot change policy, reveal secrets, approve exceptions, or trigger deployment.

## Weeks 11-12 - Portfolio Cases and Production Review

**Learn:** communicating root cause, business impact, proof, remediation, and residual risk. **Deliver:** clearly labeled lab vulnerability stories and a production gap assessment. **Accept:** each case includes reproduction evidence, owner, fix, regression result, and closure rationale; production gaps cover monitoring, incident response, backup, availability, privacy, and supply-chain controls.

Portfolio evidence is collected throughout every phase rather than postponed until the end.
