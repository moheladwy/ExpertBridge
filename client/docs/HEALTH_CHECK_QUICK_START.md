# API Health Check - Quick Start Guide

## 🚀 Overview

The client now automatically checks if the API is healthy on startup. If the API is down, users see a friendly error page instead of a broken application.

## ✅ What Happens

### When API is Healthy ✓
- Application loads normally
- Users can use all features
- No interruption to experience

### When API is Down ✗
- Professional error page is displayed
- Shows which services are failing (database, Redis, etc.)
- Provides retry button for users
- Auto-retries first 3 times (every 30 seconds)

## 🔍 How to Test

### Test 1: Healthy API
```bash
# 1. Start your backend server
# 2. Start the client
npm run dev
# 3. Navigate to http://localhost:5173
# Expected: App loads normally
```

### Test 2: Unhealthy API
```bash
# 1. Stop your backend server
# 2. Start the client
npm run dev
# 3. Navigate to http://localhost:5173
# Expected: Error page appears showing service statuses
```

### Test 3: Recovery
```bash
# 1. From the error page (with backend stopped)
# 2. Start your backend server
# 3. Click "Try Again" button on error page
# Expected: App returns to normal operation
```

### Test 4: Demo Page
```bash
# Navigate to: http://localhost:5173/api-health-demo
# This shows all health check features and controls
```

## 📋 API Endpoint

The health check calls:
```
GET ${VITE_SERVER_URL}/health
```

Expected response when healthy:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.4354885",
  "entries": {
    "self": { "status": "Healthy", "duration": "00:00:00.0224349", ... },
    "npgsql": { "status": "Healthy", "duration": "00:00:00.2283011", ... },
    "Redis": { "status": "Healthy", "duration": "00:00:00.1936799", ... },
    "Seq": { "status": "Healthy", "duration": "00:00:00.1570651", ... }
  }
}
```

## 🎯 Key Features

- ✅ **Startup Check Only** - Checks once when app loads (no periodic polling)
- ✅ **Detailed Status** - Shows which specific services are failing
- ✅ **Manual Retry** - Users can retry anytime with a button
- ✅ **Auto-Retry** - First 3 failures auto-retry every 30 seconds
- ✅ **Dark Mode** - Error page supports dark/light themes
- ✅ **10 Second Timeout** - Won't hang indefinitely

## ⚙️ Configuration

Current settings (in `ApiHealthGuard.tsx`):
```typescript
{
  checkOnStartup: true,  // Check once on app startup
  maxRetries: 5,         // Allow up to 5 manual retries
  enabled: true,         // Always enabled
}
```

## 🛠️ Disable for Development (Optional)

If you want to skip health checks during local development:

1. Open `client/src/views/components/common/ui/ApiHealthGuard.tsx`
2. Change the `enabled` option:
```typescript
const { isHealthy, ... } = useApiHealth({
  checkOnStartup: true,
  maxRetries: 5,
  enabled: import.meta.env.PROD, // Only check in production
  ...
});
```

## 📂 Important Files

- `src/hooks/useApiHealth.ts` - Core health check logic
- `src/views/components/common/ui/ApiHealthGuard.tsx` - Wrapper that protects the app
- `src/views/pages/error/ApiHealthErrorPage.tsx` - Error page shown when API is down
- `src/views/pages/test/ApiHealthDemo.tsx` - Demo page at `/api-health-demo`
- `src/lib/api/endpoints.ts` - Health endpoint URL configuration

## 📚 Full Documentation

For complete documentation, see: `docs/API_HEALTH_CHECK.md`

## 🎉 That's It!

The health check is already integrated and working. Just make sure your backend's `/health` endpoint returns the expected format!