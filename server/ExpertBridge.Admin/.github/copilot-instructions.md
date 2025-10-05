# ExpertBridge.Admin - Radzen Blazor Components Guide

## ⚠️ CRITICAL: RADZEN-ONLY UI

**ALL UI components MUST use Radzen Blazor Components exclusively.**

### Forbidden Patterns
- ❌ Bootstrap components
- ❌ Plain HTML `<table>`, `<button>`, `<input>`
- ❌ Standard Blazor components (except framework directives)
- ❌ Any other UI library

### Required Patterns
- ✅ RadzenDataGrid for all tables
- ✅ RadzenButton for all buttons
- ✅ RadzenCard for containers
- ✅ RadzenStack/RadzenRow for layouts
- ✅ Radzen form components (RadzenTextBox, RadzenDropDown, etc.)

## Setup (4 Steps)

### 1. Install Package
```bash
dotnet add package Radzen.Blazor
```

### 2. _Imports.razor
```razor
@using Radzen
@using Radzen.Blazor
```

### 3. App.razor
```razor
<!DOCTYPE html>
<html>
<head>
    <RadzenTheme Theme="material3-dark" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>
<body>
    <Routes @rendermode="InteractiveServer" />
    <script src="_framework/blazor.web.js"></script>
    <script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
</body>
</html>
```

### 4. Program.cs
```csharp
builder.Services.AddRadzenComponents();
```

## Component Categories

### 1. DATA COMPONENTS

#### RadzenDataGrid - PRIMARY TABLE COMPONENT
```razor
@* Basic DataGrid *@
<RadzenDataGrid Data="@users" TItem="User"
                AllowFiltering="true"
                AllowSorting="true"
                AllowPaging="true"
                PageSize="10">
    <Columns>
        <RadzenDataGridColumn TItem="User" Property="Email" Title="Email" />
        <RadzenDataGridColumn TItem="User" Property="Username" Title="Username" />
        <RadzenDataGridColumn TItem="User" Property="CreatedAt" Title="Created"
                            FormatString="{0:d}" />
        <RadzenDataGridColumn TItem="User" Title="Actions">
            <Template Context="user">
                <RadzenButton Icon="edit" ButtonStyle="ButtonStyle.Info"
                             Click="@(() => EditUser(user))" />
                <RadzenButton Icon="delete" ButtonStyle="ButtonStyle.Danger"
                             Click="@(() => DeleteUser(user))" />
            </Template>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>

@* Server-Side DataGrid *@
<RadzenDataGrid Count="@count" Data="@users"
                LoadData="@LoadData"
                IsLoading="@isLoading"
                AllowPaging="true" PageSize="20"
                AllowSorting="true" AllowFiltering="true"
                TItem="User">
    <Columns>
        <RadzenDataGridColumn TItem="User" Property="Email" Title="Email" />
    </Columns>
</RadzenDataGrid>

@code {
    IEnumerable<User> users;
    int count;
    bool isLoading;

    async Task LoadData(LoadDataArgs args)
    {
        isLoading = true;
        var query = dbContext.Users.AsQueryable();

        if (!string.IsNullOrEmpty(args.Filter))
            query = query.Where(args.Filter);

        count = await query.CountAsync();
        users = await query.Skip(args.Skip ?? 0)
                          .Take(args.Top ?? 20)
                          .ToListAsync();
        isLoading = false;
    }
}
```

#### RadzenTree - Hierarchical Data
```razor
<RadzenTree Data="@items" Style="width: 100%; height: 300px">
    <RadzenTreeLevel TextProperty="Name"
                     ChildrenProperty="Children"
                     HasChildren="@(item => item.Children.Any())" />
</RadzenTree>
```

### 2. FORM COMPONENTS

