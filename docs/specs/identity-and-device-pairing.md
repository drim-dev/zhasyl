# Identity and Device Pairing

Adults own family access to Station Zhasyl-1. Children use a named profile and a paired browser
without receiving an email account or password. The Next.js application is the public identity
boundary; the ASP.NET Core API remains private.

## Actors

| Actor | Authentication | Current capabilities |
|---|---|---|
| Adult | Auth.js social session | Create child profiles, issue pairing codes, view and revoke paired devices |
| Child | Opaque device session in an HttpOnly cookie | Confirm the active child profile |
| Anonymous browser | None | Read published content and exchange a valid pairing code |

An adult account may own multiple child profiles. A child profile has a stable UUID, a localized
display name, and an explicit learning locale. It has no email address or password.

## Adult Sign-In

Auth.js provides the public OAuth flow. Google, GitHub, and GitLab providers are enabled only when
their matching `AUTH_*_ID` and `AUTH_*_SECRET` variables are configured. A local credentials
provider is available outside production so the family journey can be tested without creating an
OAuth application.

After provider authentication, the BFF registers or resolves the adult through the private
`POST /api/auth/oauth-sign-in` endpoint. Provider and provider subject form the stable external
identity. Repeated sign-in is idempotent. The Auth.js JWT stores only the resulting adult UUID and
standard session claims.

The backend does not link a second provider merely because it reports the same email address.
Such a sign-in returns a conflict until an authenticated account-linking flow is implemented;
this prevents an unverified or recycled provider email from taking over an existing family.

The private API currently trusts `X-Adult-Id` and `X-Adult-Email` headers supplied by the BFF.
These headers are not a public authentication protocol and the API must remain unreachable from
the public network.

## Child Pairing

1. The signed-in adult creates a child profile.
2. The adult requests a pairing code for that profile.
3. The API returns a random eight-character code displayed as `XXXX-XXXX`.
4. The child enters the code and a device label on `/connect`.
5. The BFF exchanges the code for an opaque device token and stores it in the
   `zhasyl.child-session` HttpOnly, SameSite=Lax cookie.
6. Subsequent child requests can resolve the paired profile through the private API.

A pairing code expires after ten minutes and succeeds only once. The database stores only its
SHA-256 hash. Pairing attempts are rate-limited by the source address observed and forwarded by
the private BFF.

A child device session expires after ninety days. The browser receives the raw token once; the
database stores only its SHA-256 hash. An adult can revoke a device immediately, after which the
application treats that browser as unpaired the next time it resolves the session.

## API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/oauth-sign-in` | Private BFF | Resolve or create an adult from an OAuth identity |
| GET | `/api/adult/children` | Adult | List owned child profiles and active devices |
| POST | `/api/adult/children` | Adult | Create a child profile |
| POST | `/api/adult/children/{childId}/pairing-codes` | Adult | Issue a one-use pairing code |
| DELETE | `/api/adult/children/{childId}/devices/{deviceId}` | Adult | Revoke one child device session |
| POST | `/api/child/pair` | Anonymous, rate-limited | Exchange a pairing code for a device token |
| GET | `/api/child/session` | Child device | Resolve the active child profile |

Validation and application failures use RFC 7807 Problem Details with a stable `errorCode`.
Ownership is checked for every adult mutation; UUID knowledge alone never grants access.

## Deliberate Boundaries

This slice does not add classes, teacher accounts, child passwords, account recovery, invitations,
production OAuth applications, consent records, or administrative account management. Workspace
persistence will attach learning state to the resolved child profile rather than to a browser or
pairing code.
