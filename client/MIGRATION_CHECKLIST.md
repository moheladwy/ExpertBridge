# ✅ UI Migration Checklist

## Installation Complete ✅

All required shadcn/ui components have been installed:
- dialog, form, field, toggle, toggle-group
- popover, tooltip, scroll-area, switch
- radio-group, checkbox, progress, spinner

## Ready to Start Migration

### Phase 1: Setup ✅ DONE
- [x] Install all shadcn components
- [x] Spinner component added
- [x] Typography docs reviewed
- [x] Field component ready

### Phase 2: Critical Components (Next Steps)
- [ ] Migrate TextField → Field + Input/Textarea (35+ files)
- [ ] Migrate Modal → Dialog (15+ files)
- [ ] Migrate CircularProgress → Spinner
- [ ] Migrate Typography → Tailwind classes

### Quick Start Commands

```bash
# Count MUI usage
echo "TextField: $(grep -r 'TextField' src/ --include='*.tsx' | wc -l)"
echo "Modal: $(grep -r 'Modal from "@mui' src/ --include='*.tsx' | wc -l)"
echo "CircularProgress: $(grep -r 'CircularProgress' src/ --include='*.tsx' | wc -l)"
echo "Typography: $(grep -r 'Typography from "@mui' src/ --include='*.tsx' | wc -l)"

# After migration, verify MUI removal
grep -r "@mui" src/ --include="*.tsx" --include="*.ts"
```

## See full plan: audit-report/fix-ui-plan.md
