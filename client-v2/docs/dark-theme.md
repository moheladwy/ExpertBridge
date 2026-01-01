# Dark Theme Implementation

## Overview

Dark theme has been implemented in client-v2 using `next-themes` with a toggle button component.

## Features

- ✅ **Light Mode** - Bright theme for daytime use
- ✅ **Dark Mode** - Dark theme for low-light environments
- ✅ **System Mode** - Automatically matches OS preference
- ✅ **Persistent** - Theme choice saved to localStorage
- ✅ **SSR-Safe** - No flash of unstyled content on page load

## Implementation Details

### 1. Theme Provider Setup

Theme provider is configured in [`app/providers.tsx`](app/providers.tsx):

```tsx
<ThemeProvider
  attribute="class"
  defaultTheme="system"
  enableSystem
  disableTransitionOnChange
>
  {children}
</ThemeProvider>
```

- **attribute="class"**: Adds `.dark` class to `<html>` element
- **defaultTheme="system"**: Defaults to OS preference
- **enableSystem**: Enables system theme detection
- **disableTransitionOnChange**: Prevents jarring animations on theme switch

### 2. Layout Configuration

[`app/layout.tsx`](app/layout.tsx) includes `suppressHydrationWarning` to prevent Next.js hydration warnings:

```tsx
<html lang="en" suppressHydrationWarning>
```

This is required because next-themes injects the theme before React hydrates.

### 3. CSS Variables

Dark mode colors are defined in [`app/globals.css`](app/globals.css):

```css
/* Light mode (default) */
:root {
  --background: oklch(1 0 0);
  --foreground: oklch(0.141 0.005 285.823);
  /* ... more variables */
}

/* Dark mode */
.dark {
  --background: oklch(0.141 0.005 285.823);
  --foreground: oklch(0.985 0 0);
  /* ... more variables */
}
```

The `@custom-variant dark` directive enables Tailwind's dark mode utilities:

```css
@custom-variant dark (&:is(.dark *));
```

### 4. Mode Toggle Component

[`components/mode-toggle.tsx`](components/mode-toggle.tsx) provides the UI to switch themes:

```tsx
import { useTheme } from "next-themes";

export function ModeToggle() {
  const { setTheme } = useTheme();
  
  return (
    <DropdownMenu>
      {/* Toggle button with sun/moon icons */}
      <DropdownMenuTrigger>
        <Button variant="outline" size="icon">
          <Sun className="dark:scale-0" />
          <Moon className="scale-0 dark:scale-100" />
        </Button>
      </DropdownMenuTrigger>
      
      {/* Options: Light, Dark, System */}
      <DropdownMenuContent>
        <DropdownMenuItem onClick={() => setTheme("light")}>
          Light
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => setTheme("dark")}>
          Dark
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => setTheme("system")}>
          System
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
```

## Usage

### Using the Toggle Button

Add the `<ModeToggle />` component anywhere in your UI:

```tsx
import { ModeToggle } from "@/components/mode-toggle";

export function Header() {
  return (
    <header>
      <nav>
        {/* Your nav items */}
        <ModeToggle />
      </nav>
    </header>
  );
}
```

### Programmatic Theme Control

Access theme methods via the `useTheme` hook:

```tsx
"use client";

import { useTheme } from "next-themes";

export function MyComponent() {
  const { theme, setTheme, systemTheme, resolvedTheme } = useTheme();
  
  // theme: Current theme ("light", "dark", "system")
  // systemTheme: OS-detected theme
  // resolvedTheme: Actual theme being used (resolves "system")
  
  return (
    <div>
      <p>Current theme: {theme}</p>
      <button onClick={() => setTheme("dark")}>Go Dark</button>
    </div>
  );
}
```

### Styling Dark Mode

Use Tailwind's `dark:` prefix to apply dark mode styles:

```tsx
<div className="bg-white dark:bg-gray-900 text-black dark:text-white">
  This changes color in dark mode
</div>
```

Or use CSS variables (recommended):

```tsx
<div className="bg-background text-foreground">
  This automatically adapts to theme
</div>
```

## Architecture Benefits

1. **Single Source of Truth**: `next-themes` manages theme state
2. **No Flash**: Theme applied before first paint
3. **Type-Safe**: Full TypeScript support
4. **Accessible**: Screen reader support via `sr-only` labels
5. **Performant**: CSS-only implementation, no JavaScript runtime overhead

## Testing

To test dark mode:

1. **Manual Toggle**: Click the theme toggle button
2. **System Preference**: Change your OS theme settings
3. **Persistence**: Refresh the page - theme should persist
4. **SSR**: Disable JavaScript - theme should still work

## Related Files

- [`app/providers.tsx`](app/providers.tsx) - Provider setup
- [`app/layout.tsx`](app/layout.tsx) - Root layout with suppressHydrationWarning
- [`app/globals.css`](app/globals.css) - CSS variables for light/dark themes
- [`components/mode-toggle.tsx`](components/mode-toggle.tsx) - Toggle button component
- [`components/ui/dropdown-menu.tsx`](components/ui/dropdown-menu.tsx) - Dropdown menu primitive
- [`components/ui/button.tsx`](components/ui/button.tsx) - Button primitive

## References

- [next-themes Documentation](https://github.com/pacocoursey/next-themes)
- [Next.js Dark Mode Guide](https://nextjs.org/docs/app/building-your-application/styling/css-variables#dark-mode)
- [shadcn/ui Dark Mode](https://ui.shadcn.com/docs/dark-mode)
