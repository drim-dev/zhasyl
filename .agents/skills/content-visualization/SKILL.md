---
name: content-visualization
description: Use when creating visual components (graphs, diagrams, interactive visualizations) for MDX content - provides Figure wrapper, SVG patterns, mobile adaptivity, and light/dark theme support (project)
---

# Visualization Components

Visual components for MDX content throughout the platform - interactive diagrams, function graphs, architecture visualizations, and static images. Used in course lessons, blog posts, skill pages, and any other MDX content.

## When to Use

Use this skill when:

- Creating interactive visualizations (function graphs, architecture diagrams, flowcharts)
- Adding static images with captions to any MDX content
- Building visual components for courses, blog posts, or skill documentation
- Converting `> **Изображение:**` markers in MDX to actual components

## Core Principle: Figure Wrapper

**All visual content in MDX MUST use the `<Figure>` wrapper component.**

```jsx
import { Figure } from '@/components/mdx/figure'

// In MDX:
<Figure caption="Архитектура трансформера" source="Vaswani et al., 2017">
  <TransformerDiagram />
</Figure>
```

### Figure Props

```typescript
interface FigureProps {
  children: ReactNode      // Required - the visual content
  caption?: ReactNode      // Optional - description below visual
  source?: ReactNode       // Optional - attribution/credit
  className?: string       // Optional - additional styling
  minWidth?: string        // Optional - minimum width in lightbox (e.g., "600px")
}
```

### Figure Behavior

- **Invisible wrapper** - provides spacing only (`my-8`), no border/background
- **Semantic HTML** - uses `<figure>` and `<figcaption>` elements
- **Caption styling** - `text-sm text-stone-600 dark:text-stone-400`
- **Source styling** - `text-xs text-stone-500`, supports links
- **Click to expand** - clicking opens a lightbox modal for larger view
- **Lightbox controls** - close via X button, backdrop click, or Escape key

### Lightbox with minWidth

For complex diagrams that become hard to read on mobile, use `minWidth` to force a minimum size in the lightbox with horizontal scroll:

```jsx
<Figure caption="Complex diagram" minWidth="600px">
  <ComplexDiagram />
</Figure>
```

**When to use minWidth:**
- SVG diagrams with small text labels
- Detailed flowcharts or architecture diagrams
- Any visualization that loses readability when scaled down

**Choosing minWidth values:**
- `400px` - Simple diagrams
- `500px` - Standard diagrams
- `600px` - Complex diagrams with text
- `800px` - Wide comparison charts

### Usage Examples

```jsx
// Interactive component with external caption
<Figure
  caption="Сравнение функций активации"
  source={<>По материалам <a href="https://...">Deep Learning Book</a></>}
>
  <ActivationFunctions />
</Figure>

// Component with internal title (no external caption needed)
<Figure source="Авторская иллюстрация">
  <TransformerArchitecture />
</Figure>

// Static image
<Figure caption="Механический турок" source="Гравюра, 1784">
  <img
    src="/images/lessons/ai-superpower/mechanical-turk.jpg"
    alt="Механический турок за шахматным столом"
  />
</Figure>

// No caption
<Figure>
  <SimpleChart />
</Figure>
```

## File Structure

```
frontend/
├── components/
│   ├── mdx/
│   │   └── figure.tsx                    # Universal wrapper
│   └── visualizations/                   # Shared/reusable visualizations
│       └── function-graph.tsx
└── src/
    └── content/
        ├── courses/
        │   └── {course-slug}/
        │       ├── lesson1.mdx
        │       └── components/           # Course-specific visualizations
        │           └── activation-functions.tsx
        ├── blog/
        │   └── {post-slug}/
        │       └── components/           # Post-specific visualizations
        └── skills/
            └── {skill-slug}/
                └── components/           # Skill-specific visualizations
```

**Placement guideline:**
- Reusable across multiple pages → `frontend/components/visualizations/`
- Specific to one piece of content → `components/` subfolder next to the MDX file

## Component Structure

### Basic Template