#### Complete Form Example
```razor
<RadzenTemplateForm TItem="CreateUserRequest" Data="@model" Submit="@OnSubmit">
    <RadzenStack Gap="1rem">
        <RadzenFieldset Text="User Information">
            <RadzenStack Gap="1rem">
                <RadzenFormField Text="Email" Variant="Variant.Outlined">
                    <RadzenTextBox @bind-Value="@model.Email" Name="Email" />
                    <RadzenRequiredValidator Component="Email" Text="Email is required" />
                    <RadzenEmailValidator Component="Email" Text="Invalid email" />
                </RadzenFormField>

                <RadzenFormField Text="Username" Variant="Variant.Outlined">
                    <RadzenTextBox @bind-Value="@model.Username" Name="Username" />
                    <RadzenRequiredValidator Component="Username" Text="Required" />
                    <RadzenLengthValidator Component="Username" Min="3" Max="50"
                                         Text="Username must be 3-50 characters" />
                </RadzenFormField>

                <RadzenFormField Text="Role" Variant="Variant.Outlined">
                    <RadzenDropDown @bind-Value="@model.RoleId"
                                   Data="@roles"
                                   TextProperty="Name"
                                   ValueProperty="Id" />
                </RadzenFormField>

                <RadzenFormField Text="Bio" Variant="Variant.Outlined">
                    <RadzenTextArea @bind-Value="@model.Bio" Rows="4" />
                </RadzenFormField>

                <RadzenFormField Text="Active" Variant="Variant.Outlined">
                    <RadzenCheckBox @bind-Value="@model.IsActive" />
                </RadzenFormField>
            </RadzenStack>
        </RadzenFieldset>

        <RadzenStack Orientation="Orientation.Horizontal" Gap="0.5rem"
                     JustifyContent="JustifyContent.End">
            <RadzenButton Text="Cancel" ButtonStyle="ButtonStyle.Light"
                         Click="@OnCancel" />
            <RadzenButton Text="Save" ButtonType="ButtonType.Submit"
                         ButtonStyle="ButtonStyle.Primary" />
        </RadzenStack>
    </RadzenStack>
</RadzenTemplateForm>
```

#### Individual Form Components
```razor
@* Text Input *@
<RadzenTextBox @bind-Value="@value" Placeholder="Enter text..." />

@* Numeric Input *@
<RadzenNumeric @bind-Value="@number" Min="0" Max="100" />

@* Date Picker *@
<RadzenDatePicker @bind-Value="@date" DateFormat="MM/dd/yyyy" />

@* Time Picker *@
<RadzenDatePicker @bind-Value="@time" ShowTime="true" TimeOnly="true" />

@* Dropdown *@
<RadzenDropDown @bind-Value="@selectedId"
                Data="@items"
                TextProperty="Name"
                ValueProperty="Id" />

@* Checkbox *@
<RadzenCheckBox @bind-Value="@isChecked" />

@* Switch *@
<RadzenSwitch @bind-Value="@isEnabled" />

@* Radio Buttons *@
<RadzenRadioButtonList @bind-Value="@selectedValue" TValue="string">
    <Items>
        <RadzenRadioButtonListItem Text="Option 1" Value="opt1" />
        <RadzenRadioButtonListItem Text="Option 2" Value="opt2" />
    </Items>
</RadzenRadioButtonList>

@* File Upload *@
<RadzenUpload Url="api/upload" Complete="@OnUploadComplete" />
```

### 3. LAYOUT COMPONENTS

#### RadzenCard - Container
```razor
<RadzenCard>
    <RadzenStack Gap="1rem">
        <RadzenText TextStyle="TextStyle.H5" TagName="TagName.H2">
            User Statistics
        </RadzenText>
        <RadzenText>Total Users: 1,234</RadzenText>
    </RadzenStack>
</RadzenCard>
```

#### RadzenStack - Flexible Layout
```razor
@* Vertical Stack *@
<RadzenStack Gap="1rem">
    <RadzenCard>Item 1</RadzenCard>
    <RadzenCard>Item 2</RadzenCard>
</RadzenStack>

@* Horizontal Stack *@
<RadzenStack Orientation="Orientation.Horizontal"
             Gap="1rem"
             AlignItems="AlignItems.Center"
             JustifyContent="JustifyContent.SpaceBetween">
    <RadzenText>Left Content</RadzenText>
    <RadzenButton Text="Action" />
</RadzenStack>
```

