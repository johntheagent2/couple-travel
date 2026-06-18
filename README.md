# Couple Travel

> *"A shared adventure RPG powered by real-world travel."*

A cozy, map-based travel scrapbook for two. Real trips become memories, visited cities appear as pins on a shared map, and the app grows into a living record of your adventures together. Built as a personal project — no sign-ups, no social feed, just the two of you.

---

## What it is

You and your partner share one adventure profile. Log a trip (city + dates + photos), and it appears on your map and timeline. The goal isn't a score — it's opening the app a year from now and seeing every city you've explored together.

**Phase 1 core loop:** log a trip → add photos → see your city on the map → scroll the scrapbook.

Future phases add venue-level check-ins, fog-of-war map reveal, collections ("Beaches of Vietnam 4/12"), badges, and AI-generated recaps.

---

## Stack

| Layer | Technology |
|---|---|
| Frontend | React + TypeScript + Vite, Tailwind CSS v4, Radix UI, Framer Motion, MapLibre GL JS |
| Backend | ASP.NET Core Web API (.NET 9, C#), modular feature-folder monolith |
| Database | PostgreSQL + PostGIS, EF Core + Npgsql + NetTopologySuite |
| Media | Cloudflare R2 / Azure Blob, presigned upload, thumbnails on upload |
| Geocoding | OpenStreetMap Nominatim (no API key required) |

---

## Getting started

### Prerequisites

- Node.js 20+
- .NET 9 SDK
- PostgreSQL 15+ with PostGIS extension
- Docker (optional, recommended)

### Database

```bash
createdb coupletravel
psql -d coupletravel -c "CREATE EXTENSION IF NOT EXISTS postgis;"
dotnet ef database update --project backend/CoupleTravel.Api
```

### Backend

```bash
cd backend
dotnet restore
dotnet run --project CoupleTravel.Api
# Dev server runs on http://localhost:5052
```

### Frontend

```bash
cd frontend
npm install
npm run dev
# Vite dev server proxies /v1 → http://localhost:5052
```

---

## Design constraints

These are deliberate decisions, not gaps:

- **Two users only.** No sign-up flow. Two accounts are seeded; reset passwords directly in the DB (store a bcrypt hash, not plaintext).
- **No realtime.** Plain request/response. No WebSockets, no push notifications.
- **Manual check-ins only.** No background GPS or geofencing — you tap to log a trip.
- **No AI in Phase 1.** AI captions and recaps are Phase 2+.
- **No XP or levels.** Progression is collections + map coverage + milestone badges, not a score to grind.
- **Polish is a feature.** Warm palette, soft edges, celebratory micro-animations. No gray dashboards.

---

## Project structure

```
backend/
  CoupleTravel.Api/
    Features/          # Auth, Trips, Cities, Media, ...
    Infrastructure/    # EF Core, migrations, blob storage
frontend/
  src/
    features/          # trips, map, auth, ...
    components/        # shared UI
    routes/
Documents/             # design docs (source of truth)
docker-compose.yml
```

See [`Documents/local-directory-structure.md`](Documents/local-directory-structure.md) for the full intended layout.

---

## Design documents

Read these before implementing anything non-trivial:

| Doc | Purpose |
|---|---|
| [`local-phase1-scope.md`](Documents/local-phase1-scope.md) | **Start here.** Trimmed scope for the initial release — governs what ships first. |
| [`local-PLAN.md`](Documents/local-PLAN.md) | Full product & engineering plan — long-term vision, data model, roadmap. |
| [`local-location-capture.md`](Documents/local-location-capture.md) | How city coordinates are sourced (Nominatim geocode flow). |
| [`local-fog-of-war.md`](Documents/local-fog-of-war.md) | Fog-of-war deep dive — deferred to Phase 2+. |
| [`local-directory-structure.md`](Documents/local-directory-structure.md) | Proposed directory layout for the full implementation. |

---

## Phase 1 API

```
POST   /v1/auth/login
POST   /v1/auth/logout
GET    /v1/auth/me

GET    /v1/cities/search?q=        # geocode proxy (Nominatim) + cache
POST   /v1/trips                   # create trip; idempotent via client_uuid
GET    /v1/trips?cursor=           # timeline, reverse-chronological, cursor-paginated
GET    /v1/trips/{id}
PUT    /v1/trips/{id}
DELETE /v1/trips/{id}              # soft delete

POST   /v1/trips/{id}/photos       # presigned upload → blob store
DELETE /v1/photos/{id}

GET    /v1/map/cities              # [{cityId, name, lat, lng, tripCount}]
```

All endpoints require cookie auth. API is versioned at `/v1`.
