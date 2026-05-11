# PC Client: Fixed vs Per-Page (Lists and Cards)

## 1. Fixed + per-page without affecting fixed

- **Fixed**: Toolbar and layout that every list/card page should have and that must not be removed when adding page-specific content.
- **Per-page**: Columns and menu items that vary by screen; they are **added** after the fixed part and never replace it.

### BaseListForm (recommended for new list forms)

- **BaseListForm** (in `Base\BaseListForm.cs`) provides:
  - **Fixed toolbar**: New, Edit, Delete, Refresh, Clear Filter (always present).
  - **Fixed layout**: MenuStrip on top, DataGridView below filling the form.
- **Subclass**:
  - Override **OnAddColumns()** to add grid columns. Add any “fixed” columns first (e.g. key, name), then add per-page columns. Do not remove the grid or the fixed toolbar.
  - Override **OnAddExtraMenuItems()** to add more items to the same MenuStrip (they are appended; fixed items stay).
  - Override **OnNewClick()**, **OnEditClick()**, **OnDeleteClick()**, **OnRefreshClick()**, **OnClearFilterClick()** to implement behaviour.
- So: **fixed** = toolbar + layout; **per-page** = columns + extra menu items and handlers.

### Existing list forms (e.g. UserList)

- Keep the same idea: treat New, Edit, Delete, Refresh, Clear Filter as **fixed**.
- When adding per-page menu items, **add** them (e.g. `menuStrip1.Items.Add(...)`) instead of replacing or clearing the strip, so the fixed set is never removed.
- For DataGridView: define columns in a stable order (e.g. key column first); any code that adds “per-page” columns should only add columns, not clear or replace the fixed ones.

## 2. Main form: Menu vs Menu2

- **Main**: Fixed set of buttons (User, Mapping, Prescan, etc.) plus the **Menu2** button.
- **Menu2**: Opens a **menu** (menu-style layout: vertical menu items), not a data list. Use it to choose an action; it then opens the same screens as the main buttons.

## 3. Summary

| Area            | Fixed part                         | Per-page part (add only, do not remove fixed) |
|-----------------|------------------------------------|-----------------------------------------------|
| List toolbar    | New, Edit, Delete, Refresh, Clear | Extra menu items                              |
| List grid       | Optional: key/name columns first   | Rest of columns                               |
| Main form       | Menu grid + Menu2 button          | —                                             |
| Menu2           | Menu layout (buttons/items)       | —                                             |