```tsx
'use client'

import React, { useState, useMemo } from 'react'

interface MyVisualizationProps {
  // Define any configurable props
}

export function MyVisualization({}: MyVisualizationProps) {
  // State for interactivity
  const [selected, setSelected] = useState<string>('default')

  // SVG dimensions and scaling
  const width = 500
  const height = 300
  const padding = 40

  // Coordinate transformations
  const scaleX = (x: number) => padding + ((x - xMin) / (xMax - xMin)) * (width - 2 * padding)
  const scaleY = (y: number) => height - padding - ((y - yMin) / (yMax - yMin)) * (height - 2 * padding)

  return (
    <div className="overflow-x-auto">
      <div className="min-w-[400px] flex flex-col items-center p-6 bg-slate-900 rounded-xl">
        {/* Optional internal title */}
        <h2 className="text-2xl font-bold text-white mb-4">Название визуализации</h2>

        {/* Controls */}
        <div className="flex flex-wrap gap-2 mb-6 justify-center">
          {/* Buttons, toggles, etc. */}
        </div>

        {/* SVG visualization */}
        <svg
          viewBox={`0 0 ${width} ${height}`}
          className="w-full max-w-lg bg-slate-800 rounded-lg"
        >
          {/* Grid, axes, data */}
        </svg>

        {/* Info panel */}
        <div className="mt-6 w-full max-w-lg">
          {/* Description, details, pros/cons */}
        </div>
      </div>
    </div>
  )
}
```

### Export Pattern

Components must be exported and registered in MDX:

```tsx
// In the component file
export function ActivationFunctions() { ... }

// In the MDX file - import at top
import { ActivationFunctions } from './components/activation-functions'

// Use in content
<Figure caption="...">
  <ActivationFunctions />
</Figure>
```

## Styling Guidelines

### Theme Support (Light + Dark)

**All visualizations MUST support both light and dark themes.** Use CSS custom properties to adapt colors based on theme.

#### CSS Variables Pattern

Define theme-aware CSS variables inside the SVG's `<style>` tag:

```tsx
<svg viewBox="0 0 600 300" className="w-full rounded-lg my-visualization">
  <style>{`
    .my-visualization {
      --bg: #f5f5f4;
      --surface: #fafaf9;
      --text-primary: #1c1917;
      --text-secondary: #57534e;
      --text-muted: #78716c;
      --border: #a8a29e;
    }
    .dark .my-visualization,
    :root.dark .my-visualization {
      --bg: #1e293b;
      --surface: #334155;
      --text-primary: #f8fafc;
      --text-secondary: #94a3b8;
      --text-muted: #64748b;
      --border: #475569;
    }
  `}</style>

  <rect width="600" height="300" fill="var(--bg)" />
  <text fill="var(--text-primary)">...</text>
</svg>
```

#### Color Mappings

| Purpose | Light Mode (stone) | Dark Mode (slate) |
|---------|-------------------|-------------------|
| Background | `#f5f5f4` (stone-100) | `#1e293b` (slate-800) |
| Surface/boxes | `#fafaf9` (stone-50) | `#334155` (slate-700) |
| Text primary | `#1c1917` (stone-900) | `#f8fafc` (slate-50) |
| Text secondary | `#57534e` (stone-600) | `#94a3b8` (slate-400) |
| Text muted | `#78716c` (stone-500) | `#64748b` (slate-500) |
| Borders/lines | `#a8a29e` (stone-400) | `#475569` (slate-600) |
| Grid lines | `#d6d3d1` (stone-300) | `#334155` (slate-700) |

#### Accent Colors (Same in Both Themes)

These colors provide good contrast in both light and dark modes:

- Blue: `#3b82f6` (blue-500)
- Purple: `#8b5cf6` (violet-500)
- Green: `#10b981` (emerald-500)
- Orange/Amber: `#f59e0b` (amber-500)
- Pink: `#ec4899` (pink-500)

#### When to Use CSS Variables vs Fixed Colors

**Use CSS variables for:**
- Backgrounds and surfaces
- Text colors
- Grid lines and borders
- Arrow markers

