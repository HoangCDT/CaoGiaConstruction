# PLAN-about-page-refresh

## Goal Description
Create a new "About Us" page based on the provided modern UI design. The new page will be data-driven, requiring updates to the Database and Service layer to support dynamic content for Team Members, Core Values, Milestones, and Partners. The existing About page should be preserved.

## User Review Required
> [!IMPORTANT]
> **Database Changes**: This plan requires creating a new `TeamMember` entity and modifying the `TimeLine` entity. A migration will be needed.

> [!NOTE]
> **Dynamic vs Static**:
> - **Team Members**: Will be fully dynamic (new Entity).
> - **Milestones**: Will use the existing `TimeLine` entity (updated with `Title`).
> - **Core Values & Partners**: Proposed to use the existing `Slide` system with new Categories (e.g., "CoreValues", "Partners"). This avoids creating too many small entities.
> - **Founder's Quote**: Will be managed either via a specific `TeamMember` entry (Founder role) or the `About` setting.

## Proposed Changes

### Database Layer
#### [NEW] [TeamMember.cs](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/Context/Entities/Team/TeamMember.cs)
Create a new entity to manage team members.
```csharp
public class TeamMember : EntityBase
{
    public string FullName { get; set; }
    public string Position { get; set; } // e.g., "CEO & Founder"
    public string Avatar { get; set; }
    public string Quote { get; set; } // For the Founder section or bio
    public int SortOrder { get; set; }
    public bool IsFounder { get; set; } // To distinguish the Founder section
}
```

#### [MODIFY] [TimeLine.cs](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/Context/Entities/TimeLine/TimeLine.cs)
Add a `Title` property to match the UI "Khởi đầu khát vọng" vs Year "2014".
```diff
+ public string Title { get; set; }
```

### Service Layer
#### [NEW] [AboutViewModel.cs](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/ViewModels/AboutViewModel.cs)
Create a ViewModel to aggregate data for the View.
```csharp
public class AboutViewModel
{
    public About AboutSettings { get; set; } // Existing generic content
    public TeamMember Founder { get; set; }
    public List<TeamMember> TeamMembers { get; set; }
    public List<TimeLine> Milestones { get; set; }
    public List<Slide> CoreValues { get; set; }
    public List<Slide> Partners { get; set; }
}
```

#### [NEW] [TeamMemberService.cs](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/Services/Team/TeamMemberService.cs)
Service to fetch TeamMembers (GetAll, GetFounder).

#### [MODIFY] [AboutController.cs](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/Controllers/AboutController.cs)
Add a new action `AboutPageV2()` (mapped to `/ve-chung-toi-moi` for testing) that constructs the `AboutViewModel`.

### Frontend Layer
#### [NEW] [IndexNew.cshtml](file:///Users/caodinhtrihoang/SkypeCao/03.HQsolutions/CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient/Views/About/IndexNew.cshtml)
Implement the provided HTML using Razor syntax to bind data from `AboutViewModel`.

> [!NOTE]
> **Layout Strategy**: The new design uses **Tailwind CSS**, while the existing project uses **Bootstrap** and **jQuery** (`main.js`, `my-script.js`).
> - **Isolation**: This View will be a **standalone page** (`Layout = null`) to avoid style/script conflicts.
> - **Scripts**: We will **NOT** include `main.js` or `my-script.js`.
> - **Interactivity**: Use minimal **Vanilla JS** for the Mobile Menu toggle and any simple interactions (e.g., scroll to section).

- **Hero Section**:
  - Image: `Model.AboutSettings.LogoBottom` (or new field if needed).
  - Title/Subtitle: Static or `Model.AboutSettings.AboutUs`.
- **Founder Section**:
  - Image: `Model.Founder.Avatar`
  - Name: `Model.Founder.FullName`
  - Position: `Model.Founder.Position`
  - Quote: `Model.Founder.Quote`
- **Story Section**:
  - Content: `Html.Raw(Model.AboutSettings.Content)`
- **Core Values Section**:
  - Loop `Model.CoreValues` (Type: `Slide`).
  - Icon: Map `Slide.Avatar` or `Slide.Image` to Material Symbol name (requires convention, e.g., "visibility").
- **Milestones Section**:
  - Loop `Model.Milestones` (Type: `TimeLine`).
  - Display `Year` (from `EventDate`), `Title`, `Description`.
- **Team Section**:
  - Loop `Model.TeamMembers` (excluding Founder).
- **Partners Section**:
  - Loop `Model.Partners` (Type: `Slide`).
  - Logos: `Slide.Avatar`.

## Verification Plan

### Automated Tests
- None specified (UI heavy task).

### Manual Verification
1.  **Database Migration**: Run migration `AddTeamMemberAndVerifyTimeLine` and check DB schema.
2.  **Admin Entry**: Manually insert test data into `TeamMember`, `TimeLine`, and `Slide` (Categories: CoreValue, Partner).
3.  **UI Review**:
    - Access `/ve-chung-toi-moi`.
    - Verify responsiveness on Mobile/Desktop.
    - Verify all dynamic data points display correctly.
    - Verify "Keep Existing Page": Access `/ve-chung-toi` and ensure it still works as before.
