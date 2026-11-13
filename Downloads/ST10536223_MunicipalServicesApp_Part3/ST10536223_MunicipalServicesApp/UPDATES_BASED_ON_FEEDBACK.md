# Updates Made Based on Lecturer Feedback

> **TO COMPLETE BEFORE SUBMITTING:** the rows below marked `[PASTE ...]` need your lecturer's actual
> comments from Parts 1 and 2. Replace every `[PASTE ...]` and `[DESCRIBE ...]`, then delete this
> notice. Only list updates you really made.

## Feedback received and the update made

| # | Part | Lecturer feedback (quote or summarise) | Update made | Where in the code |
|---|------|----------------------------------------|-------------|-------------------|
| 1 | Part 1 | [PASTE feedback comment 1] | [DESCRIBE what you changed] | [file / form name] |
| 2 | Part 1 | [PASTE feedback comment 2] | [DESCRIBE what you changed] | [file / form name] |
| 3 | Part 2 | [PASTE feedback comment 3] | [DESCRIBE what you changed] | [file / form name] |
| 4 | Part 2 | [PASTE feedback comment 4] | [DESCRIBE what you changed] | [file / form name] |

*Add or remove rows as needed. For each update, say what the feedback was, what you changed, and where
the marker can see it.*

---

## Other improvements made while building Part 3

These are improvements made to the existing Part 1 and Part 2 code while integrating Part 3. They are
**not** lecturer feedback, so keep them in this separate section if you keep them.

| Change | Where |
|---|---|
| The *Local Events and Announcements* and *Service Request Status* menu cards were enabled in code but still drawn in the grey "disabled" style. They are now coloured like the *Report Issues* card and labelled "Available now". | `Forms/MainMenuForm.Designer.cs`, `Forms/MainMenuForm.cs` |
| The README described only Part 1 under a Part 2 heading. It now documents all three parts, including a data-structure explanation with examples for Part 3. | `README.md` |
| The Service Request Status page was replaced with the full Part 3 version (six tabs). It reads the same `IssueRepository` as the rest of the application, so reports from Part 1 appear in it. | `Forms/ServiceRequestStatusForm*.cs` |