**Use fixed colors for:**
- Data lines and curves (accent colors)
- Highlighted elements (colored borders/strokes)
- Interactive state indicators

### Dark Theme Colors (Legacy Reference)

For components that only need dark theme (rare cases like embedded demos):

**Primary surfaces:**
- Main container: `bg-slate-900`
- Secondary surface: `bg-slate-800`
- Tertiary/cards: `bg-slate-700`

**Text:**
- Primary: `text-white`
- Secondary: `text-slate-300`
- Muted: `text-slate-400`
- Labels: `text-slate-500`

**Grid and axes:**
- Grid lines: `stroke="#334155"` (slate-700)
- Axis lines: `stroke="#64748b"` (slate-500)
- Axis labels: `fill="#64748b"`

**Accent colors for data:**
- Blue: `#3b82f6`
- Purple: `#8b5cf6`
- Green: `#10b981`
- Orange: `#f59e0b`
- Pink: `#ec4899`

### Interactive Elements

**Buttons:**
```jsx
<button
  onClick={() => setSelected(key)}
  className={`px-4 py-2 rounded-lg font-medium transition-all ${
    selected === key
      ? 'text-white shadow-lg scale-105'
      : 'bg-slate-700 text-slate-300 hover:bg-slate-600'
  }`}
  style={selected === key ? { backgroundColor: accentColor } : {}}
>
  {label}
</button>
```

**Checkboxes:**
```jsx
<label className="flex items-center gap-2 cursor-pointer">
  <input
    type="checkbox"
    checked={value}
    onChange={(e) => setValue(e.target.checked)}
    className="w-4 h-4 rounded"
  />
  <span className="text-slate-400 text-sm">{label}</span>
</label>
```

### Rounded Corners

- Main container: `rounded-xl`
- SVG container: `rounded-lg`
- Cards/panels: `rounded-lg`
- Buttons: `rounded-lg`
- Small elements: `rounded` or `rounded-md`

## SVG Best Practices

### Always Use viewBox

```jsx
<svg
  viewBox={`0 0 ${width} ${height}`}  // Defines coordinate system
  className="w-full max-w-lg"          // Responsive width
>
```

**Why:** viewBox makes SVG scale responsively while maintaining aspect ratio.

### Coordinate System

Define clear min/max ranges and scaling functions:

```tsx
const xMin = -6, xMax = 6
const yMin = -2, yMax = 2

const scaleX = (x: number) =>
  padding + ((x - xMin) / (xMax - xMin)) * (width - 2 * padding)

const scaleY = (y: number) =>
  height - padding - ((y - yMin) / (yMax - yMin)) * (height - 2 * padding)
```

**Note:** Y is inverted in SVG (0 is top), so scaleY subtracts from height.

### Grid Lines

```tsx
const gridLines = useMemo(() => {
  const lines = []

  // Vertical grid
  for (let x = xMin; x <= xMax; x += step) {
    lines.push(
      <line
        key={`vgrid-${x}`}
        x1={scaleX(x)} y1={padding}
        x2={scaleX(x)} y2={height - padding}
        stroke="#334155" strokeWidth="1"
      />
    )
  }

  // Horizontal grid
  for (let y = yMin; y <= yMax; y += step) {
    lines.push(
      <line
        key={`hgrid-${y}`}
        x1={padding} y1={scaleY(y)}
        x2={width - padding} y2={scaleY(y)}
        stroke="#334155" strokeWidth="1"
      />
    )
  }

  return lines
}, []) // Memoize if dependencies are stable
```

### Drawing Curves

```tsx
const generatePath = (fn: (x: number) => number) => {
  const points: string[] = []

  for (let x = xMin; x <= xMax; x += 0.05) {
    const y = fn(x)
    // Clamp to visible range
    if (y >= yMin - 0.5 && y <= yMax + 0.5) {
      points.push(`${scaleX(x)},${scaleY(Math.max(yMin, Math.min(yMax, y)))}`)
    }
  }

  return `M ${points.join(' L ')}`
}

// Usage
<path
  d={generatePath(Math.sin)}
  fill="none"
  stroke="#3b82f6"
  strokeWidth="3"
  strokeLinecap="round"
/>
```

