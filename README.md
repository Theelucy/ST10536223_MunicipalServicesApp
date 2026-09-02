# Municipal Services Application — Part 1 (Report Issues)

A C# .NET Framework (4.8) Windows Forms application that lets residents of a
South African municipality report service issues (potholes, water leaks,
sanitation problems, etc.). This is Part 1 of the PoE: only the **Report
Issues** task is implemented. **Local Events and Announcements** and
**Service Request Status** are visible on the main menu but disabled, as
required by the brief — they will be built in Part 2.

## Requirements to run this project

- Windows 10/11
- Visual Studio 2019 or later (Community edition is fine), with the
  **.NET desktop development** workload installed
- .NET Framework 4.8 Developer Pack (Visual Studio will prompt you to
  install this automatically if it's missing)

## How to compile and run

1. Copy the `MunicipalServicesApp` folder (the one containing
   `MunicipalServicesApp.sln`) to your Windows machine.
2. Double-click `MunicipalServicesApp.sln` to open it in Visual Studio.
3. Let Visual Studio restore/settle (there are no NuGet packages to
   restore — the project only uses built-in .NET Framework references).
4. Press **F5** (or **Ctrl+F5** to run without debugging), or use
   **Build → Build Solution** followed by running the produced
   `MunicipalServicesApp.exe` in `MunicipalServicesApp\bin\Debug\`.
5. The Main Menu form appears first.

## How to use the application

1. The app opens on a **Home dashboard**: live stat cards (Total Issues
   Reported / Pending / Resolved), a **Quick Actions** section, and a
   **Recently Reported Issues** table showing your last 5 reports. Click
   **Report Issues** (the other two action cards are intentionally disabled
   for this part of the project — they're marked "Coming in Part 2").
2. On the Report Issues screen, fill in:
   - **Location** — free text describing where the issue is.
   - **Category** — choose from the dropdown (Sanitation, Roads and
     Potholes, Water and Sewage, Electricity, Waste Management, Parks and
     Public Spaces, Other).
   - **Description** — details of the issue, typed into the rich text box.
   - **Attachments** (optional) — click **Attach Image / Document** to open
     a file picker and attach one or more images/documents related to the
     issue.
3. As you fill in the required fields, watch the **progress bar** under
   "Your Report Progress" — this is the user engagement feature. It fills
   up as you complete each required field, so you always know how close
   you are to being able to submit.
4. On the right, **"My Reported Issues"** shows every issue reported this
   session in a live table, updating as you go — this is your on-screen
   dashboard of reports.
5. Click **Submit Report**. You'll get a confirmation message with a
   reference number, the input fields will lock, and the **status label**
   (and the matching row in the reports table) will start updating
   automatically every few seconds — moving through *Submitted → Under
   Review → In Progress → Resolved* — to simulate the municipality
   processing your report and give you real-time feedback.
6. Click **Report Another Issue** to reset the form and capture a new
   report, or **Back to Main Menu** to return to the dashboard — where the
   stats and recent-issues table will reflect what you just submitted.

## Project structure

```
MunicipalServicesApp.sln
MunicipalServicesApp/
  MunicipalServicesApp.csproj
  App.config
  Program.cs                       Application entry point
  AppTheme.cs                      Shared colour palette / fonts for a consistent look
  Models/
    Issue.cs                       Issue model + in-memory List<Issue> repository + status helpers
  Forms/
    MainMenuForm.cs / .Designer.cs      Home dashboard (stats, quick actions, recent issues)
    ReportIssueForm.cs / .Designer.cs   Report Issues screen + live "My Reported Issues" grid
```

## Design notes

- **Data structure:** reported issues are stored in a `List<Issue>` inside
  a static `IssueRepository` class. A `List<T>` was chosen because the
  number of reports isn't known ahead of time and the app only needs to
  add and iterate over them — this also keeps the data available in
  memory for both the dashboard stats and the "Service Request Status"
  screen to be built in Part 2.
- **Home dashboard:** the main menu was redesigned as a dashboard rather
  than a plain list of buttons. It shows three live stat cards (Total /
  Pending / Resolved), a "Quick Actions" row for the three municipal
  tasks, and a "Recently Reported Issues" table of the last 5 reports.
  This refreshes every time the window is (re)activated, so returning
  from Report Issues always shows current numbers.
- **"My Reported Issues" panel:** the Report Issues screen itself is
  split into the report form (left) and a live table of everything
  reported this session (right). This is a preview tied directly to the
  Report Issues feature — not the full "Service Request Status" module,
  which stays disabled on the dashboard as the brief requires.
- **User engagement strategy — real-time feedback / progress tracking:**
  implemented as (a) a live completion progress bar while filling in the
  form, and (b) a simulated status timeline after submission, reflected
  in both the status label and the reports table. This was the strategy
  selected and justified in the Task 1 research document, and it directly
  answers the brief's requirement that residents "receive updates on the
  status of their service requests."
- **Consistency:** a single colour palette and font set (`AppTheme.cs`) is
  reused across both forms — dark header, green primary actions, blue
  secondary actions, and colour-coded status text everywhere a status
  appears.
- **Feedback:** `MessageBox` dialogs confirm successful submission and flag
  missing required fields.
- **Responsiveness:** the report form's field panel uses `AutoScroll` so it
  remains usable at smaller window heights; the dashboard's recent-issues
  table and the reports grid both resize with the window.

## Known limitations (by design, for this part of the PoE)

- Data is stored in memory only (no database/file persistence yet) —
  closing the app clears reported issues. Persistence can be added in a
  later part if required.
- "Local Events and Announcements" and "Service Request Status" are
  placeholders, disabled on the main menu per the brief.
