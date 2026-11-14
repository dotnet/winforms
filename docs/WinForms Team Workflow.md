# WinForms Team Workflow

This workflow applies to work items in the WinForms Runtime and Designer repositories.

## Workflow Overview

Todo → Ready → In Progress → Validation → In Review → Done

---

## Todo

### Description

The work item has been created but is not yet assigned for active work.

### Entry Criteria

- A new issue, task, bug, or feature is added to the project.
- The work item requires triage or prioritization.

### Exit Criteria

- An owner is assigned.
- The work item has enough information to begin planning or implementation.

---

## Ready

### Description

The work item is assigned and ready to be started.

### Entry Criteria

- An owner has been assigned.
- The scope and requirements are sufficiently clear.

### Exit Criteria

- The assignee begins investigation, design, implementation, or documentation work.

---

## In Progress

### Description

The assignee is actively investigating, designing, implementing, or documenting the work item.

### Entry Criteria

- Active work has started.

### Exit Criteria

- The implementation or other planned work is functionally complete.
- A pull request is available for applicable validation activities.

---

## Validation

### Description

The change is undergoing quality verification before FTE review.

Required validation activities include:

- Self-review by the assignee
- Internal peer review
- Appropriate Functional Validation
- Appropriate Regression Testing

Depending on the scope and risk of the change, additional verification activities may be required, such as Test Team validation.

### Entry Criteria

- The implementation is functionally complete.
- The pull request is available for validation activities.

### Exit Criteria

- All validation activities have been completed.
- Validation findings have been addressed.
- The pull request is ready for FTE review.
---

## In Review

### Description

The pull request has completed the applicable pre-FTE validation and is waiting for FTE review and approval.

### Entry Criteria

- The pull request is ready for FTE review.
- The `waiting-review` label is applied when applicable.
### Exit Criteria

- Required review feedback has been addressed.
- Required approval has been received.
- The pull request is ready to merge.

---

## Done

### Description

The planned work has been completed and accepted.

Any planned post-merge Test Team validation may continue as a separate validation activity and may result in follow-up work if issues are found.

### Entry Criteria

- The pull request has been merged.
- Or the work item has otherwise met its agreed completion condition.

### Exit Criteria

- None.

This is the final workflow state.