## Mobile Adaptivity

### Strategy: Responsive with Minimum Width

Components scale down responsively until they hit a minimum width, then horizontal scroll appears.

### Implementation Pattern

**Each component handles its own scroll container:**

```tsx
export function MyVisualization() {
  return (
    <div className="overflow-x-auto">
      <div className="min-w-[400px]">  {/* Adjust min-width per component */}
        {/* Content */}
      </div>
    </div>
  )
}
```

**Choosing min-width:**
- Simple charts: `min-w-[300px]`
- Standard diagrams: `min-w-[400px]`
- Complex visualizations: `min-w-[500px]`
- Wide comparisons: `min-w-[600px]`

### Touch Interactions

Replace hover with tap-to-toggle:

```tsx
const [activePoint, setActivePoint] = useState<{x: number, y: number} | null>(null)

// Desktop: show on hover
const handleMouseMove = (e: React.MouseEvent<SVGSVGElement>) => {
  const rect = e.currentTarget.getBoundingClientRect()
  const x = calculateX(e.clientX - rect.left)
  const y = fn(x)
  setActivePoint({ x, y })
}

const handleMouseLeave = () => setActivePoint(null)

// Mobile: tap to toggle (same point stays until tap elsewhere)
const handleClick = (e: React.MouseEvent<SVGSVGElement>) => {
  const rect = e.currentTarget.getBoundingClientRect()
  const x = calculateX(e.clientX - rect.left)
  const y = fn(x)
  setActivePoint(prev =>
    prev?.x === x ? null : { x, y }
  )
}

<svg
  onMouseMove={handleMouseMove}
  onMouseLeave={handleMouseLeave}
  onClick={handleClick}
>
```

### Button Rows

Always use flex-wrap for control buttons:

```tsx
<div className="flex flex-wrap gap-2 mb-6 justify-center">
  {options.map(opt => (
    <button key={opt.key} ...>{opt.label}</button>
  ))}
</div>
```

## Accessibility

### Required Practices

1. **Alt text for images:**
   ```jsx
   <img
     src="/images/..."
     alt="Описание изображения на русском"  // Descriptive alt text
   />
   ```

2. **ARIA labels for interactive SVG:**
   ```jsx
   <svg
     role="img"
     aria-label="График функций активации"
   >
   ```

3. **Keyboard-accessible controls:**
   ```jsx
   <button
     onClick={handleClick}
     onKeyDown={(e) => e.key === 'Enter' && handleClick()}
     tabIndex={0}
   >
   ```

4. **Focus indicators:**
   ```jsx
   <button className="... focus:outline-none focus:ring-2 focus:ring-brand-500">
   ```

## Library Policy

### Raw SVG First

**Default approach:** Use raw SVG for all visualizations.

**Why:**
- Full styling control
- No bundle size impact
- Matches dark theme perfectly
- Simpler debugging

### When to Add Libraries

**Only consider external libraries when:**
- Complex statistical charts with many data points (consider Recharts)
- Real-time updating charts (consider Visx)
- Complex animations (consider Framer Motion)

**Always get approval before adding dependencies.**

## Checklist

Before marking a visualization complete:

- [ ] Wrapped in `<Figure>` component
- [ ] Caption and/or source provided (if needed)
- [ ] Uses `viewBox` for responsive SVG
- [ ] Has `overflow-x-auto` + `min-w-*` for mobile
- [ ] Touch interactions work (tap instead of hover where applicable)
- [ ] **Supports both light and dark themes** (CSS variables pattern)
- [ ] Button rows use `flex-wrap`
- [ ] Accessible (alt text, ARIA labels, keyboard navigation)
- [ ] No external dependencies (or justified if added)
- [ ] `'use client'` directive if component has state
- [ ] Consider `minWidth` prop if diagram has small text/details that need lightbox scroll