#### RadzenRow/RadzenColumn - Grid System
```razor
<RadzenRow Gap="1rem">
    <RadzenColumn Size="12" SizeMD="6" SizeLG="4">
        <RadzenCard>Column 1</RadzenCard>
    </RadzenColumn>
    <RadzenColumn Size="12" SizeMD="6" SizeLG="4">
        <RadzenCard>Column 2</RadzenCard>
    </RadzenColumn>
    <RadzenColumn Size="12" SizeMD="12" SizeLG="4">
        <RadzenCard>Column 3</RadzenCard>
    </RadzenColumn>
</RadzenRow>
```

#### RadzenLayout - Main Layout
```razor
<RadzenLayout>
    <RadzenHeader>
        <RadzenStack Orientation="Orientation.Horizontal"
                     AlignItems="AlignItems.Center">
            <RadzenSidebarToggle Click="@(() => sidebarExpanded = !sidebarExpanded)" />
            <RadzenLabel Text="ExpertBridge Admin" />
        </RadzenStack>
    </RadzenHeader>

    <RadzenSidebar @bind-Expanded="@sidebarExpanded">
        <NavMenu />
    </RadzenSidebar>

    <RadzenBody>
        <RadzenContentContainer Name="main">
            @Body
        </RadzenContentContainer>
    </RadzenBody>

    <RadzenFooter>
        <RadzenLabel Text="© 2025 ExpertBridge" />
    </RadzenFooter>
</RadzenLayout>
```

### 4. NAVIGATION COMPONENTS

#### RadzenPanelMenu - Sidebar Navigation
```razor
<RadzenPanelMenu>
    <RadzenPanelMenuItem Text="Dashboard" Icon="dashboard" Path="/" />
    <RadzenPanelMenuItem Text="Users" Icon="people" Path="/users" />
    <RadzenPanelMenuItem Text="Posts" Icon="article">
        <RadzenPanelMenuItem Text="All Posts" Path="/posts" />
        <RadzenPanelMenuItem Text="Flagged" Path="/posts/flagged" />
    </RadzenPanelMenuItem>
    <RadzenPanelMenuItem Text="Settings" Icon="settings" Path="/settings" />
</RadzenPanelMenu>
```

#### RadzenTabs
```razor
<RadzenTabs>
    <Tabs>
        <RadzenTabsItem Text="Overview">
            <RadzenCard>Overview content</RadzenCard>
        </RadzenTabsItem>
        <RadzenTabsItem Text="Details">
            <RadzenCard>Details content</RadzenCard>
        </RadzenTabsItem>
    </Tabs>
</RadzenTabs>
```

#### RadzenSteps - Wizard
```razor
<RadzenSteps @bind-SelectedIndex="@selectedStep">
    <Steps>
        <RadzenStepsItem Text="Step 1">
            <RadzenCard>Step 1 content</RadzenCard>
        </RadzenStepsItem>
        <RadzenStepsItem Text="Step 2">
            <RadzenCard>Step 2 content</RadzenCard>
        </RadzenStepsItem>
    </Steps>
</RadzenSteps>
```

### 5. FEEDBACK COMPONENTS

