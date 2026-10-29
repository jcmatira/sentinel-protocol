# Security Policy

## Secrets are never committed

This is a **public** repository. It must never contain credentials of any kind.

Blocked by `.gitignore`:

- `.env` / `.env.*`
- private keys: `*.pem`, `*.key`, `*.p12`, `*.pfx`, `*.keystore`, `*.jks`,
  `id_rsa*`, `id_ed25519*`
- token files: `*.token`, `*.secret`, `credentials*.json`, `secrets*.json`,
  `serviceAccount*.json`
- package auth: `.npmrc`, `.pypirc`, `.netrc`
- logs: `*.log` (Unity logs can contain session access tokens)
- Unity `Library/`, `Temp/`, `Logs/`, `UserSettings/` and build output

If you add a new config file that contains a secret, add its name to
`.gitignore` **before** staging it.

## Before every push

```bash
# list anything that would be committed
git status --short

# scan staged files for common secret patterns
git grep --cached -nE "ghp_|github_pat_|AKIA[0-9A-Z]{16}|BEGIN .*PRIVATE KEY|api[_-]?key|password\s*[:=]"
```

If that scan returns anything real, remove it from history (rotate the secret
first, then rewrite history) — deleting the file in a later commit is **not**
enough on a public repo.

## What is *not* a secret in this project

`ProjectSettings/ProjectSettings.asset` contains a Unity Cloud
`cloudProjectId` and `organizationId`. These are identifiers used by Unity
Services, not credentials. They are safe to publish, but do not add service
tokens or API keys to the project.

## Reporting

If you find a leaked credential or another security issue, do not open a public
issue with the value. Contact the repository owner privately and rotate the
credential immediately.

## Contributor rules

- Never commit tokens, passwords, private keys or `.env` files.
- Never paste a token into an issue, PR, commit message or chat.
- Use environment variables or the Unity/OS credential store for local secrets.
