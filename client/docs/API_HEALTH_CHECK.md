# API Health Check System

This document describes the API health check system implemented in the ExpertBridge client application.

## Overview

The API health check system monitors the backend API's availability on application startup and automatically redirects users to an error page when the API is unhealthy. It provides startup health validation, manual retry options, and user-friendly error handling.

## Architecture

### Components

1. **`useApiHealth` Hook** (`src/hooks/useApiHealth.ts`)
   - Core hook that manages health check logic
   - Performs periodic health checks
   - Handles retries and error states
   - Provides health status and control methods

2. **`ApiHealthGuard` Component** (`src/views/components/common/ui/ApiHealthGuard.tsx`)
   - Wrapper component that protects the entire application
   - Automatically shows error page when API is unhealthy
   - Handles recovery when API becomes healthy again

3. **`ApiHealthErrorPage` Component** (`src/views/pages/error/ApiHealthErrorPage.tsx`)
   - User-friendly error page displayed when API is unavailable
   - Provides manual retry functionality
   - Auto-retry mechanism with exponential backoff

4. **`ApiHealthIndicator` Component** (`src/views/components/common/ui/ApiHealthIndicator.tsx`)
   - Optional visual indicator for API health status
   - Can be embedded in navigation or status bars
   - Supports different sizes and display modes

## How It Works

### Health Check Process

1. **Startup Check**: On application load, the system performs a single health check
2. **Error Detection**: If health check fails, shows the error page
3. **Manual Retry**: Users can manually retry the health check from the error page
4. **Recovery**: When API becomes healthy again, automatically returns to normal operation

### Health Check Endpoint

The system calls the `/health` endpoint configured in `src/lib/api/endpoints.ts`:

```typescript
export const HEALTH_CHECK_URL = `${BASE_URL}/health`;
```