#### Buttons
```razor
@* Primary Button *@
<RadzenButton Text="Primary" ButtonStyle="ButtonStyle.Primary"
             Click="@OnClick" />

@* With Icon *@
<RadzenButton Text="Save" Icon="save" ButtonStyle="ButtonStyle.Success" />

@* Icon Only *@
<RadzenButton Icon="delete" ButtonStyle="ButtonStyle.Danger"
             ButtonType="ButtonType.Button" />

@* Loading State *@
<RadzenButton Text="Submit" IsBusy="@isLoading"
             Click="@OnSubmit" />

@* Button Styles *@
<RadzenButton Text="Primary" ButtonStyle="ButtonStyle.Primary" />
<RadzenButton Text="Secondary" ButtonStyle="ButtonStyle.Secondary" />
<RadzenButton Text="Success" ButtonStyle="ButtonStyle.Success" />
<RadzenButton Text="Danger" ButtonStyle="ButtonStyle.Danger" />
<RadzenButton Text="Warning" ButtonStyle="ButtonStyle.Warning" />
<RadzenButton Text="Info" ButtonStyle="ButtonStyle.Info" />
<RadzenButton Text="Light" ButtonStyle="ButtonStyle.Light" />
```

#### Notifications (inject NotificationService)
```razor
@inject NotificationService NotificationService

<RadzenButton Text="Show Notification" Click="@ShowNotification" />

@code {
    void ShowNotification()
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Success",
            Detail = "Operation completed successfully",
            Duration = 4000
        });
    }
}
```

#### Dialogs (inject DialogService)
```razor
@inject DialogService DialogService

<RadzenButton Text="Open Dialog" Click="@OpenDialog" />

@code {
    async Task OpenDialog()
    {
        var result = await DialogService.OpenAsync<EditUserDialog>("Edit User",
            new Dictionary<string, object> { { "UserId", userId } },
            new DialogOptions { Width = "600px" });

        if (result != null)
        {
            // Handle result
        }
    }

    async Task ConfirmDelete()
    {
        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this user?",
            "Confirm Delete",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed == true)
        {
            await DeleteUser();
        }
    }
}
```

#### Progress Indicators
```razor
@* Progress Bar *@
<RadzenProgressBar Value="@progress" Max="100" />

@* Progress Bar with Label *@
<RadzenProgressBar Value="@progress" Max="100" ShowValue="true" />

@* Indeterminate Progress *@
<RadzenProgressBarCircular ShowValue="false" Mode="ProgressBarMode.Indeterminate" />

@* Badge *@
<RadzenBadge Text="12" BadgeStyle="BadgeStyle.Danger" />
```

### 6. CHART COMPONENTS

#### Line Chart
```razor
<RadzenChart>
    <RadzenLineSeries Data="@data" CategoryProperty="Date"
                     ValueProperty="Value" />
</RadzenChart>
```

#### Bar Chart
```razor
<RadzenChart>
    <RadzenColumnSeries Data="@stats" CategoryProperty="Month"
                       Title="Users" ValueProperty="UserCount" />
</RadzenChart>
```

#### Pie Chart
```razor
<RadzenChart>
    <RadzenPieSeries Data="@distribution"
                    CategoryProperty="Category"
                    ValueProperty="Count" />
</RadzenChart>
```

#### Area Chart
```razor
<RadzenChart>
    <RadzenAreaSeries Data="@metrics" CategoryProperty="Time"
                     ValueProperty="Value" />
</RadzenChart>
```

## Advanced Patterns

### Master-Detail with DataGrid
```razor
<RadzenDataGrid Data="@users" TItem="User"
                RowExpand="@OnRowExpand">
    <Template Context="user">
        <RadzenCard>
            <RadzenDataGrid Data="@user.Posts" TItem="Post">
                <Columns>
                    <RadzenDataGridColumn TItem="Post" Property="Title" />
                    <RadzenDataGridColumn TItem="Post" Property="CreatedAt" />
                </Columns>
            </RadzenDataGrid>
        </RadzenCard>
    </Template>
    <Columns>
        <RadzenDataGridColumn TItem="User" Property="Email" />
    </Columns>
</RadzenDataGrid>
```

### Custom Template Columns
```razor
<RadzenDataGridColumn TItem="User" Title="Status">
    <Template Context="user">
        <RadzenBadge Text="@(user.IsActive ? "Active" : "Inactive")"
                    BadgeStyle="@(user.IsActive ? BadgeStyle.Success : BadgeStyle.Danger)" />
    </Template>
</RadzenDataGridColumn>
```

