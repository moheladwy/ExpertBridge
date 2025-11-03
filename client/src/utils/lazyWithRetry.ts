import { lazy, ComponentType, LazyExoticComponent } from 'react';

// Type for the import function
type ComponentImport = () => Promise<{ default: ComponentType<any> }>;

// Cache for storing component promises for preloading
const componentCache = new Map<string, Promise<{ default: ComponentType<any> }>>();

/**
 * Enhanced lazy loading with retry logic for chunk load failures
 *
 * @param componentImport - Dynamic import function
 * @param componentName - Optional name for debugging and caching
 * @param retries - Number of retry attempts (default: 3)
 * @param delay - Delay between retries in ms (default: 1000)
 * @returns Lazy loaded component
 */
export function lazyWithRetry(
  componentImport: ComponentImport,
  componentName?: string,
  retries: number = 3,
  delay: number = 1000
): LazyExoticComponent<ComponentType<any>> {
  return lazy(() => retryImport(componentImport, componentName, retries, delay));
}

/**
 * Retry logic for dynamic imports
 */
async function retryImport(
  componentImport: ComponentImport,
  componentName?: string,
  retriesLeft: number = 3,
  delay: number = 1000
): Promise<{ default: ComponentType<any> }> {
  // Check cache first if component name is provided
  if (componentName && componentCache.has(componentName)) {
    try {
      return await componentCache.get(componentName)!;
    } catch (error) {
      // If cached import failed, remove from cache and continue
      componentCache.delete(componentName);
    }
  }

  try {
    const importPromise = componentImport();

    // Cache the promise if component name is provided
    if (componentName) {
      componentCache.set(componentName, importPromise);
    }

    const component = await importPromise;

    // Validate the imported module has a default export
    if (!component.default) {
      throw new Error(`Component ${componentName || 'unknown'} does not have a default export`);
    }

    return component;
  } catch (error: any) {
    // Check if it's a chunk loading error
    const isChunkLoadError =
      error.message?.includes('Loading chunk') ||
      error.message?.includes('Failed to fetch dynamically imported module') ||
      error.message?.includes('Loading CSS chunk') ||
      error.name === 'ChunkLoadError';

    console.error(
      `Failed to load component${componentName ? ` "${componentName}"` : ''}: ${error.message}`,
      `Retries left: ${retriesLeft}`
    );

    if (retriesLeft === 0) {
      // On final failure, add more context to the error
      error.componentName = componentName;
      error.isChunkLoadError = isChunkLoadError;

      // If it's a chunk error on final retry, suggest page reload
      if (isChunkLoadError) {
        console.error(
          'Chunk loading failed after all retries. User may need to refresh the page.'
        );
      }

      throw error;
    }

    // Wait before retrying
    await new Promise(resolve => setTimeout(resolve, delay));

    // For chunk load errors, try to refresh the page's chunks by clearing module cache
    if (isChunkLoadError && retriesLeft === 1) {
      // On second to last retry, try a different strategy
      // This is a last resort that might help with stale chunks
      if (typeof window !== 'undefined' && 'location' in window) {
        // Clear any service worker cache if present
        if ('caches' in window) {
          try {
            const cacheNames = await caches.keys();
            await Promise.all(
              cacheNames.map(cacheName => {
                if (cacheName.includes('chunk') || cacheName.includes('assets')) {
                  return caches.delete(cacheName);
                }
                return Promise.resolve();
              })
            );
          } catch (e) {
            console.error('Failed to clear caches:', e);
          }
        }
      }
    }

    // Exponential backoff for subsequent retries
    const nextDelay = delay * (4 - retriesLeft);

    // Retry with decremented counter and increased delay
    return retryImport(componentImport, componentName, retriesLeft - 1, nextDelay);
  }
}

/**
 * Preload a lazy component for faster subsequent loading
 *
 * @param componentImport - Dynamic import function
 * @param componentName - Optional name for caching
 * @returns Promise that resolves when component is loaded
 */
export function preloadComponent(
  componentImport: ComponentImport,
  componentName?: string
): Promise<void> {
  return retryImport(componentImport, componentName, 1, 0)
    .then(() => {
      console.log(`Preloaded component${componentName ? ` "${componentName}"` : ''}`);
    })
    .catch(error => {
      // Preload errors are non-critical, just log them
      console.warn(`Failed to preload component${componentName ? ` "${componentName}"` : ''}:`, error);
    });
}

/**
 * Create a lazy component with automatic preloading based on user interaction
 *
 * @param componentImport - Dynamic import function
 * @param componentName - Optional name for debugging
 * @param preloadOn - Event to trigger preloading ('hover' | 'focus' | 'visible')
 * @returns Object with lazy component and preload trigger
 */
export function lazyWithPreload(
  componentImport: ComponentImport,
  componentName?: string,
  preloadOn: 'hover' | 'focus' | 'visible' = 'hover'
) {
  const LazyComponent = lazyWithRetry(componentImport, componentName);

  const preload = () => preloadComponent(componentImport, componentName);

  const triggerProps = {
    hover: {
      onMouseEnter: preload,
      onTouchStart: preload,
    },
    focus: {
      onFocus: preload,
    },
    visible: {
      // Intersection observer would be set up separately
      'data-preload': 'visible',
    },
  };

  return {
    Component: LazyComponent,
    preload,
    triggerProps: triggerProps[preloadOn],
  };
}

/**
 * Clear the component cache
 * Useful for testing or when you want to force fresh imports
 */
export function clearComponentCache(): void {
  componentCache.clear();
  console.log('Component cache cleared');
}

/**
 * Check if a component is cached
 */
export function isComponentCached(componentName: string): boolean {
  return componentCache.has(componentName);
}