Expected response format:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.4354885",
  "entries": {
    "self": {
      "data": {},
      "duration": "00:00:00.0224349",
      "status": "Healthy",
      "tags": ["live"]
    },
    "npgsql": {
      "data": {},
      "duration": "00:00:00.2283011",
      "status": "Healthy",
      "tags": ["live"]
    },
    "Redis": {
      "data": {},
      "duration": "00:00:00.1936799",
      "status": "Healthy",
      "tags": ["live"]
    },
    "Seq": {
      "data": {},
      "duration": "00:00:00.1570651",
      "status": "Healthy",
      "tags": []
    }
  }
}
```

Health check evaluation:
- **Healthy**: HTTP 200 status code AND `status` field equals "Healthy"
- **Unhealthy**: Any other status code, network error, or `status` field not "Healthy"

### Integration Points

The system is integrated at the application root level:

```typescript
// src/App.tsx
function App() {
  return (
    <AuthPromptProvider>
      <ApiHealthGuard>
        <AppContent />
      </ApiHealthGuard>
    </AuthPromptProvider>
  );
}
```

## Configuration

### useApiHealth Hook Options

```typescript
interface UseApiHealthOptions {
  checkOnStartup?: boolean;  // Check on startup (default: true)
  maxRetries?: number;       // Max retry attempts (default: 3)
  enabled?: boolean;         // Enable/disable health checking (default: true)
  onHealthChange?: (isHealthy: boolean) => void; // Health change callback
}
```

### Example Usage

```typescript
const {
  isHealthy,
  isLoading,
  error,
  lastChecked,
  retryCount,
  checkHealth,
  retryHealthCheck,
  resetHealth,
  canRetry
} = useApiHealth({
  checkOnStartup: true,  // Check on startup
  maxRetries: 5,         // Allow 5 retries
  enabled: true,         // Enable monitoring
  onHealthChange: (healthy) => {
    console.log(`API is now ${healthy ? 'healthy' : 'unhealthy'}`);
  }
});
```

## API Reference

### useApiHealth Hook

#### Returns

- `isHealthy: boolean` - Current health status
- `isLoading: boolean` - Whether a health check is in progress
- `error: string | null` - Last error message
- `lastChecked: Date | null` - Timestamp of last health check
- `retryCount: number` - Number of consecutive failures
- `healthData: HealthCheckResponse | null` - Detailed health check data from API
- `checkHealth: () => Promise<boolean>` - Manually trigger health check
- `retryHealthCheck: () => Promise<void>` - Retry with backoff logic
- `resetHealth: () => void` - Reset to healthy state
- `canRetry: boolean` - Whether retry is available

### ApiHealthIndicator Props

```typescript
interface ApiHealthIndicatorProps {
  className?: string;           // Additional CSS classes
  showLabel?: boolean;         // Show status text (default: false)
  size?: 'sm' | 'md' | 'lg';  // Icon size (default: 'sm')
}
```

### ApiHealthErrorPage Props

```typescript
interface ApiHealthErrorPageProps {
  onRetry?: () => void;        // Retry callback
  isRetrying?: boolean;        // Show retry loading state
}
```

## Usage Examples

### Basic Implementation

```typescript
// Wrap your app with the health guard
function App() {
  return (
    <ApiHealthGuard>
      <YourAppContent />
    </ApiHealthGuard>
  );
}
```

### Custom Health Monitoring

```typescript
function CustomHealthMonitor() {
  const { isHealthy, error, healthData, checkHealth } = useApiHealth({
    checkOnStartup: true,
    maxRetries: 3,
    onHealthChange: (healthy) => {
      if (!healthy) {
        // Custom error handling
        showNotification('API is experiencing issues');
      }
    }
  });

  return (
    <div>
      <button onClick={checkHealth}>Check API Health</button>
      <span>Status: {isHealthy ? 'Healthy' : 'Unhealthy'}</span>
      {error && <p>Error: {error}</p>}
      {healthData && (
        <div>
          <p>Total Duration: {healthData.totalDuration}</p>
          <ul>
            {Object.entries(healthData.entries).map(([service, details]) => (
              <li key={service}>
                {service}: {details.status} ({details.duration})
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
```

### Navigation Bar Integration

```typescript
function NavBar() {
  return (
    <nav>
      {/* Other nav items */}
      <ApiHealthIndicator showLabel size="sm" />
    </nav>
  );
}
```

## Error Handling

### Automatic Error Page

When the API is unhealthy on startup, users are automatically shown a friendly error page that:

- Explains the situation clearly with service status details
- Provides manual retry options
- Shows retry attempt counts
- Auto-retries for the first 3 attempts (with 30-second intervals)
- Includes helpful troubleshooting information
- Displays detailed service health information (database, Redis, etc.)

### Custom Error Handling

You can implement custom error handling by using the `onHealthChange` callback:

```typescript
const { isHealthy } = useApiHealth({
  onHealthChange: (healthy) => {
    if (!healthy) {
      // Custom logic for API failures
      analytics.track('api_health_failed');
      showCustomErrorModal();
    } else {
      // Custom logic for API recovery
      analytics.track('api_health_recovered');
      hideCustomErrorModal();
    }
  }
});
```

## Testing

### Demo Page

A demo page is available at `/api-health-demo` that shows:

- Current API health status with detailed service information
- Manual health check controls
- Different indicator component variations
- Health check response data (status, duration, service details)
- Configuration information

### Manual Testing

1. **Healthy State**: Start app with backend running - should load normally
2. **Unhealthy State**: Start app with backend stopped - should show error page
3. **Recovery**: Click retry on error page after starting backend
4. **Service Details**: Check that individual service status is displayed (database, Redis, etc.)
5. **Indicators**: Verify health status indicators show correct state

## Troubleshooting

### Common Issues

1. **False Positives**
   - Check network connectivity
   - Verify health endpoint URL configuration
   - Ensure proper CORS setup

2. **Health Check Timeout**
   - Default timeout is 10 seconds
   - Increase if backend response is slow
   - Check for network latency issues

3. **Parsing Errors**
   - Ensure backend returns proper JSON format
   - Verify `status` field is "Healthy" when all services are up
   - Check that all required fields (`status`, `totalDuration`, `entries`) are present

### Configuration Adjustments

```typescript
// For production - default configuration
const healthConfig = {
  checkOnStartup: true,   // Check once on startup
  maxRetries: 5,          // Allow manual retries
  enabled: true
};

// For development - disable to avoid errors during dev
const healthConfig = {
  checkOnStartup: false,  // Don't check in development
  maxRetries: 1,
  enabled: process.env.NODE_ENV === 'production'
};
```

## Best Practices

1. **Production Configuration**
   - Keep startup-only checking to avoid unnecessary API calls
   - Allow multiple manual retries for users
   - Implement proper error tracking and analytics
   - Display detailed service status to help diagnose issues

2. **Development**
   - Consider disabling in development environment (set `enabled: false`)
   - Use demo page at `/api-health-demo` for testing
   - Monitor console logs for debugging

3. **User Experience**
   - Provide clear error messages with service details
   - Offer manual retry options prominently
   - Show loading states during checks
   - Display which specific services are failing (database, Redis, etc.)

4. **Performance**
   - Startup-only checking minimizes API load
   - 10-second timeout prevents long waits
   - Manual retry gives users control

## Environment Variables

Ensure these environment variables are properly configured:

- `VITE_SERVER_URL`: Base URL for the backend API
- The health endpoint will be constructed as `${VITE_SERVER_URL}/health`

## Future Enhancements

Potential improvements to consider:

1. **Periodic Background Checks**: Optional periodic checks after successful startup
2. **Service-Specific Status**: Show which individual services are failing
3. **Service Worker Integration**: Offline detection and handling
4. **Analytics Integration**: Track health metrics and user impact
5. **Degraded Mode**: Partial functionality when specific services are unavailable
6. **Health History**: Track and display health check history over time