### Inline Editing in DataGrid
```razor
<RadzenDataGrid @ref="grid" Data="@users" TItem="User"
                EditMode="DataGridEditMode.Single"
                RowUpdate="@OnUpdateRow">
    <Columns>
        <RadzenDataGridColumn TItem="User" Property="Email">
            <EditTemplate Context="user">
                <RadzenTextBox @bind-Value="user.Email" />
            </EditTemplate>
        </RadzenDataGridColumn>
        <RadzenDataGridColumn TItem="User" Title="Actions">
            <Template Context="user">
                <RadzenButton Icon="edit" Click="@(() => grid.EditRow(user))" />
            </Template>
            <EditTemplate Context="user">
                <RadzenButton Icon="check" Click="@(() => grid.UpdateRow(user))" />
                <RadzenButton Icon="close" Click="@grid.CancelEditRow" />
            </EditTemplate>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>

@code {
    RadzenDataGrid<User> grid;

    async Task OnUpdateRow(User user)
    {
        await dbContext.SaveChangesAsync();
        NotificationService.Notify(NotificationSeverity.Success, "Updated");
    }
}
```

## Theme Configuration

Available themes:
- `material3` - Material Design 3 Light
- `material3-dark` - Material Design 3 Dark (recommended)
- `material` - Material Design Light
- `standard` - Radzen Standard
- `default` - Default theme
- `humanistic` - Humanistic theme
- `software` - Software theme
- `fluent` - Fluent Design
- `fluent-dark` - Fluent Design Dark

```razor
<RadzenTheme Theme="material3-dark" />
```

## Best Practices

### 1. Always Use Radzen Components
✅ DO:
```razor
<RadzenButton Text="Click Me" Click="@OnClick" />
<RadzenDataGrid Data="@users" TItem="User">...</RadzenDataGrid>
```

❌ DON'T:
```razor
<button @onclick="OnClick">Click Me</button>
<table>...</table>
```

### 2. Use RadzenStack for Layouts
✅ DO:
```razor
<RadzenStack Gap="1rem">
    <RadzenCard>Item 1</RadzenCard>
    <RadzenCard>Item 2</RadzenCard>
</RadzenStack>
```

❌ DON'T:
```razor
<div class="container">
    <div class="card">Item 1</div>
</div>
```

### 3. Server-Side DataGrid for Large Datasets
```razor
<RadzenDataGrid LoadData="@LoadData" Count="@count"
                AllowPaging="true" PageSize="20">
```

### 4. Use Templates for Custom Rendering
```razor
<RadzenDataGridColumn TItem="User" Title="Actions">
    <Template Context="user">
        <RadzenButton Icon="edit" Click="@(() => Edit(user))" />
    </Template>
</RadzenDataGridColumn>
```

### 5. Inject Services in Components
```razor
@inject DialogService DialogService
@inject NotificationService NotificationService
@inject TooltipService TooltipService
```

### 6. Use Variants for Consistent Styling
```razor
<RadzenFormField Variant="Variant.Outlined">
    <RadzenTextBox />
</RadzenFormField>
```

## Anti-Patterns

❌ **NEVER use Bootstrap classes**
```razor
<!-- WRONG -->
<div class="btn btn-primary">Button</div>
<div class="table">...</div>
```

❌ **NEVER use plain HTML form elements**
```razor
<!-- WRONG -->
<input type="text" />
<button>Click</button>
<select>...</select>
```

❌ **NEVER use standard Blazor components when Radzen equivalent exists**
```razor
<!-- WRONG -->
<InputText @bind-Value="model.Name" />

<!-- CORRECT -->
<RadzenTextBox @bind-Value="model.Name" />
```

## Complete Page Example

```razor
@page "/users"
@inject ExpertBridgeDbContext DbContext
@inject DialogService DialogService
@inject NotificationService NotificationService

<RadzenStack Gap="1rem">
    <RadzenRow AlignItems="AlignItems.Center" JustifyContent="JustifyContent.SpaceBetween">
        <RadzenColumn>
            <RadzenText TextStyle="TextStyle.H3" TagName="TagName.H1">
                User Management
            </RadzenText>
        </RadzenColumn>
        <RadzenColumn Size="12" SizeMD="auto">
            <RadzenButton Text="Add User" Icon="add"
                         ButtonStyle="ButtonStyle.Primary"
                         Click="@OpenCreateDialog" />
        </RadzenColumn>
    </RadzenRow>

    <RadzenCard>
        <RadzenDataGrid @ref="grid" Data="@users" TItem="User"
                        AllowFiltering="true"
                        AllowSorting="true"
                        AllowPaging="true"
                        PageSize="20"
                        IsLoading="@isLoading">
            <Columns>
                <RadzenDataGridColumn TItem="User" Property="Email" Title="Email" />
                <RadzenDataGridColumn TItem="User" Property="Username" Title="Username" />
                <RadzenDataGridColumn TItem="User" Property="CreatedAt"
                                     Title="Created" FormatString="{0:d}" />
                <RadzenDataGridColumn TItem="User" Title="Status">
                    <Template Context="user">
                        <RadzenBadge Text="@(user.IsDeleted ? "Deleted" : "Active")"
                                    BadgeStyle="@(user.IsDeleted ? BadgeStyle.Danger : BadgeStyle.Success)" />
                    </Template>
                </RadzenDataGridColumn>
                <RadzenDataGridColumn TItem="User" Title="Actions" Width="150px">
                    <Template Context="user">
                        <RadzenStack Orientation="Orientation.Horizontal" Gap="0.5rem">
                            <RadzenButton Icon="edit"
                                         ButtonStyle="ButtonStyle.Info"
                                         Click="@(() => EditUser(user))" />
                            <RadzenButton Icon="delete"
                                         ButtonStyle="ButtonStyle.Danger"
                                         Click="@(() => DeleteUser(user))" />
                        </RadzenStack>
                    </Template>
                </RadzenDataGridColumn>
            </Columns>
        </RadzenDataGrid>
    </RadzenCard>
</RadzenStack>

@code {
    RadzenDataGrid<User> grid;
    IEnumerable<User> users;
    bool isLoading;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
    }

    async Task LoadUsers()
    {
        isLoading = true;
        users = await DbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        isLoading = false;
    }

    async Task OpenCreateDialog()
    {
        var result = await DialogService.OpenAsync<CreateUserDialog>(
            "Create User",
            new DialogOptions { Width = "600px" });

        if (result != null)
        {
            await LoadUsers();
            await grid.Reload();
        }
    }

    async Task EditUser(User user)
    {
        var result = await DialogService.OpenAsync<EditUserDialog>(
            "Edit User",
            new Dictionary<string, object> { { "UserId", user.Id } },
            new DialogOptions { Width = "600px" });

        if (result != null)
        {
            await LoadUsers();
            await grid.Reload();
        }
    }

    async Task DeleteUser(User user)
    {
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to delete user '{user.Email}'?",
            "Confirm Delete",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed == true)
        {
            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync();
            await LoadUsers();
            await grid.Reload();

            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = "User deleted successfully"
            });
        }
    }
}
```

## Resources

- **Documentation**: https://blazor.radzen.com/
- **Get Started**: https://blazor.radzen.com/get-started
- **DataGrid Docs**: https://blazor.radzen.com/datagrid
- **Theme Builder**: https://blazor.radzen.com/theme-builder
- **GitHub**: https://github.com/radzenhq/radzen-blazor

## Remember

1. **ONLY Radzen components** - No exceptions
2. **Use RadzenDataGrid** for all tables
3. **Inject services** (NotificationService, DialogService)
4. **Use material3-dark theme** for consistent admin UI
5. **Server-side DataGrid** for large datasets
6. **Templates** for custom column rendering
7. **RadzenStack** for layouts instead of divs
8. **Validation** with Radzen validators in forms
